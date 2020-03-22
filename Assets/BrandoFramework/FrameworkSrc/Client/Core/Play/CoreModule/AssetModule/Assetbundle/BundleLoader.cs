using Common;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



#region Head

// Author:                LinYuzhou
// CreateDate:            2019/5/17 10:51:16
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    [Singleton]
    //[DefaultInjecType(typeof(IBundleLoader))]
    public class BundleLoader : LoaderBase<string, IBundleLoadTask, IBundleRef>, IBundleLoader
    {

        public BundleLoader()
        {
            LoadMax = -1;
        }

#if UNITY_EDITOR

        //private IRefAnalyzeProfiler _analyzeProfiler;

        //private IRefAnalyzeProfiler AnalyzeProfiler
        //{
        //    get
        //    {
        //        if (_analyzeProfiler != null)
        //        {
        //            return _analyzeProfiler;
        //        }

        //        _analyzeProfiler = RefAnalyzeProfiler.GetAnalyzeProfiler(U3dGlobal.CurrentAppId);
        //        return _analyzeProfiler;
        //    }
        //}

#endif

        protected override IBundleLoadTask CreateTask() => new BundleLoadTask();
        [Inject]
        protected readonly IBundlePathInfoHelepr BundlePathInfoHelepr;
        [Inject]
        protected readonly IBundlePathHelper BundlePathHelper;
        [Inject]
        protected readonly IBundleDependInfoHelper DependInfoHelper;

        #region 异步加载

        protected override void StartOneLoadAsync(IBundleLoadTask task)
        {
            string path = null;

            path = BundlePathInfoHelepr.GetBundlePath(task.QueryId);
            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.LogError($"目标bundle路径{path}不存在!");
#endif
            }

            var request = AssetBundle.LoadFromFileAsync(path);

#if UNITY_EDITOR
            ////AnalyzeProfiler.SaveStartLoad(task.TargetBundleId);
#endif

            LoadingIds.Add(task.TargetBundleId);
            request.completed += op =>
            {
                IBundleRef bundleRef = null;
                try
                {
                    LoadingIds.Remove(task.TargetBundleId);

                    if (request.assetBundle == null)
                    {
                        bundleRef = null;
                        if (Buffer.HasValue(task.TargetBundleId))
                        {
                            bundleRef = Buffer.GetValue(task.TargetBundleId);
                        }
                        if (bundleRef == null)
                            throw new Exception($"Bundle__{task.TargetBundleId}加载失败！");
                    }
                    else
                    {
                        bundleRef = new BundleRef();
                        bundleRef.Update(request.assetBundle, OnBundleRelease);
                        SetLoadState(task.TargetBundleId, LoadState.Loaded);
                        Buffer.TryCache(task.TargetBundleId, bundleRef);
                        TryIncreaseBundleRef(bundleRef);
                    }

#if UNITY_EDITOR
                    ////AnalyzeProfiler.SaveEndLoad(task.TargetBundleId);
#endif
                }
                catch(Exception e)
                {
#if DEBUG
                    Debug.LogError(e.Message + e.StackTrace);
#endif
                }
                finally
                {
                    string targetBundleId = task.TargetBundleId;
                    RestoreTask(task);
                    Callbcker.Callback(targetBundleId, bundleRef);
                }
            };

            void TryIncreaseBundleRef(IBundleRef bundleRef)
            {
                if (task.InitiatedAssetId == null)
                {
                    bundleRef.Use();
                }
            }
        }

#if DEBUG
        private int m_countChian; //链式加载堆栈计数，以免死循环
