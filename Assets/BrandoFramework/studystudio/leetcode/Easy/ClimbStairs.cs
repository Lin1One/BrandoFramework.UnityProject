#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 假设你正在爬楼梯。需要 n 阶你才能到达楼顶。
        // 每次你可以爬 1 或 2 个台阶。你有多少种不同的方法可以爬到楼顶呢？
        // 注意：给定 n 是一个正整数。

        // 示例 1：
        // 输入： 2
        // 输出： 2
        // 解释： 有两种方法可以爬到楼顶。
        // 1.  1 阶 + 1 阶
        // 2.  2 阶

        // 示例 2：
        // 输入： 3
        // 输出： 3
        // 解释： 有三种方法可以爬到楼顶。
        // 1.  1 阶 + 1 阶 + 1 阶
        // 2.  1 阶 + 2 阶
        // 3.  2 阶 + 1 阶


        //记忆化递归
        //时间复杂度：O(n)，树形递归的大小可以达到 n。
        //空间复杂度：O(n)，递归树的深度可以达到 n。
        public int climbStairs(int n)
        {
            int[] memo = new int[n + 1];
            return climb_Stairs(0, n, memo);
        }

        public int climb_Stairs(int i, int n, int[] memo)
        {
            if (i > n)
            {
                return 0;
            }
            if (i == n)
            {
                return 1;
            }
            if (memo[i] > 0)
            {
                return memo[i];
            }
            memo[i] = climb_Stairs(i + 1, n, memo) + climb_Stairs(i + 2, n, memo);
            return memo[i];
        }

        //方法：动态规划
        //这个问题可以被分解为一些包含最优子结构的子问题，即它的最优解可以从其子问题的最优解来有效地构建
        //第 i 阶可以由以下两种方法得到：
        //在第 (i-1)(i−1) 阶后向上爬 1 阶。
        //在第 (i-2)(i−2) 阶后向上爬 2 阶。
        //所以到达第 i 阶的方法总数就是到第 (i-1) 阶和第 (i-2) 阶的方法数之和。
        //时间复杂度：O(n)，单循环到 n
        //空间复杂度：O(n)，dp 数组用了 n 的空间。
        public int ClimbStairs2(int n)
        {
            if(n == 1)
            {
                return 1;
            }
            int[] dp = new int[n + 1];
            dp[1] = 1;
            dp[2] = 2;
            for(var i = 3;i<=n;i++)
            {
                dp[i] = dp[i - 2] + dp[i - 1];
            }
            return dp[n];
        }

        //斐波那契数 : Fib(n)=Fib(n−1)+Fib(n−2) 
        public int ClimbStairs3(int n)
        {
            if(n == 1)
            {
                return 1;
            }
            int first = 1;
            int second = 2;
            for(var i = 3;i<=n;i++)
            {
                var third = first + second;
                first = second;
                second = third;
            }
            return second;
        }
    }
}


