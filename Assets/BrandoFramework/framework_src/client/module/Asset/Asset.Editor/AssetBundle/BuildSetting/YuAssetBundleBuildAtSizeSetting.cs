#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com



#endregion

using Sirenix.OdinInspector;
using System;


namespace Client.Assets.Editor
{
    /// <summary>
    /// 基于尺寸自动分包的AssetBundle打包配置。
    /// </summary>
    [Serializable]
    public class YuAssetBundleBuildAtSizeSetting
    {
        [BoxGroup("尺寸分包信息")]
        [LabelText("分包尺寸")]
        public int PackageSize = 2000;

        [BoxGroup("尺寸分包信息")]
        [ShowInInspector]
        [ReadOnly]
        [LabelText("分包信息中资源总数")]
        private int sizeTotal;

        public void SetSizeTotal(int i) => sizeTotal = i;

        [BoxGroup("尺寸分包信息")]
        [HideLabel]
        [ReadOnly]
        public YuAssetBundleSizeInfo SizeInfo;

    }
}

