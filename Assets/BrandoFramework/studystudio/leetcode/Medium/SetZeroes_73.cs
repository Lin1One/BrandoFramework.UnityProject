﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 73. 矩阵置零
        // 给定一个 m x n 的矩阵，
        // 如果一个元素为 0，则将其所在行和列的所有元素都设为 0。请使用原地算法。

        // 示例 1:
        // 输入: 
        // [
        //   [1,1,1],
        //   [1,0,1],
        //   [1,1,1]
        // ]
        // 输出: 
        // [
        //   [1,0,1],
        //   [0,0,0],
        //   [1,0,1]
        // ]

        // 示例 2:
        // 输入: 
        // [
        //   [0,1,2,0],
        //   [3,4,5,2],
        //   [1,3,1,5]
        // ]
        // 输出: 
        // [
        //   [0,0,0,0],
        //   [0,4,5,0],
        //   [0,3,1,0]
        // ]
        // 进阶:
        // 一个直接的解决方案是使用  O(mn) 的额外空间，但这并不是一个好的解决方案。
        // 一个简单的改进方案是使用 O(m + n) 的额外空间，但这仍然不是最好的解决方案。
        // 你能想出一个常数空间的解决方案吗？

        public void SetZeroes(int[][] matrix) 
        {
            
        }

        // 方法 1：额外存储空间方法
        // 想法
        // 如果矩阵中任意一个格子有零我们就记录下它的行号和列号，
        // 这些行和列的所有格子在下一轮中全部赋为零。

        // 算法
        // 我们扫描一遍原始矩阵，找到所有为零的元素。
        // 如果我们找到 [i, j] 的元素值为零，我们需要记录下行号 i 和列号 j。
        // 用两个 sets ，一个记录行信息一个记录列信息。
        // if cell[i][j] == 0 
        // {
        //     row_set.add(i)
        //     column_set.add(j)
        // }
        // 最后，我们迭代原始矩阵，对于每个格子检查行 r 和列 c 是否被标记过，
        // 如果是就将矩阵格子的值设为 0。
        // if r in row_set or c in column_set 
        // {
        //     cell[r][c] = 0
        // }


        // 方法 2：O(1)空间的暴力
        // 想法
        // 在上面的方法中我们利用额外空间去记录需要置零的行号和列号，
        // 通过修改原始矩阵可以避免额外空间的消耗。
        // 算法

        // 遍历原始矩阵，如果发现如果某个元素 cell[i][j] 为 0，
        // 我们将第 i 行和第 j 列的所有非零元素设成很大的负虚拟值（比如说 -1000000）。
        // 注意，正确的虚拟值取值依赖于问题的约束，任何允许值范围外的数字都可以作为虚拟之。
        // 最后，我们便利整个矩阵将所有等于虚拟值（常量在代码中初始化为 MODIFIED）的元素设为 0。

        // 想法
        // 第二种方法不高效的地方在于我们会重复对同一行或者一列赋零。
        // 我们可以推迟对行和列赋零的操作。
        // 我们可以用每行和每列的第一个元素作为标记，
        // 这个标记用来表示这一行或者这一列是否需要赋零。
        // 这意味着对于每个节点不需要访问 M+N 个格子而是只需要对标记点的两个格子赋值。

        // if cell[i][j] == 0 
        // {
        //     cell[i][0] = 0
        //     cell[0][j] = 0
        // }
        // 这些标签用于之后对矩阵的更新，如果某行的第一个元素为零就将整行置零，
        // 如果某列的第一个元素为零就将整列置零。

        // 算法
        // 遍历整个矩阵，如果 cell[i][j] == 0 就将第 i 行和第 j 列的第一个元素标记。
        // 第一行和第一列的标记是相同的，都是 cell[0][0]，
        // 所以需要一个额外的变量告知第一列是否被标记，同时用 cell[0][0] 继续表示第一行的标记。
        // 然后，从第二行第二列的元素开始遍历，如果第 r 行或者第 c 列被标记了，
        // 那么就将 cell[r][c] 设为 0。
        // 这里第一行和第一列的作用就相当于方法一中的 row_set 和 column_set 。
        // 然后我们检查是否 cell[0][0] == 0 ，如果是则赋值第一行的元素为零。
        // 然后检查第一列是否被标记，如果是则赋值第一列的元素为零。
        
        public void SetZeroes1(int[][] matrix) 
        {
            if(matrix == null)
            {
                return;
            }
            bool isCol = false;
            int R = matrix.Length;
            int C = matrix[0].Length;

            for (int i = 0; i < R; i++) 
            {
                if (matrix[i][0] == 0) 
                {
                    isCol = true;
                }
                for (int j = 1; j < C; j++) 
                {
                    if (matrix[i][j] == 0) 
                    {
                        matrix[0][j] = 0;
                        matrix[i][0] = 0;
                    }
                }
            }

            for (int i = 1; i < R; i++) 
            {
                for (int j = 1; j < C; j++) 
                {
                    if (matrix[i][0] == 0 || matrix[0][j] == 0) 
                    {
                        matrix[i][j] = 0;
                    }
                }
            }

            if (matrix[0][0] == 0) 
            {
                for (int j = 0; j < C; j++) 
                {
                    matrix[0][j] = 0;
                }
            }

            if (isCol) 
            {
                for (int i = 0; i < R; i++) 
                {
                    matrix[i][0] = 0;
                }
            }
        }

    }
}

