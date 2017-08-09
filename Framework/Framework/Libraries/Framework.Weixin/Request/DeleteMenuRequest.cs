using System.Net.Http;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class DeleteMenuRequest : RequestObject<MenuResponse>
    {
        public DeleteMenuRequest(string token)
        {
            access_token = token;
        }

        public override string RequestUrl
        {
            get
            {
                return $"https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={access_token}";
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
    }
}
