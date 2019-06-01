using client_common;
using System;
using System.Collections.Generic;

namespace client_module_network
{
    public abstract class U3dAbsNetChannel: IU3dNetChannel
    {
        public string ChannelId { get; set; }

        public NetServiceType ServiceType { get; }

        protected string _url;

        protected int _port;

        /// <summary>
        /// 
        /// </summary>
        protected byte[] receiveDataBuffer { get; } = new byte[1024];

        protected IByteBuf readReceiveDataBuf { get; } = new ByteBuf(40960);

        protected NetState state;

        /// <summary>
        /// 消息仓库
        /// </summary>
        private IMessageStorage messageStorage;
        public IMessageStorage MessageStorage =>
            messageStorage ?? (messageStorage = new MessageStorage());

        /// <summary>
        /// 当前连接次数。
        /// 从应用启动到现在一共成功建立的连接次数。
        /// </summary>
        protected int ConnectCount { get; set; }

        public abstract void BeginConnect(string url, int port);

        public abstract void BeginSend(byte[] bytes);

        public abstract void Close();


        #region 通信频道生命周期 

        protected Action<IU3dNetChannel> onConnect;

        protected Action<IU3dNetChannel> onReConnect;

        protected Action<IU3dNetChannel> onDisConnect;

        protected Action<IU3dNetChannel> onClose;

        public void OnConnect(Action<IU3dNetChannel> callback)
        {
            onConnect = callback;
        }

        public void OnReconnect(Action<IU3dNetChannel> callback)
        {
            onReConnect = callback;
        }

        public void OnDisconnect(Action<IU3dNetChannel> callback)
        {
            onDisConnect = callback;
        }

        public void OnClose(Action<IU3dNetChannel> callback)
        {
            onClose = callback;
        }

        #endregion

        /// <summary>
        /// 上行数据加工器
        /// </summary>
        protected List<IMessageUpHandler> UpHandlers { get; }
            = new List<IMessageUpHandler>(1);

        /// <summary>
        /// 下行数据加工器
        /// </summary>
        protected List<IMessageDownHandler> DownHandlers { get; }
            = new List<IMessageDownHandler>(1);

        public void AddUpHandler(IMessageUpHandler upHandler)
        {
            UpHandlers.Add(upHandler);
        }

        public void AddDownHandler(IMessageDownHandler downHandler)
        {
            DownHandlers.Add(downHandler);
        }

    }
}
