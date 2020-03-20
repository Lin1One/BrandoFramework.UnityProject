using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 59. 螺旋矩阵 II
        // 给定一个正整数 n，生成一个包含 1 到 n^2 所有元素，
        // 且元素按顺时针顺序螺旋排列的正方形矩阵。

        // 示例:

        // 输入: 3
        // 输出:
        // [
        //  [ 1, 2, 3 ],
        //  [ 8, 9, 4 ],
        //  [ 7, 6, 5 ]
        // ]

        public int[][] GenerateMatrix(int n) 
        {
            if(n <= 0)
            {
                return null;
            }
            
            int Num = 1;
            int[][] result = new int[n][];
            for(var i = 0;i<result.Length;i++)
            {
                result[i] = new int[n];
            }

            if(n == 1)
            {
                result[0][0] = 1;
                return result;
            }

            int left = 0;
            int right = n - 1;
            int top = 0;
            int bottom = n - 1;
            while(left <= right && top <= bottom)
            {
                for (int i = left; i <= right; i++)
                {
                    result[top][i] = Num;
                    Num++;
                }
                for (int j = top + 1; j <= bottom; j++)
                {
                    result[j][right] = Num;
                    Num++;
                }
                if (left < right && top < bottom) 
                {
                    for (int i = right - 1; i > left; i--) 
                    {
                        result[bottom][i] = Num;
                        Num++;
                    }    
                    for (int j = bottom; j > top; j--) 
                    {
                        result[j][left]= Num;
                        Num++;
                    }
                }
                left++;
                right--;
                top++;
                bottom--;
            }
            return result;
        }
        public int[][] GenerateMatrix1(int n) 
        {
            int[][] arr = new int[n][];
            for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = new int[n];
                }
            int c = 1, j = 0;
            while (c <= n * n) 
            {
                for (int i = j; i < n - j; i++)
                    arr[j][i] = c++;
                for (int i = j + 1; i < n - j; i++)
                    arr[i][n - j - 1] = c++;
                for (int i = n - j - 2; i >= j; i--)
                    arr[n - j - 1][i] = c++;
                for (int i = n -j - 2; i > j; i--)
                    arr[i][j] = c++;
                j++;
            }
            return arr;
        }
    }
}


