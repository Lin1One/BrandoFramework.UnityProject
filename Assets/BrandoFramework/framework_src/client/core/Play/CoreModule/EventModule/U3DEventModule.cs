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

        private readonly EventDespatcher eventApiCore = new EventDespatcher();

        public U3DEventModule()
        {
            //创建 UnityComponent 以监听 Update 周期函数
            eventComponent = YuU3dInjector.MonoInjectorInstance.GetMono<UnityEventComponent>();
            WatchUnityEvent(UnityEventType.Update, eventApiCore.ExecuteEvent);
        }

        #region Unity事件

        public void WatchUnityEvent(UnityEventType type, Action action, int executeCount = -1)
        {
            eventComponent.WatchUnityEvent(type, action, executeCount);
        }

        public void RemoveUnityEvent(UnityEventType type, Action action)
        {
            eventComponent.RemoveUnityEvent(type, action);
        }

        #endregion

        #region 业务事件

        public int WatchEvent(EventCode eventCode, Action action, int executeCount = -1)
        {
            return eventApiCore.WatchEvent(eventCode, action, executeCount);
        }

        public int WatchEvent(EventCode eventCode, Action<object> action, int executeCount = -1)
        {
            return eventApiCore.WatchEvent(eventCode, action, executeCount);
        }

        public void TriggerEvent(EventCode eventCode, Action onCompelted = null, object data = null)
        {
            eventApiCore.TriggerEvent(eventCode, onCompelted, data);
        }

        public void TriggerEventSync(EventCode eventCode, Action onCompelted = null, object data = null)
        {
            eventApiCore.TriggerEventSync(eventCode, onCompelted, data);
        }

        public void RemoveAllHandlers(EventCode eventCode)
        {
            eventApiCore.RemoveAllHandler(eventCode);
        }

        public void RemoveEventHandler(EventCode eventCode, int handlerId)
        {
            eventApiCore.RemoveHandlerById(eventCode, handlerId);
        }
        #endregion




    }
}