
namespace client_module_network
{
    /// <summary>
    /// 网络消息业务处理器。
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// 处理器可以处理的消息编号。
        /// </summary>
        int MessageId { get; }
    }
}

