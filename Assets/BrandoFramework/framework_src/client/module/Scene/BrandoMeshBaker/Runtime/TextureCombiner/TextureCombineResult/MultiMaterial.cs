using System.Collections.Generic;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// �ϲ�����
    /// ����ʹ��ͬһ shader �Ķ�����ʺϲ�Ϊ����һ������
    /// </summary>
    [System.Serializable]
    public class MultiMaterial
    {
        public Material combinedMaterial;
        public bool considerMeshUVs;
        public List<Material> sourceMaterials = new List<Material>();
    }
}