#region Head

// Author:                LinYuzhou
// CreateDate:            10/31/2019 19:58:56
// Email:                 836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;



namespace Client.Core
{
    [Serializable]
    public class BundleDependInfoVisual
    {
        public List<string> DirectDepends;

        #region 构造

        public BundleDependInfoVisual(BundleDependInfo dependInfo)
        {
            DirectDepends = dependInfo.DirectDepends;
        }

        #endregion
    }

    [Serializable]
    public class BundleDependInfo
    {
        public string BundleId { get; }
        public List<string> DirectDepends { get; }

        #region 构造

        public BundleDependInfo(string bundleId, string[] depends)
        {
            BundleId = bundleId;
            DirectDepends = depends.ToList();
        }

        #endregion
    }
}
