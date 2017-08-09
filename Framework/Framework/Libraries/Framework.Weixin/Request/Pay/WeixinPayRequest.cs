using System;
using Newtonsoft.Json;

using Framework.Common.Http;
using Framework.Weixin.Response.Pay;

namespace Framework.Weixin.Request.Pay
{
    /// <summary>
    /// 微信支付请求抽象类
    /// </summary>
    /// <typeparam name="TWeixinPayResponse">请求返回类型</typeparam>
    public abstract class WeixinPayRequest<TWeixinPayResponse> : RequestObject<TWeixinPayResponse> where TWeixinPayResponse : WeixinPayResponse
    {
        /// <summary>
        /// 构造函数，传入公众号参数配置
        /// </summary>
        /// <param name="settings">传入公众号参数配置</param>
        public WeixinPayRequest(WeixinSettings settings)
        {
            WxSettings = settings;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appid">公众号id</param>
        /// <param name="mch_id">商户id</param>
        /// <param name="partnerKey">支付密钥</param>
        public WeixinPayRequest(string appid, string mch_id, string partnerKey)
        {
            WxSettings = new WeixinSettings
            {
                AppID = appid,
                MchId = mch_id,
                PartnerKey = partnerKey
            };
        }
        /// <summary>
        /// 微信公众号参数配置
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [UrlParamIgnore]
        protected WeixinSettings WxSettings { get; }
        /// <summary>
        /// 支付密钥，只读属性
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        [UrlParamIgnore]
        public virtual string key
        {
            get
            {
                return WxSettings.PartnerKey;
            }
        }
        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区
        /// 自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <returns></returns>       
        protected virtual string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        /// <summary>
        ///  生成随机串，随机串包含字母或数字
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
        /// <summary>
        /// 把资金转换成分
        /// </summary>
        /// <param name="totalFee"></param>
        /// <returns></returns>
        public virtual int ConverDecimalToIntMoney(decimal totalFee)
        {
            return int.Parse((totalFee * 100).ToString("f0"));
        }
    }
}
