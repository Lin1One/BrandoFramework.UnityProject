using Common;
using System;

namespace Client.Core
{
    /// <summary>
    /// Unity事件模块。
    /// 提供事件触发和事件观察的API接口。
    /// </summary>
    public interface IU3DEventModule : IModule, IModuleConfigAble
    {
        #region Unity事件操作

        /// <summary>
        /// 注册一个Unity事件。
        /// </summary>
        void WatchUnityEvent(UnityEventType type, Action action);

        /// <summary>
        /// 移除一个Unity事件。
        /// </summary>
        void RemoveUnityEvent(UnityEventType type, Action action);

        #endregion

        #region 逻辑事件操作

        int WatchEvent(EventCode eventCode, Action action);

        int WatchEvent(EventCode eventCode, Action<object> action);

        int WatchEvent<T1>(EventCode eventCode, Action<T1> action);

        int WatchEvent<T1,T2>(EventCode eventCode, Action<T1,T2> action);

        void TriggerEvent(EventCode eventCode, Action onCompelted = null);

        void TriggerEvent(EventCode eventCode, object eventData, Action onCompelted = null);

        //void TriggerEvent<T1>(EventCode eventCode, T1 eventData1, Action onCompelted = null);

        //void TriggerEvent<T1,T2>(EventCode eventCode, T1 eventData1, T2 eventData2, Action onCompelted = null);

        void TriggerEventSync(EventCode eventCode, object data = null,Action onCompelted = null);

        void TriggerEventSync<T1>(EventCode eventCode, T1 data, Action onCompelted = null);

        void TriggerEventSync<T1,T2>(EventCode eventCode,T1 data1, T2 data2, Action onCompelted = null);

        /// <summary>
        /// 移除一个普通事件相关的所有事件处理器。
        /// </summary>
        void RemoveAllHandlers(EventCode eventCode);

        /// <summary>
        /// 移除一个指定的事件处理器。
        /// </summary>
        void RemoveEventHandler(EventCode eventCode, int handlerId);

        #endregion
    }
}