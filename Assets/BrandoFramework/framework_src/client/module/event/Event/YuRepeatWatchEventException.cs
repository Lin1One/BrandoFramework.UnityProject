using System;

namespace YuCommon
{
    /// <summary>
    /// 事件处理器重复添加异常。
    /// </summary>
    public class YuRepeatWatchEventException : Exception
    {
        public YuRepeatWatchEventException(string message)
            : base(message)
        {
        }
    }
}

