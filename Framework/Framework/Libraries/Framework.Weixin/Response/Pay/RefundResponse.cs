namespace Framework.Weixin.Response.Pay
{
    public class RefundResponse:WeixinPayResponse
    {
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public virtual string appid { get; set; }
        /// <summary>
        /// 商户号	
        /// </summary>
        public virtual string mch_id { get; set; }

        public string device_info { get; set; }

        public string transaction_id { get; set; }

        public string out_trade_no { get; set; }

        public string out_refund_no { get; set; }

        public string refund_id { get; set; }

        public string refund_channel { get; set; }

        public int refund_fee { get; set; }

        public int total_fee { get; set; }

        public int fee_type { get; set; }

        public int cash_fee { get; set; }

        public int cash_refund_fee { get; set; }

        public int coupon_refund_fee { get; set; }

        public int coupon_refund_count { get; set; }

        public string coupon_refund_id { get; set; }
    }
}