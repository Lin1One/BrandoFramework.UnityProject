using Client.Module.UnityComponent;
using client_common;
using System;

namespace client_module_event
{
    /// <summary>
    /// Unity事件模块。
    /// </summary>
    public class U3DEventModule : IU3DEventModule
    {
        private readonly UnityEventComponent eventComponent;

        private readonly EventDespatcher eventApiCore = new EventDespatcher();

        public U3DEventModule()
        {
            eventComponent = YuU3dInjector.MonoInjectorInstance.GetMono<UnityEventComponent>();
            WatchUnityEvent(YuUnityEventType.Update, eventApiCore.ExecuteEvent);
        }

        #region Unity事件

        public void WatchUnityEvent(YuUnityEventType type, Action action, int executeCount = -1)
        {
            eventComponent.WatchUnityEvent(type, action, executeCount);
        }

        public void RemoveUnityEvent(YuUnityEventType type, Action action)
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

        public void RemoveAllHandler(EventCode eventCode)
        {
            eventApiCore.RemoveAllHandler(eventCode);
        }

        public void RemoveSpecifiedHandler(EventCode eventCode, int handlerId)
        {
            eventApiCore.RemoveSpecifiedHandler(eventCode, handlerId);
        }

        #endregion




    }
}