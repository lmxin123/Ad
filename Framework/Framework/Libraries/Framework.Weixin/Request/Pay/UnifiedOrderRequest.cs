using System;

using Framework.Weixin.Request.Pay;
using Framework.Weixin.Response.Pay;
using Framework.Common;

namespace Framework.Weixin.Request
{
    /// <summary>
    /// 请求支付参数
    /// </summary>
    public class UnifiedOrderPayParamsRequest
    {
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 支付内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 支付成功后的通知地址，会自动拼接域名，只需要转入路径
        /// </summary>
        public string NotifyPath { get; set; }
        /// <summary>
        /// 用户openid
        /// </summary>
        public string OpenId { get; set; }
    }
    /// <summary>
    /// 统一下单支付请求
    /// </summary>
    public class UnifiedOrderRequest : WeixinPayRequest<UnifiedOrderResponse>
    {
        public UnifiedOrderRequest(WeixinSettings settings) : base(settings) { }
        /// <summary>
        /// 构造函数，为避免出错，包括了必要参数
        /// </summary>
        /// <param name="appid">微信分配的公众账号ID</param>
        /// <param name="mch_id">微信支付分配的商户号</param>
        /// <param name="partnerKey">支付密钥，用于生成签名</param>
        /// <param name="body">商品或支付单简要描述</param>
        /// <param name="totalFee">订单总金额</param>
        /// <param name="spbillCreateIp">APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP。</param>
        /// <param name="notifyUrl">接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。</param>
        /// <param name="tradeType">取值如下：JSAPI，NATIVE，APP</param>
        /// <param name="openId">trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识。</param>
        public UnifiedOrderRequest(string appid, string mch_id, string partnerKey, string body, decimal totalFee, string spbillCreateIp, string notifyUrl, string openId, string tradeType = "JSAPI") : base(appid, mch_id, partnerKey)
        {
            this.body = body;
            trade_type = tradeType;
            total_fee = ConverDecimalToIntMoney(totalFee);
            spbill_create_ip = spbillCreateIp;
            notify_url = notifyUrl;
            openid = openId;
            out_trade_no = Utility.GenerateTradeNo(WxSettings.MchId);
            nonce_str = Utility.GenerateNonceStr();
        }
        /// <summary>
        /// 请求地址
        /// </summary>
        public override string RequestUrl
        {
            get
            {
                return "https://api.mch.weixin.qq.com/pay/unifiedorder";
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
        /// 根据当前系统时间加随机序列和商户id来生成订单号
        /// </summary>
        /// <param name="mchId"></param>
        /// <returns></returns>
        public string out_trade_no { get; set; }
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
        /// 签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 设备号，可选参数
        /// 终端设备号(门店号或收银设备ID)，注意：PC网页或公众号内支付请传"WEB"
        /// </summary>
        public string device_info { get; set; } = "WEB";

        public string body { get; set; }

        public string detail { get; set; } = "";
        /// <summary>
        /// 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        public string attach { get; set; } = "";
        /// <summary>
        /// 符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string fee_type { get; set; } = "CNY";

        public int total_fee { get; set; }

        public string spbill_create_ip { get; set; } = "";
        /// <summary>
        /// 订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010。
        /// </summary>
        public string time_start { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        /// <summary>
        /// 订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010。
        /// </summary>
        public string time_expire { get; set; } = "";
        /// <summary>
        /// 商品标记，代金券或立减优惠功能的参数
        /// </summary>
        public string goods_tag { get; set; } = "";
        /// <summary>
        /// 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 取值如下：JSAPI，NATIVE，APP
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// trade_type=NATIVE，此参数必传。此id为二维码中包含的商品ID，商户自行定义。
        /// </summary>
        public string product_id { get; set; } = "";
        /// <summary>
        /// no_credit--指定不能使用信用卡支付
        /// </summary>
        public string limit_pay { get; set; } = "";
        /// <summary>
        /// trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识。
        /// </summary>
        public string openid { get; set; }
    }
}