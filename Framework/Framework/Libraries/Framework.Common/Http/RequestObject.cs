using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace Framework.Common.Http
{
    /// <summary>
    /// web请求抽象类
    /// </summary>
    public abstract class RequestObject<TResponse> where TResponse : ResponseObject
    {
        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public abstract string RequestUrl { get; }

        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual HttpMethod Method
        {
            get { return HttpMethod.Post; }
        }

        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
        }

        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual bool IsStreamResponse { get { return false; } }

        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual bool IsByteRequest { get { return false; } }
        /// <summary>
        /// 生成请求数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GenerateRequestData(RequestStringDataTypes type)
        {
            string data = string.Empty;
            switch (type)
            {
                case RequestStringDataTypes.Xml:
                    data =Utility.ToXml(this);
                    break;
                default:
                    data = JsonConvert.SerializeObject(this);
                    break;
            }
            return data;
        }

        public virtual byte[] ToByte(RequestStringDataTypes type)
        {
            return Encoding.UTF8.GetBytes(
                GenerateRequestData(type));
        }
    }
}
