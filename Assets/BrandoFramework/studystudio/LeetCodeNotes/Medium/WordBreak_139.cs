using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 139. 单词拆分
        // 给定一个非空字符串 s 和一个包含非空单词列表的字典 wordDict，
        // 判定 s 是否可以被空格拆分为一个或多个在字典中出现的单词。

        // 说明：

        // 拆分时可以重复使用字典中的单词。
        // 你可以假设字典中没有重复的单词。

        // 示例 1：
        // 输入: s = "leetcode", wordDict = ["leet", "code"]
        // 输出: true
        // 解释: 返回 true 因为 "leetcode" 可以被拆分成 "leet code"。

        // 示例 2：
        // 输入: s = "applepenapple", wordDict = ["apple", "pen"]
        // 输出: true
        // 解释: 返回 true 因为 "applepenapple" 可以被拆分成 "apple pen apple"。
        // 注意你可以重复使用字典中的单词。

        // 示例 3：
        // 输入: s = "catsandog", wordDict = ["cats", "dog", "sand", "and", "cat"]
        // 输出: false

        public bool WordBreak(string s, IList<string> wordDict) 
        {
            //  Dictionary<string,int> wordLengthDict = new Dictionary<string,int>();
            //  foreach(var word in wordDict)
            //  {
            //      wordLengthDict.Add(word,word.Length);
            //  }
            //  bool result = false;
            //  foreach(var wordAndIndex in wordLengthDict)
            //  {
            //      int i = 0;
            //      if(s.Substring(i,wordAndIndex.value) == wordAndIndex.key)
            //      {
            //          i += wordAndIndex.value;
            //      }
            //  }
            return false;
        }

        public bool wordBreak(string s, List<string> wordDict) 
        {
            return word_Break(s, new HashSet<string>(wordDict), 0, new bool?[s.Length]);
        }
        public bool word_Break(string s, HashSet<string> wordDict, int start, bool?[] memo) 
        {
            if (start == s.Length)
            {
                return true;
            }
            if (memo[start] != null) 
            {
                return (bool)memo[start];
            }
            for (int end = start + 1; end <= s.Length; end++) 
            {
                if (wordDict.Contains(s.Substring(start, end - start)) && word_Break(s, wordDict, end, memo)) 
                {
                    memo[start] = true;
                    return (bool)memo[start];
                }
            }
            memo[start] = false;
            return (bool)memo[start];
        }

        // 方法 4：使用动态规划
        // 算法

        // 这个方法的想法是对于给定的字符串（ss）可以被拆分成子问题 s1s1 和 s2s2 。如果这些子问题都可以独立地被拆分成符合要求的子问题，那么整个问题 ss 也可以满足。也就是，如果 "\text{catsanddog}catsanddog" 可以拆分成两个子字符串 "\text{catsand}catsand" 和 "\text{dog}dog" 。子问题 "\text{catsand}catsand" 可以进一步拆分成 "\text{cats}cats" 和 "\text{and}and" ，这两个独立的部分都是字典的一部分，所以 "\text{catsand}catsand" 满足题意条件，再往前， "\text{catsand}catsand" 和 "\text{dog}dog" 也分别满足条件，所以整个字符串 "\text{catsanddog}catsanddog" 也满足条件。

        // 现在，我们考虑 \text{dp}dp 数组求解的过程。我们使用 n+1n+1 大小数组的 \text{dp}dp ，其中 nn 是给定字符串的长度。我们也使用 2 个下标指针 ii 和 jj ，其中 ii 是当前字符串从头开始的子字符串（s's 
        // ′
        //  ）的长度， jj 是当前子字符串（s's 
        // ′
        //  ）的拆分位置，拆分成 s'(0,j)s 
        // ′
        //  (0,j) 和 s'(j+1,i)s 
        // ′
        //  (j+1,i) 。

        // 为了求出 \text{dp}dp 数组，我们初始化 \text{dp}[0]dp[0] 为 \text{true}true ，这是因为空字符串总是字典的一部分。 \text{dp}dp 数组剩余的元素都初始化为 \text{false}false 。

        // 我们用下标 ii 来考虑所有从当前字符串开始的可能的子字符串。对于每一个子字符串，我们通过下标 jj 将它拆分成 s1's1 
        // ′
        //   和 s2's2 
        // ′
        //   （注意 ii 现在指向 s2's2 
        // ′
        //   的结尾）。为了将 \text{dp}[i]dp[i] 数组求出来，我们依次检查每个 \text{dp}[j]dp[j] 是否为 \text{true}true ，也就是子字符串 s1's1 
        // ′
        //   是否满足题目要求。如果满足，我们接下来检查 s2's2 
        // ′
        //   是否在字典中。如果包含，我们接下来检查 s2's2 
        // ′
        //   是否在字典中，如果两个字符串都满足要求，我们让 \text{dp}[i]dp[i] 为 \text{true}true ，否则令其为 \text{false}false 。

        public bool WordBreak1(string s, IList<string> wordDict) 
        {
            var  wordDictSet = new HashSet<string>(wordDict);
            bool[] dp = new bool[s.Length + 1];
            dp[0] = true;
            for (int i = 1; i <= s.Length; i++) 
            {
                for (int j = 0; j < i; j++) 
                {
                    if (dp[j] && wordDictSet.Contains(s.Substring(j, i - j))) 
                    {
                        dp[i] = true;
                        break;
                    }
                }
            }
            return dp[s.Length];
        }

        public bool WordBreak2(string s, IList<string> wordDict) 
        {
            bool[] dp = new bool[s.Length+1];
            dp[0] = true;
            for(var i = 1;i< s.Length;i++)
            {
                for(var j = 0;j< i;j++)
                {
                    if(dp[j]&&wordDict.Contains(s.Substring(j,i - j)))
                    {
                        dp[i] = true;
                        break;
                    }
                }
            }
            return dp[s.Length];
        }
    }
}




