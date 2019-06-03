using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SIE.SCADA.WebApi
{
    public class WebApiClient : IDisposable
    {
        HttpClient _client;
        
        public bool IsStart { get; private set; }

        public WebApiConfig Config {get;set;}

        public ApiCommand Command { get; set; }
   
        public WebApiClient(WebApiConfig config)
        {
            Config = config;
        }

        public void Start()
        {
            if (IsStart)
            {
                return;
            }
            _client = new HttpClient();
            _client.Timeout = Config.TimeSpan;
            string url = "http://" + Config.IP + ":" + Config.Port;
            Uri uri = _client.BaseAddress = new Uri(url);
            Command = new ApiCommand(_client, Config.DataFormatter.ToString());
            IsStart = true;
        }

        public void Stop()
        {
            if (!IsStart)
            {
                return;
            }
            _client.Dispose();
            IsStart = false;
        }



        public void Dispose()
        {
            //throw new NotImplementedException();
            if (_client != null)
            {
                Stop();
            }
        }
    }
}
