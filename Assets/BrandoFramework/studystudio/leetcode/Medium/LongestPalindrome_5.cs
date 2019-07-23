using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        public string LongestPalindrome_Test(string s) 
        {
            return null;
        }
        // 给定一个字符串 s，找到 s 中最长的回文子串。你可以假设 s 的最大长度为 1000。
        
        // 示例 1：
        // 输入: "babad"
        // 输出: "bab"
        // 注意: "aba" 也是一个有效答案。

        // 示例 2：
        // 输入: "cbbd"
        // 输出: "bb"







        //我们可以每次循环选择一个中心，
        //进行左右扩展，判断左右字符是否相等即可。
        //由于存在奇数的字符串和偶数的字符串，
        //所以我们需要从一个字符开始扩展，或者从两个字符之间开始扩展，
        //所以总共有 n+n-1 个中心。


        //中心扩展
        public string longestPalindrome3(string s) 
        {
            if(string.IsNullOrEmpty(s))
            {
                return "";
            }
            int start = 0,end = 0;
            for(int i = 0;i<s.Length;i++)
            {
                int len1 = FindPalindromeFromStrCenter(s,i,i);      //奇数回文
                int len2 = FindPalindromeFromStrCenter(s,i,i+ 1);   //偶数回文
                int len = Math.Max(len1,len2);
                if(len > end - start)
                {
                    start = i - (len - 1) / 2;
                    end = i + len / 2;
                }
            }
            return s.Substring(start,end + 1 - start);
        }

        private int FindPalindromeFromStrCenter(string s,int left,int right)
        {
            int L = left;
            int R = right;
            while(L >= 0 && R < s.Length && s[L] == s[R])
            {
                L--;
                R++;
            }
            return R - L - 1;
        }



        //  方法一：最长公共子串
        // 常见错误

        // 有些人会忍不住提出一个快速的解决方案，不幸的是
        //这个解决方案有缺陷(但是可以很容易地纠正)：

        // 反转 S，使之变成 S'
        // 找到 S和 S' 之间最长的公共子串，这也必然是最长的回文子串。

        // 这似乎是可行的，让我们看看下面的一些例子。

        // 例如，S = “caba” , S' = “abac”
        // S以及 S'之间的最长公共子串为 “aba”，恰恰是答案。

        // 让我们尝试一下这个例子：S = “abacdfgdcaba”, S' = “abacdgfdcaba”
        // S 以及 S' 之间的最长公共子串为 “abacd”。显然，这不是回文。

        // 算法

        // 我们可以看到，当 S 的其他部分中存在非回文子串的反向副本时，
        // 最长公共子串法就会失败。为了纠正这一点，
        //每当我们找到最长的公共子串的候选项时，
        //都需要检查子串的索引是否与反向子串的原始索引相同。
        //如果相同，那么我们尝试更新目前为止找到的最长回文子串；
        //如果不是，我们就跳过这个候选项并继续寻找下一个候选。

        // 这给我们提供了一个复杂度为 O(n^2)
        //  动态规划解法，它将占用 O(n^2)
        //  ) 的空间（可以改进为使用 O(n)O(n) 的空间）。

        //反转求公共子串，最长公共子串法
        public string LongestPalindrome1(string s) 
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            string reverStr = new string(arr);
            StringBuilder strBuilder = new StringBuilder();
            for(var i = 0;i < s.Length;i++)
            {
                //if()
            }
            return null;
        }



        // 关于求最长公共子串（不是公共子序列），有很多方法，这里用动态规划的方法， 
        //整体思想就是，申请一个二维的数组初始化为 0，然后判断对应的字符是否相等，相等的话

        // arr [ i ][ j ] = arr [ i - 1 ][ j - 1] + 1 。

        // 当 i = 0 或者 j = 0 的时候单独分析，字符相等的话 arr [ i ][ j ] 就赋为 1 。

        // arr [ i ][ j ] 保存的就是公共子串的长度。

        public String longestPalindrome2(String s) 
        {
            // if (s.equals(""))
            //     return "";
            // String origin = s;
            // String reverse = new StringBuffer(s).reverse().toString(); //字符串倒置


            // int length = s.length();
            // int[][] arr = new int[length,length];
            // int maxLen = 0;
            // int maxEnd = 0;
            // for (int i = 0; i < length; i++)
            // {
            //     for (int j = 0; j < length; j++) {
            //         if (origin.charAt(i) == reverse.charAt(j)) {
            //             if (i == 0 || j == 0) {
            //                 arr[i][j] = 1;
            //             } else {
            //                 arr[i][j] = arr[i - 1][j - 1] + 1;
            //             }
            //         }
            //         if (arr[i][j] > maxLen) { 
            //             maxLen = arr[i][j];
            //             maxEnd = i; //以 i 位置结尾的字符
            //         }

            //     }
            // }
            // return s.substring(maxEnd - maxLen + 1, maxEnd + 1);
            return null;
        }
 




    }
}


