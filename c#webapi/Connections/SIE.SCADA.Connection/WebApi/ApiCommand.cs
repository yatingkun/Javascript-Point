using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SIE.SCADA.Connection;
using SIE.SCADA.Connection.Date;

namespace SIE.SCADA.WebApi
{
    public class ApiCommand 
    {
        private string _error;
        HttpClient _client;
        HttpResponseMessage _response;
        public string DataFormatter { get; set; }

        public ApiCommand(HttpClient webApiClient,string dataFormatter)
        {
            _client = webApiClient;
            DataFormatter = dataFormatter;
        }
        /// <summary>
        /// 判断是否连接成功
        /// </summary>
        /// <returns></returns>
        public bool Connection()
        {
            try
            {
                 _response = _client.GetAsync("/DataService/webapi/IsConnection").Result;
            }
            catch(Exception ex)
            {
                if (ex!= null)
                {
                    return false;
                }
            }
            if (_response.IsSuccessStatusCode)
            {
                var resuot = _response.Content.ReadAsStringAsync();
                return true;
            }
            else
            {
                return false;
            }           
        }

        /// <summary>
        /// 根据Tag名读值
        /// </summary>
        /// <param name="tagName">Tag名</param>
        /// <returns></returns>
        public ValueItem Read(string tagName)
        {
            string result = "";
            ValueItem valueItem = new ValueItem();
            if (tagName == null) return null;
            try
            {
                _response = _client.GetAsync(string.Format("/DataService/webapi/ReadTagValue?path={0}", tagName)).Result;
                if (_response.IsSuccessStatusCode)
                {
                    result = _response.Content.ReadAsStringAsync().Result;
                }

                if (DataFormatter == "Xml")
                {
                    using (StringReader sr = new StringReader(result))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValueItem), "SieSCADAWebAPI");  //去掉命名空间
                        valueItem = xmlSerializer.Deserialize(sr) as ValueItem;
                    }
                }
                if (DataFormatter == "Json")
                {
                    valueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ValueItem>(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valueItem;
        }

        /// <summary>
        /// 批量读
        /// </summary>
        /// <param name="tagNames">Tag名的集合</param>
        /// <returns></returns>
        public List<ValueItem> Read(List<string> tagNames)
        {
            string result = "";
            StringBuilder stringBuilder = new StringBuilder();
            string tagNamesString="";
            List<ValueItem> valueItems = new List<ValueItem>();
            if (tagNames == null) return null;
            #region  拼接字符串
            foreach (var item in tagNames)   //拼接字符串
            {
                stringBuilder.Append(item);
                stringBuilder.Append(",");
            }
            if (stringBuilder.ToString().LastIndexOf(',') == stringBuilder.ToString().Length - 1)  //去掉最后一个逗号
            { 
               tagNamesString= stringBuilder.ToString().Remove(stringBuilder.ToString().Length - 1, 1);
            }
            #endregion
            try  //可能传的格式不匹配
            {
                _response = _client.GetAsync(string.Format("/DataService/webapi/ReadTagValues?paths={0}", tagNamesString)).Result;
                if (_response.IsSuccessStatusCode)  //连接成功
                {
                    result = _response.Content.ReadAsStringAsync().Result;
                }

                if (DataFormatter == "Xml")
                {
                    using (StringReader sr = new StringReader(result))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ValueItem>), "SieSCADAWebAPI");
                        valueItems = xmlSerializer.Deserialize(sr) as List<ValueItem>;
                    }
                }
                if (DataFormatter == "Json")
                {
                    valueItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValueItem>>(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valueItems;

        }

        /// <summary>
        /// 单个写值
        /// </summary>
        /// <param name="item">ValueItem对象</param>
        /// <returns></returns>
        public bool Write(ValueItem item)
        {
            bool result = true;
            string resultResponse = "";
            if (item == null) { return  false; }
            try
            {
                if (DataFormatter == "Xml")
                {
                    string xmlString = TurnToXml<ValueItem>(typeof(ValueItem), item);
                     HttpContent httpContent = new StringContent(xmlString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    var response = _client.PutAsync("/DataService/webapi/WriteTagValue", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        string newXml = System.Text.RegularExpressions.Regex.Replace(resultResponse, @"(xmlns:?[^=]*=[""][^""]*[""])", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                            System.Text.RegularExpressions.RegexOptions.Multiline);
                        using (StringReader sr = new StringReader(newXml))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(string));  //去掉命名空间
                            resultResponse = xmlSerializer.Deserialize(sr).ToString();
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                if (DataFormatter == "Json")
                {
                    string jsonString = JsonSerialize(item);
                    //设置Json格式，把参数以Json格式上传
                    HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = _client.PostAsync("/DataService/webapi/WriteTagValue", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        resultResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resultResponse);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            string[] sArrays = resultResponse.Split(';');
            foreach(var sArray in sArrays)
            {
                if (sArray.ToLower() == "false") result = false;
            }
            return result;
        }

        /// <summary>
        /// 批量写
        /// </summary>
        /// <param name="items">ValueItem对象集合</param>
        /// <returns></returns>
        public bool Write(List<ValueItem> items)
        {
            
            bool result = true;
            string resultResponse;
            List<String> resultString = new List<string>();
            if (items == null) { return false; }
            if (DataFormatter == "Xml")
            {
                string xmlString = TurnToXml<List<ValueItem>>(typeof(List<ValueItem>), items);
                HttpContent httpContent = new StringContent(xmlString, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                var response = _client.PutAsync("/DataService/webapi/WriteTagValues", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    resultResponse = response.Content.ReadAsStringAsync().Result;
                    var reg = Regex.Matches(resultResponse, "(?<=<string>)(.+?)(?=</string>)");
                    foreach (var value in reg)
                    {
                        resultString.Add(value.ToString());
                    }
                }
             }
            if (DataFormatter == "Json")
            {
                string jsonString = JsonSerialize(items);
                //设置Json格式，把参数以Json格式上传
                HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = _client.PostAsync("/DataService/webapi/WriteTagValues", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    resultResponse = response.Content.ReadAsStringAsync().Result;
                    resultString = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(resultResponse);
                }
            }
            foreach (var itemString in resultString)
            {
                string[] sArrays = itemString.Split(';');
                foreach (var sArray in sArrays)
                {
                    if (sArray.ToLower() == "false") result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 向设备变量写值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteIOTag(ValueItem value)
        {
            bool result = true;
            string resultResponse = "";
            if (value == null) { return false; }
            try
            {
                if (DataFormatter == "Xml")
                {
                    string xmlString = TurnToXml<ValueItem>(typeof(ValueItem), value);
                    HttpContent httpContent = new StringContent(xmlString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    var response = _client.PutAsync("/DataService/webapi/WriteIOTag", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        string newXml = System.Text.RegularExpressions.Regex.Replace(resultResponse, @"(xmlns:?[^=]*=[""][^""]*[""])", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                                                                                                                                           System.Text.RegularExpressions.RegexOptions.Multiline);
                        using (StringReader sr = new StringReader(newXml))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(string));  //去掉命名空间
                            resultResponse = xmlSerializer.Deserialize(sr).ToString();
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                if (DataFormatter == "Json")
                {
                    string jsonString = JsonSerialize(value);
                    //设置Json格式，把参数以Json格式上传
                    HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = _client.PostAsync("/DataService/webapi/WriteIOTag", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        resultResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(resultResponse);
                    }
                    else
                    {
                        return false;
                    }
                }
 
            }
            catch (Exception e)
            {
                throw e;
            }
            string[] sArrays = resultResponse.Split(';');
            foreach (var sArray in sArrays)
            {
                if (sArray.ToLower() == "false") result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取指定对象的属性值
        /// </summary>
        /// <param name="path"对象路径></param>
        /// <param name="propertyName">IoTags[0].Name;IoTags[*].Name</param>
        /// <returns></returns>
        public ValueItem ReadProperty(string path, string propertyName)
        {
            string result = "";
            ValueItem valueItem = new ValueItem();
            if (path == null) return null;
            try
            {
                _response = _client.GetAsync(string.Format("/DataService/webapi/ReadProperty?path={0}&propertyName={1}", path, propertyName)).Result;
                if (_response.IsSuccessStatusCode)
                {
                    result = _response.Content.ReadAsStringAsync().Result;
                }

                if (DataFormatter == "Xml")
                {
                    using (StringReader sr = new StringReader(result))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ValueItem), "SieSCADAWebAPI");  //去掉命名空间
                        valueItem = xmlSerializer.Deserialize(sr) as ValueItem;
                    }
                }
                if (DataFormatter == "Json")
                {
                    valueItem = Newtonsoft.Json.JsonConvert.DeserializeObject<ValueItem>(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return valueItem;
        }

        /// <summary>
        /// 批量获取指定对象的属性值
        /// </summary>
        /// <param name="values">格式为：对象路径#对象属性，例如["IO#Devices[*].Name","IO.OPCUA1#IoTags[*].Name"]</param>
        /// <returns></returns>
        public List<ValueItem> ReadProperties(string[] values)
        {
            bool result = true;
            string resultResponse;
            List<ValueItem> resultString = new List<ValueItem>();
            if (values == null) { return null; }
            if (DataFormatter == "Xml")
            {
                string xmlString = TurnToXml<string[]>(typeof(string[]), values);
                HttpContent httpContent = new StringContent(xmlString, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                var response = _client.PutAsync("/DataService/webapi/ReadProperties", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    resultResponse = response.Content.ReadAsStringAsync().Result;
                    var reg = Regex.Matches(resultResponse, "(?<=<string>)(.+?)(?=</string>)");
                    foreach (var value in reg)
                    {
                        if (value is ValueItem item)
                        {
                            resultString.Add(item);
                        }                         
                    }
                }
            }
            if (DataFormatter == "Json")
            {
                string jsonString = JsonSerialize(values);
                //设置Json格式，把参数以Json格式上传
                HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = _client.PostAsync("/DataService/webapi/ReadProperties", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    resultResponse = response.Content.ReadAsStringAsync().Result;
                    resultString =  Newtonsoft.Json.JsonConvert.DeserializeObject<List<ValueItem>>(resultResponse);
                }
            }
            return resultString;
        }

        /// <summary>
        /// 订阅报警记录，返回报警ID，返回-1表示调用失败
        /// </summary>
        /// <returns></returns>
        public long SubscribeAlarm()
        {
            try
            {

                var msg = new InvokeMessage(MethodType.Alarm, "SubscribeAlarm");
                var res = Invoke(msg);
                if (!res.IsSuccess) return -1;
                return Convert.ToInt64(res.ReturnBody);
            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// 通过id取消订阅报警
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoverSubscribeAlarm(long id)
        {
            try
            {

                var msg = new InvokeMessage(MethodType.Alarm, "RemoveSubcribe");
                msg.Paras = new dynamic[] { id };
                var res = Invoke(msg);
                if (!res.IsSuccess) return false;
                return Convert.ToBoolean(res.ReturnBody);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 通过订阅ID获取报警记录集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<AlarmRecord> GetAlarms(long id)
        {
            try
            {

                var msg = new InvokeMessage(MethodType.Alarm, "GetAlarmRecords");
                msg.Paras = new dynamic[] { id };
                var res = Invoke(msg);
                if (!res.IsSuccess) return null;
                return JsonDeserialize<List<AlarmRecord>>(res.ReturnBody);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 通用方法
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public InvokeMessage Invoke(InvokeMessage msg)
        {
            string resultResponse = "";
            if (msg == null) { return null; }
            try
            {
                if (DataFormatter == "Xml")
                {
                    string xmlString = TurnToXml(typeof(InvokeMessage), msg);
                    HttpContent httpContent = new StringContent(xmlString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    var response = _client.PutAsync("/DataService/webapi/Invoke", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        string newXml = Regex.Replace(resultResponse, @"(xmlns:?[^=]*=[""][^""]*[""])", "", RegexOptions.IgnoreCase |
                            RegexOptions.Multiline);
                        using (StringReader sr = new StringReader(newXml))
                        {
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InvokeMessage));  //去掉命名空间
                            return (InvokeMessage)xmlSerializer.Deserialize(sr);
                        }

                    }
                    else
                    {
                        msg.IsSuccess = false;
                        msg.ErrorMsg = "通讯错误";
                        return msg;
                    }
                }
                if (DataFormatter == "Json")
                {
                    string jsonString = JsonSerialize(msg);
                    //设置Json格式，把参数以Json格式上传
                    HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = _client.PostAsync("/DataService/webapi/Invoke", httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        resultResponse = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<InvokeMessage>(resultResponse);
                    }
                    else
                    {
                        msg.IsSuccess = false;
                        msg.ErrorMsg = "通讯错误";
                        return msg;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 把数据转化成Xml格式
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="type">数据类型</param>
        /// <param name="item"> 实体</param>
        /// <returns></returns>
        public string TurnToXml<TItem>(Type type, TItem item)
        {
            string str;
            MemoryStream stream = new MemoryStream();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(type, "SieSCADAWebAPI");
            serializer.Serialize(stream, item, ns);
            stream.Position = 0;
            using (StreamReader sr = new StreamReader(stream))
            {
                str = sr.ReadToEnd();
            }
            stream.Dispose();
            return str;
        }

        /// <summary>
        /// 序列化对象为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  string JsonSerialize(object obj)
        {
            var timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            return JsonConvert.SerializeObject(obj, Formatting.Indented, timeFormat);
        }

        /// <summary>
        /// 将字符串反序列化到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public  T JsonDeserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

    }
   
}
