using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Common.DataStruct
{
    public static class YuStringExtend
    {
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

    /// <summary>
    /// 常用扩展功能函数集合类。
    /// </summary>
    public static class YuCommonExtend
    {
        #region 集合

        #region 字典

        /// <summary>
        /// 合并两个字典
        /// </summary>
        /// <typeparam name="TKey">字典键泛型类型。</typeparam>
        /// <typeparam name="TValue">字典值泛型类型。</typeparam>
        /// <param name="left">字典一。</param>
        /// <param name="right">字典二。</param>
        /// <returns></returns>
        public static void Combin<TKey, TValue>(this IDictionary<TKey, TValue> left, Dictionary<TKey, TValue> right)
        {
            foreach (var keyValuePair in right)
            {
                if (left.ContainsKey(keyValuePair.Key))
                {
                    Debug.LogError($"Key:  {keyValuePair.Key}当前已存在，请检查！");
                }
                else
                {
                    left.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        /// <summary>
        /// 对给定字典中的每一对键值对执行给定的委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="del"></param>
        public static void ForEach<T, V>(this Dictionary<T, V> dictionary, Action<T, V> del)
        {
            foreach (var kv in dictionary)
            {
                del?.Invoke(kv.Key, kv.Value);
            }
        }

        #endregion

        #region 列表

        public static bool HasRepeat(this string[] array)
        {
            return array.ToList().HasRepeat();
        }

        private static bool HasRepeat(this List<string> list)
        {
            return HasRepeat(new HashSet<string>(), list).Count > 0;
        }

        public static List<string> FindRepeat(this IEnumerable<string> array)
        {
            return HasRepeat(new HashSet<string>(), array.ToList());
        }

        private static List<string> HasRepeat(HashSet<string> set, List<string> list)
        {
            var result = new List<string>();

            foreach (var s in list)
            {
                if (!set.Contains(s))
                {
                    set.Add(s);
                }
                else
                {
                    result.Add(s);
                }
            }

            return result;
        }

        public static Stack<T> ToStack<T>(this List<T> list)
        {
            list.Reverse();
            var stack = new Stack<T>();
            foreach (var element in list)
            {
                stack.Push(element);
            }

            return stack;
        }

        #endregion

        #endregion

        #region 类型转换关键字函数

        /// <summary>
        /// 尝试将一个对象转型为指定类型。
        /// </summary>
        /// <typeparam name="T">要转换的目标类型。</typeparam>
        /// <param name="t">实例对象。</param>
        /// <returns></returns>
        public static T As<T>(this object t) where T : class
        {
            var instance = t as T;
            return instance;
        }

        #endregion

        #region 基础语法扩展

        public static void IfElse(bool condition, Action trueAction, Action falseAction)
        {
            if (condition)
            {
                trueAction();
            }
            else
            {
                falseAction();
            }
        }

        public static void IfElse<T>(bool condition, T data, Action<T> trueAction, Action<T> falseAction)
        {
            if (condition)
            {
                trueAction(data);
            }
            else
            {
                falseAction(data);
            }
        }

        public static void IfElse<T1, T2>(bool condition, T1 data1,
            T2 data2, Action<T1> trueAction, Action<T2> falseAction)
        {
            if (condition)
            {
                trueAction(data1);
            }
            else
            {
                falseAction(data2);
            }
        }

        #endregion

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

        public static string FileName(this string source)
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
            var result = array.Select(s => (float) Convert.ToDouble(s)).ToList();
            return result;
        }

        public static T AsEnum<T>(this string str)
        {
            var result = Enum.Parse(typeof(T), str);
            return (T) result;
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

        #region 字符扩展

        #endregion

        #region 字符串列表

        public static List<string> RemoveNull(this string[] array)
        {
            var result = array.ToList().FindAll(l => !string.IsNullOrEmpty(l));
            return result;
        }

        #endregion

        #region StringBuilder

        public static void AppendNoteHead(this StringBuilder sb, string name, string email)
        {
            //sb.AppendLine("#region Head");
            //sb.AppendLine();
            //sb.AppendLine($"// Author:        {name}");
            //sb.AppendLine($"// CreateDate:    {YuDateUtility.DateAndTimeForNow}");
            //sb.AppendLine($"// Email:         {email}");
            //sb.AppendLine();
            //sb.AppendLine("#endregion");
            //sb.AppendLine();
        }

        #endregion

        #region 浮点值

        /// <summary>
        /// 比较给定的两个浮点数是否近似相等。
        /// 默认的误差值在0.001。
        /// </summary>
        /// <param name="lf"></param>
        /// <param name="rf"></param>
        /// <returns></returns>
        public static bool Equal(this float lf, float rf)
        {
            var result = Math.Abs(lf - rf) < 0.001;
            return result;
        }

        public static bool Equal(this double lf, double rf)
        {
            var result = Math.Abs(lf - rf) < 0.001;
            return result;
        }

        /// <summary>
        /// 获得一个颜色分量的整型表示（255整数颜色值）
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static int ColorInt(this float v)
        {
            var fv = v * 255;
            var result = (int) fv;
            return result;
        }

        #endregion

        #region 网络通信

        private static readonly byte[] bigPointBytes = new byte[4];


        /// <summary>
        /// Protobuf 为小端字节序
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetHeadBytes(this byte[] bytes)
        {
            try
            {
                for (var i = 3; i >= 0; i--)
                {
                    bigPointBytes[i] = bytes[3 - i];
                }

                return bigPointBytes;
            }
            catch (Exception e)
            {
                Debug.LogError("GetBigBytes Error:" + e.Message);
                return null;
            }
        }

        #endregion

        #region Type

        public static string TypeId(this object obj) => obj.GetType().Name;

        #endregion
    }
}