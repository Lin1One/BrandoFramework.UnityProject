using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //面试题05.替换空格
        //请实现一个函数，把字符串 s 中的每个空格替换成"%20"。

        //示例 1：

        //输入：s = "We are happy."
        //输出："We%20are%20happy."

        //限制：

        //0 <= s 的长度 <= 10000
        public string ReplaceSpace(string s)
        {
            StringBuilder res = new StringBuilder();
            foreach (var c in s.ToArray())
            {
                if (c == ' ')
                    res.Append("%20");
                else
                    res.Append(c);
            }
            return res.ToString();
        }
    }

}