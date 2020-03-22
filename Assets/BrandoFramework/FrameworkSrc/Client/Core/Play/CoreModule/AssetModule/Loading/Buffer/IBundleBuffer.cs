 

 #region Head

// Author:                LinYuzhou
// CreateDate:            5/16/2019 10:40:29 PM
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    internal interface IBundleBuffer : IBuffer<string, IBundleRef>
    {
        void IncreaseRef(string bundleId);

        void ReduceRef(string bundleId);
    }
}
