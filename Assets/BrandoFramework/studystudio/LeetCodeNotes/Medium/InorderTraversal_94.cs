using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 94. 二叉树的中序遍历
        // 给定一个二叉树，返回它的中序 遍历。
        
        // 示例:
        // 输入: [1,null,2,3]
        //    1
        //     \
        //      2
        //     /
        //    3
        // 输出: [1,3,2]
        //比如正常的一个满节点，
        //A：根节点、B：左节点、C：右节点，
        //前序顺序是ABC（根节点排最先，然后同级先左后右）；
        //中序顺序是BAC(先左后根最后右）；
        //后序顺序是BCA（先左后右最后根）。
        
        //中序
        public IList<int> InorderTraversal(TreeNode root) 
        {
            List<int> ans = new List<int>();
            Inorder(root,ans);
            return ans;
        }
        private void Inorder(TreeNode root,List<int> result)
        {
            if(root == null) return;
            Inorder(root.left,result);
            result.Add(root.val);
            Inorder(root.right,result); 
        }

        //前序遍历
        public IList<int> PreorderTraversal(TreeNode root) 
        {
            List<int> ans = new List<int>();
            Preorder(root,ans);
            return ans;
        }
        private void Preorder(TreeNode root,List<int> result)
        {
            if(root == null)return;
            result.Add(root.val);
            Preorder(root.left,result);
            Preorder(root.right,result); 
        }

        //后序遍历
        // 输入: [1,null,2,3]  
        //   1
        //    \
        //     2
        //    /
        //   3 

        // 输出: [3,2,1]
        public IList<int> PostorderTraversal(TreeNode root) 
        {
            List<int> ans = new List<int>();
            Postorder(root,ans);
            return ans;
        }   
        private void Postorder(TreeNode root,List<int> result)
        {
            if(root == null) return;
            Postorder(root.left,result);
            Postorder(root.right,result); 
            result.Add(root.val);
        }

    }
}




