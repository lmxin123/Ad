using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Framework.Weixin.Request;
using Framework.Weixin.Request.Pay;
using Framework.Weixin.Response;
using Framework.Weixin.Response.Pay;
using Framework.Common.Http;
using System.Web.Caching;
using Framework.Caching;
using Framework.Common;

namespace Framework.Weixin
{
    public class WeixinApiService : IWeixinApiService
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IClientService _clientService;
        private readonly WeixinSettings _weixinSetting;
        //  private readonly CommonContext _commonContext;

        public WeixinApiService(
            // IHttpContextAccessor contextAccessor, 
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            //  _commonContext = new CommonContext(contextAccessor);
            //   _weixinSetting = _commonContext.GetService<IOptions<WeixinSettings>>().Value;
            //  _logger = _commonContext.CreateLogger<WeixinApiService>();
            _clientService = new ClientService(loggerFactory);
        }

        #region 普通接口
        public async Task<TokenResponse> GetTokenAsync()
        {

            var tokenReq = new TokenRequest(_weixinSetting);
            var tokenResp = WebCache.Get<TokenResponse>(tokenReq.TokenCacheKey);
            if (tokenResp == null)
            {
                _logger.LogInformation("Token己过期，重新获取");
                tokenResp = await _clientService.ExecuteAsync(tokenReq, RequestStringDataTypes.Json);
                if (!string.IsNullOrEmpty(tokenResp.Access_Token))
                    WebCache.Set(tokenReq.TokenCacheKey, tokenResp,new CacheDependency("GetTokenAsync", new DateTime(0, 0, tokenResp.Expires_In)));
            }
            return tokenResp;
        }

        public async Task<Oauth2TokenResponse> GetOauth2TokenAsync(string code)
        {
            var tokenReq = new Oauth2TokenRequest(_weixinSetting, code);
            var tokenResp = await _clientService.ExecuteAsync(tokenReq, RequestStringDataTypes.Json);
            return tokenResp;
        }

        public async Task<Oauth2MemberfoResponse> GetWeixinInfoAsync(string access_token, string openId)
        {
            var infoReq = new Oauth2MemberInfoRequest();
            infoReq.access_token = access_token;
            infoReq.openid = openId;
            var infoResp = new Oauth2MemberfoResponse();
            try
            {
                infoResp = await _clientService.ExecuteAsync(infoReq, RequestStringDataTypes.Json);
            }
            //这个异常一般可以忽略，这里是为了不影响程序正常运行
            catch (Exception e)
            {
                _logger.LogError("执行GetWeixinInfo异常：{0}", e.Message);
            }
            return infoResp;
        }

        public async Task<TempTicketResponse> GetTempQrCodeUrlAsync(int sceneId)
        {
            var tmpTicketReq = new TempTicketRequest((await GetTokenAsync()).Access_Token, sceneId);
            var tmpTicketResp = WebCache.Get<TempTicketResponse>(tmpTicketReq.TempTicketCacheKey);
            if (tmpTicketResp == null)
            {
                tmpTicketResp = await _clientService.ExecuteAsync(tmpTicketReq, RequestStringDataTypes.Json);
                if (!string.IsNullOrEmpty(tmpTicketResp.Ticket))
                    WebCache.Set(tmpTicketReq.TempTicketCacheKey, tmpTicketResp,new CacheDependency("GetTempQrCodeUrlAsync", new DateTime(0, 1, 53)));
            }
            return tmpTicketResp;
        }

        public async Task<TmplMessageResponse> SendTmplMsgAsync(string toUser, string tmpl_Id, string url, object dataContent, string topColor = "")
        {
            var tmplMsgReq = new TmplMessageRequest
            {
                access_token = (await GetTokenAsync()).Access_Token,
                touser = toUser,
                template_id = tmpl_Id,
                url = url,
                data = dataContent,
                topColor = topColor
            };
            _logger.LogInformation("给用户 {0} 发送模板信息。", toUser);
            var tmplMsgResp = await _clientService.ExecuteAsync(tmplMsgReq, RequestStringDataTypes.Json);
            _logger.LogInformation("发送结果:{0}", JsonConvert.SerializeObject(tmplMsgResp));
            return tmplMsgResp;
        }

        public async Task<JSApiTicketResponse> GetJsAPiTicketAsync()
        {
            var jsApiTicketReq = new JSApiTicketRequest((await GetTokenAsync()).Access_Token);
            var jsApiTicketResp = WebCache.Get<JSApiTicketResponse>(jsApiTicketReq.JsApiTicketCacheKey);
            if (jsApiTicketResp == null)
            {
                _logger.LogInformation("JsApiTicket己过期，重新获取");
                jsApiTicketResp = await _clientService.ExecuteAsync(jsApiTicketReq, RequestStringDataTypes.Json);
                if (!string.IsNullOrEmpty(jsApiTicketResp.Ticket))
                    WebCache.Set(jsApiTicketReq.JsApiTicketCacheKey, jsApiTicketResp, new CacheDependency("jsApiTicketResp", new DateTime(0, 0, jsApiTicketResp.Expires_In)));
            }
            return jsApiTicketResp;
        }

