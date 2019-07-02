using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Common.Utility
{
    /// <summary>
    /// 序列化工具。
    /// </summary>
    public static class SerializeUtility
    {
              /// <summary>
        /// C#二进制序列化。
        /// </summary>
        /// <returns>The serialize.</returns>
        /// <param name="value">Value.</param>
        public static byte[] Serialize(object value)
        {
            if (value == null) return null;

            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                var bytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, bytes,
                    0, (int) ms.Length);
                return bytes;
            }
        }

        public static void SerializeAndWriteTo(object obj, string path)
        {
            var bytes = Serialize(obj);
            IOUtility.WriteAllBytes(path, bytes);
        }

        public static T Get<T>(string path) where T : class, new()
        {
            var bytes = File.ReadAllBytes(path);
            var instance = DeSerialize<T>(bytes);
            return instance;
        }

        /// <summary>
        /// C#二进制反序列化。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(byte[] value) where T : class, new()
        {
            if (value == null) return default(T);

            using (var ms = new MemoryStream(value))
            {
                var bf = new BinaryFormatter();
                var instance = (T) bf.Deserialize(ms);
                return instance;
            }
        }

        /// <summary>
        /// 反序列化目标路径上的文件。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string path) where T : class, new()
        {
            var bytes = File.ReadAllBytes(path);
            var result = DeSerialize<T>(bytes);
            return result;
        }

        /// <summary>
        /// 获得目标数据的一个克隆副本。
        /// 克隆副本和原数据将完全无关。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetCopy<T>(T source) where T : class, new()
        {
            var bytes = Serialize(source);
            var copy = DeSerialize<T>(bytes);
            return copy;
        }
    }
}


