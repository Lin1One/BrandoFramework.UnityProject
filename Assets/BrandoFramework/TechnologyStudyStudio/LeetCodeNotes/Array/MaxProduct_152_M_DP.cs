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

        public int MaxProduct4(int[] nums)
        {
            //我们先定义一个数组 dpMax，用 dpMax[i] 表示以第 i 个元素的结尾的子数组，乘积最大的值，也就是这个数组必须包含第 i 个元素。

            //那么 dpMax[i] 的话有几种取值。

            //当 nums[i] >= 0 并且dpMax[i - 1] > 0，dpMax[i] = dpMax[i - 1] * nums[i]
            //当 nums[i] >= 0 并且dpMax[i - 1] < 0，此时如果和前边的数累乘的话，会变成负数，所以dpMax[i] = nums[i]
            //当 nums[i] < 0，此时如果前边累乘结果是一个很大的负数，和当前负数累乘的话就会变成一个更大的数。所以我们还需要一个数组 dpMin 来记录以第 i 个元素的结尾的子数组，乘积最小的值。
            //当dpMin[i - 1] < 0，dpMax[i] = dpMin[i - 1] * nums[i]
            //当dpMin[i - 1] >= 0，dpMax[i] = nums[i]
            //当然，上边引入了 dpMin 数组，怎么求 dpMin 其实和上边求 dpMax 的过程其实是一样的。

            //按上边的分析，我们就需要加很多的 if else来判断不同的情况，这里可以用个技巧。

            //我们注意到上边dpMax[i] 的取值无非就是三种，dpMax[i - 1] * nums[i]、dpMin[i - 1] * nums[i] 以及 nums[i]。

            //所以我们更新的时候，无需去区分当前是哪种情况，只需要从三个取值中选一个最大的即可。

            //dpMax[i] = max(dpMax[i - 1] * nums[i], dpMin[i - 1] * nums[i], nums[i]);

            //dpMin[i] = min(dpMax[i - 1] * nums[i], dpMin[i - 1] * nums[i], nums[i]);

            //更新过程中，我们可以用一个变量 max 去保存当前得到的最大值。


            int n = nums.Length;
            if (n == 0)
            {
                return 0;
            }

            int[] dpMax = new int[n];
            dpMax[0] = nums[0];
            int[] dpMin = new int[n];
            dpMin[0] = nums[0];
            int max = nums[0];
            for (int i = 1; i < n; i++)
            {
                dpMax[i] = Math.Max(dpMin[i - 1] * nums[i], Math.Max(dpMax[i - 1] * nums[i], nums[i]));
                dpMin[i] = Math.Min(dpMin[i - 1] * nums[i], Math.Min(dpMax[i - 1] * nums[i], nums[i]));
                max = Math.Min(max, dpMax[i]);
            }
            return max;
        }

        public int MaxProduct3(int[] nums)
        {
            if (nums.Length == 0)
            {
                return 0;
            }

            int max = 1;
            int res = nums[0];
            for (int i = 0; i < nums.Length; i++)
            {
                max *= nums[i];
                res = Math.Max(res, max);
                if (max == 0)
                {
                    max = 1;
                }
            }
            max = 1;
            for (int i = nums.Length - 1; i >= 0; i--)
            {
                max *= nums[i];
                res = Math.Max(res, max);
                if (max == 0)
                {
                    max = 1;
                }
            }
            return res;
        }

        public int MaxProduct2(int[] nums)
        {
            int finalMax = int.MinValue;
            int max = 1;
            int min = 1;
            for(var i = 0;i< nums.Length;i++)
            {
                var num = nums[i];
                if (num < 0)
                {
                    var temp = max;
                    max = min;
                    min = temp;
                }
                max = Math.Max(max * num, num);
                min = Math.Min(min * num, num);
                finalMax = Math.Max(finalMax, max);
            }
            return finalMax;
        }
        public int MaxProduct(int[] nums) 
        {
            //  标签：动态规划
            // 遍历数组时计算当前最大值，不断更新
            // 令imax为当前最大值，则当前最大值为 imax = max(imax * nums[i], nums[i])
            // 由于存在负数，那么会导致最大的变最小的，最小的变最大的。
            // 因此还需要维护当前最小值imin，imin = min(imin * nums[i], nums[i])
            // 当负数出现时则imax与imin进行交换再进行下一步计算
            // 时间复杂度：O(n)

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




