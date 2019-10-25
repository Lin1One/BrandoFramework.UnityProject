using System;

namespace Client.Core
{
    #region 非泛型

    /// <summary>
    /// 事件处理器接口。
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        void HandleEvent();

        /// <summary>
        /// 判断委托是否匹配
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        bool CheckMatch(Action action);

        /// <summary>
        /// 全局唯一的事件处理器Id，用于脚本层移除指定的事件处理器。
        /// </summary>
        /// <value>The identifier.</value>
        int Id { get; }
    }

    #endregion

    #region 泛型

    /// <summary>
    /// 事件处理器接口（泛型数据）
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IEventHandler<TData>
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="eventData"></param>
        void HandleEvent(TData eventData);

        /// <summary>
        /// 判断是否是指定委托
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        bool CheckMatch(Action<TData> action);

        /// <summary>
        /// 全局唯一的事件处理器Id，用于脚本层移除指定的事件处理器。
        /// </summary>
        /// <value>The identifier.</value>
        int Id { get; }
    }

    #endregion
}