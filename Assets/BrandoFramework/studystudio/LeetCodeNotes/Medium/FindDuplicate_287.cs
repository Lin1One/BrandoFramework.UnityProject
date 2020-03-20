using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //287. 寻找重复数
        // 给定一个包含 n + 1 个整数的数组 nums，其数字都在 1 到 n 之间（包括 1 和 n），
        // 可知至少存在一个重复的整数。假设只有一个重复的整数，找出这个重复的数。
        
        // 示例 1:
        // 输入: [1,3,4,2,2]
        // 输出: 2
        
        // 示例 2:
        // 输入: [3,1,3,4,2]
        // 输出: 3
        // 说明：

        // 不能更改原数组（假设数组是只读的）。
        // 只能使用额外的 O(1) 的空间。
        // 时间复杂度小于 O(n2) 。
        // 数组中只有一个重复的数字，但它可能不止重复出现一次。


        // 方法三：弗洛伊德的乌龟和兔子（循环检测）
        // 如果我们对 nums 进行这样的解释，即对于每对索引 i 和值 v_i 而言，“下一个” v_j位于索引 v_i处，
        // 我们可以将此问题减少到循环检测。
        // 算法：
        // 首先，我们可以很容易地证明问题的约束意味着必须存在一个循环。
        //因为 nums 中的每个数字都在 1 和 n 之间，所以它必须指向存在的索引。
        //此外，由于 0 不能作为 nums 中的值出现，nums[0] 不能作为循环的一部分。

        //此题本质上就是链表找环的入口，下标可以形成一个链表环，环的入口即为重复数
        //快慢指针
        public int FindDuplicate(int[] nums) 
        {
            int slowPtr = nums[0];
            int fastPtr = nums[0];
            do 
            {
                slowPtr = nums[slowPtr];
                fastPtr = nums[fastPtr];
                fastPtr = nums[fastPtr];
            } 
            while (fastPtr != slowPtr);

            int ptr1 = nums[0];
            int ptr2 = slowPtr;
            while (ptr1 != ptr2) 
            {
                ptr1 = nums[ptr1];
                ptr2 = nums[ptr2];
            }
            return ptr1;
        }   
    }
}


