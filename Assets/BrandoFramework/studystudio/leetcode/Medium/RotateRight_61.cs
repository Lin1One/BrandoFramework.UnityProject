using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 61. 旋转链表
        // 给定一个链表，旋转链表，将链表每个节点向右移动 k 个位置，其中 k 是非负数。

        // 示例 1:
        // 输入: 1->2->3->4->5->NULL, k = 2
        // 输出: 4->5->1->2->3->NULL
        // 解释:
        // 向右旋转 1 步: 5->1->2->3->4->NULL
        // 向右旋转 2 步: 4->5->1->2->3->NULL

        // 示例 2:
        // 输入: 0->1->2->NULL, k = 4
        // 输出: 2->0->1->NULL
        // 解释:
        // 向右旋转 1 步: 2->0->1->NULL
        // 向右旋转 2 步: 1->2->0->NULL
        // 向右旋转 3 步: 0->1->2->NULL
        // 向右旋转 4 步: 2->0->1->NULL

        public ListNode RotateRight(ListNode head, int k) 
        {
            if(head == null)
            {
                return null;
            }
            if(head.next == null)
            {
                return head;
            }
            int Count = 1;
            ListNode list = head;
            while(list.next != null)
            {
                list = list.next;
                Count++;
            }
            list.next = head;

            k = k % Count;
            var i = 1;
            ListNode newTail = head;
            while(Count - i > k)
            {
                newTail = newTail.next;
                i++;
            }
            var newHead = newTail.next;
            newTail.next = null;
            return newHead;
        }
    }
}


