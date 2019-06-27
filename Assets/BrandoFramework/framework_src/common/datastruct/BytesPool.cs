#region Head

// Author:            Yu
// CreateDate:        2018/10/24 17:55:30
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System.Collections.Generic;

namespace Common
{

    /// <summary>
    /// 字节数组缓存对象池。
    /// </summary>
    public static class BytesPool
    {
        public static byte[] Take(int length)
        {
            if (!BytesPoolDict.ContainsKey(length))
            {
                BytesPoolDict.Add(length, new ObjectPool<byte[]>(
                    () => new byte[length], 1));
            }

            var targetPool = BytesPoolDict[length];
            var bytes = targetPool.Take();
            return bytes;
        }

        public static void Restore(byte[] bytes)
        {
            var targetPool = BytesPoolDict[bytes.Length];
            targetPool.Restore(bytes);
        }

        private static Dictionary<int, IObjectPool<byte[]>> bytesDictPool;

        private static Dictionary<int, IObjectPool<byte[]>> BytesPoolDict
        {
            get
            {
                if (bytesDictPool != null)
                {
                    return bytesDictPool;
                }

                bytesDictPool = new Dictionary<int, IObjectPool<byte[]>>();
                return bytesDictPool;
            }
        }
    }
}
