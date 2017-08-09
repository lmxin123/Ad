using Framework.Common;
using Framework.Weixin.Response.Pay;

namespace Framework.Weixin.Request.Pay
{
    /// <summary>
    /// 用于企业向微信用户个人付款 
    ///目前支持向指定微信用户的openid付款
    /// </summary>
    public class TransfersRequest : WeixinPayRequest<TransfersResponse>
    {
        public TransfersRequest(WeixinSettings settings) : base(settings) { }

        public TransfersRequest(string mchAppid, string mchId, string openId, string partnerKey, decimal amount, string desc, string spbillCreateIp) : base(mchAppid, mchId, partnerKey)
        {
            this.openid = openId;
            this.amount = ConverDecimalToIntMoney(amount);
            this.desc = desc;
            spbill_create_ip = spbillCreateIp;
            partner_trade_no = Utility.GenerateTradeNo(mchId);
            nonce_str = Utility.GenerateNonceStr();
        }
        /// <summary>
        /// 请求地址
        /// </summary>
        public override string RequestUrl
        {
            get
            {
                return "https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers";
            }
        }
        /// <summary>
        /// 微信分配的公众账号ID（企业号corpid即为此appId）
        /// </summary>
        public virtual string mch_appid { get { return WxSettings.AppID; } }
        /// <summary>
        /// 微信支付分配的商户号，必填
        /// </summary>
        public virtual string mchid { get { return WxSettings.MchId; } }
        /// <summary>
        /// 
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 设备号,微信支付分配的终端设备号，选填
        /// </summary>
        public string device_info { get; set; } = "WEB";
        /// <summary>
        /// 随机字符串，必填
        /// </summary>
        public virtual string nonce_str { get; set; }
        /// <summary>
        /// 签名，必填
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 商户订单号，必填
        /// </summary>
        public string partner_trade_no { get; set; }
        /// <summary>
        /// NO_CHECK：不校验真实姓名 
        ///FORCE_CHECK：强校验真实姓名（未实名认证的用户会校验失败，无法转账） 
        ///OPTION_CHECK：针对已实名认证的用户才校验真实姓名（未实名认证用户不校验，可以转账成功）
        /// </summary>
        public string check_name { get; set; } = "NO_CHECK";
        /// <summary>
        /// 收款用户真实姓名。 
        ///如果check_name设置为FORCE_CHECK或OPTION_CHECK，则必填用户真实姓名
        /// </summary>
        public string re_user_name { get; set; } = "";
        /// <summary>
        /// 企业付款金额，单位为分，必填
        /// </summary>
        public int amount { get; set; }
        /// <summary>
        /// 企业付款操作说明信息。必填。
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 调用接口的机器Ip地址，必填
        /// </summary>
        public string spbill_create_ip { get; set; }
    }
}
