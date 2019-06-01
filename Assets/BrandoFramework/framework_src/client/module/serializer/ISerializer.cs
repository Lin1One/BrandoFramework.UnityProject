
namespace client_common
{
    /// <summary>
    /// 序列化器。
    /// 负责项目中的程序实例的序列化和反序列化。
    /// 1. 网络消息。
    /// 2. 配置表。
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 将一个消息对象序列化为字节数组。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Serialize(object message);

        /// <summary>
        /// 将字节数组反序列化为一个消息对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T DeSerialize<T>(byte[] bytes) where T : class, new();
    }
}
