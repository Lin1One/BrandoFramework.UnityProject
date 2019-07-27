using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个字符串数组，将字母异位词组合在一起。
        // 字母异位词指字母相同，但排列不同的字符串。

        // 示例:
        // 输入: ["eat", "tea", "tan", "ate", "nat", "bat"],
        // 输出:
        // [
        //   ["ate","eat","tea"],
        //   ["nat","tan"],
        //   ["bat"]
        // ]

        //排序后作为的 Key 保存
        public IList<IList<string>> GroupAnagrams(string[] strs) 
        {
            if (strs.Length == 0) 
                return null;
            Dictionary<string, IList<string>> ans = 
                new Dictionary<string, IList<string>>();
            foreach( string s in strs) 
            {
                char[] ca = s.ToCharArray();
                Array.Sort(ca);                     //关键
                string key = new string(ca);
                if (!ans.ContainsKey(key)) 
                {
                    ans.Add(key, new List<string>());
                }
                ans[key].Add(s);
            }
            return new List<IList<string>>(ans.Values);
        }

        // public IList<IList<string>> GroupAnagrams2(string[] strs) 
        // {
        //     if (strs.Length == 0) return null;
        //     Dictionary<string, IList<string>> ans = 
        //         new Dictionary<string, IList<string>>();
        //     int[] count = new int[26];
        //     foreach (string s in strs) 
        //     {
        //         for(int i = 0; i < count.Length;i++)
        //         {
        //             count[i] = 0;
        //         }
        //         foreach (char c in s.ToCharArray())
        //         {
        //             count[c - 'a']++;
        //         }
        //         StringBuilder sb = new StringBuilder();
        //         for (int i = 0; i < 26; i++)
        //         {
        //             sb.Append('#');
        //             sb.Append(count[i]);
        //         }
        //         string key = sb.ToString();
        //         if (!ans.ContainsKey(key)) 
        //             ans.Add(key, new ArrayList());
        //         ans[key].Add(s);
        //     }
        //     return new List<IList<string>>(ans.Values);
        // }
    }
}


