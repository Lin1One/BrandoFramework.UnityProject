using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //300. 最长上升子序列
        //给定一个无序的整数数组，找到其中最长上升子序列的长度。

        //示例:

        //输入: [10,9,2,5,3,7,101,18]
        //输出: 4 
        //解释: 最长的上升子序列是[2, 3, 7, 101]，它的长度是 4。
        //说明:

        //可能会有多种最长上升子序列的组合，你只需要输出对应的长度即可。
        //你算法的时间复杂度应该为 O(n2) 。
        //进阶: 你能将算法的时间复杂度降低到 O(n log n) 吗?


        public int lengthOfLIS2(int[] nums)
        {
            if (nums.Length == 0)
            {
                return 0;
            }
            int[] dp = new int[nums.Length];
            dp[0] = 1;
            int result = 1;
            for (int i = 1; i < dp.Length; i++)
            {
                int maxval = 0;
                for (int j = 0; j < i; j++)
                {
                    if (nums[i] > nums[j])
                    {
                        maxval = Math.Max(maxval, dp[j]);
                    }
                }
                dp[i] = maxval + 1;
                result = Math.Max(result, dp[i]);
            }
            return result;
        }

        public int lengthOfLIS3(int[] nums)
        {
            //第 1 步：定义状态
            //由于一个子序列一定会以一个数结尾，于是将状态定义成：
            //dp[i] 表示以 nums[i] 结尾的「上升子序列」的长度。
            //注意：这个定义中 nums[i] 必须被选取，且必须是这个子序列的最后一个元素。

            //第 2 步：考虑状态转移方程
            //遍历到 nums[i] 时，需要把下标 i 之前的所有的数都看一遍；
            //只要 nums[i] 严格大于在它位置之前的某个数，
            //那么 nums[i] 就可以接在这个数后面形成一个更长的上升子序列；
            //因此，dp[i] 就等于下标 i 之前严格小于 nums[i] 的状态值的最大者 + 1。
            //语言描述：在下标 i 之前严格小于 nums[i] 的所有状态值中的最大者 + 1。

            //第 3 步：考虑初始化
            //dp[i] = 1，1 个字符显然是长度为 1 的上升子序列。

            //第 4 步：考虑输出
            //这里要注意，不能返回最后一个状态值；
            //还是根据定义，最后一个状态值只是以 nums[len - 1] 结尾的「上升子序列」的长度；
            //状态数组 dp 的最大值才是最后要输出的值。

            //第 5 步：考虑状态压缩。
            //遍历到一个新数的时候，之前所有的状态值都得保留，因此无法压缩。

            int len = nums.Length;
            if (len < 2)
            {
                return len;
            }

            int[] dp = new int[len];
            for(var i = 0;i< len;i++ )
            {
                dp[i] = 1;
            }

            for (int i = 1; i < len; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (nums[j] < nums[i])
                    {
                        dp[i] = Math.Max(dp[i], dp[j] + 1);
                    }
                }
            }

            int res = 0;
            for (int i = 0; i < len; i++)
            {
                res = Math.Max(res, dp[i]);
            }
            return res;
        }

        int lengthOfLIS(int[] nums)
        {
            int len = 1;
            int n = nums.Length;
            if (n == 0)
                return 0;
            var dp = new int[n +1];
            dp[0] = nums[0];
            for (int i = 1; i < n; ++i)
            {
                if (nums[i] > dp[len])
                {
                    len++;
                    dp[len] = nums[i];
                }
                else
                {
                    // 如果找不到说明所有的数都比 nums[i] 大，此时要更新 d[1]，
                    // 所以这里将 pos 设为 0
                    int l = 1, r = len, pos = 0; 
                    while (l <= r)
                    {
                        int mid = (l + r) >> 1;
                        if (dp[mid] < nums[i])
                        {
                            pos = mid;
                            l = mid + 1;
                        }
                        else
                        {
                            r = mid - 1;
                        }
                    }
                    dp[pos + 1] = nums[i];
                }
            }
            return len;
        }

        public int LengthOfLIS4(int[] nums)
        {
            //方法二：修改状态定义（同时用到了贪心算法、二分查找）
            //依然是着眼于一个上升子序列的结尾的元素；
            //如果已经得到的上升子序列的结尾的数越小，遍历的时候后面接上一个数，
            //就会有更大的可能性构成一个更长的上升子序列；
            //既然结尾越小越好，我们可以记录在长度固定的情况下，结尾最小的那个元素的数值，这样定义也是为了方便得到「状态转移方程」。
            //为了与之前的状态区分，这里将状态数组命名为 tail。

            //第 1 步：定义新状态（特别重要）
            //tail[i] 表示长度为 i + 1 的所有上升子序列的结尾的最小值。

            //说明：
            //tail[0] 表示长度为 1 的所有上升子序列中，结尾最小的那个元素的数值，
            //以题目中的示例为例[10, 9, 2, 5, 3, 7, 101, 18] 中，
            //容易发现长度为 2 的所有上升子序列中，
            //结尾最小的是子序列[2, 3] ，因此 tail[1] = 3
            //下标和长度有一个 1 的偏差；
            //状态定义其实也描述了状态转移方程。

            //第 2 步：思考状态转移方程
            //从直觉上看，数组 tail 也是一个严格上升数组。我们需要证明一下。
            //我们只需要维护状态数组 tail 的定义
            //它的长度就是最长上升子序列的长度。

            //下面说明如何在遍历中，如何维护状态数组 tail 的定义：
            //算法的执行流程
            //1、设置一个数组 tail，初始时为空；
            //注意：数组 tail 虽然是有序数组，但它不是问题中的「最长上升子序列」（下文还会强调），
            //不能命名为 LIS。有序数组 tail 只是用于求解 LIS 问题的辅助数组。

            //2、在遍历数组 nums 的过程中，每来一个新数 num，如果这个数严格大于有序数组 tail 的最后一个元素，
            //就把 num 放在有序数组 tail 的后面，否则进入第 3 点；
            //注意：这里的大于是「严格大于」，不包括等于的情况。

            //3、在有序数组 tail 中查找第 1 个等于大于 num 的那个数，试图让它变小；
            //如果有序数组 tail 中存在等于 num 的元素，什么都不做，
            //因为以 num 结尾的最短的「上升子序列」已经存在；
            //如果有序数组 tail 中存在大于 num 的元素，找到第 1 个，让它变小，
            //这样我们就找到了一个结尾更小的相同长度的上升子序列。
            //说明：我们再看一下数组 tail[i] 的定义：
            //长度为 i + 1 的所有最长上升子序列的结尾的最小值。
            //因此，在遍历的过程中，我们试图让一个大的值变小是合理的。

            //这一步可以认为是「贪心算法」，总是做出在当前看来最好的选择，当前「最好的选择」是：
            //当前只让让第 1 个严格大于 nums[i] 的数变小，变成 nums[i]，
            //这一步操作是“无后效性”的。
            //由于是在有序数组中的操作，因此可以使用「二分查找算法」。

            //4、遍历新的数 num ，先尝试上述第 2 点，第 2 点行不通则执行第 3 点，
            //直到遍历完整个数组 nums，最终有序数组 tail 的长度，
            //就是所求的“最长上升子序列”的长度。

            //第 3 步：思考初始化
            //dp[0] = nums[0]，在只有 1 个元素的情况下，它当然是长度为 1 并且结尾最小的元素。

            //第 4 步：思考输出
            //数组 tail 的长度，上文其实也已经说了，还是依据定义，
            //tail[i] 表示长度固定为 i + 1 的所有「上升子序列」的结尾元素中最小的那个，
            //长度最多就是数组 tail 的长度。

            //第 5 步：思考状态压缩
            //无法压缩。

            int len = nums.Length;
            if (len <= 1)
            {
                return len;
            }

            // tail 数组的定义：长度为 i + 1 的上升子序列的末尾最小是几
            int[] tail = new int[len];
            // 遍历第 1 个数，直接放在有序数组 tail 的开头
            tail[0] = nums[0];
            // end 表示有序数组 tail 的最后一个已经赋值元素的索引
            int end = 0;

            for (int i = 1; i < len; i++)
            {
                // 【逻辑 1】比 tail 数组实际有效的末尾的那个元素还大
                if (nums[i] > tail[end])
                {
                    // 直接添加在那个元素的后面，所以 end 先加 1
                    end++;
                    tail[end] = nums[i];
                }
                else
                {
                    // 使用二分查找法，在有序数组 tail 中
                    // 找到第 1 个大于等于 nums[i] 的元素，尝试让那个元素更小
                    int left = 0;
                    int right = end;
                    while (left < right)
                    {
                        // 选左中位数不是偶然，而是有原因的，原因请见 LeetCode 第 35 题题解
                        // int mid = left + (right - left) / 2;
                        int mid = left + ((right - left) >> 1);
                        if (tail[mid] < nums[i])
                        {
                            // 中位数肯定不是要找的数，把它写在分支的前面
                            left = mid + 1;
                        }
                        else
                        {
                            right = mid;
                        }
                    }
                    // 走到这里是因为 【逻辑 1】 的反面，
                    // 因此一定能找到第 1 个大于等于 nums[i] 的元素
                    // 因此，无需再单独判断
                    tail[left] = nums[i];
                }
            }
            // 此时 end 是有序数组 tail 最后一个元素的索引
            // 题目要求返回的是长度，因此 +1 后返回
            end++;
            return end;
        }

        public int lengthOfLIS5(int[] nums)
        {
            //DP
            var len = nums.Length;
            if (len < 2)
            {
                return len;
            }
            var maxCount = new int[len];

            for(var i =  0;i< len;i++)
            {
                maxCount[i] = 1;
            }
            int maxNum = 0;
            for(var i = 1;i< len;i ++)
            {
                for(var j = 0;j< i;j++)
                {
                    if(nums[i]> nums[j])
                    {
                        maxCount[i] = Math.Max(maxCount[j] + 1, maxCount[i]);
                    }
                }
                maxNum = Math.Max(maxCount[i], maxNum);
            }
            return maxNum;

        }
    }
}


