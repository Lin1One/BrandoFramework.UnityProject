#region Head

// Author:            LinYuzhou
// CreateDate:        2018/10/29 19:36:00
// Email:             836945613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Client.LegoUI
{
    [System.Serializable]
    public class LegoUISoundInfo
    {
        public static LegoUISoundInfo Instance;

        public List<string> UISoundNumIdList;
        public List<string> UISoundNameIdList;

        public static string GetSoundFileName(string soundId)
        {
            if (Instance == null)
            {
                var resId = "Setting/YuSoundSetting/" + ProjectInfoDati.GetActualInstance().DevelopProjectName+
                    "SoundInfo";
                var textAsset = Resources.Load<TextAsset>(resId);
                Instance =
                    JsonUtility.FromJson<LegoUISoundInfo>(textAsset.text);
            }
            string soundFileName = "";
            int index = Instance.UISoundNumIdList.IndexOf(soundId);

            if (index != -1)
            {
                soundFileName = Instance.UISoundNameIdList[index];
            }
            return soundFileName;
        }

    }
}