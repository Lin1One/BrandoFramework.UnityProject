using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定两个非空链表来代表两个非负整数。数字最高位位于链表开始位置。
        // 它们的每个节点只存储单个数字。将这两数相加会返回一个新的链表。

        // 你可以假设除了数字 0 之外，这两个数字都不会以零开头。

        // 进阶:

        // 如果输入链表不能修改该如何处理？换句话说，你不能对列表中的节点进行翻转。

        // 示例:
        // 输入: (7 -> 2 -> 4 -> 3) + (5 -> 6 -> 4)
        // 输出: 7 -> 8 -> 0 -> 7

        #region 补 0
        // 执行用时 :
        //260 ms 10.71%
        // 内存消耗 :
        // 25.8 MB 16.67%
        public ListNode AddTwoNumbers(ListNode l1, ListNode l2) 
        {
            ListNode newList = new ListNode(1);
            ListNode newListHead = newList;
            int l1NodeCount = GetListNodeCount(l1);
            int l2NodeCount = GetListNodeCount(l2);
            int lengthLess = l1NodeCount - l2NodeCount;
            int less = Math.Abs(lengthLess);

            ListNode addZeroList = new ListNode(0);
            ListNode zeroListHead = addZeroList;
            while(less > 0)
            {
                less--;
                ListNode newNode = new ListNode(0);
                addZeroList.next = newNode;
                addZeroList = addZeroList.next;
            }

            if(lengthLess > 0)
            {
                addZeroList.next = l2;
                newList.next = AddTwoNode(l1,zeroListHead.next);
            }
            else if(lengthLess < 0)
            {
                addZeroList.next = l1;
                newList.next = AddTwoNode(zeroListHead.next,l2);
            }
            else
            {
                newList.next = AddTwoNode(l1,l2);
            }
            if(isUp)
            {
                 return newListHead;
            }
            return newListHead.next;
        }

        private int GetListNodeCount(ListNode l)
        {
            int i =0; 
            while(l != null)
            {
                l = l.next;
                i++;
            }
            return i;
        }
        bool isUp = false;
        
        // [7,2,4,3]
        // [0,5,6,4]
        private ListNode AddTwoNode(ListNode l1, ListNode l2)
        {
            if(l1 != null && l2!=null)
            {
                ListNode newNode = new ListNode(0);
                newNode.next = AddTwoNode(l1.next,l2.next);
                int sum = isUp? l1.val + l2.val + 1:l1.val + l2.val;
                isUp = sum > 9;
                newNode.val = sum % 10;
                return newNode;
            }
            return null;
        }

    #endregion
        
        #region 辅助栈
        //栈
        public ListNode AddTwoNumbers3(ListNode l1, ListNode l2)
        {
            Stack<int> stack1 = new Stack<int>();
            Stack<int> stack2 = new Stack<int>();
            ListNode node1=l1;
            while(node1!=null)
            {
                stack1.Push(node1.val);
                node1 = node1.next;
            }

            ListNode node2=l2;
            while(node2!=null)
            {
                stack2.Push(node2.val);
                node2 = node2.next;
            }
            ListNode head=null;
            int flag = 0;
            while(stack1.Count != 0||stack2.Count == 0|| flag!=0)
            {
                int value = 0;
                if(stack1.Count!=0)
                    value+=stack1.Pop();
                if(stack2.Count!=0)
                    value+=stack2.Pop();
                value += flag;
                ListNode node=new ListNode(value%10);
                flag=value/10;
                node.next=head;
                head =node;
            }
            return head;
        }
    #endregion

    }
}


