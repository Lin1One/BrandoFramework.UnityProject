
#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Client.Core
{
    public abstract class AbsBuffer<TKey, TValue> : IBuffer<TKey, TValue>
    {
        protected Dictionary<TKey, TValue> Maps { get; }
            = new Dictionary<TKey, TValue>();

        public bool HasValue(TKey key) => Maps.ContainsKey(key);

        public void CheckAanExecute(TKey key, Action<TValue> onExist, Action<TKey> onNull)
        {
            if (HasValue(key))
            {
                onExist(Maps[key]);
            }
            else
            {
                onNull(key);
            }
        }

        public bool TryCache(TKey key, TValue value) => false;
        public bool TryRemove(TKey key) => Maps.Remove(key);
        public TValue GetValue(TKey key) => Maps[key];
    }
}
