namespace Framework.Weixin.Response.Pay
{
    public class OrderQueryResponse : WeixinPayResponse
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

        public string openid { get; set; }

        public string is_subscribe { get; set; }

        public string trade_type { get; set; }

        public string trade_state { get; set; }

        public string bank_type { get; set; }

        public int total_fee { get; set; }

        public string fee_type { get; set; }

        public int cash_fee { get; set; }

        public string cash_fee_type { get; set; }

        public int coupon_fee { get; set; }

        public int coupon_count { get; set; }

        public string transaction_id { get; set; }

        public string out_trade_no { get; set; }

        public string attach { get; set; }

        public string time_end { get; set; }

        public string trade_state_desc { get; set; }
    }
}