#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/2 20:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common.Editor;
using Client.Utility;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace Client.Assets.Editor
{
    public class AtlasMenuItemExtend
    {
        [MenuItem("Assets/Atlas/更新当前选择图集的信息")]
        private static void UpdateAtlasInfo()
        {
            // var spriteNames = new List<string>();
            //var selects = YuEditorUtility.GetSelectPaths();
            //var localSavePath = YuU3dAppSettingDati.CurrentActual?.Helper.LocalHttpRootDir
            //                        + YuU3dAppSettingDati.CurrentActual?.LocAppId + "_AtlasInfo.bytes";

            //var bytes = File.Exists(localSavePath) ? File.ReadAllBytes(localSavePath) : null;
            //Client.LegoUI.YuLegoAppAtlasInfo AppAtlasInfo;
            //if (bytes == null)
            //{
            //    AppAtlasInfo = new Client.LegoUI.YuLegoAppAtlasInfo();
            //}
            //else
            //{
            //    AppAtlasInfo =
            //        YuSerializeUtility.DeSerialize<Client.LegoUI.YuLegoAppAtlasInfo>(bytes);
            //    if (AppAtlasInfo == null)
            //    {
            //        AppAtlasInfo = new Client.LegoUI.YuLegoAppAtlasInfo();
            //    }
            //}

            ////foreach (var item in AppAtlasInfo.SpriteIdMap)
            ////{
            ////    Debug.LogError(item.Key + " -- " + item.Value);
            ////}

            //int count = 0;
            //int maxCount = selects.Count;
            //foreach (var item in selects)
            //{
            //    count++;
            //    EditorUtility.DisplayProgressBar("更新精灵图片数据", "正在更新图片所在的图集数据", ((float)count) / maxCount);
            //    var texImporter = AssetImporter.GetAtPath(item) as TextureImporter;
            //    if (texImporter != null)
            //    {
            //        var atlasId = Path.GetFileNameWithoutExtension(item);
            //        foreach (var spritesheet in texImporter.spritesheet)
            //        {
            //            var spriteName = spritesheet.name.ToLower();
            //            if (!AppAtlasInfo.SpriteIdMap.ContainsKey(spriteName))
            //            {
            //                AppAtlasInfo.SpriteIdMap.Add(spriteName, atlasId);
            //                Debug.Log("已添加精灵图集信息：  " + spriteName + "  --  " + atlasId);
            //            }
            //            else
            //            {
            //                AppAtlasInfo.SpriteIdMap[spriteName] = atlasId;
            //                Debug.Log("已添加精灵图集信息：  " + spriteName + "  --  " + atlasId);
            //            }
            //        }
            //    }
            //}
            //bytes = YuSerializeUtility.Serialize(AppAtlasInfo);
            //YuIOUtility.WriteAllBytes(localSavePath, bytes);
            //EditorUtility.ClearProgressBar();
            //AssetDatabase.Refresh();
        }
    }

}