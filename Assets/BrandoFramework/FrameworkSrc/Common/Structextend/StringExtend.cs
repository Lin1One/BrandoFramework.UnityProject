using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    public static class StringExtend
    {
        #region 字符串扩展

        /// <summary>
        /// 替换字符串中的双反斜线为单斜线。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ReplaceDoubleBackslash(this string source)
        {
            var result = source.Replace("\\", "/");
            return result;
        }

        /// <summary>
        /// 确保一个目录字符串以 / 结尾。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureDirEnd(this string path)
        {
            if (!path.EndsWith("/"))
            {
                path += "/";
            }

            return path;
        }

        /// <summary>
        /// 判断一个字符串是否包含大写字母
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool Containcapital(this string source)
        {
            return Regex.IsMatch(source, "[A-Z]");
        }

        public static bool IsChinese(this string CString)
        {
            return Regex.IsMatch(CString, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 字符串是否以数字开头
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsStartWithDigit(this string source)
        {
            return char.IsDigit(source.First());
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetFileName(this string source)
        {
            var result = Path.GetFileNameWithoutExtension(source);
            return result;
        }

        public static int ToInt32(this string str)
        {
            var result = Convert.ToInt32(str);
            return result;
        }

        public static bool ToBool(this string str)
        {
            return str != "0";
        }

        public static List<int> ToListInt(this string str, char split)
        {
            var array = str.Split(split);
            var result = array.Select(s => Convert.ToInt32(s)).ToList();
            return result;
        }

        public static List<float> ToListFloat(this string str, char split)
        {
            var array = str.Split(split);
            var result = array.Select(s => (float)Convert.ToDouble(s)).ToList();
            return result;
        }

        public static T AsEnum<T>(this string str)
        {
            var result = Enum.Parse(typeof(T), str);
            return (T)result;
        }

        public static string CutHead(this string source, int index)
        {
            var length = source.Length;
            var result = source.Substring(index, length - index);
            return result;
        }

        public static string CutEnd(this string source, int index)
        {
            var length = source.Length;
            var result = source.Substring(0, length - index);
            return result;
        }

        /// <summary>
        /// 将字符串转换为首字母大写的格式。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToStartLower(this string source)
        {
            var appender = new StringBuilder();
            appender.Clear();
            var length = source.Length;

            for (int i = 0; i < length; i++)
            {
                if (i == 0)
                {
                    var c = source[i].ToString().ToLower();
                    appender.Append(c);
                }
                else
                {
                    appender.Append(source[i].ToString());
                }
            }

            return appender.ToString();
        }

        #endregion

        #region 字符串列表

        public static List<string> RemoveNull(this string[] array)
        {
            var result = array.ToList().FindAll(l => !string.IsNullOrEmpty(l));
            return result;
        }

        #endregion

        #region 扩展所需基础结构

        public struct YuStringExendProvider
        {
            public string SourceString { get; }

            public StringBuilder Sb { get; private set; }

            public YuStringExendProvider(string source, StringBuilder sb)
            {
                SourceString = source;
                Sb = sb;
            }

            public void ReleaseSb() => Sb = null;
        }

        public static YuStringExendProvider Extend(this string source)
        {
            var extend = new YuStringExendProvider(source, OutSb);
            return extend;
        }

        #endregion

        #region 具体扩展API

        private static StringBuilder sb;
        private static StringBuilder OutSb => sb ?? (sb = new StringBuilder());

        public static bool IsDirectory(this YuStringExendProvider exend)
        {
            return Directory.Exists(exend.SourceString);
        }

        public static bool IsFile(this YuStringExendProvider extend)
        {
            return File.Exists(extend.SourceString);
        }

        public static string GetNowChineseDate(this YuStringExendProvider extend)
        {
            var nowStr = DateTime.Now.ToString("yyyyMMddHHmm");
            extend.Sb.Append(nowStr.Substring(0, 4));
            extend.Sb.Append("年");
            var monStr = nowStr.Substring(4, 2);
            var monNum = Convert.ToInt32(monStr);
            if (monNum < 10)
            {
                monStr = monStr.Substring(1, 1);
            }

            extend.Sb.Append(monStr);
            extend.Sb.Append("月");
            extend.Sb.Append(nowStr.Substring(6, 2));
            extend.Sb.Append("日");
            extend.Sb.Append(nowStr.Substring(8, 2));
            extend.Sb.Append("点");
            extend.Sb.Append(nowStr.Substring(10, 2));
            extend.Sb.Append("分");

            var result = extend.Sb.ToString();
            extend.Sb.Clear();
            return result;
        }

        #endregion
    }
}