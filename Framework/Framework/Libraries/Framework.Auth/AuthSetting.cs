using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Auth
{
    /// <summary>
    /// 授权相关配置
    /// </summary>
    public class AuthSetting
    {
        static NameValueCollection settings = ConfigurationManager.AppSettings;

        public static string Administrator
        {
            get
            {
                return settings["Administrator"] ?? "sa";
            }
        }

        public static string AdminPassowrd
        {
            get
            {
                return settings["AdminPassowrd"] ?? "123456";
            }
        }

        public static string AdminRoleName
        {
            get
            {
                return settings["AdminRoleName"] ?? "Admin";
            }
        }

        public static string LoginUrl
        {
            get
            {
                return settings["LoginUrl"] ?? "/user/login";
            }
        }

        public static int ExpireTimeSpanInMinutes
        {
            get
            {
                int v = 0;
                int.TryParse(settings["ExpireTimeSpanInMinutes"], out v);
                return v == 0 ? 120 : v;
            }
        }
    }
}
