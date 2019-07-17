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

        public class TreeNode 
        {
            public int val;
            public TreeNode left;
            public TreeNode right;
            public TreeNode(int x) { val = x; }
         }
        public bool IsSymmetric(TreeNode root)
        {
            return true;
        }
    }
}

