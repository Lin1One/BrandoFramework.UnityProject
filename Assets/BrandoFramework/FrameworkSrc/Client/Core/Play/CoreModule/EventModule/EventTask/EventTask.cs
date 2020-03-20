using System;

namespace Client.Core
{
    /// <summary>
    /// 事件任务信息。
    /// </summary>
    public class BaseEventTask
    {
        /// <summary>
        /// 事件任务的事件码，描述了事件所属的模块及事件Id。
        /// </summary>
        public EventCode eventCode { get; set; }

        /// <summary>
        /// 事件完成委托。
        /// </summary>
        public Action onCompelted { get; set; }

        public virtual void Reset()
        {
            eventCode = null;
            onCompelted = null;
        }
    }
    /// <summary>
    /// 事件任务信息。
    /// </summary>
    public class EventTask : BaseEventTask
    {
        /// <summary>
        /// 事件数据。
        /// </summary>
        public object eventData { get; private set; }

        public EventTask Init(EventCode _eventCode,Action action,object data = null )
        {
            eventCode = _eventCode;
            onCompelted = action;
            eventData = data;
            return this;
        }

        public override void Reset()
        {
            base.Reset();
            eventData = null;
        }
    }

    ///// <summary>
    ///// 事件任务信息。
    ///// </summary>
    //public class EventTask<T1> : BaseEventTask
    //{
    //    /// <summary>
    //    /// 事件数据。
    //    /// </summary>
    //    /// <value>The event data.</value>
    //    public T1 EventData { get; private set; }

    //    public EventTask<T1> Init(EventCode _eventCode, Action action, T1 eventData)
    //    {
    //        eventCode = eventCode;
    //        onCompelted = action;
    //        EventData = eventData;
    //        return this;
    //    }

    //    public override void Reset()
    //    {
    //        base.Reset();
    //        EventData = default(T1);
    //    }
    //}

    ///// <summary>
    ///// 事件任务信息。
    ///// </summary>
    //public class EventTask<T1,T2> : BaseEventTask
    //{
    //    public T1 EventData1 { get; private set; }

    //    public T2 EventData2 { get; private set; }

    //    public EventTask<T1, T2> Init(EventCode _eventCode, Action action, T1 eventData1, T2 eventData2)
    //    {
    //        EventData1 = eventData1;
    //        EventData2 = eventData2;
    //        return this;
    //    }

    //    public override void Reset()
    //    {
    //        base.Reset();
    //        EventData1 = default(T1);
    //        EventData2 = default(T2);
    //    }
    //}
}