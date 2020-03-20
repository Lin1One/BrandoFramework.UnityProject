using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        //79. 单词搜索
        //给定一个二维网格和一个单词，找出该单词是否存在于网格中。
        //单词必须按照字母顺序，通过相邻的单元格内的字母构成，
        //其中“相邻”单元格是那些水平相邻或垂直相邻的单元格。
        //同一个单元格内的字母不允许被重复使用。

        // 示例:
        // board =
        // [
        //   ['A','B','C','E'],
        //   ['S','F','C','S'],
        //   ['A','D','E','E']
        // ]

        // 给定 word = "ABCCED", 返回 true.
        // 给定 word = "SEE", 返回 true.
        // 给定 word = "ABCB", 返回 false.


        private bool[,] hasUseCell;
        private int[,] directionArray = new int[,]{{1,0},{-1,0},{0,1},{0,-1}};
        private int rowCount;
        private int colCount;
        public bool Exist(char[][] board, string word) 
        {
            if (board == null||board.Length == 0)
            {
                return false;
            }
            rowCount = board.Length;
            colCount = board[0].Length;
            hasUseCell = new bool[rowCount,colCount];
            for(var i = 0; i < rowCount;i++)
            {
                for(var j = 0; j < colCount;j++)
                {
                    if(board[i][j] == word[0])
                    {
                        if(SearchNeighbor(board,i,j,word,1))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool SearchNeighbor(char[][] board,int row,int col,string word,int curIndex)
        {
            if(hasUseCell[row,col])
            {
                return false;
            }
            else
            {
                hasUseCell[row,col] = true;
            }
            if(curIndex == word.Length)
            {
                return true;
            }

            char targetChar = word[curIndex];

            for(var i = 0;i< 4 ;i++)
            {
                var newRow = row + directionArray[i,0];
                var newCol = col + directionArray[i,1];
                if(newRow < rowCount && 
                    newRow >= 0 && 
                    newCol < colCount &&
                    newCol >= 0
                    )
                {
                    if(targetChar == board[newRow][newCol])
                    {
                        if(SearchNeighbor(board,newRow,newCol,word,curIndex+1))
                        {
                            return true;
                        }
                    }
                    
                }
            }
            hasUseCell[row,col] = false;
            return false;
        }

    }
}




