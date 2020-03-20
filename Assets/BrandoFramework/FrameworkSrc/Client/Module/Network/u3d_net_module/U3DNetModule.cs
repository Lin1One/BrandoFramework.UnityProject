using System;
using System.Collections.Generic;

namespace client_module_network
{
    public class U3DNetModule : IU3DNetModule
    {
        private NetServiceType netServiceType;

        /// <summary>
        /// 当前默认通信频道
        /// </summary>
        private IU3dNetChannel currentChannel;

        private IMessageDispatcher messageDispatcher;

        public Dictionary<string, IU3dNetChannel> ChannelDic { get; } = 
            new Dictionary<string, IU3dNetChannel>();

        #region 外部调用 API

        #region 发送

        public void SendMessage(string channelId, int messageId, object obj = null)
        {
            var bytes = messageDispatcher.GetRequestMessageBytes(messageId, obj);
            SendBytesMessage(channelId, bytes);
        }

        public void SendMessage(int messageId, object obj = null)
        {
            var bytes = messageDispatcher.GetRequestMessageBytes(messageId, obj);
            SendBytesMessage(bytes);
        }

        private void SendBytesMessage(string channelId, byte[] bytes)
        {
            if (!ChannelDic.ContainsKey(channelId))
            {
                throw new Exception($"目标频道{channelId} 不存在");
            }
            var channel = ChannelDic[channelId];
            channel.BeginSend(bytes);
        }

        private void SendBytesMessage(byte[] bytes)
        {
            if(currentChannel == null)
            {
                throw new Exception($"当前频道为空");
            }
            currentChannel.BeginSend(bytes);
        }

        #endregion

        #region 断开
        public void CloseChannel(string channelId)
        {
#if UNITY_EDITOR
            if (!ChannelDic.ContainsKey(channelId))
            {
                throw new Exception($"目标频道{channelId}不存在！");
            }
#endif

            var channel = ChannelDic[channelId];
            channel.Close();
            ChannelDic.Remove(channelId);
        }

        public void ColseChannel()
        {
            CloseChannel(currentChannel.ChannelId);
        }

        #endregion

        public IU3dNetChannel RegisterNetChannel(string channelId,
            NetServiceType serviceType, 
            Action<IU3dNetChannel> onConnect, 
            Action<IU3dNetChannel> onReConnect, 
            Action<IU3dNetChannel> onDisConnect, 
            Action<IU3dNetChannel> onClose)
        {
            if(ChannelDic.ContainsKey(channelId))
            {
                return ChannelDic[channelId];
            }
            else
            {
                switch(serviceType)
                {
                    case NetServiceType.Socket:
                        var socketChannel = new U3dNetChannelOriginSocket();
                        socketChannel.OnConnect(onConnect);
                        socketChannel.OnReconnect(onReConnect);
                        socketChannel.OnDisconnect(onDisConnect);
                        socketChannel.OnClose(onClose);
                        return socketChannel;
                    default:
                        return null;
                }
            }
        }

        #endregion

    }
}

