using System.Net.Http;

using Newtonsoft.Json;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class TokenRequest : RequestObject<TokenResponse>
    {
        public TokenRequest(WeixinSettings settings)
        {
            AppId = settings.AppID;
            AppSecret = settings.AppSecret;
        }
        public TokenRequest(string appId, string secret)
        {
            AppId = appId;
            AppSecret = secret;
        }
        public override string RequestUrl
        {
            get
            {
                return string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppId, AppSecret);
            }
        }

        public string AppId
        {
            get;
            set;
        }

        public string AppSecret
        {
            get;
            set;
        }

        public override HttpMethod Method
        {
            get
            {
                return HttpMethod.Get;
            }
        }

        [JsonIgnore]
        public string TokenCacheKey
        {
            get
            {
                return string.Format("TokenRequest_{0}_{1}", AppId, AppSecret);
            }
        }
    }
}