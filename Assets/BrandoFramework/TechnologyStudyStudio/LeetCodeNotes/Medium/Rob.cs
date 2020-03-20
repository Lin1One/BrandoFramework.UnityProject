using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 337. 打家劫舍 III
        // 在上次打劫完一条街道之后和一圈房屋后，
        // 小偷又发现了一个新的可行窃的地区。这个地区只有一个入口，
        // 我们称之为“根”。 除了“根”之外，每栋房子有且只有一个“父“房子与之相连。
        // 一番侦察之后，聪明的小偷意识到“这个地方的所有房屋的排列类似于一棵二叉树”。 
        // 如果两个直接相连的房子在同一天晚上被打劫，房屋将自动报警。
        // 计算在不触动警报的情况下，小偷一晚能够盗取的最高金额。

        // 示例 1:

        // 输入: [3,2,3,null,3,null,1]

        //      3
        //     / \
        //    2   3
        //     \   \ 
        //      3   1

        // 输出: 7 
        // 解释: 小偷一晚能够盗取的最高金额 = 3 + 3 + 1 = 7.
        // 示例 2:

        // 输入: [3,4,5,1,3,null,1]

        //      3
        //     / \
        //    4   5
        //   / \   \ 
        //  1   3   1

        // 输出: 9
        // 解释: 小偷一晚能够盗取的最高金额 = 4 + 5 = 9.


        public int Rob(TreeNode root) 
        {
            return 0;
        }

        public int Rob1(TreeNode root) 
        {
            int[] res = DoRob(root);
            return Math.Max(res[0],res[1]);
        }
        //res[0]为不包括根节点的最大值，res[1]为包括根节点的最大值
        private int[] DoRob(TreeNode root)
        {
            int[] res = new int[2];
            if(root == null)
                return res;
            int[] left = DoRob(root.left);
            int[] right = DoRob(root.right);
            //不包含根节点，最大值为两个子树的最大值之和
            res[0] = Math.Max(left[0],left[1])+Math.Max(right[0],right[1]);
            //包含根节点，最大值为两个子树不包含根节点的最大值加上根节点的值
            res[1] = left[0] + right[0] + root.val;
            return res;
        }

        public int Rob2(TreeNode root)
        {
            return doRob(root);
        }
        private int doRob(TreeNode node)
        {
            if (node == null)
                return 0;
            int include = node.val;
            int exclude = doRob(node.left) + doRob(node.right);
            if (node.left != null)
            {
                include += doRob(node.left.left);
                include += doRob(node.left.right);
            }
            if (node.right != null)
            {
                include += doRob(node.right.left);
                include += doRob(node.right.right);
            }
            return Math.Max(include,exclude);
        }



    }
}


