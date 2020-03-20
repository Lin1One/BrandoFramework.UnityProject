

using Common;

namespace client_module_network
{
    /// <summary>
    /// 通信下行阶段业务处理器。
    /// </summary>
    public interface IMessageDownHandler
    {
        /// <summary>
        /// 对socket通信消息的字节数组进行业务处理。
        /// </summary>
        /// <param name="byteBuf"></param>
        byte[] Handle(IByteBuf byteBuf);
    }
}



