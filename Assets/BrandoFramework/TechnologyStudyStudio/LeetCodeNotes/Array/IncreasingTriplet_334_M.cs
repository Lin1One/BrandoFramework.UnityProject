﻿using System.Linq;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //334. 递增的三元子序列
        //给定一个未排序的数组，判断这个数组中是否存在长度为 3 的递增子序列。

        //数学表达式如下:

        //如果存在这样的 i, j, k, 且满足 0 ≤ i<j<k ≤ n-1，
        //使得 arr[i] < arr[j] < arr[k] ，返回 true ; 否则返回 false 。
        //说明: 要求算法的时间复杂度为 O(n)，空间复杂度为 O(1) 。

        //示例 1:
        //输入: [1,2,3,4,5]
        //输出: true

        //示例 2:
        //输入: [5,4,3,2,1]
        //输出: false

        public bool IncreasingTriplet1(int[] nums)
        {
        //首先，新建两个变量 small 和 mid ，分别用来保存题目要我们求的长度为 3 的递增子序列的最小值和中间值。

        //接着，遍历数组，每遇到一个数字，将它和 small 和 mid 相比，
        //若小于等于 small ，则替换 small；
        //否则，若小于等于 mid，则替换 mid；
        //否则，若大于 mid，则说明我们找到了长度为 3 的递增数组！

        //上面的求解过程中有个问题：当已经找到了长度为 2 的递增序列，这时又来了一个比 small 还小的数字，为什么可以直接替换 small 呢，
        //            这样 small 和 mid 在原数组中并不是按照索引递增的关系呀？

        //Trick 就在这里了！假如当前的 small 和 mid 为[3, 5]，这时又来了个 1。
        //假如我们不将 small 替换为 1，那么，当下一个数字是 2，后面再接上一个 3 的时候，
        //我们就没有办法发现这个[1, 2, 3] 的递增数组了！也就是说，我们替换最小值，是为了后续能够更好地更新中间值！

        //另外，即使我们更新了 small ，这个 small 在 mid 后面，没有严格遵守递增顺序，
        //但它隐含着的真相是，有一个比 small 大比 mid 小的前·最小值出现在 mid 之前。
        //因此，当后续出现比 mid 大的值的时候，我们一样可以通过当前 small 和 mid 推断的确存在着长度为 3 的递增序列。 
        //所以，这样的替换并不会干扰我们后续的计算！
            int len = nums.Length;
            if (len < 3)
            {
                return false;
            }
            int small = int.MaxValue;
            int mid = int.MaxValue;
            foreach (var num in nums)
            {
                if (num <= small)
                {
                    small = num;
                }
                else if (num <= mid)
                {
                    mid = num;
                }
                else if (num > mid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IncreasingTriplet2(int[] nums)
        {
            int m1 = int.MaxValue, m2 = int.MaxValue;
            foreach (var  num in nums)
            {
                if (m1 >= num) m1 = num;
                else if (m2 >= num) m2 = num;
                else return true;
            }
            return false;
        }
    }
}


