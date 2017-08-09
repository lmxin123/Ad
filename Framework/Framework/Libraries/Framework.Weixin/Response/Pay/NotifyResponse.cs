namespace Framework.Weixin.Response.Pay
{
    /// <summary>
    /// 支付结果通用通知参数
    /// </summary>
    public class NotifyResponse : WeixinPayResponse
    {
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public virtual string appid { get; set; }
        /// <summary>
        /// 商户号	
        /// </summary>
        public virtual string mch_id { get; set; }
        /// <summary>
        /// 设备号	
        /// </summary>
        public string device_info { get; set; } = "";
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 付款银行
        /// </summary>
        public string bank_type { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 货币种类
        /// </summary>
        public string fee_type { get; set; } = "";
        /// <summary>
        /// 现金支付金额	
        /// </summary>
        public int cash_fee { get; set; }
        /// <summary>
        /// 现金支付货币类型
        /// </summary>
        public string cash_fee_type { get; set; } = "";
        /// <summary>
        /// 代金券或立减优惠金额
        /// </summary>
        public int coupon_fee { get; set; }
        /// <summary>
        /// 代金券或立减优惠使用数量
        /// </summary>
        public int coupon_count { get; set; }
        ///代金券或立减优惠ID
        //public string coupon_id_$n { get; set; }
        ///单个代金券或立减优惠支付金额
        //public int coupon_fee_$n { get; set; }
        /// <summary>
        /// 微信支付订单号	
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号	
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商家数据包	
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 支付完成时间	
        /// </summary>
        public string time_end { get; set; } = "";
    }

    /// <summary>
    /// 商户处理后同步返回给微信参数
    /// </summary>
    public class NotifyResultResponse
    {
        public string return_code { get; set; }

        public string return_msg { get; set; }
    }
}