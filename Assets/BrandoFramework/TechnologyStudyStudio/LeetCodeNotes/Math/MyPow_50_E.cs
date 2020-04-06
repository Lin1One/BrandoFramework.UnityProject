#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Study.LeetCode
{
    public partial class Solution
    {
        //面试题16.数值的整数次方
        //实现函数double Power(double base, int exponent)，求base的exponent次方。不得使用库函数，同时不需要考虑大数问题。

        //示例 1:
        //输入: 2.00000, 10
        //输出: 1024.00000

        //示例 2:
        //输入: 2.10000, 3
        //输出: 9.26100

        //示例 3:
        //输入: 2.00000, -2
        //输出: 0.25000
        //解释: 2-2 = 1/22 = 1/4 = 0.25


        //说明:
        //-100.0 < x< 100.0
        //n 是 32 位有符号整数，其数值范围是[−231, 231 − 1] 。
        public double MyPow(double x, int n)
        {
            long N = n;
            if (x == 0)
            {
                return 0;
            }
            if (N == 0)
            {
                return 1;
            }
            if (N < 0)
            {
                x = 1 / x;
                N = -N;
            }
            double result = 1;
            double temp = x;
            for (var i = N; i > 0; i /= 2)
            {
                if (i % 2 == 1)
                {
                    result = temp * result;
                }
                temp = temp * temp;
            }
            return result;
        }

        private double FastPow(double x, long n)
        {
            if (n == 0)
            {
                return 1.0;
            }
            double halfNPow = FastPow(x, n / 2);
            if (n % 2 == 0)
            {
                return halfNPow * halfNPow;
            }
            else
            {
                return halfNPow * halfNPow * x;
            }
        }
        public double MyPow2(double x, int n)
        {
            long N = n;
            if (N < 0)
            {
                x = 1 / x;
                N = -N;
            }

            return FastPow(x, N);
        }
    }
}


