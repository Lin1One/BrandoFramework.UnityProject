using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 560. 和为K的子数组
        // 给定一个整数数组和一个整数 k，你需要找到该数组中和为 k 的连续的子数组的个数。

        // 示例 1 :
        // 输入:nums = [1,1,1], k = 2
        // 输出: 2 , [1,1] 与 [1,1] 为两种不同的情况。
        // 说明 :
        // 数组的长度为 [1, 20,000]。
        // 数组中元素的范围是 [-1000, 1000] ，且整数 k 的范围是 [-1e7, 1e7]。

        public int SubarraySum(int[] nums, int k) 
        {
            int count = 0;
            return count;
        }

        // 首先对于这类题，暴力的做法是嵌套循环，以每个元素为起始点遍历扫描，
        // 检查子数组和是否满足情况。时间复杂度为 O(n^2)，空间复杂度为O(1)。

        // 进一步的优化思路：
        // 首先：优化什么？
        // 目前时间复杂度高，空间复杂度是常数，所以优化点在时间上
        // 时间复杂度的优化都是通过空间来换取时间，
        // 而占用存储空间的数据结构应该满足常数时间查找

        // 然后：怎么优化？
        // 1.暴力算法中有没有重复运算？简单来看，有很多重复的加法运算。
        //   例如，在下标为0的时候，运算过 nums[0] + nums[1] + nums[2]，
        //   在下标为1的时候，又算了一次nums[1] + nums[2]
        // 2.找到重复运算后怎么消除重复运算？
        //   上例中，重复计算了nums[1] + nums[2]，换一种思路，实际上不需要加法，
        //   只需要 nums[0] + nums[1] + nums[2] - nums[0]，
        //   再写的清晰一些，用sum[i]表示从0到i所有元素的和，
        //   那么nums[1] + nums[2] = sum[2] - sum[0]。
        //   这里的sum[i]实际上就是一种前缀和的思路，
        //   只需要遍历一次就可以知道所有的前缀和，存在map里，用的时候就可以实现在常数时间的查找。
        // 3.有了大体的方向后，具体的map怎么存，key是什么，value是什么？
        //    对于这个问题来说，我们需要找到的是target k，
        //    即sum[j] - sum[i] = k (j > i)，k已知，sum[j]在迭代过程中逐步计算。
        //    需要存储的就只有sum[i]，查找sum[i]要常数时间，
        //    那么map的key应该是sum[i]。根据约束条件，value应该是当前值的下标。
        //    但是在实际实现的时候可以把这个约束隐藏在循环中，
        //    对于当前问题，要找到满足子数组的个数，value可以用来存储到当前位置，
        //    前缀和的个数，那么在找到满足的sum[i] = sum[j] - k的时候，
        //    最后的结果只需要加上map[sum[j] - k]即可。

        // public int SubarraySum(int[] nums, int k) 
        // {
        //     int cur = 0, res = 0;
        //     Dictionary<int, int> um = new Dictionary<int, int>();
      	//     // 注意这里前缀和多了一个0，防止漏掉数组的前缀和刚好等于k的情况
        //     um[0] = 1;
        //     foreach (int num in nums) 
        //     {
        //         cur += num;
        //         res += um.find(cur - k) == um.end() ? 0 : um[cur - k];
        //         ++um[cur];
        //     }
        //     return res;
        // }

        // 使用Map储存结果，将搜索的复杂度减小为O(n)
        // 在有了第一步结果后，我们将问题转化为：
        // 给定一个数组，是否存在两个数的差等于target，如果存在，请问有多少种情况
        // 这种情况下，就非常类似于数组中两数求和1. 两数之和，
        // 我们将遍历的结果加到set中，使用常数时间进行查找，
        // 但是第一题只要求找到一个结果即可，所以用Set(Set本质上就是Map)。
        // 但是在本文中，当我们查找dp[i]-k时，对应的dp[j]可能不唯一，
        // 这也对应了本问题的多种答案，
        // 所以，我们使用Map去存储，key是dp[i]，value是dp[j]有多少种选择。

        public int SubarraySum1(int[] nums, int k) 
        {
            if (nums == null || nums.Length == 0) 
                return 0;
            //dp[i]表示前i个数的和
            int[] sum = new int[nums.Length + 1];
            for (int i = 1; i <= nums.Length; i++) 
            {
                sum[i] = sum[i - 1] + nums[i - 1];
            }

            int ret = 0;
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 0; i < sum.Length; i++) 
            {
                if (map.ContainsKey(sum[i] - k))
                {
                    ret += map[sum[i] - k];
                }
                if(!map.ContainsKey(sum[i]))
                {
                    map.Add(sum[i],0);
                }
                map[sum[i]] += 1;
            }
            return ret;
        }

        public int SubarraySum2(int[] nums, int k) 
        {
            Dictionary<int, int> sum2count = new Dictionary<int, int>(128);

            int ret = 0;
            int sum = 0;
            int key;
            sum2count.Add(0, 1);
            foreach ( var v in nums)
            {
                sum += v;
                key = sum - k;
                if (sum2count.ContainsKey(key)) 
                    ret += sum2count[key];

                if (!sum2count.ContainsKey(sum)) 
                    sum2count.Add(sum, 1);
                else 
                    ++sum2count[sum];
            }
            return ret;
        }

    }
}


