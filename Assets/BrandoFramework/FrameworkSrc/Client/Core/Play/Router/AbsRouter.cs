#region Head

// Author:            LinYuzhou
// CreateDate:        2018/8/20 7:46:36
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using System.Collections.Generic;


namespace Client
{
    /// <summary>
    /// 路由器抽象基类。
    /// 所有业务路由器都从该类继承。
    /// 每个业务路由器都为单例类型。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    [Singleton]
    public abstract class AbsRouter<TKey, TInstance> : IRouter<TKey, TInstance>
    {
        protected readonly Dictionary<TKey, TInstance> Instances
            = new Dictionary<TKey, TInstance>();

        /// <summary>
        /// 获取一个实例。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TInstance Get(TKey key)
        {
            if (Instances.ContainsKey(key))
            {
                return Instances[key];
            }

#if UNITY_EDITOR
           // YuDebugUtility.LogError($"键为{key}的目标实例不存在，请先用菜单项构建对应的映射关系！");
#endif

            return default(TInstance);
        }

        public virtual void Redirect(TKey key, TInstance instance)
        {
            if (Instances.ContainsKey(key))
            {
                Instances[key] = instance;
                return;
            }

#if UNITY_EDITOR
            //YuDebugUtility.LogError(
            //    $"目标键为{key}的实例不存在，无法重定向！");
#endif
        }

        public virtual void AddMap(TKey key, TInstance instance)
        {
            if (!Instances.ContainsKey(key))
            {
                Instances.Add(key, instance);
                return;
            }

#if UNITY_EDITOR
            //YuDebugUtility.LogError(
             //   $"目标键为{key}的路由规则已存在，无法添加！");
#endif
        }

        public bool HasInstance(TKey key) => Instances.ContainsKey(key);
    }
}