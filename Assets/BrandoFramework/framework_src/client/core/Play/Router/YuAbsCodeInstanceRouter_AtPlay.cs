using System;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 运行时程序实例路由器。
    /// 通过给定的键返回目标类型实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuAbsCodeInstanceRouter_AtPlay<T>
        : YuAbsCodeInstanceRouter_Base<T>, IYuCodeInstanceRouter_AtPlay<T> where T : class
    {
        #region 字段及属性

        private Dictionary<string, Dictionary<string, Func<T>>> appFuncs;

        private Dictionary<string, Dictionary<string, Func<T>>> AppFuncs
            => appFuncs ?? (appFuncs = new Dictionary<string, Dictionary<string, Func<T>>>());

        #endregion


        public T GetInstance(string key, string appId = null)
        {
            //var finalAppId = appId ?? AppEntity.CurrentRuningU3DApp.LocAppId;

            ////if (!AppInstances.ContainsKey(finalAppId))
            ////{
            ////    AppInstances.Add(finalAppId, new Dictionary<string, T>());
            ////}

            T instance;

            ////var instances = AppInstances[finalAppId];
            ////if (instances.ContainsKey(key))
            ////{
            ////    instance = instances[key];
            ////}
            ////else
            ////{
            ////    var func = GetFunc(key, finalAppId);
            ////    instance = func();
            ////}

            return null;
        }

        private Func<T> GetFunc(string key, string appId)
        {
            var funcs = AppFuncs[appId];
            var func = funcs[key];
            return func;
        }

        public void RegisterFunc(string key, Func<T> func, string appId = null)
        {
            ////var finalAppId = appId ?? AppEntity.CurrentRuningU3DApp.LocAppId;

            ////if (!AppFuncs.ContainsKey(finalAppId))
            ////{
            ////    AppFuncs.Add(finalAppId, new Dictionary<string, Func<T>>());
            ////}

            ////var funcs = AppFuncs[finalAppId];
            ////if (funcs.ContainsKey(key))
            ////{
            ////    return;
            ////}

            ////funcs.Add(key, func);
        }


    }
}