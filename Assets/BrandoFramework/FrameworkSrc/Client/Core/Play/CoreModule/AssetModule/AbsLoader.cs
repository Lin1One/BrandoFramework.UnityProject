
#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56 
// Email:                 836045613@qq.com

#endregion

using Common;
using System.Collections.Generic;

namespace Client.Core
{
    public abstract class LoaderBase<TKey, TTask, TCallbackValue> : IAssetLoader<TKey, TTask, TCallbackValue>
         where TTask : class
         where TCallbackValue : class
    {
        private IU3DEventModule eventModule;
        private IU3DEventModule EventModule
        {
            get
            {
                if (eventModule == null)
                {
                    eventModule = Injector.Instance.Get<IU3DEventModule>();
                }
                return eventModule;
            }
        }

        public LoaderBase()
        {
            EventModule.WatchUnityEvent(UnityEventType.FixedUpdate, LoadUpdate);
        }


        #region 注入属性

        [Inject]
        protected readonly IBuffer<TKey, TCallbackValue> Buffer;
        [Inject]
        protected readonly ILoadCallbcker<TKey, TCallbackValue> Callbcker;

        #endregion

        #region 任务对象池

        private IGenericObjectPool<TTask> _taskPol;

        private IGenericObjectPool<TTask> TaskPool
        {
            get
            {
                if (_taskPol != null)
                {
                    return _taskPol;
                }

                _taskPol = new GenericObjectPool<TTask>(CreateTask, 100);
                return _taskPol;
            }
        }

        protected TTask TakeTask() => TaskPool.Take();
        protected void RestoreTask(TTask task) => TaskPool.Restore(task);

        protected abstract TTask CreateTask();

        #endregion

        #region 任务队列及阀门控制

        protected Queue<TTask> WaitTasks { get; } = new Queue<TTask>();
        protected HashSet<TKey> LoadingIds { get; } = new HashSet<TKey>();
        protected int WaitingCount => WaitTasks.Count;
        protected int LoadingCount => LoadingIds.Count;
        protected int LoadMax { get; set; } = 3;

        #endregion

        #region 加载状态

        protected abstract void StartOneLoadAsync(TTask task);

        private readonly Dictionary<TKey, LoadState> _assetLoadStates
            = new Dictionary<TKey, LoadState>();

        protected void SetLoadState(TKey key, LoadState loadState)
        {
            if (!_assetLoadStates.ContainsKey(key))
            {
                _assetLoadStates.Add(key, loadState);
            }
            else
            {
                _assetLoadStates[key] = loadState;
            }
        }

        protected LoadState GetLoadState(TKey key)
        {
            var state = !_assetLoadStates.ContainsKey(key)
                ? LoadState.NotLoad
                : _assetLoadStates[key];
            return state;
        }

        #endregion

        #region 加载循环

        protected virtual void LoadUpdate()
        {
            while (WaitTasks.Count > 0)
            {
                var task = WaitTasks.Dequeue();
                StartOneLoadAsync(task);
            }
        }

        #endregion
    }
}