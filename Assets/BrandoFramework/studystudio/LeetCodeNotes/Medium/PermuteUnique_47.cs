using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 47. 全排列 II
        // 给定一个可包含重复数字的序列，返回所有不重复的全排列。
        
        // 示例:
        // 输入: [1,1,2]
        // 输出:
        // [
        // [1,1,2],
        // [1,2,1],
        // [2,1,1]
        // ]

        private IList<IList<int>> PermuteUniqueResult = new List<IList<int>>();
        private bool[] used;
        public IList<IList<int>> PermuteUnique(int[] nums) 
        {
            Array.Sort(nums);
            used = new bool[nums.Length];
            PermuteUnique_Backtrack(nums,new List<int>());
            return PermuteUniqueResult;
        }

        private void PermuteUnique_Backtrack(int[] nums,IList<int> resultItem)
        {
            if(resultItem.Count == nums.Length)
            {
                PermuteUniqueResult.Add(new List<int>(resultItem));
                return;
            }
            for(var i = 0 ;i < nums.Length;i++)
            {
                if(!used[i])
                {
                    if(i > 0 && nums[i] == nums[i - 1] && !used[i - 1])
                    {
                        continue;
                    }
                    resultItem.Add(nums[i]);
                    used[i] = true;
                    PermuteUnique_Backtrack(nums,resultItem);
                    used[i] = false;
                    resultItem.RemoveAt(resultItem.Count -1);
                }
            }
        }
    }
}


