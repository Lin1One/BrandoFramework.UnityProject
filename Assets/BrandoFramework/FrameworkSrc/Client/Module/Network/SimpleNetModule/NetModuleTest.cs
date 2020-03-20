using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Encoder = System.Text.Encoding;
namespace client_module_network
{
    public class NetModuleTest : MonoBehaviour
    {

        public InputField input;
        public Text textControll;

        //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private void Start()
        {
            NetModule.AddEventListener(NetEvent.ConnectSucc, OnConnectSucc);
            NetModule.AddEventListener(NetEvent.ConnectFail, OnConnectFail);
            NetModule.AddEventListener(NetEvent.Close, OnConnectClose);
        }


        private void Update()
        {
            //textControll.text = recvStr;

            //Poll 收方法
            //if (socket == null)
            //{
            //    return;
            //}
            //if (socket.Poll(0, SelectMode.SelectRead))
            //{
            //    byte[] readBuff = new byte[1024];
            //    int count = socket.Receive(readBuff);
            //    string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            //    textControll.text = recvStr;
            //}

        }

        //玩家点击连接按钮
        public void OnConnectClick ()
        {
            NetModule.Connect("127.0.0.1",8888);
            //TODO:开始转圈圈，提示“连接中”    
        }

        //主动关闭
        public void OnCloseClick ()
        {
            NetModule.Close();
        }

        //连接成功回调    
        void OnConnectSucc(string err)
        {
            Debug.Log("OnConnectSucc");
            //TODO:进入游戏    
        }

        //连接失败回调    
        void OnConnectFail(string err)
        {
            Debug.Log("OnConnectFail" + err);
            //TODO:弹出提示框(连接失败，请重试)    
        }

        //关闭连接
        void OnConnectClose(string err)
        {
            Debug.Log("OnConnectClose");
            //TODO:弹出提示框(网络断开)        
            //TODO:弹出按钮(重新连接)    
        } 

        //#region Connect
        //public void ConnectedToServer()
        //{
            
        //    socket.Connect("127.0.0.1", 8888);
        //}

        //public void AsyncConnectedToServer()
        //{
            
        //    socket.BeginConnect("127.0.0.1", 8888, OnConnect, socket);
        //}

        //private void OnConnect(IAsyncResult ar)
        //{
        //    try
        //    {
        //        var connectedSocket = (Socket)ar.AsyncState;
        //        connectedSocket.EndConnect(ar);
        //        Debug.Log($"socket 连接成功");
        //        connectedSocket.BeginReceive(readBuff, 0, 1024,0,OnRecive, connectedSocket);
        //    }
        //    catch(SocketException ex)
        //    {
        //        Debug.Log($"socket 连接失败，报错为{ex.ToString()}");
        //    }
        //}

        //#endregion

        //#region Recive

        //byte[] readBuff = new byte[1024];
        //string recvStr = "";

        //public void ReceiveMessage()
        //{
        //    byte[] readBuff = new byte[1024];
        //    int count = socket.Receive(readBuff);
        //    string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
        //    textControll.text = recvStr;
        //    //CloseConnect();
        //}

        //private void OnRecive(IAsyncResult ar)
        //{
        //    Socket recevieSocket = (Socket)ar.AsyncState;
        //    var count = recevieSocket.EndReceive(ar);
        //    recvStr = Encoder.Default.GetString(readBuff);
        //    //textControll.text = recvStr;

        //    recevieSocket.BeginReceive(readBuff, 0, 1024, 0, OnRecive, recevieSocket);
        //}

        //#endregion

        //#region Send
        //public void SendMessage()
        //{
        //    string str = input.text;
        //    byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        //    socket.Send(bytes);
        //    ReceiveMessage();
        //}

        //public void Send()
        //{

        //    string sendStr = input.text;
        //    byte[] bytes = Encoder.Default.GetBytes(sendStr);
        //    socket.BeginSend(bytes,0, bytes.Length,0, SendCallback,socket);
        //}

        //public void SendCallback(IAsyncResult ar)
        //{
        //    //EndSend 的 返回值 count 与要发送数据的长度相同，代表数据全部发出。
        //    //如果EndSend的返回值指示未全部发完，需要再次调用BeginSend方法，以便发送未发送的数据

        //    var socket = (Socket) ar.AsyncState;
        //    var count = socket.EndSend(ar);
        //}
        //#endregion

        //public void CloseConnect()
        //{
        //    socket.Close();
        //}
    }
}


