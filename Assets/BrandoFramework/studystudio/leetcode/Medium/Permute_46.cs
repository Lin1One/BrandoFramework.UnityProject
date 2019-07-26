using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个没有重复数字的序列，返回其所有可能的全排列。
        // 示例:
        // 输入: [1,2,3]
        // 输出:
        // [
        //   [1,2,3],
        //   [1,3,2],
        //   [2,1,3],
        //   [2,3,1],
        //   [3,1,2],
        //   [3,2,1]
        // ]




        private List<IList<int>> ans = new List<IList<int>>();
        public IList<IList<int>> Permute(int[] nums)
        {
            int length = nums.Length;
            var permuteList = new List<int>();
            addPermute(nums,permuteList);
            return ans;
        }

        private void addPermute(int[] nums,List<int> permuteList)
        {
            if(permuteList.Count == nums.Length)
            {
                ans.Add(permuteList);
                return;
            }
            for(var i = 0;i < nums.Length;i++)
            {
                if(!permuteList.Contains(nums[i]))
                {
                    var newList = new List<int>(permuteList);
                    newList.Add(nums[i]);
                    addPermute(nums,newList);
                }
            }
        }


        //迭代实现（未实现）
        public IList<IList<int>> Permute3(int[] nums)
        {
            List<IList<int>> ans = new List<IList<int>>();
            int length = nums.Length;
            if(length == 1)
            {
                var oneList = new List<int>();
                oneList.Add(nums[0]);
                ans.Add(oneList);
                return ans;
            }

            if(length == 2)
            {
                var oneList1 = new List<int>(){nums[0],nums[1]};
                var oneList2 = new List<int>(){nums[1],nums[0]};
                ans.Add(oneList1);
                ans.Add(oneList2);
                return one;
            }
            for(var i = 0;i< length;i++)
            {
                int time = 0;
                for(var j  = 0; j < length;j++)
                {
                    if(i == j) continue;
                    
                    var listItem1 = new List<int>();
                    var listItem2 = new List<int>();

                    listItem1.Add(nums[i]);
                    listItem2.Add(nums[i]);

                    int rangeStart1 = (i + 1 + time) % length;
                    int rangeStart2 = (length + i - 1 - time) % length;
                    time++;

                    while(rangeStart1 != i)
                    {
                        listItem1.Add(nums[rangeStart1]);
                        listItem2.Add(nums[rangeStart2]);
                        rangeStart2 = (--rangeStart2 + length) % length;
                        rangeStart1 = ++rangeStart1 % length;
                    }
                    ans.Add(listItem1);
                    ans.Add(listItem2);
                    if(length == 3)
                    {
                        j++;
                        if(j == i) j++;
                    }
                    //}
                }
            }
            return ans;
        }


        public IList<IList<int>> Permute2(int[] nums) 
        {
            List<IList<int>> ans = new List<IList<int>>();
            int[] visited = new int[nums.Length];
            backtrack(ans, nums, new List<int>(), visited);
            return ans;
        }

        private void backtrack(List<IList<int>> res, 
            int[] nums, 
            List<int> tmp, 
            int[] visited) 
        {
            if (tmp.Count == nums.Length)
            {
                res.Add(new List<int>(tmp));
                return;
            }
            for (int i = 0; i < nums.Length; i++) 
            {
                if (visited[i] == 1) 
                    continue;
                visited[i] = 1;
                tmp.Add(nums[i]);
                backtrack(res, nums, tmp, visited);
                visited[i] = 0;
                tmp.RemoveAt(tmp.Count - 1);
            }
        }

    }
}


