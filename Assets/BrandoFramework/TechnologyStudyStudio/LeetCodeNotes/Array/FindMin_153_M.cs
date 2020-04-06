using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //153. 寻找旋转排序数组中的最小值
        //假设按照升序排序的数组在预先未知的某个点上进行了旋转。

        //(例如，数组[0, 1, 2, 4, 5, 6, 7] 可能变为[4, 5, 6, 7, 0, 1, 2] )。

        //请找出其中最小的元素。

        //你可以假设数组中不存在重复元素。

        //示例 1:

        //输入: [3,4,5,1,2]
        //输出: 1
        //示例 2:

        //输入: [4,5,6,7,0,1,2]
        //输出: 0
        public int FindMin(int[] nums)
        {
            return FindMin(nums, 0, nums.Length - 1);
        }
        private int FindMin(int[] nums, int left, int right)
        {
            if(left == right)
            {
                return nums[left];
            }
            int mid = (left + right) / 2;
            if(nums[mid] > nums[right])
            {
                return FindMin(nums, mid + 1, right);
            }
            //else if(nums[mid] < nums[left])
            //{
            //    return FindMin(nums, left, mid);
            //}
            else
            {
                return FindMin(nums, left, mid);
            }
        }
    }
}