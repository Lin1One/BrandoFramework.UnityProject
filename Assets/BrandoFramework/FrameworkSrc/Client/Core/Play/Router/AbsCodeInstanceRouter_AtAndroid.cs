

using System;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 编辑器程序实例路由器。
    /// 通过给定的键返回目标类型实例。
    /// 基于反射构建实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbsCodeInstanceRouter_AtAndroid<T>
        : AbsCodeInstanceRouter_Base<T>, ICodeInstanceRouter<T> where T : class
    {
#region 字段及属性

        private  Dictionary<string, Type> appTypes;

        private Dictionary<string, Type> AppTypesDict
            => appTypes ?? (appTypes = new Dictionary<string, Type>());

        public T GetInstance(string key)
        {
            throw new NotImplementedException();
        }

        #endregion

        //public T GetInstance(string key, string appId = null)
        //{
        //    var finalAppId = appId ?? AppEntity.CurrentRuningU3DApp.LocAppId;

        //    if (!AppInstances.ContainsKey(finalAppId))
        //    {
        //        AppInstances.Add(finalAppId, new Dictionary<string, T>());
        //    }

        //    T instance;

        //    var instances = AppInstances[finalAppId];
        //    if (instances.ContainsKey(key))
        //    {
        //        instance = instances[key];
        //    }
        //    else
        //    {
        //        var type = GetType(key, finalAppId);
        //        if (type == null)
        //        {
        //            instance = default(T);
        //        }
        //        else
        //        {
        //            instance = (T)Activator.CreateInstance(type);
        //        }
        //    }

        //    return instance;
        //}

        //private Type GetType(string key, string appId)
        //{
        //    // 确保目标应用的目标类型字典已缓存
        //    if (!AppTypesDict.ContainsKey(appId))
        //    {
        //        var appAllTypes = AppEntity.GetAppTypes(appId);
        //        var types = appAllTypes.Where(t =>
        //            typeof(T).IsAssignableFrom(t)
        //            && !t.IsAbstract
        //            && !t.IsInterface
        //        ).ToList();

        //        var typeDic = new Dictionary<string, Type>();
        //        AppTypesDict.Add(appId, typeDic);

        //        // 转换为字典
        //        foreach (var t in types)
        //        {
        //            typeDic.Add(t.Name, t);
        //        }
        //    }

        //    var targetAppTypes = AppTypesDict[appId];
        //    return targetAppTypes.ContainsKey(key) ? targetAppTypes[key] : null;
        //}
    }
}
