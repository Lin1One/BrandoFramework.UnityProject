using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// 程序实例路由器基础。
    /// 提供实例路由器所需的基础数据结构。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AbsCodeInstanceRouter_Base<T> where T : class
    {
        #region 属性

        private Dictionary<string, T> appInstances;

        protected Dictionary<string, T> AppInstances
        {
            get
            {
                if (appInstances == null)
                {
                    appInstances = new Dictionary<string, T>();
                }
                return appInstances;
            }
        }

        #endregion

        public void RedirectInstance(string key, T instance)
        {
            if (!AppInstances.ContainsKey(key))
            {
                return;
            }

            AppInstances[key] = instance;
        }

    }
}