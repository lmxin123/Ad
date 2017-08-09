using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class TempTicketResponse : ResponseObject
    {
        /// <summary>
        /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过2592000（即30天）
        /// </summary>
        public int Expire_Seconds { get; set; }
        /// <summary>
        /// 二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 用ticket换取二维码图片地址
        /// </summary>
        public string QrCodeUrl
        {
            get
            {
                return "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + Ticket;
            }
        }

        public string ErrCode
        {
            get;
            set;
        }

        public string ErrMsg
        {
            get;
            set;
        }
    }

}
