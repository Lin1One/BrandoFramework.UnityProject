#region Head

// Author:            Yu
// CreateDate:        2018/10/24 20:08:12
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace client_module_network
{
    /// <summary>
    /// socket通信调度器。
    /// </summary>
    public class MessageDispatcher : IMessageDispatcher
    {
        private bool isDispatch;

        private readonly IU3DNetModule netModule;
        //private readonly IYuU3dAppEntity appEntity;

        private readonly Dictionary<int, IRespMessageHandler> respHandlerDic
            = new Dictionary<int, IRespMessageHandler>();

        private readonly Dictionary<int, IRequestMessageHandler> requestHandlerDic
            = new Dictionary<int, IRequestMessageHandler>();

        private Dictionary<int, int> _redirectResp;

        public Dictionary<int, int> RedirectResp => _redirectResp ?? (_redirectResp = new Dictionary<int, int>());

        public void StopDispatch() => isDispatch = false;

        public void OpenDispatch() => isDispatch = true;

        public MessageDispatcher()
        {
            //socketModule = YuU3dAppUtility.Injector.Get<IYuU3dSocketModule>();
            //var eventModule = YuU3dAppUtility.Injector.Get<IYuU3DEventModule>();
            //eventModule.WatchUnityEvent(YuUnityEventType.FixedUpdate, MessageDispatch);
            //eventModule.WatchEvent(YuUnityEventCode.NetSendMessageEnqueue, OpenDispatch);
            //eventModule.WatchEvent(YuUnityEventCode.Net_PauseMessageHandle, StopDispatch);
        }


        #region 注册网络消息处理器

        public void AddRespHandler(int messageId, IRespMessageHandler handler)
        {
            if (!respHandlerDic.ContainsKey(messageId))
            {
                respHandlerDic.Add(messageId, handler);
            }
            else
            {
                Debug.LogError($"消息号为 {messageId} 的处理器重复注册");
            }
        }

        public void AddRequestHandler(int messageId, IRequestMessageHandler handler)
        {
            if (!requestHandlerDic.ContainsKey(messageId))
            {
                requestHandlerDic.Add(messageId,handler);
            }
            else
            {
                Debug.LogError($"消息号为 {messageId} 的处理器重复注册");
            }
        }

        #endregion


        #region 消息分发
        /// <summary>
        /// 接收消息分发方法，在 Unity FixedUpdate 中调用
        /// </summary>
        private void MessageDispatch()
        {
            foreach (var channel in netModule.ChannelDic.Values)
            {
                if (channel.MessageStorage.ReceiveCount == 0)
                {
                    continue;
                }

                for (int i = 0; i < channel.MessageStorage.ReceiveCount; i++)
                {
                    var bytes = channel.MessageStorage.GetReceiveMessage();
                    var messageIdBytes = bytes.GetHeadBytes();
                    var messageId = BitConverter.ToInt32(messageIdBytes, 0);
                    var messageBodyLength = bytes.Length - 4;
                    var finalBytes = BytesPool.Take(messageBodyLength);
                    Buffer.BlockCopy(bytes, 4, finalBytes, 0, messageBodyLength);
                    BytesPool.Restore(bytes);
                    InvokeRespHandler(messageId, finalBytes);
                }
            }
        }

        /// <summary>
        /// 调用消息对应的消息处理器
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        private void InvokeRespHandler(int messageId, byte[] message)
        {     
            var respHandler = respHandlerDic[messageId];

            var redicretId = -1;
            redicretId = RedirectResp.ContainsKey(messageId) ? RedirectResp[messageId] : messageId;

            if (!respHandlerDic.ContainsKey(redicretId))
            {
#if DEBUG
                Debug.LogError($"消息编号{messageId}的消息没有对应的答复处理器存在！");
#endif
                return;
            }

            var handler = respHandlerDic[redicretId];
            handler.ReceiveMessage(message);
            BytesPool.Restore(message);
        }

        public void RedirectRespHandle(int sourceId, int targetId)
        {
            if (RedirectResp.ContainsKey(sourceId))
            {
                return;
            }

            RedirectResp.Add(sourceId, targetId);
        }

        #endregion

        #region 编辑器初始化 反射获取并注册通信处理器实例

#if DEBUG

        private Dictionary<string, Type> requestTypeDict
            =  new Dictionary<string, Type>();

        private Dictionary<string, Type> respTypeDict
            = new  Dictionary<string, Type>();

        private void InitAtDebug()
        {
            //var appAsm = YuUnityIOUtility.GetUnityAssembly(app.PlayAsmId);
            //var requestTypes = YuReflectUtility.GetTypeList<IYuRequestMessageHandler>(false, false, appAsm);
            //var respTypes = YuReflectUtility.GetTypeList<IYuRespMessageHandler>(false, false, appAsm);

            //foreach (var type in requestTypes)
            //{
            //    var handler = (IYuRequestMessageHandler) Activator.CreateInstance(type);
            //    if (handler.MessageId == -1)
            //    {
            //        Debug.LogError($"目标处理器{type.Name}的消息编号为-1，不合法请检查！");
            //        continue;
            //    }

            //    if (!requestHandlerDic.ContainsKey(app.LocAppId))
            //    {
            //        requestHandlerDic.Add(app.LocAppId, new Dictionary<int, IYuRequestMessageHandler>());
            //    }

            //    var appRequestHandlers = requestHandlerDic[app.LocAppId];

            //    if (!appRequestHandlers.ContainsKey(handler.MessageId))
            //    {
            //        appRequestHandlers.Add(handler.MessageId, handler);
            //    }
            //    else
            //    {
            //        throw new Exception($"协议编号{handler.MessageId}已存在，无法添加处理器{handler.GetType().Name}！");
            //    }
            //}

            //foreach (var type in respTypes)
            //{
            //    var handler = (IYuRespMessageHandler) Activator.CreateInstance(type);
            //    if (handler.MessageId == -1)
            //    {
            //        Debug.LogError($"目标处理器{type.Name}的消息编号为-1，不合法请检查！");
            //        continue;
            //    }

            //    if (!respHandlerDic.ContainsKey(app.LocAppId))
            //    {
            //        respHandlerDic.Add(app.LocAppId, new Dictionary<int, IYuRespMessageHandler>());
            //    }

            //    var appRespHandlers = respHandlerDic[app.LocAppId];
            //    if (!appRespHandlers.ContainsKey(handler.MessageId))
            //    {
            //        appRespHandlers.Add(handler.MessageId, handler);
            //    }
            //    else
            //    {
            //        throw new Exception($"协议编号{handler.MessageId}已存在，无法添加处理器{handler.GetType().Name}！");
            //    }
            //}


            // 设置当前的请求及答复处理器字典
            //var currentAppId = appEntity.CurrentRuningU3DApp.LocAppId;
            //currentRequestHandlers = requestHandlerDic[currentAppId];
        }

#endif

        #endregion


        public byte[] GetRequestMessageBytes(int messageId, object obj = null)
        {
            if (!requestHandlerDic.ContainsKey(messageId))
            {
                throw new Exception($"消息编号为{messageId}的通信请求处理器不存在！");
            }

            var handler = requestHandlerDic[messageId];
            var bytes = handler.GetMessage(obj);
#if DEBUG
            if (bytes == null)
            {
                throw new Exception($"协议编号{messageId}的通信请求处理器{handler.GetType().Name}返回了一个空字节数组，请检查！");
            }
            return bytes;
#endif
        }


    }
}