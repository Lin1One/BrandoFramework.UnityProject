using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 33. 搜索旋转排序数组
        //  假设按照升序排序的数组在预先未知的某个点上进行了旋转。
        // ( 例如，数组 [0,1,2,4,5,6,7] 可能变为 [4,5,6,7,0,1,2] )。
        // 搜索一个给定的目标值，如果数组中存在这个目标值，则返回它的索引，否则返回 -1 。
        // 你可以假设数组中不存在重复的元素。
        // 你的算法时间复杂度必须是 O(log n) 级别。

        // 示例 1:
        // 输入: nums = [4,5,6,7,0,1,2], target = 0
        // 输出: 4

        // 示例 2:
        // 输入: nums = [4,5,6,7,0,1,2], target = 3
        // 输出: -1
        

        public int Search(int[] nums, int target) 
        {
            return 0;
        }

        private int FindRotateIndex(int[] nums,int left,int right)
        {
            if(nums[right] > nums[left])
            {
                return 0;
            }
            while(right > left)
            {
                var mid = (right+left)/2;
                if (nums[mid] > nums[mid + 1])
                    return mid + 1;
                if(nums[left] < nums[mid])
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return -1;
        }


        private int binarySearchRotateIndex(int[] nums,int left,int right,int target)
        {
            while(left <= right)
            {
                var mid = (right+left)/2;
                if(nums[mid] == target)
                {
                    return mid;
                }
                if(nums[mid] > target)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return -1;
        }

        private int binarySearchRotateIndex(int[] nums,int left,int right)
        {
            if(nums[right] >= nums[left])
            {
                return 0;
            }
            var mid = (right+left)/2;
            if(nums[mid] > nums[left])
            {
                return binarySearchRotateIndex(nums,mid + 1,right);
            }
            else if (nums[mid] < nums[left])
            {
                return binarySearchRotateIndex(nums,left,mid);
            }
            else
            {
                return mid;
            }
            return 0;
        }


        private int [] nums;
        private int target;
        public int Search1(int[] nums, int target) 
        {
            this.nums = nums;
            this.target = target;

            int n = nums.Length;

            if (n == 0)
                return -1;
            if (n == 1)
                return this.nums[0] == target ? 0 : -1;
            //查找旋转点
            int rotate_index = FindRotateIndex(0, n - 1);

            // if target is the smallest element
            if (nums[rotate_index] == target)
                return rotate_index;
            // if array is not rotated, search in the entire array
            if (rotate_index == 0)
                return search(0, n - 1);
            if (target < nums[0])
            {
                return search(rotate_index, n - 1);
            }
            else
            {
                // search in the left side
                return search(0, rotate_index);
            }
        }

        public int FindRotateIndex(int left, int right) 
        {
            if (nums[left] < nums[right])
                return 0;

            while (left <= right) 
            {
                int pivot = (left + right) / 2;
                if (nums[pivot] > nums[pivot + 1])
                    return pivot + 1;
                else 
                {
                    if (nums[pivot] < nums[left])
                        right = pivot - 1;
                    else
                        left = pivot + 1;
                }
            }
            return 0;
        }

        public int search(int left, int right) 
        {
            while (left <= right) 
            {
                int pivot = (left + right) / 2;
                if (nums[pivot] == target)
                    return pivot;
                else 
                {
                    if (target < nums[pivot])
                        right = pivot - 1;
                    else
                        left = pivot + 1;
                }
            }
            return -1;
        }

        



    }
}


