using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// 网格分析结果
    /// </summary>
    public struct MeshAnalysisResult
    {
        public Rect uvRect;
        public bool hasOutOfBoundsUVs;
        public bool hasOverlappingSubmeshVerts;
        public bool hasOverlappingSubmeshTris;
        public bool hasUVs;
        public float submeshArea;
    }


}
