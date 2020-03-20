using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 102. 二叉树的层次遍历
        // 给定一个二叉树，返回其按层次遍历的节点值。 （即逐层地，从左到右访问所有节点）。

        // 例如:
        // 给定二叉树: [3,9,20,null,null,15,7],
        //     3
        //    / \
        //   9  20
        //     /  \
        //    15   7
        // 返回其层次遍历结果：

        // [
        // [3],
        // [9,20],
        // [15,7]
        // ]


        //  深度优先搜索（DFS）
        // 在这个策略中，我们采用深度作为优先级，以便从跟开始一直到达某个确定的叶子，然后再返回根到达另一个分支。
        // 深度优先搜索策略又可以根据根节点、左孩子和右孩子的相对顺序被细分为先序遍历，中序遍历和后序遍历。

        // 宽度优先搜索（BFS）
        // 我们按照高度顺序一层一层的访问整棵树，高层次的节点将会比低层次的节点先被访问到。

        //迭代
        public IList<IList<int>> LevelOrder1(TreeNode root) 
        {
            Queue<TreeNode> levelQueue = new Queue<TreeNode>();
            List<IList<int>> ans = new List<IList<int>>();
            levelQueue.Enqueue(root);
            while(levelQueue.Count != 0)
            {
                List<int> levelValues = new List<int>();
                var levelNodeCount =  levelQueue.Count;
                for(var i = 0;i < levelNodeCount;i++)
                {
                    var node = levelQueue.Dequeue();
                    if(node != null)
                    {
                        levelValues.Add(node.val);
                        levelQueue.Enqueue(node.left);
                        levelQueue.Enqueue(node.right);
                    }        
                }
                if(levelValues.Count > 0)
                {
                    ans.Add(levelValues);
                }
                
            }
            return ans;
        }

        //
        public IList<IList<int>> LevelOrder2(TreeNode root) 
        { 
            List<IList<int>> ans = new List<IList<int>>();
            AddLevelValue(root,0,ans);
            return ans;
        }

        private void AddLevelValue(TreeNode node,int level,List<IList<int>> ans)
        {
            if(node == null)
                return;
            if(ans.Count == level )
            {
                ans.Add(new List<int>());
            }
            ans[level].Add(node.val);
            AddLevelValue(node.left,level+1,ans);
            AddLevelValue(node.right,level+1,ans);
        }

    }
}




