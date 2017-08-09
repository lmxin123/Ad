using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Framework.Weixin.Response
{
    public abstract class WeiXinResponse
    {
        public string ToUserName
        {
            get
            {
                return new XCData(WeiXin.WeiXinRequest.FromUserName).ToString();
            }
            set { }
        }
        public string FromUserName
        {
            get
            {
                return new XCData(WeiXin.WeiXinRequest.ToUserName).ToString();
            }
            set { }
        }
        public long CreateTime
        {
            get
            {
                DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime nowTime = DateTime.Now;
                long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
                return unixTime;
            }
            set {; }
        }
        public virtual string MsgType { get; set; }
        protected XDocument XDoc = new XDocument();

        public override string ToString()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            MemoryStream ms = new MemoryStream();

            XmlWriter xtw = XmlTextWriter.Create(ms, settings);
            XmlRootAttribute rootAttr = new XmlRootAttribute("xml");
            XmlSerializer seri = new XmlSerializer(GetType(), rootAttr);

            seri.Serialize(xtw, this);

            string xml = Encoding.UTF8.GetString(ms.ToArray());
            xml = Regex.Replace(xml, "<xml xmlns[^>]*", "<xml").Replace("&lt;", "").Replace("&gt;", "");

            return xml;
        }

        public WeiXinContext WeiXin { get; }

        public XDocument Serialize<T>(T source)
        {
            XDocument target = new XDocument();
            XmlSerializer s = new XmlSerializer(typeof(T));
            XmlWriter writer = target.CreateWriter();
            s.Serialize(writer, source);
            writer.Close();
            return target;
        }

        public WeiXinResponse() { }

        public WeiXinResponse(WeiXinContext weiXin)
        {
            WeiXin = weiXin;
        }
    }
}
