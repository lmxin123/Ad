using Framework.Common.Http;

namespace Framework.Weixin.Response.Pay
{
    public abstract class WeixinPayResponse : ResponseObject
    {
        /// <summary>
        /// 随机字符串	
        /// </summary>
        public virtual string nonce_str { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public virtual string sign { get; set; }
        /// <summary>
        /// 业务结果	
        /// </summary>
        public virtual string return_code { get; set; }
        /// <summary>
        /// 返回信息	
        /// </summary>
        public virtual string return_msg { get; set; } = "";
        /// <summary>
        /// 返回状态码	
        /// </summary>
        public virtual string result_code { get; set; } = "";
        /// <summary>
        /// 错误代码	
        /// </summary>
        public virtual string err_code { get; set; } = "";
        /// <summary>
        /// 错误代码描述	
        /// </summary>
        public virtual string err_code_des { get; set; } = "";
    }
}
