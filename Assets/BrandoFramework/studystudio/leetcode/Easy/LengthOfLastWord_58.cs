using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 58. 最后一个单词的长度
        // 给定一个仅包含大小写字母和空格 ' ' 的字符串，返回其最后一个单词的长度。
        // 如果不存在最后一个单词，请返回 0 。
        // 说明：一个单词是指由字母组成，但不包含任何空格的字符串。

        // 示例:
        // 输入: "Hello World"
        // 输出: 5
        public int LengthOfLastWord(string s) 
        {
            var end = s.Length - 1;
            while(end >= 0 && s[end] == ' ')
            {
                end--;
            }
            int newEnd = end;
            while(newEnd >= 0 && s[newEnd] != ' ')
            {
                newEnd--;
            }
            return end - newEnd;
        }

    }
}


