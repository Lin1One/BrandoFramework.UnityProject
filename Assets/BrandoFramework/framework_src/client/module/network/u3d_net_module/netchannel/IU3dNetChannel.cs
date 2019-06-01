using System;

namespace client_module_network
{
    public interface IU3dNetChannel
    {
        string ChannelId { get; set; }

        NetServiceType ServiceType { get; }

        IMessageStorage MessageStorage { get; }

        void OnConnect(Action<IU3dNetChannel> callback);

        void OnReconnect(Action<IU3dNetChannel> callback);

        void OnDisconnect(Action<IU3dNetChannel> callback);

        void OnClose(Action<IU3dNetChannel> callback);

        void AddUpHandler(IMessageUpHandler upHandler);

        void AddDownHandler(IMessageDownHandler downHandler);

        void BeginConnect(string url, int port);

        void BeginSend(byte[] bytes);

        void Close();
    }
}
