using Client;
using Common;

namespace Client
{
    /// <summary>
    /// 以分帧执行的方式构造目标类型的所有实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Singleton]
    public abstract class YuAbsCodeInstanceRouter<T>
        : IYuCodeInstanceRouter<T> where T : class
    {
        private IYuCodeInstanceRouter<T> router;

        private IYuCodeInstanceRouter<T> Router
        {
            get
            {
                if (router != null)
                {
                    return router;
                }

#if UNITY_EDITOR
                router = new YuAbsCodeInstanceRouter_AtEditor<T>();
#elif UNITY_ANDROID
                router = new YuAbsCodeInstanceRouter_AtAndroid<T>();
#else 
                router = new YuAbsCodeInstanceRouter_AtPlay<T>();
#endif

                return router;
            }
        }

        public  T GetInstance(string key, string appId = null)
        {
            return Router.GetInstance(key, appId);
        }

        public void RedirectInstance(string key, T instance, string appId = null)
        {
            Router.RedirectInstance(key, instance, appId);
        }
    }
}