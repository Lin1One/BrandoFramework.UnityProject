using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 581. 最短无序连续子数组
        // 给定一个整数数组，你需要寻找一个连续的子数组，
        // 如果对这个子数组进行升序排序，那么整个数组都会变为升序排序。
        // 你找到的子数组应是最短的，请输出它的长度。

        // 示例 1:

        // 输入: [2, 6, 4, 8, 10, 9, 15]
        // 输出: 5
        // 解释: 你只需要对 [6, 4, 8, 10, 9] 进行升序排序，那么整个表都会变为升序排序。
        // 说明 :

        // 输入的数组长度范围在 [1, 10,000]。
        // 输入的数组可能包含重复元素 ，所以升序的意思是<=。
        public int FindUnsortedSubarray1(int[] nums) 
        {
            if(nums == null )
            {
                return 0;
            }

            if(nums.Length == 1)
            {
                return 0;
            }
            
            int rightIndex = -1;

            int max = int.MinValue;
            for(var i = 0;i< nums.Length;i++)
            {
                if(nums[i] >= max)
                {
                    max = nums[i];
                }
                else
                {
                    rightIndex = i;
                }
            }

            int min = int.MaxValue;
            int leftIndex = rightIndex + 1;
            for(var i = rightIndex;i>= 0;i--)
            {
                if(nums[i] <= min)
                {
                    min = nums[i];
                }
                else
                {
                    leftIndex = i;
                }
            }

            return rightIndex - leftIndex + 1;
            
        }



        public int FindUnsortedSubarray(int[] nums) 
        {
        //从左到右扫描（或从右到左）找局部极大值（或局部极小值），若位置放置不正确，找到其应该存在的地方
        int length = nums.Length;
        //赋初始开始和结束值
        int leftIndex = -1;
        int rightIndex = -2;
        //结束值赋为-2是考虑在数组本身就是有序时,return也可以给出正确值
        int min = nums[length - 1];
        int max = nums[0];
        for(int i = 0, pos = 0; i < length; i++) 
        {
            pos = length - 1 - i;
            //找出局部极大、极小值
            max = Math.Max(max, nums[i]);
            min = Math.Min(min, nums[pos]);
            //如果当前值小于局部极大值，用end记录该位置，找到该max的合适位置，
            if(nums[i] < max)
                rightIndex = i;
            //如果当前值大于局部极小值，用star记录该位置，找到该star的合适位置
            if(nums[pos] > min)
                leftIndex = pos;
        }
        //返回开始和结束的索引差
        return rightIndex - leftIndex + 1;
    }



    }
}


