#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 35. 搜索插入位置
        // 给定一个排序数组和一个目标值，在数组中找到目标值，
        // 并返回其索引。如果目标值不存在于数组中，返回它将会被按顺序插入的位置。

        // 你可以假设数组中无重复元素。
        // 示例 1:
        // 输入: [1,3,5,6], 5
        // 输出: 2

        // 示例 2:
        // 输入: [1,3,5,6], 2
        // 输出: 1

        // 示例 3:
        // 输入: [1,3,5,6], 7
        // 输出: 4

        // 示例 4:
        // 输入: [1,3,5,6], 0
        // 输出: 0

        public int SearchInsert(int[] nums, int target) 
        {
            return SearchInsert_binarySearch(nums,0,nums.Length - 1,target);
        }

        private int SearchInsert_binarySearch(int[] nums,int left,int right,int target)
        {
            while(left <= right)
            {
                var mid = (left + right)/2;
                if(nums[mid] == target)
                {
                    return mid;
                }
                if(nums[mid]> target)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return left;
        }

        //二分查找模板
        public int searchInsertTemplate2(int[] nums, int target) 
        {
            int left = 0, right = nums.Length; // 注意
            while(left < right) 
            { // 注意
                int mid = (left + right) / 2; // 注意
                if(nums[mid] == target) 
                {
                    // 相关逻辑
                } 
                else if(nums[mid] < target) 
                {
                    left = mid + 1; // 注意
                } 
                else 
                {
                    right = mid; // 注意
                }
            }
            // 相关返回值
            return 0;
        }
        public int searchInsertTemplate1(int[] nums, int target)
        {
            int left = 0, right = nums.Length - 1; // 注意
            while(left <= right) 
            { // 注意
                int mid = (left + right) / 2; // 注意
                if(nums[mid] == target) 
                { // 注意
                    // 相关逻辑
                } 
                else if(nums[mid] < target) 
                {
                    left = mid + 1; // 注意
                } 
                else 
                {
                    right = mid - 1; // 注意
                }
            }
            // 相关返回值
            return 0;
        }
    }
}


