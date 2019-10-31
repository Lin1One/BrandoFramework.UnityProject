
#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using Common;

namespace Client.Core
{
    [Singleton]
    //[DefaultInjecType(typeof(IBuffer<string, UnityEngine.Object>))]
    public class AssetBuffer : AbsBuffer<string, UnityEngine.Object>, IAssetBuffer
    {
    }
}
