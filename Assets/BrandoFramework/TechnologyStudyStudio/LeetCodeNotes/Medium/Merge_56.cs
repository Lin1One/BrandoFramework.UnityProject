using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 合并区间
        // 给出一个区间的集合，请合并所有重叠的区间。

        // 示例 1:
        // 输入: [[1,3],[2,6],[8,10],[15,18]]
        // 输出: [[1,6],[8,10],[15,18]]
        // 解释: 区间 [1,3] 和 [2,6] 重叠, 将它们合并为 [1,6].

        // 示例 2:
        // 输入: [[1,4],[4,5]]
        // 输出: [[1,5]]
        // 解释: 区间 [1,4] 和 [4,5] 可被视为重叠区间。

        public int[][] Merge2(int[][] intervals) 
        {
            List<int[]> ansList = new List<int[]>();
            Array.Sort(intervals,(a,b)=> a[0].CompareTo(b[0]));
            for(var i = 0;i<intervals.Length;i++)
            {
                if(ansList.Count == 0)
                {
                    ansList.Add(intervals[i]);
                    continue;
                }
                var left = intervals[i][0];
                var right = intervals[i][1];
                var rightestInterval = ansList[ansList.Count - 1];
                if(left > rightestInterval[1])
                {
                    ansList.Add(intervals[i]);
                    continue;
                }
                else
                {
                    if(right > rightestInterval[1])
                    {
                        rightestInterval[1] = right;
                    }
                }
            }
            return ansList.ToArray();
        }


    // 方法 2：排序
    // 如果我们按照区间的 start 大小排序，
    // 那么在这个排序的列表中可以合并的区间一定是连续的。
    // 首先，我们将列表按上述方式排序。
    // 然后，我们将第一个区间插入 merged 数组中，
    // 然后按顺序考虑之后的每个区间：
    // 如果当前区间的左端点在前一个区间的右端点之后，
    // 那么他们不会重合，我们可以直接将这个区间插入 merged 中；
    // 否则，他们重合，我们用当前区间的右端点更新前一个区间的右端点 end 
    // 如果前者数值比后者大的话。

// 一个简单的证明：假设算法在某些情况下没能合并两个本应合并的区间，
//那么说明存在这样的三元组 i，j 和 k 以及区间 intsints 
//满足 i < j < ki<j<k 并且 (ints[i]ints[i], ints[k]ints[k]) 可以合并，
//而 (ints[i]ints[i], ints[j]ints[j]) 和 (ints[j]ints[j], ints[k]ints[k]) 
//不能合并。这说明满足下面的不等式：

// ints[i].end < ints[j].start \\ ints[j].end < ints[k].start \\ ints[i].end \geq ints[k].start \\
// ints[i].end<ints[j].start
// ints[j].end<ints[k].start
// ints[i].end≥ints[k].start

// 我们联立这些不等式（注意还有一个显然的不等式 ints[j].start \leq ints[j].endints[j].start≤ints[j].end），可以发现冲突：

// \begin{aligned} ints[i].end < ints[j].start \leq ints[j].end < ints[k].start \\ ints[i].end \geq ints[k].start \end{aligned}
// ints[i].end<ints[j].start≤ints[j].end<ints[k].start
// ints[i].end≥ints[k].start
// ​	
 

// 因此，所有能够合并的区间必然是连续的。

// 考虑上面的例子，当列表已经排好序，能够合并的区间构成了连通块。

    // private class IntervalComparator  Comparator<Interval> 
    // {
    //     public int compare(Interval a, Interval b) {
    //         return a.start < b.start ? -1 : a.start == b.start ? 0 : 1;
    //     }
    // }

    // public int[][] merge(int[][] intervals) 
    // {
    //     Collections.sort(intervals, new IntervalComparator());

    //     LinkedList<Interval> merged = new LinkedList<Interval>();
    //     for (Interval interval : intervals) 
    //     {

    //         if (merged.isEmpty() || 
    //             merged.getLast().end < interval[0])
    //         {
    //             merged.add(interval);
    //         }
    //         else {
    //             merged.getLast().end = Math.max(merged.getLast().end, interval.end);
    //         }
    //     }

    //     return merged;
    // }

// 复杂度分析

// 时间复杂度：O(nlogn)

// 除去 sort 的开销，我们只需要一次线性扫描，
//所以主要的时间开销是排序的 O(nlgn)O(nlgn)

// 空间复杂度：O(1)O (or O(n)O(n))

// 如果我们可以原地排序 intervals ，就不需要额外的存储空间；
// 否则，我们就需要一个线性大小的空间去存储 intervals 的备份，来完成排序过程。
    }
}


