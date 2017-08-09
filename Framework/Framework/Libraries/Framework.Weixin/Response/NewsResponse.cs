using System.Collections.Generic;
using System.Xml.Linq;

namespace Framework.Weixin.Response
{
    public class NewsResponse : WeiXinResponse
    {
        List<item> _articles;

        public override string MsgType
        {
            get
            {
                return new XCData("news").ToString();
            }
            set {; }
        }

        public int ArticleCount { get; set; }

        public List<item> Articles
        {
            get
            {
                if (_articles == null)
                    _articles = new List<item>();
                return _articles;
            }
        }
        public NewsResponse() : base() { }
        public NewsResponse(WeiXinContext weiXin) : base(weiXin) { }

        public void AddArticle(string title, string description, string picUrl, string url)
        {
            item item = new item()
            {
                Title = title,
                Description = description,
                PicUrl = picUrl,
                Url = url
            };
            Articles.Add(item);
        }

        public class item
        {
            private string _Title;
            private string _Description;
            private string _PicUrl;
            private string _Url;

            public string Title
            {
                get
                {
                    return new XCData(_Title).ToString();
                }
                set { _Title = value; }
            }
            public string Description
            {
                get
                {
                    return new XCData(_Description).ToString();
                }
                set { _Description = value; }
            }
            public string PicUrl
            {
                get
                {

                    return new XCData(_PicUrl).ToString();
                }
                set { _PicUrl = value; }
            }
            public string Url
            {
                get
                {
                    return new XCData(_Url).ToString();
                }
                set { _Url = value; }
            }
        }

    }
}