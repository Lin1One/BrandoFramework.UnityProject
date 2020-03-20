using System.Collections.Generic;
using UnityEngine;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 请判断一个链表是否为回文链表。
        
        // 示例 1:
        // 输入: 1->2
        // 输出: false
        // 示例 2:
        // 输入: 1->2->2->1
        // 输出: true
        
        // 进阶：
        // 你能否用 O(n) 时间复杂度和 O(1) 空间复杂度解决此题？


        //双指针
        public bool IsPalindrome(ListNode head) 
        {
            // 边缘条件: 链表为空或只有一位时 返回真
            if (head == null || head.next == null) 
            {
                return true;
            }
            // 边缘条件: 链表只有两位时 直接判断这两位是否相等
            if (head.next.next == null) 
            {
                return head.val == head.next.val;
            }
            // 快慢指针
            ListNode slow = head;
            ListNode fast = head;
            // 声明last指针 用以记录循环的慢指针上一次遍历的节点
            // 在遍历时转向前半部分链表
            ListNode lastNode = null;
            // 用以记录遍历过程的临时值
            ListNode tmp = null;
            while (fast.next != null && fast.next.next != null) 
            {
                // 记录下当前遍历的节点 等下赋值给last指针
                tmp = slow;
                slow = slow.next;
                fast = fast.next.next;

                // 反转链表
                if (lastNode == null) 
                {
                    // 第一次循环 last指针指向头部 尾部置空（变头为尾）
                    lastNode = head;
                    lastNode.next = null;
                } 
                else 
                {
                    // 后续循环 将last指针赋值给当前遍历节点的下一位
                    tmp.next = lastNode;
                    // last指针更新为当前遍历节点
                    lastNode = tmp;
                }
            }

            // 重新定义快慢指针使命
            // 慢指针: 往前遍历; 快指针: 往后遍历
            if (fast.next == null) 
            {
                // 如果是奇数个 slow此时位于中间数位置
                // fast需要向后一位
                // slow需要向前一位 即当前tmp值
                fast = slow.next;
                slow = tmp;
            } 
            else 
            {
                // 如果是偶数个 slow此时位于前半部分最后一位数位置
                // fast需要向后一位
                // slow不需要移动 但是需要补充指针转向操作
                fast = slow.next;
                slow.next = tmp;
            }
            // 遍历判断
            while (slow != null || fast != null) 
            {
                if (slow == null || 
                    fast == null || 
                    slow.val != fast.val) 
                {
                    return false;
                }
                slow = slow.next;
                fast = fast.next;
            }
            return true;
        }

        public bool IsPalindrome2(ListNode head) 
        {
            if(head==null || head.next==null)
            {
                return true;
            }

            List<ListNode> a = new List<ListNode>();
            List<ListNode> b = new List<ListNode>();

            int num = 1;//链表长度

            ListNode  p = head;
            //计算链表长度
            while(p.next != null)
            {
                num++;
                p=p.next;
            }

            int midnum = num / 2;
            p = head;
            for(int i=0;i< midnum;i++)
            {
                a.Add(p);
                p=p.next;
            }
            //若是奇数长度则省略最中间的元素
            if(num % 2==1)
            {
                p=p.next;
            }

            for(int i=0;i<midnum;i++)
            {
                b.Add(p);
                p= p.next;
            }

            for(int i=0;i<midnum;i++)
            {
                int j=midnum-1 + i;
                if( j >= num)
                    return false;
                if(a[i].val != b[j].val)
                {
                    return false;
                } 
            }
            return true;
        }
    }
}


