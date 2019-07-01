#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/1 11:44:51
// Email:             836045613@qq.com



#endregion

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Client.Assets.Editor
{
    [Serializable]
    public class YuAssetBundleBaseNode
    {
        [LabelText("AssetBundle文件名")] public string BundleId;

        [LabelText("AssetBundle包含的文件名列表")] public List<string> FileIds;

        [GUIColor(0.235f, 0.7f, 0.44f)]
        [Button("打包该节点", ButtonSizes.Medium)]
        [HorizontalGroup("底部按钮")]
        private void BuildNode()
        {
        }

        [GUIColor(0.235f, 0.7f, 0.44f)]
        [Button("清理所有包名然后打包该节点", ButtonSizes.Medium)]
        [HorizontalGroup("底部按钮")]
        private void CleanAllAndBuildNode()
        {
        }

        public void AddFiles(List<string> fileIds)
        {
            if (FileIds == null)
            {
                FileIds = new List<string>();
            }

            FileIds.Clear();
            FileIds.AddRange(fileIds);
        }
    }
}