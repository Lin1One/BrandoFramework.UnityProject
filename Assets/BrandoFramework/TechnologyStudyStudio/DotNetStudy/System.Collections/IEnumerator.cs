#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using System;

namespace Study.DotNet.System.Collections
{
    /// <summary>
    /// ���������ṩ�򵥵ĵ�������Ԫ�صķ���
    /// </summary>
    public interface IEnumerator
    {
        /// <summary>
        /// ��������������һԪ��
        /// </summary>
        bool MoveNext();

        /// <summary>
        /// ���ص�ǰԪ��
        /// </summary>
        Object Current { get; }

        /// <summary>
        /// ���õ���������ʼλ��
        /// </summary>
        void Reset();
    }
}

