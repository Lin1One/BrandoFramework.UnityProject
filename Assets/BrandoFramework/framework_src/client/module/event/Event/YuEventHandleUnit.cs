using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace client_module_event
{
    /// <summary>
    /// 事件处理单元
    /// </summary>
    public class YuEventHandleUnit
    {
        /// <summary>
        /// 带有数据参数的事件处理器调用者字典。
        /// </summary>
        private readonly Dictionary<string, ITallyEventHandlerCaller<object>> m_UseDataEventCallers
        = new Dictionary<string, ITallyEventHandlerCaller<object>>();

        /// <summary>
        /// 无需数据的事件处理器调用者字典。
        /// </summary>
        private readonly Dictionary<string, ITallyEventHandlerCaller> m_NotDataEventCallers
        = new Dictionary<string, ITallyEventHandlerCaller>();

        #region 观察事件

        /// <summary>
        /// 观察一个需要数据的事件。
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <param name="action">Action.</param>
        /// <param name="executeCount">Execute count.</param>
        public int WatchEvent(string eventId, Action<object> action, int executeCount = -1)
        {
            if (m_UseDataEventCallers.ContainsKey(eventId))
            {
                var caller = m_UseDataEventCallers[eventId];
                var handlerId = caller.AddHandler(action, executeCount);
                return handlerId;
            }
            else
            {
                var newCaller = new YuTallyEventHandlerCaller<object>();
                m_UseDataEventCallers.Add(eventId, newCaller);
                var handlerId = newCaller.AddHandler(action, executeCount);
                return handlerId;
            }
        }

        /// <summary>
        /// 观察一个无需数据的事件。
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <param name="action">Action.</param>
        /// <param name="executeCount">Execute count.</param>
        public int WatchEvent(string eventId, Action action, int executeCount = -1)
        {
            if (m_NotDataEventCallers.ContainsKey(eventId))
            {
                var caller = m_NotDataEventCallers[eventId];
                var handlerId = caller.AddHandler(action, executeCount);
                return handlerId;
            }
            else
            {
                var newCaller = new YuTallyEventHandlerCaller();
                m_NotDataEventCallers.Add(eventId, newCaller);
                var handlerId = newCaller.AddHandler(action, executeCount);
                return handlerId;
            }
        }

        #endregion

        #region 移除事件处理

        private void RemoveUseDataEvent(string eventName)
        {
            if (!m_UseDataEventCallers.ContainsKey(eventName)) return;

            m_UseDataEventCallers.Remove(eventName);
        }

        private void RemoveNotDataEvent(string eventName)
        {
            if (!m_NotDataEventCallers.ContainsKey(eventName)) return;

            m_NotDataEventCallers.Remove(eventName);
        }

        public void RemoveEvent(string eventName)
        {
            RemoveUseDataEvent(eventName);
            RemoveNotDataEvent(eventName);
        }

        private void RemoveSpecifiedHandlerAtNotData(string eventName, int handlerId)
        {
            if (!m_NotDataEventCallers.ContainsKey(eventName))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除不存在的无数据事件处理器，事件名为{eventName}");
#endif
            }
            else
            {
                var caller = m_NotDataEventCallers[eventName];
                caller.RemoveHandler(handlerId);
                if (caller.EventHanlderCount == 0)
                {
                    m_NotDataEventCallers.Remove(eventName);
                }
            }
        }

        private void RemoveSpecifiedHandlerAtUseData(string eventName, int handlerId)
        {
            if (!m_UseDataEventCallers.ContainsKey(eventName))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除不存在的有数据事件处理器，事件名为{eventName}");
#endif
            }
            else
            {
                var caller = m_UseDataEventCallers[eventName];
                caller.RemoveHandler(handlerId);
                if (caller.EventHanlderCount == 0)
                {
                    m_UseDataEventCallers.Remove(eventName);
                }
            }
        }

        public void RemoveSpecifiedHandler(string eventName, int handlerId)
        {
            RemoveSpecifiedHandlerAtNotData(eventName, handlerId);
            RemoveSpecifiedHandlerAtUseData(eventName, handlerId);
        }

        #endregion

        #region 事件执行

        /// <summary>
        /// 事件调用者唤起次数。
        /// </summary>
        private int m_ExecuteCallCount;

        /// <summary>
        /// 触发事件调用者清理的唤起次数阈值。
        /// </summary>
        private const int ON_CLEAN_MAX = 10;

        private void ClearZero()
        {
            m_UseDataEventCallers.Values.ToList().ForEach(c => c.ClearZero());
            m_NotDataEventCallers.Values.ToList().ForEach(c => c.ClearZero());
        }

        private void TryCleanZero()
        {
            m_ExecuteCallCount++;
            if (m_ExecuteCallCount <= ON_CLEAN_MAX) return;

            ClearZero();
            m_ExecuteCallCount = 0;
        }

        public void ExecuteEvent(string eventId, object data)
        {
            if (!m_UseDataEventCallers.ContainsKey(eventId) &&
                !m_NotDataEventCallers.ContainsKey(eventId))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("没有发现目标事件{0}", eventId));
#endif
                return;
            }

            if (m_UseDataEventCallers.ContainsKey(eventId))
            {
                var caller = m_UseDataEventCallers[eventId];
                caller.CallEventHanlder(data);
                TryCleanZero();
            }

            if (m_NotDataEventCallers.ContainsKey(eventId))
            {
                var caller = m_NotDataEventCallers[eventId];
                caller.CallEventHanlder();
                TryCleanZero();
            }
        }


        #endregion
    }
}


