using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //删除链表中等于给定值 val 的所有节点。

        //示例:

        //输入: 1->2->6->3->4->5->6, val = 6
        //输出: 1->2->3->4->5

        public ListNode RemoveElements(ListNode head, int val)
        {
            var newList = new ListNode(0);
            var newHead = newList;
            newList.next = head;
            while (newList != null && newList.next != null)
            {
                if (newList.next.val == val)
                {
                    newList.next = newList.next.next;
                }
                else
                {
                    newList = newList.next;
                }
            }
            return newHead.next;
        }
    }
}


