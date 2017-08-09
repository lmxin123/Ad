using System;
using System.Net.Http;

namespace Framework.Common.Extensions
{
    /// <summary>
    /// HttpContext类的扩展
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取服务端ip地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIpAddress(this HttpContent context)
        {
            if (null == context)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return string.Empty;
            //var connection = context..Features.Get<IHttpConnectionFeature>();
           // return connection?.RemoteIpAddress?.ToString();
        }
    }
}
