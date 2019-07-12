#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图可交互控件音效开发助手。
    /// </summary>
#if DEBUG
    [ExecuteInEditMode]
#endif
    public class YuLegoSoundHelper : YuDevelopHelper
    {
        [LabelText("按钮点击音效Id")]
        public string AudioClipId;

        [ReadOnly]
        [LabelText("按钮音频文件名称")]
        public string audioClipFileName;

    }
}