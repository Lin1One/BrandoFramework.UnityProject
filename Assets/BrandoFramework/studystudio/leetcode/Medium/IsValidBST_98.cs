using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //  98. 验证二叉搜索树
        // 给定一个二叉树，判断其是否是一个有效的二叉搜索树。
        // 假设一个二叉搜索树具有如下特征：
        // 节点的左子树只包含小于当前节点的数。
        // 节点的右子树只包含大于当前节点的数。
        // 所有左子树和右子树自身必须也是二叉搜索树。

        // 示例 1:
        // 输入:
        //     2
        //    / \
        //   1   3
        // 输出: true
        // 示例 2:

        // 输入:
        //     5
        //    / \
        //   1   4
        //      / \
        //     3   6
        // 输出: false
        // 解释: 输入为: [5,1,4,null,null,3,6]。
        //      根节点的值为 5 ，但是其右子节点值为 4 。
        
        //输入:
        //     2
        //    / \
        //   1   3
        // 输出: true
        //[2,1,3]
        public bool IsValidBST(TreeNode root) 
        {
            bool result = false;
            if(root == null)
            {
                return true;
            }
            result = IsValid(root.left,root.val,int.MaxValue,true)
                &&  IsValid(root.right,root.val,root.val,false);
            return result;
        }

        private bool IsValid(TreeNode root,
            int parentNodeValue,
            int maxValue,
            bool isLeftNode)
        {
            bool result = false;
            if(root == null)
            {
                return true;
            }
            result = IsValid(root.left,root.val,parentNodeValue,true);
            if(!result) return result;

            result = isLeftNode ? root.val < parentNodeValue :
                     root.val > parentNodeValue && root.val < maxValue;
            if(!result) return result;

            result = IsValid(root.right,root.val,parentNodeValue,false);
            return result;
        }


        public bool IsValid1(TreeNode node, int lower, int upper) 
        {
            if (node == null) return true;

            int val = node.val;
            if (val <= lower) return false;
            if (val >= upper) return false;

            if (!IsValid1(node.right, val, upper)) return false;
            if (!IsValid1(node.left, lower, val)) return false;
            return true;
        }

        //递归
        public bool IsValidBST1(TreeNode root)
        {
            return IsValid1(root.left, int.MinValue, root.val)&&IsValid1(root.right, root.val, int.MaxValue);
        }


//         //迭代
//         LinkedList<TreeNode> stack = new LinkedList();
//   LinkedList<Integer> uppers = new LinkedList(),
//           lowers = new LinkedList();

//   public void update(TreeNode root, Integer lower, Integer upper) {
//     stack.add(root);
//     lowers.add(lower);
//     uppers.add(upper);
//   }

//   public boolean isValidBST(TreeNode root) {
//     Integer lower = null, upper = null, val;
//     update(root, lower, upper);

//     while (!stack.isEmpty()) {
//       root = stack.poll();
//       lower = lowers.poll();
//       upper = uppers.poll();

//       if (root == null) continue;
//       val = root.val;
//       if (lower != null && val <= lower) return false;
//       if (upper != null && val >= upper) return false;
//       update(root.right, val, upper);
//       update(root.left, lower, val);
//     }
//     return true;
//   }



        public bool IsValidBST2(TreeNode root) 
        {
            Stack<TreeNode> stack = new Stack<TreeNode>();
            double inorder = double.MinValue;

            while (stack.Count!= 0 || root != null) 
            {
                while (root != null) 
                {
                    stack.Push(root);
                    root = root.left;
                }
                root = stack.Pop();
            // If next element in inorder traversal
            // is smaller than the previous one
            // that's not BST.
                if (root.val <= inorder) 
                {
                    return false;
                }
                inorder = root.val;
                root = root.right;
            }
            return true;
        }



    }
}




