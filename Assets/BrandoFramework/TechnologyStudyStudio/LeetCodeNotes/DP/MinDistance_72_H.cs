#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //72. 编辑距离
        //给你两个单词 word1 和 word2，请你计算出将 word1 转换成 word2 所使用的最少操作数 。

        //你可以对一个单词进行如下三种操作：

        //插入一个字符
        //删除一个字符
        //替换一个字符


        //示例 1：

        //输入：word1 = "horse", word2 = "ros"
        //输出：3
        //解释：
        //horse -> rorse(将 'h' 替换为 'r')
        //rorse -> rose(删除 'r')
        //rose -> ros(删除 'e')
        //示例 2：

        //输入：word1 = "intention", word2 = "execution"
        //输出：5
        //解释：
        //intention -> inention(删除 't')
        //inention -> enention(将 'i' 替换为 'e')
        //enention -> exention(将 'n' 替换为 'x')
        //exention -> exection(将 'n' 替换为 'c')
        //exection -> execution(插入 'u')

        public int MinDistance(string word1, string word2)
        {
            //步骤一、定义数组元素的含义
            //由于我们的目的求将 word1 转换成 word2 所使用的最少操作数 。
            //那我们就定义 dp[i] [j] 的含义为：当字符串 word1 的长度为 i，
            //字符串 word2 的长度为 j 时，将 word1 转化为 word2 所使用的最少操作次数为 dp[i] [j]。

            //有时候，数组的含义并不容易找，所以还是那句话，我给你们一个套路，剩下的还得看你们去领悟。
            //步骤二：找出关系数组元素间的关系式
            //接下来我们就要找 dp[i][j] 元素之间的关系了，比起其他题，这道题相对比较难找一点，但是，不管多难找，大部分情况下，dp[i][j] 和 dp[i - 1][j]、dp[i][j - 1]、dp[i - 1][j - 1] 肯定存在某种关系。因为我们的目标就是，**从规模小的，通过一些操作，推导出规模大的。对于这道题，我们可以对 word1 进行三种操作

            //   插入一个字符 删除一个字符 替换一个字符

            //由于我们是要让操作的次数最小，所以我们要寻找最佳操作。那么有如下关系式：

            //一、如果我们 word1[i] 与 word2[j] 相等，这个时候不需要进行任何操作，显然有 dp[i] [j] = dp[i - 1][j - 1]。（别忘了 dp[i] [j] 的含义哈）。

            //二、如果我们 word1[i] 与 word2[j] 不相等，这个时候我们就必须进行调整，而调整的操作有 3 种，我们要选择一种。三种操作对应的关系试如下（注意字符串与字符的区别）：

            //（1）、如果把字符 word1[i] 替换成与 word2[j] 相等，则有 dp[i] [j] = dp[i - 1][j - 1] + 1;

            //（2）、如果在字符串 word1末尾插入一个与 word2[j] 相等的字符，则有 dp[i] [j] = dp[i][j - 1] + 1;

            //（3）、如果把字符 word1[i] 删除，则有 dp[i] [j] = dp[i - 1][j] + 1;

            //那么我们应该选择一种操作，使得 dp[i] [j] 的值最小，显然有

            //dp[i] [j] = min(dp[i - 1][j - 1]，dp[i][j - 1]，dp[[i - 1][j]]) + 1;

            //于是，我们的关系式就推出来了，

            //步骤三、找出初始值
            //显然，当 dp[i] [j] 中，如果 i 或者 j 有一个为 0，那么还能使用关系式吗？
            //答是不能的，因为这个时候把 i - 1 或者 j - 1，就变成负数了，数组就会出问题了，
            //所以我们的初始值是计算出所有的 dp[0] [0….n] 和所有的 dp[0….m] [0]。
            //这个还是非常容易计算的，因为当有一个字符串的长度为 0 时，
            //转化为另外一个字符串，那就只能一直进行插入或者删除操作了。

            int n1 = word1.Length;
            int n2 = word2.Length;
            int[,] dp = new int[n1 + 1,n2 + 1];
            for (int j = 1; j <= n2; j++)
            {
                dp[0, j] = dp[0, j - 1] + 1;
            }
            for (int i = 1; i <= n1; i++)
            {
                dp[i, 0] = dp[i - 1, 0] + 1;
            }

            for (int i = 1; i <= n1; i++)
            {
                for (int j = 1; j <= n2; j++)
                {
                    if (word1[i - 1] == word2[j - 1])
                    {
                        dp[i,j] = dp[i - 1,j - 1];
                    }
                    else
                    {
                        dp[i,j] = Math.Min(Math.Min(dp[i - 1,j - 1], dp[i,j - 1]), dp[i - 1,j]) + 1;
                    }
                }
            }
            return dp[n1,n2];
        }
    }
}


