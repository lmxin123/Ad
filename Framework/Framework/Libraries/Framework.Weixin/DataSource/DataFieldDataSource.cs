using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Framework.Weixin.Response;
using Framework.Weixin.Command;

namespace Framework.Weixin.DataSource
{
    public class DataFieldDataSource : DataSourceBase
    {
        public DataFieldDataSource(OTTCommand command, WeiXinContext weiXin) : base(command, weiXin) { }

        public override WeiXinResponse GetResponse()
        {
            WeiXinResponse resp = null;
            dynamic data = JsonConvert.DeserializeObject<dynamic>(Command.Data);
            switch (this.Command.Style)
            {
                case 1:
                    TextResponse textResp = new TextResponse(this.WeiXin);
                    textResp.Content = data.content;
                    resp = textResp;
                    break;
                case 3:
                default:
                    NewsResponse newsResp = new NewsResponse(WeiXin);

                    foreach (dynamic item in data)
                    {
                        string newsTitle = (string)item.title;
                        if (newsTitle == null) newsTitle = "";
                        string content = (string)item.content;
                        if (content == null) content = "";
                        string image = (string)item.image;
                        if (image == null)
                        {
                            image = "";
                        }

                        string url = (string)item.url;
                        if (url == null) url = "";

                        newsResp.AddArticle(newsTitle,content, image, url);
                    }
                    newsResp.ArticleCount = newsResp.Articles.Count;
                    resp = newsResp;
                    break;
            }
            return resp;
        }
    }
}
