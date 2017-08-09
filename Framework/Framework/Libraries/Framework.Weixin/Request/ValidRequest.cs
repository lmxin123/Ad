using System;
using System.Collections.Specialized;

namespace Framework.Weixin.Request
{
    public class ValidRequest : WeiXinRequest
    {
        NameValueCollection _query;
        public ValidRequest(WeiXinContext weiXin) : base(weiXin) { }

        public NameValueCollection Query
        {
            get
            {
                if (_query == null)
                {
                    _query = WeiXin.HttpRequest.QueryString;
                }
                return _query;
            }
        }

        public string Signature
        {
            get
            {
                return WeiXin.HttpRequest.QueryString["signature"];
            }
        }
        public string Timestamp
        {
            get
            {
                return WeiXin.HttpRequest.QueryString["timestamp"];
            }
        }
        public string Nonce
        {
            get
            {
                return WeiXin.HttpRequest.QueryString["nonce"];
            }
        }
        public string EchoStr
        {
            get
            {
                return WeiXin.HttpRequest.QueryString["echostr"];
            }
        }
    }
}