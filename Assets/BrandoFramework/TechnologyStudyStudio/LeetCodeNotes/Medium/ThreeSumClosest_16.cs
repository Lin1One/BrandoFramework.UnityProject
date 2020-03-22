using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 16. 最接近的三数之和
        // 给定一个包括 n 个整数的数组 nums 和 一个目标值 target。找出 nums 中的三个整数，
        // 使得它们的和与 target 最接近。返回这三个数的和。假定每组输入只存在唯一答案。
        // 例如，给定数组 nums = [-1，2，1，-4], 和 target = 1.
        // 与 target 最接近的三个数的和为 2. (-1 + 2 + 1 = 2).

        public int ThreeSumClosest0322(int[] nums, int target)
        {
            Array.Sort(nums);
            if(nums == null)
            {
                return 0;
            }
            int sum = 0;
            int closetSum = int.MaxValue;
            for (var i = 0;i<nums.Length - 2 ;i++)
            {
                if(i > 0 && nums[i] == nums[i -1] )
                {
                    continue;
                }
                int start = i + 1;
                int end = nums.Length - 1;
                while(start < end)
                {
                    sum = nums[i] + nums[start] + nums[end];
                    if (Math.Abs(target - sum) < Math.Abs(target - closetSum))
                    {
                        closetSum = sum;
                    }
                    if (sum > target )
                    {
                        end--;
                    }
                    else if(sum < target)
                    {
                        start++;
                    }
                    else
                    {
                        return closetSum;
                    }
                }
            }
            return closetSum;

        }

        public int ThreeSumClosest(int[] nums, int target) 
        {
            Array.Sort(nums);
            int closesSum = nums[0]+ nums[1] + nums[2];
            for(var i = 0;i<nums.Length - 2;i++)
            {
                int secondNum = i + 1;
                int thirdNum = nums.Length - 1;
                int tempSum = 0;

                while(secondNum < thirdNum)
                {
                    tempSum = nums[i]+ nums[secondNum] + nums[thirdNum];
                    if(Math.Abs(target - tempSum) < Math.Abs(target - closesSum))
                    {
                        closesSum = tempSum;
                    }
                    if(tempSum > target)
                    {
                        thirdNum--;
                    }
                    else if(tempSum < target)
                    {
                        secondNum++;
                    }
                    else
                        return closesSum;
                }
                
            }
            return closesSum;
        }

    }
}




