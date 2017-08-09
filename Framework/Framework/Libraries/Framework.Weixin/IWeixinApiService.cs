using System.Threading.Tasks;

using Framework.Weixin.Response;
using Framework.Weixin.Response.Pay;

namespace Framework.Weixin
{
    /// <summary>
    /// 微信公众平台所有接口
    /// 接口方法全部为异步方法，调用时请根据实际需要以同步或者异步方式使用
    /// </summary>
    public interface IWeixinApiService
    {
        #region 普通接口
        /// <summary>
        /// 获取access token，access_token是公众号的全局唯一票据，
        /// 公众号调用各接口时都需使用access_token
        /// </summary>
        /// <returns></returns>
        Task<TokenResponse> GetTokenAsync();
        /// <summary>
        /// 授权access_token和普通access_token的区别
        ///微信网页授权是通过OAuth2.0机制实现的，
        ///在用户授权给公众号后，
        ///公众号可以获取到一个网页授权特有的接口调用凭证（网页授权access_token）
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Oauth2TokenResponse> GetOauth2TokenAsync(string code);
        /// <summary>
        /// 获取微信用户信息
        /// </summary>
        /// <param name="oauth2Token"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<Oauth2MemberfoResponse> GetWeixinInfoAsync(string oauth2Token, string openId);
        /// <summary>
        /// 获取临时二维码地址
        /// </summary>
        /// <param name="sceneId">场景值ID，临时二维码时为32位非0整型</param>
        /// <returns></returns>
        Task<TempTicketResponse> GetTempQrCodeUrlAsync(int sceneId);
        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="toUser">用户openid</param>
        /// <param name="tmpl_Id">模板id，请使用TmplMessagesIds类型赋值</param>
        /// <param name="url">模板id点击跳转地址</param>
        /// <param name="dataContent">模板数据内容，不需要再加上data</param>
        /// <param name="topColor">信息头部颜色</param>
        /// <returns></returns>
        Task<TmplMessageResponse> SendTmplMsgAsync(string toUser, string tmpl_Id, string url, object dataContent, string topColor = "");
        /// <summary>
        /// 获得jsapi_ticket
        /// </summary>
        /// <returns></returns>
        Task<JSApiTicketResponse> GetJsAPiTicketAsync();
        /// <summary>
        /// 获取微信自定义菜单，包括个性化菜单，默认缓存两小时
        /// </summary>
        /// <returns></returns>
        Task<MenuResponse> GetMenu();
        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        Task<MenuResponse> CreateMenu(string button);
        /// <summary>
        /// 删除自定义菜单
        /// </summary>
        /// <returns></returns>
        Task<MenuResponse> DeleteMenu();
        #endregion

        #region 微信支付接口
        /// <summary>
        /// 退款，需要用到证书
        /// </summary>
        /// <param name="transactionId">交易号</param>
        /// <param name="outRefundNo"></param>
        /// <param name="refundFee"></param>
        /// <returns></returns>
        Task<RefundResponse> RefundAsync(string transactionId, string outRefundNo, decimal totalFee, decimal refundFee);
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        Task<OrderQueryResponse> OrderQueryAsync(string transactionId);
        /// <summary>
        /// 企业向微信用户个人付款 
        /// </summary>
        /// <param name="openId">微信用户标识</param>
        /// <param name="amount">金额</param>
        /// <param name="desc">付款描述</param>
        /// <param name="spbillCreateIp">客户端ip地址</param>
        /// <param name="reUserName">用户真实姓名，默认为空</param>
        /// <returns></returns>
        Task<TransfersResponse> TransfersAsync(string openId, decimal amount, string desc, string spbillCreateIp = null, string reUserName = "");
        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="amount"></param>
        /// <param name="notifyPath"></param>
        /// <param name="productId"></param>
        /// <param name="body"></param>
        /// <param name="attach"></param>
        /// <returns></returns>
        Task<UnifiedOrderPayParamsResponse> Unifiedorder(string openId, decimal amount, string notifyPath, string productId, string body, string attach);
        #endregion
    }
}
