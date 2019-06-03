using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SIE.SCADA.Connection.Date
{
    [Serializable]
    [DataContract(Namespace = "SieSCADAWebAPI")]
    public class AlarmRecord
    {
        [DataMember]
        public long AlarmID { get; set; }

        [DataMember]
        public string AlarmType { get; set; }

        [DataMember]
        public AlarmLevel AlarmLevel { get; set; } = AlarmLevel.Info;

        [DataMember]
        public object AlarmValue { get; set; }

        [DataMember]
        public object LimitValue { get; set; }

        [DataMember]
        public object RecoveryValue { get; set; }

        [DataMember]
        public AlarmStatus AlarmStatus { get; set; }

        [DataMember]
        public string AlarmSource { get; set; }

        [DataMember]
        public string AlarmContent { get; set; }

        [DataMember]
        public string AlarmReason { get; set; }

        [DataMember]
        public DateTime TriggerTime { get; set; }

        [DataMember]
        public DateTime AckedTime { get; set; }

        [DataMember]
        public DateTime RecoveryTime { get; set; }

        [DataMember]
        public string AckReason { get; set; }

        [DataMember]
        public string AckSource { get; set; }

        [DataMember]
        public string ImproveStrategy { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 反序列化时使用
        /// </summary>
        public AlarmRecord() { }      

        /// <summary>
        /// 通过属性名称获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(AlarmID):
                    return AlarmID;
                case nameof(AlarmType):
                    return AlarmType;
                case nameof(AlarmLevel):
                    return AlarmLevel;
                case nameof(AlarmValue):
                    return AlarmValue;
                case nameof(LimitValue):
                    return LimitValue;
                case nameof(RecoveryValue):
                    return RecoveryValue;
                case nameof(AlarmStatus):
                    return AlarmStatus;
                case nameof(AlarmSource):
                    return AlarmSource;
                case nameof(AlarmContent):
                    return AlarmContent;
                case nameof(AlarmReason):
                    return AlarmReason;
                case nameof(TriggerTime):
                    return TriggerTime;
                case nameof(RecoveryTime):
                    return RecoveryTime;
                case nameof(AckedTime):
                    return AckedTime;
                case nameof(AckReason):
                    return AckReason;
                case nameof(AckSource):
                    return AckSource;
                case nameof(ImproveStrategy):
                    return ImproveStrategy;
                case nameof(Remark):
                    return Remark;
                case nameof(Description):
                    return Description;
                default:
                    return null;
            }
        }

        public bool SetPropertyValue(string propertyName, object value)
        {
            try
            {
                switch (propertyName)
                {
                    case nameof(AlarmID):
                        AlarmID = Convert.ToInt64(value);
                        return true;
                    case nameof(AlarmType):
                        AlarmType = value.ToString();
                        return true;
                    case nameof(AlarmLevel):
                        AlarmLevel = (AlarmLevel)Enum.Parse(typeof(AlarmLevel), value.ToString());
                        return true;
                    case nameof(AlarmValue):
                        AlarmValue = value;
                        return true;
                    case nameof(LimitValue):
                        LimitValue = value;
                        return true;
                    case nameof(RecoveryValue):
                        RecoveryValue = value;
                        return true;
                    case nameof(AlarmStatus):
                        AlarmStatus = (AlarmStatus)Enum.Parse(typeof(AlarmStatus), value.ToString());
                        return true;
                    case nameof(AlarmSource):
                        AlarmSource = value.ToString();
                        return true;
                    case nameof(AlarmContent):
                        AlarmContent = value.ToString();
                        return true;
                    case nameof(AlarmReason):
                        AlarmReason = value.ToString();
                        return true;
                    case nameof(TriggerTime):
                        TriggerTime = Convert.ToDateTime(value);
                        return true;
                    case nameof(RecoveryTime):
                        RecoveryTime = Convert.ToDateTime(value);
                        return true;
                    case nameof(AckedTime):
                        AckedTime = Convert.ToDateTime(value);
                        return true;
                    case nameof(AckReason):
                        AckReason = value.ToString();
                        return true;
                    case nameof(AckSource):
                        AckSource = value.ToString();
                        return true;
                    case nameof(ImproveStrategy):
                        ImproveStrategy = value.ToString();
                        return true;
                    case nameof(Remark):
                        Remark = value.ToString();
                        return true;
                    case nameof(Description):
                        Description = value.ToString();
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return string.Format("AlarmID:{0}, AlarmType:{1}, AlarmStatus: {10}, AlarmLevel:{2} ,AlarmValue:{3}, LimitValue:{4}, RecoveryValue:{5}, AlarmContent:{6}, TriggerTime: {7}, AckedTime:{8}, RecoveryTime:{9}, Description:{11}",
                AlarmID,  AlarmType, AlarmLevel, AlarmValue, LimitValue, RecoveryValue, AlarmContent, TriggerTime, AckedTime, RecoveryTime, AlarmStatus, Description);
        }
    }
}
