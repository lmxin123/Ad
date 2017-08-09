using Newtonsoft.Json;

namespace Framework.Weixin
{
    public class WeixinSettings
    {
        public string OriginalID { get; set; }
        public string AppID { get; set; }
        public string AppSecret { get; set; }
        public string ValidToken { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 支付密钥
        /// </summary>
        public string PartnerKey { get; set; }
        /// <summary>
        /// 证书路径
        /// </summary>
        public string CertFullName { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassword { get; set; }
        /// <summary>
        /// 微信授权地址，需要替换redirect_uri，scope参数
        /// </summary>
        [JsonIgnore]
        public string AuthorizeUrl
        {
            get
            {
                return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect",
                AppID, "{0}", "{1}");
            }
        }
        /// <summary>
        /// 是否启用分销功能
        /// </summary>
        public bool EnableDistribution { get; set; }
    }
}
