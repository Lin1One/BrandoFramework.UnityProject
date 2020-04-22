using System.Collections.Generic;
using UnityEngine;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 你是一个专业的小偷，计划偷窃沿街的房屋。
        // 每间房内都藏有一定的现金，影响你偷窃的唯一制约因素就是相邻的房屋装有相互连通的防盗系统，
        // 如果两间相邻的房屋在同一晚上被小偷闯入，系统会自动报警。

        // 给定一个代表每个房屋存放金额的非负整数数组，
        // 计算你在不触动警报装置的情况下，能够偷窃到的最高金额。

        // 示例 1:
        // 输入: [1,2,3,1]
        // 输出: 4
        // 解释: 偷窃 1 号房屋 (金额 = 1) ，然后偷窃 3 号房屋 (金额 = 3)。
        // 偷窃到的最高金额 = 1 + 3 = 4 。

        // 示例 2:
        // 输入: [2,7,9,3,1]
        // 输出: 12
        // 解释: 偷窃 1 号房屋 (金额 = 2), 偷窃 3 号房屋 (金额 = 9)，接着偷窃 5 号房屋 (金额 = 1)。
        // 偷窃到的最高金额 = 2 + 9 + 1 = 12 。

        public int Rob2(int[] nums)
        {
            var dp = new int[nums.Length + 1];
            dp[0] = 0;
            dp[1] = nums[0];
            for(var i = 2;i<nums.Length + 1;i++)
            {
                dp[i] = Math.Max(dp[i - 1], dp[i - 2] + nums[i - 1]);
            }
            return dp[nums.Length];
        }


        public int Rob(int[] nums)
        {      // 方法一：动态规划
               // 算法
               // 考虑所有可能的抢劫方案过于困难。一个自然而然的想法是首先从最简单的情况开始。记：

            // f(k) = 从前 k 个房屋中能抢劫到的最大数额
            // A_i = 第 i 个房屋的钱数。

            // 看 n = 1 的情况，显然 f(1) = A_1

            // 看 n = 2，f(2) = max(A_1, A_2)。

            //  n = 3，有两个选项:
            //      抢第三个房子，将数额与第一个房子相加。
            //      不抢第三个房子，保持现有最大数额。

            // 显然，你想选择数额更大的选项。于是，可以总结出公式：

            //动态规划公式： f(k) = max(f(k – 2) + A_k,f(k – 1))

            // 我们选择 f(–1) = f(0) = 0 为初始情况，这将极大地简化代码。
            // 答案为 f(n)。可以用一个数组来存储并计算结果。不过由于每一步你只需要前两个最大值，
            // 两个变量就足够用了。
            // Java
            // public int rob(int[] num) {
            //     int prevMax = 0;
            //     int currMax = 0;
            //     for (int x : num) {
            //         int temp = currMax;
            //         currMax = Math.max(prevMax + x, currMax);
            //         prevMax = temp;
            //     }
            //     return currMax;
            // }

            // 复杂度分析
            // 时间复杂度：O(n)。其中 n 为房子的数量。
            // 空间复杂度：O(1)。

            //第 k 家
            //动态规划公式： f(k) = max(f(k – 2) + A_k,f(k – 1))
            int prevMax = 0;    //k-2
            int currMax = 0;    //k-1
            foreach (int x in nums)
            {
                int temp = currMax;
                currMax = Math.Max(prevMax + x, currMax);
                prevMax = temp;
            }
            return currMax;
        }

        public int rob(int[] nums) 
        {
            int len = nums.Length;
            if(len == 0)
                return 0;
            int[] dp = new int[len + 1];
            dp[0] = 0;
            dp[1] = nums[0];
            for(int i = 2; i <= len; i++) 
            {
                dp[i] = Math.Max(dp[i-1], dp[i-2] + nums[i-1]);
            }
            return dp[len];
        }

    }
}


