using System;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// 存储加入网格合并的源游戏对象相关信息
    /// </summary>
    [System.Serializable]
    public class DynamicGameObjectInMeshCombine : IComparable<DynamicGameObjectInMeshCombine>
    {
        public GameObject gameObject;   //源游戏物体
        public int instanceID;          //源游戏物体实例 ID
        public int numVerts;            //顶点数
        public string name;             //
        public int vertIdx;             //
        public Vector3 meshSize = Vector3.one;  //在世界坐标网格尺寸
        public bool show = true;
        public bool invertTriangles = false;    //反转三角形
        public bool _beingDeleted = false;      //将被移除合并
        /// <summary>
        /// subMesh 索引
        /// 组合网格对每个合并材质有一个 subMesh
        /// 源网格可以有任意数量的子网格，会根据其所用的材质映射到一个合并网格
        ///     如果两个不同的子网格具有相同的材质，则它们将合并到相同的结果子网格中
        /// </summary>
        // These are result mesh submeshCount comine these into a class.
        public int[] submeshTriIdxs;
        /// <summary>
        /// 在各 submesh 的三角形个数
        /// </summary>
        public int[] submeshNumTris;
        public int _triangleIdxAdjustment = 0;

        #region Shape

        public int blendShapeIdx;       
        public int numBlendShapes;

        #endregion

        #region Bone

        public int[] indexesOfBonesUsed = new int[0];   //bones数组中不同的骨骼列表
        //used so we don't have to call GetBones and GetBindposes twice
        [NonSerialized]
        public Transform[] _tmpCachedBones;
        [NonSerialized]
        public Matrix4x4[] _tmpCachedBindposes;
        [NonSerialized]
        public BoneWeight[] _tmpCachedBoneWeights;
        [NonSerialized]
        public int[] _tmpIndexesOfSourceBonesUsed;

        #endregion

        #region LightMap

        public int lightmapIndex = -1;
        public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

        #endregion

        /// <summary>
        /// These are source go mesh submeshCount todo combined these into a class.
        /// Maps each submesh in source mesh to a submesh in combined mesh.
        /// 目标 Submesh 索引
        /// 源网格中的每个子网格映射到组合网格中的一个子网格。
        /// </summary>
        public int[] targetSubmeshIdxs;

        /// <summary>
        /// 在合并材质的 UVRects 
        /// </summary>
        public Rect[] uvRects;

        /// <summary>
        /// If AllPropsUseSameMatTiling is the rect that was used for sampling the atlas texture from the source texture including both mesh uvTiling and material tiling.
        /// else is the source mesh obUVrect. We don't need to care which.
        ///如果 AllPropsUseSameMatTiling 为 true 
        ///则是用于从源纹理（包括网格uvTiling和材质 tiling ）中采样地图集纹理的rect。
        ///否则则是源网格 obUVrect。 
        /// </summary>
        public Rect[] encapsulatingRect;

        /// <summary>
        /// 如果 AllPropsUseSameMatTiling 为 true，则是源纹理材料 tiling。
        /// 否则则统一为 0,0,1,1。 
        /// </summary>
        public Rect[] sourceMaterialTiling;

        /// <summary>
        /// 每个 submesh 都使用 obUVRect 
        /// </summary>
        public Rect[] obUVRects;

        /// <summary>
        /// 游戏物体子网格-三角形二维数组
        /// </summary>
        [NonSerialized]
        public SerializableIntArray[] _tmpSubmeshTris;


        public int CompareTo(DynamicGameObjectInMeshCombine b)
        {
            return this.vertIdx - b.vertIdx;
        }
    }

        
    
}