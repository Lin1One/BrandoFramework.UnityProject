using Common;
using System;

namespace client_module_event
{
    /// <summary>
    /// 事件执行器接口。
    /// </summary>
    public interface IYuEventExecutor<T> 
    {
        /// <summary>
        /// 事件数据。
        /// 可能为空。
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// 事件委托。
        /// </summary>
        Action<T> EventDel { get; set; }

        /// <summary>
        /// 执行器的剩余执行次数。
        /// </summary>
        int ResidueCount { get; }

        /// <summary>
        /// 前一个事件执行器。
        /// </summary>
        IYuEventExecutor<T> PrevExecutor { get; set; }

        /// <summary>
        /// 后一个事件执行器。
        /// 可能为空。
        /// </summary>
        IYuEventExecutor<T> NextExecutor { get; set; }

        /// <summary>
        /// 执行事件的相关逻辑。
        /// </summary>
        void ExexuteEvent();
    }


    /// <summary>
    /// 事件执行器。
    /// </summary>
    public class YuEventExecutor<T> : IYuEventExecutor<T>
    {
        /// <summary>
        /// 事件数据。
        /// 可能为空。
        /// </summary>
        public T Data { get; set; }

        public Action<T> EventDel { get; set; }

        /// <summary>
        /// 执行器的剩余执行次数。
        /// </summary>
        public int ResidueCount { get; private set; } = -1;

        /// <summary>
        /// 前一个事件执行器。
        /// 可能为空。
        /// </summary>
        public IYuEventExecutor<T> PrevExecutor { get; set; }

        /// <summary>
        /// 后一个事件执行器。
        /// 可能为空。
        /// </summary>
        public IYuEventExecutor<T> NextExecutor { get; set; }

        public void ExexuteEvent()
        {
            EventDel(Data);
            NextExecutor?.ExexuteEvent();
        }

        /// <summary>
        /// 重置状态以重新使用该实例。
        /// </summary>
        public void Reset()
        {
            Data = default(T);
            ResidueCount = -1;
            PrevExecutor = null;
            NextExecutor = null;
        }
    }

    /// <summary>
    /// 事件执行器对象池。
    /// </summary>
    public static class YuEventExecutorPool<T>
    {
        private static IYuEventExecutor<T> singleEventExecutor;

        private static IObjectPool<IYuEventExecutor<T>> executorPool;

        private static IObjectPool<IYuEventExecutor<T>> ExecutorPool
        {
            get
            {
                if (executorPool != null)
                {
                    return executorPool;
                }

                executorPool = new ObjectPool<IYuEventExecutor<T>>(
                    () => new YuEventExecutor<T>(), 2);
                return executorPool;
            }
        }

        public static IYuEventExecutor<T> Take()
        {
            // 绝大多数泛型事件都只需要一个事件执行器。
            // 当默认的事件执行器数量为空时，则直接创建一个实例赋给默认事件执行器并返回
            // 该操作不启动对象池机制以节省内存。
            if (singleEventExecutor == null)
            {
                singleEventExecutor = new YuEventExecutor<T>();
                return singleEventExecutor;
            }

            // 强制归还默认的事件执行器。
            ExecutorPool.ForceRestore(singleEventExecutor);
            // 再构造一个新的事件执行器并返回。
            var executor = ExecutorPool.Take();
            return executor;
        }

        public static void Restore(IYuEventExecutor<T> executor) => ExecutorPool.Restore(executor);
    }
}       