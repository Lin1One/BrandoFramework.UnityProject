#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56 AM
// Email:                 836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.Core
{
    internal class BundleRef : IBundleRef
    {
        private AssetBundle _assetBundle;
        private ushort _refNum;
        public ushort RefCount => _refNum;

        private Action<string> _onRelease;

#if UNITY_EDITOR

        ////private IRefAnalyzeProfiler _analyzeProfiler;

        ////private IRefAnalyzeProfiler AnalyzeProfiler
        ////{
        ////    get
        ////    {
        ////        if (_analyzeProfiler != null)
        ////        {
        ////            return _analyzeProfiler;
        ////        }

        ////        _analyzeProfiler = RefAnalyzeProfiler.GetAnalyzeProfiler(U3dGlobal.CurrentAppId);
        ////        return _analyzeProfiler;
        ////    }
        ////}

#endif

        public void Update(AssetBundle assetBundle, Action<string> onUnuse)
        {
            _bundleId = assetBundle.name;
            _assetBundle = assetBundle;
            _refNum = 0;
            _onRelease = onUnuse;
        }

        #region 异步加载

        public static bool open = true;

        public void LoadAsync<TAsset>(string assetId, Action<TAsset> onload) where TAsset : Object
        {
            var request = _assetBundle.LoadAssetAsync<TAsset>(assetId);
            request.completed += op =>
            {
                _refNum++;
                var asset = (op as AssetBundleRequest).asset as TAsset;
#if UNITY_EDITOR
                    CheckLoadResult(assetId, asset);
                ////AnalyzeProfiler?.AddAssetToBundleRef(BundleId, asset.name);
#endif
                onload(asset);
            };
        }

        public void LoadAllAsync<TAsset>(Action<List<TAsset>> onload) where TAsset : Object
        {
            var request = _assetBundle.LoadAllAssetsAsync<TAsset>();
            request.completed += op =>
            {
                _refNum++;
                var assets = request.allAssets.OfType<TAsset>().ToList();
                onload(assets);
#if UNITY_EDITOR
                var setId = assets[0].GetType() + "集合";
                ////AnalyzeProfiler?.AddAssetToBundleRef(BundleId, setId);
#endif
            };
        }

        #endregion

        #region 同步加载

        public TAsset Load<TAsset>(string assetId) where TAsset : Object
        {
            try
            {
                var asset = _assetBundle.LoadAsset<TAsset>(assetId);
#if UNITY_EDITOR
                CheckLoadResult(assetId, asset);

                ////AnalyzeProfiler?.AddAssetToBundleRef(BundleId, asset.name);
#endif
                return asset;
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.LogError("加载资源失败：" + assetId + "\n" + e.Message + e.StackTrace);
#endif
                return null;
            }
        }

        private void CheckLoadResult(string assetId, object asset)
        {
            if (asset == null)
            {
                Debug.LogError($"资源{assetId}加载失败！");
            }
        }

        public List<TAsset> LoadAll<TAsset>() where TAsset : Object
        {
            var assets = _assetBundle.LoadAllAssets<TAsset>().ToList();

////#if UNITY_EDITOR
////            AnalyzeProfiler?.AddAssetToBundleRef(BundleId, BundleId);
////#endif

            return assets;
        }

        #endregion

        #region Bundle 使用记录

        public void Use()
        {
            _refNum++;
        }

        public void Unuse()
        {
            _refNum--;
            if (_refNum > 0)
            {
                return;
            }

            _assetBundle.Unload(true);
            _onRelease?.Invoke(BundleId);
        }


        private string _bundleId;
        public string BundleId => _bundleId;

        public void Reset()
        {
            _refNum = 0;
            _assetBundle = null;
            _onRelease = null;
            _bundleId = null;
        }

        #endregion
    }
}