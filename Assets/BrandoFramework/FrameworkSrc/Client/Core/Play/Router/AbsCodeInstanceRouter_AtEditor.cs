#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    /// <summary>
    /// 编辑器程序实例路由器。
    /// 通过给定的键返回目标类型实例。
    /// 基于反射构建实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbsCodeInstanceRouter_AtEditor<T> : AbsCodeInstanceRouter_Base<T>, 
           ICodeInstanceRouter<T> where T : class
    {
        #region 字段及属性

        private Dictionary<string, Type> appTypes;

        private Dictionary<string, Type> AppTypesDict
            => appTypes ?? (appTypes = new Dictionary<string, Type>());

        #endregion

        public T GetInstance(string key)
        {
            //return null;
            //var finalAppId = appId ?? AppEntity.CurrentRuningU3DApp.LocAppId;

            //if (!AppInstances.ContainsKey(finalAppId))
            //{
            //    AppInstances.Add(finalAppId, new Dictionary<string, T>());
            //}

            T instance;

            //var instances = AppInstances[finalAppId];
            if (AppInstances.ContainsKey(key))
            {
                instance = AppInstances[key];
            }
            else
            {
                var type = GetType(key);
                if (type == null)
                {
                    instance = default(T);
                }
                else
                {
                    instance = (T)Activator.CreateInstance(type);
                }
            }

            return instance;
        }

        private Type GetType(string key)
        {
            // 确保目标应用的目标类型字典已缓存
            if (!AppTypesDict.ContainsKey(key))
            {
                //var appAllTypes = AppEntity.GetAppTypes(appId);
                //var types = appAllTypes.Where(t =>
                //    typeof(T).IsAssignableFrom(t)
                //    && !t.IsAbstract
                //    && !t.IsInterface
                //).ToList();

                //var typeDic = new Dictionary<string, Type>();
                //AppTypesDict.Add(appId, typeDic);

                //// 转换为字典
                //foreach (var t in types)
                //{
                //    typeDic.Add(t.Name, t);
                //}
            }

            //var targetAppTypes = AppTypesDict[appId];
            return AppTypesDict.ContainsKey(key) ? AppTypesDict[key] : null;
        }
    }
}

#endif