using UnityEngine;

namespace Client.UI
{
    /// <summary>
    /// ���ʿ��޸Ľӿ�
    /// </summary>
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
