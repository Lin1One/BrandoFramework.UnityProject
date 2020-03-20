#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 86. 分隔链表
        // 给定一个链表和一个特定值 x，对链表进行分隔，
        // 使得所有小于 x 的节点都在大于或等于 x 的节点之前。
        // 你应当保留两个分区中每个节点的初始相对位置。

        // 示例:

        // 输入: head = 1->4->3->2->5->2, x = 3
        // 输出: 1->2->2->4->3->5

        public ListNode Partition(ListNode head, int x) 
        {
            if( head == null || head.next == null)
            {
                return head;
            }
            ListNode newListHead = new ListNode(0);
            newListHead.next = head;
            ListNode beforePivotNode = newListHead;
            while(beforePivotNode != null)
            {
                if(beforePivotNode.next != null && beforePivotNode.next.val >= x)
                {
                    break;
                }
                beforePivotNode = beforePivotNode.next;
            }
            if(beforePivotNode == null)
            {
                return head;
            }
            ListNode PivotNode = beforePivotNode.next;
            ListNode afterPivotNode = PivotNode;
            while(afterPivotNode != null)
            {
                if(afterPivotNode.next != null && afterPivotNode.next.val < x)
                {
                    var temp = afterPivotNode.next.next;
                    beforePivotNode.next = afterPivotNode.next;
                    afterPivotNode.next = temp;
                    beforePivotNode = beforePivotNode.next;
                    beforePivotNode.next = PivotNode;
                }
                else
                {
                    afterPivotNode = afterPivotNode.next;
                }
            }
            return newListHead.next;
        }

        public ListNode Partition2(ListNode head, int x) 
        {
            ListNode before_head = new ListNode(0);
            ListNode before = before_head;
            ListNode after_head = new ListNode(0);
            ListNode after = after_head;

            while (head != null) 
            {
                if (head.val < x) 
                {
                    before.next = head;
                    before = before.next;
                } 
                else 
                {
                    after.next = head;
                    after = after.next;
                }
                head = head.next;
            }
            after.next = null;
            before.next = after_head.next;
            return before_head.next;
        }
    }
}


