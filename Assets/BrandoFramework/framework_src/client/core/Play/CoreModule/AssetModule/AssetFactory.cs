#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

namespace Client.Core
{
    public static class AssetFactory
    {
        public static IBundleRef CreateBundleRef() => new BundleRef();
    }
}
