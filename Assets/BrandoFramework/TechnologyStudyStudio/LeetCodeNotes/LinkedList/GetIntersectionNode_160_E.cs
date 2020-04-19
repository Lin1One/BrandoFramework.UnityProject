#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 编写一个程序，找到两个单链表相交的起始节点。
        // 如下面的两个链表：
        // 在节点 c1 开始相交。
        
        // 示例 1：
        // 输入：intersectVal = 8, listA = [4,1,8,4,5], listB = [5,0,1,8,4,5], 
        // skipA = 2, skipB = 3
        // 输出：Reference of the node with value = 8
        // 输入解释：相交节点的值为 8 （注意，如果两个列表相交则不能为 0）。
        //从各自的表头开始算起，链表 A 为 [4,1,8,4,5]，链表 B 为 [5,0,1,8,4,5]。
        //在 A 中，相交节点前有 2 个节点；在 B 中，相交节点前有 3 个节点。

        // public class ListNode 
        // {
        //     public int val;
        //     public ListNode next;
        //     public ListNode(int x) { val = x; }
        // }
        


        public ListNode GetIntersectionNode(ListNode headA, ListNode headB) 
        {
            //双指针法：
            //创建两个指针 pA 和 pB，分别初始化为链表 A 和 B 的头结点。
            //然后让它们向后逐结点遍历。
            // 当 pA 到达链表的尾部时，将它重定位到链表 B 的头结点 (你没看错，就是链表 B); 
            // 类似的，当 pB 到达链表的尾部时，将它重定位到链表 A 的头结点。
            // 若在某一时刻 pApA 和 pBpB 相遇，则 pApA/pBpB 为相交结点。
            // 想弄清楚为什么这样可行, 可以考虑以下两个链表: A={1,3,5,7,9,11} 和 B={2,4,9,11}，
            //相交于结点 9。 由于 B.length (=4) < A.length (=6)，pB 比 pA 少经过 2 个结点，
            // 会先到达尾部。将 pB 重定向到 A 的头结点，pA 重定向到 B 的头结点后，pB 要比 pA 多走 2 个结点。
            // 因此，它们会同时到达交点。
            // 如果两个链表存在相交，它们末尾的结点必然相同。因此当 pApA/pBpB 到达链表结尾时，
            //记录下链表 A/B 对应的元素。若最后元素不相同，则两个链表不相交。

            if (headA == null|| headB == null)
            {
                return null;
            }
            ListNode firstListNode = headA;
            ListNode secondListNode = headB;
            int firstListFinalValue = 0;
            int secondListFinalValue = 0;
            bool firstListEnd = false;
            bool secondListEnd = false;
            while(firstListNode != secondListNode)
            { 

                if(firstListNode.next == null)
                {
                    firstListFinalValue = firstListNode.val;
                    firstListNode = headB;
                    firstListEnd = true;
                }
                else
                {
                    firstListNode = firstListNode.next;
                }

                if(secondListNode.next == null)
                {
                    secondListFinalValue = secondListNode.val;
                    secondListNode = headA;
                    secondListEnd = true;
                }
                else
                {
                    secondListNode = secondListNode.next;
                }
                if(firstListEnd && secondListEnd)
                {
                    if(firstListFinalValue != secondListFinalValue)
                    return null;
                }

            }
            return firstListNode;
        }

        // 遍历链表 A 并将每个结点的地址/引用存储在哈希表中。然后检查链表 B 中的每一个结点 
        //bi是否在哈希表中。若在，则 bi为相交结点。

        // 复杂度分析
        // 时间复杂度 : O(m+n)O(m+n)。
        // 空间复杂度 : O(m)O(m) 或 O(n)O(n)。 
    }
}

