using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个包含 n 个整数的数组 nums，判断 nums 中是否存在三个元素 a，b，c ，
        // 使得 a + b + c = 0 ？找出所有满足条件且不重复的三元组。
        // 注意：答案中不可以包含重复的三元组。
        // 例如, 给定数组 nums = [-1, 0, 1, 2, -1, -4]，
        // 满足要求的三元组集合为：
        // [
        //   [-1, 0, 1],
        //   [-1, -1, 2]
        // ]

        public IList<IList<int>> ThreeSum0322(int[] nums)
        {
            List<IList<int>> results = new List<IList<int>>();
            Array.Sort(nums);
            if (nums == null) return results;
            for (var i = 0;i< nums.Length - 2;i++)
            {
                if (nums[i] > 0)
                {
                    break;
                }
                if (i > 0 && nums[i] == nums[i - 1])
                {
                    continue; // 去重
                }
                int start = i + 1;
                int end = nums.Length - 1;

                while(start < end)
                {
                    if(nums[i]+ nums[start]+ nums[end] > 0)
                    {
                        end--;
                    }
                    else if (nums[i] + nums[start] + nums[end] < 0)
                    {
                        start++;
                    }
                    else
                    {
                        IList<int> result = new List<int>();
                        result.Add(nums[i]);
                        result.Add(nums[start]);
                        result.Add(nums[end]);
                        results.Add(result);
                        while (start < end && nums[start] == nums[start + 1])
                        {
                            start++; // 去重
                        }
                        while (start < end && nums[end] == nums[end - 1])
                        {
                            end--; // 去重
                        }
                        start++;
                        end--;
                    }
                }
            }
            return results;
        }








        //排序，双指针
        // 首先对数组进行排序，排序后固定一个数nums[i]，
        // 再使用左右指针指向nums[i]后面的两端，数字分别为nums[L]和nums[R]，
        // 计算三个数的和sum判断是否满足为 0，满足则添加进结果集
        // 如果nums[i]大于 0，则三数之和必然无法等于 0，结束循环
        // 如果nums[i] == nums[i−1]，则说明该数字重复，会导致结果重复，所以应该跳过
        // 当sum == 00 时，nums[L] == nums[L+1]则会导致结果重复，应该跳过，L++
        // 当sum == 00 时，nums[R] == nums[R−1]则会导致结果重复，应该跳过，R−−
        // 时间复杂度：O(n^2)，n为数组长度
        public IList<IList<int>> ThreeSum(int[] nums) 
        {
            List<IList<int>> ans = new List<IList<int>>();
            Array.Sort(nums); // 排序
            int len = nums.Length;
            if(nums == null || len < 3) return ans;
            for (int i = 0; i < len ; i++) 
            {
                if(nums[i] > 0) 
                    break; // 如果当前数字大于0，则三数之和一定大于0，所以结束循环
                if(i > 0 && nums[i] == nums[i-1]) 
                    continue; // 去重
                int L = i+1;
                int R = len-1;
                while(L < R)
                {
                    int sum = nums[i] + nums[L] + nums[R];
                    if(sum == 0)
                    {
                        ans.Add(new List<int>(){nums[i],nums[L],nums[R]});
                        while (L<R && nums[L] == nums[L+1]) L++; // 去重
                        while (L<R && nums[R] == nums[R-1]) R--; // 去重
                        L++;
                        R--;
                    }
                    else if (sum < 0) L++;
                    else if (sum > 0) R--;
                }
            }        
            return ans;
        }
    }
}


