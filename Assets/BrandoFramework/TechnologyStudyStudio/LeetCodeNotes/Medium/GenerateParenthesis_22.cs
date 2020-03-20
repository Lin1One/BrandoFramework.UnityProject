using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {

        public List<string> generateParenthesis_Test(int n) 
        {
            return null;
        }

        // 给出 n 代表生成括号的对数，请你写出一个函数，
        // 使其能够生成所有可能的并且有效的括号组合。
        // 例如，给出 n = 3，生成结果为：
        // [
        //   "((()))",
        //   "(()())",
        //   "(())()",
        //   "()(())",
        //   "()()()"
        // ]

        private string[] Parentheses = {"()","(",")"};

        public IList<string> GenerateParenthesis(int n) 
        {
            IList<string> parentheses = new List<string>{"()"};
            while(n > 1)
            {
                parentheses = Generate(parentheses);
                n--;
            }
            return parentheses;
        }
        private IList<string> Generate(IList<string> originStrs)
        {
            var stringList = new List<string>();
            for(var i = 0 ;i< originStrs.Count;i++)
            {
                var addTwoSideStr = Parentheses[1] + originStrs[i] + Parentheses[2];
                stringList.Add(addTwoSideStr);
                var addRightStr =  originStrs[i] + Parentheses[0];
                var addLeftStr =   Parentheses[0] + originStrs[i];
                stringList.Add(addLeftStr);
                if(!stringList.Contains(addRightStr))
                {
                    stringList.Add(addRightStr);
                }
            }
            return stringList;
        }

        // 方法二：回溯法
        // 只有在我们知道序列仍然保持有效时才添加 '(' or ')'，
        // 而不是像 方法一 那样每次添加。
        // 我们可以通过跟踪到目前为止放置的左括号和右括号的数目来做到这一点，

        // 如果我们还剩一个位置，我们可以开始放一个左括号。 
        //如果它不超过左括号的数量，我们可以放一个右括号。
        public List<string> generateParenthesis(int n) 
        {
            List<string> ans = new List<string>();
            backtrack(ans, "", 0, 0, n);
            return ans;
        }

        public void backtrack(List<string> ans, 
            string cur, 
            int open, 
            int close, 
            int max)
        {
        if (cur.Length == max * 2) 
        {
            ans.Add(cur);
            return;
        }

        if (open < max)
            backtrack(ans, cur + "(", open+1, close, max);
        if (close < open)
            backtrack(ans, cur + ")", open, close+1, max);
        }

    }
}


