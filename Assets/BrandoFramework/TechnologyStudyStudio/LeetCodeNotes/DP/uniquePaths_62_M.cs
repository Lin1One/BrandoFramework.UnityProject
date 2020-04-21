using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 一个机器人位于一个 m x n 网格的左上角 
        // （起始点在下图中标记为“Start” ）。
        // 机器人每次只能向下或者向右移动一步。
        // 机器人试图达到网格的右下角（在下图中标记为“Finish”）。
        // 问总共有多少条不同的路径？
        // 说明：m 和 n 的值均不超过 100。


        // 示例 1:
        // 输入: m = 3, n = 2
        // 输出: 3
        // 解释:
        // 从左上角开始，总共有 3 条路径可以到达右下角。
        // 1. 向右 -> 向右 -> 向下
        // 2. 向右 -> 向下 -> 向右
        // 3. 向下 -> 向右 -> 向右

        // 示例 2:
        // 输入: m = 7, n = 3
        // 输出: 28


        int count = 0;
        public int UniquePaths(int m, int n) 
        {
            Move(m - 1,n - 1,0,0);
            return count;
        }

        private void Move(int m,int n,int currentM,int currentN)
        {
            if(currentM == m && currentN == n)
            {
                count++;
                return;
            }
            if(currentM < m)
            {
                Move(m,n,++currentM,currentN);
            }
            if(currentN < n)
            {
                Move(m,n,currentM,++currentN);
            }
        }


        //递归
        private int[,] mome = new int[101,101];
        public int UniquePaths2(int m, int n) 
        {
            if(m == 1||n == 1)
            {
                return 1;
            }
            if(m == 2 || n == 2)
            {
                return m == 2?n:m;
            }
            if(mome[m,n] > 0)
            {
                return mome[m,n];
            }
            if( m > 2 && n > 2)
            {
                mome[m - 1,n] = UniquePaths2(m - 1,n);
                mome[m,n - 1] = UniquePaths2(m,n - 1);
                mome[m,n] = mome[m - 1,n] + mome[m,n - 1];
            }
            return mome[m,n];
        }

        private int MoveToLastPathCount(int m,int n)
        {
            int pathCount = 0;

            if( m > 2 && n > 2)
            {
                pathCount = MoveToLastPathCount(m - 1,n)
                + MoveToLastPathCount(m,n-1);
            }
            return pathCount;
        }


        public int UniquePaths3(int m, int n) 
        {
            //O(n^2) 空间复杂度
            int[,] dp = new int[m,n];
            for (int i = 0; i < n; i++)
                dp[0,i] = 1;
            for (int i = 0; i < m; i++)
                dp[i,0] = 1;
            for (int i = 1; i < m; i++) 
            {
                for (int j = 1; j < n; j++) 
                {
                    dp[i,j] = dp[i - 1,j] + dp[i,j - 1];
                }
            }
            return dp[m - 1,n - 1];  
        }

        public int uniquePaths4(int m, int n) 
        {
            //O(2n) 空间复杂度
            int[] pre = new int[n];
            int[] cur = new int[n];
            for(var i = 0;i< n;i++)
            {
                pre[i] = 1;
                cur[i] = 1;
            }
            for (int i = 1; i < m;i++)
            {
                for (int j = 1; j < n; j++)
                {
                    cur[j] = cur[j-1] + pre[j];
                }
                Array.Copy(cur, pre , n);
            }
            return pre[n-1]; 
        }
        
        public int uniquePaths5(int m, int n) 
        {
            //O(n) 空间复杂度
            int[] cur = new int[n];
            for (int i = 0; i < n; i++)
            {
                cur[i] = 1;
            }
            for (int i = 1; i < m;i++)
            {
                for (int j = 1; j < n; j++)
                {
                    cur[j] = cur[j] + cur[j-1];
                }
            }
            return cur[n-1];
        }

    }
}


