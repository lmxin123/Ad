namespace Framework.Weixin.Request
{
    public class EventRequest : WeiXinRequest
    {
        public EventRequest(WeiXinContext weiXin) : base(weiXin) { }
        public string Event
        {
            get
            {
                return GetParameter("Event");
            }
        }
        public string EventKey
        {
            get
            {
                return GetParameter("EventKey");
            }
        }
    }
}