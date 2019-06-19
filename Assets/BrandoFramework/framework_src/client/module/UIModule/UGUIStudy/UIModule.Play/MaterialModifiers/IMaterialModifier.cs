using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// 材质修改接口
    /// </summary>
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
