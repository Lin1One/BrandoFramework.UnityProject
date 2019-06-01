#region Head

// Author:                liuruoyu1981
// CreateDate:            5/16/2019 10:40:29 PM
// Email:                 liuruoyu1981@gmail.com

#endregion


using System;

namespace Client.Assets
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
