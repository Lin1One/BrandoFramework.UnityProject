#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 538. 把二叉搜索树转换为累加树
        // 给定一个二叉搜索树,把它转换成为累加树（Greater Tree)，
        // 使得每个节点的值是原来的节点值加上所有大于它的节点值之和。

        // 例如：

        // 输入: 二叉搜索树:
        //               5
        //             /   \
        //            2     13

        // 输出: 转换为累加树:
        //              18
        //             /   \
        //           20     13

        // 思路
        // dfs逆中序遍历

        // 递归函数的参数
        // node：当前遍历到的节点
        // sum：累加值

        // 过程逻辑
        // 退出条件：当前遍历到的节点为空，返回sum值

        // 过程是逆中序遍历:
        // 1.带着sum去遍历右子树，返回遍历后累加得到的sum
        // 2.暂存当前节点的值，用于累加sum
        // 3.将从右子树返回的sum累加到当前节点的值
        // 4.将sum与步骤2暂存的值进行累加
        // 5.带着在当前层累加好的sum去遍历左子树

        // 返回当前层得到的更新好的sum

        public TreeNode ConvertBST1(TreeNode root) 
        {
            if (root != null) 
            {
                DFS(root, 0);
            }
            return root;
        }

        private int DFS(TreeNode node, int sum) 
        {
            if (node == null)
            {
                return sum;
            }
            
            sum = DFS(node.right, sum);
            
            int nodeValue = node.val;
            node.val += sum;
            sum += nodeValue;
            
            sum = DFS(node.left, sum);
            
            return sum;
        }

        public TreeNode ConvertBST2(TreeNode root) 
        {
            int sum = 0;
            BST(root,ref sum);
            return root;
        }
        
        private void BST(TreeNode root,ref int sum)
        {
            if(root == null)
                return;
            BST(root.right,ref sum);
            sum += root.val;
            root.val = sum;
            BST(root.left,ref sum);
        }
    }
}


