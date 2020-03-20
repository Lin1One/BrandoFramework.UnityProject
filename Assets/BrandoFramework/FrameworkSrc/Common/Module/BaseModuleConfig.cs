#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

namespace Common
{

    public class BaseModuleConfig<T> : IModuleConfig where T:class ,IModuleConfigAble
    {
        public virtual void ApplyConfig()
        {
            Injector.Instance.Get<T>().ApplyConfig(this);
        }
    }
}

