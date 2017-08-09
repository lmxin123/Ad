using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Framework.Weixin.Response;
using Framework.Weixin.Request;
using Framework.Weixin.DataSource;
using Newtonsoft.Json;

namespace Framework.Weixin.Command
{
    public abstract class CommandBase : ICommand
    {
        public WeiXinContext WeiXin
        {
            get;
            set;
        }

        public CommandBase(WeiXinContext weiXin)
        {
            this.WeiXin = weiXin;
        }

        public abstract Task<WeiXinResponse> ExecuteAsync();


        public abstract bool CanExecute
        {
            get;
        }

        public virtual string GetTextContentOrEventKey()
        {
            TextRequest textRequest = this.WeiXin.WeiXinRequest as TextRequest;
            if (textRequest != null)
                return textRequest.Content.Trim();

            EventRequest eventRequest = this.WeiXin.WeiXinRequest as EventRequest;
            if (eventRequest != null)
                return eventRequest.EventKey;

            return string.Empty;
        }

        public virtual WeiXinResponse GetResponse(OTTCommand command)
        {
            IDataSource dataSource = null;
            switch (command.DataSource)
            {
                case "text":
                    dataSource = new DataFieldDataSource(command, this.WeiXin);
                    break;
                case "DataField":
                    // dataSource = new DataFieldDataSource(command, this.WeiXin);
                    break;
                case "Article":
                    // dataSource = new ArticleDataSource(command, this.WeiXin);
                    break;
                case "Product":
                    //dataSource = new ProductDataSource(command, this.WeiXin);
                    break;
                case "CompanyInfo":
                    // dataSource = new CompanyInfoDataSource(command, this.WeiXin);
                    break;
                case "DataIndex":
                    //dataSource = new SearchDataSource(command, this.WeiXin);
                    break;
                case "User":
                    //dataSource = new UserDataSource(command, this.WeiXin);
                    break;
            }

            if (dataSource != null)
            {
                return dataSource.GetResponse();
            }
            else
            {
                throw new Exception("没有匹配的数据源");
            }
        }

        public virtual WeiXinResponse GetResponse()
        {
            IDataSource dataSource = null;
            var data = new
            {
                title = "感谢您关注",
                content = "测试",
                image = "http://fx.51llgo.com",
                url = "http://mp.weixin.qq.com/s?__biz=MzI4MzEwODQ4Mw==&mid=401947642&idx=1&sn=c01694a4767684dd9fb9f2edf225abb8#rd"
            };
            List <dynamic> dataItems = new List<dynamic>();
            dataItems.Add(data);
            var command = new OTTCommand()
            {
                Command = "subscribe",
                CommandId = 1382,
                Data =JsonConvert.SerializeObject(dataItems),
                DataSource = "DataField",
                OTT = "WeiXin",
                SiteId = 0,
                Style = 3
            };

            dataSource = new DataFieldDataSource(command, WeiXin);

            if (dataSource != null)
            {
                return dataSource.GetResponse();
            }
            else
            {
                throw new Exception("没有匹配的数据源");
            }
        }
    }

    public class OTTCommand
    {
        public int CommandId { get; set; }

        public int SiteId { get; set; }

        public bool IsSysCommand { get; set; }

        public string Command { get; set; }

        public string OTT { get; set; }

        public string DataSource { get; set; }

        public string DataIds { get; set; }

        public bool Random { get; set; }

        public string OrderBy { get; set; }

        public string WhereClause { get; set; }

        public int Style { get; set; }

        public string Format { get; set; }

        public int Size { get; set; }

        public string Data { get; set; }

        public string Config { get; set; }

        public string Summary { get; set; }

        public int Status { get; set; }
    }
}