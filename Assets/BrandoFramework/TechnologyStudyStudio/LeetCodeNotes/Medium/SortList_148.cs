using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 148.排序链表
        // 在 O(nlogn) 时间复杂度和常数级空间复杂度下，对链表进行排序。
        
        // 示例 1:
        // 输入: 4->2->1->3
        // 输出: 1->2->3->4
        
        // 示例 2:
        // 输入: -1->5->3->4->0
        // 输出: -1->0->3->4->5

        public ListNode SortList0321(ListNode head) 
        {
            if (head == null || head.next == null)
            {
                return head;
            }
            ListNode slowNode = head;
            ListNode fastNode = head.next;
            while(fastNode != null && fastNode.next != null)
            {
                slowNode = slowNode.next;
                fastNode = fastNode.next.next;
            }
            var temp = slowNode.next;
            slowNode.next = null;
            ListNode newLeftNode = SortList0321(head);
            ListNode newRightNode = SortList0321(temp);
            ListNode result = new ListNode(0);
            ListNode curNode = result;
            while (newLeftNode != null && newRightNode != null)
            {
                if(newLeftNode.val > newRightNode.val)
                {
                    curNode.next = newRightNode;
                    newRightNode = newRightNode.next;
                }
                else
                {
                    curNode.next = newLeftNode;
                    newLeftNode = newLeftNode.next;
                }
                curNode = curNode.next;
            }
            curNode.next = newLeftNode == null ? newRightNode : newLeftNode;
            return result.next;
        }


        // 解答一：归并排序（递归法）
        // 题目要求时间空间复杂度分别为O(nlogn)和O(1)，
        // 根据时间复杂度我们自然想到二分法，从而联想到归并排序；

        // 对数组做归并排序的空间复杂度为 O(n)
        // 分别由新开辟数组 O(n) 和递归函数调用O(logn)组成，而根据链表特性：
        // 数组额外空间：链表可以通过修改引用来更改节点顺序，
        //      无需像数组一样开辟额外空间；
        // 递归额外空间：递归调用函数将带来O(logn)的空间复杂度，
        //      因此若希望达到O(1)空间复杂度，则不能使用递归。

        // 通过递归实现链表归并排序，有以下两个环节：
        // 分割 cut 环节： 找到当前链表中点，并从中点将链表断开
        //  （以便在下次递归 cut 时，链表片段拥有正确边界）；
        // 我们使用 fast,slow 快慢双指针法，奇数个节点找到中点，偶数个节点找到中心左边的节点。
        //      找到中点 slow 后，执行 slow.next = None 将链表切断。
        // 递归分割时，输入当前链表左端点 head 和中心节点 slow 的下一个节点 tmp(因为链表是从 slow 切断的)。
        
        // cut 递归终止条件： 当head.next == None时，说明只有一个节点了，直接返回此节点。
        
        // 合并 merge 环节： 将两个排序链表合并，转化为一个排序链表。
        
        // 双指针法合并，建立辅助 ListNode h 作为头部。
        // 设置两指针 left, right 分别指向两链表头部，比较两指针处节点值大小，由小到大加入合并链表头部，
            // 指针交替前进，直至添加完两个链表。
        // 返回辅助ListNode h 作为头部的下个节点 h.next。
        // 时间复杂度 O(l + r)，l, r 分别代表两个链表长度。
        // 当题目输入的 head == None 时，直接返回None。
        public ListNode SortList1(ListNode head) 
        {
            if (head == null || head.next == null)
            {
                return head;
            }
                
            ListNode fast = head.next;
            ListNode slow = head;
            while (fast != null && fast.next != null) 
            {
                slow = slow.next;
                fast = fast.next.next;
            }
            //中点
            ListNode tmp = slow.next;
            //切断
            slow.next = null;
            ListNode left = SortList1(head);
            ListNode right = SortList1(tmp);

            //合并
            ListNode h = new ListNode(0);
            ListNode res = h;
            while (left != null && right != null) 
            {
                if (left.val < right.val) 
                {
                    h.next = left;
                    left = left.next;
                }
                else 
                {
                    h.next = right;
                    right = right.next;
                }
                h = h.next;
            }
            h.next = left != null ? left : right;
            return res.next;
        }

   
    

        // 对于非递归的归并排序，需要使用迭代的方式替换cut环节：
        // 我们知道，cut环节本质上是通过二分法得到链表最小节点单元，再通过多轮合并得到排序结果。
        // 每一轮合并merge操作针对的单元都有固定长度intv，例如：
        // 第一轮合并时intv = 1，即将整个链表切分为多个长度为1的单元，并按顺序两两排序合并，合并完成的已排序单元长度为2。
        // 第二轮合并时intv = 2，即将整个链表切分为多个长度为2的单元，并按顺序两两排序合并，合并完成已排序单元长度为4。
        // 以此类推，直到单元长度intv >= 链表长度，代表已经排序完成。
        // 根据以上推论，我们可以仅根据intv计算每个单元边界，并完成链表的每轮排序合并，例如:
        // 当intv = 1时，将链表第1和第2节点排序合并，第3和第4节点排序合并，……。
        // 当intv = 2时，将链表第1-2和第3-4节点排序合并，第5-6和第7-8节点排序合并，……。
        // 当intv = 4时，将链表第1-4和第5-8节点排序合并，第9-12和第13-16节点排序合并，……。
        // 此方法时间复杂度O(nlogn)，空间复杂度O(1)O(1)。

     
    }
}




