using Newtonsoft.Json;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class Oauth2MemberInfoRequest : RequestObject<Oauth2MemberfoResponse>
    {
        public override string RequestUrl
        {
            get { return string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", access_token, openid); }
        }

        public string openid { get; set; }

        public string access_token { get; set; }
        [JsonIgnore]
        public string WeixinInfoCaheKey
        {
            get
            {
                return string.Format(string.Format("{0}_{0}", GetType().Name, openid));
            }
        }
    }
}
