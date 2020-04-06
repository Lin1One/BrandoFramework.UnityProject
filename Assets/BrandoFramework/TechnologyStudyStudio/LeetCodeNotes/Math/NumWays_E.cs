#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        //面试题10- II.青蛙跳台阶问题
        //一只青蛙一次可以跳上1级台阶，也可以跳上2级台阶。求该青蛙跳上一个 n 级的台阶总共有多少种跳法。

        //答案需要取模 1e9+7（1000000007），如计算初始结果为：1000000008，请返回 1。

        //示例 1：

        //输入：n = 2
        //输出：2
        //示例 2：

        //输入：n = 7
        //输出：21
        //提示：

        //0 <= n <= 100
        public int NumWays(int n)
        {
            if (n < 2)
            {
                return 1;
            }
            int lastOneSteps = 0;
            int lastTwoSteps = 1;
            for (var i = 2; i <= n; i++)
            {
                int temp = lastOneSteps + lastTwoSteps;
                lastOneSteps = lastTwoSteps % 1000000007;
                lastTwoSteps = temp % 1000000007;
            }
            return (lastTwoSteps + lastOneSteps) % 1000000007;
        }
    }
}


