using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATools.Socket
{
    /// <summary>
    /// 封包
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// 封包类型
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
    }
}
