﻿using System;

namespace Client.Core
{
    /// <summary>
    /// Unity事件模块。
    /// 提供事件触发和事件观察的API接口。
    /// </summary>
    public interface IU3DEventModule 
    {
        #region Unity事件操作

        /// <summary>
        /// 注册一个Unity事件。
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="action">Action.</param>
        /// <param name="executeCount">Execute count.</param>
        void WatchUnityEvent(UnityEventType type, Action action, int executeCount = -1);

        /// <summary>
        /// 移除一个Unity事件。
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="action">Action.</param>
        void RemoveUnityEvent(UnityEventType type, Action action);

        #endregion

        #region 业务事件操作

        /// <summary>
        /// 注册一个无需数据的普通事件并返回对应事件处理器的身份Id。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="action">Action.</param>
        /// <param name="executeCount">Execute count.</param>
        int WatchEvent(EventCode eventCode, Action action, int executeCount = -1);

        /// <summary>
        /// 注册一个需要数据的普通事件并返回对应事件处理器的身份Id。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="action">Action.</param>
        /// <param name="executeCount">Execute count.</param>
        int WatchEvent(EventCode eventCode, Action<object> action, int executeCount = -1);

        /// <summary>
        /// 触发一个普通事件。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="onCompelted">On compelted.</param>
        /// <param name="data">Data.</param>
        void TriggerEvent(EventCode eventCode, Action onCompelted = null,
            object data = null);

        /// <summary>
        /// 同步触发一个普通事件。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="onCompelted">On compelted.</param>
        /// <param name="data">Data.</param>
        void TriggerEventSync(EventCode eventCode, Action onCompelted = null,
            object data = null);

        /// <summary>
        /// 移除一个普通事件相关的所有事件处理器。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        void RemoveAllHandlers(EventCode eventCode);

        /// <summary>
        /// 移除一个指定的事件处理器。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="handlerId">Handler identifier.</param>
        void RemoveEventHandler(EventCode eventCode, int handlerId);

        #endregion
    }
}