using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class TokenResponse : ResponseObject
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string Access_Token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int Expires_In { get; set; }
    }
}
