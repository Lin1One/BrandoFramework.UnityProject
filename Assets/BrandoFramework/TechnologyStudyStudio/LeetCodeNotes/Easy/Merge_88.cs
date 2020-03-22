#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 88. 合并两个有序数组
        // 给定两个有序整数数组 nums1 和 nums2，将 nums2 合并到 nums1 中，
        // 使得 num1 成为一个有序数组。

        // 说明:

        // 初始化 nums1 和 nums2 的元素数量分别为 m 和 n。
        // 你可以假设 nums1 有足够的空间（空间大小大于或等于 m + n）来保存 nums2 中的元素。
        // 示例:

        // 输入:
        // nums1 = [1,2,3,0,0,0], m = 3
        // nums2 = [2,5,6],       n = 3

        // 输出: [1,2,2,3,5,6]

        public void Merge0321(int[] nums1, int m, int[] nums2, int n)
        {
            int numCount = m + n - 1;
            int ptr1 = m - 1;
            int ptr2 = n - 1;
            while(ptr1 > -1 && ptr2 > -1)
            {
                int numInArray1 = nums1[ptr1];
                int numInArray2 = nums2[ptr2];
                if (numInArray1 > numInArray2)
                {
                    nums1[numCount] = numInArray1;
                    --m;
                }
                else
                {
                    nums1[numCount] = numInArray2;
                    --n;
                }
                numCount--;
            }
            Array.Copy(nums2, 0, nums1, 0, ptr2 + 1);
        }


        // 方法一 : 合并后排序
        // 最朴素的解法就是将两个数组合并之后再排序。
        // 这是由于这种方法没有利用两个数组本身已经有序这一点。
        // public void merge(int[] nums1, int m, int[] nums2, int n) 
        // {
        //     System.arraycopy(nums2, 0, nums1, m, n);
        //     Arrays.sort(nums1);
        // }


        // 方法二 : 双指针 / 从前往后
        // 一般而言，对于有序数组可以通过 双指针法 达到O(n + m)的时间复杂度。

        // 最直接的算法实现是将指针 p1 置为 nums1的开头， 
        // p2为 nums2的开头，在每一步将最小值放入输出数组中。

        // 由于 nums1 是用于输出的数组，需要将nums1中的前m个元素放在其他地方，
        // 也就需要 O(m) 的空间复杂度。

        // 方法三 : 双指针 / 从后往前
        // 方法二已经取得了最优的时间复杂度O(n + m)，
        // 但需要使用额外空间。这是由于在从头改变nums1的值时，需要把nums1中的元素存放在其他位置。

        // 如果我们从结尾开始改写 nums1 的值又会如何呢？这里没有信息，因此不需要额外空间。

        // 这里的指针 p 用于追踪添加元素的位置。


        public void Merge(int[] nums1, int m, int[] nums2, int n) 
        {
            int finalPtr= m + n - 1;
            int ptr1 = m - 1;
            int ptr2 = n - 1;
            while(ptr2 >= 0 && ptr1 >= 0)
            {
                if(nums2[ptr2] > nums1[ptr1])
                {
                    nums1[finalPtr] = nums1[ptr2];
                    ptr2--;
                }
                else
                {
                    nums1[finalPtr] = nums1[ptr1];
                    ptr1--;
                }
                finalPtr--;
            }

                Array.Copy(nums2,0,nums1,0,ptr2+1);
            
            return;
        }
    }
}


