using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //[5,7,7,8,8,10]
        public int[] SearchRange_Test(int[] nums, int target) 
        {
            var left = searchRangeLeft(nums,target);
            var right = searchRangeRight(nums,target);
            int[] ans = {left,right};
            return ans;
        }

        private int searchRangeLeft(int[] nums,int target)
        {
            int left = 0;
            int right = nums.Length;
            while(left < right)
            {
                int pivot =( left + right)/2;
                if(nums[pivot] == target)
                {
                    right = pivot;
                }
                else if(nums[pivot] > target)
                {
                    right = pivot;
                }
                else
                {
                    left = pivot + 1;
                }
            }
            // target 比所有数都大
            if (left == nums.Length) return -1;
            // 类似之前算法的处理方式
            return nums[left] == target ? left : -1;
        }

        private int searchRangeRight(int[] nums,int target)
        {
            int left = 0;
            int right = nums.Length;
            while(left < right)
            {
                int pivot =(left + right)/2;
                if(nums[pivot] == target)
                {
                    left = pivot + 1;
                }
                else if(nums[pivot] > target)
                {
                    right = pivot;
                }
                else
                {
                    left = pivot + 1;
                }
            }
            // target 比所有数都大
            if (left == 0) return -1;
            return nums[left-1] == target ? (left-1) : -1;
        }
        
        // 给定一个按照升序排列的整数数组 nums，和一个目标值 target。
        // 找出给定目标值在数组中的开始位置和结束位置。
        // 你的算法时间复杂度必须是 O(log n) 级别。
        // 如果数组中不存在目标值，返回 [-1, -1]。


        // 示例 1:
        // 输入: nums = [5,7,7,8,8,10], target = 8
        // 输出: [3,4]
        // 示例 2:

        // 输入: nums = [5,7,7,8,8,10], target = 6
        // 输出: [-1,-1]

        //[1]1

        //[0,-1]
        public int[] SearchRange(int[] nums, int target) 
        {
            int[] ans = {-1,-1};
            var firstIndex = searchIndex(nums,target,true);
            if (firstIndex == nums.Length || nums[firstIndex] != target) 
            {
                return ans;
            }
            ans[0] = firstIndex;
            ans[1] = searchIndex(nums, target, false) -1;
            return ans;
        }

        private int searchIndex(int[] nums,int target,bool isSearchInLeft)
        {
            int leftIndex = 0;
            int rightIndex = nums.Length;
            while(leftIndex < rightIndex)
            {
                int pivot = (leftIndex + rightIndex)/2;
                if(nums[pivot] > target|| (isSearchInLeft&& nums[pivot] == target))
                {
                    rightIndex = pivot;
                }
                else
                {
                    leftIndex = pivot + 1;
                }
            }
            return leftIndex;
        }



    }
}


