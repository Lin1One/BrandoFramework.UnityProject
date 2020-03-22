using Client;
using Common;

namespace Client
{
    /// <summary>
    /// 以分帧执行的方式构造目标类型的所有实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Singleton]
    public abstract class AbsCodeInstanceRouter<T> : ICodeInstanceRouter<T> where T : class
    {
        private ICodeInstanceRouter<T> router;

        private ICodeInstanceRouter<T> Router
        {
            get
            {
                if (router != null)
                {
                    return router;
                }

#if UNITY_EDITOR
                router = new AbsCodeInstanceRouter_AtEditor<T>();
#elif UNITY_ANDROID
                router = new YuAbsCodeInstanceRouter_AtAndroid<T>();
#else 
                router = new YuAbsCodeInstanceRouter_AtPlay<T>();
#endif
                return router;
            }
        }

        public  T GetInstance(string key)
        {
            return Router.GetInstance(key);
        }

        public void RedirectInstance(string key, T instance)
        {
            Router.RedirectInstance(key, instance);
        }
    }
}