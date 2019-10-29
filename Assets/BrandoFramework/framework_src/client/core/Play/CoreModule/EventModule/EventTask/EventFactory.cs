using Common;
using System.Collections.Generic;

namespace Client.Core
{
    /// <summary>
    /// 事件工厂。
    /// 1. 构建及缓存事件码。
    /// </summary>
    public static class EventFactory
    {
        private static readonly Dictionary<string, Dictionary<string, EventCode>> EventCodes
            = new Dictionary<string, Dictionary<string, EventCode>>();

        /// <summary>
        /// 获得一个事件码对象。
        /// </summary>
        /// <param name="moduleType">事件所属模块类型。</param>
        /// <param name="id">事件Id。</param>
        /// <returns></returns>
        public static EventCode GetEventCode(string moduleType, string id)
        {
            if (!EventCodes.ContainsKey(moduleType))
            {
                var newCode = CreateNewEventCode(moduleType, id);
                //EventCodes.Add(moduleType, new Dictionary<string, EventCode>());
                return newCode;
            }

            var moduleCodes = EventCodes[moduleType];
            if (!moduleCodes.ContainsKey(id))
            {
                return CreateNewEventCode(moduleType, id);
            }

            return moduleCodes[id];
        }

        private static EventCode CreateNewEventCode(string moduleType, string id)
        {
            var eventCode = new EventCode(moduleType, id);
            return eventCode;
        }

        /// <summary>
        /// 事件任务对象池。
        /// </summary>
        public static readonly IGenericObjectPool<EventTask> EventTaskPool
        = new GenericObjectPool<EventTask>(() => new EventTask(), 10);




    }
}
