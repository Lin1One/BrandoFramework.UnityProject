#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 10:28:56 AM
// Email:                 836045613@qq.com

#endregion


using Common;

namespace Client.Core
{
    public interface IBundleLoadTask : IReset
    {
        string InitiatedAssetId { get; set; }
        string InitiatedBundleId { get; set; }
        string TargetBundleId { get; set; }
        
        /// <summary>
        /// 用于进行BundleID查找的最终字符串。
        /// </summary>
        string QueryId { get; }

        bool IsMainTask { get; }
    }
}
