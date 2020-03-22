#if UNITY_EDITOR

#region Head

// Author:            Yu
// CreateDate:        2018/8/15 14:51:21
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图按钮控件开发助手。
    /// </summary>
    [ExecuteInEditMode]
    public class YuLegoButtonHelper ////: YuDevelopHelper
    {
        [LabelText("按钮点击音效Id")]
        public int AudioClipId;

        [ReadOnly]
        [LabelText("按钮音频文件名称")]
        public string audioClipFileName;

        [BoxGroup("精灵过渡")]
        [LabelText("正常状态精灵Id")]
        public string NormalSpriteId;

        [BoxGroup("精灵过渡")]
        [LabelText("高亮状态精灵Id")]
        public string HighlightedSpriteId;

        [BoxGroup("精灵过渡")]
        [LabelText("按下状态精灵Id")]
        public string PressedSpriteId;

        [BoxGroup("精灵过渡")]
        [LabelText("禁用状态精灵Id")]
        public string DisabledSpriteId;

        ////private YuAppHelper CurrentHelper => YuU3dAppSettingDati.CurrentActual.Helper;


        public void FixedButtonImage()
        {
            ////var image = gameObject.GetComponent<YuLegoImage>();
            ////var normalSpritePath = YuUnityIOUtility.GetAssetsPath(CurrentHelper.OriginAtlasSpriteDir + NormalSpriteId + ".png");
            ////var sprite = AssetDatabaseUtility.LoadAssetAtPath<Sprite>(normalSpritePath);
            ////image.sprite = sprite;
        }
    }
}

#endif
