using System;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// �洢��������ϲ���Դ��Ϸ���������Ϣ
    /// </summary>
    [System.Serializable]
    public class DynamicGameObjectInMeshCombine : IComparable<DynamicGameObjectInMeshCombine>
    {
        public GameObject gameObject;   //Դ��Ϸ����
        public int instanceID;          //Դ��Ϸ����ʵ�� ID
        public int numVerts;            //������
        public string name;             //
        public int vertIdx;             //
        public Vector3 meshSize = Vector3.one;  //��������������ߴ�
        public bool show = true;
        public bool invertTriangles = false;    //��ת������
        public bool _beingDeleted = false;      //�����Ƴ��ϲ�
        /// <summary>
        /// subMesh ����
        /// ��������ÿ���ϲ�������һ�� subMesh
        /// Դ������������������������񣬻���������õĲ���ӳ�䵽һ���ϲ�����
        ///     ���������ͬ�������������ͬ�Ĳ��ʣ������ǽ��ϲ�����ͬ�Ľ����������
        /// </summary>
        // These are result mesh submeshCount comine these into a class.
        public int[] submeshTriIdxs;
        /// <summary>
        /// �ڸ� submesh �������θ���
        /// </summary>
        public int[] submeshNumTris;
        public int _triangleIdxAdjustment = 0;

        #region Shape

        public int blendShapeIdx;       
        public int numBlendShapes;

        #endregion

        #region Bone

        public int[] indexesOfBonesUsed = new int[0];   //bones�����в�ͬ�Ĺ����б�
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
        /// Ŀ�� Submesh ����
        /// Դ�����е�ÿ��������ӳ�䵽��������е�һ��������
        /// </summary>
        public int[] targetSubmeshIdxs;

        /// <summary>
        /// �ںϲ����ʵ� UVRects 
        /// </summary>
        public Rect[] uvRects;

        /// <summary>
        /// If AllPropsUseSameMatTiling is the rect that was used for sampling the atlas texture from the source texture including both mesh uvTiling and material tiling.
        /// else is the source mesh obUVrect. We don't need to care which.
        ///��� AllPropsUseSameMatTiling Ϊ true 
        ///�������ڴ�Դ������������uvTiling�Ͳ��� tiling ���в�����ͼ�������rect��
        ///��������Դ���� obUVrect�� 
        /// </summary>
        public Rect[] encapsulatingRect;

        /// <summary>
        /// ��� AllPropsUseSameMatTiling Ϊ true������Դ������� tiling��
        /// ������ͳһΪ 0,0,1,1�� 
        /// </summary>
        public Rect[] sourceMaterialTiling;

        /// <summary>
        /// ÿ�� submesh ��ʹ�� obUVRect 
        /// </summary>
        public Rect[] obUVRects;

        /// <summary>
        /// ��Ϸ����������-�����ζ�ά����
        /// </summary>
        [NonSerialized]
        public SerializableIntArray[] _tmpSubmeshTris;


        public int CompareTo(DynamicGameObjectInMeshCombine b)
        {
            return this.vertIdx - b.vertIdx;
        }
    }

        
    
}