using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Framework.Weixin.Command;
using Framework.Weixin.Request;
using Framework.Weixin.Response;
using System.Web;

namespace Framework.Weixin
{
    public class WeiXinContext
    {
        private readonly ILogger _logger;
       // private readonly CommonContext _commonContext;
        List<ICommand> _commands;
        List<Type> _requestTypes;

        public WeiXinContext(HttpRequest request, HttpResponse response, ILogger logger)
        {
            _logger = logger;
            HttpRequest = request;
            HttpResponse = response;
            XDoc = XDocument.Load(InputStream);
        }
        public HttpRequest HttpRequest { get; private set; }
        public HttpResponse HttpResponse { get; private set; }
        public WeiXinRequest WeiXinRequest { get; private set; }
        public WeiXinResponse WeiXinResponse { get; private set; }
        public XDocument XDoc { get; }
        public Stream InputStream
        {
            get
            {
                return HttpRequest.InputStream;
            }
        }

        public List<ICommand> Commands
        {
            get
            {
                if (_commands == null)
                    _commands = new List<ICommand>();
                return _commands;
            }
        }

        public List<Type> RequestTypes
        {
            get
            {
                if (_requestTypes == null)
                    _requestTypes = new List<Type>();
                return _requestTypes;
            }
        }

        public async Task<string> ExecuteAsync()
        {
            var result = "";
            if (!string.IsNullOrEmpty(HttpRequest.QueryString["echostr"]))
            {
                WeiXinRequest = new ValidRequest(this);
            }
            else
            {
                try
                {
                    _logger.LogInformation("后台接收到数据流：{0}", XDoc.ToString());
                    Type type = RequestTypes.Find(T => T.Name.Equals(string.Format("{0}Request", XDoc.Root.Element("MsgType").Value), StringComparison.CurrentCultureIgnoreCase));

                    if (type != null)
                    {
                        WeiXinRequest = (WeiXinRequest)Activator.CreateInstance(type, new object[1] { this });
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("命令执行出错：{0}", ex.StackTrace);
                }
            }

            ICommand command = Commands.Find(C => C.CanExecute);

            if (command != null)
            {
                try
                {
                    WeiXinResponse = await command.ExecuteAsync();
                    result = WeiXinResponse.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("命令执行出错：{0}", ex.StackTrace);
                }
            }

            return result;
        }
    }
}