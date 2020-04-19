using System.Collections.Generic;
using UnityEngine;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 23. 合并K个排序链表
        // 合并 k 个排序链表，返回合并后的排序链表。请分析和描述算法的复杂度。

        // 示例:

        // 输入:
        // [
        //   1->4->5,
        //   1->3->4,
        //   2->6
        // ]
        // 输出: 1->1->2->3->4->4->5->6


        public ListNode MergeKLists(ListNode[] lists) 
        {
            // 逐一合并
            if (lists == null ||lists.Length==0)
            {
                return null;
            }
            if(lists.Length == 1)
            {
                return lists[0];
            }
            ListNode ans = lists[0];
            for(int i = 1; i< lists.Length; i++)
            {
                ans = MergeList(ans,lists[i]);
            }
            return ans;
        }

        private ListNode MergeList(ListNode l1,ListNode l2)
        {
            ListNode newList = new ListNode(0);
            ListNode result = newList;
            ListNode list1 = l1;
            ListNode list2 = l2;
            while(list1 != null && list2 != null)
            {
                if(list1.val < list2.val)
                {
                    newList.next = list1;
                    list1 = list1.next;
                }
                else
                {
                    newList.next = list2;
                    list2 = list2.next;
                }
                newList = newList.next;
            }
            newList.next = list1 != null? list1 : list2;
            return result.next;
        }

        //优先级队列

        // 方法 5：分治
        // 将 k 个链表配对并将同一对中的链表合并。
        public ListNode MergeKLists2(ListNode[] lists) 
        {
            if (lists == null || lists.Length == 0) 
                return null;
            return Merge(lists, 0, lists.Length - 1);
        }

        private ListNode Merge(ListNode[] lists, int left, int right) 
        {
            if (left == right) 
                return lists[left];
            int mid = left + (right - left) / 2;
            ListNode l1 = Merge(lists, left, mid);
            ListNode l2 = Merge(lists, mid + 1, right);
            return MergeList(l1, l2);
        }
    }
}