        public async Task<MenuResponse> GetMenu()
        {
            var getMenuReq = new GetMenuRequest((await GetTokenAsync()).Access_Token);
            var getMenuResp = WebCache.Get<MenuResponse>(getMenuReq.MenuRequestCacheKey);
            if (getMenuResp == null)
            {
                _logger.LogInformation("Menu己过期，重新获取");
                getMenuResp = await _clientService.ExecuteAsync(getMenuReq, RequestStringDataTypes.String);
                WebCache.Set(getMenuReq.MenuRequestCacheKey, getMenuResp, new CacheDependency("GetMenu", new DateTime(2, 0, 0)));
            }
            return getMenuResp;
        }

        public async Task<MenuResponse> CreateMenu(string button)
        {
            var createMenuReq = new CreateMenuRequest((await GetTokenAsync()).Access_Token);
            createMenuReq.button = button;
            var createMenuResp = await _clientService.ExecuteAsync(createMenuReq, RequestStringDataTypes.String);
            return createMenuResp;
        }
        public async Task<MenuResponse> DeleteMenu()
        {
            var delMenuReq = new CreateMenuRequest((await GetTokenAsync()).Access_Token);
            var delMenuResp = await _clientService.ExecuteAsync(delMenuReq, RequestStringDataTypes.String);
            return delMenuResp;
        }
        #endregion

        #region 微信支付接口
        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="transactionId">微信支付交易号</param>
        /// <param name="outRefundNo">商户系统内部的退款单号，商户系统内部唯一，同一退款单号多次请求只退一笔</param>
        /// <returns></returns>
        public async Task<RefundResponse> RefundAsync(string transactionId, string outRefundNo, decimal totalFee, decimal refundFee)
        {
           // var appEnv = _commonContext.GetService<IApplicationEnvironment>();
            var clientService = new ClientService(_loggerFactory);
            var refundReq = new RefundRequest(_weixinSetting);
            refundReq.transaction_id = transactionId;
            refundReq.out_refund_no = outRefundNo;
            refundReq.total_fee = refundReq.ConverDecimalToIntMoney(totalFee);
            refundReq.refund_fee = refundReq.ConverDecimalToIntMoney(refundFee);
            refundReq.nonce_str = Utility.GenerateNonceStr();
            refundReq.sign = Utility.MakeSign(refundReq, _weixinSetting.PartnerKey);

            var resp = await clientService.ExecuteAsync(
                refundReq,
                RequestStringDataTypes.Xml,
                _weixinSetting.CertPassword,
                _weixinSetting.CertFullName);
            return resp;
        }

        public async Task<OrderQueryResponse> OrderQueryAsync(string transactionId)
        {
            var orderQueryReq = new OrderQueryRequest(
                _weixinSetting.AppID,
                _weixinSetting.MchId,
                _weixinSetting.PartnerKey,
                transactionId);
            orderQueryReq.sign = Utility.MakeSign(orderQueryReq, _weixinSetting.PartnerKey);
            var orderQueryResp = await _clientService.ExecuteAsync(orderQueryReq, RequestStringDataTypes.Xml);
            return orderQueryResp;
        }

        public async Task<TransfersResponse> TransfersAsync(string openId, decimal amount, string desc, string spbillCreateIp, string reUserName = "")
        {
            var transferReq = new TransfersRequest(
                _weixinSetting.AppID,
                _weixinSetting.MchId,
                 openId,
                _weixinSetting.PartnerKey,
                amount,
                desc,
                spbillCreateIp ?? "192.168.1.122");

            transferReq.sign = Utility.MakeSign(transferReq, _weixinSetting.PartnerKey);

            //var appEnv = _commonContext.GetService<IApplicationEnvironment>();
            var clientService = new ClientService(_loggerFactory);
            var resp = await clientService.ExecuteAsync(
                transferReq,
                RequestStringDataTypes.Xml,
                _weixinSetting.CertPassword,
                _weixinSetting.CertFullName);
            return resp;
        }

        public async Task<UnifiedOrderPayParamsResponse> Unifiedorder(string openId, decimal amount, string notifyPath, string productId, string body, string attach)
        {
            var unifiedOrderReq = new UnifiedOrderRequest(
                    _weixinSetting.AppID,
                    _weixinSetting.MchId,
                    _weixinSetting.PartnerKey,
                    body,
                   amount,
                    "192.168.99.1",
                   notifyPath,
                    openId);

            unifiedOrderReq.attach = attach;
            unifiedOrderReq.detail = body;
            unifiedOrderReq.product_id = productId;
            unifiedOrderReq.sign = Utility.MakeSign(unifiedOrderReq, _weixinSetting.PartnerKey);

            var unifiedOrderResp = await _clientService.ExecuteAsync(unifiedOrderReq, RequestStringDataTypes.Xml);

            var payParams = new UnifiedOrderPayParamsResponse();
            payParams.returnCode = unifiedOrderResp.return_code;
            payParams.returnMsg = unifiedOrderResp.return_msg;

            if (unifiedOrderResp.return_code == "SUCCESS")
            {
                payParams.appId = _weixinSetting.AppID;
                payParams.nonceStr = Utility.GenerateNonceStr();
                payParams.package = string.Format("prepay_id={0}", unifiedOrderResp.prepay_id);
                payParams.timeStamp = Utility.GenerateTimeStamp();
                payParams.signType = "MD5";

                payParams.OpenId = openId;
                payParams.OutTradeNo = unifiedOrderReq.out_trade_no;
                payParams.paySign = Utility.MakeSign(payParams, _weixinSetting.PartnerKey);
            }

            return payParams;
        }

        #endregion
    }
}
