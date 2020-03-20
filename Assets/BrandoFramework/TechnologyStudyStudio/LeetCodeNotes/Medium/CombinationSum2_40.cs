using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 40. 组合总和 II
        // 给定一个数组 candidates 和一个目标数 target ，
        // 找出 candidates 中所有可以使数字和为 target 的组合。

        // candidates 中的每个数字在每个组合中只能使用一次。

        // 说明：
        // 所有数字（包括目标数）都是正整数。
        // 解集不能包含重复的组合。 

        // 示例 1:
        // 输入: candidates = [10,1,2,7,6,1,5], target = 8,
        // 所求解集为:
        // [
        //   [1, 7],
        //   [1, 2, 5],
        //   [2, 6],
        //   [1, 1, 6]
        // ]
        
        // 示例 2:
        // 输入: candidates = [2,5,2,1,2], target = 5,
        // 所求解集为:
        // [
        //   [1,2,2],
        //   [5]
        // ]


        // 思路分析：

        // 这道题其实比上一问更简单，思路是：

        // 以 target 为根结点，依次减去数组中的数字，直到小于 0 或者等于 0，
        // 把等于 0 的结果记录到结果集中。

        // 当然你也可以以 0 为根结点，依次加上数组中数字，直到大于 target 或者等于 target，
        // 把等于 target 的结果记录到结果集中。

        // “解集不能包含重复的组合”，就暗示我们得对数组先排个序（“升序”或者“降序”均可，下面示例中均使用“升序”）。
        // “candidates 中的每个数字在每个组合中只能使用一次”，
        // 那就按照顺序依次减去数组中的元素，递归求解即可：遇到 0 就结算且回溯，遇到负数也回溯。
        // candidates 中的数字可以重复，可以借助「力扣」第 47 题：全排列 II 的思想，
        // 在搜索的过程中，找到可能发生重复结果的分支，把它剪去。
 
        private IList<IList<int>> CombinationSum2Result = new List<IList<int>>();
        public IList<IList<int>> CombinationSum2(int[] candidates, int target) 
        {
            if(candidates == null || candidates.Length == 0)
            {
                return null;    
            }

            Array.Sort(candidates);
            List<int> oneResult = new List<int>();
            SolveBack(candidates,target,oneResult,0,0);
            return CombinationSum2Result;
            
        }

        private void SolveBack(int[] candidates,
            int target,
            IList<int> list,
            int currentIndex,
            int sum)
        {
            if(sum >= target|| currentIndex == candidates.Length)
            {
                if(sum == target)
                {
                    CombinationSum2Result.Add(new List<int>(list));
                }
                return;
            }
            for(var i = currentIndex;i< candidates.Length;i++)
            {
                if(candidates[i] > target)
                {
                    continue;
                }
                if (i > currentIndex && candidates[i - 1] == candidates[i]) 
                {
                    continue;
                }
                sum += candidates[i];
                list.Add(candidates[i]);
                SolveBack(candidates,target,list,i+1,sum);
                list.RemoveAt(list.Count - 1);
                sum -= candidates[i];
            }
        }

    }
}




