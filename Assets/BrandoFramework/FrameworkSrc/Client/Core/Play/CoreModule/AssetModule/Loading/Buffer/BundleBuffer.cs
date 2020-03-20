#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using Common;

namespace Client.Core

{
    [Singleton]
    //[DefaultInjecType(typeof(IBuffer<string, IBundleRef>))]
    public class BundleBuffer : AbsBuffer<string, IBundleRef>, IBundleBuffer
    {
        public void IncreaseRef(string bundleId) => Maps[bundleId].Use();
        public void ReduceRef(string bundleId)=> Maps[bundleId].Unuse();
    }
}
