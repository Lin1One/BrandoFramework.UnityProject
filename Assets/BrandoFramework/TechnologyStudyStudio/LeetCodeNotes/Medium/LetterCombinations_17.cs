using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个仅包含数字 2-9 的字符串，返回所有它能表示的字母组合。
        // 给出数字到字母的映射如下（与电话按键相同）。注意 1 不对应任何字母。
        
        // 示例:
        // 输入："23"
        // 输出：["ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf"].
        // 说明:
        // 尽管上面的答案是按字典序排列的，但是你可以任意选择答案输出的顺序。

        Dictionary<char, string> phoneChar = new Dictionary<char,string>
        {
            {'2', "abc"},
            {'3', "def"},
            {'4', "ghi"},
            {'5', "jkl"},
            {'6', "mno"},
            {'7', "pqrs"},
            {'8', "tuv"},
            {'9', "wxyz"}
        };
         
        //回溯
        // 回溯是一种通过穷举所有可能情况来找到所有解的算法。
        // 如果一个候选解最后被发现并不是可行解，回溯算法会舍弃它，
        // 并在前面的一些步骤做出一些修改，并重新尝试找到可行解。
        // 给出如下回溯函数 backtrack(combination, next_digits) ，
        //它将一个目前已经产生的组合 combination 和接下来准备要输入的数字 next_digits 作为参数。
        // 如果没有更多的数字需要被输入，那意味着当前的组合已经产生好了。 
        // 如果还有数字需要被输入： 遍历下一个数字所对应的所有映射的字母。 
        //将当前的字母添加到组合最后，也就是 combination = combination + letter 。 
        //重复这个过程，输入剩下的数字： backtrack(combination + letter, next_digits[1:])。
        List<string> output = new List<string>();
        public IList<string> LetterCombinations(string digits) 
        {
            if (digits.Length != 0)
            backtrack("", digits);
            return output;
        }
        public void backtrack(string combination, string next_digits) 
        {
            if (next_digits.Length== 0) 
            {
                output.Add(combination);
            }
            else 
            {
                char digit = next_digits[0];
                string letters = phoneChar[digit];
                for (int i = 0; i < letters.Length; i++) 
                {
                    char letter = phoneChar[digit][i];
                    backtrack(combination + letter, next_digits.Substring(1));
                }
            }
        }
    }
}


