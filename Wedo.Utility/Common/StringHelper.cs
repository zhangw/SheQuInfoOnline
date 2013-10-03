using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using Wedo.Utility.Log;

namespace System
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringHelper
    {
       /// <summary>
       /// 随机获取字符串
       /// </summary>
       /// <param name="maxSize"></param>
       /// <param name="random_range"></param>
       /// <returns></returns>
        public static string RandomString(int maxSize, string random_range = null)
        {
            string RANDOM_RANGE = random_range ?? "1234567890";
            Random rand = new Random( unchecked(((int)DateTime.Now.Ticks)));
            StringBuilder sb = new StringBuilder(maxSize);
            for (int i = 0; i < maxSize; i++)
            {
                sb.Append(RANDOM_RANGE[rand.Next(RANDOM_RANGE.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainMessage"></param>
        /// <returns></returns>
        public static string HashPassword(string plainMessage)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainMessage);
            using (HashAlgorithm sha = new SHA256Managed())
            {
                byte[] encryptedBytes = sha.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(sha.Hash);
            }
        }

        /// <summary>
        /// 根据判断smallStr中的所有单词是否包含在bigStr中
        /// </summary>
        /// <param name="bigStr">长的字符</param>
        /// <param name="smallStr">短的字符</param>
        /// <param name="isCapitalization">区分大小写，默认不区分</param>
        /// <returns>如果短字符中的所有单词都在长</returns>
        public static bool CheckContainsByWord(string bigStr, string smallStr, bool isCapitalization = false)
        {
            if (string.IsNullOrWhiteSpace(bigStr) || string.IsNullOrWhiteSpace(smallStr))
                return false;

            if (isCapitalization == false)
            {
                bigStr = bigStr.ToLower();
                smallStr = smallStr.ToLower();
            }

            List<string> bigStrs = bigStr.Split(' ').ToList();
            List<string> smallStrs = smallStr.Split(' ').ToList();

            int smallCount = smallStrs.Count;

            int count = 0;
            foreach (var item in smallStrs)
            {
                if (bigStrs.Contains(item))
                    count++;
            }

            return count == smallCount;
        }

        /// <summary>
        /// remove more spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveMoreSpaces(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return source;
            var strs = source.Split(' ').ToList();
            strs.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            return string.Join(" ", strs).Trim();
        }


        /// <summary>
        /// 确认字符是否包含，如果为中文，直接判断是否包含即可；
        /// 如果为英文，则需要根据单词区分
        /// </summary>
        /// <param name="bigStr"></param>
        /// <param name="smallStr"></param>
        /// <param name="isCapitalization"></param>
        /// <returns></returns>
        public static bool Contains(string bigStr, string smallStr, bool isEnglish, bool isCapitalization = false)
        {
            if (string.IsNullOrWhiteSpace(bigStr) || string.IsNullOrWhiteSpace(smallStr))
                return false;

            bigStr = bigStr.Trim();
            smallStr = smallStr.Trim();

            if (isCapitalization == false)
            {
                bigStr = bigStr.ToLower();
                smallStr = smallStr.ToLower();
            }

            if (!bigStr.Contains(' ') && !smallStr.Contains(' '))
            {
                if (isEnglish)
                    return bigStr == smallStr;
                else
                    return bigStr.Contains(smallStr);
            }

            string formatBig = RemoveMoreSpaces(bigStr) + " ";
            string formatSmall = RemoveMoreSpaces(smallStr) + " ";
            return formatBig.Contains(formatSmall);
        }

         /// <summary>
        /// 将字符串转为值类型，如果没有得到或者错误返回为空
        /// </summary>
        /// <typeparam name="T">指定值类型</typeparam>
        /// <param name="str">传入字符串</param>
        /// <returns>可空值</returns>
        public static Nullable<T> TryParseTo<T>(this string str) where T : struct
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    MethodInfo method = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
                    if (method != null)
                    {
                        T result = (T)method.Invoke(null, new string[] { str });
                        return result;
                    }
                }
            }
            catch
            {
               
            }
            return null;
        }

        public static Nullable<T> ChangeType<T>(this object obj) where T:struct {
            if (obj == null)
                return null;
            if (obj is T)
                return (T)obj;
            else
            {
               return obj.ToString().TryParseTo<T>();
            }
        }

        /// <summary>
        /// 是否为空或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 是否不为空或者空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// yyyy-MM-dd hh:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToCommonStr(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd hh:mm:ss");
        }

        /// <summary>
        /// yyyyMMddhhmmss
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToCommonStr1(this DateTime time)
        {
            return time.ToString("yyyyMMddhhmmss");
        }

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToCommonStr2(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        public static List<T> ParseToList<T>(this string str,char split=',') where T:struct
        {
            List<T> list = new List<T>();
            if (str.IsNotNullOrWhiteSpace())
            {
                var arr = str.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in arr)
                {
                    var val = item.TryParseTo<T>();
                    if (val.HasValue)
                        list.Add(val.Value);
                }
            }
            return list;
        }
    }
}
