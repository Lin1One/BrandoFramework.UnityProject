#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using Common;
using System;
using System.Collections.Generic;


namespace Client.Core
{
    [DefaultInjecType(typeof(AssetModule))]
    public interface IAssetModule : IModule
    {
        void LoadAsync<TAsset>(string assetId, Action<TAsset> callback) where TAsset : UnityEngine.Object;

        void LoadAllAsync<TAsset>(string bundleId, Action<List<TAsset>> callback) where TAsset : UnityEngine.Object;

        TAsset Load<TAsset>(string assetId) where TAsset : UnityEngine.Object;

        List<TAsset> LoadAll<TAsset>(string bundleId) where TAsset : UnityEngine.Object;

        void ReleaseAsset(List<string> assetIds);

        void ReleaseAsset(string assetId);
    }
}