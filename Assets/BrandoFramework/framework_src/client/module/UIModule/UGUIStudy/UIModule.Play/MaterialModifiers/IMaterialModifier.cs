using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// �����޸Ľӿ�
    /// </summary>
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
