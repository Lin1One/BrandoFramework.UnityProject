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
        //217. 存在重复元素
        //给定一个整数数组，判断是否存在重复元素。
        //如果任何值在数组中出现至少两次，函数返回 true。如果数组中每个元素都不相同，则返回 false。

        //示例 1:
        //输入: [1,2,3,1]
        //输出: true

        //示例 2:
        //输入: [1,2,3,4]
        //输出: false

        //示例 3:
        //输入: [1,1,1,3,3,4,3,2,4,2]
        //输出: true

        public bool ContainsDuplicate200330(int[] nums)
        {
            Array.Sort(nums);
            for (int i = 0; i < nums.Length - 1; ++i)
            {
                if (nums[i] == nums[i + 1])
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsDuplicate(int[] nums)
        {
            var numSet = new HashSet<int>();
            foreach(var num in nums)
            {
                if(numSet.Contains(num))
                {
                    return true;
                }
                numSet.Add(num);
            }
            return false;
        }
    }
}

