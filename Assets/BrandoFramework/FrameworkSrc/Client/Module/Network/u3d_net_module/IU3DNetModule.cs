using System;
using System.Collections.Generic;

namespace client_module_network
{
    /// <summary>
    /// Unity3d 网络模块
    /// </summary>
    public interface IU3DNetModule
    {
        Dictionary<string, IU3dNetChannel> ChannelDic { get; }
        /// <summary>
        /// 向指定的通信通道发送消息
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="bytes"></param>
        void SendMessage(string channelId, int messageId, object obj = null);

        /// <summary>
        /// 向默认通信频道发送消息
        /// </summary>
        /// <param name="bytes"></param>
        void SendMessage(int messageId, object obj = null);

        /// <summary>
        /// 关闭指定的通信频道
        /// </summary>
        /// <param name="channelId"></param>
        void CloseChannel(string channelId);

        /// <summary>
        /// 关闭默认通信频道
        /// </summary>
        /// <param name="channelId"></param>
        void ColseChannel();

        /// <summary>
        /// 注册一个新的网络通信频道
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="serviceType"></param>
        /// <param name="onConnect"></param>
        /// <param name="onReConnect"></param>
        /// <param name="onDisConnect"></param>
        /// <param name="onClose"></param>
        /// <returns></returns>
        IU3dNetChannel RegisterNetChannel(
            string channelId,
            NetServiceType serviceType,
            Action<IU3dNetChannel> onConnect = null,
            Action<IU3dNetChannel> onReConnect = null,
            Action<IU3dNetChannel> onDisConnect = null,
            Action<IU3dNetChannel> onClose = null
            );
    }
}
