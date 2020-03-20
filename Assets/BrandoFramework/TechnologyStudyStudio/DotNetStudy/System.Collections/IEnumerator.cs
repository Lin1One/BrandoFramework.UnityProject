#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.DotNet.System.Collections
{
    /// <summary>
    /// 迭代器，提供简单的迭代集合元素的方法
    /// </summary>
    public interface IEnumerator
    {
        /// <summary>
        /// 迭代器迭代至下一元素
        /// </summary>
        bool MoveNext();

        /// <summary>
        /// 返回当前元素
        /// </summary>
        Object Current { get; }

        /// <summary>
        /// 重置迭代器至初始位置
        /// </summary>
        void Reset();
    }
}

