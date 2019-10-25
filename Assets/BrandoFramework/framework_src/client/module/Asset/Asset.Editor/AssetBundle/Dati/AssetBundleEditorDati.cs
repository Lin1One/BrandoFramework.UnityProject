#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/2 20:44:51
// Email:             836045613@qq.com

#endregion

using Common;
using Common.PrefsData;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Client.Assets.Editor
{
    [Serializable]
    [YuDatiDesc(YuDatiSaveType.Single, typeof(AssetBundleEditorDati),
        "应用配置及资料/AssetBundle/打包配置")]
    public class AssetBundleEditorDati : GenericSingleDati<AssetBundleEditor,AssetBundleEditorDati>
    {
        #region 注册ProjectChange事件

        public override void OnEnable()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGui;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;
        }

        private void ProjectWindowItemOnGui(string guid, Rect selectionrect)
        {
            var isSelected = IsSelected(guid);
            if (!isSelected)
            {
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guid).EnsureDirEnd();
            if (!System.IO.Directory.Exists(path))
            {
                return;
            }

            var targetSetting = ActualSerializableObject.GetSetting(path);
            if (targetSetting == null)
            {
                return;
            }

            ActualSerializableObject.TargetDirSetting = targetSetting;
        }

        private static bool IsSelected(string guid)
        {
            return Selection.assetGUIDs.Any(guid.Contains);
        }

        ////public static AssetBundleEditor TryGetAssetBundleSetting(string appId)
        ////{
        ////    //var appAssetBundleSetting = GetMultiAtId(appId);
        ////    //return appAssetBundleSetting.ActualSerializableObject;
        ////}

        #endregion
    }
}