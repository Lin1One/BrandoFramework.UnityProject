using System.Collections.Generic;
using UnityEngine;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个数组 nums，编写一个函数将所有 0 移动到数组的末尾，
        // 同时保持非零元素的相对顺序。
        // 示例:

        // 输入: [0,1,0,3,12]
        // 输出: [1,3,12,0,0]
        public void MoveZeroes1(int[] nums) 
        {
            int zeroCount = 0;
            int validNumIndex = 0;
            for(int i = 0;i<nums.Length;i++)
            {
                if(nums[i] == 0)
                {
                    zeroCount++;
                }
                else
                {
                    nums[validNumIndex] = nums[i];
                    validNumIndex++;
                }
            }
            for(var i = validNumIndex;i<nums.Length;i++)
            {
                nums[i] = 0;
            }

        }
        public void MoveZeroes2(int[] nums) 
        {
            int n = nums.Length;
            // Count the zeroes
            int numZeroes = 0;
            for (int i = 0; i < n; i++) 
            {
                if(nums[i] == 0)
                    numZeroes ++;
            }

            // Make all the non-zero elements retain their original order.
            List<int> ans = new List<int>();
            for (int i = 0; i < n; i++) 
            {
                if (nums[i] != 0) 
                {
                    ans.Add(nums[i]);
                }
            }

            // Move all zeroes to the end
            while (numZeroes > 0) 
            {
                ans.Add(0);
                numZeroes--;
            }

            // Combine the result
            for (int i = 0; i < n; i++) 
            {
                nums[i] = ans[i];
            }
        }
    }
}


