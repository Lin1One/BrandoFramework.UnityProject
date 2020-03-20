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

            //Poll �շ���
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

        //��ҵ�����Ӱ�ť
        public void OnConnectClick ()
        {
            NetModule.Connect("127.0.0.1",8888);
            //TODO:��ʼתȦȦ����ʾ�������С�    
        }

        //�����ر�
        public void OnCloseClick ()
        {
            NetModule.Close();
        }

        //���ӳɹ��ص�    
        void OnConnectSucc(string err)
        {
            Debug.Log("OnConnectSucc");
            //TODO:������Ϸ    
        }

        //����ʧ�ܻص�    
        void OnConnectFail(string err)
        {
            Debug.Log("OnConnectFail" + err);
            //TODO:������ʾ��(����ʧ�ܣ�������)    
        }

        //�ر�����
        void OnConnectClose(string err)
        {
            Debug.Log("OnConnectClose");
            //TODO:������ʾ��(����Ͽ�)        
            //TODO:������ť(��������)    
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
        //        Debug.Log($"socket ���ӳɹ�");
        //        connectedSocket.BeginReceive(readBuff, 0, 1024,0,OnRecive, connectedSocket);
        //    }
        //    catch(SocketException ex)
        //    {
        //        Debug.Log($"socket ����ʧ�ܣ�����Ϊ{ex.ToString()}");
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
        //    //EndSend �� ����ֵ count ��Ҫ�������ݵĳ�����ͬ����������ȫ��������
        //    //���EndSend�ķ���ֵָʾδȫ�����꣬��Ҫ�ٴε���BeginSend�������Ա㷢��δ���͵�����

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


