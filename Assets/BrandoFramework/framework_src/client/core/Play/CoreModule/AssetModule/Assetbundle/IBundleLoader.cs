#region Head

// Author:                LinYuzhou
// CreateDate:            2019/10/31 19:37:16
// Email:                 836045613@qq.com

#endregion

using System;
using System.Collections.Generic;

namespace Client.Core
{
    public interface IBundleLoader : IAssetLoader<string, IBundleLoadTask, IBundleRef>
    {
        void LoadAsync(string assetId, Action<IBundleRef> callback);

        IBundleRef Load(string assetId);

        void ReleaseAsset(List<string> assetIds);

        void ReleaseTarget(string assetId);

        void SetCurrentAppAssetBundleInfo(byte[] bytes);
        void Use(string assetId);
    }
}