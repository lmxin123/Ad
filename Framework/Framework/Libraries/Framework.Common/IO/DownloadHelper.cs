using System;
using System.IO;
using System.Net;

namespace Framework.Common.IO
{
    public class DownloadHelper
    {

        /// <summary>
        /// Http下载文件，支持断点续传
        /// </summary>
        ///<param name="srcUrl">连接</param>
        /// <param name="savePath">保存路径</param>
        public static bool HttpDownloadFile(string srcUrl, string savePath)
        {
            bool ok = false;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            FileStream fileStream = null;
            try
            {
                // 设置参数
                request = WebRequest.Create(srcUrl) as HttpWebRequest;
                request.Method = "GET";
                request.Accept = "*/*";
                request.Timeout = 1000 * 1000 * 60;
                ServicePointManager.DefaultConnectionLimit = 100;
                //创建本地文件写入流
                if (File.Exists(savePath))
                {
                    fileStream = File.OpenWrite(savePath);
                    var offSet = fileStream.Length;
                    fileStream.Seek(offSet, SeekOrigin.Current);
                    //断点续传提供参数
                    request.AddRange("bytes", offSet, 41129896);//GetContentLength(srcUrl)
                }
                else
                {
                    fileStream = new FileStream(savePath, FileMode.Create);
                }
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                using (Stream contentStream = response.GetResponseStream())
                {
                    byte[] buffer = new byte[2048];
                    int size = contentStream.Read(buffer, 0, buffer.Length);
                    while (size > 0)
                    {
                        fileStream.Write(buffer, 0, size);
                        size = contentStream.Read(buffer, 0, buffer.Length);
                    }
                    contentStream.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                fileStream?.Close();
                response?.Close();
                request?.Abort();
            }
            return ok;
        }
        /// <summary>
        /// 获取下载文件的内容长度
        /// </summary>
        /// <param name="srcUrl">链接</param>
        /// <returns></returns>
        public static long GetContentLength(string srcUrl)
        {
            long len = 0;
            try
            {
                HttpWebRequest request = WebRequest.Create(srcUrl) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                len = response.ContentLength;
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return len;
        }
    }
}