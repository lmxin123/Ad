namespace Framework.Weixin.Response
{
    public class TextResponse : WeiXinResponse
    {
        public override string MsgType
        {
            get { return "text"; }
        }

        public string Content { get; set; }

        public TextResponse() : base() { }

        public TextResponse(WeiXinContext weiXin) : base(weiXin) { }
    }
}