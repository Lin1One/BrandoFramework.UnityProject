

#region Head

// Author:                liuruoyu1981
// CreateDate:            5/16/2019 10:28:56 PM
// Email:                 liuruoyu1981@gmail.com

#endregion

using System;
using System.Collections.Generic;


namespace Client.Assets
{
    public interface IAssetModule : IAssetLoader<string, IAssetLoadTask, UnityEngine.Object>, IModule
    {
        void LoadAsync<TAsset>(string assetId, Action<TAsset> callback) where TAsset : UnityEngine.Object;

        void LoadAllAsync<TAsset>(string bundleId, Action<List<TAsset>> callback) where TAsset : UnityEngine.Object;

        TAsset Load<TAsset>(string assetId) where TAsset : UnityEngine.Object;

        List<TAsset> LoadAll<TAsset>(string bundleId) where TAsset : UnityEngine.Object;

        void ReleaseAsset(List<string> assetIds);

        void ReleaseTarget(string assetId);
    }
}