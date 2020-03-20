#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.DotNet.System.Collections
{
    /// <summary>
    /// ���Ͻӿ�
    /// </summary>
    public interface ICollection : IEnumerable
    {
        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void CopyTo(Array array, int index);

        int Count{ get; }

        /// <summary>
        /// ����һ�� Object ����ͬ���������ڼ���
        /// </summary>
        Object SyncRoot{ get; }

        bool IsSynchronized{ get; }
    }
}

