using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class TmplMessageResponse : ResponseObject
    {
        public int Errcode { get; set; }
        public string Errmsg { get; set; }
        public string Msgid { get; set; }
    }
}