#endif

        public void LoadAsync(string assetId, Action<IBundleRef> callback)
        {
            var assetLowerId = assetId.ToLower();
            var bundleId = BundlePathInfoHelepr.GetBundleId(assetLowerId);
            var loadState = GetLoadState(bundleId);

            switch (loadState)
            {
                case LoadState.NotLoad:
                    OnNotLoad();
                    break;
                case LoadState.Loaded:
                    OnLoaded();
                    break;
                case LoadState.Loading:
                    OnLoading();
                    break;
                default:
                    callback?.Invoke(null);
                    throw new ArgumentOutOfRangeException();
            }

            void OnNotLoad()
            {
                AsyncTask task = AsyncTask.Get();
                task.SetCallback(callback);
                Callbcker.AddCallback(bundleId, task.MainLoadDown);

                var dependInfo = DependInfoHelper.GetDependInfo(bundleId);
                var dependIds = dependInfo.DirectDepends;
                if (dependIds.Count == 0)
                {
                    Callbcker.AddCallback(bundleId, task.OneLoadDown);
                    task.AddCount();
                    SetLoadState(bundleId, LoadState.Loading);
                    WaitTasks.Enqueue(CreateMainTask(assetLowerId, bundleId));
                }
                else
                {
#if DEBUG
                    m_countChian = 0;
#endif
                    PushChianTask(bundleId, dependInfo, task);
                }
            }

            void OnLoaded()
            {
                callback(Buffer.GetValue(bundleId));
                Buffer.As<BundleBuffer>().IncreaseRef(bundleId);
            }

            void OnLoading()
            {
                Callbcker.AddCallback(bundleId, callback);
            }
        }

        private IBundleLoadTask CreateMainTask(string assetId, string bundleId)
        {
            var task = TakeTask();
            task.InitiatedAssetId = assetId;
            task.InitiatedBundleId = bundleId;
            task.TargetBundleId = bundleId;
            return task;
        }

        private void PushChianTask(string initiatedBundleId, BundleDependInfo dependInfo,AsyncTask asyncTask)
        {

#if DEBUG
            m_countChian++;
            if (m_countChian > 10000)
            {
                Debug.LogError("严重错误：异步加载堆栈超过10000，请检查加载逻辑或资源依赖关系：" + initiatedBundleId);
                return;
            }
#endif

            var dependIds = dependInfo.DirectDepends;
            var dependCount = dependIds.Count;
            var index = 0;

            LoadState loadState = GetLoadState(dependInfo.BundleId);

            if (loadState == LoadState.AddTask)
            {
                return;
            }
            SetLoadState(dependInfo.BundleId, LoadState.AddTask);

            while (index < dependCount)
            {
                var sonId = dependIds[index];

                loadState = GetLoadState(sonId);

                if (loadState != LoadState.NotLoad)
                {
                    index++;
                    continue;
                }

                var sonDepndInfo = DependInfoHelper.GetDependInfo(sonId);
                if (sonDepndInfo.DirectDepends.Count == 0)
                {
                    var sonTask = TakeTask();
                    asyncTask.AddCount();
                    Callbcker.AddCallback(sonId, asyncTask.OneLoadDown);
                    sonTask.InitiatedBundleId = initiatedBundleId;
                    sonTask.TargetBundleId = sonId;
                    //LoadingIds.Add(sonId);
                    WaitTasks.Enqueue(sonTask);
                    SetLoadState(sonId, LoadState.Loading);
                    index++;

#if UNITY_EDITOR
                    ////AnalyzeProfiler.AddBundleToBundleRef(sonId, dependInfo.BundleId);
#endif
                }
                else
                {
                    PushChianTask(initiatedBundleId, sonDepndInfo,asyncTask);
                    index++;
                }
            }

            //loadState = GetLoadState(dependInfo.BundleId);

            //if (loadState != LoadState.NotLoad)
            //{
            //    return;
            //}

            var task = TakeTask();
            asyncTask.AddCount();
            Callbcker.AddCallback(dependInfo.BundleId, asyncTask.OneLoadDown);
            task.InitiatedBundleId = initiatedBundleId;
            task.TargetBundleId = dependInfo.BundleId;
            //LoadingIds.Add(dependInfo.BundleId);
            WaitTasks.Enqueue(task);
            SetLoadState(task.TargetBundleId, LoadState.Loading);

#if UNITY_EDITOR
            if (task.IsMainTask)
            {
                ////AnalyzeProfiler.AddAssetToBundleRef(task.TargetBundleId, initiatedBundleId);
            }
            else
            {
                ////AnalyzeProfiler.AddBundleToBundleRef(task.TargetBundleId, initiatedBundleId);
            }
#endif
        }

        #endregion

        #region BundleRef对象获取及回收

        private readonly IGenericObjectPool<IBundleRef> _bundleRefPool
            = new GenericObjectPool<IBundleRef>(AssetFactory.CreateBundleRef, 100);

        private IBundleRef TakeRef() => _bundleRefPool.Take();
        private void RestoreRef(IBundleRef bundleRef) => _bundleRefPool.Restore(bundleRef);

        #endregion

        #region 同步加载

        public IBundleRef Load(string assetId)
                {
                    var assetLowerId = assetId.ToLower();
                    var bundleId = BundlePathInfoHelepr.GetBundleId(assetLowerId);
                    var loadState = GetLoadState(bundleId);

                    switch (loadState)
                    {
                        case LoadState.NotLoad:
                            return OnNotLoad();
                        case LoadState.Loaded:
                            return OnLoaded();
                        case LoadState.Loading:
                            //Todo 待完成
                            //Debug.LogError("错误，在异步加载中，调用了词资源的同步加载");
                            return OnNotLoad();
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    IBundleRef OnNotLoad()
                    {
                        var dependInfo = DependInfoHelper.GetDependInfo(bundleId);
                        if(dependInfo == null)
                        {
                            dependInfo = new BundleDependInfo(bundleId, new string[0]);
                        }
        #if DEBUG
                        m_countChian = 0;
        #endif

                        return LoadBundleChian(bundleId, dependInfo);
                    }

                    IBundleRef OnLoaded()
                    {
                        var bundleRef = Buffer.GetValue(bundleId);
                        return bundleRef;
                    }

                    IBundleRef LoadBundleChian(string initiatedBundleId, BundleDependInfo dependInfo)
                    {
        #if DEBUG
                        m_countChian++;
                        if (m_countChian > 100)
                        {
                            Debug.LogError("严重错误：同步加载堆栈超过100，请检查加载逻辑或资源依赖关系：" + initiatedBundleId);
                            return null;
                        }
        #endif

                        var dependIds = dependInfo.DirectDepends;
                        var dependCount = dependIds.Count;

                        var loadState2 = GetLoadState(dependInfo.BundleId);
                        IBundleRef bundleRef2;
                        if (loadState2 == LoadState.Loaded)
                        {
                            bundleRef2 = Buffer.GetValue(dependInfo.BundleId);
                            bundleRef2.Use();
                            return bundleRef2;
                        }
                        else if (loadState2 == LoadState.AddTask)
                        {
                            return null;
                        }
                        SetLoadState(dependInfo.BundleId, LoadState.AddTask);

                        for (int index = 0; index < dependCount; index++)
                        {
                            var sonId = dependIds[index];

                            var sonDepndInfo = DependInfoHelper.GetDependInfo(sonId);
                            if (sonDepndInfo.DirectDepends.Count > 0)
                            {
                                LoadBundleChian(initiatedBundleId, sonDepndInfo);
                            }
                            else
                            {
                                loadState2 = GetLoadState(sonId);
                                IBundleRef bundleRef;
                                if (loadState2 != LoadState.NotLoad)
                                {
                                    if (loadState2 == LoadState.Loaded)
                                    {
                                        bundleRef = Buffer.GetValue(sonId);
                                        bundleRef.Use();
                                        continue;
                                    }
                                    else if (loadState2 == LoadState.AddTask)
                                    {
                                        continue;
                                    }
                                }

                                var bundlePath = BundlePathInfoHelepr.GetBundlePath(sonId);

        #if UNITY_EDITOR
                                ////AnalyzeProfiler.SaveStartLoad(sonId);
        #endif

                                var assetBundle = AssetBundle.LoadFromFile(bundlePath);
                                bundleRef = new BundleRef();
                                if (assetBundle != null)
                                {
                                    bundleRef.Update(assetBundle, OnBundleRelease);
                                    bundleRef.Use();
                                    Buffer.TryCache(sonId, bundleRef);
                                    SetLoadState(sonId, LoadState.Loaded);
                                }
                                else
                                {
                                    SetLoadState(sonId, LoadState.NotLoad);
                                }

        #if UNITY_EDITOR
                                ////AnalyzeProfiler.AddBundleToBundleRef(sonId, dependInfo.BundleId);
        #endif
                            }

                        }

                        loadState2 = GetLoadState(dependInfo.BundleId);
                        if (loadState2 == LoadState.Loaded)  //Todo 如果在异步加载中应该怎么设计？
                        {
                            bundleRef2 = Buffer.GetValue(dependInfo.BundleId);
                            bundleRef2.Use();
                            return bundleRef2;
                        }
                        var bundlePath2 = BundlePathInfoHelepr.GetBundlePath(dependInfo.BundleId);
        #if UNITY_EDITOR
                        ////AnalyzeProfiler.SaveStartLoad(dependInfo.BundleId);
        #endif
                        var assetBundle2 = AssetBundle.LoadFromFile(bundlePath2);
                        bundleRef2 = new BundleRef();
                        if (assetBundle2 != null)
                        {
                            bundleRef2.Update(assetBundle2, OnBundleRelease);
                            bundleRef2.Use();
                            Buffer.TryCache(dependInfo.BundleId, bundleRef2);
                            SetLoadState(dependInfo.BundleId, LoadState.Loaded);
                        }
                        else
                        {
                            SetLoadState(dependInfo.BundleId, LoadState.NotLoad);
                            return null;
                        }

        #if UNITY_EDITOR
                        ////AnalyzeProfiler.AddBundleToBundleRef(initiatedBundleId, dependInfo.BundleId);
        #endif
                        return bundleRef2;
                    }
                }

        #endregion

        #region 引用+1

                public void Use(string assetId)
                {
                    var bundleId = BundlePathInfoHelepr.GetBundleId(assetId);
                    var bundleRef = Buffer.GetValue(bundleId);
                    bundleRef.Use();
                }

        #endregion

        #region 卸载

                public void ReleaseAsset(List<string> assetIds)
                {
                    foreach (var assetId in assetIds)
                    {
                        var bundleId = BundlePathInfoHelepr.GetBundleId(assetId);
                        Buffer.GetValue(bundleId).Unuse();

        #if UNITY_EDITOR
                        ////AnalyzeProfiler.RemoveAssetToBundleRef(bundleId, assetId);
        #endif
                    }
                }

                public void ReleaseTarget(string assetId)
                {
                    var bundleId = BundlePathInfoHelepr.GetBundleId(assetId);
                    var bundleRef = Buffer.GetValue(bundleId);
                    bundleRef.Unuse();
                    SetLoadState(bundleId, LoadState.NotLoad);
                    //            TryRestoreBundleRef(bundleRef);

        #if UNITY_EDITOR
                    ////AnalyzeProfiler.RemoveAssetToBundleRef(bundleId, assetId);
        #endif
                }

                public void SetCurrentAppAssetBundleInfo(byte[] bytes)
                {
                    //IYuU3dAppEntity appEntity = YuU3dAppUtility.Injector.Get<IYuU3dAppEntity>();
                    //var locApp = appEntity.CurrentRuningU3DApp;
                    //currentAppAssetBundleInfo = YuSerializeUtility.DeSerialize<YuAppAssetBundleInfo>(bytes);
                }

                //        private void TryRestoreBundleRef(IBundleRef bundleRef)
                //        {
                //            if (bundleRef.RefCount > 0)
                //            {
                //                return;
                //            }
                //
                //            RestoreRef(bundleRef);
                //        }

                private void OnBundleRelease(string bundleId)
                {
                    SetLoadState(bundleId, LoadState.NotLoad);
                    Buffer.TryRemove(bundleId);
                    // todo 从Buff中移除bundleref

                    var dependInfo = DependInfoHelper.GetDependInfo(bundleId);
                    foreach (var directDepend in dependInfo.DirectDepends)
                    {
                        var bundleRef = Buffer.GetValue(directDepend);
                        bundleRef.Unuse();

        #if UNITY_EDITOR
                        ////AnalyzeProfiler.RemoveBundleToBundleRef(directDepend, bundleId);
        #endif
                    }
                }

                #endregion

        #region  异步加载任务
        //异步加载任务
        private class AsyncTask
        {
            #region 池
            public static AsyncTask Get()
            {
                if(pools.Count >0)
                {
                    var temp = pools[pools.Count - 1];
                    temp.Reset();
                    pools.RemoveAt(pools.Count - 1);
                    return temp;
                }
                return new AsyncTask();
            }
            public static void Recover(AsyncTask obj)
            {
                if(pools.Contains(obj) || obj == null)
                {
#if DEBUG
                    Debug.LogError("错误：Bundle异步加载任务池回收异常");
#endif
                    return;
                }
                pools.Add(obj);
            }

            private static readonly List<AsyncTask> pools = new List<AsyncTask>();

            #endregion


            //public string AssetId;                  //加载的资源id
            private Action<IBundleRef> m_callback;     //加载成功后的回调
            private IBundleRef m_ref;               //主任务bundle引用
            private int m_count;            //需要加载的bundle数
            
            public void SetCallback(Action<IBundleRef> callback)
            {
                m_callback = callback;
            }

            public void AddCount()
            {
                m_count++;
            }

            public void OneLoadDown(IBundleRef _ref)
            {
                m_count--;
                CheckEnd();
            }

            public void MainLoadDown(IBundleRef _ref)
            {
                m_ref = _ref;
            }

            private void CheckEnd()
            {
                if(m_count == 0)
                {
                    m_callback(m_ref);
                    m_callback = null;
                    m_ref = null;

                    Recover(this);
                }
            }

            public void Reset()
            {
                m_ref = null;
                m_count = 0;
                m_callback = null;
            }
        }
        #endregion
    }


}