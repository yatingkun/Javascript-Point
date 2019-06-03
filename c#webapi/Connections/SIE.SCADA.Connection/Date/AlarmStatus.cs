using System;
using System.Collections.Generic;
using System.Text;

namespace SIE.SCADA.Connection.Date
{
    /// <summary>
    /// 报警状态
    /// </summary>
    [Serializable]
    [Flags]
    public enum AlarmStatus
    {
        /// <summary>
        /// 报警
        /// </summary>
        Alarm = 0x01,

        /// <summary>
        /// 应答
        /// </summary>
        Acked = 0x02,

        /// <summary>
        /// 报警应答
        /// </summary>
        AlarmAcked = 0x03,

        /// <summary>
        /// 恢复
        /// </summary>
        Recovery = 0x04,

        /// <summary>
        /// 报警恢复
        /// </summary>
        AlarmRecovery = 0x05,

        /// <summary>
        /// 报警应答恢复
        /// </summary>
        AlarmAckedRecovery = 0x07,

        /// <summary>
        /// 从列表中移除报警
        /// </summary>
        Remove = 0x08,
    }
}
