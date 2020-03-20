#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion

using System;

namespace Client.Core
{
    public interface IAssetLoadTask
    {
        string AssetId { get; set; }
        Type AssetType { get; set; }

        Action<UnityEngine.Object> Callback { get; set; }
    }
}
