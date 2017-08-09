using System;
using Framework.Common.Http;
using Framework.Weixin.Response;
using System.Net.Http;

namespace Framework.Weixin.Request
{
    public class CreateMenuRequest : RequestObject<MenuResponse>
    {
        public CreateMenuRequest(string token)
        {
            access_token = token;
        }

        public override string RequestUrl
        {
            get
            {
                return $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={access_token}";
            }
        }

        public string access_token { get; set; }

        /// <summary>
        /// json字符
        /// </summary>
        public string button { get; set; }
    }
}
