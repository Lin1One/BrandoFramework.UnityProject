#region Head

// Author:            LinYuzhou
// CreateDate:        2018/5/26 9:07:32
// Email:             836045613@qq.com

#endregion

namespace Client
{
    /// <summary>
    /// 路由器，用于依据指定类型的key获取制定类型的实例。
    /// 可以在运行时修改路由规则而重定向实例获取。
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TInstance">实例类型</typeparam>
    public interface IRouter<TKey, TInstance>
    {
        /// <summary>
        /// 获取一个实例。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TInstance Get(TKey key);

        /// <summary>
        /// 修改指定key的路由目标。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="instance"></param>
        void Redirect(TKey key, TInstance instance);

        /// <summary>
        /// 增加一个映射。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="instance"></param>
        void AddMap(TKey key, TInstance instance);
    }

    /// <summary>
    /// 行为路由器。
    /// 该路由器的实例必须是可以被无参调用的行为处理器。
    /// 可以使用制定的key手动触发对应的处理器的行为处理逻辑。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    //public interface IYuActionRouter<TKey, THandler> : IYuRouter<TKey, THandler>
    //    where THandler : IYuActionHandler
    //{
    //    /// <summary>
    //    /// 触发目标行为处理器的逻辑处理。
    //    /// </summary>
    //    /// <param name="key"></param>
    //    bool TriggerAction(TKey key);
    //}
}