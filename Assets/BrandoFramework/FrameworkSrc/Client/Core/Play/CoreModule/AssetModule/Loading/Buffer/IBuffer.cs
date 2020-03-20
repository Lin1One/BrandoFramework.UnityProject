#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion


using System;

namespace Client.Core
{
    public interface IBuffer<TKey, TValue>
    {
        bool HasValue(TKey key);

        void CheckAanExecute(TKey key, Action<TValue> onExist, Action<TKey> onNull);

        bool TryCache(TKey key, TValue value);

        bool TryRemove(TKey key);

        TValue GetValue(TKey key);
    }
}
