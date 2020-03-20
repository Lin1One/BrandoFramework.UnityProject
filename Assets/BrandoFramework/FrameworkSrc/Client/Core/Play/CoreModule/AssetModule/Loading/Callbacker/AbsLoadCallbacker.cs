 

using System;
using System.Collections.Generic;


#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

namespace Client.Core
{
    public abstract class AbsLoadCallbacker<TKey, TValue> : ILoadCallbcker<TKey, TValue>
        where TValue : class
    {
        protected Dictionary<TKey, LinkedList<Action<TValue>>> Callbackers { get; }
            = new Dictionary<TKey, LinkedList<Action<TValue>>>();

        public void Callback(TKey key, TValue value)
        {
            if (!Callbackers.ContainsKey(key))
            {
                return;
            }

            var callbackers = Callbackers[key];

            foreach (var callbacker in callbackers)
            {
                callbacker(value);
            }
            
//            Callbackers[key].ForEach(c => c(value));
            Callbackers.Remove(key);
        }

        private LinkedList<Action<TValue>> CreateLink(Action<TValue> c)
        {
            var link = new LinkedList<Action<TValue>>();
            link.AddLast(c);
            return link;
        }

        public void AddCallback(TKey key, Action<TValue> callback)
        {
            if (!Callbackers.ContainsKey(key))
            {
                Callbackers.Add(key, CreateLink(callback));
            }
            else
            {
                Callbackers[key].AddLast(callback);
            }
        }
    }
}
