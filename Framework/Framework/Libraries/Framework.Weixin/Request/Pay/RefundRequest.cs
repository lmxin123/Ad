using Framework.Common;
using Framework.Weixin.Response.Pay;

namespace Framework.Weixin.Request.Pay
{
    /// <summary>
    /// 退款请求
    /// </summary>
    public class RefundRequest : WeixinPayRequest<RefundResponse>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wxSettings"></param>
        public RefundRequest(WeixinSettings wxSettings) : base(wxSettings)
        {
            op_user_id = wxSettings.MchId;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="mch_id"></param>
        /// <param name="partnerKey"></param>
        /// <param name="tranId"></param>
        /// <param name="outRefundNo"></param>
        /// <param name="totalFee"></param>
        /// <param name="refundFee"></param>
        public RefundRequest(string appid, string mch_id, string partnerKey, string tranId, string outRefundNo, decimal totalFee, decimal refundFee) : base(appid, mch_id, partnerKey)
        {
            transaction_id = tranId;
            total_fee = ConverDecimalToIntMoney(totalFee);
            refund_fee = ConverDecimalToIntMoney(refundFee);
            nonce_str = Utility.GenerateNonceStr();
            out_refund_no = outRefundNo;
        }
        /// <summary>
        /// 请求地址
        /// </summary>
        public override string RequestUrl
        {
            get
            {
                return "https://api.mch.weixin.qq.com/secapi/pay/refund";
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
        /// 签名
        /// </summary>
        public string sign { get; set; }
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
        /// 随机数，默认返回Guid.NewGuid()去除'-'
        /// </summary>
        public virtual string nonce_str { get; set; }
        /// <summary>
        /// 设备号，可选参数
        /// 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
        /// </summary>
        public virtual string device_info { get; set; } = "WEB";
        /// <summary>
        /// 微信支付交易号,与微信订单号二选一
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号,与微信支付交易号二选一
        /// </summary>
        public string out_trade_no { get; set; } = "";
        /// <summary>
        /// 退款交易号
        /// </summary>
        public string out_refund_no { get; set; }
        /// <summary>
        /// 退款订单总金额，单位分
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 退款金额，单位分
        /// </summary>
        public int refund_fee { get; set; }
        /// <summary>
        /// 货币种类,非必需
        /// </summary>
        public string refund_fee_type { get; set; } = "CNY";
        /// <summary>
        /// 操作员帐号, 默认为商户号	,必需
        /// </summary>
        public string op_user_id { get; set; }
    }
}