using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Core.UnityComponent
{
    public class UnityEventComponent : AbsSingletonMonoComponent<UnityEventComponent>
    {
        private readonly Dictionary<UnityEventType, ITallyEventHandlerCaller>
            eventCallers = new Dictionary<UnityEventType, ITallyEventHandlerCaller>();

        private void InvokeEventCaller(UnityEventType type)
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
            InvokeEventCaller(UnityEventType.AppQuit);
        }

        private void OnApplicationFocus(bool focus)
        {
            InvokeEventCaller(UnityEventType.AppFoucs);
        }

        private void OnApplicationPause(bool pause)
        {
            InvokeEventCaller(UnityEventType.AppPause);
        }

        #endregion

        #region 帧循环事件

        private void FixedUpdate()
        {
            InvokeEventCaller(UnityEventType.FixedUpdate);
        }

        private void Update()
        {
            InvokeEventCaller(UnityEventType.Update);
        }

        private void LateUpdate()
        {
            InvokeEventCaller(UnityEventType.LateUpdate);
        }

        private void OnGUI()
        {
            InvokeEventCaller(UnityEventType.OnGUI);
        }

        #endregion

        #region Unity事件API

        public void WatchUnityEvent
        (
            UnityEventType type,
            Action action,
            int executeCount = -1
        )
        {
            if (!eventCallers.ContainsKey(type))
            {
                var newCaller = new TallyEventHandlerCaller();
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
        public void RemoveUnityEvent(UnityEventType type, Action action)
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