#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个大小为 n 的数组，找到其中的众数。
        // 说众数是指在数组中出现次数大于 ⌊ n/2 ⌋ 的元素。
        // 你可以假设数组是非空的，并且给定的数组总是存在众数。
        
        // 示例 1:

        // 输入: [3,2,3]
        // 输出: 3
        // 示例 2:

        // 输入: [2,2,1,1,1,2,2]
        // 输出: 2


        // 方法 6：Boyer-Moore 投票算法
        // 想法
        // 如果我们把众数记为 +1 ，把其他数记为 -1 ，将它们全部加起来，显然和大于 0 ，
        // 从结果本身我们可以看出众数比其他数多。

        // 算法
        // 本质上， Boyer-Moore 算法就是找 nums 的一个后缀 suf 
        // 其中 suf[0] 就是后缀中的众数。
        // 我们维护一个计数器，如果遇到一个我们目前的候选众数，就将计数器加一，否则减一。
        // 只要计数器等于 0 ，我们就将 nums 中之前访问的数字全部忘记 ，
        // 并把下一个数字当做候选的众数。直观上这个算法不是特别明显为何是对的，
        // 我们先看下面这个例子（竖线用来划分每次计数器归零的情况）
        // [7, 7, 5, 7, 5, 1 | 5, 7 | 5, 5, 7, 7 | 7, 7, 7, 7]

        // 首先，下标为 0 的 7 被当做众数的第一个候选。在下标为 5 处，计数器会变回0 。
        // 所以下标为 6 的 5 是下一个众数的候选者。由于这个例子中 7 是真正的众数，
        // 所以通过忽略掉前面的数字，我们忽略掉了同样多数目的众数和非众数。
        // 因此， 7 仍然是剩下数字中的众数。
        // [7, 7, 5, 7, 5, 1 | 5, 7 | 5, 5, 7, 7 | 1, 2, 6, 6]

        // 现在，众数是 5 （在计数器归零的时候我们把候选从 7 变成了 5）。
        // 此时，我们的候选者并不是真正的众数，
        // 但是我们在 遗忘 前面的数字的时候，要去掉相同数目的众数和非众数
        //（如果遗忘更多的非众数，会导致计数器变成负数）。
        // 因此，上面的过程说明了我们可以放心地遗忘前面的数字，
        // 并继续求解剩下数字中的众数。
        // 最后，总有一个后缀满足计数器是大于 0 的，此时这个后缀的众数就是整个数组的众数。



        public int MajorityElement(int[] nums) 
        {
            int count = 0;
            int candidate = 0;

            foreach (var  num in nums)
            {
                if (count == 0) 
                {
                    candidate = num;
                }
                count += (num == candidate) ? 1 : -1;
            }
            return candidate;
        }


// 方法 5：分治
// 想法
// 如果我们知道数组左边一半和右边一半的众数，我们就可以用线性时间知道全局的众数是哪个。

// 算法
// 这里我们使用经典的分治算法递归求解，直到所有的子问题都是长度为 1 的数组。
//由于传输子数组需要额外的时间和空间，所以我们实际上只传输子区间的左右指针 lo 和 hi 
//表示相应区间的左右下标。
//长度为 1 的子数组中唯一的数显然是众数，直接返回即可。
//如果回溯后某区间的长度大于 1 ，我们必须将左右子区间的值合并。
//如果它们的众数相同，那么显然这一段区间的众数是它们相同的值。
//否则，我们需要比较两个众数在整个区间内出现的次数来决定该区间的众数。
//原问题的答案就是下标为 0 和 n 之间的众数这一子问题。
        private int countInRange(int[] nums, int num, int lo, int hi) 
        {
            int count = 0;
            for (int i = lo; i <= hi; i++) 
            {
                if (nums[i] == num) 
                {
                    count++;
                }
            }
            return count;
        }

        private int majorityElemeec(int[] nums, int lo, int hi) 
        {
            // base case; the only element in an array of size 1 is the majority
            // element.
            if (lo == hi) 
            {
                return nums[lo];
            }

            // recurse on left and right halves of this slice.
            int mid = (hi-lo)/2 + lo;
            int left = majorityElemeec(nums, lo, mid);
            int right = majorityElemeec(nums, mid+1, hi);

            // if the two halves agree on the majority element, return it.
            if (left == right) 
            {
                return left;
            }

            // otherwise, count each element and return the "winner".
            int leftCount = countInRange(nums, left, lo, hi);
            int rightCount = countInRange(nums, right, lo, hi);

            return leftCount > rightCount ? left : right;
        }

        public int majorityElement(int[] nums) 
        {
            return majorityElemeec(nums, 0, nums.length-1);
        }

    }
}

