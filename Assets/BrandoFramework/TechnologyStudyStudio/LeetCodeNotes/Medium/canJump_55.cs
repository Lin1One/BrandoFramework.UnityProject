using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 给定一个非负整数数组，你最初位于数组的第一个位置。
        // 数组中的每个元素代表你在该位置可以跳跃的最大长度。

        // 判断你是否能够到达最后一个位置。

        // 示例 1:
        // 输入: [2,3,1,1,4]
        // 输出: true
        // 解释: 从位置 0 到 1 跳 1 步, 然后跳 3 步到达最后一个位置。

        // 示例 2:
        // 输入: [3,2,1,0,4]
        // 输出: false
        // 解释: 无论怎样，你总会到达索引为 3 的位置。但该位置的最大跳跃长度是 0 ， 
        // 所以你永远不可能到达最后一个位置。
        public bool CanJump(int[] nums) 
        {
            return true;
        }
        
        public bool canJump1(int[] nums)
        {
            return canJumpFromPosition(0, nums);
        }   
        public bool canJumpFromPosition(int position, int[] nums) 
        {
            if (position == nums.Length - 1) 
            {
                return true;
            }
            int furthestJump = Math.Min(position + nums[position], nums.Length - 1);
            for (int nextPosition = position + 1; nextPosition <= furthestJump; nextPosition++) 
            {
                if (canJumpFromPosition(nextPosition, nums)) 
                {
                    return true;
                }
            }
            return false;
        }

        // 方法 2：自顶向下的动态规划 记忆表
        // 自顶向下的动态规划可以理解成回溯法的一种优化。我们发现当一个坐标已经被确定为好 / 坏之后，
        // 结果就不会改变了，这意味着我们可以记录这个结果，每次不用重新计算。
        // 因此，对于数组中的每个位置，我们记录当前坐标是好 / 坏，
        // 记录在数组 memo 中，定义元素取值为 GOOD ，BAD，UNKNOWN。这种方法被称为记忆化。

        // 例如，对于输入数组 nums = [2, 4, 2, 1, 0, 2, 0] 的记忆表如下，G 代表 GOOD，B 代表 BAD。
        // 我们发现不能从下标 2，3，4 到达最终坐标 6，但可以从 0，1，5 和 6 到达最终坐标 6。
        // Index	0	1	2	3	4	5	6
        // nums	    2	4	2	1	0	2	0
        // memo 	G	G	B	B	B	G	G
        // 步骤

        // 初始化 memo 的所有元素为 UNKNOWN，除了最后一个显然是 GOOD （自己一定可以跳到自己）
        // 优化递归算法，每步回溯前先检查这个位置是否计算过（当前值为：GOOD / BAD）
        // 如果已知直接返回结果 True / False
        // 否则按照之前的回溯步骤计算
        // 计算完毕后，将结果存入memo表中

        private bool canJump2(int[] nums)
        {
            int[] positionStateArray = new int[nums.Length];
            return canJumpFromPosition2(0, nums,positionStateArray);
        }   
        public bool canJumpFromPosition2(int position, int[] nums,int[] positionStates) 
        {
            if (position == nums.Length - 1) 
            {
                return true;
            }
            if(positionStates[position] == 1)
            {
                return true;
            }
            else if( positionStates[position] == -1)
            {
                return false;
            }
            else
            {
                int furthestJump = Math.Min(position + nums[position], nums.Length - 1);
                for (int nextPosition = position + 1; nextPosition <= furthestJump; nextPosition++) 
                {
                    if (canJumpFromPosition(nextPosition, nums)) 
                    {
                        positionStates[position] = 1;
                        return true;
                    }
                }
                positionStates[position] = -1;
                return false;
            }
        }


        //  自底向上的动态规划
        // 底向上和自顶向下动态规划的区别就是消除了回溯，
        // 在实际使用中，自底向下的方法有更好的时间效率因为我们不再需要栈空间，可以节省很多缓存开销。
        // 更重要的事，这可以让之后更有优化的空间。回溯通常是通过反转动态规划的步骤来实现的。
        // 这是由于我们每次只会向右跳动，意味着如果我们从右边开始动态规划，每次查询右边节点的信息，
        // 都是已经计算过了的，不再需要额外的递归开销，因为我们每次在 memo 表中都可以找到结果。
        public bool canJump3(int[] nums) 
        {
            bool[] positionStateArray = new bool[nums.Length];
            positionStateArray[nums.Length - 1] = true;

            for (int i = nums.Length - 2; i >= 0; i--) 
            {
                int furthestJump = Math.Min(i + nums[i], nums.Length - 1);
                for (int j = i + 1; j <= furthestJump; j++) 
                {
                    if (positionStateArray[j]) 
                    {
                        positionStateArray[i] = true;
                        break;
                    }
                }
            }
            return positionStateArray[0];
            
        }
        // 复杂度分析
        // 时间复杂度：O(n^2)，数组中的每个元素，假设为 i，
        // 需要搜索右边相邻的 nums[i] 个元素查找是否有 GOOD 的坐标。 nums[i] 最多为 n，n 是 nums 数组的大小。
        // 空间复杂度：O(n)，记忆表的存储开销。 

        //  方法 4：贪心
        // 当我们把代码改成自底向上的模式，我们会有一个重要的发现，
        // 从某个位置出发，我们只需要找到第一个标记为 GOOD 的坐标（由跳出循环的条件可得），
        // 也就是说找到最左边的那个坐标。
        // 如果我们用一个单独的变量来记录最左边的 GOOD 位置，
        // 我们就可以避免搜索整个数组，进而可以省略整个 memo 数组。

        // 从右向左迭代，对于每个节点我们检查是否存在一步跳跃可以到达 GOOD 的位置
        // （currPosition + nums[currPosition] >= leftmostGoodIndex）。
        // 如果可以到达，当前位置也标记为 GOOD ，同时，这个位置将成为新的最左边的 GOOD 位置，
        // 一直重复到数组的开头，如果第一个坐标标记为 GOOD 意味着可以从第一个位置跳到最后的位置。

        // 模拟一下这个操作，对于输入数组 nums = [9, 4, 2, 1, 0, 2, 0]，
        // 我们用 G 表示 GOOD，用 B 表示 BAD 和 U 表示 UNKNOWN。
        // 我们需要考虑所有从 0 出发的情况并判断坐标 0 是否是好坐标。
        // 由于坐标 1 是 GOOD，我们可以从 0 跳到 1 并且 1 最终可以跳到坐标 6，
        // 所以尽管 nums[0] 可以直接跳到最后的位置，我们只需要一种方案就可以知道结果。

        // Index	0	1	2	3	4	5	6
        // nums	    9	4	2	1	0	2	0
        // memo	    U	G	B	B	B	G	G
        // Java

    public bool canJump4(int[] nums) 
    {
        int lastPos = nums.Length - 1;
        for (int i = nums.Length - 1; i >= 0; i--) 
        {
            if (i + nums[i] >= lastPos) 
            {
                lastPos = i;
            }
        }
        return lastPos == 0;
    }

    // 复杂度分析
    // 时间复杂度：O(n)O(n)，只需要访问 nums 数组一遍，共 nn 个位置，nn 是 nums 数组的长度。
    // 空间复杂度：O(1)O(1)，不需要额外的空间开销。 

    public bool canJump5(int[] nums) 
    {
        int max = 0; 
        for (int i = 0; i < nums.Length; i++) 
        {
            if (i > max) 
            {
                return false;
            }
            max = Math.Max(max, nums[i] + i);
        }
        return true;
    }


    }
}


