#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System;
namespace Study.LeetCode
{
    public partial class Solution
    {
        // 29. 两数相除
        // 给定两个整数，被除数 dividend 和除数 divisor。
        // 将两数相除，要求不使用乘法、除法和 mod 运算符。
        // 返回被除数 dividend 除以除数 divisor 得到的商。

        // 示例 1:
        // 输入: dividend = 10, divisor = 3
        // 输出: 3

        // 示例 2:
        // 输入: dividend = 7, divisor = -3
        // 输出: -2

        // 说明:
        // 被除数和除数均为 32 位有符号整数。
        // 除数不为 0。
        // 假设我们的环境只能存储 32 位有符号整数，
        // 其数值范围是 [−231,  231 − 1]。本题中，如果除法结果溢出，则返回 231 − 1。


     //思想就是:除数倍增,商的计数器count也倍增,直到除数刚好比被除数小,
     //就用被除数减去除数,更新数值,再进行下一次循环,直到被除数绝对值小于除数绝对值,
    // 其间需要考虑除数溢出,count溢出的情况。
        public int divide1(int dividend, int divisor) 
        {
            //排除特殊情况
            if (dividend == int.MinValue && divisor == -1)
                return int.MaxValue;
            //计数器
            int count = 0;
            //记录加倍前的doubledivisor
            int tmpdivisor = divisor;
            //记录加倍后的doubledivisor
            int doubledivisor = tmpdivisor;
            //记录加倍前的counttime
            int counttime = 1;
            //记录加倍前的counttime
            int doublecounttime = counttime;
            while (dividend != 0) 
            {
                //恢复初始情况
                tmpdivisor = divisor;
                doubledivisor = tmpdivisor;
                counttime = 1;
                doublecounttime = counttime;
                //这样判断可以避免溢出,不要用abs()
                if ((dividend <= 0 && divisor < 0 && dividend <= divisor) ||
                        (dividend >= 0 && divisor > 0 && dividend >= divisor)||
                        (dividend <= 0 && divisor > 0 && dividend <= 0-divisor) ||
                        (dividend >= 0 && divisor < 0 && 0-dividend <= divisor)) 
                {
                    //除数和被除数同号的情况,防止溢出
                    while ((dividend <= 0 && divisor < 0 && dividend <= doubledivisor && 
                    (int.MinValue-doubledivisor)<=doubledivisor) ||
                            (dividend >= 0 && divisor > 0 && dividend >= doubledivisor &&
                            (int.MaxValue-doubledivisor)>=doubledivisor) ||
                            (dividend <= 0 && divisor > 0 && dividend <= 0-doubledivisor && 
                            (int.MinValue+doubledivisor)<=0-doubledivisor) ||
                            (dividend >= 0 && divisor < 0 && 0-dividend <= doubledivisor) && 
                            (int.MaxValue+doubledivisor)>=0-doubledivisor) 
                    {
                        //记录加倍前的doubledivisor,counttime
                        tmpdivisor = doubledivisor;
                        counttime = doublecounttime;
                        //加倍,注意不能超过范围
                        doubledivisor += doubledivisor;
                        doublecounttime += doublecounttime;
                    }
                    //更新数值
                    count += (dividend <= 0 && divisor < 0) ||
                            (dividend >= 0 && divisor > 0) ? counttime:0-counttime;
                    dividend -= (dividend <= 0 && divisor < 0) ||
                            (dividend >= 0 && divisor > 0) ? tmpdivisor:0-tmpdivisor;

                }
                else 
                {
                    return count;
                }
            }
            return count;
        }
        
        // 除了异或判断符号位，其他没有用到位运算，都是加减运算，性能还是够用的。
        // 题目分类提示了二分查找，那么按二分查找的思路去想就行了，既然要求不能使用乘除模运算，
        // 那么就让除数不断倍增，与被除数对比，倍增过头了就初始化为原始的除数值再次循环倍增，
        // 循环过程中被除数减去除数不断减小，直到小于除数为止，
        // 其过程就是把二分查找中的 mid=(left+right)/2 替换成了divisor*2，
        // 把 left 和 right 替换成了 divisor_tmp 和 divideng_tmp，这样理解就比较直观了。
        public int divide2(int dividend, int divisor) 
        {
            /** 除数为零就返回-1 按照测试样例的要求写的*/
            if (divisor==0)
                return -1;
            if (dividend==0)
                return 0;
            // -2147483648, -1 这个测试样例的确没想到
            if (dividend == int.MinValue && divisor == -1)
                return int.MaxValue;
            /** 符号位的处理参考了大佬的异或处理方法*/
            bool negetive= (dividend^ divisor)<0;

            /** div_count 是当前divisor_tmp相对于divisor的倍数 */
            int res = 0; 
            int div_count = 1;

            /** 因为值溢出之后边界问题处理太繁琐了，直接将数值转为long省去麻烦 */
            long dividend_tmp = Math.Abs((long)dividend);
            long divisor_tmp = Math.Abs((long)divisor);
            
            while (dividend_tmp >= divisor_tmp) 
            {
                dividend_tmp -= divisor_tmp;
                res += div_count;
                
                if (dividend_tmp < Math.Abs((long)divisor))
                    break;
                
                /** divisor_tmp无法倍增时，就将其初始化为 divisor绝对值，重新开始下一轮倍增*/
                if (dividend_tmp - divisor_tmp < divisor_tmp) 
                {
                    divisor_tmp = Math.Abs(divisor);
                    div_count = 1;
                    continue;
                } 
                
                /** 不断倍增divisor_tmp直到和dividend_tmp一样大*/
                divisor_tmp += divisor_tmp;

                div_count += div_count;
            }
            return negetive ? 0 - res : res;
        }

        public int Divide(int dividend, int divisor) 
        {
            bool dividendNega = true;
            bool divisorNega = true;
            if (dividend > 0)
            {
                dividend = -dividend;
                dividendNega = false;
            }
            if (divisor > 0)
            {
                divisor = -divisor;
                divisorNega = false;
            }
            var result = 0;
            while (dividend <= divisor)
            {
                var currentDivisot = divisor;
                var num = 1;
                while (dividend <= currentDivisot)
                {
                    dividend -= currentDivisot;
                    result += num;
                    //防止溢出
                    if (currentDivisot > int.MinValue >> 1)
                    {
                        currentDivisot += currentDivisot;
                        num += num;
                    }
                }
            }
            if (result == int.MinValue && dividendNega == divisorNega)
            {
                return int.MaxValue;
            }
            if (dividendNega == divisorNega)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }
}


