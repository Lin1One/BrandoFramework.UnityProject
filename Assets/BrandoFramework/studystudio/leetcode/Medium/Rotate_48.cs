using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 48. 旋转图像
        // 给定一个 n × n 的二维矩阵表示一个图像。
        // 将图像顺时针旋转 90 度。
        // 说明：
        // 你必须在原地旋转图像，这意味着你需要直接修改输入的二维矩阵。
        // 请不要使用另一个矩阵来旋转图像。

        // 示例 1:
        // 给定 matrix = 
        // [
        //   [1,2,3],
        //   [4,5,6],
        //   [7,8,9]
        // ],

        // 原地旋转输入矩阵，使其变为:
        // [
        //   [7,4,1],
        //   [8,5,2],
        //   [9,6,3]
        // ]
        // 示例 2:
        // 给定 matrix =
        // [
        //   [ 5, 1, 9,11],
        //   [ 2, 4, 8,10],
        //   [13, 3, 6, 7],
        //   [15,14,12,16]
        // ], 

        // 原地旋转输入矩阵，使其变为:
        // [
        //   [15,13, 2, 5],
        //   [14, 3, 4, 1],
        //   [12, 6, 8, 9],
        //   [16, 7,10,11]
        // ]

        public void Rotate(int[][] matrix)
        {
            int n = matrix.Length;
            for(var i = 0;i<n/2;i++)
            {
                int moveStep = n - 2 * i - 1;
                int newX = i;
                int newY = i;
                do
                {

                }
                while(newX != i && newY != i);
            }
        }

        public void rotate1(int[][] matrix) 
        {
            int n = matrix.Length;
            for (int i = 0; i < n / 2 + n % 2; i++) 
            {
                for (int j = 0; j < n / 2; j++) 
                {
                    int[] tmp = new int[4];
                    //行
                    int row = i;
                    int col = j;
                    for (int k = 0; k < 4; k++) 
                    {
                        tmp[k] = matrix[row][col];
                        int x = row;
                        row = col;
                        col = n - 1 - x;
                    }
                    for (int k = 0; k < 4; k++) 
                    {
                        matrix[row][col] = tmp[(k + 3) % 4];
                        int x = row;
                        row = col;
                        col = n - 1 - x;
                    }
                }
            }
        }

        public void Rotate2(int[][] matrix) 
        {
            int n = matrix.Length;
            for (int i = 0; i < (n + 1) / 2; i ++) 
            {
                for (int j = 0; j < n / 2; j++) 
                {
                    int temp = matrix[n - 1 - j][i];
                    matrix[n - 1 - j][i] = matrix[n - 1 - i][n - j - 1];
                    matrix[n - 1 - i][n - j - 1] = matrix[j][n - 1 -i];
                    matrix[j][n - 1 - i] = matrix[i][j];
                    matrix[i][j] = temp;
                }
            }
        }
    }
}


