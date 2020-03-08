using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.Core
{
    #region 非泛型

    public class EventHandlerList : IEventHandlerList
    {
        private List<IEventHandler> m_Handlers
            = new List<IEventHandler>();

        public int EventHanlderCount => m_Handlers.Count;

        public int AddHandler(Delegate eventAction)
        {
            var existHanlder = m_Handlers.Find(h => h.CheckMatch(eventAction));
            if (existHanlder != null)
            {
                Debug.LogError
                    ($"尝试添加一个已经存在的事件处理器，事件处理器为{eventAction.Method.Name}！");
                return -1;
            }

            var handler = new EventHandler(eventAction);
            m_Handlers.Add(handler);
            return handler.Id;
        }

        public void CallEventHanlder(object data)
        {
            for (int i = 0; i < m_Handlers.Count; i++)
            {
                var handler = m_Handlers[i];
                handler.HandleEvent(data);
            }
        }

        public void RemoveHandler(Delegate evenAction)
        {
            var handler = m_Handlers.Find(h => h.CheckMatch(evenAction));
            if (handler == null) return;

            m_Handlers.Remove(handler);
        }

        public void RemoveHandler(int handlerId)
        {
            var handler = m_Handlers.Find(h => h.Id == handlerId);
            if (handler == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除不存在的事件处理器，事件Id为{handlerId}！");
#endif
                return;
            }
            m_Handlers.Remove(handler);
        }
    }

    #endregion

//    #region 泛型

//    public class EventHandlerList<TData> : IEventHandlerList<TData>
//    {
//        private List<IEventHandler<TData>> m_Handlers
//            = new List<IEventHandler<TData>>();

//        public int EventHanlderCount => m_Handlers.Count;

//        public int AddHandler(Action<TData> eventAction)
//        {
//            var handler = new EventHandler<TData>( eventAction);
//            m_Handlers.Add(handler);
//            return handler.Id;
//        }

//        public void CallEventHanlder(TData td)
//        {
//            for (int i = 0; i < m_Handlers.Count(); i++)
//            {
//                var handler = m_Handlers[i];
//                handler.HandleEvent(td);
//            }
//        }

//        public void RemoveHandler(Action<TData> evenAction)
//        {
//            var handler = m_Handlers.Find(h => h.CheckMatch(evenAction));
//            if (handler == null) return;

//            m_Handlers.Remove(handler);
//        }

//        public void RemoveHandler(int handlerId)
//        {
//            var handler = m_Handlers.Find(h => h.Id == handlerId);
//            if (handler == null)
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(
//                    string.Format("尝试移除不存在的事件处理器，事件Id为{0}！", handlerId));
//#endif
//                return;
//            }

//            m_Handlers.Remove(handler);
//        }
//    }

//    public class EventHandlerList<T1,T2> : IEventHandlerList<T1, T2>
//    {
//        private List<IEventHandler<T1, T2>> m_Handlers
//            = new List<IEventHandler<T1, T2>>();

//        public int EventHanlderCount => m_Handlers.Count;

//        public int AddHandler(Action<T1, T2> eventAction)
//        {
//            var handler = new EventHandler<T1, T2>(eventAction);
//            m_Handlers.Add(handler);
//            return handler.Id;
//        }

//        public void CallEventHanlder(T1 eventData1, T2 eventData2)
//        {
//            for (int i = 0; i < m_Handlers.Count(); i++)
//            {
//                var handler = m_Handlers[i];
//                handler.HandleEvent(eventData1, eventData2);
//            }
//        }

//        public void RemoveHandler(Action<T1, T2> evenAction)
//        {
//            var handler = m_Handlers.Find(h => h.CheckMatch(evenAction));
//            if (handler == null) return;

//            m_Handlers.Remove(handler);
//        }

//        public void RemoveHandler(int handlerId)
//        {
//            var handler = m_Handlers.Find(h => h.Id == handlerId);
//            if (handler == null)
//            {
//#if UNITY_EDITOR
//                Debug.LogWarning(
//                    string.Format("尝试移除不存在的事件处理器，事件Id为{0}！", handlerId));
//#endif
//                return;
//            }

//            m_Handlers.Remove(handler);
//        }
//    }

//    #endregion
}