using UnityEngine;

namespace Client.UI
{
    public interface IMaterialModifier
    {
        Material GetModifiedMaterial(Material baseMaterial);
    }
}
