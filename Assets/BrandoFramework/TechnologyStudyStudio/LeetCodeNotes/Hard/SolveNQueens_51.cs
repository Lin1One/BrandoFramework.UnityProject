using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 51. N 皇后
        // n 皇后问题研究的是如何将 n 个皇后放置在 n×n 的棋盘上，
        // 并且使皇后彼此之间不能相互攻击。
        // 给定一个整数 n，返回所有不同的 n 皇后问题的解决方案。
        // 每一种解法包含一个明确的 n 皇后问题的棋子放置方案，
        // 该方案中 'Q' 和 '.' 分别代表了皇后和空位。

        // 示例:

        // 输入: 4
        // 输出: [
        //  [".Q..",  // 解法 1
        //   "...Q",
        //   "Q...",
        //   "..Q."],

        //  ["..Q.",  // 解法 2
        //   "Q...",
        //   "...Q",
        //   ".Q.."]
        // ]



#region 普通回溯
        public  IList<IList<String>> SolveNQueens1(int n) 
        {
            bool[] rowUsed = new bool[n];
            bool[] colUsed = new bool[n];
            //主对角线 i>=j时 [i-j] i<j时 [j-i+n-1]
            bool[] zhuxieUsed = new bool[2*n-1];
            //次对角线 [i+j]
            bool[] fuxieUsed = new bool[2*n-1];
            IList<IList<String>> result = new List<IList<String>>();
            IList<String> list = new List<String>();
            SolveBack2(zhuxieUsed,fuxieUsed,rowUsed,colUsed,n,result,list);
            return result;
    }

        private  void SolveBack2(
            bool[] zhuxieUsed,
            bool[] fuxieUsed,
            bool[] rowUsed, 
            bool[] colUsed, 
            int n, 
            IList<IList<String>> result, 
            IList<String> list) 
        {
            if (list.Count == n) 
            {
                result.Add(new List<String>(list));
            }
            else 
            {
                //行
                for (int i = 0;i < n;i++)
                {
                    if (i == list.Count)
                    {
                        for (int j = 0; j < n;j++)
                        {
                            bool canUse;
                            if (i >= j)
                            {
                                canUse = rowUsed[i] || colUsed[j] || zhuxieUsed[i-j]|| fuxieUsed[i+j];
                            }
                            else 
                            {
                                canUse = rowUsed[i] || colUsed[j] || zhuxieUsed[j-i+n-1] || fuxieUsed[i+j];
                            }
                            if (!canUse)
                            {
                                list.Add(putString(j,n));
                                rowUsed[i] = true;
                                colUsed[j] = true;
                                if (i >=j)
                                {
                                    zhuxieUsed[i-j] = true;
                                }
                                else 
                                {
                                    zhuxieUsed[j-i+n-1] = true;
                                }
                                fuxieUsed[i+j] = true;

                                SolveBack2(zhuxieUsed,fuxieUsed,rowUsed,colUsed,n,result,list);

                                list.RemoveAt(list.Count - 1);
                                rowUsed[i] = false;
                                colUsed[j] = false;
                                if (i >=j)
                                {
                                    zhuxieUsed[i-j] = false;
                                }
                                else 
                                {
                                    zhuxieUsed[j-i+n-1] = false;
                                }
                                fuxieUsed[i+j] = false;
                            }
                        }
                    }
                }
            }
        }

        private string putString(int index,int n)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0;i<n;i++) 
            {
                if (i == index)
                {
                    sb.Append("Q");
                }
                else 
                {
                    sb.Append(".");
                }
            }
            return sb.ToString();
        }
#endregion

        //private 
        public IList<IList<string>> SolveNQueens(int n) 
        {
            return null;
        }

List<IList<string>> result = new List<IList<string>>();
    
    public IList<IList<string>> SolveNQueens2(int n) 
    {
        var temp = new List<char[]>();        
        for(int i= 0; i< n;i++)
        {
            //temp.Add(Enumerable.Repeat('.', n).ToArray());
        }
        Search(0, n, temp);
        return result;
    }
    
    void Search(int row, int n, List<char[]> temp)
    {
        if(row==n)
        {
            //result.Add(temp.Select(cs=>new string(cs)).ToList());
            return;
        }
        for(int c=0; c< n; c++)
        {
            bool ok = true;            
            for(int above = 1; above<=row; above++)
            {
                if(temp[row - above][c] == 'Q' 
                   || (c-above >=0 && temp[row -above][c-above]  =='Q')
                   || (c+above < n  && temp[row - above][c+above]  =='Q'))
                {
                    ok = false;
                    break;
                }
            }
            if(ok)
            {
                temp[row][c] = 'Q';
                Search(row+1, n, temp);
                temp[row][c] = '.';
            }
        }
    }




    }
}




