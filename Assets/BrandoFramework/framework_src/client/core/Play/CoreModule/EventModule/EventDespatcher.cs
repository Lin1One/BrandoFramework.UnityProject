
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Core
{
    /// <summary>
    /// 事件模块的核心API实现类。
    /// </summary>
    public class EventDespatcher
    {
        private readonly Dictionary<string, EventHandleUnit> eventHandleUnits
            = new Dictionary<string, EventHandleUnit>();

        private static readonly object lockObj = new object();

        private readonly Queue<EventTask> willDoTasks = new Queue<EventTask>();
        private readonly Queue<EventTask> waitTasks = new Queue<EventTask>();

        #region 触发事件

        /// <summary>
        /// 触发一个事件。
        /// 该操作会将一个事件任务信息实例加入任务队列，实际执行将稍后处理。
        /// 考虑到线程安全，该操作内部将会做加锁操作。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="onCompleted">On completed.</param>
        /// <param name="data">Data.</param>
        public void TriggerEvent(EventCode eventCode, Action onCompleted = null,
            object data = null)
        {
            lock (lockObj)
            {
                var eventTask = EventFactory.EventTaskPool.Take();
                eventTask.Init(eventCode, onCompleted, data);
                waitTasks.Enqueue(eventTask);
            }
        }

        /// <summary>
        /// 事件机制轮询方法
        /// </summary>
        public void ExecuteEvent()
        {
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
                if (!eventHandleUnits.ContainsKey(task.EventCode.EventModuleType))
                {
#if UNITY_EDITOR
                    Debug.LogWarning("尝试触发一个没有响应处理器的事件，"
                                              + $"事件Id为{task.EventCode.EventID}");
#endif
                    EventFactory.EventTaskPool.Restore(task);
                    continue;
                }

                var unit = eventHandleUnits[task.EventCode.EventModuleType];
                unit.ExecuteEvent(task.EventCode.EventID, task.EventData);
                task.OnCompelted?.Invoke();
                EventFactory.EventTaskPool.Restore(task);
            }
        }

        /// <summary>
        /// 立即触发一个事件。
        /// </summary>
        /// <param name="eventCode">Event code.</param>
        /// <param name="onCompleted">On completed.</param>
        /// <param name="data">Data.</param>
        public void TriggerEventSync(EventCode eventCode, Action onCompleted = null,
            object data = null)
        {
            var eventTask = EventFactory.EventTaskPool.Take();
            eventTask.Init(eventCode, onCompleted, data);
            if (!eventHandleUnits.ContainsKey(eventTask.EventCode.EventModuleType))
            {
#if UNITY_EDITOR
                Debug.LogWarning("尝试触发一个没有响应处理器的事件，"
                                            + $"事件Id为{eventTask.EventCode.EventID}");
#endif
                EventFactory.EventTaskPool.Restore(eventTask);
                return;
            }

            var unit = eventHandleUnits[eventTask.EventCode.EventModuleType];
            unit.ExecuteEvent(eventTask.EventCode.EventID, eventTask.EventData);
            eventTask.OnCompelted?.Invoke();
            EventFactory.EventTaskPool.Restore(eventTask);
        }

        #endregion

        #region 观察事件

        public int WatchEvent(EventCode eventCode, Action action,
            int executeCount = -1)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new EventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventID, action, executeCount);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventID, action, executeCount);
                return handlerId;
            }
        }

        public int WatchEvent(EventCode eventCode, Action<object> action,
            int executeCount = -1)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new EventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventID, action, executeCount);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventID, action, executeCount);
                return handlerId;
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
            unit.RemoveSpecifiedHandler(eventCode.EventID, handlerId);
        }
        #endregion


    }
}