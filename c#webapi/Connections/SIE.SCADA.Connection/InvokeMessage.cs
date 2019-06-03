using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SIE.SCADA.Connection
{
    [Serializable]
    [DataContract(Namespace = "SieSCADAWebAPI")]
    public class InvokeMessage
    {
        [DataMember]
        public MethodType MethodType { get; set; } = MethodType.Alarm;

        /// <summary>
        /// 方法名称
        /// </summary>
        [DataMember]
        public string MethodName { get; set; } = "";

        /// <summary>
        /// 参数对象
        /// </summary>
        [DataMember]
        public dynamic[] Paras { get; set; } = new dynamic[] { };

        /// <summary>
        /// 操作是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; } = true;

        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string ErrorMsg { get; set; } = "";

        /// <summary>
        /// 返回的序列化对象
        /// </summary>
        [DataMember]
        public string ReturnBody { get; set; } = "";

        public InvokeMessage()
        {

        }

        public InvokeMessage(MethodType methodType, string methodName)
        {
            MethodType = methodType;
            MethodName = methodName;
        }
    }

    [Serializable]
    public enum MethodType
    {
        IO = 0x01,

        Tag = 0x02,

        Alarm = 0x04,
    }
}
