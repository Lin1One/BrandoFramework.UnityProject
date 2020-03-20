

namespace client_module_network
{
    /// <summary>
    /// socket通信调度器。
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// 获取指定编号的目标协议消息字节数组。
        /// </summary>
        /// <param name="messageId">协议编号。</param>
        /// <param name="obj">需要传递的数据。</param>
        byte[] GetRequestMessageBytes(int messageId, object obj = null);

        #region 注册网络消息处理器

        void AddRespHandler(int messageId, IRespMessageHandler handler);

        void AddRequestHandler(int messageId, IRequestMessageHandler handler);

        /// <summary>
        /// 重定向答复消息处理的调度关系。
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        void RedirectRespHandle(int sourceId, int targetId);

        #endregion

        #region 开启和关闭消息调度

        void StopDispatch();
        void OpenDispatch();

        #endregion
    }
}