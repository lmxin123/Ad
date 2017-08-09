using System.Net.Http;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class GetMenuRequest : RequestObject<MenuResponse>
    {
        public GetMenuRequest(string token)
        {
            access_token = token;
        }

        public override string RequestUrl
        {
            get
            {
                return $"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={access_token}";
            }
        }

        public string access_token { get; set; }

        public override HttpMethod Method
        {
            get
            {
                return HttpMethod.Get;
            }
        }

        public string MenuRequestCacheKey
        {
            get
            {
                return "MenuRequest_CacheKey";
            }
        }
    }
}
