#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //给定一个数组，将数组中的元素向右移动 k个位置，其中 k是非负数。

        //示例 1:
        //输入: [1,2,3,4,5,6,7] 和 k = 3
        //输出: [5,6,7,1,2,3,4]
        //解释:
        //向右旋转 1 步: [7,1,2,3,4,5,6]
        //向右旋转 2 步: [6,7,1,2,3,4,5]
        //向右旋转 3 步: [5,6,7,1,2,3,4]

        //示例 2:
        //输入: [-1,-100,3,99] 和 k = 2
        //输出: [3,99,-1,-100]
        //解释: 
        //向右旋转 1 步: [99,-1,-100,3]
        //向右旋转 2 步: [3,99,-1,-100]
        //说明:

        //尽可能想出更多的解决方案，至少有三种不同的方法可以解决这个问题。
        //要求使用空间复杂度为 O(1) 的 原地算法。

        public void Rotate0330(int[] nums, int k)
        {
            if(nums == null || nums.Length == 0)
            {
                return;
            }
            var length = nums.Length;
            var rotateStep = k % length;
            var rotateCount = 0;
            var curIndex = 0;
            var curTemp = nums[curIndex];
            while (rotateCount < length)
            {
                var newIndex = (curIndex + rotateStep) % ( length);
                var temp = nums[newIndex];
                nums[newIndex] = curTemp;
                curTemp = temp;
                rotateCount++;
                curIndex = newIndex;
            }
        }

        public void Rotate0330_1(int[] nums,int k)
        {
            if (nums == null || nums.Length == 0)
            {
                return;
            }
            var length = nums.Length;
            var rotateStep = k % length;
            var rotateCount = 0;
            for (var i = 0; rotateCount < nums.Length; i++)
            {
                int curIndex = i;
                int preValue = nums[curIndex];
                do
                {
                    var newIndex = (curIndex + rotateStep) % nums.Length;
                    var temp = nums[newIndex];
                    nums[newIndex] = preValue;
                    preValue = temp;
                    curIndex = newIndex;
                    rotateCount++;
                }
                while (i != curIndex);
            }
        }


        public void rotate(int[] nums, int k)
        {
            //这个方法基于这个事实：当我们旋转数组 k 次， k\% n 个尾部元素会被移动到头部，剩下的元素会被向后移动。
            //在这个方法中，我们首先将所有元素反转。然后反转前 k 个元素，再反转后面 n-kn−k 个元素，就能得到想要的结果。
            k %= nums.Length;
            reverse(nums, 0, nums.Length - 1);
            reverse(nums, 0, k - 1);
            reverse(nums, k, nums.Length - 1);
        }

        private void reverse(int[] nums, int start, int end)
        {
            while (start < end)
            {
                int temp = nums[start];
                nums[start] = nums[end];
                nums[end] = temp;
                start++;
                end--;
            }
        }
    }
}

