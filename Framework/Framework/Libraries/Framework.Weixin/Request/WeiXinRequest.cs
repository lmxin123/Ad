using System.Xml.Linq;

namespace Framework.Weixin.Request
{
    public abstract class WeiXinRequest
    {
        public readonly XDocument _xmlDoc;

        public WeiXinContext WeiXin { get; private set; }

        public string ToUserName
        {
            get
            {
                return GetParameter("ToUserName");
            }
        }

        public string FromUserName
        {
            get
            {
                return GetParameter("FromUserName");
            }
        }

        public string CreateTime
        {
            get
            {
                return GetParameter("CreateTime");
            }
        }

        public string MsgType
        {
            get
            {
                return GetParameter("MsgType");
            }
        }

        public string MsgId
        {
            get
            {
                return GetParameter("MsgId");
            }
        }

        public virtual string GetParameter(string name)
        {
            XElement node = WeiXin.XDoc.Root.Element(name);
            if (node != null)
                return node.Value;
            return string.Empty;
        }

        public WeiXinRequest(WeiXinContext weiXin)
        {
            WeiXin = weiXin;
        }
    }
}