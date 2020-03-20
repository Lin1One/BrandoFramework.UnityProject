using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //96. 不同的二叉搜索树
        // 给定一个整数 n，求以 1 ... n 为节点组成的二叉搜索树有多少种？
        // 示例:
        // 输入: 3
        // 输出: 5
        // 解释:
        // 给定 n = 3, 一共有 5 种不同结构的二叉搜索树:
        //    1         3     3      2      1
        //     \       /     /      / \      \
        //      3     2     1      1   3      2
        //     /     /       \                 \
        //    2     1         2                 3

        // 二叉查找树（Binary Search Tree），（又：二叉搜索树，二叉排序树）
        // 它或者是一棵空树，或者是具有下列性质的二叉树： 
        //  若它的左子树不空，则左子树上所有结点的值均小于它的根结点的值； 
        //  若它的右子树不空，则右子树上所有结点的值均大于它的根结点的值； 
        //  它的左、右子树也分别为二叉排序树。


        public int NumTrees(int n) 
        {
            if(n < 3)
            {
                return n;
            }
            int[] childTrssCounts = new int[n+1];
            childTrssCounts[0] = 1;
            childTrssCounts[1] = 1;
            childTrssCounts[2] = 2;
            for(var i = 3;i<=n ;i++)
            {
                for(var j = 0; j < i ;j++)
                {
                    childTrssCounts[i] += childTrssCounts[j] * childTrssCounts[i - j - 1];
                }
            }
            return childTrssCounts[n];
        }


            public int numTrees(int n) 
            {
                int[] G = new int[n + 1];
                G[0] = 1;
                G[1] = 1;

                for (int i = 2; i <= n; ++i) 
                {
                    for (int j = 1; j <= i; ++j) 
                    {
                        G[i] += G[j - 1] * G[i - j];
                    }
                }
                return G[n];
            }


    }
}




