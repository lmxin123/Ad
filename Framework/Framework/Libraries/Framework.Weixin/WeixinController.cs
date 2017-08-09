using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Framework.Weixin.Command;
using Framework.Weixin.Request;
using Framework.Weixin.Response;
using Framework.Common.Http;
using Framework.Common.Json;
using Framework.Common.Extensions;

namespace Framework.Weixin
{
    /// <summary>
    /// 微信通用交互类
    /// </summary>
    [Route("weixin/[action]")]
    public class WeixinController : Controller
    {
        private readonly ILogger _logger;
        private readonly IClientService _clientService;
        private readonly WeixinSettings _weixinSetting;
        private readonly IWeixinApiService _weixinService;
      //  private readonly CommonContext _commonContext;
        private ResultModel _result;
        private WeiXinContext _weiXinContext;
        public WeixinController(
           // IHttpContextAccessor contextAccessor,
            ILoggerFactory loggerFactory)
        {
          //  _commonContext = new CommonContext(contextAccessor);
           // _weixinSetting = _commonContext.GetService<IOptions<WeixinSettings>>().Value;
            //_weixinService = _commonContext.GetService<IWeixinApiService>();
          //  _logger = _commonContext.CreateLogger<WeixinController>();
            _clientService = new ClientService(loggerFactory);
            _result = new ResultModel();
        }

        public virtual WeiXinContext WeixinContext
        {
            get
            {
                if (_weiXinContext == null)
                {
                   // _weiXinContext = new WeiXinContext(_commonContext);
                }
                return _weiXinContext;
            }
        }

        /// <summary>
        /// 与微信服务器数据交互
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Service()
        {
            WeixinContext.RequestTypes.Add(typeof(EventRequest));
            WeixinContext.Commands.Add(new ValidCommand(WeixinContext, _weixinSetting));

            string result = await WeixinContext.ExecuteAsync();
            _logger.LogInformation("返回微信服务器数据：{0}", result);

            return Content(result);
        }
        /// <summary>
        /// 生成前端脚本接口签名
        /// </summary>
        /// <param name="nonceStr"></param>
        /// <param name="timestamp"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetSinature(string nonceStr, string timestamp, string url)
        {
            var jsApiTicket = await _weixinService.GetJsAPiTicketAsync();
            string string1 = $"jsapi_ticket={jsApiTicket.Ticket}&noncestr={nonceStr}&timestamp={timestamp}&url={url}";
            return Content(string1.SHA1Encrypt());
        }
        /// <summary>
        /// 生成统一支付订单参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultModel> GenerateUnifiedOrderParams(UnifiedOrderPayParamsRequest model)
        {
            try
            {
                var notifyUrl = model.NotifyPath;
                var paramsResp = await _weixinService.Unifiedorder(model.OpenId, model.Amount, notifyUrl, model.ProductId, model.Body, model.Attach);
                if (paramsResp.returnCode != "SUCCESS")
                {
                    _result.Message = paramsResp.returnMsg;
                    _result.Status = 0;
                }
                else
                {
                    _result.Data = paramsResp;
                    _logger.LogInformation("支付参数：{0}", JsonConvert.SerializeObject(paramsResp));
                }
            }
            catch (Exception e)
            {
                _result.Status = 0;
                _result.Message = e.Message;
                _logger.LogError("请求微信支付参数异常：{0}", e.Message + e.StackTrace);
            }
            return _result;
        }
        /// <summary>
        /// 获取微信自定义菜单数据
        /// </summary>
        /// <returns></returns>
        public ResultModel GetMenu()
        {
            try
            {
                var response = _weixinService.GetMenu().Result;
                _result.Data = response.Body;
            }
            catch (Exception e)
            {
                _result.Status = 0;
                _result.Message = "获取自宝义菜单失败！";
                _logger.LogError($"{_result.Message}{e.Message + e.StackTrace}");
            }
            return _result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel CreateMenu(string button)
        {
            if (string.IsNullOrEmpty(button))
            {
                _result.Status = 0;
                _result.Message = "参数有误！";
            }
            else
            {
                var response = new MenuResponse();
                try
                {
                    response = _weixinService.CreateMenu(button).Result;
                }
                catch (Exception e)
                {
                    _result.Status = 0;
                    _result.Message = response.errmsg;
                    _logger.LogError($"{_result.Message}{e.Message + e.StackTrace}");
                }
            }
            return _result;
        }
        /// <summary>
        /// 删除微信自定义菜单
        /// </summary>
        /// <returns></returns>
        public ResultModel DeleteMenu()
        {
            var response = new MenuResponse();
            try
            {
                response = _weixinService.DeleteMenu().Result;
                _result.Message = "删除成功！";
            }
            catch (Exception e)
            {
                _result.Status = 0;
                _result.Message = response.errmsg;
                _logger.LogError($"{_result.Message}{e.Message + e.StackTrace}");
            }
            return _result;
        }
    }
}
