using Client.Utility;
using Common;
using Common.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


#region Head

// Author:                LinYuzhou
// CreateDate:            5/16/2019 11:25:26 PM
// Email:                 LinYuzhou@gmail.com

#endregion


namespace Client.Core
{
    [Singleton]
    [DefaultInjecType(typeof(IAssetModule))]
    public class AssetModule : LoaderBase<string, IAssetLoadTask, UnityEngine.Object>, IAssetModule
    {
        #region 构造
        //private readonly ProjectInfo _appEntity;
        private readonly ProjectInfo projectInfo;

        [Inject]
        protected IAssetInfoHelper assetInfoHelper;
        protected IAssetInfoHelper AssetInfoHelper
        {
            get
            {
                if (assetInfoHelper == null)
                {
                    assetInfoHelper = Injector.Instance.Get<IAssetInfoHelper>();
                }
                return assetInfoHelper;
            }
        }

        public AssetModule()
        {
            LoadMax = 3;
            projectInfo = ProjectInfoDati.GetActualInstance();
            //_appEntity = U3dGlobal.Get<IYuU3dAppEntity>();
            _isLoadBundle = false;//_appEntity.RunSetting.IsLoadFromAssetBundle;
            InitLoadActions();
        }

        #endregion

        public void InitModule()
        {
        }

        #region 注入字段

        [Inject] private readonly IBundleLoader _bundleLoader;

        #endregion



        #region 基础API

        protected override IAssetLoadTask CreateTask() => new AssetLoadTask();

        private string GetAssetPath(string assetId)
        {
            var assetInfo = AssetInfoHelper.GetAssetInfo(assetId);
            var path = assetInfo?.GetEditorPath(projectInfo.ProjectAssetDatabaseDir);
            return path;
        }

        #endregion

        #region 加载

        #region 异步加载

        #region 异步加载单个资源

        public void LoadAsync<TAsset>(string assetId, Action<TAsset> callback) where TAsset : Object
        {
            if (_isLoadBundle)
            {
                if(Buffer.HasValue(assetId))
                {
                    callback(Buffer.GetValue(assetId) as TAsset);
                    return;
                }

                var task = TakeTask();
                task.AssetId = assetId;
                task.AssetType = typeof(TAsset);
                task.Callback = (asset) => { callback?.Invoke(asset as TAsset); };
                WaitTasks.Enqueue(task);
            }
            else
            {
                string path = GetAssetPath(assetId);
                var asset = AssetDatabaseUtility.LoadAssetAtPath<TAsset>(path);
                callback(asset);
            }
        }

        private bool _isLoadBundle;

        protected override void StartOneLoadAsync(IAssetLoadTask task)
        {
            var assetId = task.AssetId;
            var loadState = GetLoadState(assetId);
            var callback = task.Callback;

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
                    throw new ArgumentOutOfRangeException();
            }

            if (LoadingIds.Contains(assetId))
            {
                return;
            }

            void OnNotLoad()
            {
                Callbcker.AddCallback(assetId, ConvertAction);
                SetLoadState(assetId, LoadState.Loading);

                LoadingIds.Add(assetId);
                StartLoadBundle(task);
            }

            void OnLoaded()
            {
                var asset = Buffer.GetValue(assetId);
                callback(asset);
            }

            void OnLoading()
            {
                Callbcker.AddCallback(assetId, ConvertAction);
            }


            void ConvertAction(UnityEngine.Object tCallback)
            {
                LoadingIds.Remove(assetId);
                var finalAsset = tCallback;
                callback?.Invoke(finalAsset);
            }
        }

        private void StartLoadBundle(IAssetLoadTask task)
        {
            var assetT = task.AssetType;
            var loadAction = loadActions[assetT];
            loadAction(task);
        }

        #region 类型转范型加载

        private readonly Dictionary<Type, Action<IAssetLoadTask>> loadActions
            = new Dictionary<Type, Action<IAssetLoadTask>>();

        private void InitLoadActions()
        {
            loadActions.Add(typeof(TextAsset), LoadTextAssetAsync);
            loadActions.Add(typeof(AudioClip), LoadAudioClipAsync);
            loadActions.Add(typeof(GameObject), LoadPrefabAsync);
            loadActions.Add(typeof(Shader), LoadShaderAsync);
            loadActions.Add(typeof(Texture2D), LoadTexture2dAsync);
            loadActions.Add(typeof(Font), LoadFontAsync);
            loadActions.Add(typeof(Material), LoadMaterialAsync);
            loadActions.Add(typeof(RuntimeAnimatorController), LoadAnimaControlAsync);
        }

        private void LoadTextAssetAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<TextAsset>(task);
        private void LoadAudioClipAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<AudioClip>(task);
        private void LoadPrefabAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<GameObject>(task);
        private void LoadShaderAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<Shader>(task);
        private void LoadTexture2dAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<Texture2D>(task);
        private void LoadFontAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<Font>(task);
        private void LoadMaterialAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<Material>(task);
        private void LoadAnimaControlAsync(IAssetLoadTask task) => LoadAssetAsyncAtType<RuntimeAnimatorController>(task);

