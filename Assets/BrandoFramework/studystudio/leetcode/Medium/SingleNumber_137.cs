#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion
using System.Collections.Generic;
using System;

namespace Study.LeetCode
{
    public partial class Solution
    {
        public int SingleNumber2(int[] nums)
        {
            int result = 0;
            for(int i = 0;i < 32; i++)
            {
                int count = 0;
                foreach(var num in nums)
                {
                    count += (num >> i) & 1;
                }
                result ^= (count % 3) << i;
            }
            return result;
        }

        //public int SingleNumber0319(int[] nums)
        //{
        //    int once = 0;
        //    int twice = 0;
        //    foreach (var num in nums)
        //    {
        //        once = ~twice & (num ^ once);
        //        twice = ~once & (num ^ twice);
        //    }

        //    return once;
        //}

        /// <summary>
        /// 仅当 seen_twice 未变时，改变 seen_once。

        /// 仅当 seen_once 未变时，改变seen_twice。
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public int SingleNumber0319(int[] nums)
        {
            int Once = 0;
            int Twice = 0;
            foreach(var num in nums)
            {
                Once = ~Twice & (Once ^ num);
                Twice = ~Once & (Twice ^ num);
            }
            return Once;
        }

    }

}

