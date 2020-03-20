using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 617. 合并二叉树
        // 给定两个二叉树，想象当你将它们中的一个覆盖到另一个上时，两个二叉树的一些节点便会重叠。
        // 你需要将他们合并为一个新的二叉树。
        // 合并的规则是如果两个节点重叠，
        // 那么将他们的值相加作为节点合并后的新值，否则不为 NULL 的节点将直接作为新二叉树的节点。

        // 示例 1:

        // 输入: 
        //     Tree 1                     Tree 2                  
        //          1                         2                             
        //         / \                       / \                            
        //        3   2                     1   3                        
        //       /                           \   \                      
        //      5                             4   7                  
        // 输出: 
        // 合并后的树:
        //          3
        //         / \
        //        4   5
        //       / \   \ 
        //      5      


        public TreeNode MergeTrees(TreeNode t1, TreeNode t2) 
        {
            var ans = TreeDepth(t1,t2);
            return ans;
        }

    
        private TreeNode TreeDepth(TreeNode t1, TreeNode t2)
        {
            if(t1 == null )
            {
                return t2;
            }
            if(t2 == null)
            {
                return t1;
            }
            t1.val += t2.val;
            t1.left = TreeDepth(t1.left,t2.left); 
            t1.right = TreeDepth(t1.right,t2.right); 
            return t1;
        }


        public TreeNode MergeTrees2(TreeNode t1, TreeNode t2) 
        {
            if (t1 == null)
            {
                return t2;
            }
            if (t2 == null)
            {
                return t1;
            }

            TreeNode treeNode = new TreeNode(t1.val+t2.val);//计算节点值之和
            treeNode.left = MergeTrees(t1.left,t2.left);//计算右节点值之和
            treeNode.right = MergeTrees(t1.right, t2.right);//计算左节点之和
            return treeNode;
    }

    }
}


