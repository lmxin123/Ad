using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace Framework.Common.Json
{
    /// <summary>
    /// 用于后台与前台Ajax请求的数据交互
    /// </summary>
    [JsonObject]
    public class GeneralResponseModel<T>
    {
        public GeneralResponseModel()
        {
            Success = true;
            Message = "success";
            MessageCode = (int)SuccessCode.Success;
        }
        public bool Success { get; set; }

        public int MessageCode { get; set; }
        /// <summary>
        /// 返回的数据对象
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 结果记录总数，一般用于分页
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 信息提示
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 验证错误信息集合
        /// </summary>
        /// <summary>
        /// 简单序列化
        /// </summary>
        [JsonIgnore]
        public bool SimpleJson { get; set; }
        /// <summary>
        /// 简单序列化时只序列化某些字段
        /// </summary>
        [JsonIgnore]
        public string JsonFields { get; set; }
    }

    public enum ErrorCode
    {
        UnknownError = 40000,

        ProcessError = 40001,

        DataBaseError = 41000,

        DataError = 410001,

        RequestParamError = 42000,

        InternalServiceError = 43000,

        RemoteServiceError = 44000,

        ModelValidateError = 45000,

        AuthFailError = 46000
    }

    public enum SuccessCode
    {
        Success = 20000,
    }
}
