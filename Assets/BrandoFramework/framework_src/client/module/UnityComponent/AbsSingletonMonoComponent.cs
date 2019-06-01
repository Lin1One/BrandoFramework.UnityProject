using UnityEngine;

namespace Client.Module.UnityComponent
{
    /// <summary>
    /// mono单例基类。
    /// 继承自该类的mono组件只允许被创建一个实例.
    /// </summary>
    public abstract class AbsSingletonMonoComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
#if DEBUG
        protected virtual void OnEnable()
        {
            var type = GetType();
            var instances = FindObjectsOfType(type);
            if (instances.Length <= 1)
            {
                return;
            }
            Destroy(this);
            Debug.LogError($"单例Mono类型{type}尝试创建多个实例!");
        }

#endif
    }
}