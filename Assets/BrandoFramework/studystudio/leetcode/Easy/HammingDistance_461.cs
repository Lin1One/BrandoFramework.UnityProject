#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 461. 汉明距离
        // 两个整数之间的汉明距离指的是这两个数字对应二进制位不同的位置的数目。
        // 给出两个整数 x 和 y，计算它们之间的汉明距离。

        // 注意：
        // 0 ≤ x, y < 231.

        // 示例:
        // 输入: x = 1, y = 4
        // 输出: 2

        // 解释:
        // 1   (0 0 0 1)
        // 4   (0 1 0 0)
        //        ↑   ↑

        // 上面的箭头指出了对应二进制位不同的位置。


        public int HammingDistance(int x, int y) 
        {
            //bitCount 数出整数二进制下 1 的个数
            //1^0 = 1 ,0^1 =1 ,0^0 = 0 ,1^1 = 0
            var xor = (x^y);
            int count = 0;
            while (xor != 0)
            {
                if ((xor & 1) == 1)
                {
                    count++;
                }
                xor >>= 1;
            }
            return count;
        }


        public int HammingDistance2(int x,int y)
        {
            var xor = x^y;
            int count = 0;
            while(xor != 0)
            {
                count++;
                xor = (xor - 1 )&xor;
            }
            return count;
        }

        //字符串遍历
        public int HammingDistance3(int x,int y)
        {
            var xor = x^y;
            string xorStr = System.Convert.ToString(xor,2);
            int count = 0;
            foreach(var num in xorStr)
            {
                if(num == '1')
                {
                    count++;
                }
            }
            return count;
        }
        public static int bitCount(int i) 
        {
            int count = 0;
            while(i!=0)
            {
                count++;
                i = (i-1)&i;
            }
            return count;
        }

        //Java 库实现
        // public static int bitCount(int i) 
        // {
        //     // HD, Figure 5-2
        //     i = i - ((i >>> 1) & 0x55555555);
        //     i = (i & 0x33333333) + ((i >>> 2) & 0x33333333);
        //     i = (i + (i >>> 4)) & 0x0f0f0f0f;
        //     i = i + (i >>> 8);
        //     i = i + (i >>> 16);
        //     return i & 0x3f;
        // }
    }
}


