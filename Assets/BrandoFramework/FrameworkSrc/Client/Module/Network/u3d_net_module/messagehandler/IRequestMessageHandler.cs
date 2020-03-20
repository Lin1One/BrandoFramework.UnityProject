
namespace client_module_network
{
    /// <summary>
    /// 消息请求处理器。
    /// 负责构建发送给服务器的消息实例。
    /// </summary>
    public interface IRequestMessageHandler : IMessageHandler
    {
        byte[] GetMessage(object obj);
    }
}

