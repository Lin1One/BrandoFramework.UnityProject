using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 24. 两两交换链表中的节点
        // 给定一个链表，两两交换其中相邻的节点，并返回交换后的链表。
        // 你不能只是单纯的改变节点内部的值，而是需要实际的进行节点交换。

        // 示例:
        // 给定 1->2->3->4, 你应该返回 2->1->4->3.

        public ListNode SwapPairs1(ListNode head) 
        {
            if(head == null)
            {
                return null;
            }
            ListNode newListHead = new ListNode(0);
            ListNode previousNode = newListHead;
            ListNode firstNode = head;
            previousNode.next = firstNode;
            while(firstNode != null && firstNode.next != null)
            {
                var tempNode = firstNode.next;
                previousNode.next = tempNode;
                firstNode.next = tempNode.next;
                tempNode.next = firstNode;
                previousNode = firstNode;
                firstNode = firstNode.next;
            }
            return newListHead.next;
        }

        public ListNode SwapPairs2(ListNode head) 
        {
            if(head == null || head.next == null)
            {
                return head;
            }
            var first = head;
            var second = head.next;
            var temp = second.next;
            second.next = first;
            first.next = SwapPairs2(temp);
            return second;
        }
        

    }
}


