using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace Study.LeetCode
{
    public partial class Solution
    {
        // 200. 岛屿数量
        // 给定一个由 '1'（陆地）和 '0'（水）组成的的二维网格，计算岛屿的数量。
        // 一个岛被水包围，并且它是通过水平方向或垂直方向上相邻的陆地连接而成的。
        // 你可以假设网格的四个边均被水包围。

        // 示例 1:
        // 输入:
        // 11110
        // 11010
        // 11000
        // 00000

        // 输出: 1
        
        // 示例 2:
        // 输入:
        // 11000
        // 11000
        // 00100
        // 00011

        // 输出: 3

        int xLength = 0;
        int yLength = 0;
        public int NumIslands(char[][] grid) 
        {
            if (grid == null || grid.Length == 0) 
            {
                return 0;
            }
            xLength = grid.Length;
            yLength = grid[0].Length;
            int result = 0;
            for(var i = 0;i< xLength;i++)
            {
                for(var j = 0;j < yLength;j++)
                {
                    if(grid[i][j] == '1')
                    {
                        result++;
                        MarkInDfs(i,j,grid);
                    }
                }
            }
            return result;
            
        }
        private void MarkInDfs(int x,int y,char[][] grid)
        {
            if (x < 0 || y < 0 || x >= xLength || y >= yLength || grid[x][y] == '0' )
            {
                return;
            }
            grid[x][y] = '0';
            MarkInDfs(x + 1,y,grid);
            MarkInDfs(x - 1,y,grid);
            MarkInDfs(x,y + 1,grid);
            MarkInDfs(x,y - 1,grid);
        }

        //  方法一：深度优先搜索
        // 将二维网格看成一个无向图，竖直或水平相邻的 1 之间有边。

        // 算法
        // 线性扫描整个二维网格，如果一个结点包含 1，则以其为根结点启动深度优先搜索。
        // 在深度优先搜索过程中，每个访问过的结点被标记为 0。
        // 计数启动深度优先搜索的根结点的数量，即为岛屿的数量。

        private void DfsMark(char[][] grid, int r, int c) 
        {
            int nr = grid.Length;
            int nc = grid[0].Length;

            if (r < 0 || c < 0 || r >= nr || c >= nc || grid[r][c] == '0') 
            {
                return;
            }

            grid[r][c] = '0';
            DfsMark(grid, r - 1, c);
            DfsMark(grid, r + 1, c);
            DfsMark(grid, r, c - 1);
            DfsMark(grid, r, c + 1);
        }

        public int NumIslands1(char[][] grid) 
        {
            if (grid == null || grid.Length == 0) 
            {
                return 0;
            }

            int nr = grid.Length;
            int nc = grid[0].Length;
            int num_islands = 0;
            for (int r = 0; r < nr; ++r) 
            {
                for (int c = 0; c < nc; ++c) 
                {
                    if (grid[r][c] == '1') 
                    {
                    ++num_islands;
                    DfsMark(grid, r, c);
                    }
                }
            }
            return num_islands;
        }

        // 方法二: 广度优先搜索 

        // 线性扫描整个二维网格，如果一个结点包含 1，则以其为根结点启动广度优先搜索。
        // 将其放入队列中，并将值设为 0 以标记访问过该结点。迭代地搜索队列中的每个结点，直到队列为空。
        public int NumIslands2(char[][] grid) 
        {
            if (grid == null || grid.Length == 0) 
            {
                return 0;
            }

            int nr = grid.Length;
            int nc = grid[0].Length;
            int num_islands = 0;

            for (int r = 0; r < nr; ++r) 
            {
                for (int c = 0; c < nc; ++c) 
                {
                    if (grid[r][c] == '1') 
                    {
                        ++num_islands;
                        grid[r][c] = '0'; // mark as visited
                        Queue<int> neighbors = new Queue<int>();
                        neighbors.Enqueue(r * nc + c);
                        while (neighbors.Count != 0) 
                        {
                            int id = neighbors.Dequeue();
                            int row = id / nc;
                            int col = id % nc;
                            if (row - 1 >= 0 && grid[row-1][col] == '1') 
                            {
                                neighbors.Enqueue((row-1) * nc + col);
                                grid[row-1][col] = '0';
                            }
                            if (row + 1 < nr && grid[row+1][col] == '1') 
                            {
                                neighbors.Enqueue((row+1) * nc + col);
                                grid[row+1][col] = '0';
                            }
                            if (col - 1 >= 0 && grid[row][col-1] == '1') 
                            {
                                neighbors.Enqueue(row * nc + col-1);
                                grid[row][col-1] = '0';
                            }
                            if (col + 1 < nc && grid[row][col+1] == '1') 
                            {
                                neighbors.Enqueue(row * nc + col+1);
                                grid[row][col+1] = '0';
                            }
                        }
                    }
                }
            }
            return num_islands;
        }

        //并查集
        class UnionFind 
        {
            int count; // # of connected components
            int[] parent;
            int[] rank;

            public UnionFind(char[][] grid) 
            { 
                count = 0;
                int m = grid.Length;
                int n = grid[0].Length;
                parent = new int[m * n];
                rank = new int[m * n];
                for (int i = 0; i < m; ++i) 
                {
                    for (int j = 0; j < n; ++j) {
                    if (grid[i][j] == '1') 
                    {
                        parent[i * n + j] = i * n + j;
                        ++count;
                    }
                    rank[i * n + j] = 0;
                    }
                }
            }

            public int find(int i) 
            { // path compression
                if (parent[i] != i) 
                    parent[i] = find(parent[i]);
                return parent[i];
            }

            public void union(int x, int y) 
            { // union with rank
                int rootx = find(x);
                int rooty = find(y);
                if (rootx != rooty) 
                {
                    if (rank[rootx] > rank[rooty]) 
                    {
                        parent[rooty] = rootx;
                    } 
                    else if (rank[rootx] < rank[rooty]) 
                    {
                        parent[rootx] = rooty;
                    } 
                    else 
                    {
                        parent[rooty] = rootx; rank[rootx] += 1;
                    }
                    --count;
                }
            }

            public int getCount() 
            {
                return count;
            }
        }

        public int NumIslands3(char[][] grid) 
        {
            if (grid == null || grid.Length == 0) 
            {
                return 0;
            }

            int nr = grid.Length;
            int nc = grid[0].Length;
            int num_islands = 0;
            UnionFind uf = new UnionFind(grid);
            for (int r = 0; r < nr; ++r) 
            {
                for (int c = 0; c < nc; ++c) 
                {
                    if (grid[r][c] == '1') 
                    {
                        grid[r][c] = '0';
                        if (r - 1 >= 0 && grid[r-1][c] == '1')
                        {
                            uf.union(r * nc + c, (r-1) * nc + c);
                        }
                        if (r + 1 < nr && grid[r+1][c] == '1') 
                        {
                            uf.union(r * nc + c, (r+1) * nc + c);
                        }
                        if (c - 1 >= 0 && grid[r][c-1] == '1') 
                        {
                            uf.union(r * nc + c, r * nc + c - 1);
                        }
                        if (c + 1 < nc && grid[r][c+1] == '1') 
                        {
                            uf.union(r * nc + c, r * nc + c + 1);
                        }
                    }
                }
            }
            return uf.getCount();
        }


        // 方法三：并查集 
        // 遍历二维网格，将竖直或水平相邻的陆地联结。最终，返回并查集数据结构中相连部分的数量。 





    }
}




