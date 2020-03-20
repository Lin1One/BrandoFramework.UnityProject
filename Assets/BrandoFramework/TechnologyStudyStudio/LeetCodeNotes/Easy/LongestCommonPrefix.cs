#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
namespace Study.LeetCode
{
    public partial class Solution
    {
        // 14. 最长公共前缀
        // 编写一个函数来查找字符串数组中的最长公共前缀。
        // 如果不存在公共前缀，返回空字符串 ""。

        // 示例 1:
        // 输入: ["flower","flow","flight"]
        // 输出: "fl"

        // 示例 2:
        // 输入: ["dog","racecar","car"]
        // 输出: ""
        // 解释: 输入不存在公共前缀。

        // 说明:
        // 所有输入只包含小写字母 a-z 。

        public string LongestCommonPrefix(string[] strs) 
        {
            if (strs.Length == 0) return "";
            string prefix = strs[0];
            for (int i = 1; i < strs.Length; i++)
            {
                while (strs[i].IndexOf(prefix) != 0) 
                {
                    prefix = prefix.Substring(0, prefix.Length - 1);
                    if (prefix.Length == 0) 
                    return "";
                }        
            }
            return prefix;
        }

        public string longestCommonPrefix(string[] strs) 
        {
            if (strs == null || strs.Length == 0) return "";
            for (int i = 0; i < strs[0].Length; i++)
            {
                char c = strs[0][i];
                for (int j = 1; j < strs.Length; j ++) 
                {
                    if (i == strs[j].Length || strs[j][i] != c)
                        return strs[0].Substring(0, i);             
                }
            }
            return strs[0];
        }

        //分治法
        public string LongestCommonPrefix1(string[] strs) 
        {
            if (strs == null || strs.Length == 0) return "";    
                return longestCommonPrefix(strs, 0 , strs.Length - 1);
        }

        private string longestCommonPrefix(string[] strs, int l, int r) 
        {
            if (l == r) 
            {
                return strs[l];
            }
            else 
            {
                int mid = (l + r)/2;
                string lcpLeft =   longestCommonPrefix(strs, l , mid);
                string lcpRight =  longestCommonPrefix(strs, mid + 1,r);
                return commonPrefix(lcpLeft, lcpRight);
            }
        }

        string commonPrefix(string left,string right) 
        {
            int min = Math.Min(left.Length, right.Length);       
            for (int i = 0; i < min; i++) 
            {
                if (left[i] != right[i])
                {
                    return left.Substring(0, i);
                }  
            }
            return left.Substring(0, min);
        }

        //二分查找
        public string LongestCommonPrefix2(string[] strs) 
        {
            if (strs == null || strs.Length == 0)
                return "";
            int minLen = int.MaxValue;
            foreach (string str in strs)
                minLen = Math.Min(minLen, str.Length);
            int low = 1;
            int high = minLen;
            while (low <= high) 
            {
                int middle = (low + high) / 2;
                if (isCommonPrefix(strs, middle))
                    low = middle + 1;
                else
                    high = middle - 1;
            }
            return strs[0].Substring(0, (low + high) / 2);
        }

        private bool isCommonPrefix(string[] strs, int len)
        {
            string str1 = strs[0].Substring(0,len);
            for (int i = 1; i < strs.Length; i++)
                if (!strs[i].StartsWith(str1))
                    return false;
            return true;
        }




    }
}


