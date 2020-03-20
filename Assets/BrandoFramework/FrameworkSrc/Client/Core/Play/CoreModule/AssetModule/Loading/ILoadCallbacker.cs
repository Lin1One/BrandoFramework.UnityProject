#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using System;
namespace Client.Core
{
    public interface ILoadCallbcker<TKey, TValue> where TValue : class
    {
        void Callback(TKey key, TValue value);

        void AddCallback(TKey key, Action<TValue> callback);
    }
}
