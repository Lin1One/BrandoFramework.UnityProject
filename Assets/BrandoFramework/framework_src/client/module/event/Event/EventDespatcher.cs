
using System;
using System.Collections.Generic;
using UnityEngine;

namespace client_module_event
{
    /// <summary>
    /// 事件模块的核心API实现类。
    /// </summary>
    public class EventDespatcher
    {
        private readonly Dictionary<string, YuEventHandleUnit> eventHandleUnits
            = new Dictionary<string, YuEventHandleUnit>();

        private static readonly object lockObj = new object();

        private readonly Queue<YuEventTask> willDoTasks = new Queue<YuEventTask>();
        private readonly Queue<YuEventTask> waitTasks = new Queue<YuEventTask>();

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
                var eventTask = YuEventFactory.EventTaskPool.Take();
                eventTask.Init(eventCode, onCompleted, data);
                waitTasks.Enqueue(eventTask);
            }
        }

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
                                              + $"事件Id为{task.EventCode.EventName}");
#endif
                    YuEventFactory.EventTaskPool.Restore(task);
                    continue;
                }

                var unit = eventHandleUnits[task.EventCode.EventModuleType];
                unit.ExecuteEvent(task.EventCode.EventName, task.EventData);
                task.OnCompelted?.Invoke();
                YuEventFactory.EventTaskPool.Restore(task);
            }
        }

        #region 观察事件

        public int WatchEvent(EventCode eventCode, Action action,
            int executeCount = -1)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new YuEventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventName, action, executeCount);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventName, action, executeCount);
                return handlerId;
            }
        }

        public int WatchEvent(EventCode eventCode, Action<object> action,
            int executeCount = -1)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
                var newUnit = new YuEventHandleUnit();
                eventHandleUnits.Add(eventCode.EventModuleType, newUnit);
                var handlerId = newUnit.WatchEvent(eventCode.EventName, action, executeCount);
                return handlerId;
            }
            else
            {
                var unit = eventHandleUnits[eventCode.EventModuleType];
                var handlerId = unit.WatchEvent(eventCode.EventName, action, executeCount);
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
                    $"尝试移除一个当前不存在的事件处理器，事件Id为{eventCode.EventName}。");
#endif
                return;
            }

            var unit = eventHandleUnits[eventCode.EventModuleType];
            unit.RemoveEvent(eventCode.EventName);
        }

        #endregion

        public void RemoveSpecifiedHandler(EventCode eventCode, int handlerId)
        {
            if (!eventHandleUnits.ContainsKey(eventCode.EventModuleType))
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"尝试移除一个当前不存在的事件处理器，事件Id为{eventCode.EventName}。");
#endif
                return;
            }

            var unit = eventHandleUnits[eventCode.EventModuleType];
            unit.RemoveSpecifiedHandler(eventCode.EventName, handlerId);
        }
    }
}