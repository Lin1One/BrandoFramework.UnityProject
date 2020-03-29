using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructStudio
{
    /// <summary>
    /// 二叉搜索树：结点的左子节点的值永远小于该结点的值，而右子结点的值永远大于该结点的值 称为二叉搜索树
    /// </summary>
    public class LinkBinarySearchTree : LinkBinaryTree<int>
    {
        #region 插入

        /// <summary>
        /// 递归插入
        /// </summary>
        public void InsertRecursively(int element)
        {
            Root = InsertRecursively(Root,new TreeNode<int>(element));
        }

        private TreeNode<int> InsertRecursively(TreeNode<int> rootNode ,TreeNode<int> newNode)
        {
            if(rootNode == null)
            {
                return newNode;
            }
            if(newNode.Valve < rootNode.Valve)
            {
                rootNode.LeftChild = InsertRecursively(rootNode.LeftChild, newNode);
            }
            else
            {
                rootNode.RightChild = InsertRecursively(rootNode.RightChild, newNode);
            }
            return rootNode;
        }

        /// <summary>
        /// 遍历插入
        /// </summary>
        public void InsetTraversal(int element)
        {
            InsertTraversal(new TreeNode<int>(element));
        }

        private void InsertTraversal(TreeNode<int> elementNode)
        {
            if(Root == null)
            {
                Root = elementNode;
                return;
            }
            TreeNode<int> parentNode;
            var curNode = Root;
            while(true)
            {
                parentNode = curNode;
                if (elementNode.Valve < curNode.Valve)
                {
                    curNode = curNode.LeftChild;
                    if (curNode == null)
                    {
                        parentNode.LeftChild = elementNode;
                        break;
                    }
                }
                else
                {
                    curNode = curNode.RightChild;
                    if(curNode == null)
                    {
                        parentNode.RightChild = elementNode;
                        break;
                    }
                }
            }
        }

        #endregion

        #region 删除

        /// <summary>
        /// 递归删除
        /// </summary>
        /// <param name="element"></param>
        public void RemoveRecursively(int element)
        {
            Root = RemoveRecursively(Root, element);
        }

        private TreeNode<int> RemoveRecursively(TreeNode<int> curNode, int element)
        {
            if(curNode == null)
            {
                return null;
            }
            if(element < curNode.Valve)
            {
                curNode.LeftChild = RemoveRecursively(curNode.LeftChild, element);
                return curNode;
            }
            else if(element > curNode.Valve)
            {
                curNode.RightChild = RemoveRecursively(curNode.RightChild, element);
                return curNode;
            }
            else
            {
                if(curNode.LeftChild == null)
                {
                    var rightNode = curNode.RightChild;
                    curNode.RightChild = null;
                    return rightNode;
                }

                if(curNode.RightChild == null)
                {
                    var leftNode = curNode.LeftChild;
                    curNode.LeftChild = null;
                    return leftNode;
                }

                var newNode = GetMaxNode(curNode.LeftChild);
                newNode.LeftChild = RemoveMaxNode(curNode.LeftChild);
                newNode.RightChild = curNode.RightChild;
                curNode.LeftChild = curNode.RightChild = null;
                return newNode;
            }
        }

        /// <summary>
        /// 二叉树的删除
        /// 被删节点是叶子节点
        /// 被删节点有左孩子没右孩子
        /// 被删节点有右孩子没左孩子
        /// 被删节点有两个孩子
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public TreeNode<int> Remove(int element)
        {
            var parent = Root;
            var current = Root;
            if(Root == null)
            {
                return null;
            }
            //首先找到需要被删除的节点&其父节点
            while (current.Valve != element)
            {
                if (element < current.Valve)
                {
                    if (current.LeftChild == null)
                    {
                        break;
                    }
                    parent = current;
                    current = current.LeftChild;
                }
                else if (element > current.Valve)
                {
                    if (current.RightChild == null)
                    {
                        break;
                    }
                    parent = current;
                    current = current.RightChild;
                }
            }

            if(current.Valve != element)
            {
                return null;
            }
            //找到被删除节点后，分四种情况进行处理
            //情况一，所删节点是叶子节点时，直接删除即可
            if(current.LeftChild == null && current.RightChild == null)
            {
                if(current == Root)
                {
                    Root = null;
                }
                else if(current.Valve > element)
                {
                    parent.LeftChild = null;
                }
                else
                {
                    parent.RightChild = null;
                }
            }
            //情况二，所删节点只有左孩子节点时
            else if (current.LeftChild != null && current.RightChild == null)
            {
                if(element < parent.Valve)
                {
                    parent.LeftChild = current.LeftChild;
                }
                else
                {
                    parent.RightChild = current.LeftChild;
                }
            }
            //情况三，所删节点只有右孩子节点时
            else if(current.LeftChild == null && current.RightChild != null)
            {
                if (element < parent.Valve)
                {
                    parent.LeftChild = current.RightChild;
                }
                else
                {
                    parent.RightChild = current.RightChild;
                }
            }
            //情况四，所删节点有左右两个孩子
            else
            {
                //current是被删的节点，temp是被删左子树最右边的节点
                TreeNode<int> temp;
                
                temp = current.LeftChild;
                while (temp.RightChild != null)
                {
                    temp = temp.RightChild;
                }
                temp.RightChild = current.LeftChild;
                if (current.Valve < parent.Valve)
                {
                    parent.LeftChild = temp;
                }
                else if(current.Valve > parent.Valve)
                {
                    parent.RightChild = temp;
                }
                else
                /// current 和 parent 相同，即为根节点
                {
                    Root = temp;
                }
            }
            return current;
        }

        #endregion

        #region 查找

        public TreeNode<int> SearchRecursively(TreeNode<int> curNode,int element)
        {
            if(curNode == null)
            {
                return null;
            }
            if(element < curNode.Valve)
            {
                return SearchRecursively(curNode.LeftChild, element);
            }
            else if(element > curNode.Valve)
            {
                return SearchRecursively(curNode.RightChild, element);
            }
            else
            {
                return curNode;
            }
        }

        public TreeNode<int> Search(int element)
        {
            TreeNode<int> curNode = Root;
            while(curNode != null)
            {
                if(element > curNode.Valve )
                {
                    curNode = curNode.RightChild;
                }
                else if(element < curNode.Valve)
                {
                    curNode = curNode.LeftChild;
                }
                else
                {
                    return curNode;
                }
            }
            return null;
        }

        #endregion

        public TreeNode<int> GetMinNode(TreeNode<int> rootNode)
        {
            var curNode = rootNode;
            while(curNode.LeftChild != null)
            {
                curNode = curNode.LeftChild;
            }
            return curNode;
        }

        public TreeNode<int> GetMaxNode(TreeNode<int> rootNode)
        {
            var curNode = rootNode;
            while (curNode.RightChild != null)
            {
                curNode = curNode.RightChild;
            }
            return curNode;
        }

        public TreeNode<int> RemoveMaxNode(TreeNode<int> rootNode)
        {
            var curNode = rootNode;
            TreeNode<int> parentNode = null;
            while (curNode.RightChild != null)
            {
                parentNode = curNode;
                curNode = curNode.RightChild;
            }
            parentNode.RightChild = null;
            return rootNode;

        }

    }
}
