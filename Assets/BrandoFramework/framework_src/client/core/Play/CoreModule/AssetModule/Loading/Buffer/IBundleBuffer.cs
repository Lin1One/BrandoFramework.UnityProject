 

 #region Head

// Author:                liuruoyu1981
// CreateDate:            5/16/2019 10:40:29 PM
// Email:                 liuruoyu1981@gmail.com

#endregion


namespace Client.Core
{
    internal interface IBundleBuffer : IBuffer<string, IBundleRef>
    {
        void IncreaseRef(string bundleId);

        void ReduceRef(string bundleId);
    }
}
