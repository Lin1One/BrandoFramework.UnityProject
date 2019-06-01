using client_common;
using Common.Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


#region Head

// Author:                liuruoyu1981
// CreateDate:            5/16/2019 11:25:26 PM
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Assets
{
    [Singleton]
    //[DefaultInjecType(typeof(IAssetModule))]
    public class AssetModule : AbsLoader<string, IAssetLoadTask, UnityEngine.Object>, IAssetModule
    {
        #region 构造

        //private readonly IYuU3dAppEntity _appEntity;

        public AssetModule()
        {
            //_appEntity = U3dGlobal.Get<IYuU3dAppEntity>();
            //_isLoadBundle = _appEntity.RunSetting.IsLoadFromAssetBundle;
            InitLoadActions();
        }

        #endregion

        //[YuInject] private readonly IBundleLoader _bundleLoader;



        #region 基础API

        protected override IAssetLoadTask CreateTask() => new AssetLoadTask();

        private string GetAssetPath(string assetId)
        {
            var assetInfo = AssetInfoHelper.GetAssetInfo(assetId);
            var path = assetInfo.GetEditorPath($"Assets/GameProjects/{U3dDevelopConfig.Config.CurrentDevelopProjectName}/AssetDatabase/");
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
                var loadState = GetLoadState(assetId);

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
            }
            else
            {
                string path = GetAssetPath(assetId);
                var asset = YuAssetDatabaseUtility.LoadAssetAtPath<TAsset>(path);
                callback(asset);
            }

            void ConvertAction(UnityEngine.Object tCallback)
            {
                Buffer.TryCache(assetId, tCallback);
                //LoadingIds.TryRemove(assetId);
                var finalAsset = tCallback as TAsset;
                callback?.Invoke(finalAsset);
            }

            void OnNotLoad()
            {
                Callbcker.AddCallback(assetId, ConvertAction);
                var task = TakeTask();
                task.AssetId = assetId;
                task.AssetType = typeof(TAsset);
                WaitTasks.Enqueue(task);
            }

            void OnLoaded()
            {
                var asset = Buffer.GetValue(assetId) as TAsset;
                callback(asset);
            }

            void OnLoading()
            {
                Callbcker.AddCallback(assetId, ConvertAction);
            }
        }

        private bool _isLoadBundle;

        protected override void StartOneLoadAsync(IAssetLoadTask task)
        {
            var assetId = task.AssetId;

            if (LoadingIds.Contains(assetId))
            {
                return;
            }

            LoadingIds.Add(assetId);
            StartLoadBundle(task);
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

        private void LoadAnimaControlAsync(IAssetLoadTask task) =>
            LoadAssetAsyncAtType<RuntimeAnimatorController>(task);

        private void LoadAssetAsyncAtType<TAsset2>(IAssetLoadTask task) where TAsset2 : UnityEngine.Object
        {
            var id = task.AssetId;
            //_bundleLoader.LoadAsync(id, bf => bf.LoadAsync<TAsset2>(id, OnAssetLoaded));

            void OnAssetLoaded(TAsset2 asset2)
            {
#if UNITY_EDITOR
                if (asset2 != null && asset2 is GameObject)
                {
                    var prefab = asset2 as GameObject;
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

                if (asset2 == null)
                {
                    Debug.LogError("加载ab资源失败：" + id);
                }
#endif

                Callbcker.Callback(id, asset2);
                RestoreTask(task);
            }
        }

        #endregion

        #endregion

        public void LoadAllAsync<TAsset>(string bundleId, Action<List<TAsset>> callback) where TAsset : Object
        {
            //if (_isLoadBundle)
            //{
            //    var bundleRef = _bundleLoader.Load(bundleId);
            //    bundleRef.LoadAllAsync(callback);
            //    return;
            //}

            var assets = YuAssetDatabaseUtility.LoadAllAssetsAtPath<TAsset>(GetAssetPath(bundleId));
            callback(assets);
        }

        #endregion

        #region 同步加载

        public TAsset Load<TAsset>(string assetId) where TAsset : Object
        {
            //if (Buffer.HasValue(assetId))
            //{
            //    //if(_isLoadBundle)
            //    //{
            //    //    _bundleLoader.Use(assetId.ToLower());
            //    //}
            //    return Buffer.GetValue(assetId) as TAsset;
            //}

            TAsset asset;

            //            if (_isLoadBundle)
            //            {
            ////                var bundleRef = _bundleLoader.Load(assetId);
            ////                asset = bundleRef?.Load<TAsset>(assetId);

            ////#if UNITY_EDITOR
            ////                if (asset != null && asset is GameObject)
            ////                {
            ////                    var prefab = asset as GameObject;
            ////                    foreach (var render in prefab.GetComponentsInChildren<Renderer>())
            ////                    {
            ////                        foreach (var mat in render.sharedMaterials)
            ////                        {
            ////                            if (mat != null)
            ////                            {
            ////                                var shaderName = mat.shader.name;
            ////                                var shader = Shader.Find(shaderName);
            ////                                if (shader != null)
            ////                                {
            ////                                    mat.shader = shader;
            ////                                }
            ////                            }
            ////                        }
            ////                    }
            ////                }
            ////                if (asset == null)
            ////                {
            ////                    Debug.LogError("加载ab资源失败：" + assetId);
            ////                    return null;
            ////                }
            ////#endif
            //            }
            //            else
            //            {

            //            }
            asset = YuAssetDatabaseUtility.LoadAssetAtPath<TAsset>(GetAssetPath(assetId));

            //Buffer.TryCache(assetId, asset);
            return asset;
        }

        public List<TAsset> LoadAll<TAsset>(string bundleId) where TAsset : Object
        {
            List<TAsset> assets;

            //if (_isLoadBundle)
            //{
            //    //var bundleRef = _bundleLoader.Load(bundleId);
            //    //assets = bundleRef.LoadAll<TAsset>();
            //}
            //else
            //{
            assets = YuAssetDatabaseUtility.LoadAllAssetsAtPath<TAsset>(GetAssetPath(bundleId));
            //}

            return assets;
        }

        #endregion

        #endregion

        #region 卸载

        public void ReleaseAsset(List<string> assetIds)
        {
            foreach (var assetId in assetIds)
            {
                Buffer.TryRemove(assetId);
            }

            if (_isLoadBundle)
            {
                //_bundleLoader.ReleaseAsset(assetIds);
            }
        }

        public void ReleaseTarget(string assetId)
        {
            Buffer.TryRemove(assetId);

            if (_isLoadBundle)
            {
               // _bundleLoader.ReleaseTarget(assetId);
            }
        }

        #endregion

        public void Dispose()
        {
        }

        public bool IsReady { get; }
    }
}