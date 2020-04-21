

using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个整数数组 nums ，找到一个具有最大和的连续子数组（子数组最少包含一个元素），返回其最大和。
        // 示例:
        // 输入: [-2,1,-3,4,-1,2,1,-5,4],
        // 输出: 6
        // 解释: 连续子数组 [4,-1,2,1] 的和最大，为 6。

        public int MaxSubArray1(int[] nums)
        {
            int result = nums[0];
            int tempSum = 0;
            foreach (var num in nums)
            {
                if (tempSum > 0)
                {
                    tempSum = tempSum + num;
                }
                else
                {
                    tempSum = num;
                }
                result = tempSum > result ? tempSum : result;
            }
            return result;
        }

        public int MaxSubArray2(int[] nums)
        {
            int result = nums[0];
            int tempSum = 0;
            for (var i = 0; i < nums.Length; i++)
            {
                if (tempSum > 0)
                {
                    tempSum = tempSum + nums[i];
                }
                else
                {
                    tempSum = nums[i];
                }
                result = tempSum > result ? tempSum : result;
            }
            return result;
        }

        public int MaxSubArray3(int[] nums)
        {
            //DP
            if (nums == null || nums.Length == 0)
            {
                return 0;
            }
            var maxNum = new int[nums.Length];
            maxNum[0] = nums[0];
            int result = maxNum[0];
            for (var i = 1;i< nums.Length; i++)
            {
                maxNum[i] = Math.Max(nums[i], nums[i] + maxNum[i - 1]);
                result = Math.Max(result, maxNum[i]);
            }
            return result;
        }

        public int MaxSubArray4(int[] nums)
        {
            //DP
            if (nums == null || nums.Length == 0)
            {
                return 0;
            }
            int curNum = nums[0];
            int result = curNum;
            for (var i = 1; i < nums.Length; i++)
            {
                curNum = Math.Max(nums[i], nums[i] + curNum);
                result = Math.Max(result, curNum);
            }
            return result;
        }
    }
}
