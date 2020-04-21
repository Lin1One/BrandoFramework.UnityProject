using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //一个机器人位于一个 m x n 网格的左上角 （起始点在下图中标记为“Start” ）。

        //机器人每次只能向下或者向右移动一步。机器人试图达到网格的右下角（在下图中标记为“Finish”）。

        //现在考虑网格中有障碍物。那么从左上角到右下角将会有多少条不同的路径？

        //网格中的障碍物和空位置分别用 1 和 0 来表示。

        //说明：m 和 n 的值均不超过 100。

        //示例 1:
        //输入:
        //[
        // [0,0,0],
        //  [0,1,0],
        //  [0,0,0]
        //]
        //输出: 2
        //解释:
        //3x3 网格的正中间有一个障碍物。
        //从左上角到右下角一共有 2 条不同的路径：
        //1. 向右 -> 向右 -> 向下 -> 向下
        //2. 向下 -> 向下 -> 向右 -> 向右

        public int UniquePathsWithObstacles(int[][] obstacleGrid)
        {
            if(obstacleGrid == null)
            {
                return 0;
            }
            if (obstacleGrid[0][0] == 1)
            {
                return 0;
            }
            int row = obstacleGrid.Length;
            int col = obstacleGrid[0].Length;
            int[,] dp = new int[row, col];
            dp[0,0] = 1;
            for (int i = 1; i < col; i++)
            {
                if(obstacleGrid[0][i] == 1)
                {
                    dp[0, i] = 0;
                }
                else if(obstacleGrid[0][i] == 0 && obstacleGrid[0][i - 1] == 1)
                {
                    obstacleGrid[0][i] = 1;
                    dp[0,i] = 0;
                }
                else
                {
                    dp[0, i] = 1;
                }
            }
            for (int i = 1; i < row; i++)
            {
                if (obstacleGrid[i][0] == 1)
                {
                    dp[i, 0] = 0;
                }
                else if (obstacleGrid[i][0] == 0 && obstacleGrid[i - 1][0] == 1)
                {
                    obstacleGrid[i][0] = 1;
                    dp[i, 0] = 0;
                }
                else
                {
                    dp[i, 0] = 1;
                }
            }
                
            for (int i = 1; i < row; i++)
            {
                for (int j = 1; j < col; j++)
                {
                    var right = obstacleGrid[i][j - 1] == 0 ? dp[i, j - 1] : 0;
                    var up = obstacleGrid[i - 1][j] == 0 ? dp[i - 1, j] : 0;
                    dp[i, j] = obstacleGrid[i][j] == 0 ? right + up : 0;
                }
            }
            return dp[row - 1, col - 1];
        }

        public int UniquePathsWithObstacles3(int[][] obstacleGrid)
        {

            int R = obstacleGrid.Length;
            int C = obstacleGrid[0].Length;

            // If the starting cell has an obstacle, then simply return as there would be
            // no paths to the destination.
            if (obstacleGrid[0][0] == 1)
            {
                return 0;
            }

            // Number of ways of reaching the starting cell = 1.
            obstacleGrid[0][0] = 1;

            // Filling the values for the first column
            for (int i = 1; i < R; i++)
            {
                obstacleGrid[i][0] = (obstacleGrid[i][0] == 0 && obstacleGrid[i - 1][0] == 1) ? 1 : 0;
            }

            // Filling the values for the first row
            for (int i = 1; i < C; i++)
            {
                obstacleGrid[0][i] = (obstacleGrid[0][i] == 0 && obstacleGrid[0][i - 1] == 1) ? 1 : 0;
            }

            // Starting from cell(1,1) fill up the values
            // No. of ways of reaching cell[i][j] = cell[i - 1][j] + cell[i][j - 1]
            // i.e. From above and left.
            for (int i = 1; i < R; i++)
            {
                for (int j = 1; j < C; j++)
                {
                    if (obstacleGrid[i][j] == 0)
                    {
                        obstacleGrid[i][j] = obstacleGrid[i - 1][j] + obstacleGrid[i][j - 1];
                    }
                    else
                    {
                        obstacleGrid[i][j] = 0;
                    }
                }
            }

            // Return value stored in rightmost bottommost cell. That is the destination.
            return obstacleGrid[R - 1][C - 1];
        }

        public int UniquePathsWithObstacles2(int[][] arr)
        {
            if (arr[0][0]++ == 1) return 0;
            int m = arr.Length, n = arr[0].Length;
            for (int i = 1; i < n; i++)
                arr[0][i] = arr[0][i] == 1 ? 0 : arr[0][i - 1];
            for (int i = 1; i < m; i++)
                arr[i][0] = arr[i][0] == 1 ? 0 : arr[i - 1][0];
            for (int i = 1; i < m; i++)
                for (int j = 1; j < n; j++)
                    arr[i][j] = arr[i][j] == 1 ? 0 : arr[i - 1][j] + arr[i][j - 1];
            return arr[--m][--n];
        }

    }
}


