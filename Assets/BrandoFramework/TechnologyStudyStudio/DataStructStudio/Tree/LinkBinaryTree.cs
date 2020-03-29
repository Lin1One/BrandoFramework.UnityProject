using System;
using System.Collections.Generic;

namespace DataStructStudio
{
    public class LinkBinaryTree<T>
    {
        private TreeNode<T> rootNode;
        public TreeNode<T> Root
        {
            get { return rootNode; }
            set { rootNode = value; }
        }

        public LinkBinaryTree()
        {
            rootNode = null;
        }

        public LinkBinaryTree(T val)
        {
            TreeNode<T> p = new TreeNode<T>(val);
            rootNode = p;
        }

        public LinkBinaryTree(T val, TreeNode<T> lp, TreeNode<T> rp)
        {
            TreeNode<T> p = new TreeNode<T>(val, lp, rp);
            rootNode = p;
        }

        public bool isEmpty()
        {
            return rootNode == null;
        }

        //获取根结点
        public TreeNode<T> GetRoot()
        {
            return Root;
        }

        //获取结点的左孩子结点
        public static TreeNode<T> GetLChild(TreeNode<T> p)
        {
            return p.LeftChild;
        }

        public static TreeNode<T> GetRChild(TreeNode<T> p)
        {
            return p.RightChild;
        }

        //将结点p的左子树插入值为val的新结点，原来的左子树称为新结点的左子树
        public static void InsertToLeft(T val, TreeNode<T> p)
        {
            var oriLeft = p.LeftChild;
            var newLeft = new TreeNode<T>(val, oriLeft,null);
            p.LeftChild = newLeft;
        }

        //将结点p的左子树插入值为val的新结点，原来的左子树称为新结点的左子树
        public static void InsertToRight(T val, TreeNode<T> p)
        {
            var oriRight = p.RightChild;
            var newLeft = new TreeNode<T>(val, null, oriRight);
            p.RightChild = newLeft;
        }

        public static void DeleteLeftChild(TreeNode<T> p)
        {
            if(p == null || p.LeftChild == null )
            {
                return;
            }
            p.LeftChild = null;
        }

        public static void DeleteRightChild(TreeNode<T> p)
        {
            if (p == null || p.RightChild == null)
            {
                return;
            }
            p.RightChild = null;
        }

        /// <summary>
        /// 在二叉树中查找 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TreeNode<T> Search(TreeNode<T> root, T value)
        {
            if(root == null )
            {
                return root;
            }

            if(root.Valve.Equals(value))
            {
                return root;
            }

            var searchInLeft = Search(root.LeftChild, value);
            if(searchInLeft != null)
            {
                return searchInLeft;
            }

            var searchiInRight = Search(root.RightChild, value);
            if(searchiInRight != null)
            {
                return searchiInRight;
            }
            return null;
        }

        /// <summary>
        /// 判断是否为叶子节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsLeaf(TreeNode<T> node)
        {
            if ((node != null) && (node.RightChild == null) && (node.LeftChild == null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 前序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public static void PreOrder(TreeNode<T> root,Action<T> action)
        {
            if (root == null)
            {
                return;
            }
            action?.Invoke(root.Valve);
            PreOrder(root.LeftChild, action);
            PreOrder(root.RightChild, action);
        }

        /// <summary>
        /// 中序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public static void Inorder(TreeNode<T> root,Action<T> action)
        {
            if(root == null)
            {
                return;
            }
            Inorder(root.LeftChild, action);
            action?.Invoke(root.Valve);
            Inorder(root.RightChild, action);
        }

        /// <summary>
        /// 后序遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public static void PostOrder(TreeNode<T> root,Action<T> action)
        {
            if(root == null)
            {
                return;
            }
            PostOrder(root.LeftChild, action);
            PostOrder(root.RightChild, action);
            action?.Invoke(root.Valve);
        }

        /// <summary>
        /// 层次遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="action"></param>
        public static void LevelOrder(TreeNode<T> root, Action<T> action)
        {
            if (root == null)
            {
                return;
            }
            var nodeQueue = new Queue<TreeNode<T>>();
            nodeQueue.Enqueue(root);
            while(nodeQueue.Count > 0)
            {
                var node = nodeQueue.Dequeue();
                action(node.Valve);
                if (node.LeftChild != null)
                {
                    nodeQueue.Enqueue(node.LeftChild);
                }
                if(node.RightChild != null)
                {
                    nodeQueue.Enqueue(node.RightChild);
                }
            }
        }
    }
        
}


