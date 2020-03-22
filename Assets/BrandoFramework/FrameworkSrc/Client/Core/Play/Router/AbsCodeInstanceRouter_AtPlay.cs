using System;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 运行时程序实例路由器。
    /// 通过给定的键返回目标类型实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbsCodeInstanceRouter_AtPlay<T> : AbsCodeInstanceRouter_Base<T>, 
        ICodeInstanceRouter_AtPlay<T> where T : class
    {
        #region 字段及属性

        private Dictionary<string, Func<T>> appFuncs;

        private Dictionary<string, Func<T>> AppFuncs
            => appFuncs ?? (appFuncs = new Dictionary<string, Func<T>>());

        #endregion


        public T GetInstance(string key)
        {
            T instance;

            if (AppInstances.ContainsKey(key))
            {
                instance = AppInstances[key];
            }
            else
            {
                var func = GetFunc(key);
                instance = func();
            }

            return null;
        }

        private Func<T> GetFunc(string key)
        {
            var func = AppFuncs[key];
            return func;
        }

        public void RegisterFunc(string key, Func<T> func)
        {
            if (AppFuncs.ContainsKey(key))
            {
                return;
            }
            AppFuncs.Add(key, func);
        }


    }
}