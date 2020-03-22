 

using System;

 

#region Head

// Author:                LinYuzhou
// CreateDate:            2019/5/16 21:05:25
// Email:                 836045613@qq.com

#endregion


namespace YuU3dPlay
{
    public interface ILoadCallbcker<TKey, TValue> where TValue : class
    {
        void Callback(TKey key, TValue value);

        void AddCallback(TKey key, Action<TValue> callback);
    }
}
