#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    public class LegoUIMetaAsset : ScriptableObject
    {
        [Title("乐高组件元数据")] [HideLabel] public LegoUIMeta UiMeta;

        public static LegoUIMetaAsset GetAsset(LegoUIMeta uiMeta)
        {
            var asset = CreateInstance<LegoUIMetaAsset>();
            asset.UiMeta = uiMeta;
            asset.name = uiMeta.RootMeta.TypeId;

            return asset;
        }
    }
}