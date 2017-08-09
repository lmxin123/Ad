using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Hdy.Media
{
    public class AppSetting
    {
        static NameValueCollection settings = ConfigurationManager.AppSettings;
        /// <summary>
        /// 营业执照存储路径
        /// </summary>
        public static string BusinessLicensePath
        {
            get
            {
                return settings["BusinessLicensePath"];
            }
        }
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
        /// 默认地区信息
        /// </summary>
        public static string[] DefaultLocation
        {
            get
            {
                string[] de = new string[]
                {
                    string.Empty,
                    string.Empty,
                    string.Empty
                };

                var d = settings["DefaultLocation"];
                if (!string.IsNullOrEmpty(d))
                {
                    string[] dArr = d.Split(',');
                    if (dArr.Length == 0)
                        dArr = d.Split('，');

                    if (dArr.Length >= 1)//为了避免报错，这里多加了几个判断
                        de[0] = dArr[0];
                    if (dArr.Length >= 2)
                        de[1] = dArr[1];
                    if (dArr.Length >= 3)
                        de[2] = dArr[2];
                }
                return de;
            }
        }
        
    }
}