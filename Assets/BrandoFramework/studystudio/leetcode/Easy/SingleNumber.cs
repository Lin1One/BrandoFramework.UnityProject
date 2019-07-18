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
        // 给定一个非空整数数组，除了某个元素只出现一次以外，其余每个元素均出现两次。
        // 找出那个只出现了一次的元素。
        // 说明：
        // 你的算法应该具有线性时间复杂度。 你可以不使用额外空间来实现吗？

        // 示例 1:
        // 输入: [2,2,1]
        // 输出: 1

        // 示例 2:
        // 输入: [4,1,2,1,2]
        // 输出: 4

        //列表保存
        public int SingleNumber1(int[] nums) 
        {
            var tempList =  new List<int>();
            for (var i = 0;i < nums.Length;i++)
            {
                if (tempList.Contains(nums[i]))
                {
                    tempList.Remove(nums[i]);
                }
                else
                {
                    tempList.Add(nums[i]);
                }
            }       
            return tempList[0];
        }

        //字典保存
        public int SingleNumber2(int[] nums) 
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (var i = 0;i < nums.Length;i++)
            {
                if(!dic.ContainsKey(nums[i]))
                {
                    dic.Add(nums[i],1);
                }
                else
                {
                    dic[nums[i]]++;
                }
            }
            foreach(var key in dic.Keys)
            {
                if (dic[key] == 1)
                {
                    return key;
                }
            }
            return -1; // can't find it.
        }


        //异或运算！
        public int SingleNumber3(int[] nums)
        {
            int ans = nums[0];
            if (nums.Length > 1) 
            {
                for (int i = 1; i < nums.Length; i++) 
                {
                    ans = ans ^ nums[i];
                }
            }
            return ans; 
        }



    // def singleNumber(self, nums):
    //     """
    //     :type nums: List[int]
    //     :rtype: int
    //     """
    //     hash_table = {}
    //     for i in nums:
    //         try:
    //             hash_table.pop(i)
    //         except:
    //             hash_table[i] = 1
    //     return hash_table.popitem()[0]
    }

}

