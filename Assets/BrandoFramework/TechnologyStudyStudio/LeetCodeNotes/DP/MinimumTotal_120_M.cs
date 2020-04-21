using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.LeetCode
{
    public partial class Solution
    {

        //120. 三角形最小路径和
        //给定一个三角形，找出自顶向下的最小路径和。每一步只能移动到下一行中相邻的结点上。

        //例如，给定三角形：

        //[
        //     [2],
        //    [3,4],
        //   [6,5,7],
        //  [4,1,8,3]
        //]
        //自顶向下的最小路径和为 11（即，2 + 3 + 5 + 1 = 11）。

        //说明：

        //如果你可以只使用 O(n) 的额外空间（n 为三角形的总行数）来解决这个问题，那么你的算法会很加分。

        public int MinimumTotal1(IList<IList<int>> triangle)
        {
            //自底向上
            int rowCount = triangle.Count;
            for(var i = rowCount - 2;i>= 0;i--)
            {
                var curRow = triangle[i];
                for (var j = 0;j < curRow.Count;j++)
                {
                    triangle[i][j] = Math.Min(triangle[i + 1][j], triangle[i + 1][j + 1]) + triangle[i][j];
                }
            }
            return triangle[0][0];
        }

        public int MinimumTotal2(IList<IList<int>> triangle)
        {
            //O(N) 空间
            //自底向上
            int rowCount = triangle.Count;
            int colMaxCount = triangle[rowCount - 1].Count;
            int[] colMinNum = new int[colMaxCount + 1];
            for (var i = rowCount - 1; i >= 0; i--)
            {
                var curRow = triangle[i];
                for (var j = 0; j < curRow.Count; j++)
                {
                    colMinNum[j] = Math.Min(colMinNum[j], colMinNum[j + 1]) + triangle[i][j];
                }
            }
            return colMinNum[0];
        }
    }
}
