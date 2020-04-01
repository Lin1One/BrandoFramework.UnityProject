using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 240. 搜索二维矩阵 II
        // 编写一个高效的算法来搜索 m x n 矩阵 matrix 中的一个目标值 target。
        // 该矩阵具有以下特性：

        // 每行的元素从左到右升序排列。
        // 每列的元素从上到下升序排列。
        // 示例:

        // 现有矩阵 matrix 如下：

        //[1,1] 2
        // [
        //   [1,   4,  7, 11, 15],
        //   [2,   5,  8, 12, 19],
        //   [3,   6,  9, 16, 22],
        //   [10, 13, 14, 17, 24],
        //   [18, 21, 23, 26, 30]
        // ]

        public bool SearchMatrix0402(int[,] matrix, int target)
        {
            if (matrix == null)
            {
                return false;
            }
            int row = matrix.Length - 1;
            int col = 0;
            while (row >= 0 && col < matrix.GetLength(2))
            {
                if (matrix[row, col] > target)
                {
                    row--;
                }
                else if (matrix[row, col] < target)
                {
                    col++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public bool SearchMatrix(int[,] matrix, int target) 
        {
            if(matrix == null || matrix.Length == 0|| matrix.GetLength(1) == 0)
                return false;
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            int minLength = Math.Min(m,n);
            int targetIndex = 0;
            for(targetIndex = 0 ;targetIndex< minLength;targetIndex++)
            {
                if(matrix[targetIndex,targetIndex] >= target)
                {
                    if(matrix[targetIndex,targetIndex] == target)
                        return true;
                    for(var i = 0;i < targetIndex;i++)
                    {
                        if(matrix[i,targetIndex] == target)
                        return true;
                    }
                    for(var i = 0;i < targetIndex;i++)
                    {
                        if(matrix[targetIndex,i] == target)
                        return true;
                    }
                    return false;
                }
            }
            if(m > n)
            {
                var lastLength = m - n;
                for(var i = 0;i < lastLength;i++)
                {
                    if(matrix[targetIndex + i,targetIndex] >= target)
                    {
                        for(var j = 0;j <= targetIndex;i++)
                        {
                            if(matrix[targetIndex + i,j] == target)
                            {
                                return true;
                            }
                        }
                        return false;

                    }
                }
            }
            else
            {
                var lastLength = n - m;
                for(var i = 0;i < lastLength;i++)
                {
                    if(matrix[targetIndex ,targetIndex + i] >= target)
                    {
                        for(var j = 0;j <= targetIndex;i++)
                        {
                            if(matrix[j,targetIndex+ i] == target)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            
            return false;

            //for(var i == )
        }



        public bool SearchMatrix(int[][] matrix, int target)
        {       // 方法一：暴力法
                // 对于每一行我们可以像搜索未排序的一维数组——通过检查每个元素来判断是否有目标值。

            // 算法：
            // 这个算法并没有做到聪明的事情。
            // 我们循环数组，依次检查每个元素。
            // 如果，我们找到了，我们返回 true。
            // 否则，对于搜索到末尾都没有返回的循环，
            // 我们返回 false。此算法在所有情况下都是正确的答案，
            // 因为我们耗尽了整个搜索空间。
            for (int i = 0; i < matrix.Length; i++) 
            {
                for (int j = 0; j < matrix[0].Length; j++) 
                {
                    if (matrix[i][j] == target) 
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public bool SearchMatrix1(int[,] matrix, int target)
        {        // 方法二：二分法搜索
                 // 矩阵已经排过序，就需要使用二分法搜索以加快我们的算法。

            // 算法：
            // 首先，我们确保矩阵不为空。
            // 那么，如果我们迭代矩阵对角线，
            // 从当前元素对列和行搜索，
            // 我们可以保持从当前 (row,col) 对开始的行和列为已排序。 
            //因此，我们总是可以二分搜索这些行和列切片。
            //我们以如下逻辑的方式进行 : 
            //在对角线上迭代，二分搜索行和列，
            //直到对角线的迭代元素用完为止（意味着我们可以返回 false ）
            //或者找到目标（意味着我们可以返回 true ）。
            //binary search 函数的工作原理和普通的二分搜索一样,
            //但需要同时搜索二维数组的行和列。
            if (matrix == null || matrix.GetLength(0) == 0) 
            {
                return false;
            }
            int m = matrix.GetLength(0);
            int n = matrix.GetLength(1);
            // iterate over matrix diagonals
            int shorterDim = Math.Min(matrix.Length, matrix.GetLength(1));
            for (int i = 0; i < shorterDim; i++) 
            {
                bool verticalFound = BinarySearch(matrix, target, i, true);
                bool horizontalFound = BinarySearch(matrix, target, i, false);
                if (verticalFound || horizontalFound) 
                {
                    return true;
                }
            }
            return false; 
        }
        private bool BinarySearch(int[,] matrix, int target, int start, bool vertical)
         {
            int lo = start;
            int hi = vertical ? matrix.GetLength(1)-1 : matrix.GetLength(0)-1;

            while (hi >= lo) 
            {
                int mid = (lo + hi)/2;
                if (vertical) 
                { // searching a column
                    if (matrix[start,mid] < target) 
                    {
                        lo = mid + 1;
                    }
                    else if (matrix[start,mid] > target) 
                    {
                        hi = mid - 1;
                    } 
                    else 
                    {
                        return true;
                    }
                } 
                else 
                { // searching a row
                    if (matrix[mid,start] < target) 
                    {
                        lo = mid + 1;
                    } 
                    else if (matrix[mid,start] > target) 
                    {
                        hi = mid - 1;
                    }
                    else 
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // 复杂度分析
        // 时间复杂度：O(lg(n!))。
        // 这个算法产生的时间复杂度并不是特别明显的是 O(lg(n!))，
        // 所以让我们一步一步地分析它。在主循环中执行的工作量逐渐最大，
        // 它运行 min(m,n) 次迭代，其中 m 表示行数，n 表示列数。
        // 在每次迭代中，我们对长度为 m-i 和 n-i 的数组片执行两次二分搜索。
        // 因此，循环的每一次迭代都以 O(lg(m-i)+lg(n-i)) 时间运行，
        // 其中 i 表示当前迭代。我们可以将其简化为 O(lg(n-i))= O(lg(n-i))O(2⋅lg(n−i))=O(lg(n−i)) 
        // 在最坏的情况是 n\approx mn≈m 。当 n \ll mn≪m 时会发生什么（不损失一般性）；
        // nn 将在渐近分析中占主导地位。
        //通过汇总所有迭代的运行时间，我们得到以下表达式：
        // \quad {O}(lg(n) + lg(n-1) + lg(n-2) + \ldots + lg(1))
        // O(lg(n)+lg(n−1)+lg(n−2)+…+lg(1))

        // 然后，我们可以利用对数乘法规则（lg(a)+lg(b)=lg(ab)lg(a)+lg(b)=lg(ab)）将复杂度改写为：
        // {O}(lg(n) + lg(n-1) + lg(n-2) + \ldots + lg(1))O(lg(n)+lg(n−1)+lg(n−2)+…+lg(1))
        // ={O}(lg(n \cdot (n-1) \cdot (n-2) \cdot \ldots \cdot 1))=O(lg(n⋅(n−1)⋅(n−2)⋅…⋅1))
        // = {O}(lg(1 \cdot \ldots \cdot (n-2) \cdot (n-1) \cdot n))=O(lg(1⋅…⋅(n−2)⋅(n−1)⋅n))
        // = {O}(lg(n!))=O(lg(n!))

        // 空间复杂度：O(1)，因为我们的二分搜索实现并没有真正地切掉矩阵中的行和列的副本，
        // 我们可以避免分配大于常量内存。



        // 方法三：搜索空间的缩减
        // 我们可以将已排序的二维矩阵划分为四个子矩阵，
        // 其中两个可能包含目标，其中两个肯定不包含。

        // 算法：
        // 由于该算法是递归操作的，
        // 因此可以通过它的基本情况和递归情况的正确性来判断它的正确性。

        // 基本情况 ：
        // 对于已排序的二维数组，有两种方法可以确定一个任意元素目标是否可以用常数时间判断。
        // 第一，如果数组的区域为零，则它不包含元素，因此不能包含目标。
        // 其次，如果目标小于数组的最小值或大于数组的最大值，那么矩阵肯定不包含目标值。

        // 递归情况：
        // 如果目标值包含在数组内，因此我们沿着索引行的矩阵中间列 ，
        // matrix[row-1][mid] < target < matrix[row][mid]matrix[row−1][mid]<target<matrix[row][mid] ，
        // （很明显，如果我们找到 target ，我们立即返回 true）。
        // 现有的矩阵可以围绕这个索引分为四个子矩阵；
        // 左上和右下子矩阵不能包含目标（通过基本情况部分来判断），
        // 所以我们可以从搜索空间中删除它们 。另外，左下角和右上角的子矩阵是二维矩阵，
        // 因此我们可以递归地将此算法应用于它们。


        public bool SearchMatrix0402(int[][] matrix, int target)
        {
            //方法四：
            //因为矩阵的行和列是排序的（分别从左到右和从上到下），所以在查看任何特定值时，我们可以修剪O(m)O(m)或O(n)O(n)元素。

            //算法：
            //首先，我们初始化一个指向矩阵左下角的(row，col)指针。然后，直到找到目标并返回 true（或者指针指向矩阵维度之外的(row，col) 为止，
            //我们执行以下操作：如果当前指向的值大于目标值，则可以 “向上” 移动一行。 
            //否则，如果当前指向的值小于目标值，则可以移动一列。
            //不难理解为什么这样做永远不会删减正确的答案；
            //因为行是从左到右排序的，所以我们知道当前值右侧的每个值都较大。 
            //因此，如果当前值已经大于目标值，我们知道它右边的每个值会比较大。
            //也可以对列进行非常类似的论证，因此这种搜索方式将始终在矩阵中找到目标（如果存在）。


            // start our "pointer" in the bottom-left
            int row = matrix.Length - 1;
            int col = 0;

            while (row >= 0 && col < matrix[0].Length)
            {
                if (matrix[row][col] > target)
                {
                    row--;
                }
                else if (matrix[row][col] < target)
                {
                    col++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}


