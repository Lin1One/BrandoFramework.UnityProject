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




