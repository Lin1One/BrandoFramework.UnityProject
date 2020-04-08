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
        //350. 两个数组的交集 II
        //给定两个数组，编写一个函数来计算它们的交集。

        //示例 1:

        //输入: nums1 = [1,2,2,1], nums2 = [2,2]
        //输出: [2,2]
        //示例 2:

        //输入: nums1 = [4,9,5], nums2 = [9,4,9,8,4]
        //输出: [4,9]
        //说明：

        //输出结果中每个元素出现的次数，应与元素在两个数组中出现的次数一致。
        //我们可以不考虑输出结果的顺序。
        //进阶:

        //如果给定的数组已经排好序呢？你将如何优化你的算法？
        //如果 nums1 的大小比 nums2 小很多，哪种方法更优？
        //如果 nums2 的元素存储在磁盘上，磁盘内存是有限的，并且你不能一次加载所有的元素到内存中，你该怎么办？

        public int[] Intersect(int[] nums1, int[] nums2)
        {
            //Hash 表
            Dictionary<int, int> num1Dic = new Dictionary<int, int>();
            for(var i = 0;i< nums1.Length;i++)
            {
                if(!num1Dic.ContainsKey(nums1[i]))
                {
                    num1Dic.Add(nums1[i], 0);
                }
                num1Dic[nums1[i]]++;
            }
            List<int> result = new List<int>();
            foreach(var num in nums2)
            {
                if(num1Dic.ContainsKey(num) && num1Dic[num] > 0)
                {
                    num1Dic[num]--;
                    result.Add(num);
                }
            }
            return result.ToArray();
        }

        public int[] Intersect2(int[] nums1, int[] nums2)
        {
            Array.Sort(nums1);
            Array.Sort(nums2);
            int i = 0, j = 0, k = 0;
            while (i < nums1.Length && j < nums2.Length)
            {
                if (nums1[i] < nums2[j])
                {
                    i++;
                }
                else if (nums1[i] > nums2[j])
                {
                    j++;
                }
                else
                {
                    nums1[k] = nums1[i];
                    k++;
                    i++;
                    ++j;
                }
            }
            var res = new int[k];
            Array.Copy(nums1, res, k);
            return res;
        }
    }
}

