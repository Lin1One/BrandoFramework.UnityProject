using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace client_module_network
{
    /// <summary>
    /// C# 原生 Socket
    /// </summary>
    public class U3dNetChannelOriginSocket : U3dAbsNetChannel
    {
        private Socket socket;

        private Thread thead;

        private AutoResetEvent TimeoutEvent { get; } = new AutoResetEvent(false);

        #region 连接

        public override void BeginConnect(string url, int port)
        {
            _url = url;
            _port = port;
            BeginConnectOnThread();
        }

        private void BeginConnectOnThread()
        {
            thead?.Abort();
            thead = null;
            thead = new Thread(ConnectAtThread);
            thead.Start();
        }

        private void ConnectAtThread()
        {
#if UNITY_EDITOR
            switch (state)
            {
                case NetState.Connecting:
                    Debug.Log($"socket频道{ChannelId}正在连接！");
                    return;
                case NetState.Connected:
                    Debug.Log($"socket频道{ChannelId}已连接！");
                    return;
            }
#endif
            // 开始连接
            state = NetState.Connecting;
            var addresses = Dns.GetHostAddresses(_url);
            var firstAddress = addresses[0];
            socket = firstAddress.AddressFamily == AddressFamily.InterNetworkV6
                ? new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp)
                : new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.BeginConnect(_url, _port, OnConnected, socket);

            // 处理连接失败
            if (TimeoutEvent.WaitOne(3000))
            {
                return;
            }

            Close();
            //EventModule.TriggerEvent(YuUnityEventCode.NetConnectTimeOut);
#if UNITY_EDITOR
            Debug.LogError($"通信频道{ChannelId}连接超时！");
#endif
        }

        private void OnConnected(IAsyncResult ar)
        {
            socket.EndConnect(ar);
            TimeoutEvent.Set();
            ConnectCount++;
            state = NetState.Connected;

            // 发送消息
            // 注册物理时间帧循环事件每帧尝试
            //EventModule.WatchUnityEvent(YuUnityEventType.FixedUpdate, SendLoop);

            // 接收消息
            socket.BeginReceive(receiveDataBuffer, 0, receiveDataBuffer.Length,
                SocketFlags.None, OnReceive, socket);
            //EventModule.TriggerEvent(ConnectCount == 1
            //        ? YuUnityEventCode.NetOnFirstConnected
            //        : YuUnityEventCode.NetReConnected
            //);

            //EventModule.TriggerEvent(YuUnityEventCode.NetConnectSucceed);
            onConnect?.Invoke(this);
        }


        #endregion

        #region 发送

        public override void BeginSend(byte[] bytes)
        {
            MessageStorage.PushSendMessage(bytes);
        }

        /// <summary>
        /// 内部发送循环
        /// </summary>
        private void SendLoop()
        {
            if (MessageStorage.SendCount <= 0)
            {
                return;
            }
            var bytes = MessageStorage.GetSendMessage();
            foreach (var handler in UpHandlers)
            {
                bytes = handler.Handle(bytes);
            }

            socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, OnSended, socket);
        }

        private void OnSended(IAsyncResult ar)
        {
            socket.EndSend(ar);
        }

        #endregion

        #region 断开

        public override void Close()
        {
            if (state !=  NetState.Connected)
            {
                return;
            }

            socket.Close();
            socket = null;
            //EventModule.RemoveUnityEvent(YuUnityEventType.FixedUpdate, SendLoop);
            state = NetState.NotConnect;
            onClose?.Invoke(this);
        }

        #endregion

        #region 接收

        private void OnReceive(IAsyncResult ar)
        {

            var receiveLength = socket.EndReceive(ar);
            if (receiveLength == 0)
            {
                Close();
                onClose?.Invoke(this);
                return;
            }

            readReceiveDataBuf.WriteBytes(receiveDataBuffer, receiveLength);
            LoopParseReceiveBytes();
            socket.BeginReceive(receiveDataBuffer, 0, receiveDataBuffer.Length,
                SocketFlags.None, OnReceive, socket);
        }

        protected void LoopParseReceiveBytes()
        {
            while (true)
            {
                foreach (var downHandler in DownHandlers)
                {
                    downHandler.Handle(readReceiveDataBuf);
                    if (readReceiveDataBuf.LastBytes != null)
                    {
                        continue;
                    }
                    return;
                }

                MessageStorage.PushReceiveMessage(readReceiveDataBuf.LastBytes);
                readReceiveDataBuf.LastBytes = null;
            }
        }

        #endregion

    }
}
