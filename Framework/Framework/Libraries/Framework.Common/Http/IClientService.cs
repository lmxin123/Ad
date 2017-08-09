using System.Threading.Tasks;

namespace Framework.Common.Http
{
    /// <summary>
    /// 提供普通网络请求和带证书的网络请求方法
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// 网络请求，异步执行
        /// </summary>
        /// <typeparam name="TResponseType">请求返回类型</typeparam>
        /// <param name="req">请求类型</param>
        /// <param name="reqDataType">请求的数据类型</param>
        /// <returns>TResponse类型数据</returns>
        Task<TResponse> ExecuteAsync<TResponse>(
            RequestObject<TResponse> req, 
            RequestStringDataTypes reqDataType) 
            where TResponse : ResponseObject, new();
        /// <summary>
        /// 带证书的网络请求，异步执行
        /// </summary>
        /// <typeparam name="TResponse">请求返回类型</typeparam>
        /// <param name="req">请求类型</param>
        /// <param name="reqDataType">请求的数据类型</param>
        /// <param name="password">证书密码</param>
        /// <param name="fileName">证书完整名称，包含路径</param>
        /// <returns>TResponse类型数据</returns>
        Task<TResponse> ExecuteAsync<TResponse>(
            RequestObject<TResponse> req,
            RequestStringDataTypes reqDataType,
            string password,
            string fileName)
            where TResponse : ResponseObject, new();
    }
}
