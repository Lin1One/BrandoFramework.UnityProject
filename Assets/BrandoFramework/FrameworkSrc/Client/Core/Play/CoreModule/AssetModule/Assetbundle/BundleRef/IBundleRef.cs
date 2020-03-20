#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56 AM
// Email:                 836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.Core
{
    public interface IBundleRef
    {
        void LoadAsync<TAsset>(string assetId, Action<TAsset> onload) where TAsset : UnityEngine.Object;

        void LoadAllAsync<TAsset>(Action<List<TAsset>> onload) where TAsset : UnityEngine.Object;

        TAsset Load<TAsset>(string assetId) where TAsset : UnityEngine.Object;

        List<TAsset> LoadAll<TAsset>() where TAsset : UnityEngine.Object;

        void Use();

        void Unuse();
        
        ushort RefCount { get; }

        void Update(AssetBundle assetBundle, Action<string> onUnuse);
        
        string BundleId { get; }
    }
}
