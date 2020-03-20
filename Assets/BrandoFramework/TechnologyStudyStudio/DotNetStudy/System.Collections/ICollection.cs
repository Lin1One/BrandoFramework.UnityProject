#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.DotNet.System.Collections
{
    /// <summary>
    /// 集合接口
    /// </summary>
    public interface ICollection : IEnumerable
    {
        /// <summary>
        /// 拷贝至数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void CopyTo(Array array, int index);

        int Count{ get; }

        /// <summary>
        /// 返回一个 Object 用于同步，可用于加锁
        /// </summary>
        Object SyncRoot{ get; }

        bool IsSynchronized{ get; }
    }
}

