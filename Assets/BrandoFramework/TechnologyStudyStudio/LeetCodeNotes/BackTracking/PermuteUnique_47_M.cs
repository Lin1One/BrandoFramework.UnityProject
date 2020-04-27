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

        public IList<IList<int>> PermuteUnique1(int[] nums)
        {
            // 排序（升序或者降序都可以），排序是剪枝的前提
            Array.Sort(nums);
            var usedNums = new bool[nums.Length];
            IList<IList<int>> result = new List<IList<int>>();
            PermuteUnique_Backtrack(nums, new List<int>(), usedNums, result);
            return result;
            //时间复杂度：O(N×N!)，这里 N 为数组的长度。
            //空间复杂度：O(N×N!)。
        }

        private void PermuteUnique_Backtrack(int[] nums, IList<int> resultItem,bool[] used, IList<IList<int>> result)
        {
            if(resultItem.Count == nums.Length)
            {
                result.Add(new List<int>(resultItem));
                return;
            }

            for(var i = 0;i< nums.Length;i++)
            {
                if(used[i])
                {
                    continue;
                }
                if (i > 0 && nums[i] == nums[i - 1] && used[i - 1])
                {
                    continue;
                }
                resultItem.Add(nums[i]);
                used[i] = true;
                PermuteUnique_Backtrack(nums, resultItem, used,result);
                used[i] = false;
                resultItem.RemoveAt(resultItem.Count - 1);
                
            }
        }
    }
}


