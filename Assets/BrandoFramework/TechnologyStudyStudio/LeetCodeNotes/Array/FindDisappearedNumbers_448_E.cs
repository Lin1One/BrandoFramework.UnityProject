#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 448. 找到所有数组中消失的数字
        // 给定一个范围在  1 ≤ a[i] ≤ n ( n = 数组大小 ) 的 整型数组，
        // 数组中的元素一些出现了两次，另一些只出现一次。
        // 找到所有在 [1, n] 范围之间没有出现在数组中的数字。
        // 您能在不使用额外空间且时间复杂度为O(n)的情况下完成这个任务吗? 
        // 你可以假定返回的数组不算在额外空间内。

        // 示例:

        // 输入:
        // [4,3,2,7,8,2,3,1]

        // 输出:
        // [5,6]

        public IList<int> FindDisappearedNumbers(int[] nums) 
        {
            // [1,2,3,4,5,6,7,8]
            // [1,2,2,3,3,4,7,8]
            // [3,4,5,6]
//[1,1]
            // [5,6]
            IList<int> list = new List<int>();
            Array.Sort(nums);
            int noExistNum = 1;
            int length = nums.Length;
            for(var i = 0;i< length;i++)
            {
                if(nums[i] == noExistNum)
                {
                    noExistNum++;
                }
                else if(nums[i] > noExistNum)
                {
                    for(var j = 0;j < nums[i] - noExistNum; j++)
                    {
                        list.Add(noExistNum + j);
                    }
                    noExistNum = nums[i]+1;
                }
            }
            if(noExistNum <= length)
            {
                for(var i = noExistNum;i <= length; i++)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public IList<int> FindDisappearedNumbers2(int[] nums)
        {
            List<int> result = new List<int>();
            if(nums == null || nums.Length == 0)
            {
                return result;
            }
            for(var i = 0;i< nums.Length;i++)
            {
                var value = Math.Abs( nums[i]) - 1;
                if(nums[value] > 0)
                {
                    nums[value] *= -1;
                }
            }
            for(var i = 1;i <= nums.Length;i++)
            {
                if(nums[i - 1] > 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}


