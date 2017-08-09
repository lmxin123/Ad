using System.IO;
using Newtonsoft.Json;

namespace Framework.Common.Http
{
    /// <summary>
    /// web请求返回抽象类
    /// </summary>
    public abstract class ResponseObject
    {
        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public Stream Stream { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual string Body { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual int Err_Code { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        [UrlParamIgnore]
        public virtual string Err_Msg { get; set; }

        public virtual TResponse GetResponseData<TResponse>(RequestStringDataTypes dType) where TResponse : ResponseObject, new()
        {
            TResponse resp = new TResponse();
            resp.Stream = Stream;
            resp.Body = Body;
            switch (dType)
            {
                case RequestStringDataTypes.Json:
                    resp = JsonConvert.DeserializeObject<TResponse>(Body);
                    break;
                case RequestStringDataTypes.Xml:
                    resp = Utility.FromXml<TResponse>(stringBody: Body);
                    break;
                default:
                    break;
            }

            return resp;
        }
    }
}
