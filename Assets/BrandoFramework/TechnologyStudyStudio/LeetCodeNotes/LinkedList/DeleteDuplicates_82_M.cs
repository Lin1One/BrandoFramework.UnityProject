#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 82. 删除排序链表中的重复元素 II
        // 给定一个排序链表，删除所有含有重复数字的节点，只保留原始链表中 没有重复出现 的数字。

        // 示例 1:
        // 输入: 1->2->3->3->4->5
        // 输出: 1->2->5

        // 示例 2:
        // 输入: 1->1->1->2->3
        // 输出: 2->3

        public ListNode DeleteDuplicates_2(ListNode head) 
        {
            ListNode newHeadNode = new ListNode(0);
            newHeadNode.next = head;
            ListNode slowNode = newHeadNode;
            ListNode fastNode = newHeadNode.next;
            slowNode.next = head;
            while(fastNode != null)
            {
                while(fastNode.next != null && fastNode.val == fastNode.next.val)
                {
                    //移动至相同值节点的最后一个
                    fastNode = fastNode.next; 
                }
                //判断是否遇到了相同值节点
                if(slowNode.next == fastNode)
                {
                    slowNode = slowNode.next;
                }
                else
                {
                    //慢指针指向跳过了相同值节点的快指针
                    slowNode.next = fastNode.next;
                }
                fastNode = fastNode.next; 
            }
            return newHeadNode.next;
        }
    }
}


