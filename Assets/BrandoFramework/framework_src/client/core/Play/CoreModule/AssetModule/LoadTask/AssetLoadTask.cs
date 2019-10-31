#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using Common;
using System;

namespace Client.Core
{
    internal class AssetLoadTask : IAssetLoadTask, IReset
    {
        public string AssetId { get; set; }
        public Type AssetType { get; set; }

        public Action<UnityEngine.Object> Callback { get; set; }

        public void Reset()
        {
            AssetId = null;
            AssetType = null;
            Callback = null;
        }
    }
}
