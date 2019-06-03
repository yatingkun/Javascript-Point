using System;
using System.Collections.Generic;
using System.Text;

namespace SIE.SCADA.Connection.Date
{
    /// <summary>
    /// 报警级别
    /// </summary>
    [Serializable]
    public enum AlarmLevel
    {
        /// <summary>
        /// 提示
        /// </summary>
        Info = 0x01,

        /// <summary>
        /// 轻微
        /// </summary>
        Minor = 0x02,

        /// <summary>
        /// 一般
        /// </summary>
        Medium = 0x04,

        /// <summary>
        /// 重要
        /// </summary>
        Major = 0x08,

        /// <summary>
        /// 严重的
        /// </summary>
        Serious = 0x10,
    }
}
