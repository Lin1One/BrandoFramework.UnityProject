using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //  78. 子集
        // 给定一组不含重复元素的整数数组 nums，返回该数组所有可能的子集（幂集）。
        // 说明：解集不能包含重复的子集。

        // 示例:
        // 输入: nums = [1,2,3]
        // 输出:
        // [
        //   [3],
        //   [1],
        //   [2],
        //   [1,2,3],
        //   [1,3],
        //   [2,3],
        //   [1,2],
        //   []
        // ]
        
        public IList<IList<int>> Subsets(int[] nums) 
        {
            var ans = new List<IList<int>>();
            int bitCount  = 1 << nums.Length;
            for(var i = 0;i< bitCount;i++)
            {
                var itemList = new List<int>();
                for(var j = 0;j< nums.Length;j++)
                {
                    if( ((i >> j) & 1) == 1)
                    {
                        itemList.Add(nums[j]);
                    }
                }
                ans.Add(itemList);
            }
            return ans;
        }

        // 对于给定一个集合里，所有元素的集合它们应该满足这样一个公式: 
        // 假设所有的组合数之和为 sum，则有sum = C(n, 0) + C(n, 1) + ...+ C(n, n); 
        // 分别对应取集合中的一个元素，两个元素...n个元素。
        // 而通过数学公式二项式定义，这个和是等于 2的n次方。
        // 就是说，我们所有取的组合数为一个指数函数。
        // 假设输入是1、2、3。
        // 首先全部的子集为【000】【001】【010】【100】【011】【101】【110】【111】 
        // 1表示这一位的数字存在，例如 【010】 表示只含有 2
        // 由此发现子集所代表的二进制数全部小于 1 << 数组.length 
        // 第一层循环 for (int i = 0; i < (1 << size); i++)

        // 然后根据【i】 的二进制数中 【1】 的位置取得子集
        //位运算
        public IList<IList<int>> Subsets1(int[] nums) 
        {
            List<IList<int>> res = new List<IList<int>>();
            for (int i = 0; i < (1 << nums.Length); i++) 
            {
                List<int> sub = new List<int>();
                for (int j = 0; j < nums.Length; j++)
                {
                    if (((i >> j) & 1) == 1) 
                    {
                        sub.Add(nums[j]);
                    }
                }
                res.Add(sub);
            }
            return res;
        }

        //逐个枚举，空集的幂集只有空集，每增加一个元素，
        //让之前幂集中的每个集合，追加这个元素，就是新增的子集。
        //循环枚举
        public IList<IList<int>> enumerate(int[] nums) 
        {
            List<IList<int>> res = new List<IList<int>>();
            res.Add(new List<int>());
            for (var i = 0;i < nums.Length;i++) 
            {
                int listCount = res.Count;
                for (int j = 0;j < listCount; j++) 
                {
                    List<int> newSub = new List<int>(res[j]);
                    newSub.Add(nums[i]);
                    res.Add(newSub);
                }
            }
            return res;
        }

    
        //递归枚举
        public IList<IList<int>> Subsets2(int[] nums) 
        {
            List<IList<int>> res = new List<IList<int>>();
            res.Add(new List<int>());
            recursion(nums,0,res);
            return res;
        }
        public void recursion(int[] nums, int i, List<IList<int>> res) 
        {
            if (i >= nums.Length) 
                return;
            int size = res.Count;
            for (int j = 0; j < size; j++) 
            {
                List<int> newSub = new List<int>(res[j]);
                newSub.Add(nums[i]);
                res.Add(newSub);
            }
            recursion(nums, i + 1, res);
        }

        //回溯递归
        public IList<IList<int>> Subsets4(int[] nums) 
        {
            List<IList<int>> res = new List<IList<int>>();
            backtrack(0, nums, res, new List<int>());
            return res;

        }

        private void backtrack(int i, int[] nums, 
            List<IList<int>> res, 
            List<int> tmp) 
        {
            res.Add(new List<int>(tmp));
            for (int j = i; j < nums.Length; j++) 
            {
                tmp.Add(nums[j]);
                backtrack(j + 1, nums, res, tmp);
                tmp.RemoveAt(tmp.Count - 1);
            }
        }
    }
}




