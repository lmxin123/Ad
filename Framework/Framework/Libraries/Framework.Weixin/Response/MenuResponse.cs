
using Framework.Common.Http;

namespace Framework.Weixin.Response
{
    /// <summary>
    /// 微信自定义菜单查询，创建，删除统一返回类
    /// </summary>
    public class MenuResponse : ResponseObject
    {
        public int errcode { get; set; }

        public string errmsg { get; set; }
    }
}
