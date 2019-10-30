#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Client.Core;
using Common.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 应用的文本样式数据。
    /// </summary>
    [System.Serializable]
    public class YuAppTextStyleInfo
    {
        [PropertyOrder(1)]
        [LabelText("应用文本样式列表")]
        public List<YuLegoTextStyleInfo> AppTextStyleInfos;

        public static Dictionary<string, YuAppTextStyleInfo> AppTextStyleInfoDic =
            new Dictionary<string, YuAppTextStyleInfo>();

        private static YuAppTextStyleInfo currentAppTextStyle;
        public static YuAppTextStyleInfo CurrentAppTextStyle
        {
            get
            {
                if (currentAppTextStyle != null)
                {
                    return currentAppTextStyle;
                }
                else
                {
                    string curAppID = ProjectInfoDati.GetActualInstance().DevelopProjectName;
                    if (AppTextStyleInfoDic.ContainsKey(curAppID))
                    {
                        currentAppTextStyle = AppTextStyleInfoDic[curAppID];
                    }
                    else
                    {
                        var resId = "Setting/YuTextStyle/" + curAppID + "TextStyleInfo";
                        var textAsset = Resources.Load<TextAsset>(resId);
                        currentAppTextStyle = JsonUtility.FromJson<YuAppTextStyleInfo>(textAsset.text);
                    }
                    return currentAppTextStyle;
                }
            }
        }

        public static YuLegoTextStyleInfo GetTextStyleInfoFromCurrentAppTextStyle(int styleId)
        {
            foreach(var textStyleInfo in CurrentAppTextStyle.AppTextStyleInfos)
            {
                if(textStyleInfo.Id == styleId)
                {
                    return textStyleInfo;
                }
            }
            return null;
        }

        [PropertyOrder(0)]
        [Button("保存修改",25)]
        private static void SaveTextStyleChange()
        {
            var appID = ProjectInfoDati.GetActualInstance().DevelopProjectName;
            currentAppTextStyle =
                AppTextStyleInfoDic[appID];
            string fullPath = GetAppTextStyleSettingTxtPath(
                appID);
            var jsContent = JsonUtility.ToJson(currentAppTextStyle);
            ////jsContent = EditorAPIInvoker.PrettifyJsonString(jsContent);
            IOUtility.WriteAllText(fullPath, jsContent);
        }

        private static string GetAppTextStyleSettingTxtPath(string appID)
        {
            var assetDirPath = "/_Yu/Resources/Setting/YuTextStyle/";
            var fullPath = Application.dataPath + assetDirPath + appID + "TextStyleInfo.txt";
            return fullPath;
        }
    }
}