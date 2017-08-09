using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class JSApiTicketResponse : ResponseObject
    {
        public string Ticket { get; set; }
        public int Expires_In { get; set; }
        public string ErrCode { get; set; }
        public string ErrMsg { get; set; }
    }
}
