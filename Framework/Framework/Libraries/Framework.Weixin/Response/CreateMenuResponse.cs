using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    public class CreateMenuResponse : ResponseObject
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }
    }
}
