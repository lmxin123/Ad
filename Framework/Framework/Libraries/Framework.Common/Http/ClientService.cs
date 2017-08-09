using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace Framework.Common.Http
{
    /// <summary>
    /// 网络请求服务
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="loggerFactory">日志工厂</param>
        public ClientService(ILoggerFactory loggerFactory)
        {
               _logger = loggerFactory.CreateLogger<ClientService>();
        }
        /// <summary>
        /// 请求头信息
        /// </summary>
        public IDictionary<string, string> Headers { get; set; } 
            = new Dictionary<string, string>();

        public async Task<TResponse> ExecuteAsync<TResponse>(
            RequestObject<TResponse> req, 
            RequestStringDataTypes reqDataType) 
            where TResponse : ResponseObject, new()
        {
            TResponse resp = new TResponse();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(req.RequestUrl);
                httpClient.Timeout = new TimeSpan(0, 0, 8);
                if (Headers != null && Headers.Count > 0)
                    foreach (var keyValue in Headers)
                        httpClient.DefaultRequestHeaders.Add(keyValue.Key, keyValue.Value);
                HttpResponseMessage httpRespMsg;
                if (req.Method == HttpMethod.Post)
                {
                    string reqData = req.GenerateRequestData(reqDataType),
                     mediaType = reqDataType == RequestStringDataTypes.Json ? "application/json" : "application/xml";

                    var postData = new StringContent(reqData, System.Text.Encoding.UTF8, mediaType);

                    _logger.LogInformation("网络请求：{0}参数：{1}", req.RequestUrl, reqData);

                    httpRespMsg = await httpClient.PostAsync(req.RequestUrl, postData);
                }
                else
                {
                    httpRespMsg = await httpClient.GetAsync(req.RequestUrl);
                }

                if (req.IsStreamResponse)
                {
                    resp.Stream = await httpRespMsg.Content.ReadAsStreamAsync();
                    _logger.LogInformation("返回结果：{0}", resp.Stream.ToString());

                }
                else
                {
                    resp.Body = await httpRespMsg.Content.ReadAsStringAsync();
                    _logger.LogInformation("返回结果：{0}", resp.Body);
                    resp = resp.GetResponseData<TResponse>(reqDataType);
                }
            }

            return resp;
        }

        public async Task<TResponse> ExecuteAsync<TResponse>(
            RequestObject<TResponse> req, 
            RequestStringDataTypes reqDataType,
            string password,
            string fileName) 
            where TResponse : ResponseObject, new()
        {
            TResponse resp = new TResponse();

            HttpWebRequest request = null;
            WebResponse response = null;
            Stream reqStream = null;

            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (req.RequestUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(req.RequestUrl);

                request.Method = "POST";
                request.Timeout = 10 * 1000;

                var reqData = req.GenerateRequestData(RequestStringDataTypes.Xml);
                _logger.LogInformation("网络请求：{0}参数：{1}", req.RequestUrl, reqData);

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = Encoding.UTF8.GetBytes(reqData);
                request.ContentLength = data.Length;

                X509Certificate2 cert = new X509Certificate2(fileName, password);
                request.ClientCertificates.Add(cert);

                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();

                //获取服务端返回
                response = await request.GetResponseAsync();

                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd().Trim();
                resp = Utility.FromXml<TResponse>(stringBody: result);
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                _logger.LogInformation("网络请求出错：{0}", e.Message);
                System.Threading.Thread.ResetAbort();
            }
            catch (WebException e)
            {
                _logger.LogInformation("网络请求出错：{0}", e.ToString());

                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    _logger.LogInformation("StatusCode：{0}", ((HttpWebResponse)e.Response).StatusCode);
                    _logger.LogInformation("StatusDescription : " + ((HttpWebResponse)e.Response).StatusDescription);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("网络请求出错：{0}", e.ToString());
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return resp;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

    }
}
