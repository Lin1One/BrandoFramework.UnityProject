using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //103. 二叉树的锯齿形层次遍历
        //给定一个二叉树，返回其节点值的锯齿形层次遍历。（即先从左往右，再从右往左进行下一层遍历，以此类推，层与层之间交替进行）。

        //例如：
        //给定二叉树[3, 9, 20, null, null, 15, 7],

        //    3
        //   / \
        //  9  20
        //    /  \
        //   15   7
        //返回锯齿形层次遍历如下：

        //[
        //  [3],
        //  [20,9],
        //  [15,7]
        //]
        public IList<IList<int>> ZigzagLevelOrder(TreeNode root)
        {
            IList<IList<int>> result = new List<IList<int>>();
            if (root == null)
            {
                return result;
            }
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(root);
            bool isReverse = true;
            while (queue.Count > 0)
            {
                int nodeCount = queue.Count;
                List<int> temp = new List<int>();
                for (var i = 0; i < nodeCount; i++)
                {
                    var curNode = queue.Dequeue();
                    if (curNode.left != null)
                    {
                        queue.Enqueue(curNode.left);
                    }
                    if (curNode.right != null)
                    {
                        queue.Enqueue(curNode.right);
                    }
                    if (isReverse)
                    {
                        temp.Add(curNode.val);
                    }
                    else
                    {
                        temp.Insert(0, curNode.val);
                    }
                }
                isReverse = !isReverse;
                result.Add(temp);
            }
            return result;
        }

    }
}




