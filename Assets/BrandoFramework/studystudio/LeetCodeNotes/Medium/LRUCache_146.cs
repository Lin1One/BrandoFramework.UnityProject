using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 146. LRU缓存机制
        // 运用你所掌握的数据结构，设计和实现一个  LRU (最近最少使用) 缓存机制。
        // 它应该支持以下操作： 获取数据 get 和 写入数据 put 。
        // 获取数据 get(key) - 如果密钥 (key) 存在于缓存中，则获取密钥的值（总是正数），否则返回 -1。
        // 写入数据 put(key, value) - 如果密钥不存在，则写入其数据值。
        //      当缓存容量达到上限时，它应该在写入新数据之前删除最近最少使用的数据值，从而为新的数据值留出空间。

        // 进阶:

        // 你是否可以在 O(1) 时间复杂度内完成这两种操作？

        // 示例:

        // LRUCache cache = new LRUCache( 2 /* 缓存容量 */ );

        // cache.put(1, 1);
        // cache.put(2, 2);
        // cache.get(1);       // 返回  1
        // cache.put(3, 3);    // 该操作会使得密钥 2 作废
        // cache.get(2);       // 返回 -1 (未找到)
        // cache.put(4, 4);    // 该操作会使得密钥 1 作废
        // cache.get(1);       // 返回 -1 (未找到)
        // cache.get(3);       // 返回  3
        // cache.get(4);       // 返回  4
        public class LRUCache 
        {
            //方法 2：哈希表 + 双向链表
            private int m_capacity = 0;
            private int count = 0; 
            private Dictionary<int,int> keyValue;
            private Dictionary<int,int> keyUseTime;
            public LRUCache(int capacity) 
            {
                m_capacity = capacity;
            }
            
            public int Get(int key) 
            {
                if(keyValue.ContainsKey(key))
                {
                    keyUseTime[key]++;
                    return keyValue[key];
                }
                return -1;
                
            }
            
            public void Put(int key, int value) 
            {
                if(count == m_capacity)
                {
                    
                }
            }
        }



        /**
        * Your LRUCache object will be instantiated and called as such:
        * LRUCache obj = new LRUCache(capacity);
        * int param_1 = obj.Get(key);
        * obj.Put(key,value);
        */

        public class LRUCache1 
        {
            class DLinkedNode 
            {
                public int key;
                public int value;
                public DLinkedNode prev;
                public DLinkedNode next;
            }

            private void addNode(DLinkedNode node) 
            {
                node.prev = head;
                node.next = head.next;
                head.next.prev = node;
                head.next = node;
            }

            private void removeNode(DLinkedNode node)
            {
                DLinkedNode prev = node.prev;
                DLinkedNode next = node.next;
                prev.next = next;
                next.prev = prev;
            }

            private void moveToHead(DLinkedNode node)
            {
                removeNode(node);
                addNode(node);
            }

            private DLinkedNode popTail() 
            {

                DLinkedNode res = tail.prev;
                removeNode(res);
                return res;
            }

            private Dictionary<int, DLinkedNode> cache =
                    new Dictionary<int, DLinkedNode>();
            private int size;
            private int capacity;
            private DLinkedNode head, tail;

            public LRUCache1(int capacity) 
            {
                this.size = 0;
                this.capacity = capacity;

                head = new DLinkedNode();
                // head.prev = null;

                tail = new DLinkedNode();
                // tail.next = null;

                head.next = tail;
                tail.prev = head;
            }

            public int get(int key)
             {
                DLinkedNode node = cache[key];
                if (node == null) return -1;

                // move the accessed node to the head;
                moveToHead(node);

                return node.value;
            }

            public void put(int key, int value) 
            {
                DLinkedNode node = cache[key];

                if(node == null) 
                {
                    DLinkedNode newNode = new DLinkedNode();
                    newNode.key = key;
                    newNode.value = value;

                    cache.Add(key, newNode);
                    addNode(newNode);

                    ++size;

                    if(size > capacity) 
                    {
                        // pop the tail
                        DLinkedNode tail = popTail();
                        cache.Remove(tail.key);
                        --size;
                    }
                } 
                else 
                {
                    // update the value.
                    node.value = value;
                    moveToHead(node);
                }
            }
        }

    }
}




