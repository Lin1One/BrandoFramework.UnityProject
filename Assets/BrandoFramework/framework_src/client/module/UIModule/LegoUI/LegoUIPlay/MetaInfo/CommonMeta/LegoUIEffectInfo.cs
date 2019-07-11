using Sirenix.OdinInspector;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI特效信息。
    /// </summary>
    [System.Serializable]
    public class LegoUIEffectInfo
    {
        [LabelText("预制体Id")]
        public string PrefabId;

        [LabelText("是否默认激活")]
        public bool IsActive;

        public static LegoUIEffectInfo Create(Transform trm)
        {
            var effectInfo = new LegoUIEffectInfo { PrefabId = trm.name, IsActive = trm.gameObject.activeSelf };

            return effectInfo;
        }
    }
}