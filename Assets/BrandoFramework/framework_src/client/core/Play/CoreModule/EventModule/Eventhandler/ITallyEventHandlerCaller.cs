using System;

namespace Client.Core
{

    public interface IEventHandlerList
    {
        /// <summary>
        /// 添加一个事件处理器
        /// </summary>
        /// <param name="eventAction">事件处理委托</param>
        /// <param name="executeCount">事件处理委托的执行次数，默认为-1即不限制次数</param>
        int AddHandler(Delegate eventAction);

        /// <summary>
        /// 移除一个事件处理委托
        /// </summary>
        /// <param name="evenAction"></param>
        void RemoveHandler(Delegate evenAction);

        /// <summary>
        /// 通过全局唯一的事件处理器Id移除一个事件处理器。
        /// </summary>
        /// <param name="handlerId">Handler identifier.</param>
        void RemoveHandler(int handlerId);

        /// <summary>
        /// 调用所有的事件处理器
        /// </summary>
        void CallEventHanlder(object data);

        int EventHanlderCount { get; }

    }

    ///// <summary>
    ///// 泛型可计数事件处理器调用者接口，该调用者可以调用需要一个泛型参数的事件处理器
    ///// </summary>
    //public interface IEventHandlerList<TData>
    //{
    //    /// <summary>
    //    /// 添加一个事件处理器
    //    /// </summary>
    //    /// <param name="eventAction">事件处理委托</param>
    //    /// <param name="executeCount">事件处理委托的执行次数，默认为-1即不限制次数</param>
    //    int AddHandler(Action<TData> eventAction);

    //    /// <summary>
    //    /// 移除一个事件处理委托
    //    /// </summary>
    //    /// <param name="evenAction"></param>
    //    void RemoveHandler(Action<TData> evenAction);

    //    /// <summary>
    //    /// 通过全局唯一的事件处理器Id移除一个事件处理器。
    //    /// </summary>
    //    /// <param name="handlerId">Handler identifier.</param>
    //    void RemoveHandler(int handlerId);

    //    /// <summary>
    //    /// 调用所有的泛型事件处理器
    //    /// </summary>
    //    void CallEventHanlder(TData td);

    //    int EventHanlderCount { get; }

    //}

    ///// <summary>
    ///// 泛型可计数事件处理器调用者接口，该调用者可以调用需要一个泛型参数的事件处理器
    ///// </summary>
    //public interface IEventHandlerList<T1,T2>
    //{
    //    /// <summary>
    //    /// 添加一个事件处理器
    //    /// </summary>
    //    /// <param name="eventAction">事件处理委托</param>
    //    /// <param name="executeCount">事件处理委托的执行次数，默认为-1即不限制次数</param>
    //    int AddHandler(Action<T1, T2> eventAction);

    //    /// <summary>
    //    /// 移除一个事件处理委托
    //    /// </summary>
    //    /// <param name="evenAction"></param>
    //    void RemoveHandler(Action<T1, T2> evenAction);

    //    /// <summary>
    //    /// 通过全局唯一的事件处理器Id移除一个事件处理器。
    //    /// </summary>
    //    /// <param name="handlerId">Handler identifier.</param>
    //    void RemoveHandler(int handlerId);

    //    /// <summary>
    //    /// 调用所有的泛型事件处理器
    //    /// </summary>
    //    void CallEventHanlder(T1 data, T2 data2);

    //    int EventHanlderCount { get; }
    //}

}

