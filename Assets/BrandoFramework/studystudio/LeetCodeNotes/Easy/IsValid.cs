#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System.Collections.Generic;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //给定一个只包括 '('，')'，'{'，'}'，'['，']' 的字符串，判断字符串是否有效。
        //有效字符串需满足：

        //左括号必须用相同类型的右括号闭合。
        //左括号必须以正确的顺序闭合。
        //注意空字符串可被认为是有效字符串。

        //示例 1:

        //输入: "()"
        //输出: true
        //示例 2:

        //输入: "()[]{}"
        //输出: true
        //示例 3:

        //输入: "(]"
        //输出: false
        //示例 4:

        //输入: "([)]"
        //输出: false
        //示例 5:

        //输入: "{[]}"
        //输出: true
        public bool IsValid1(string s)
        {
            var mappings = new Dictionary<char, char>();
            mappings.Add(')', '(');
            mappings.Add('}', '{');
            mappings.Add(']', '[');

            if (string.IsNullOrEmpty(s))
            {
                return true;
            }
            Stack<char> stack = new Stack<char>();
            var length = s.Length;

            if (length % 2 != 0)
            {
                return false;
            }
            for (var i = 0; i < length; i++)
            {
                if (mappings.ContainsKey(s[i]))
                {
                    if (stack.Count != 0 && stack.Peek() == mappings[s[i]])
                    {
                        stack.Pop();
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    stack.Push(s[i]);
                }
            }
            return stack.Count == 0;

        }
    }
}

