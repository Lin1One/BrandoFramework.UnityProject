using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //面试题13.机器人的运动范围
        //地上有一个m行n列的方格，从坐标[0, 0] 到坐标[m - 1, n - 1] 。
        //一个机器人从坐标[0, 0] 的格子开始移动，它每次可以向左、右、上、下移动一格（不能移动到方格外），
        //也不能进入行坐标和列坐标的数位之和大于k的格子。
        //例如，当k为18时，机器人能够进入方格[35, 37] ，因为3+5+3+7=18。
        //但它不能进入方格[35, 38]，因为3+5+3+8=19。请问该机器人能够到达多少个格子？

        //示例 1：

        //输入：m = 2, n = 3, k = 1
        //输出：3
        //示例 1：

        //输入：m = 3, n = 1, k = 0
        //输出：1
        //提示：

        //1 <= n,m <= 100
        //0 <= k <= 20

        //private int[,] directionArray = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
        public bool[,] position;
        public int MovingCount(int m, int n, int k)
        {
            position = new bool[m, n];
            return MovingCount(m, n, k, 0, 0);
        }

        public int MovingCount(int m, int n, int k, int x, int y)
        {
            if (position[x, y] == true)
            {
                return 0;
            }
            if (calPostionVal(x, y) > k)
            {
                position[x, y] = true;
                return 0;
            }
            position[x, y] = true;
            int result = 1;
            for (var i = 0; i < 4; i++)
            {
                var newX = x + directionArray[i, 0];
                var newY = y + directionArray[i, 1];
                if (newX < m && newX >= 0 && newY < n && newY >= 0)
                {
                    result += MovingCount(m, n, k, newX, newY);
                }
            }
            return result;
        }

        public int calPostionVal(int x, int y)
        {
            int val = x / 10 + y / 10 + x % 10 + y % 10;
            return val;
        }

    }
}




