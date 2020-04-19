#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 92. 反转链表 II
        // 反转从位置 m 到 n 的链表。请使用一趟扫描完成反转。

        // 说明:
        // 1 ≤ m ≤ n ≤ 链表长度。

        // 示例:
        // 输入: 1->2->3->4->5->NULL, m = 2, n = 4
        // 输出: 1->4->3->2->5->NULL


        public ListNode ReverseBetween(ListNode head, int m, int n) 
        {
            if(m == n || head == null)
            {
                return head;
            }
            ListNode temp = head;
            ListNode beforeStartNode = head;
            int reverseCount = n - m;
            int toStartReverse = m - 1;
            while(temp != null)
            {
                if(toStartReverse > 0)
                {
                    if(toStartReverse == 1)
                    {
                        beforeStartNode = temp;
                    }
                    toStartReverse--;
                }
                else
                {
                    if(m == 1)
                    {
                        return ReverseChildList(head,reverseCount);
                    }
                    beforeStartNode.next = ReverseChildList(temp,reverseCount);
                    break;
                }
                temp = temp.next;
            }
            
            return head;
        }

        private ListNode ReverseChildList(ListNode head,int reverseCount)
        {
            ListNode tempHeadNode = new ListNode(0);
            tempHeadNode.next = head;
            while(reverseCount > 0)
            {
                var temp = head.next;
                head.next = temp.next;
                temp.next = tempHeadNode.next;
                tempHeadNode.next = temp;
                reverseCount--;
            }
            return tempHeadNode.next;
        }
    }
}


