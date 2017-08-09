using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Framework.Common.Mvc
{
    public class VideoResult : ActionResult
    {
        public string FullPath { get; set; }

        public VideoResult(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path 参数不能为null");

            FullPath = path;
        }

        string Name
        {
            get
            {
                string name = string.Empty;
                if (!string.IsNullOrEmpty(FullPath))
                {
                    var index = FullPath.LastIndexOf("/");
                    if (index == -1)
                        index = FullPath.LastIndexOf("\"");
                    if (index == -1)
                        index = FullPath.LastIndexOf("//");
                    if (index == -1)
                        index = FullPath.LastIndexOf("\\");

                    if (index > -1)
                        name = FullPath.Substring(index + 1);
                }
                return name;
            }
        }

        /// <summary> 
        /// 
        /// </summary> 
        /// <param name="context"></param> 
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + Name);
            var file = new FileInfo(FullPath);
            if (file.Exists)
            {
                var stream = file.OpenRead();
                var bytesinfile = new byte[stream.Length];
                stream.Read(bytesinfile, 0, (int)file.Length);
                context.HttpContext.Response.BinaryWrite(bytesinfile);
            }
        }
    }
}
