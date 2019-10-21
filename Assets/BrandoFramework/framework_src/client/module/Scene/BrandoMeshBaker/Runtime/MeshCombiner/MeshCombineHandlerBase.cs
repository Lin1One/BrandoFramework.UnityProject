using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    //TODO bug with triangles if using showHide with AddDelete reproduce by using the AddDeleteParts script and changeing some of it to show hide
    [System.Serializable]
    public abstract class MeshCombineHandlerBase : IMeshCombinerSetting
    {
        public delegate void GenerateUV2Delegate(Mesh m, float hardAngle, float packMargin);

        /// <summary>
        /// ��֤����
        /// </summary>
        [SerializeField]
        protected ValidationLevel _validationLevel = ValidationLevel.robust;
        public virtual ValidationLevel validationLevel
        {
            get { return _validationLevel; }
            set { _validationLevel = value; }
        }

        /// <summary>
        /// �ϲ�������
        /// </summary>
        [SerializeField] protected string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// �ϲ�������Դ
        /// </summary>
        [SerializeField]
        protected TextureBakeResults _textureBakeResults;
        public virtual TextureBakeResults textureBakeResults
        {
            get { return _textureBakeResults; }
            set { _textureBakeResults = value; }
        }

        /// <summary>
        /// �ϲ���������Ϸ����
        /// </summary>
        [SerializeField]
        protected GameObject _resultSceneObject;
        public virtual GameObject resultSceneObject
        {
            get { return _resultSceneObject; }
            set { _resultSceneObject = value; }
        }

        [SerializeField]
        protected UnityEngine.Renderer _targetRenderer;
        public virtual Renderer targetRenderer
        {
            get { return _targetRenderer; }
            set
            {
                if (_targetRenderer != null && _targetRenderer != value)
                {
                    Debug.LogWarning("Previous targetRenderer was not null. Combined mesh may be being used by more than one Renderer");
                }
                _targetRenderer = value;
            }
        }


        /// <summary>
        /// �������ÿ���Ϊ��������Դ�����������ϲ�������
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object _settingsHolder;
        public virtual IMeshCombinerSettingHolder settingsHolder
        {
            get
            {
                if (_settingsHolder != null)
                {
                    if (_settingsHolder is IMeshCombinerSettingHolder)
                    {
                        return (IMeshCombinerSettingHolder)_settingsHolder;
                    }
                    else
                    {
                        _settingsHolder = null;
                    }
                }
                return null;
            }
            set
            {
                if (value is UnityEngine.Object)
                {
                    _settingsHolder = (UnityEngine.Object)value;
                }
                else
                {
                    Debug.LogError("The settings holder must be a UnityEngine.Object");
                }
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        protected IMeshCombinerSetting settings
        {
            get
            {
                if (_settingsHolder != null)
                {
                    return settingsHolder.GetMeshBakerSettings();
                }
                else
                {
                    return this;
                }
            }
        }

        #region �Դ��ϲ�������������

        [SerializeField]
        protected RendererType _renderType;
        public virtual RendererType renderType
        {
            get { return _renderType; }
            set { _renderType = value; }
        }

        [SerializeField]
        protected OutputOptions _outputOption;
        public virtual OutputOptions outputOption
        {
            get { return _outputOption; }
            set { _outputOption = value; }
        }

        [SerializeField]
        protected LightmapOptions _lightmapOption = LightmapOptions.ignore_UV2;
        public virtual LightmapOptions lightmapOption
        {
            get { return _lightmapOption; }
            set { _lightmapOption = value; }
        }

        [SerializeField]
        protected bool _doNorm = true;
        public virtual bool doNorm
        {
            get { return _doNorm; }
            set { _doNorm = value; }
        }

        [SerializeField]
        protected bool _doTan = true;
        public virtual bool doTan
        {
            get { return _doTan; }
            set { _doTan = value; }
        }

        [SerializeField]
        protected bool _doCol;
        public virtual bool doCol
        {
            get { return _doCol; }
            set { _doCol = value; }
        }

        [SerializeField]
        protected bool _doUV = true;
        public virtual bool doUV
        {
            get { return _doUV; }
            set { _doUV = value; }
        }

        //only included for backward compatibility. Does nothing
        public virtual bool doUV1
        {
            get { return false; }
            set { }
        }

        public virtual bool doUV2()
        {
            return _lightmapOption == LightmapOptions.copy_UV2_unchanged ||
                _lightmapOption == LightmapOptions.preserve_current_lightmapping ||
                _lightmapOption == LightmapOptions.copy_UV2_unchanged_to_separate_rects;
        }

        [SerializeField]
        protected bool _doUV3;
        public virtual bool doUV3
        {
            get { return _doUV3; }
            set { _doUV3 = value; }
        }

        [SerializeField]
        protected bool _doUV4;
        public virtual bool doUV4
        {
            get { return _doUV4; }
            set { _doUV4 = value; }
        }

        [SerializeField]
        protected bool _doBlendShapes;
        public virtual bool doBlendShapes
        {
            get { return _doBlendShapes; }
            set { _doBlendShapes = value; }
        }

        [SerializeField]
        protected bool _recenterVertsToBoundsCenter = false;
        public virtual bool recenterVertsToBoundsCenter
        {
            get { return _recenterVertsToBoundsCenter; }
            set { _recenterVertsToBoundsCenter = value; }
        }

        [SerializeField]
        protected bool _clearBuffersAfterBake = false;
        public virtual bool clearBuffersAfterBake
        {
            get { return _clearBuffersAfterBake; }
            set
            {
                Debug.LogError("Not implemented.");
                _clearBuffersAfterBake = value;
            }
        }

        [SerializeField]
        public bool _optimizeAfterBake = true;
        public bool optimizeAfterBake
        {
            get { return _optimizeAfterBake; }
            set { _optimizeAfterBake = value; }
        }

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("uv2UnwrappingParamsHardAngle")]
        protected float _uv2UnwrappingParamsHardAngle = 60f;
        public float uv2UnwrappingParamsHardAngle
        {
            get { return _uv2UnwrappingParamsHardAngle; }
            set { _uv2UnwrappingParamsHardAngle = value; }
        }

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("uv2UnwrappingParamsPackMargin")]
        protected float _uv2UnwrappingParamsPackMargin = .005f;
        public float uv2UnwrappingParamsPackMargin
        {
            get { return _uv2UnwrappingParamsPackMargin; }
            set { _uv2UnwrappingParamsPackMargin = value; }
        }

        #endregion

        /// <summary>
        /// �Ƿ�ʹ����ʱ�ϲ���ͼ
        /// </summary>
        protected bool _usingTemporaryTextureBakeResult;
        public abstract int GetLightmapIndex();
        public abstract void ClearBuffers();
        public abstract void ClearMesh();
        public abstract void DisposeRuntimeCreated();
        public abstract void DestroyMesh();
        public abstract void DestroyMeshEditor(EditorMethodsInterface editorMethods);
        public abstract List<GameObject> GetObjectsInCombined();
        public abstract int GetNumObjectsInCombined();
        public abstract int GetNumVerticesFor(GameObject go);
        public abstract int GetNumVerticesFor(int instanceID);

        /// <summary>
        /// ��Ӻ��Ƴ��ϲ�����������б����ɺϲ��������ݣ� apply ֮���� mesh �ɼ�
        /// </summary>
        public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);
        public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource);

        /// <summary>
        /// Updates the data in the combined mesh for meshes that are already in the combined mesh.
        /// This is faster than adding and removing a mesh and has a much lower memory footprint.
        /// This method can only be used if the meshes being updated have the same layout(number of 
        /// vertices, triangles, submeshes).
        /// This is faster than removing and re-adding
        /// For efficiency update as few channels as possible.
        /// Apply must be called to apply the changes to the combined mesh
        /// Ϊ�Ѻϲ��������Ѵ��ڵ���������������ݣ������Ӻ�ɾ������Ҫ�죬����ռ�õ��ڴ��ٵöࡣ
        /// ����Ҫ���µ����������ͬ�Ĳ��֣����㣬�����Σ������񣩡�
        /// ���ɾ�����������Ҫ��
        /// Ϊ�����Ч�ʣ��뾡�����ٵ� channels��
        /// �����Apply������Ӧ������ϵ�����
        /// </summary>		
        public abstract bool UpdateGameObjects(GameObject[] gos, bool recalcBounds = true,
                                        bool updateVertices = true, bool updateNormals = true, bool updateTangents = true,
                                        bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false,
                                        bool updateColors = false, bool updateSkinningInfo = false);

        /// <summary>
        /// ���ϲ�����������Ӧ���� mesh ��Դ��
        /// </summary>		
        public virtual void Apply()
        {
            Apply(null);
        }

        /// <summary>
        /// ���ϲ�����������Ӧ���� mesh ��Դ��
        /// </summary>
        /// <param name='uv2GenerationMethod'>
        /// Uv2 generation This should be null when calling Apply at runtime.
        /// ͨ���Ǳ༭���෽��Unwrapping.GenerateSecondaryUVSet
        /// ����ʱΪ null
        /// </param>
        public abstract void Apply(GenerateUV2Delegate uv2GenerationMethod);

        /// <summary>
        /// Apply the specified triangles, vertices, normals, tangents, uvs, colors, uv1, uv2, bones and uv2GenerationMethod.
        /// ���ϲ����������ݣ������Σ����㣬���ߣ����ߣ�uvs����ɫ��uv1��uv2���������ݣ�Ӧ���� mesh ��Դ��
        /// </summary>
        public abstract void Apply(bool triangles,
            bool vertices,
            bool normals,
            bool tangents,
            bool uvs,
            bool uv2,
            bool uv3,
            bool uv4,
            bool colors,
            bool bones = false,
            bool blendShapeFlag = false,
            GenerateUV2Delegate uv2GenerationMethod = null);

        public abstract bool CombinedMeshContains(GameObject go);
        public abstract void UpdateSkinnedMeshApproximateBounds();
        public abstract void UpdateSkinnedMeshApproximateBoundsFromBones();
        public abstract void CheckIntegrity();

        /// <summary>
        /// Updates the skinned mesh approximate bounds from the bounds of the source objects.
        /// ��Դ����ı߽������Ƥ��������Ʊ߽硣
        /// </summary>		
        public abstract void UpdateSkinnedMeshApproximateBoundsFromBounds();

        /// <summary>
        /// Updates the skinned mesh bounds by creating a bounding box that contains the bones (skeleton) of the source objects.
        /// ͨ����������Դ����Ĺ������Ǽܣ��ı߽����������Ƥ������߽硣
        /// </summary>		
        public static void UpdateSkinnedMeshApproximateBoundsFromBonesStatic(Transform[] bs, SkinnedMeshRenderer smr)
        {
            Vector3 max, min;
            max = bs[0].position;
            min = bs[0].position;
            for (int i = 1; i < bs.Length; i++)
            {
                Vector3 v = bs[i].position;
                if (v.x < min.x) min.x = v.x;
                if (v.y < min.y) min.y = v.y;
                if (v.z < min.z) min.z = v.z;
                if (v.x > max.x) max.x = v.x;
                if (v.y > max.y) max.y = v.y;
                if (v.z > max.z) max.z = v.z;
            }
            Vector3 center = (max + min) / 2f;
            Vector3 size = max - min;
            Matrix4x4 w2l = smr.worldToLocalMatrix;
            Bounds b = new Bounds(w2l * center, w2l * size);
            smr.localBounds = b;
        }

        /// <summary>
        /// Updates the skinned mesh bounds by creating a bounding box that contains the bones (skeleton) of the source objects.
        /// ͨ����������Դ����Ĺ������Ǽܣ��ı߽����������Ƥ������߽硣
        /// </summary>
        public static void UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(List<GameObject> objectsInCombined, SkinnedMeshRenderer smr)
        {
            Bounds b = new Bounds();
            Bounds bigB = new Bounds();
            if (MeshBakerUtility.GetBounds(objectsInCombined[0], out b))
            {
                bigB = b;
            }
            else
            {
                Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
                return;
            }
            for (int i = 1; i < objectsInCombined.Count; i++)
            {
                if (MeshBakerUtility.GetBounds(objectsInCombined[i], out b))
                {
                    bigB.Encapsulate(b);
                }
                else
                {
                    Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
                    return;
                }
            }
            smr.localBounds = bigB;
        }

        /// <summary>
        /// ������ʱ�ϲ�����
        /// </summary>
        protected virtual bool _CreateTemporaryTextrueBakeResult(GameObject[] gos, List<Material> matsOnTargetRenderer)
        {
            if (GetNumObjectsInCombined() > 0)
            {
                Debug.LogError("Can't add objects if there are already objects in combined mesh when 'Texture Bake Result' is not set. " +
                    "Perhaps enable 'Clear Buffers After Bake'");
                return false;
            }
            _usingTemporaryTextureBakeResult = true;
            _textureBakeResults = TextureBakeResults.CreateForMaterialsOnRenderer(gos, matsOnTargetRenderer);
            return true;
        }

        public abstract List<Material> GetMaterialsOnTargetRenderer();

        #region ��������
        /// <summary>
        /// Builds a map for mapping blend shapes in the source SkinnedMeshRenderers to blend shapes in the
        /// combined skinned meshes. If you need to serialize the map then use: BuildSourceBlendShapeToCombinedSerializableIndexMap.
        ///// </summary>
        //[System.Obsolete("BuildSourceBlendShapeToCombinedIndexMap is deprecated. The map will be attached to the combined SkinnedMeshRenderer object as the MB_BlendShape2CombinedMap Component.")]
        //public abstract Dictionary<MBBlendShapeKey, MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap();


        //public class MBBlendShapeKey
        //{
        //    public GameObject gameObject;
        //    public int blendShapeIndexInSrc;

        //    public MBBlendShapeKey(GameObject srcSkinnedMeshRenderGameObject, int blendShapeIndexInSource)
        //    {
        //        gameObject = srcSkinnedMeshRenderGameObject;
        //        blendShapeIndexInSrc = blendShapeIndexInSource;
        //    }

        //    public override bool Equals(object obj)
        //    {
        //        if (!(obj is MBBlendShapeKey) || obj == null)
        //        {
        //            return false;
        //        }
        //        MBBlendShapeKey other = (MBBlendShapeKey)obj;
        //        return (gameObject == other.gameObject && blendShapeIndexInSrc == other.blendShapeIndexInSrc);
        //    }

        //    public override int GetHashCode()
        //    {
        //        int hash = 23;
        //        unchecked
        //        {
        //            hash = hash * 31 + gameObject.GetInstanceID();
        //            hash = hash * 31 + blendShapeIndexInSrc;
        //        }
        //        return hash;
        //    }
        //}

        //public class MBBlendShapeValue
        //{
        //    public GameObject combinedMeshGameObject;
        //    public int blendShapeIndex;
        //}

        #endregion
    }
}