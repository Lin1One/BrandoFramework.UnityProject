using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 221. 最大正方形
        // 在一个由 0 和 1 组成的二维矩阵内，找到只包含 1 的最大正方形，并返回其面积。

        // 示例:
        // 输入: 
        // 1 0 1 0 0
        // 1 0 1 1 1
        // 1 1 1 1 1
        // 1 0 0 1 0
        //输出: 4

        public int MaximalSquare(char[][] matrix) 
        {
            return 0;
        }

        public int MaximalSquare1(char[][] matrix) 
        {
            // 我们用一个例子来解释这个方法：
            // 0 1 1 1 0
            // 1 1 1 1 1
            // 0 1 1 1 1
            // 0 1 1 1 1
            // 0 0 1 1 1
            // 我们用 0 初始化另一个矩阵 dp，维数和原始矩阵维数相同；
            // dp(i,j) 表示的是由 1 组成的最大正方形的边长；
            // 从 (0,0) 开始，对原始矩阵中的每一个 1，我们将当前元素的值更新为
            // dp(i, j)=min(dp(i−1, j), dp(i−1, j−1), dp(i, j−1)) + 1

            // 我们还用一个变量记录当前出现的最大边长，这样遍历一次，
            // 找到最大的正方形边长 maxsqlen，那么结果就是 maxsqlen^2 
            int rows = matrix.Length;
            int cols = rows > 0 ? matrix[0].Length : 0;
            int[,] dp = new int[rows + 1,cols + 1];
            int maxsqlen = 0;
            for (int i = 1; i <= rows; i++) 
            {
                for (int j = 1; j <= cols; j++) 
                {
                    if (matrix[i-1][j-1] == '1')
                    {
                        dp[i,j] = Math.Min(Math.Min(dp[i,j - 1], dp[i - 1,j]), 
                            dp[i - 1,j - 1]) + 1;
                        maxsqlen = Math.Max(maxsqlen, dp[i,j]);
                    }
                }
            }
            return maxsqlen * maxsqlen;
        }

        public int MaximalSquare2(char[][] matrix) 
        {
            // 方法三：动态规划优化
            // 在前面的动态规划解法中，计算 i^{th} 行（row）的 dp 方法中，
            // 我们只使用了上一个元素和第 (i−1) th行，
            // 因此我们不需要二维 dp 矩阵，因为一维 dp 足以满足此要求。
            // 我们扫描一行原始矩阵元素时，
            // 根据公式：
            // dp[j]=min(dp[j-1],dp[j],prev)
            // 更新数组 dp，其中 prev 指的是 dp[j-1]，
            // 对于每一行，我们重复相同过程并在 dp 矩阵中更新元素。
            int rows = matrix.Length;
            int cols = rows > 0 ? matrix[0].Length : 0;
            int[] dp = new int[cols + 1];
            int maxsqlen = 0;
            int prev = 0;
            for (int i = 1; i <= rows; i++) 
            {
                for (int j = 1; j <= cols; j++) 
                {
                    int temp = dp[j];
                    if (matrix[i - 1][j - 1] == '1') 
                    {
                        dp[j] = Math.Min(Math.Min(dp[j - 1], prev), dp[j]) + 1;
                        maxsqlen = Math.Min(maxsqlen, dp[j]);
                    } 
                    else 
                    {
                        dp[j] = 0;
                    }
                    prev = temp;
                }
        }
        return maxsqlen * maxsqlen;
        }

    }
}




