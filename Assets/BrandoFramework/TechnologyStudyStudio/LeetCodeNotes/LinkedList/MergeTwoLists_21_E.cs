using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 将两个有序链表合并为一个新的有序链表并返回。
        // 新链表是通过拼接给定的两个链表的所有节点组成的。 
        // 示例：
        // 输入：1->2->4, 1->3->4
        // 输出：1->1->2->3->4->4

        //迭代
        public ListNode MergeTwoLists(ListNode l1, ListNode l2) 
        {
            List<>.in
            ListNode newListNode = new ListNode(0);
            ListNode newListHead = newListNode;
            ListNode firstList = l1;
            ListNode secondList = l2;

            while(firstList != null && secondList !=null)
            {
                if(firstList.val < secondList.val)
                {
                    newListNode.next = firstList;     
                    firstList = firstList.next;      
                }
                else
                {
                    newListNode.next = secondList;
                    secondList = secondList.next;
                }
                newListNode = newListNode.next;
            }
            newListNode.next = firstList != null?firstList :secondList;
            return newListHead.next;
        }
    }
}


