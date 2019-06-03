using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SIE.SCADA.WebApi
{
    /// <summary>
    /// 数据值项
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "SieSCADAWebAPI")]
    public class ValueItem
    {
        /// <summary>
        /// 对象路径
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// 对象值
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// 质量戳
        /// </summary>
        [DataMember]
        public ushort QualityStamp { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValueItem()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">路径</param>
        /// <param name="value">值</param>
        public ValueItem(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public ValueItem(string key, object value, ushort qualityStamp, DateTime timeStamp)
        {
            Key = key;
            Value = value;
            TimeStamp = timeStamp;
            QualityStamp = qualityStamp;

        }
    }
}
