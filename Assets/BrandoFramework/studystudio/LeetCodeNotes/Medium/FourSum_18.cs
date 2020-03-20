using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 18. 四数之和
        // 给定一个包含 n 个整数的数组 nums 和一个目标值 target，
        // 判断 nums 中是否存在四个元素 a，b，c 和 d ，
        // 使得 a + b + c + d 的值与 target 相等？找出所有满足条件且不重复的四元组。

        // 注意：
        // 答案中不可以包含重复的四元组。

        // 示例：
        // 给定数组 nums = [1, 0, -1, 0, -2, 2]，和 target = 0。
        // 满足要求的四元组集合为：
        // [
        //   [-1,  0, 0, 1],
        //   [-2, -1, 1, 2],
        //   [-2,  0, 0, 2]
        // ]

        private IList<IList<int>> FourSumResult = new List<IList<int>>();
        public IList<IList<int>> FourSum(int[] nums, int target) 
        {
            if(nums == null || nums.Length < 4)
            {
                return FourSumResult;
            }
            Array.Sort(nums);
            SearFourNum(nums,target,new List<int>(),0,0);
            return FourSumResult;
        }
        private void SearFourNum(int[] nums,
            int target,
            IList<int> currentNums,
            int currentIndex,
            int currentSum)
        {
            if(currentNums.Count == 4)
            {
                if(currentSum == target)
                {
                    FourSumResult.Add(new List<int>(currentNums));
                }
                return;
            }
            for(var i = currentIndex;i < nums.Length;i++)
            {
                if(i > currentIndex &&  nums[i] == nums[i-1])
                {
                    continue;
                }
                currentNums.Add(nums[i]);
                currentSum += nums[i];
                SearFourNum(nums,target,currentNums,i+1,currentSum);
                currentNums.RemoveAt(currentNums.Count - 1);
                currentSum -= nums[i];
            }
        }


// [-3,-1,0,2,4,5]
// 0
        //private IList<IList<int>> FourSumResult = new List<IList<int>>();
        public IList<IList<int>> FourSum2(int[] nums, int target) 
        {
            if(nums == null || nums.Length < 4)
            {
                return FourSumResult;
            }
            Array.Sort(nums);
            int firstNumPtr = 0;
            int secondNumPtr = nums.Length - 1;

            bool left = true;
            while(secondNumPtr > firstNumPtr + 2)
            {
                List<int> currentNums = new List<int>();
                int thirdNumPtr = firstNumPtr + 1;
                int fourthNumPtr = secondNumPtr - 1;
                currentNums.Add(nums[firstNumPtr]);
                currentNums.Add(nums[secondNumPtr]);
                int secondSum = nums[firstNumPtr] + 
                    nums[secondNumPtr];
                while(thirdNumPtr < fourthNumPtr)
                {
                    int tempSum = secondSum + nums[thirdNumPtr] + nums[fourthNumPtr];
                    if(tempSum > target)
                    {
                        fourthNumPtr--;
                    }
                    else if(tempSum < target)
                    {
                        thirdNumPtr++;
                    }
                    else
                    {
                        currentNums.Add(nums[thirdNumPtr]);
                        currentNums.Add(nums[fourthNumPtr]);
                        FourSumResult.Add(new List<int>(currentNums));
                        currentNums.RemoveAt(currentNums.Count - 1);
                        currentNums.RemoveAt(currentNums.Count - 1);
                        thirdNumPtr++;
                    }
                }
                if(left)
                {
                    left = false;
                    firstNumPtr++;
                }
                else
                {
                    left = true;
                    secondNumPtr--;
                }
            }
            return FourSumResult;
        }
        //K 数通用函数
        public  IList<IList<int>> kSum(int[] nums,int target,int k, int start)
        {
            IList<IList<int>> res = new List<IList<int>>();
            if(start >= nums.Length)
                return res;
            if(k==2)
            {
                int l = start;
                int h = nums.Length-1;
                while(l < h)
                {
                    if(nums[l]+nums[h]==target)
                    {
                        List<int> list = new List<int>();
                        list.Add(nums[l]);
                        list.Add(nums[h]);
                        res.Add(list);
                        while( l < h && nums[l]==nums[l+1])
                            l++;
                        while(l < h && nums[h]==nums[h-1])
                            h--;
                        l++;
                        h--;
                    }
                    else if(nums[l]+nums[h]<target)
                        l++;
                    else
                        h--;
                }
                return res;
            }
            if( k > 2 )
            {
                for(int i = start; i < nums.Length - k +1;i++)
                {
                    IList<IList<int>> temp = kSum(nums, target - nums[i], k - 1, i + 1);
                    if(temp.Count != 0) 
                    {
                        foreach (IList<int> list in temp) 
                        {
                            list.Add(nums[i]);
                            res.Add(list);
                        }
                    }
                    while(i<nums.Length-1 && nums[i]==nums[i+1])
                    {
                        i++;
                    }
                }
                return res;
            }
            return res;
        }
    
        public IList<IList<int>> FourSum3(int[] nums, int target)
    {
        Array.Sort(nums); // 从小到大排序
        var res = new List<IList<int>>();
        var length = nums.Length;
        for (int first = 0; first < length - 3; first++)
        {
            if (first != 0 && nums[first] == nums[first - 1]) continue; // 第一个数去重
            if (nums[first] + nums[first + 1] + nums[first + 2] + nums[first + 3] > target) 
            continue;
            //if (nums[first] + nums[length - 3] + nums[length - 2] + nums[length - 1] < target) break;

            for (int second = first + 1; second < length - 2; second++)
            {
                if (second != first + 1 && nums[second] == nums[second - 1]) 
                    continue; // 第二个数去重

                var third = second + 1;
                var fourth = length - 1;
                if (nums[first] + nums[second] + nums[third] + nums[third + 1] > target) 
                    continue;
                //if (nums[first] + nums[second] + nums[fourth - 1] + nums[fourth] < target) break;

                while (third < fourth)
                {
                    while (third < fourth && third > second + 1 && nums[third - 1] == nums[third]) 
                        third++; // 第三个数去重
                    while (third < fourth && fourth < length - 1 && nums[fourth] == nums[fourth + 1]) 
                        fourth--; // 第四个数去重
                    if (third >= fourth) 
                        break;
                    int sum = nums[first] + nums[second] + nums[third] + nums[fourth];

                    if (sum == target) 
                        res.Add(new int[] { nums[first], nums[second], nums[third++], nums[fourth--] });
                    else if (sum < target) 
                        third++;
                    else 
                        fourth--;
                }
            }
        }
        return res;
    }
    }
}




