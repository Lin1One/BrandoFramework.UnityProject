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
    // 给定一个数组，它的第 i 个元素是一支给定股票第 i 天的价格。
    // 如果你最多只允许完成一笔交易（即买入和卖出一支股票），设计一个算法来计算你所能获取的最大利润。
    // 注意你不能在买入股票前卖出股票。

    // 示例 1:
    // 输入: [7,1,5,3,6,4]
    // 输出: 5
    // 解释: 在第 2 天（股票价格 = 1）的时候买入，在第 5 天（股票价格 = 6）的时候卖出，最大利润 = 6-1 = 5 。
    // 注意利润不能是 7-1 = 6, 因为卖出价格需要大于买入价格。
    // 示例 2:

    // 输入: [7,6,4,3,1]
    // 输出: 0
    // 解释: 在这种情况下, 没有交易完成, 所以最大利润为 0。

        public int MaxProfit(int[] prices) 
        {
            //暴力
            //我们需要找出给定数组中两个数字之间的最大差值（即，最大利润）。
            //此外，第二个数字（卖出价格）必须大于第一个数字（买入价格）。
            //形式上，对于每组 i 和 j（其中 j > i）我们需要找出 \max(prices[j] - prices[i]))。
            int maxProfit = 0;
            for(var i = 0;i<prices.Length;i++)
            {
                for(var j = i+1;j<prices.Length;j++)
                {
                    if(prices[i] > prices[j])
                    {
                        continue;
                    }
                    var tempProfit = prices[j] - prices[i];
                    maxProfit = Math.Max(maxProfit,tempProfit);
                }
            }
            return maxProfit;
        }

        public int MaxProfit0427(int[] prices) 
        {
        int minprice = int.MaxValue;
        int maxprofit = 0;
        for (int i = 0; i < prices.Length; i++) 
        {
            if (prices[i] < minprice)
            {
                minprice = prices[i];
            }
            else if (prices[i] - minprice > maxprofit)
            {
                maxprofit = prices[i] - minprice;
            }  
        }
        return maxprofit;
        }

        public int maxProfit2(int[] prices)
        {
            if(prices == null|| prices.Length == 1)
            {
                return 0;
            }
            int result = 0;
            int buyPrices = int.MaxValue;
            for(var i = 0;i<prices.Length;i++)
            {
                if(prices[i]< buyPrices)
                {
                    buyPrices = prices[i];
                }
                else
                {
                    result = Math.Max(result, prices[i] - buyPrices);
                }
            }
            return result;
        }

        public int maxProfit3(int[] prices)
        {
            //DP
            if (prices == null)
            {
                return 0;
            }
            var maxProfit = 0;
            var result = 0;
            for (var i = 0; i < prices.Length; i++)
            {
                var buyPrice = prices[i];
                for (var j = i + 1; j < prices.Length; j++)
                {
                    maxProfit = Math.Max(maxProfit, prices[j] - buyPrice);
                }
                result = Math.Max(maxProfit, result);
            }
            return result;
        }
    }

}

