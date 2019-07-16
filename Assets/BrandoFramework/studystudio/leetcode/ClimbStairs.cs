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


        public int ClimbStairs(int n)
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
    }
}


