using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给出两个 非空 的链表用来表示两个非负的整数。
        // 其中，它们各自的位数是按照 逆序 的方式存储的，并且它们的每个节点只能存储 一位 数字。
        // 如果，我们将这两个数相加起来，则会返回一个新的链表来表示它们的和。
        // 您可以假设除了数字 0 之外，这两个数都不会以 0 开头。

        // 示例：
        // 输入：(2 -> 4 -> 3) + (5 -> 6 -> 4)
        // 输出：7 -> 0 -> 8
        // 原因：342 + 465 = 807
        //[5]
        //[5]
        public ListNode AddTwoNumbers1_Test(ListNode l1, ListNode l2) 
        {
            ListNode newList = new ListNode(0);
            ListNode newListHead = newList;
            int l1val = 0;
            int l2val = 0;
            int sum;
            bool Up = false;
            while(l1 != null || l2!= null)
            {
                 l1val = l1 != null?l1.val:0;
                 l2val = l2 != null?l2.val:0;
                sum = Up? l1val + l2val + 1:l1val + l2val;
                if(sum > 9)
                {
                    Up = true;
                }
                else
                    Up = false;
                int newValue = sum % 10;
                newList.next = new ListNode(newValue);
                newList = newList.next;
                l1 = l1 != null?l1.next:null;
                l2 = l2 != null?l2.next:null;
            }
            if(Up)
            {
                newList.next = new ListNode(1);
            }
            return newListHead.next;
        }
    
        public ListNode AddTwoNumbers2(ListNode l1,ListNode l2) 
        {
        
        ListNode targetListNode = new ListNode(0);
        ListNode currentListNode =  targetListNode;
        int isUp = 0;
        int l1val = 0;
        int l2val = 0;
        int targetVal;
        while(l1 != null || l2 !=null)
        {
             l1val = l1 != null?l1.val:0;
             l2val = l2 != null?l2.val:0;
             targetVal = l1val + l2val + isUp;
            isUp = targetVal/10;
            currentListNode.next = new ListNode(targetVal%10);
            currentListNode= currentListNode.next;
            l1 = l1!=null ?l1.next:null;
            l2 = l2!= null?l2.next:null;
        }
        if(isUp>0)
        {
            currentListNode.next = new ListNode(isUp);
        }
        
        return targetListNode.next;
        }
    }
}


