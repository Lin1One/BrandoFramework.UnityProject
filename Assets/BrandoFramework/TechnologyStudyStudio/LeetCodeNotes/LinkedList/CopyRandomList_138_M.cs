using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Study.LeetCode
{
    public partial class Solution
    {

        public class RandomListNode
        {
            public int val;
            public RandomListNode next;
            public RandomListNode random;

            public RandomListNode(int _val)
            {
                val = _val;
                next = null;
                random = null;
            }
        }
        //138. 复制带随机指针的链表
        //给定一个链表，每个节点包含一个额外增加的随机指针，该指针可以指向链表中的任何节点或空节点。
        //要求返回这个链表的 深拷贝。 
        //我们用一个由 n 个节点组成的链表来表示输入/输出中的链表。每个节点用一个[val, random_index] 表示：
        //val：一个表示 Node.val 的整数。
        //random_index：随机指针指向的节点索引（范围从 0 到 n-1）；如果不指向任何节点，则为  null 。
        public RandomListNode CopyRandomList(RandomListNode head)
        {
            if(head == null)
            {
                return null;
            }
            Dictionary<RandomListNode, RandomListNode> originNodeToNewNode = new Dictionary<RandomListNode, RandomListNode>();
            RandomListNode newHead = new RandomListNode(head.val);
            originNodeToNewNode.Add(head, newHead);
            RandomListNode curNewNode = newHead;
            RandomListNode curNode = head;
            while(curNode.next != null)
            {
                RandomListNode newNext = new RandomListNode(curNode.next.val);
                curNewNode.next = newNext;
                curNewNode = curNewNode.next;
                originNodeToNewNode.Add(curNode.next, newNext);
                curNode = curNode.next;
            }
            curNewNode.next = null;
            curNewNode = newHead;
            while (curNewNode != null)
            {
                if(head.random != null)
                {
                    RandomListNode randomNode;
                    if(originNodeToNewNode.TryGetValue(head.random,out randomNode))
                    {
                        curNewNode.random = randomNode;
                    }
                }
                curNewNode = curNewNode.next;
                head = head.next;
            }
            return newHead;
        }

        public RandomListNode CopyRandomList1(RandomListNode head)
        {
            var nodeCache = new Dictionary<RandomListNode, RandomListNode>();
            return CopyRandomListNode(head, nodeCache);
        }

        private RandomListNode CopyRandomListNode(RandomListNode node,Dictionary<RandomListNode, RandomListNode> cache)
        {
            if(node == null)
            {
                return null;
            }
            RandomListNode newNode;
            if(cache.TryGetValue(node,out newNode))
            {
                return newNode;
            }
            newNode = new RandomListNode(node.val);
            cache.Add(node, newNode);
            newNode.next = CopyRandomListNode(node.next, cache);
            newNode.random = CopyRandomListNode(node.random, cache);
            return newNode;
        }

        public RandomListNode CopyRandomList2(RandomListNode head)
        {
            if (head == null)
            {
                return null;
            }
            RandomListNode curNode = head;
            while(curNode != null)
            {
                RandomListNode newNode = new RandomListNode(curNode.val);
                newNode.next = curNode.next;
                curNode.next = newNode;
                curNode = newNode.next;
            }
            curNode = head;
            RandomListNode curNewNode;
            while (curNode != null)
            {
                curNewNode = curNode.next;
                curNewNode.random = curNode.random != null ? curNode.random.next : null;
                curNode = curNewNode.next;
            }
            RandomListNode newHead;
            curNode = head;
            newHead = head.next;
            curNewNode = newHead;
            while(curNode != null)
            {
                curNode.next = curNewNode.next;
                curNewNode.next = curNode.next != null ? curNode.next.next : null;
                curNode = curNode.next;
                curNewNode = curNewNode.next;
            }
            return newHead;
        }
    }
}
