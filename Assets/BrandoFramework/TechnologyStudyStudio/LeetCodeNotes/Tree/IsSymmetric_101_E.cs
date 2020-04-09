#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个二叉树，检查它是否是镜像对称的。
        // 例如，二叉树 [1,2,2,3,4,4,3] 是对称的。
        //     1
        //    / \
        //   2   2
        //  / \ / \
        // 3  4 4  3
        // 但是下面这个 [1,2,2,null,3,null,3] 则不是镜像对称的:

        //     1
        //    / \
        //   2   2
        //    \   \
        //    3    3

        public bool IsSymmetric1(TreeNode leftNode, TreeNode rightNode)
        {
            if(leftNode == null && rightNode == null)
            {
                return true;
            }
            if (leftNode == null || rightNode == null)
            {
                return false;
            }
            if(leftNode.val != rightNode.val)
            {
                return false;
            }
            return IsSymmetric1(leftNode.left, rightNode.right) && IsSymmetric1(leftNode.right, rightNode.left);
        }

        public bool IsSymmetric(TreeNode root)
        {
            //1.时间复杂度：O(n)，因为我们遍历整个输入树一次，
            //所以总的运行时间为 O(n)，其中 n 是树中结点的总数。
            //2.空间复杂度：递归调用的次数受树的高度限制。
            //在最糟糕情况下，树是线性的，其高度为 O(n)
            //因此，在最糟糕的情况下，由栈上的递归调用造成的空间复杂度为 O(n)。

            if (root == null) return true;
            return IsTreeSymmertric(root.left,root.right);
        }
        private bool IsTreeSymmertric(TreeNode leftNode,TreeNode rightNode)
        {

            if(leftNode == null && rightNode == null)
            {
                return true;
            }
            if(leftNode == null || rightNode == null)
            {
                return false;
            }
            return (leftNode.val == rightNode.val) &&
                IsTreeSymmertric(leftNode.right, rightNode.left)   &&
                IsTreeSymmertric(leftNode.left, rightNode.right);
        }


        public bool isSymmetric2(TreeNode root) 
        {
            //我们也可以利用队列进行迭代。
            //队列中每两个连续的结点应该是相等的，而且它们的子树互为镜像。
            //最初，队列中包含的是 root 以及 root。
            //该算法的工作原理类似于 BFS，但存在一些关键差异。
            //每次提取两个结点并比较它们的值。
            //然后，将两个结点的左右子结点按相反的顺序插入队列中。
            //当队列为空时，或者我们检测到树不对称（即从队列中取出两个不相等的连续结点）时，该算法结束。
            Queue<TreeNode> q = new Queue<TreeNode>();
            q.Enqueue(root);
            q.Enqueue(root);
            while (q.Count != 0) {
                TreeNode t1 = q.Dequeue();
                TreeNode t2 = q.Dequeue();
                if (t1 == null && t2 == null) continue;
                if (t1 == null || t2 == null) return false;
                if (t1.val != t2.val) return false;
                q.Enqueue(t1.left);
                q.Enqueue(t2.right);
                q.Enqueue(t1.right);
                q.Enqueue(t2.left);
            }
            return true;
}


    }

}

