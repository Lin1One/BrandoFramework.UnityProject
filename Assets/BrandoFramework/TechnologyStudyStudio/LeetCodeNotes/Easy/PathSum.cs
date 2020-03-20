using System.Collections.Generic;
using UnityEngine;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个二叉树，它的每个结点都存放着一个整数值。
        // 找出路径和等于给定数值的路径总数。
        // 路径不需要从根节点开始，也不需要在叶子节点结束，
        // 但是路径方向必须是向下的（只能从父节点到子节点）。
        // 二叉树不超过1000个节点，且节点数值范围是 [-1000000,1000000] 的整数。

        // 示例：
        // root = [10,5,-3,3,2,null,11,3,-2,null,1], sum = 8

        //       10
        //      /  \
        //     5   -3
        //    / \    \
        //   3   2   11
        //  / \   \
        // 3  -2   1

        // 返回 3。和等于 8 的路径有:

        // 1.  5 -> 3
        // 2.  5 -> 2 -> 1
        // 3.  -3 -> 11


        // 路径的开头可以不是根节点，结束也可以不是叶子节点，是不是有点复杂？
        // 如果问题是这样：找出以根节点为开始，任意节点可作为结束，且路径上的节点和为 sum 的路径的个数；
        // 是不是前序遍历一遍二叉树就可以得到所有这样的路径？是的；
        // 如果这个问题解决了，那么原问题可以分解成多个这个问题；
        // 是不是和数线段是同一个问题，只不过线段变成了二叉树；
        // 在解决了以根节点开始的所有路径后，就要找以根节点的左孩子和右孩子开始的所有路径，
        // 三个节点构成了一个递归结构；

        // 递归
        public int PathSum1(TreeNode root, int sum) 
        {
            if (root == null) 
            {
                return 0;
            }

            return Paths(root, sum) 
                + PathSum1(root.left, sum) 
                + PathSum1(root.right, sum);
        }

        private int Paths(TreeNode root, int sum) 
        {
            if (root == null) 
            {
                return 0;
            }

            int res = 0;
            if (root.val == sum)
            {
                res += 1;            
            }
            
            res += Paths(root.left, sum - root.val);
            res += Paths(root.right, sum - root.val);
            return res;
        }


    //     //  2.第一种做法很明显效率不够高，存在大量重复计算
    //     // 所以第二种做法，采取了类似于数组的前n项和的思路，
    //     // 比如sum[4] == sum[1]，那么1到4之间的和肯定为0
    //     // 对于树的话，采取 DFS加回溯，每次访问到一个节点，把该节点加入到当前的pathSum中
    //     // 然后判断是否存在一个之前的前n项和，其值等于pathSum与sum之差
    //     // 如果有，就说明现在的前n项和，减去之前的前n项和，等于sum，
    //     // 那么也就是说，这两个点之间的路径和，就是sum
    //     // 最后要注意的是，记得回溯，把路径和弹出去


    // public int pathSum2(TreeNode root, int sum) 
    // {
    //     Dictionary<int, int> map = new Dictionary<int, int>();
    //     map.Add(0, 1);
    //     return helper(root, map, sum, 0);
    // }
    
    // int helper(TreeNode root, Dictionary<int, int> map, int sum, int pathSum)
    // {
    //     int res = 0;
    //     if(root == null) return 0;
        
    //     pathSum += root.val;
    //     res += map.getOrDefault(pathSum - sum, 0);
    //     map.put(pathSum, map.getOrDefault(pathSum, 0) + 1);
    //     res = helper(root.left, map, sum, pathSum) + helper(root.right, map, sum, pathSum) + res;
    //     map.put(pathSum, map.get(pathSum) - 1);
    //     return res;
    // }
    }
}


