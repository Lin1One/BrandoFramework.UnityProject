using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 114. 二叉树展开为链表
        // 给定一个二叉树，原地将它展开为链表。

        // 例如，给定二叉树

        //     1
        //    / \
        //   2   5
        //  / \   \
        // 3   4   6
        // 将其展开为：

        // 1
        //  \
        //   2
        //    \
        //     3
        //      \
        //       4
        //        \
        //         5
        //          \
        //           6


        // [1,2,5,3,4,null,6]
        // 输出
        // [1,null,2,null,3]
        // 预期结果
        // [1,null,2,null,3,null,4,null,5,null,6]
        public void Flatten(TreeNode root) 
        {
            if(root == null)
                return;
            Flatten(root.left);
            Flatten(root.right);
            var temp = root.right;
            root.right = root.left;
            root.left = null;
            while(root.right != null)
            {
                root = root.right;
            }
            root.right = temp;
        }

        TreeNode last = null;
        private void Flatten2(TreeNode root)
        {
            if (root == null) return;
            // 前序：注意更新last节点，包括更新左右子树
            if (last != null) 
            {
                last.left = null;
                last.right = root;
            }
            last = root;
            // 前序：注意备份右子节点，规避下一节点篡改
            TreeNode copyRight = root.right;
            Flatten2(root.left);
            Flatten2(copyRight);
        }

        public void Flatten3(TreeNode root) 
        {
            if (root == null)
                return;
            Flatten3(root.left);
            Flatten3(root.right);
            TreeNode tmp = root.right;
            root.right = root.left;
            root.left = null;
            while(root.right != null)
            {
                root = root.right;
            } 
            root.right = tmp;
        }
    }
}




