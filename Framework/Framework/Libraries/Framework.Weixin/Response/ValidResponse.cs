namespace Framework.Weixin.Response
{
    public class ValidResponse : WeiXinResponse
    {
        public ValidResponse() : base() { }
        public ValidResponse(WeiXinContext weiXin) : base(weiXin) { }

        public string EchoStr { get; set; }

        public override string MsgType
        {
            get { return "valid"; }
        }

        public override string ToString()
        {
            return this.EchoStr;
        }
    }
}