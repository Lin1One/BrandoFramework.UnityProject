using System;
using System.Collections.Generic;

namespace Common
{
    public interface IReset
    {
        void Reset();
    }

    /// <summary>
    /// 基础泛型对象池。
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T> where T : class
    {
        /// <summary>
        /// 当前还未使用处于空闲状态的实例列表。
        /// </summary>
        private List<T> AvaliableObjects { get; }

        /// <summary>
        /// 当前被调度使用的实例列表。
        /// </summary>
        private List<T> DispatchedObjects { get; set; }

        private readonly Func<T> createFunc;
        private readonly Action<T> onCreated;
        private readonly string key;
        private readonly Action<T> destroy;
        private readonly int initCount;

        /// <summary>
        /// 构建一个对象池。
        /// </summary>
        /// <param name="createFunc">用于创建实例的委托。</param>
        /// <param name="initCount">初始化的实例数量。</param>
        /// <param name="poolKey">对象的友好命名。</param>
        /// <param name="onCreated">实例创建完毕的委托。</param>
        /// <param name="destroy">清理实例时的委托。</param>
        public ObjectPool
        (
            Func<T> createFunc,
            int initCount,
            string poolKey = null,
            Action<T> onCreated = null,
            Action<T> destroy = null
        )
        {
            AvaliableObjects = new List<T>(initCount);

            this.initCount = initCount;
            this.createFunc = createFunc;
            this.onCreated = onCreated;
            this.destroy = destroy;
            key = poolKey;

            for (var i = 0; i < initCount; i++)
            {
                var instance = createFunc();
                onCreated?.Invoke(instance);
                AvaliableObjects.Add(instance);
            }
        }

        public T Take()
        {
            T instance;

            if (AvaliableObjects.Count == 0)
            {
                instance = createFunc();
                onCreated?.Invoke(instance);
            }
            else
            {
                instance = AvaliableObjects[AvaliableObjects.Count - 1];
                AvaliableObjects.Remove(instance);
            }

            if (DispatchedObjects == null)
            {
                DispatchedObjects = new List<T>(initCount);
            }

            DispatchedObjects.Add(instance);
            return instance;
        }

        public bool Restore(T t)
        {
            //  必须是由该池创建的对象才允许被归还到池内。
            if (t == null || !DispatchedObjects.Contains(t))
            {
                return false;
            }

            var reset = t as IReset;
            reset?.Reset();
            DispatchedObjects.Remove(t);
            AvaliableObjects.Add(t);
            return true;
        }

        public void ForceRestore(T t)
        {
            var reset = t as IReset;
            reset?.Reset();
            AvaliableObjects.Add(t);
        }

        private void RestoreAll()
        {
            while (DispatchedObjects.Count > 0)
            {
                Restore(DispatchedObjects[DispatchedObjects.Count - 1]);
            }
        }

        public int UseCount
        {
            get { return DispatchedObjects.Count; }
        }

        public void Clear()
        {
            RestoreAll();

            foreach (var instance in AvaliableObjects)
            {
                if (instance != null)
                {
                    destroy?.Invoke(instance);
                }
            }

            AvaliableObjects.Clear();
            DispatchedObjects.Clear();
        }
    }
}
