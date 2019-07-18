#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
    //给定一个二叉树，找出其最大深度。
    // 二叉树的深度为根节点到最远叶子节点的最长路径上的节点数。
    // 说明: 叶子节点是指没有子节点的节点。
    // 示例：
    // 给定二叉树 [3,9,20,null,null,15,7]，
    //     3
    //    / \
    //   9  20
    //     /  \
    //    15   7
    // 返回它的最大深度 3 。

        public int MaxDepth(TreeNode root)
        {
            if(root == null)
            {
                return 0;
            }
            else
            {
                int leftHeight = MaxDepth(root.left);
                int rightHeight = MaxDepth(root.right);
                return (leftHeight > rightHeight ? leftHeight : rightHeight) + 1;
            }
        }

        private bool CheckHaveChildNode(TreeNode root,ref int depth)
        {
            if(root == null)
            {
                return false;
            }
            if(root.left != null|| root.right != null)
            {
                depth++;
            }
            if(CheckHaveChildNode(root.left,ref depth)||CheckHaveChildNode(root.right,ref depth))
            {
                return true;
            }
            else
            {
                return false;
            } 
        }

        //递归,直观的方法是通过递归来解决问题。DFS（深度优先搜索）
        //时间复杂度：我们每个结点只访问一次，因此时间复杂度为 O(N)， 其中 N 是结点的数量。
        //空间复杂度：在最糟糕的情况下，树是完全不平衡的，例如每个结点只剩下左子结点，递归将会被调用 N 次（树的高度），
        //因此保持调用栈的存储将是 O(N)。但在最好的情况下（树是完全平衡的），树的高度将是 log(N)log(N)。因此，在这种情况下的空间复杂度将是 O(\log(N))。
        public int maxDepth1(TreeNode root) 
        {
            if (root == null)
            {
                return 0;
            }
            else 
            {
                int left_height = maxDepth1(root.left);
                int right_height = maxDepth1(root.right);
                return System.Math.Max(left_height, right_height) + 1;
            }
        }

        //迭代
        //使用 DFS 策略访问每个结点，同时在每次访问时更新最大深度。
        //所以我们从包含根结点且相应深度为 1 的栈开始。然后我们继续迭代：
        //将当前结点弹出栈并推入子结点。每一步都会更新深度。
        public int maxDepth2(TreeNode root) 
        {
            Stack<KeyValuePair<TreeNode, int>> stack = new Stack<KeyValuePair<TreeNode, int>>();
            if (root != null) 
            {
                stack.Push(new KeyValuePair<TreeNode, int>(root, 1));
            }

            int depth = 0;
            while (stack.Count != 0) 
            {
                KeyValuePair<TreeNode, int> current = stack.Pop();
                root = current.Key;
                int current_depth = current.Value;
                if (root != null) 
                {
                    depth = Math.Max(depth, current_depth);
                    stack.Push(new KeyValuePair<TreeNode, int>(root.left, current_depth + 1));
                    stack.Push(new KeyValuePair<TreeNode, int>(root.right, current_depth + 1));
                }
            }
            return depth;
        }
    }

}

