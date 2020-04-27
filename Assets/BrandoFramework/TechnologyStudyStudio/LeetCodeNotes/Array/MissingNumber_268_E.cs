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
        //268. 缺失数字
        //给定一个包含 0, 1, 2, ..., n 中 n 个数的序列，找出 0 .. n 中没有出现在序列中的那个数。
        //示例 1:
        //输入: [3,0,1]
        //输出: 2
        //示例 2:
        //输入: [9,6,4,2,3,5,7,0,1]
        //输出: 8
        //说明:
        //你的算法应具有线性时间复杂度。你能否仅使用额外常数空间来实现?
        
        public int MissingNumber1(int[] nums)
        {
            //由于异或运算（XOR）满足结合律，
            //并且对一个数进行两次完全相同的异或运算会得到原来的数，
            //因此我们可以通过异或运算找到缺失的数字。

            //我们知道数组中有 n 个数，并且缺失的数在[0..n] 中。
            //因此我们可以先得到[0..n] 的异或值，再将结果对数组中的每一个数进行一次异或运算。
            //未缺失的数在[0..n] 和数组中各出现一次，因此异或后得到 0。
            //而缺失的数字只在[0..n] 中出现了一次，
            //在数组中没有出现，因此最终的异或结果即为这个缺失的数字。

            //在编写代码时，由于[0..n] 恰好是这个数组的下标加上 n，
            //因此可以用一次循环完成所有的异或运算，例如下面这个例子：
            //下标  0   1   2   3
            //数字  0   1   3   4
            //可以将结果的初始值设为 n，再对数组中的每一个数以及它的下标进行一个异或运算，即：

            //= 4∧(0∧0)∧(1∧1)∧(2∧3)∧(3∧4)
            //=(4∧4)∧(0∧0)∧(1∧1)∧(3∧3)∧2
            //= 0∧0∧0∧0∧2
            //= 2

            //就得到了缺失的数字为 2。

            //异或
            int n = nums.Length;
            int missNum = n;
            for (int i = 0; i < nums.Length; i++)
            {
                missNum ^= i ^ nums[i];
            }
            return missNum;
        }

        public int MissingNumber2(int[] nums)
        {
            //求和
            int n = nums.Length;
            int elementSum = 0;
            int totalSum = n*(n + 1)/2;
            for (int i = 0; i < n; i++)
            {
                elementSum += nums[i];
            }
            return totalSum - elementSum;
        }

        public int MissingNumber3(int[] nums)
        {
            //排序
            Array.Sort(nums);

            // 判断 n 是否出现在末位
            if (nums[nums.Length - 1] != nums.Length)
            {
                return nums.Length;
            }
            // 判断 0 是否出现在首位
            else if (nums[0] != 0)
            {
                return 0;
            }

            // 此时缺失的数字一定在 (0, n) 中
            for (int i = 1; i < nums.Length; i++)
            {
                int expectedNum = nums[i - 1] + 1;
                if (nums[i] != expectedNum)
                {
                    return expectedNum;
                }
            }
            // 未缺失任何数字（保证函数有返回值）
            return -1;
        }
    }
}

