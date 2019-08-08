using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 52. N皇后 II
        // n 皇后问题研究的是如何将 n 个皇后放置在 n×n 的棋盘上，并且使皇后彼此之间不能相互攻击。
        // 给定一个整数 n，返回 n 皇后不同的解决方案的数量。

        // 示例:
        // 输入: 4
        // 输出: 2
        // 解释: 4 皇后问题存在如下两个不同的解法。
        // [
        //  [".Q..",  // 解法 1
        //   "...Q",
        //   "Q...",
        //   "..Q."],

        //  ["..Q.",  // 解法 2
        //   "Q...",
        //   "...Q",
        //   ".Q.."]
        // ]

        private int queensResult;
        private int queensResultCount = 0;
        public int TotalNQueens(int n) 
        {
            bool[] rowUsed = new bool[n];
            bool[] colUsed = new bool[n];
            //主对角线 i>=j时 [i-j] i<j时 [j-i+n-1]
            bool[] leftUpToRightDownLinesUsed = new bool[2*n-1];
            //次对角线 [i+j]
            bool[] RightUpToLeftDownLinesUsed = new bool[2*n-1];
            SolveBack(leftUpToRightDownLinesUsed,
                RightUpToLeftDownLinesUsed,
                rowUsed,
                colUsed,
                n,
                0);
            return queensResultCount;
        }

        private  void SolveBack(
            bool[] zhuxieUsed,
            bool[] fuxieUsed,
            bool[] rowUsed, 
            bool[] colUsed, 
            int n,
            int currentRow
            )
        {
            if (currentRow == n) 
            {
                queensResultCount++;
            }
            else 
            {
                int i = currentRow;
                for (int j = 0; j < n;j++)
                {
                    bool isUse;
                    if (i >= j)
                    {
                        isUse = rowUsed[i] || colUsed[j] || zhuxieUsed[i-j]|| fuxieUsed[i+j];
                    }
                    else 
                    {
                        isUse = rowUsed[i] || colUsed[j] || zhuxieUsed[j-i+n-1] || fuxieUsed[i+j];
                    }
                    if (!isUse)
                    {
                        queensResult++;
                        rowUsed[i] = true;
                        colUsed[j] = true;
                        if (i >=j)
                        {
                            zhuxieUsed[i-j] = true;
                        }
                        else 
                        {
                            zhuxieUsed[j-i+n-1] = true;
                        }
                        fuxieUsed[i+j] = true;

                        SolveBack(zhuxieUsed,fuxieUsed,rowUsed,colUsed,n,i + 1);

                        queensResult--;
                        rowUsed[i] = false;
                        colUsed[j] = false;
                        if (i >=j)
                        {
                            zhuxieUsed[i-j] = false;
                        }
                        else 
                        {
                            zhuxieUsed[j-i+n-1] = false;
                        }
                        fuxieUsed[i+j] = false;
                    }
                }
            }
        }
    }
}




