
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Core
{
    /// <summary>
    /// 事件模块的核心API实现类。
    /// </summary>
    public class EventDispatcher
    {
        private readonly Dictionary<string, EventHandleUnit> eventHandleUnits
            = new Dictionary<string, EventHandleUnit>();

        private Dictionary<string, List<Delegate>> eventHandlerList = 
            new Dictionary<string, List<Delegate>>();

        private static readonly object lockObj = new object();

        private readonly Queue<EventTask> willDoTasks = new Queue<EventTask>();
        private readonly Queue<EventTask> waitTasks = new Queue<EventTask>();

        #region 观察事件

        public int WatchEvent(EventCode eventCode, Action action)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new EventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventID, action);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventID, action);
                return handlerId;
            }
        }

        public int WatchEvent(EventCode eventCode, Action<object> action)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new EventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventID, action);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventID, action);
                return handlerId;
            }
        }

        public int WatchEvent<T1>(EventCode eventCode, Action<T1> action)
        {
            if (!eventHandlerList.ContainsKey(eventCode.EventID))
            {
                var newList = new List<Delegate>();
                eventHandlerList.Add(eventCode.EventID, newList);
            }
            var list = eventHandlerList[eventCode.EventID];
            ///未实现
            var handlerId = TallyEventHandlerCounter.GetEventHandlerId();
            list.Add(action);
            return handlerId;
            
        }

        public int WatchEvent<T1, T2>(EventCode eventCode, Action<T1, T2> action)
        {
            if (!eventHandlerList.ContainsKey(eventCode.EventID))
            {
                var newList = new List<Delegate>();
                eventHandlerList.Add(eventCode.EventID, newList);
            }
            var list = eventHandlerList[eventCode.EventID];
            var handlerId = TallyEventHandlerCounter.GetEventHandlerId();
            list.Add(action);
            return handlerId;
        }

        #endregion


        #region 触发事件

        /// <summary>
        /// 触发一个事件。
        /// 该操作会将一个事件任务信息实例加入任务队列，实际执行将稍后处理。
        /// 考虑到线程安全，该操作内部将会做加锁操作。
        /// </summary>
        public void TriggerEvent(EventCode eventCode, object data = null ,Action onCompleted = null)
        {
            lock (lockObj)
            {
                var eventTask = EventFactory.EventTaskPool.Take();
                eventTask.Init(eventCode, onCompleted, data);
                waitTasks.Enqueue(eventTask);
            }
        }

        //public void TriggerEvent<T1>(EventCode eventCode, T1 eventData, Action onCompleted = null)
        //{
        //    lock (lockObj)
        //    {
        //        var eventTask = EventFactory<T1>.EventTaskPool.Take();
        //        eventTask.Init(eventCode, onCompleted, eventData);
        //        waitTasks.Enqueue(eventTask);
        //    }
        //}

        //public void TriggerEvent<T1,T2>(EventCode eventCode, T1 eventData1, T2 eventData2, Action onCompleted = null)
        //{
        //    lock (lockObj)
        //    {
        //        var eventTask = EventFactory<T1, T2>.EventTaskPool.Take();
        //        eventTask.Init(eventCode, onCompleted, eventData1, eventData2);
        //        waitTasks.Enqueue(eventTask);
        //    }
        //}


        /// <summary>
        /// 事件机制轮询方法
        /// </summary>
        public void ExecuteEvent()
        {
            //考虑到线程安全，将即将执行的 Task 取出
            lock (lockObj)
            {
                var waitTaskCount = waitTasks.Count;
                for (var i = 0; i < waitTaskCount; i++)
                {
                    var waitTask = waitTasks.Dequeue();
                    willDoTasks.Enqueue(waitTask);
                }
            }

            if (willDoTasks.Count == 0)
            {
                return;
            }

            var taskCount = willDoTasks.Count;

            for (var i = 0; i < taskCount; i++)
            {
                var task = willDoTasks.Dequeue();
                if (!eventHandleUnits.ContainsKey(task.eventCode.EventModuleType))
                {
#if UNITY_EDITOR
                    Debug.LogWarning("尝试触发一个没有响应处理器的事件，"
                                              + $"事件Id为{task.eventCode.EventID}");
#endif
                    EventFactory.EventTaskPool.Restore(task);
                    continue;
                }

                var unit = eventHandleUnits[task.eventCode.EventModuleType];
                unit.ExecuteEvent(task.eventCode.EventID, task.eventData);
                task.onCompelted?.Invoke();
                EventFactory.EventTaskPool.Restore(task);
            }
        }

        /// <summary>
        /// 立即触发一个事件。
        /// </summary>
        public void TriggerEventSync(EventCode eventCode, Action onCompleted = null,
            object data = null)
        {
            var eventTask = EventFactory.EventTaskPool.Take();
            eventTask.Init(eventCode, onCompleted, data);
            if (!eventHandleUnits.ContainsKey(eventTask.eventCode.EventModuleType))
            {
#if UNITY_EDITOR
                Debug.LogWarning("尝试触发一个没有响应处理器的事件，"
                                            + $"事件Id为{eventTask.eventCode.EventID}");
#endif
                EventFactory.EventTaskPool.Restore(eventTask);
                return;
            }

            var unit = eventHandleUnits[eventTask.eventCode.EventModuleType];
            unit.ExecuteEvent(eventTask.eventCode.EventID, eventTask.eventData);
            eventTask.onCompelted?.Invoke();
            EventFactory.EventTaskPool.Restore(eventTask);
        }

        /// <summary>
        /// 立即触发一个事件。
        /// </summary>
        public void TriggerEventSync<T1>(EventCode eventCode, T1 data, Action onCompleted = null)
        {
            List<Delegate> actionList;
            if(eventHandlerList.TryGetValue(eventCode.EventID,out actionList))
            {
                foreach(var action in actionList)
                {
                    ((Action<T1>)action)(data);
                }
            }
        }

        /// <summary>
        /// 立即触发一个事件。
        /// </summary>
        public void TriggerEventSync<T1,T2>(EventCode eventCode, T1 data1, T2 data2,Action onCompleted = null)
        {
            List<Delegate> actionList;
            if (eventHandlerList.TryGetValue(eventCode.EventID, out actionList))
            {
                foreach (var action in actionList)
                {
                    ((Action<T1,T2>)action)(data1, data2);
                }
            }
        }

        #endregion


        #region 移除事件

        /// <summary>
        /// 移除指定事件的所有事件处理器。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        public void RemoveAllHandler(EventCode eventCode)
        {
            if (eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除一个当前不存在的事件处理器，事件Id为{eventCode.EventID}。");
#endif
                return;
            }

            var unit = eventHandleUnits[eventCode.EventModuleType];
            unit.RemoveEvent(eventCode.EventID);
        }

        public void RemoveHandlerById(EventCode eventCode, int handlerId)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除一个当前不存在的事件处理器，事件Id为{eventCode.EventID}。");
#endif
                return;
            }

            var unit = eventHandleUnits[eventCode.EventModuleType];
            unit.RemoveHandler(eventCode.EventID, handlerId);
        }
        #endregion


    }
}