using System;

namespace Client.Core
{
    #region 非泛型

    public interface ITallyEventHandlerCaller
    {
        /// <summary>
        /// 添加一个事件处理器
        /// </summary>
        /// <param name="eventAction">事件处理委托</param>
        /// <param name="executeCount">事件处理委托的执行次数，默认为-1即不限制次数</param>
        int AddHandler(Action eventAction, int executeCount = -1);

        /// <summary>
        /// 移除一个事件处理委托
        /// </summary>
        /// <param name="evenAction"></param>
        void RemoveHandler(Action evenAction);

        /// <summary>
        /// 通过全局唯一的事件处理器Id移除一个事件处理器。
        /// </summary>
        /// <param name="handlerId">Handler identifier.</param>
        void RemoveHandler(int handlerId);

        /// <summary>
        /// 事件处理器数量
        /// </summary>
        int EventHanlderCount { get; }

        /// <summary>
        /// 调用所有的事件处理器
        /// </summary>
        void CallEventHanlder();

        /// <summary>
        /// 移除执行次数为零的事件处理器
        /// </summary>
        void ClearZero();
    }

    #endregion

    #region 泛型

    /// <summary>
    /// 泛型可计数事件处理器调用者接口，该调用者可以调用需要一个泛型参数的事件处理器
    /// </summary>
    public interface ITallyEventHandlerCaller<TData>
    {
        /// <summary>
        /// 添加一个事件处理器
        /// </summary>
        /// <param name="eventAction">事件处理委托</param>
        /// <param name="executeCount">事件处理委托的执行次数，默认为-1即不限制次数</param>
        int AddHandler(Action<TData> eventAction, int executeCount = -1);

        /// <summary>
        /// 移除一个事件处理委托
        /// </summary>
        /// <param name="evenAction"></param>
        void RemoveHandler(Action<TData> evenAction);

        /// <summary>
        /// 通过全局唯一的事件处理器Id移除一个事件处理器。
        /// </summary>
        /// <param name="handlerId">Handler identifier.</param>
        void RemoveHandler(int handlerId);

        /// <summary>
        /// 事件处理器数量
        /// </summary>
        int EventHanlderCount { get; }

        /// <summary>
        /// 调用所有的泛型事件处理器
        /// </summary>
        void CallEventHanlder(TData td);

        /// <summary>
        /// 移除执行次数为零的事件处理器
        /// </summary>
        void ClearZero();
    }

    #endregion
}

