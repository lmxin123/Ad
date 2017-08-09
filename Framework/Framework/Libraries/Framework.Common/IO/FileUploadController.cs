using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Framework.Common.IO
{
    public class FileController : ApiController
    {
        [HttpPost]
        public async Task<IEnumerable<string>> Post()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                string fullPath = HttpContext.Current.Server.MapPath("~/Content");
                CustomMultipartFormDataStreamProvider streamProvider = new CustomMultipartFormDataStreamProvider(fullPath);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t =>
                {
                  //  if (t.IsFaulted || t.IsCanceled)
                   //     throw new HttpResponseException(HttpStatusCode.InternalServerError);

                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        return "File saved as " + info.FullName + " (" + info.Length + ")";
                    });
                    return fileInfo;

                });
                return await task;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
        }
    }
    public class CustomMultipartFormDataStreamProvider : System.Net.Http.MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        {

        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            string fileName;
            if (!string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName))
            {
                fileName = headers.ContentDisposition.FileName;
            }
            else
            {
                fileName = Guid.NewGuid().ToString() + ".data";
            }
            return fileName.Replace("\"", string.Empty);
        }
    }

}
