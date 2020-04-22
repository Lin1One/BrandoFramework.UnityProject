using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个包含非负整数的 m x n 网格，
        // 请找出一条从左上角到右下角的路径，使得路径上的数字总和为最小。
        // 说明：每次只能向下或者向右移动一步。
        
        // 示例:
        // 输入:
        // [
        //   [1,3,1],
        //   [1,5,1],
        //   [4,2,1]
        // ]
        // 输出: 7
        // 解释: 因为路径 1→3→1→1→1 的总和最小。

        //[[1,3,1],[1,5,1],[4,2,1]]
        // 输出
        // 0
        // 预期结果
        // 7
        //[[0,1],
        // [1,0]]
        // 输出
        // 2
        // 预期结果
        // 1   
        public int MinPathSum(int[][] grid) 
        {
            if(grid == null || grid.Length == 0)
            {
                return 0;
            }
            int m = grid.Length;
            int n = grid[0].Length;
            int[,] minPathCost = new int[m,n];
            for (int i = 0; i < m ; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    minPathCost[i,j] = -1;
                }
            }
            minPathCost[0,0] = grid[0][0]; 
            for(var i = 1;i< n ;i++)
            {
                minPathCost[0,i] =  minPathCost[0,i - 1] + grid[0][i];
            }
            for(var i = 1;i< m ;i++)
            {
                minPathCost[i,0] =  minPathCost[i - 1,0] + grid[i][0];
            }
            return GetPathCost(m - 1,n-1,minPathCost,grid);
        }

        private int GetPathCost(int m,int n,int[,] minPathCost,int[][] grid)
        {
            if(minPathCost[m,n] > -1)
            {
                return minPathCost[m,n];
            }
            minPathCost[m,n] = Math.Min(GetPathCost(m - 1,n,minPathCost,grid) + grid[m][n],
                GetPathCost(m,n - 1,minPathCost,grid) + grid[m ][n]);
            return minPathCost[m,n];
        }

        public int MinPathSum1(int[][] grid)
        {
            if (grid == null || grid.Length == 0)
            {
                return 0;
            }
            var colCount = grid[0].Length;
            var curRow = new int[colCount];
            curRow[0] = grid[0][0];
            for (var i = 1;i < colCount; i++)
            {
                curRow[i] = grid[0][i] + curRow[i - 1];
            }
            for(var i = 1;i < grid.Length;i++)
            {
                curRow[0] = curRow[0] + grid[i][0];
                for (var j = 1;j<colCount;j++)
                {
                    curRow[j] = Math.Min(curRow[j - 1], curRow[j])+ grid[i][j];
                }
            }
            return curRow[colCount - 1];
        }

    }
}


