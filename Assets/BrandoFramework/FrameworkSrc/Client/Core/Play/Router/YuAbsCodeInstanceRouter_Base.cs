using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 程序实例路由器基础。
    /// 提供实例路由器所需的基础数据结构。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class YuAbsCodeInstanceRouter_Base<T> where T : class
    {
        #region 属性

        private Dictionary<string, Dictionary<string, T>> appInstances;

        protected Dictionary<string, Dictionary<string, T>> AppInstances
        {
            get
            {
                if (appInstances != null)
                {
                    return appInstances;
                }

                // 容量设置为3，绝大多数情况下同时运行3个应用应该足够，如不够则自动扩容
                appInstances = new Dictionary<string, Dictionary<string, T>>(3);
                return appInstances;
            }
        }

        //private IYuU3dAppEntity appEntity;

        //protected IYuU3dAppEntity AppEntity => appEntity ?? (appEntity = YuU3dAppUtility.Injector.Get<IYuU3dAppEntity>());

        #endregion

        public void RedirectInstance(string key, T instance, string appId = null)
        {
            ////var finalAppId = appId ?? AppEntity.CurrentRuningU3DApp.LocAppId;

            ////if (!AppInstances.ContainsKey(finalAppId))
            ////{
            ////    return;
            ////}

            ////var instances = AppInstances[finalAppId];
            ////if (!instances.ContainsKey(key))
            ////{
            ////    return;
            ////}

            ////instances[key] = instance;
        }

    }
}