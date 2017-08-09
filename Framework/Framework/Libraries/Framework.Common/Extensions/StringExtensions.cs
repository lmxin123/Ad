using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Xml;
using System.Collections.Generic;

namespace Framework.Common.Extensions
{
    /// <summary>
    /// StringUtil 的摘要说明。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 获取字符串长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetLength(this string value)
        {
            byte[] mybyte = Encoding.UTF8.GetBytes(value);
            return mybyte.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException">数组为空</exception>
        /// <exception cref="ArgumentNullException">目标字符不为能空</exception>
        /// <returns></returns>
        public static bool StartsWith(this string[] array, string value)
        {
            if (array.Length == 0)
                throw new ArgumentException("数组为空！");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("目标字符不为能空！");

            foreach (string str in array)
            {
                if (value.ToLower().StartsWith(str.ToLower())) return true;
            }
            return false;
        }

        /// <summary>
        /// 清除字串中的HTML代码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ClearHtmlCode(this string value)
        {
            return value.ClearHtmlCode(false);
        }

        /// <summary>
        /// 清除字串中的HTML代码，是否保留部份简单标签
        /// </summary>
        /// <param name="value"></param>
        /// <param name="retainSimple"></param>
        /// <returns></returns>
        public static string ClearHtmlCode(this string value, bool retainSimple)
        {
            if (!retainSimple)
                value = Regex.Replace(value, "<br[^>]*?>|<p[^>]*?>", System.Environment.NewLine, (RegexOptions)25);
            if (retainSimple)
                value = Regex.Replace(value, @"<(?!(img|/?strong|/?b|/?br|/?p|/?embed)\W*).*?>", string.Empty, (RegexOptions)25);
            else
                value = Regex.Replace(value, "<.*?>", string.Empty, (RegexOptions)25);
            value = Regex.Replace(value, "&nbsp;", " ", (RegexOptions)25);
            return value;
        }

        /// <summary>
        /// 清理Script脚本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ClearScriptCode(this string value)
        {
            value = Regex.Replace(value, "<script.*?>.*?</script>", string.Empty, (RegexOptions)25);
            value = Regex.Replace(value, @"on\w*=("".*?""|'.*?'|\w*)", string.Empty, (RegexOptions)25);
            return value;
        }

        /// <summary>
        /// 清理A标签（链接）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ClearATag(this string value)
        {
            value = Regex.Replace(value, "<a[^>]*?>(.*?)</a>", "$1", (RegexOptions)25);
            return value;
        }

        /// <summary>
        /// 清理Img标签（图片）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ClearImgTag(this string value)
        {
            value = Regex.Replace(value, "<img [^<]*>", string.Empty, (RegexOptions)25);
            return value;
        }

        /// <summary>
        /// 对 字符串 进行 HTML 编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string value)
        {
            value = value.Replace("  ", "&nbsp;&nbsp;");
            value = Regex.Replace(value, "\r\n", "<br />");
            value = Regex.Replace(value, "\r", "<br />");
            value = Regex.Replace(value, "\n", "<br />");
            return value;
        }
        /// <summary>
        /// 这个MD5支持中文
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string MD5(this string value)
        {
            byte[] b = Encoding.Default.GetBytes(value);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        public static string SHA1Encrypt(this string value)
        {
            byte[] strRes = Encoding.UTF8.GetBytes(value);
            strRes = SHA1.Create().ComputeHash(strRes);
            StringBuilder sb = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                sb.AppendFormat("{0:x2}", iByte);
            }
            return sb.ToString();
        }
        private static byte[] AESKeys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };
        public static string AESKey = "";

        /// <summary>
        /// 将int[]转换成string[]
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string[] IntArrayToStringArray(int[] array)
        {
            string[] strArr = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                strArr[i] = array[i].ToString();
            }

            return strArr;
        }

        /// <summary>
        /// 将 string[] 转换成 int[]
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static int[] StringArrayToIntArray(string[] array)
        {
            int[] array1 = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array1[i] = Convert.ToInt32(array[i]);
            }
            return array1;
        }

        /// <summary>
        /// 清除字串中的HTML代码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>清除HTML代码之后的字符串</returns>
        public static string ClearHtmlCode(object input)
        {
            return ClearHtmlCode(input, false);
        }

        /// <summary>
        /// 清除字串中的HTML代码，是否保留部份标签
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>清除HTML代码之后的字符串</returns>
        public static string ClearHtmlCode(object input, bool retainSimple)
        {
            string str = Convert.ToString(input);
            if (!retainSimple)
                str = Regex.Replace(str, "<br[^>]*?>|<p[^>]*?>", System.Environment.NewLine, (RegexOptions)25);
            if (retainSimple)
                str = Regex.Replace(str, @"<(?!(img|/?strong|/?b|/?br|/?p|/?embed)\W*).*?>", string.Empty, (RegexOptions)25);
            else
                str = Regex.Replace(str, "<.*?>", string.Empty, (RegexOptions)25);
            str = Regex.Replace(str, "&nbsp;", " ", (RegexOptions)25);
            return str;
        }


        /// <summary>
        /// 清理A标签（链接）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ClearLinks(string input)
        {
            input = Regex.Replace(input, "<a[^>]*?>(.*?)</a>", "$1", (RegexOptions)25);
            return input;
        }

        /// <summary>
        /// 清理Img标签（图片）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ClearImgs(string input)
        {
            input = Regex.Replace(input, "<img [^<]*>", string.Empty, (RegexOptions)25);
            return input;
        }

        /// <summary>
        /// 以中文形式显示日期
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime datetime)
        {
            DateTime today = DateTime.Now;

            TimeSpan ts = today.Subtract(datetime);
            if (ts.TotalMinutes < 60)
            {
                if (ts.TotalMinutes < 1)
                    return "不到一分钟前";
                else
                    return string.Format("{0}分钟前", ts.Minutes);
            }
            if (ts.TotalHours < 5)
            {
                if (ts.TotalHours - ts.Hours < 0.5)
                    return string.Format("{0}个小时前", ts.Hours);
                else
                    return string.Format("{0}个半小时前", ts.Hours);
            }

            ts = today.Date.Subtract(datetime.Date);

            if (ts.Days == 0)
            {
                return string.Format("今天{0}点{1}分", datetime.Hour, datetime.Minute);
            }
            if (ts.Days == 1)
            {
                return string.Format("昨天{0}点{1}分", datetime.Hour, datetime.Minute);
            }
            if (ts.Days == 2)
            {
                return string.Format("前天{0}点{1}分", datetime.Hour, datetime.Minute);
            }
            //if (ts.Days == 3)
            //{
            //    return string.Format("大前天{0}点{1}分", dt.Hour, dt.Minute);
            //}
            return datetime.ToString("yyyy-MM-dd hh:mm");
        }

    }
}
