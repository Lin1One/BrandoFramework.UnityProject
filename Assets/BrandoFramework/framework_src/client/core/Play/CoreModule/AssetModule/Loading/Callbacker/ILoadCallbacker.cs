 

using System;

 

#region Head

// Author:                liuruoyu1981
// CreateDate:            2019/5/16 21:05:25
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace YuU3dPlay
{
    public interface ILoadCallbcker<TKey, TValue> where TValue : class
    {
        void Callback(TKey key, TValue value);

        void AddCallback(TKey key, Action<TValue> callback);
    }
}