using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.Core
{
    /// <summary>
    /// 事件处理单元
    /// 负责管理某个事件业务类型中事件
    /// </summary>
    public class EventHandleUnit
    {
        /// <summary>
        /// 事件处理器调用者字典。
        /// </summary>
        private readonly Dictionary<string, IEventHandlerList> EventCallers
            = new Dictionary<string, IEventHandlerList>();

        public int WatchEvent(string eventId, Action action)
        {
            if (EventCallers.ContainsKey(eventId))
            {
                var caller = EventCallers[eventId];
                var handlerId = caller.AddHandler(action);
                return handlerId;
            }
            else
            {
                var newCaller = new EventHandlerList();
                EventCallers.Add(eventId, newCaller);
                var handlerId = newCaller.AddHandler(action);
                return handlerId;
            }
        }


        public int WatchEvent(string eventId, Action<object> action)
        {
            if (EventCallers.ContainsKey(eventId))
            {
                var caller = EventCallers[eventId];
                var handlerId = caller.AddHandler(action);
                return handlerId;
            }
            else
            {
                var newCaller = new EventHandlerList();
                EventCallers.Add(eventId, newCaller);
                var handlerId = newCaller.AddHandler(action);
                return handlerId;
            }
        }


        public void RemoveEvent(string eventName)
        {
            if (!EventCallers.ContainsKey(eventName)) return;

            EventCallers.Remove(eventName);
        }

        public void RemoveHandler(string eventName, int handlerId)
        {
            if (!EventCallers.ContainsKey(eventName))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除不存在的无数据事件处理器，事件名为{eventName}");
#endif
            }
            else
            {
                var caller = EventCallers[eventName];
                caller.RemoveHandler(handlerId);
                if (caller.EventHanlderCount == 0)
                {
                    EventCallers.Remove(eventName);
                }
            }
        }

        public void ExecuteEvent(string eventId, object data)
        {
            if (!EventCallers.ContainsKey(eventId))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("没有发现目标事件{0}", eventId));
#endif
                return;
            }
            if (EventCallers.ContainsKey(eventId))
            {
                var caller = EventCallers[eventId];
                caller.CallEventHanlder(data);
            }
        }
    }

//    /// <summary>
//    /// 事件处理单元
//    /// 负责管理某个事件业务类型中事件
//    /// </summary>
//    public class EventHandleUnit<T1,T2>
//    {
//        private readonly Dictionary<string, IEventHandlerList<T1, T2>> eventCallers
//            = new Dictionary<string, IEventHandlerList<T1, T2>>();

//        /// <summary>
//        /// 观察事件。
//        /// </summary>
//        /// <param name="eventId">Event identifier.</param>
//        /// <param name="action">Action.</param>
//        /// <param name="executeCount">Execute count.</param>
//        public int WatchEvent(string eventId, Action<T1, T2> action, int executeCount = -1)
//        {
//            if (eventCallers.ContainsKey(eventId))
//            {
//                var caller = eventCallers[eventId];
//                var handlerId = caller.AddHandler(action);
//                return handlerId;
//            }
//            else
//            {
//                var newCaller = new EventHandlerList<T1, T2>();
//                eventCallers.Add(eventId, newCaller);
//                var handlerId = newCaller.AddHandler(action);
//                return handlerId;
//            }
//        }


//        public void RemoveSpecifiedHandler(string eventName, int handlerId)
//        {
//            if (!eventCallers.ContainsKey(eventName))
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(
//                    $"尝试移除不存在的无数据事件处理器，事件名为{eventName}");
//#endif
//            }
//            else
//            {
//                var caller = eventCallers[eventName];
//                caller.RemoveHandler(handlerId);
//                if (caller.EventHanlderCount == 0)
//                {
//                    eventCallers.Remove(eventName);
//                }
//            }
//        }

//        public void ExecuteEvent(string eventId, T1 data1, T2 data2)
//        {
//            if (!eventCallers.ContainsKey(eventId))
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(string.Format("没有发现目标事件{0}", eventId));
//#endif
//                return;
//            }

//            if (eventCallers.ContainsKey(eventId))
//            {
//                var caller = eventCallers[eventId];
//                caller.CallEventHanlder(data1, data2);
//            }
//        }

//    }

//    /// <summary>
//    /// 事件处理单元
//    /// 负责管理某个事件业务类型中事件
//    /// </summary>
//    public class EventHandleUnit<T1>
//    {
//        /// <summary>
//        /// 无需数据的事件处理器调用者字典。
//        /// </summary>
//        private readonly Dictionary<string, IEventHandlerList<T1>> eventCallers
//            = new Dictionary<string, IEventHandlerList<T1>>();

//        #region 观察事件

//        /// <summary>
//        /// 观察一个需要数据的事件。
//        /// </summary>
//        /// <param name="eventId">Event identifier.</param>
//        /// <param name="action">Action.</param>
//        /// <param name="executeCount">Execute count.</param>
//        public int WatchEvent(string eventId, Action<T1> action, int executeCount = -1)
//        {
//            if (eventCallers.ContainsKey(eventId))
//            {
//                var caller = eventCallers[eventId];
//                var handlerId = caller.AddHandler(action, executeCount);
//                return handlerId;
//            }
//            else
//            {
//                var newCaller = new EventHandlerList<T1>();
//                eventCallers.Add(eventId, newCaller);
//                var handlerId = newCaller.AddHandler(action, executeCount);
//                return handlerId;
//            }
//        }

//        #endregion

//        #region 移除事件处理

//        public void RemoveEvent(string eventName)
//        {
//            if (!eventCallers.ContainsKey(eventName)) return;

//            eventCallers.Remove(eventName);
//        }

//        public void RemoveHandler(string eventName, int handlerId)
//        {
//            if (!eventCallers.ContainsKey(eventName))
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(
//                    $"尝试移除不存在的无数据事件处理器，事件名为{eventName}");
//#endif
//            }
//            else
//            {
//                var caller = eventCallers[eventName];
//                caller.RemoveHandler(handlerId);
//                if (caller.EventHanlderCount == 0)
//                {
//                    eventCallers.Remove(eventName);
//                }
//            }
//        }

//        #endregion

//        #region 事件执行

//        public void ExecuteEvent(string eventId, T1 data)
//        {
//            //!m_UseDataEventCallers.ContainsKey(eventId) &&
//            if (!eventCallers.ContainsKey(eventId))
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(string.Format("没有发现目标事件{0}", eventId));
//#endif
//                return;
//            }

//            if (eventCallers.ContainsKey(eventId))
//            {
//                var caller = eventCallers[eventId];
//                caller.CallEventHanlder(data);
//            }
//        }

//        ///// <summary>
//        ///// 事件调用者唤起次数。
//        ///// </summary>
//        //private int m_ExecuteCallCount;

//        ///// <summary>
//        ///// 触发事件调用者清理的唤起次数阈值。
//        ///// </summary>
//        //private const int ON_CLEAN_MAX = 10;

//        //private void ClearZero()
//        //{
//        //    //m_UseDataEventCallers.Values.ToList().ForEach(c => c.ClearZero());
//        //    m_NotDataEventCallers.Values.ToList().ForEach(c => c.ClearZero());
//        //}

//        //private void TryCleanZero()
//        //{
//        //    m_ExecuteCallCount++;
//        //    if (m_ExecuteCallCount <= ON_CLEAN_MAX) return;

//        //    ClearZero();
//        //    m_ExecuteCallCount = 0;
//        //}

//        #endregion
    //}
}


