using Newtonsoft.Json;
using Framework.Common.Http;

namespace Framework.Weixin.Response.Pay
{
    /// <summary>
    /// 
    /// </summary>
    public class UnifiedOrderResponse: WeixinPayResponse
    {
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public virtual string appid { get; set; }
        /// <summary>
        /// 商户号	
        /// </summary>
        public virtual string mch_id { get; set; }

        public string device_info { get; set; }

        public string trade_type { get; set; }

        public string prepay_id { get; set; }

        public string code_url { get; set; }
    }
    /// <summary>
    /// 支付参数
    /// </summary>
    public class UnifiedOrderPayParamsResponse
    {
        public string appId { get; set; }

        public string nonceStr { get; set; }

        public string package { get; set; }

        public string timeStamp { get; set; }

        public string signType { get; set; }

        [XmlIgnore]
        [UrlParamIgnore]
        public string paySign { get; set; }

        //额外参数
        [XmlIgnore]
        [UrlParamIgnore]
        public string OutTradeNo { get; set; }
        [XmlIgnore]
        [UrlParamIgnore]
        public string OpenId { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        [UrlParamIgnore]
        public string returnCode { get; set; }
        [JsonIgnore]
        [XmlIgnore]
        [UrlParamIgnore]
        public string returnMsg { get; set; }
    }
}