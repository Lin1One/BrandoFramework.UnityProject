using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 215. 数组中的第K个最大元素
        // 在未排序的数组中找到第 k 个最大的元素。
        // 请注意，你需要找的是数组排序后的第 k 个最大的元素，而不是第 k 个不同的元素。

        // 示例 1:
        // 输入: [3,2,1,5,6,4] 和 k = 2
        // 输出: 5

        // 示例 2:
        // 输入: [3,2,3,1,2,4,5,5,6] 和 k = 4
        // 输出: 4


        public int FindKthLargest(int[] nums, int k) 
        {
            Array.Sort(nums);
            return nums[nums.Length - k];
        }

        public int FindKthLargest1(int[] nums, int k)
        {
            // 方法一：堆
            // 思路是创建一个大顶堆，将所有数组中的元素加入堆中，
            // 并保持堆的大小小于等于 k。这样，堆中就保留了前 k 个最大的元素。这样，堆顶的元素就是正确答案。
            // 像大小为 k 的堆中添加元素的时间复杂度为 O(logk)，
            // 我们将重复该操作 N 次，故总时间复杂度为 O(Nlogk)。

            // 在 Python 的 heapq 库中有一个 nlargest 方法，具有同样的时间复杂度，能将代码简化到只有一行。
            // 本方法优化了时间复杂度，但需要 O(k) 的空间复杂度。
            return 0;
//  // init heap 'the smallest element first'
//         PriorityQueue<int> heap =
//             new PriorityQueue<int>((n1, n2) -> n1 - n2);

//         // keep k largest elements in the heap
//         for (int n: nums) {
//           heap.add(n);
//           if (heap.size() > k)
//             heap.poll();
//         }

//         // output
//         return heap.poll();        
        }

        public int FindKthLargest2(int[] nums, int k) 
        {
            // 方法二：快速选择
            // 快速选择算法 的平均时间复杂度为 O(N)。就像快速排序那样，本算法也是 Tony Hoare 发明的，
            // 因此也被称为 Hoare选择算法。

            // 本方法大致上与快速排序相同。简便起见，注意到第 k 个最大元素也就是第 N - k 个最小元素，
            // 因此可以用第 k 小算法来解决本问题。
            // 首先，我们选择一个枢轴，并在线性时间内定义其在排序数组中的位置。
            // 为了实现划分，沿着数组移动，将每个元素与枢轴进行比较，并将小于枢轴的所有元素移动到枢轴的左侧。
            // 这样，在输出的数组中，枢轴达到其合适位置。
            // 所有小于枢轴的元素都在其左侧，所有大于或等于的元素都在其右侧。
            // 这样，数组就被分成了两部分。如果是快速排序算法，会在这里递归地对两部分进行快速排序，
            // 时间复杂度为 O(NlogN)。
            // 而在这里，由于知道要找的第 N - k 小的元素在哪部分中，
            // 我们不需要对两部分都做处理，这样就将平均时间复杂度下降到 O(N)。

            // 最终的算法十分直接了当 :
            // 随机选择一个枢轴。
            // 使用划分算法将枢轴放在数组中的合适位置 pos。将小于枢轴的元素移到左边，大于等于枢轴的元素移到右边。
            // 比较 pos 和 N - k 以决定在哪边继续递归处理。
            // ! 注意，本算法也适用于有重复的数组

            int size = nums.Length;
            // kth largest is (N - k)th smallest
            return Quickselect(0, size - 1, size - k,nums);
        }

        public int Quickselect(int left, int right, int k_smallest,int[] nums) 
        {
            if (left == right)
                return nums[left]; 

            System.Random random_num = new System.Random();
            int pivot_index = left + random_num.Next(right - left); 
            pivot_index = Partition(left, right, pivot_index,nums);

            if (k_smallest == pivot_index)
                return nums[k_smallest];
            else if (k_smallest < pivot_index)
                return Quickselect(left, pivot_index - 1, k_smallest,nums);
            else
                return Quickselect(pivot_index + 1, right, k_smallest,nums);
        }

        ///划分
        public int Partition(int left, int right, int pivot_index,int[] nums) 
        {
            int pivot = nums[pivot_index];
            // 1. move pivot to end
            Swap(pivot_index, right,nums);
            int store_index = left;

            // 2. move all smaller elements to the left
            for (int i = left; i <= right; i++) 
            {
                if (nums[i] < pivot) 
                {
                    Swap(store_index, i,nums);
                    store_index++;
                }
            }
            // 3. move pivot to its final place
            Swap(store_index, right,nums);
            return store_index;
        }

        public void Swap(int a, int b,int[] nums) 
        {
            int tmp = nums[a];
            nums[a] = nums[b];
            nums[b] = tmp;
        }
        public int FindKthLargest3(int[] nums,int k)
        {
            return QuickSelect(nums, 0, nums.Length - 1, k);
        }

        private int QuickSelect(int[] nums ,int left,int right,int k_largest)
        {
            if(left >= right)
            {
                return nums[left];
            }
            var pivotIndex = left;
            var pivot = nums[left];
            var l = left;
            var r = right;
            while(l < r)
            {
                while(nums[r] >= pivot && l < r)
                {
                    r--;
                }
                while(nums[l] <= pivot && l < r )
                {
                    l++;
                }
                if(l < r)
                {
                    Swap(l, r, nums);
                }
            }
            nums[left] = nums[l];
            nums[l] = pivot;
            pivotIndex = l;
            if (pivotIndex < nums.Length - k_largest)
                return QuickSelect(nums, pivotIndex + 1, right, k_largest);
            else if (pivotIndex > nums.Length  - k_largest)
                return QuickSelect(nums, left, pivotIndex - 1, k_largest);
            else
                return pivot;

        }

    }
}




