using System.Collections.Generic;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// 合并材质
    /// 用于使用同一 shader 的多个材质合并为单独一个材质
    /// </summary>
    [System.Serializable]
    public class MultiMaterial
    {
        public Material combinedMaterial;
        public bool considerMeshUVs;
        public List<Material> sourceMaterials = new List<Material>();
    }
}