using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //  236. 二叉树的最近公共祖先
        // 给定一个二叉树, 找到该树中两个指定节点的最近公共祖先。
        // 百度百科中最近公共祖先的定义为：“对于有根树 T 的两个结点 p、q，
        // 最近公共祖先表示为一个结点 x，满足 x 是 p、q 的祖先且 x 的深度尽可能大
        // （一个节点也可以是它自己的祖先）。”

        // 例如，给定如下二叉树:  root = [3,5,1,6,2,0,8,null,null,7,4]

        // 示例 1:

        // 输入: root = [3,5,1,6,2,0,8,null,null,7,4], p = 5, q = 1
        // 输出: 3
        // 解释: 节点 5 和节点 1 的最近公共祖先是节点 3。
        // 示例 2:

        // 输入: root = [3,5,1,6,2,0,8,null,null,7,4], p = 5, q = 4
        // 输出: 5
        // 解释: 节点 5 和节点 4 的最近公共祖先是节点 5。
        // 因为根据定义最近公共祖先节点可以为节点本身。


        // 方法一：递归
        // 这种方法非常直观。先深度遍历改树。
        // 当你遇到节点 p 或 q 时，返回一些布尔标记。
        // 该标志有助于确定是否在任何路径中找到了所需的节点。
        // 最不常见的祖先将是两个子树递归都返回真标志的节点。
        // 它也可以是一个节点，它本身是p或q中的一个，
        // 对于这个节点,子树递归返回一个真标志。

        // 让我们看看基于这个想法的形式算法。

        // 算法：
        // 从根节点开始遍历树。
        // 如果当前节点本身是 p 或 q 中的一个，我们会将变量 mid 标记为 true，
        //      并继续搜索左右分支中的另一个节点。
        // 如果左分支或右分支中的任何一个返回 true，则表示在下面找到了两个节点中的一个。
        // 如果在遍历的任何点上，左、右或中三个标志中的任意两个变为 true，
        //      这意味着我们找到了节点 p 和 q 的最近公共祖先。
        // 让我们看一个示例，然后搜索树中两个节点 9 和 11 的最近公共祖先。
        private TreeNode ans;
        public TreeNode lowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q) 
        {
            // Traverse the tree
            this.ans = null;
            this.RecurseTree(root, p, q);
            return this.ans;
        }

        private bool RecurseTree(TreeNode currentNode,TreeNode p, TreeNode q)
        {
            if(currentNode == null)
            {
                return false;
            }
            bool leftTreeHaveTarget = RecurseTree(currentNode.left,p,q);
            bool rightTreeHaveTarget = RecurseTree(currentNode.right,p,q);
            bool currentNodeIsTarget = currentNode == p || currentNode == q;
            if(leftTreeHaveTarget && rightTreeHaveTarget)
            {
                ans =  currentNode;
            }
            if(leftTreeHaveTarget && currentNodeIsTarget)
            {
                ans =  currentNode;
            }
            if(rightTreeHaveTarget && currentNodeIsTarget)
            {
                ans =  currentNode;
            }
            return leftTreeHaveTarget || rightTreeHaveTarget || currentNodeIsTarget;
        }
        // private boolean recurseTree(TreeNode currentNode, TreeNode p, TreeNode q) {

        //     // If reached the end of a branch, return false.
        //     if (currentNode == null) 
        //     {
        //         return false;
        //     }

        //     // Left Recursion. If left recursion returns true, set left = 1 else 0
        //     int left = this.recurseTree(currentNode.left, p, q) ? 1 : 0;

        //     // Right Recursion
        //     int right = this.recurseTree(currentNode.right, p, q) ? 1 : 0;

        //     // If the current node is one of p or q
        //     int mid = (currentNode == p || currentNode == q) ? 1 : 0;


        //     // If any two of the flags left, right or mid become True
        //     if (mid + left + right >= 2) 
        //     {
        //         this.ans = currentNode;
        //     }

        //     // Return true if any one of the three bool values is True.
        //     return (mid + left + right > 0);
        // }





        public TreeNode LowestCommonAncestor(TreeNode root, TreeNode p, TreeNode q) 
        {
            return null;
        }

        // 方法二：使用父指针迭代
        // 如果每个节点都有父指针，那么我们可以从 p 和 q 返回以获取它们的祖先。
        // 在这个遍历过程中，我们得到的第一个公共节点是 LCA 节点。
        // 我们可以在遍历树时将父指针保存在字典中。

        // 算法：
        // 从根节点开始遍历树。
        // 在找到 p 和 q 之前，将父指针存储在字典中。
        // 一旦我们找到了 p 和 q，我们就可以使用父亲字典获得 p 的所有祖先，
        //      并添加到一个称为祖先的集合中。
        // 同样，我们遍历节点 q 的祖先。
        // 如果祖先存在于为 p 设置的祖先中，这意味着这是 p 和 q 之间的第一个共同祖先
        // （同时向上遍历），因此这是 LCA 节点。


        // 在前面的方法中，我们在回溯过程中遇到 LCA。
        // 我们可以摆脱回溯过程本身。
        // 在这种方法中，我们总是有一个指向可能 LCA 的指针，
        // 当我们找到两个节点时，我们返回指针作为答案。

        // 算法：
        // 从根节点开始。
        // 将 (root, root_state) 放在堆栈上。
        // root_state 定义要遍历该节点的一个子节点还是两个子节点。
        // 当堆栈不为空时，查看堆栈的顶部元素，该元素表示为 (parent_node, parent_state)。
        // 在遍历 parent_node 的任何子节点之前，我们检查 parent_node 本身是否是 p 或 q 中的一个。
        // 当我们第一次找到 p 或 q 的时候，设置一个布尔标记，名为 one_node_found 为 true 。还可以通过在变量 LCA_index 中记录堆栈的顶部索引来跟踪最近的公共祖先。因为堆栈的所有当前元素都是我们刚刚发现的节点的祖先。
        // 第二次 parent_node == p or parent_node == q 意味着我们找到了两个节点，我们可以返回 LCA node。
        // 每当我们访问 parent_node 的子节点时，我们将 (parent_node, updated_parent_state) 推到堆栈上。我们更新父级的状态为子级/分支已被访问/处理，并且相应地更改状态。
        // 当状态变为 BOTH_DONE 时，最终会从堆栈中弹出一个节点，这意味着左、右子树都被推到堆栈上并进行处理。如果 one_node_found 是 true 的，那么我们需要检查被弹出的顶部节点是否可能是找到的节点的祖先之一。在这种情况下，我们需要将LCA_index减少一个。因为其中一位祖先被弹出了。 当同时找到 p 和 q 时，LCA_index 将指向堆栈中包含 p 和 q 之间所有公共祖先的索引。并且 LCA_index 元素具有p和q之间的最近公共祖先。



    }
}




