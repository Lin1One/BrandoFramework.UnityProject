using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 材质可修改接口
    /// </summary>
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
