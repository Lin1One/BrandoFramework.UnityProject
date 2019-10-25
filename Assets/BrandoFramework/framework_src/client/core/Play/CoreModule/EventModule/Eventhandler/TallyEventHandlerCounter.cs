using System.Threading;

namespace Client.Core
{
    /// <summary>
    /// 事件处理器Id计数器，用于为事件处理器实例提供全局唯一的身份Id。
    /// </summary>
    public static class TallyEventHandlerCounter
    {
        private static int EventHandlerId;

        /// <summary>
        /// 获得一个全局唯一的事件处理器身份Id。
        /// </summary>
        /// <returns>The event handler identifier.</returns>
        public static int GetEventHandlerId()
        {
            Interlocked.Increment(ref EventHandlerId);
            return EventHandlerId;
        }
    }
}


