#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System.Collections.Generic;

namespace Client.LegoUI
{
    internal static class YuLegoListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly YuLegoObjectPool<List<T>> s_ListPool =
            new YuLegoObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}