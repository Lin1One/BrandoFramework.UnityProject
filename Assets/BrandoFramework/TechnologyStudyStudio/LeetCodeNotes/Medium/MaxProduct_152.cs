using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 152. 乘积最大子序列
        // 给定一个整数数组 nums ，找出一个序列中乘积最大的连续子序列（该序列至少包含一个数）。
        // 示例 1:
        // 输入: [2,3,-2,4]
        // 输出: 6
        // 解释: 子数组 [2,3] 有最大乘积 6。

        // 示例 2:
        // 输入: [-2,0,-1]
        // 输出: 0
        // 解释: 结果不能为 2, 因为 [-2,-1] 不是子数组。


        //  标签：动态规划
        // 遍历数组时计算当前最大值，不断更新
        // 令imax为当前最大值，则当前最大值为 imax = max(imax * nums[i], nums[i])
        // 由于存在负数，那么会导致最大的变最小的，最小的变最大的。
        // 因此还需要维护当前最小值imin，imin = min(imin * nums[i], nums[i])
        // 当负数出现时则imax与imin进行交换再进行下一步计算
        // 时间复杂度：O(n)

        public int MaxProduct(int[] nums) 
        {
            int max = int.MinValue;
            int imax = 1;
            int imin = 1;
            for(int i=0; i<nums.Length; i++)
            {
                //遇负数
                if(nums[i] < 0)
                { 
                    int tmp = imax;
                    imax = imin;
                    imin = tmp;
                }
                imax = Math.Max(imax*nums[i], nums[i]);
                imin = Math.Min(imin*nums[i], nums[i]);
                max = Math.Max(max, imax);
            }
            return max;
        }
    }
}




