using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //面试题06.从尾到头打印链表
        //输入一个链表的头节点，从尾到头反过来返回每个节点的值（用数组返回）。

        //示例 1：

        //输入：head = [1,3,2]
        //输出：[2,3,1]

        //限制：

        //0 <= 链表长度 <= 10000
        public int[] ReversePrint1(ListNode head)
        {
            var result = new List<int>();
            PushListVal(head, result);
            return result.ToArray();
        }

        private void PushListVal(ListNode head ,List<int> helpList)
        {
            if(head == null)
            {
                return;
            }
            PushListVal(head.next, helpList);
            helpList.Add(head.val);
        }

        public int[] ReversePrint2(ListNode head)
        {
            var result = new List<int>();
            while(head != null)
            {
                result.Add(head.val);
                head = head.next;
            }

            for(var i = 0;i< result.Count/2;i++)
            {
                var temp = result[i];
                result[i] = result[result.Count - i - 1];
                result[result.Count - i - 1] = temp;
            }
            return result.ToArray();
        }
    }
}
