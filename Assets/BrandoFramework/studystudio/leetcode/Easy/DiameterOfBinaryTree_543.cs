using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 543. 二叉树的直径
        // 给定一棵二叉树，你需要计算它的直径长度。
        // 一棵二叉树的直径长度是任意两个结点路径长度中的最大值。这条路径可能穿过根结点。
//[4,-7,-3,null,null,-9,-3,9,-7,-4,null,6,null,-6,-6,null,null,0,6,5,null,9,null,null,-1,-4,null,null,null,-2]
        // 示例 :
        // 给定二叉树

        //           1
        //          / \
        //         2   3
        //        / \     
        //       4   5    
        // 返回 3, 它的长度是路径 [4,2,1,3] 或者 [5,2,1,3]。

        // 注意：两结点之间的路径长度是以它们之间边的数目表示。


        private int currentDiameter = 0;
        public int DiameterOfBinaryTree(TreeNode root) 
        {
            GetDiameter(root);
            return currentDiameter;
        }

        private int GetDiameter(TreeNode root)
        {
            if(root == null)
            { 
                return 0;
            }

            int leftDepth = GetDiameter(root.left);
            int rightDepth = GetDiameter(root.right);
            currentDiameter = Math.Max(currentDiameter,leftDepth + rightDepth);
            return Math.Max(leftDepth,rightDepth) + 1;
        }

            public int DiameterOfBinaryTree1(TreeNode root) {
        GetDepthAdd(root);
        return max;
    }
    int max=0;
    //遍历每一个节点,求出此节点作为根的树的深度,那么,左子树深度加右子树深度的最大值即是答案
    public int GetDepthAdd(TreeNode root)
    {
        if(root==null)
            return 0;
        int x=GetDepthAdd(root.left);
        int y=GetDepthAdd(root.right);
        if(x+y>max)//两结点最深长度相加，若没有这个条件，则为寻找根节点最深长度
            max=x+y;
        return Math.Max(x,y)+1;
    }
    }
}


