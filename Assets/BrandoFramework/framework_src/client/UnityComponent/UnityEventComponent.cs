
using client_module_event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Module.UnityComponent
{
    public class UnityEventComponent : AbsSingletonMonoComponent<UnityEventComponent>
    {
        private readonly Dictionary<YuUnityEventType, ITallyEventHandlerCaller>
            eventCallers = new Dictionary<YuUnityEventType, ITallyEventHandlerCaller>();

        private void InvokeEventCaller(YuUnityEventType type)
        {
            if (!eventCallers.ContainsKey(type))
            {
                return;
            }

            eventCallers[type].CallEventHanlder();
        }

        #region App生命周期事件

        private void OnApplicationQuit()
        {
            InvokeEventCaller(YuUnityEventType.AppQuit);
        }

        private void OnApplicationFocus(bool focus)
        {
            InvokeEventCaller(YuUnityEventType.AppFoucs);
        }

        private void OnApplicationPause(bool pause)
        {
            InvokeEventCaller(YuUnityEventType.AppPause);
        }

        #endregion

        #region 帧循环事件

        private void FixedUpdate()
        {
            InvokeEventCaller(YuUnityEventType.FixedUpdate);
        }

        private void Update()
        {
            InvokeEventCaller(YuUnityEventType.Update);
        }

        private void LateUpdate()
        {
            InvokeEventCaller(YuUnityEventType.LateUpdate);
        }

        private void OnGUI()
        {
            InvokeEventCaller(YuUnityEventType.OnGUI);
        }

        #endregion

        #region Unity事件API

        public void WatchUnityEvent
        (
            YuUnityEventType type,
            Action action,
            int executeCount = -1
        )
        {
            if (!eventCallers.ContainsKey(type))
            {
                var newCaller = new YuTallyEventHandlerCaller();
                newCaller.AddHandler(action, executeCount);
                eventCallers.Add(type, newCaller);
            }
            else
            {
                var caller = eventCallers[type];
                caller.AddHandler(action, executeCount);
            }
        }

        /// <summary>
        /// 移除一个Unity类型的事件处理器。
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="action">Action.</param>
        public void RemoveUnityEvent(YuUnityEventType type, Action action)
        {
            if (!eventCallers.ContainsKey(type))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除一个不存在的Unity事件类型，事件类型为{type}");
#endif
                return;
            }

            var caller = eventCallers[type];
            caller.RemoveHandler(action);
        }

        #endregion
    }
}