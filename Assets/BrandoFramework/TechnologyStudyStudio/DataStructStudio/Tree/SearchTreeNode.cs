using System;
using System.Collections.Generic;

namespace DataStructStudio
{
    /// <summary>
    /// 二叉树节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchTreeNode : TreeNode<int>
    {
        private int offset;      //位置

        public SearchTreeNode(int val,int offset)
        {
            Valve = val;
            this.offset = offset;
        }

        //位置属性
        public int Offset
        {
            get { return offset; }
            set { offset = value; }
        }
    }    
}


