namespace Framework.Weixin.Request
{
    public class TextRequest : WeiXinRequest
    {
        public TextRequest(WeiXinContext weiXin) : base(weiXin) { }
        public string Content
        {
            get
            {
                return GetParameter("Content");
            }
        }
    }
}