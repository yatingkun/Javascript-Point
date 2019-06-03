using System;
using System.Collections.Generic;
using System.Text;

namespace SIE.SCADA.WebApi
{
    public class WebApiConfig
    {
        public string IP { get; set; }

        public int Port { get; set; }

        public DataFormatter DataFormatter { get; set; }

        /// <summary>
        /// WEBAPI连接超时时间，默认3s
        /// </summary>
        public TimeSpan TimeSpan { get; set; } = new TimeSpan(0, 0, 3);

    }
    public enum DataFormatter
    {
        Xml,
        Json,
    }
}
