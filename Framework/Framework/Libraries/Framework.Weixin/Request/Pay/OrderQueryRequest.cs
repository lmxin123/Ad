using Framework.Common;
using Framework.Weixin.Response.Pay;

namespace Framework.Weixin.Request.Pay
{
    /// <summary>
    /// 订单查询请求
    /// </summary>
    public class OrderQueryRequest : WeixinPayRequest<OrderQueryResponse>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wxSettings"></param>
        public OrderQueryRequest(WeixinSettings wxSettings) : base(wxSettings)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="mch_id"></param>
        /// <param name="partnerKey"></param>
        /// <param name="tranId"></param>
        public OrderQueryRequest(string appid, string mch_id, string partnerKey, string tranId) : base(appid, mch_id, partnerKey)
        {
            transaction_id = tranId;
            nonce_str = Utility.GenerateNonceStr();
        }
        /// <summary>
        /// 请求地址
        /// </summary>
        public override string RequestUrl
        {
            get
            {
                return "https://api.mch.weixin.qq.com/pay/orderquery";
            }
        }
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public virtual string appid
        {
            get
            {
                return WxSettings.AppID;
            }
        }
        /// <summary>
        /// 商户id
        /// </summary>
        public virtual string mch_id
        {
            get
            {
                return WxSettings.MchId;
            }
        }
        /// <summary>
        /// 微信支付交易号,与微信订单号二选一
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 随机数，默认返回Guid.NewGuid()去除'-'
        /// </summary>
        public virtual string nonce_str { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
    }
}