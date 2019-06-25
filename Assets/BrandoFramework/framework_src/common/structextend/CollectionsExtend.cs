using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.DataStruct
{
    /// <summary>
    /// 常用扩展功能函数集合类。
    /// </summary>
    public static class CollectionsExtend
    {
        #region 字典

        /// <summary>
        /// 合并两个字典
        /// </summary>
        /// <typeparam name="TKey">字典键泛型类型。</typeparam>
        /// <typeparam name="TValue">字典值泛型类型。</typeparam>
        /// <param name="left">字典一。</param>
        /// <param name="right">字典二。</param>
        /// <returns></returns>
        public static void Combin<TKey, TValue>(this IDictionary<TKey, TValue> left, 
            Dictionary<TKey, TValue> right)
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

    }
}