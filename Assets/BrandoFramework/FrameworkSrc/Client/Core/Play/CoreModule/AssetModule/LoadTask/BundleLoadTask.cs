#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion


namespace Client.Core
{
    internal class BundleLoadTask : IBundleLoadTask
    {
        public string InitiatedAssetId { get; set; }
        public string InitiatedBundleId { get; set; }
        public string TargetBundleId { get; set; }

        public string QueryId
        {
            get
            {
                if (InitiatedAssetId != null)
                {
                    return InitiatedAssetId;
                }

                return TargetBundleId;
            }
        }

        public bool IsMainTask => InitiatedBundleId == TargetBundleId;

        public void Reset()
        {
            InitiatedAssetId = null;
            InitiatedBundleId = null;
            TargetBundleId = null;
        }
    }
}