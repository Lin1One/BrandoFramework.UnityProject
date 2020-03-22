using Client.Core.UnityComponent;
using Common;
using System;

namespace Client.Core
{
    /// <summary>
    /// Unity事件模块
    /// 初始化时，创建一个空 GameObject 挂载 UnityEventComponent 
    /// 以将 EventDespatcher 的 Update 方法加入到 unity 的主循环中
    /// 核心实现与 Unity 相关的接口分离
    /// </summary>
    public class U3DEventModule : IU3DEventModule
    {
        private readonly UnityEventComponent eventComponent;

        private readonly EventDispatcher eventDispatcher = new EventDispatcher();

        public U3DEventModule()
        {
            //创建 UnityComponent 以监听 Update 周期函数
            eventComponent = U3dInjector.MonoInjectorInstance.GetMono<UnityEventComponent>();
        }

        public void InitModule()
        {
            WatchUnityEvent(UnityEventType.Update, eventDispatcher.ExecuteEvent);
        }

        public void ApplyConfig(IModuleConfig config)
        {
            EventModuleConfig eventModuelConfig = config as EventModuleConfig;
        }

        #region Unity事件

        public void WatchUnityEvent(UnityEventType type, Action action)
        {
            eventComponent.WatchUnityEvent(type, action);
        }

        public void RemoveUnityEvent(UnityEventType type, Action action)
        {
            eventComponent.RemoveUnityEvent(type, action);
        }

        #endregion

        #region 业务事件

        public int WatchEvent(EventCode eventCode, Action action)
        {
            return eventDispatcher.WatchEvent(eventCode, action);
        }

        public int WatchEvent(EventCode eventCode, Action<object> action)
        {
            return eventDispatcher.WatchEvent(eventCode, action);
        }

        public int WatchEvent<T1>(EventCode eventCode, Action<T1> action)
        {
            return eventDispatcher.WatchEvent(eventCode, action);
        }

        public int WatchEvent<T1, T2>(EventCode eventCode, Action<T1, T2> action)
        {
            return eventDispatcher.WatchEvent(eventCode, action);
        }

        public void TriggerEvent(EventCode eventCode, Action onCompelted = null)
        {
            eventDispatcher.TriggerEvent(eventCode, onCompelted);
        }

        public void TriggerEvent(EventCode eventCode, object eventData, Action onCompelted = null)
        {
            eventDispatcher.TriggerEvent(eventCode, eventData, onCompelted);
        }

        //public void TriggerEvent<T1>(EventCode eventCode, T1 eventData1, Action onCompelted = null)
        //{
        //    eventDispatcher.TriggerEvent(eventCode, eventData1, onCompelted);
        //}

        //public void TriggerEvent<T1, T2>(EventCode eventCode, T1 eventData1, T2 eventData2, Action onCompelted = null)
        //{
        //    eventDispatcher.TriggerEvent(eventCode, eventData1, eventData2,onCompelted);
        //}

        public void TriggerEventSync(EventCode eventCode, object data = null, Action onCompelted = null)
        {
            eventDispatcher.TriggerEventSync(eventCode, onCompelted, data);
        }

        public void TriggerEventSync<T1>(EventCode eventCode, T1 data, Action onCompelted = null)
        {
            eventDispatcher.TriggerEventSync(eventCode, data, onCompelted);
        }

        public void TriggerEventSync<T1, T2>(EventCode eventCode, T1 data1, T2 data2, Action onCompelted = null)
        {
            eventDispatcher.TriggerEventSync(eventCode, data1, data2,onCompelted);
        }

        public void RemoveAllHandlers(EventCode eventCode)
        {
            eventDispatcher.RemoveAllHandler(eventCode);
        }

        public void RemoveEventHandler(EventCode eventCode, int handlerId)
        {
            eventDispatcher.RemoveHandlerById(eventCode, handlerId);
        }
        #endregion




    }
}