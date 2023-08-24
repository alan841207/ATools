using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATools.Socket
{
    /// <summary>
    /// 封包类型
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// 回显
        /// </summary>
        Echo = 1,
        /// <summary>
        /// 时间
        /// </summary>
        Time
    }
}
