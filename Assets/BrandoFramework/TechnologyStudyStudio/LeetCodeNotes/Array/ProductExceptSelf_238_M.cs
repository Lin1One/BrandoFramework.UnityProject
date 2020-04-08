using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Linq;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 238. 除自身以外数组的乘积
        // 给定长度为 n 的整数数组 nums，其中 n > 1，返回输出数组 output ，
        // 其中 output[i] 等于 nums 中除 nums[i] 之外其余各元素的乘积。

        // 示例:
        // 输入: [1,2,3,4]
        // 输出: [24,12,8,6]
        // 说明: 请不要使用除法，且在 O(n) 时间复杂度内完成此题。
        // 进阶：
        // 你可以在常数空间复杂度内完成这个题目吗？
        // （ 出于对空间复杂度分析的目的，输出数组不被视为额外空间。）


        public int[] ProductExceptSelf0408(int[] nums)
        {
            int[] result = new int[nums.Length];
            int leftProduct = 1;
            for(var i = 0;i< nums.Length; i++)
            {
                result[i] = leftProduct;
                leftProduct *= nums[i];
            }
            int rightProduct = 1;
            for (var i = nums.Length - 1; i >= 0 ; i--)
            {
                result[i] *= rightProduct;
                rightProduct *= nums[i];
            }
            return result;
        }

        public int[] ProductExceptSelf(int[] nums) 
        {
            var length = nums.Length;
            int[] result = new int[length];
            int leftProduct = 1;
            for(var i = 0;i < length;i++)
            {
                result[i] = leftProduct;
                leftProduct *= nums[i];
            }
            int leftRightProduct = 1;
            for(var i = length - 1;i >= 0; i--)
            {
                result[i] *= leftRightProduct;
                leftRightProduct *= nums[i];
            }
            return result;
        }

        public int[] ProductExceptSelf2(int[] nums)
        {
            int[] result = Enumerable.Repeat(1, nums.Length).ToArray();
            for (int i = 1; i < nums.Length; i++)
            {
                result[i] = result[i-1] * nums[i - 1];
            }
            int right = 1;
            for (int i = nums.Length - 1; i >= 0; i--)
            {
                result[i] *= right;
                right *= nums[i];
            }
            return result;
        }

        public int[] productExceptSelf(int[] nums) 
        {
            //左区，乘 右区

            // 因为空间复杂度要求O(1)、不能使用除法，因此一定需要在乘法过程中得到所有答案；
            // 我们可以将res数组列成乘积形式，形成一个矩阵，可以发现矩阵次主角线全部为1（因为当前数字不相乘，因此等价为乘1）；
            // 因此，我们分别计算矩阵的上三角和下三角，并且在计算过程中储存过程值，最终可以在遍历2遍nums下完成结果计算。
            // res					
            // res[0] =	1	num[1]	...	num[n-2]	num[n-1]
            // res[1] =	num[0]	1	...	num[n-2]	num[n-1]
            // ...	...	...	...	num[n-2]	num[n-1]
            // res[n-2] =	num[0]	num[1]	...	1	num[n-1]
            // res[n-1] =	num[0]	num[1]	...	num[n-2]	1
            int[] res = new int[nums.Length];
            int k = 1;
            for(int i = 0; i < nums.Length; i++)
            {
                res[i] = k;
                k = k * nums[i]; // 此时数组存储的是除去当前元素左边的元素乘积
            }
            k = 1;
            for(int i = nums.Length - 1; i >= 0; i--)
            {
                res[i] *= k; // k为该数右边的乘积。
                k *= nums[i]; // 此时数组等于左边的 * 该数右边的。
            }
            return res;
        }
    }
}


