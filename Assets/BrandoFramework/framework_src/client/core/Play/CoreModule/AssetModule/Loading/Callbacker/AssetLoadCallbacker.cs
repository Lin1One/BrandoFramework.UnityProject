
#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using Common;

namespace Client.Core
{
    [Singleton]
    [DefaultInjecType(typeof(ILoadCallbcker<string, UnityEngine.Object>))]
    public class AssetLoadCallbacker : AbsLoadCallbacker<string, UnityEngine.Object>, IAssetLoadCallbacker
    {
    }
}
