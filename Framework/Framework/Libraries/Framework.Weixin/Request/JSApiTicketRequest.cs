using System.Net.Http;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class JSApiTicketRequest : RequestObject<JSApiTicketResponse>
    {
        public JSApiTicketRequest(string token)
        {
            access_token = token;
        }

        public override string RequestUrl
        {
            get
            {
                return string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", access_token);
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

        public string JsApiTicketCacheKey
        {
            get
            {
                return "JSApiTicketRequest_CacheKey";
            }
        }
    }
}