        private void LoadAssetAsyncAtType<TAsset2>(IAssetLoadTask task) where TAsset2 : UnityEngine.Object
        {
            var id = task.AssetId;
            _bundleLoader.LoadAsync(id, bf =>
            {
                if(Buffer.HasValue(task.AssetId))
                {
                    var asset = Buffer.GetValue(task.AssetId) as TAsset2;
                    Callbcker.Callback(task.AssetId, asset);
                    RestoreTask(task);
                }
                else
                {
                    if (bf != null)
                    {
                        bf.LoadAsync<TAsset2>(id, OnAssetLoaded);
                    }
                    else
                    {
                        OnAssetLoaded(null);
                    }
                }
            }
            );

            void OnAssetLoaded(TAsset2 asset2)
            {
                //_bundleLoader.ReleaseTarget(task.AssetId);
#if UNITY_EDITOR
                if (asset2 != null)
                {
                    RefrashShader(asset2);
                }
                else
                {
                    Debug.LogError("加载ab资源失败：" + id);
                }
#endif
                if (asset2 != null)
                {
                    //HideLostRes(asset2);
                    Buffer.TryCache(task.AssetId, asset2);
                    SetLoadState(task.AssetId, LoadState.Loaded);
                }
                Callbcker.Callback(task.AssetId, asset2);
                RestoreTask(task);
            }
        }

        #endregion

        #endregion

        public void LoadAllAsync<TAsset>(string bundleId, Action<List<TAsset>> callback) where TAsset : Object
        {
            if (_isLoadBundle)
            {
                var bundleRef = _bundleLoader.Load(bundleId);
                bundleRef.LoadAllAsync(callback);
                return;
            }

            var assets = AssetDatabaseUtility.LoadAllAssetsAtPath<TAsset>(GetAssetPath(bundleId));
            callback(assets);
        }

        #endregion

        #region 同步加载

        #endregion

        #endregion

        public TAsset Load<TAsset>(string assetId) where TAsset : Object
        {
            if (Buffer.HasValue(assetId))
            {
                if(_isLoadBundle)
                {
                    _bundleLoader.Use(assetId.ToLower());
                }
                return Buffer.GetValue(assetId) as TAsset;
            }

            TAsset asset;

            if (_isLoadBundle)
            {
                var bundleRef = _bundleLoader.Load(assetId);
                asset = bundleRef?.Load<TAsset>(assetId);

                if (asset == null)
                {
#if DEBUG
                    Debug.LogError("加载ab资源失败：" + assetId);
#endif
                    return null;
                }
#if UNITY_EDITOR
                else
                {
                    RefrashShader(asset);
                }
#endif
                //HideLostRes(asset);
            }
            else
            {
                asset = AssetDatabaseUtility.LoadAssetAtPath<TAsset>(GetAssetPath(assetId));
            }

            Buffer.TryCache(assetId, asset);
            return asset;
        }

        //private void HideLostRes(Object asset)
        //{
        //    if (asset is GameObject)
        //    {
        //         var prefab = asset as GameObject;
        //        foreach (var render in prefab.GetComponentsInChildren<Renderer>())
        //        {
        //            foreach (var mat in render.sharedMaterials)
        //            {
        //                if (mat == null || mat.shader == null)
        //                {
        //                    render.enabled = false;
        //                }
        //            }
        //        }
        //    }
        //    else if (asset is Material)
        //    {
        //        var mat = asset as Material;
        //        if(mat.shader == null)
        //        {
        //            mat.shader = Shader.Find("");
        //        }
        //    }
        //}


#if UNITY_EDITOR

        private void RefrashShader(Object asset)
        {
            if (asset is GameObject)
            {
                var prefab = asset as GameObject;
                foreach (var render in prefab.GetComponentsInChildren<Renderer>())
                {
                    foreach (var mat in render.sharedMaterials)
                    {
                        if (mat != null)
                        {
                            var shaderName = mat.shader.name;
                            var shader = Shader.Find(shaderName);
                            if (shader != null)
                            {
                                mat.shader = shader;
                            }
                        }
                    }
                }
            }
            else if (asset is Material)
            {
                var mat = asset as Material;
                var shaderName = mat.shader.name;
                var shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    mat.shader = shader;
                }
            }
        }


#endif

        public List<TAsset> LoadAll<TAsset>(string bundleId) where TAsset : Object
        {
            List<TAsset> assets;

            if (_isLoadBundle)
            {
                var bundleRef = _bundleLoader.Load(bundleId);
                assets = bundleRef.LoadAll<TAsset>();
            }
            else
            {
                assets = AssetDatabaseUtility.LoadAllAssetsAtPath<TAsset>(GetAssetPath(bundleId));
            }

            return assets;
        }

#region 卸载

        public void ReleaseAsset(List<string> assetIds)
        {
            foreach (var assetId in assetIds)
            {
                var asset = Buffer.GetValue(assetId);
                if (asset != null)
                {
                    Resources.UnloadAsset(asset);
                }
                Buffer.TryRemove(assetId);
            }

            if (_isLoadBundle)
            {
                _bundleLoader.ReleaseAsset(assetIds);
            }
        }

        public void ReleaseAsset(string assetId)
        {
            var asset = Buffer.GetValue(assetId);
            if(asset != null)
            {
                Resources.UnloadAsset(asset);
            }
            Buffer.TryRemove(assetId);

            if (_isLoadBundle)
            {
                _bundleLoader.ReleaseTarget(assetId);
            }
        }

#endregion

        public void Dispose()
        {
        }

        public bool IsReady { get; }
    }
}