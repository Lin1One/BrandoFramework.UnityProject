using System;
using System.Collections.Generic;

namespace DataStructStudio
{
    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T>
    {
        protected T value;
        protected TreeNode<T> leftChild;
        protected TreeNode<T> rightChild;

        public TreeNode(T val, TreeNode<T> left, TreeNode<T> right)
        {
            value = val;
            leftChild = left;
            rightChild = right;
        }

        public TreeNode(TreeNode<T> left, TreeNode<T> right)
        {
            value = default(T);
            leftChild = left;
            rightChild = right;
        }

        public TreeNode(T val)
        {
            value = val;
        }

        public TreeNode()
        {
            value = default(T);
        }

        public T Valve
        {
             get { return value; }
             set { this.value = value; }
        }
 
         public TreeNode<T> LeftChild
         {
             get { return leftChild; }
             set { leftChild = value; }
         }
 
         public TreeNode<T> RightChild
         {
             get { return rightChild; }
             set { rightChild = value; }
         }
    }    
}


