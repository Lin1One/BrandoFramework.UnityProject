namespace Common
{

    public interface IReset
    {
        void Reset();
    }

    /// <summary>
    /// 泛型对象池接口。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericObjectPool<T> where T : class
    {
        /// <summary>
        /// 获取一个对象实例。
        /// </summary>
        /// <returns></returns>
        T Take();

        /// <summary>
        /// 归还一个对象。
        /// 只有由该对象池创建的实例才能被归还到该对象池。
        /// 返回一个是否归还成功的结果。
        /// </summary>
        /// <returns>The restore.</returns>
        /// <param name="t">T.</param>
        bool Restore(T t);

        /// <summary>
        /// 强制归还一个实例到池子中。
        /// 该操作会无视实例是否由目标对象池所构造。
        /// </summary>
        /// <param name="t"></param>
        void ForceRestore(T t);

        /// <summary>
        /// 当前已使用实例数量。
        /// </summary>
        int UseCount { get; }

        /// <summary>
        /// 清理对象池。
        /// </summary>
        void Clear();
    }
}
