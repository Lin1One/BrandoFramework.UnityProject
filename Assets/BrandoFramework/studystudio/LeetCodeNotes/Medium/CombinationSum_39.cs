using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个无重复元素的数组 candidates 和一个目标数 target ，
        // 找出 candidates 中所有可以使数字和为 target 的组合。
        // candidates 中的数字可以无限制重复被选取。
        
        // 说明：
        // 所有数字（包括 target）都是正整数。
        // 解集不能包含重复的组合。 
        
        // 示例 1:
        // 输入: candidates = [2,3,6,7], target = 7,
        // 所求解集为:
        // [
        //   [7],
        //   [2,2,3]
        // ]

        // 示例 2:
        // 输入: candidates = [2,3,5], target = 8,
        // 所求解集为:
        // [
        //   [2,2,2,2],
        //   [2,3,3],
        //   [3,5]
        // ]
        private List<IList<int>> res = new List<IList<int>>();
        private int[] candidates;
        private int len;
        public IList<IList<int>> CombinationSum(int[] candidates, int target) 
        {
            int len = candidates.Length;
            if (len == 0)
             {
                return res;
            }
            // 优化添加的代码1：先对数组排序，可以提前终止判断
            Array.Sort(candidates);
            this.len = len;
            this.candidates = candidates;
            findCombinationSum(target, 0, new Stack<int>());
            return res;
        }

        private void findCombinationSum(int residue, int start, Stack<int> pre) 
        {
            if (residue == 0) 
            {
                res.Add(new List<int>(pre));
                return;
            }
            // 优化添加的代码2：在循环的时候做判断，尽量避免系统栈的深度
            // residue - candidates[i] 表示下一轮的剩余，如果下一轮的剩余都小于 0 ，就没有必要进行后面的循环了
            // 这一点基于原始数组是排序数组的前提，因为如果计算后面的剩余，只会越来越小
            for (int i = start; i < len && residue - candidates[i] >= 0; i++)
            {
                pre.Push(candidates[i]);
                // 【关键】因为元素可以重复使用，这里递归传递下去的是 i 而不是 i + 1
                findCombinationSum(residue - candidates[i], i, pre);
                pre.Pop();
            }
        }


    // List<List<Integer>> res = new ArrayList<>();

    // public List<List<Integer>> combinationSum(int[] candidates, int target) {
    //     int sum = 0;
    //     List<Integer> list = new ArrayList<>();
    //     addElement(candidates, list, sum, target,0);
    //     return res;

    // }

    // private void addElement(int[] candidates, List<Integer> list, int sum, int target,int index) {
    //     if (sum > target)
    //         return;
    //     if (sum == target) {
    //         res.add(list);
    //         return;
    //     }
    //     for (int i = index; i < candidates.length; i++
    //     ) {
    //         List<Integer> t_list = new ArrayList<>(list);
    //         t_list.add(candidates[i]);
    //         addElement(candidates, t_list, sum + candidates[i], target,i);
    //     }

    // }

    }
}


