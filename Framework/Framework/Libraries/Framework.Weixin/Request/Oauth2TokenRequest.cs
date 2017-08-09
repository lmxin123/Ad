using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class Oauth2TokenRequest : RequestObject<Oauth2TokenResponse>
    {
        WeixinSettings _weixinSetting;
        private string _appID;
        private string _appSecret;
        private string _code;

        /// <summary>
        /// 注意需要额外传入 code
        /// </summary>
        /// <param name="weixinSetting"></param>
        public Oauth2TokenRequest(WeixinSettings weixinSetting, string code)
        {
            _weixinSetting = weixinSetting;
            _code = code;
        }

        public Oauth2TokenRequest(string appid, string appSecret, string code)
        {
            _code = code;
            _appID = appid;
            _appSecret = appSecret;
        }

        public override string RequestUrl
        {
            get {
                return string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", appid, appsecret, code); }
        }

        public string appid
        {
            get
            {
                return _weixinSetting == null ? _appID : _weixinSetting.AppID;
            }
        }

        public string appsecret
        {
            get
            {
                return _weixinSetting == null ? _appSecret : _weixinSetting.AppSecret;
            }
        }

        public string code
        {
            get { return _code; }
        }
    }
}
