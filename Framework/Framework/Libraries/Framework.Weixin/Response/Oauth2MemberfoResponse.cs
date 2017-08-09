using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class Oauth2MemberfoResponse : ResponseObject
    {
        public string OpenId { get; set; }
        public string UnionID { get; set; }
        public string NickName { get; set; }
        public int Sex { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string HeadImgUrl { get; set; }
        public string[] Privilege { get; set; }
        public string ErrCode { get; set; }
        public string ErrMsg { get; set; }
    }

}
