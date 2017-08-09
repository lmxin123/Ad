namespace Framework.Weixin.Response.Pay
{
    public class TransfersResponse:WeixinPayResponse
    {
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string mch_appid { get; set; }
        /// <summary>
        /// 商户号	
        /// </summary>
        public string mchid { get; set; }

        public string device_info { get; set; }

        public string partner_trade_no { get; set; }

        public string payment_no { get; set; }

        public string payment_time { get; set; }
    }
}
