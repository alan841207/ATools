using HPSocket;
using HPSocket.Tcp;
using HPSocket.Thread;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ThreadPool = HPSocket.Thread.ThreadPool;
using Timer = System.Timers.Timer;


namespace ATools.Socket
{
    /// <summary>
    /// SOCKET TCP Helper
    /// </summary>
    public class TcpServerHelper
    {

        private readonly ITcpServer _server = new TcpServer();

        /// <summary>
        /// 接受的信息
        /// </summary>
        public string Msg { get; set; }


        ///// <summary>
        /// 最大封包长度
        /// </summary>
        private const int MaxPacketSize = 4096;

        /// <summary>
        /// 线程池
        /// </summary>
        private readonly ThreadPool _threadPool = new ThreadPool();

        /// <summary>
        /// 线程池回调函数
        /// </summary>
        private TaskProcEx _taskTaskProc;



        /// <summary>
        /// 构造函数
        /// </summary>
        public TcpServerHelper(string ip= "0.0.0.0", ushort port=5555)
        {
            // 演示设置server属性

            // 缓冲区大小应根据实际业务设置, 并发数和包大小都是考虑条件
            // 都是小包的情况下4K合适, 刚好是一个内存分页(在非托管内存, 相关知识查百度)
            // 大包比较多的情况下, 应根据并发数设置比较大但是又不会爆内存的值
            _server.SocketBufferSize = 4096; // 4K

            // server绑定地址和端口
            _server.Address = ip;
            _server.Port = port;

            // 演示绑定事件
            _server.OnPrepareListen += OnPrepareListen;
            _server.OnAccept += OnAccept;
            _server.OnReceive += OnReceive;
            _server.OnClose += OnClose;
            _server.OnShutdown += OnShutdown;

        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }
        
        private HandleResult OnShutdown(IServer sender)
        {

            LogHelper.InfoLineWithHour($"OnShutdown({sender.Address}:{sender.Port})");

            return HandleResult.Ok;
        }

        private HandleResult OnClose(IServer sender, IntPtr connId, SocketOperation socketOperation, int errorCode)
        {
            var client = sender.GetExtra<ClientInfo>(connId);
            if (client != null)
            {
                sender.RemoveExtra(connId);
                client.PacketData.Clear();
                return HandleResult.Error;
            }

            return HandleResult.Ok;
        }

        private HandleResult OnReceive(IServer sender, IntPtr connId, byte[] data)
        {
            LogHelper.InfoLineWithHour($"OnReceive({connId}), data length: {data.Length}");
            var client = sender.GetExtra<ClientInfo>(connId);
            if (client == null)
            {
                return HandleResult.Error;
            }

            client.PacketData.AddRange(data);

            // 总长度小于包头
            if (client.PacketData.Count < sizeof(int))
            {
                return HandleResult.Ok;
            }

            HandleResult result;
            const int headerLength = sizeof(int);
            do
            {
                // 取头部字节得到包头
                var packetHeader = client.PacketData.GetRange(0, headerLength).ToArray();

                // 两端字节序要保持一致
                // 如果当前环境不是小端字节序
                if (!BitConverter.IsLittleEndian)
                {
                    // 翻转字节数组, 变为小端字节序
                    Array.Reverse(packetHeader);
                }

                // 得到包头指向的数据长度
                var dataLength = BitConverter.ToInt32(packetHeader, 0);

                // 完整的包长度(含包头和完整数据的大小)
                var fullLength = dataLength + headerLength;

                if (dataLength < 0 || fullLength > MaxPacketSize)
                {
                    result = HandleResult.Error;
                    break;
                }

                // 如果来的数据小于一个完整的包
                if (client.PacketData.Count < fullLength)
                {
                    // 下次数据到达处理
                    result = HandleResult.Ignore;
                    break;
                }

                // 是不是一个完整的包(包长 = 实际数据长度 + 包头长度)
                if (client.PacketData.Count == fullLength)
                {
                    // 得到完整包并处理
                    var fullData = client.PacketData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(sender, client, fullData);

                    // 清空缓存
                    client.PacketData.Clear();
                    break;
                }

                // 如果来的数据比一个完整的包长
                if (client.PacketData.Count > fullLength)
                {
                    // 先得到完整包并处理
                    var fullData = client.PacketData.GetRange(headerLength, dataLength).ToArray();
                    result = OnProcessFullPacket(sender, client, fullData);
                    if (result == HandleResult.Error)
                    {
                        break;
                    }
                    // 再移除已经读了的数据
                    client.PacketData.RemoveRange(0, fullLength);

                    // 剩余的数据下个循环处理
                }

            } while (true);


            return result;
        }


        private HandleResult OnProcessFullPacket(IServer sender, ClientInfo client, byte[] data)
        {
            // 这里来的都是完整的包, 但是这里不做耗时操作, 仅把数据放入队列
            var packet = JsonConvert.DeserializeObject<Packet>(Encoding.UTF8.GetString(data));
            this.Msg = Encoding.UTF8.GetString(data);
            var result = HandleResult.Ok;
            switch (packet.Type)
            {
                case PacketType.Echo: // 假如回显是一个非耗时操作, 在这处理
                    {
                        // 组织packet为一个json
                        var json = JsonConvert.SerializeObject(new Packet
                        {
                            Type = packet.Type,
                            Data = packet.Data,
                        });

                        // json转字节数组
                        var bytes = Encoding.UTF8.GetBytes(json);

                        // 先发包头
                        if (!SendPacketHeader(sender, client.ConnId, bytes.Length))
                        {
                            result = HandleResult.Error;
                            break;
                        }

                        // 再发实际数据
                        if (!sender.Send(client.ConnId, bytes, bytes.Length))
                        {
                            result = HandleResult.Error;
                        }

                        // 至此完成回显
                        break;
                    }
                case PacketType.Time: // 假如获取服务器时间是耗时操作, 将该操作放入队列
                    {
                        // 向线程池提交任务
                        if (!_threadPool.Submit(_taskTaskProc, new TaskInfo
                        {
                            Client = client,
                            Packet = packet,
                        }))
                        {
                            result = HandleResult.Error;
                        }

                        break;
                    }
                default:
                    result = HandleResult.Error;
                    break;
            }
            return result;
        }


        private bool SendPacketHeader(IServer sender, IntPtr connId, int dataLength)
        {
            // 包头转字节数组
            var packetHeaderBytes = BitConverter.GetBytes(dataLength);

            // 两端字节序要保持一致
            // 如果当前环境不是小端字节序
            if (!BitConverter.IsLittleEndian)
            {
                // 翻转字节数组, 变为小端字节序
                Array.Reverse(packetHeaderBytes);
            }

            return sender.Send(connId, packetHeaderBytes, packetHeaderBytes.Length);
        }


        private HandleResult OnAccept(IServer sender, IntPtr connId, IntPtr client)
        {

            // 获取客户端地址
            if (!sender.GetRemoteAddress(connId, out var ip, out var port))
            {
                return HandleResult.Error;
            }

            LogHelper.InfoLineWithHour($"OnAccept({connId}), ip: {ip}, port: {port}");

            // 设置附加数据, 用来做粘包处理
            sender.SetExtra(connId, new ClientInfo
            {
                ConnId = connId,
                PacketData = new List<byte>(),
            });

            return HandleResult.Ok;
        }

        private HandleResult OnPrepareListen(IServer sender, IntPtr listen)
        {
            LogHelper.InfoLineWithHour($"OnPrepareListen({sender.Address}:{sender.Port}), listen: {listen}, hp-socket version: {sender.Version}");
            return HandleResult.Ok;
        }
    }
}
