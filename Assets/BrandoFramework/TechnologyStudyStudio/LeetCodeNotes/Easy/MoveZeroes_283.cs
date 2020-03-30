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
        public void MoveZero0330(int[] nums)
        {
            int curIndex = 0;
            for(var i = 0;i< nums.Length;i++)
            {
                if(nums[i] != 0)
                {
                    var temp = nums[i];
                    nums[i] = 0;
                    nums[curIndex] = temp;
                    curIndex++;
                }
            }
        }

        public void MoveZero03302(int[] nums)
        {
            int curIndex = 0;
            for(var i = 0;i<nums.Length;i++)
            {
                if(nums[i] != 0)
                {
                    if( i != curIndex)
                    {
                        var temp = nums[i];
                        nums[i] = 0;
                        nums[curIndex] = temp;
                    }
                    curIndex++;
                }
            }
        }

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
        public void moveZeroes(int[] nums)
        {// 方法三：最优解
         //前一种方法的操作是局部优化的。
         // 例如，所有（除最后一个）前导零的数组：[0，0，0，…，0，1]。对数组执行多少写操作？对于前面的方法，它写 0 (n-1) 次，这是不必要的。
         //我们本可以只写一次。怎么用？… 只需固定非 0 元素。

            // 最优方法也是上述解决方案的一个细微扩展。一个简单的实现是，如果当前元素是非 0 的，那么它的正确位置最多可以是当前位置或者更早的位置。
            // 如果是后者，则当前位置最终将被非 0 或 0 占据，该非 0 或 0 位于大于 “cur” 索引的索引处。
            // 我们马上用 0 填充当前位置，这样不像以前的解决方案，我们不需要在下一个迭代中回到这里。
            //换句话说，代码将保持以下不变：
            //慢指针（lastnonzerofoundat）之前的所有元素都是非零的。
            //当前指针和慢速指针之间的所有元素都是零。
            //因此，当我们遇到一个非零元素时，我们需要交换当前指针和慢速指针指向的元素，然后前进两个指针。如果它是零元素，我们只前进当前指针。
            int lastNonZeroFoundAt = 0;
            for (int cur = 0; cur < nums.Length; cur++)
            {
                if (nums[cur] != 0)
                {
                    //swap(nums[lastNonZeroFoundAt], nums[cur]);
                    lastNonZeroFoundAt++;
                }
            }
        }


////        350. 两个数组的交集 II
////给定两个数组，编写一个函数来计算它们的交集。

////示例 1:

////输入: nums1 = [1,2,2,1], nums2 = [2,2]
////        输出: [2,2]
////        示例 2:

////输入: nums1 = [4,9,5], nums2 = [9,4,9,8,4]
////        输出: [4,9]
////        说明：

////输出结果中每个元素出现的次数，应与元素在两个数组中出现的次数一致。
////我们可以不考虑输出结果的顺序。
////进阶:

////如果给定的数组已经排好序呢？你将如何优化你的算法？
////如果 nums1 的大小比 nums2 小很多，哪种方法更优？
////如果 nums2 的元素存储在磁盘上，磁盘内存是有限的，并且你不能一次加载所有的元素到内存中，你该怎么办？
    }
}


