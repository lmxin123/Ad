using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Hdy.Media.Service
{
    public class AppSetting
    {
        static NameValueCollection settings = ConfigurationManager.AppSettings;
        /// <summary>
        /// 图片广告存储路径
        /// </summary>
        public static string AdImagePath
        {
            get
            {
                return settings["AdImagePath"];
            }
        }
        /// <summary>
        /// 视频广告存储路径
        /// </summary>
        public static string AdVideoPath
        {
            get
            {
                return settings["AdVideoPath"];
            }
        }
        /// <summary>
        /// 压缩包存放路径
        /// </summary>
        public static string ZipFileStoragePath
        {
            get
            {
                return string.Concat(settings["ZipFileStoragePath"], DateTime.Now.ToString("yyyy-MM"), "\\");
            }
        }

        public static string ZipFileRequestPath
        {
            get
            {
                return string.Concat("http://", HttpContext.Current.Request.Url.Authority, "/Service/dowload?deviceCode={0}&id={1}");
            }
        }
    }
}