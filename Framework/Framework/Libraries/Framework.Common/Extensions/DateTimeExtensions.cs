using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Framework.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToChinaTime(this DateTime value)
        {
            TimeZoneInfo zone = TimeZoneInfo.GetSystemTimeZones().First(Z => Z.Id == "China Standard Time");
            return TimeZoneInfo.ConvertTime(value, zone);
        }

        public static DateTime ToLocalTime(this DateTime value, bool isChinaTime)
        {
            TimeZoneInfo zone = TimeZoneInfo.GetSystemTimeZones().First(Z => Z.Id == "China Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(value, zone).ToLocalTime();
        }

        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区
        /// 自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <typeparam name="T">string,int,long</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToTimeStamp(this DateTime value)
        {
            TimeSpan ts = value - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalSeconds;
        }

        public static string ToDayOfWeek(this DateTime value, bool isShortName = false)
        {
            string week = isShortName ? "周" : "星期";

            switch (value.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    week += "日";
                    break;
                case DayOfWeek.Monday:
                    week += "一";
                    break;
                case DayOfWeek.Tuesday:
                    week += "二";
                    break;
                case DayOfWeek.Wednesday:
                    week += "三";
                    break;
                case DayOfWeek.Thursday:
                    week += "四";
                    break;
                case DayOfWeek.Friday:
                    week += "五";
                    break;
                case DayOfWeek.Saturday:
                    week += "六";
                    break;
                default:
                    week = "星期有误";
                    break;
            }
            return week;
        }
    }
}
