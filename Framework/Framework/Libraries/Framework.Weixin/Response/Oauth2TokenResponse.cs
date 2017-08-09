using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class Oauth2TokenResponse : ResponseObject
    {
        public string Access_Token { get; set; }

        public int Expires_In { get; set; }

        public string Refresh_Token { get; set; }

        public string OpenId
        {
            get;
            set;
        }

        public string Scope
        {
            get;
            set;
        }

        public string ErrCode
        {
            get;
            set;
        }
        public string ErrMsg
        {
            get;
            set;
        }
    }

}
