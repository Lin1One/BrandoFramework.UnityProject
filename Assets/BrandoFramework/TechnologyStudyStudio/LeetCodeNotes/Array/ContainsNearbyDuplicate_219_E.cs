#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //219. 存在重复元素 II
        //给定一个整数数组和一个整数 k，判断数组中是否存在两个不同的索引 i 和 j，使得 nums[i] = nums[j]，并且 i 和 j 的差的 绝对值 至多为 k。

        //示例 1:
        //输入: nums = [1,2,3,1], k = 3
        //输出: true

        //示例 2:
        //输入: nums = [1,0,1,1], k = 1
        //输出: true

        //示例 3:
        //输入: nums = [1,2,3,1,2,3], k = 2
        //输出: false

        public bool ContainsNearbyDuplicate(int[] nums, int k)
        {
            if(nums == null || k <= 0)
            {
                return false;
            }
            Dictionary<int, int> ValToIndex = new Dictionary<int, int>();
            for(var i = 0;i<nums.Length;i++)
            {
                if(ValToIndex.ContainsKey(nums[i]))
                {
                    var index = ValToIndex[nums[i]];
                    if((i - index) <= k)
                    {
                        return true;
                    }
                    else
                    {
                        ValToIndex[nums[i]] = i;
                    }
                }
                else
                {
                    ValToIndex.Add(nums[i], i);
                }
            }
            return false;
        }

        public bool containsNearbyDuplicate(int[] nums, int k)
        {
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < nums.Length; i++)
            {
                if (set.Contains(nums[i]))
                {
                    return true;
                }
                set.Add(nums[i]);
                if (set.Count > k)
                {
                    set.Remove(nums[i - k]);
                }
            }
            return false;
        }
    }
}

