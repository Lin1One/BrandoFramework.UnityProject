using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 54. 螺旋矩阵
        // 给定一个包含 m x n 个元素的矩阵（m 行, n 列），请按照顺时针螺旋顺序，
        // 返回矩阵中的所有元素。

        // 示例 1:
        // 输入:
        // [
        //  [ 1, 2, 3 ],
        //  [ 4, 5, 6 ],
        //  [ 7, 8, 9 ]
        // ]
        // 输出: [1,2,3,6,9,8,7,4,5]

        // 示例 2:
        // 输入:
        // [
        //   [1, 2, 3, 4],
        //   [5, 6, 7, 8],
        //   [9,10,11,12]
        // ]
        // 输出: [1,2,3,4,8,12,11,10,9,5,6,7]

        public IList<int> SpiralOrder3(int[][] matrix)
        {
            IList<int> result = new List<int>();
            if (matrix.Length == 0)
            {
                return result;
            }
            int rowNum = matrix.Length;
            int colNum = matrix[0].Length;
            int numCount = rowNum * colNum;
            int left = 0;
            int right = colNum - 1;
            int top = 0;
            int bottom = rowNum - 1;
            while(result.Count < numCount)
            {
                for(var i = left;i <= right;i++)
                {
                    result.Add(matrix[top][i]);
                }
                top++;
                if (result.Count < numCount)
                {
                    for(var i = top;i <= bottom;i++)
                    {
                        result.Add(matrix[i][right]);
                    }
                    right--;
                }
                if (result.Count < numCount)
                {
                    for (var i = right; i >= left; i--)
                    {
                        result.Add(matrix[bottom][i]);
                    }
                    bottom--;
                }
                if (result.Count < numCount)
                {
                    for (var i = bottom; i >= top; i--)
                    {
                        result.Add(matrix[i][left]);
                    }
                    left++;
                }
            }
            return result;
        }


        public IList<int> SpiralOrder(int[][] matrix) 
        {
            IList<int> result = new List<int>();
            if( matrix.Length == 0)
            {
                return result;
            }
            int rowStartIndex = 0;
            int rowEndIndex = matrix.Length - 1;
            int colStartIndex = 0;
            int colEndIndex = matrix[0].Length - 1;
            
            while(rowStartIndex <= rowEndIndex && colStartIndex <= colEndIndex)
            {
                for (int i = colStartIndex; i <= colEndIndex; i++) 
                    result.Add(matrix[rowStartIndex][i]);
                for (int j = rowStartIndex + 1; j <= rowEndIndex; j++) 
                    result.Add(matrix[j][colEndIndex]);
                if (rowStartIndex < rowEndIndex && colStartIndex < colEndIndex) 
                {
                    for (int i = colEndIndex - 1; i > colStartIndex; i--) 
                        result.Add(matrix[rowEndIndex][i]);
                    for (int j = rowEndIndex; j > rowStartIndex; j--) 
                        result.Add(matrix[j][colStartIndex]);
                }
                rowStartIndex++;
                rowEndIndex--;
                colStartIndex++;
                colEndIndex--;
            }
            return result;
        }

        public IList<int> SpiralOrder2(int[][] matrix)
        {
            List<int> list = new List<int>();
            if (matrix == null || matrix.Length == 0)
            {
                return list;
            }
            int rowNum = matrix.Length;
            int colNum = matrix[0].Length;
            int left = 0;
            int right = colNum - 1;
            int top = 0;
            int bottom = rowNum - 1;
            while (list.Count < rowNum * colNum)
            {
                for (int col = left; col <= right; col++)
                {
                    list.Add(matrix[top][col]);
                }
                top++;
                if (list.Count < rowNum * colNum)
                {
                    for (int row = top; row <= bottom; row++)
                    {
                        list.Add(matrix[row][right]);
                    }
                    right--;
                }
                if (list.Count < rowNum * colNum)
                {
                    for (int col = right; col >= left; col--)
                    {
                        list.Add(matrix[bottom][col]);
                    }
                    bottom--;
                }
                if (list.Count<rowNum*colNum)
                {
                    for (int row = bottom; row >=top; row--)
                    {
                        list.Add(matrix[row][left]);
                    }
                    left++;
                }
            }
            return list;
        }
    }
}


