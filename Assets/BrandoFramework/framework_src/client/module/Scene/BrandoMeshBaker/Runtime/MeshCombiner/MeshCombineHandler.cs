using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace GameWorld
{
    [Serializable]
    public partial class MeshCombineHandler : MeshCombineHandlerBase
    {
        public override RendererType renderType
        {
            set
            {
                if (value == RendererType.skinnedMeshRenderer && _renderType == RendererType.meshRenderer)
                {
                    if (MeshData.boneWeights.Length != MeshData.verts.Length)
                        Debug.LogError("设置为 SkinnedMeshRenderer 时需将当前的合并网格删除，可将合并的场景物体删除");
                }
                _renderType = value;
            }
        }

        //初始化合并器
        bool _Initialize(int numResultMats)
        {
            //初始化 lightMap
            if (mbDynamicObjectsInCombinedMesh.Count == 0)
            {
                lightmapIndex = -1;
            }
            //初始化 网格
            if (_mesh == null)
            {
                Debug.Log("初始化网格合并器，创建初始网格");
                _mesh = GetMesh();
            }
            //初始化 sourceGoToDynamicGoMap
            if (sourceGoToDynamicGoMap.Count != mbDynamicObjectsInCombinedMesh.Count)
            {
                //重新设置网格合并物体信息Map
                sourceGoToDynamicGoMap.Clear();
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    if (mbDynamicObjectsInCombinedMesh[i] != null)
                    {
                        if (mbDynamicObjectsInCombinedMesh[i].gameObject == null)
                        {
                            Debug.LogError("This MeshBaker contains information from a previous bake that is incomlete. " +
                                "It may have been baked by a previous version of Mesh Baker. " +
                                "If you are trying to update/modify a previously baked combined mesh. Try doing the original bake.");
                            return false;
                        }

                        sourceGoToDynamicGoMap.Add(mbDynamicObjectsInCombinedMesh[i].gameObject, mbDynamicObjectsInCombinedMesh[i]);
                    }
                }
                //BoneWeights are not serialized get from combined mesh
                MeshData.boneWeights = _mesh.boneWeights;
            }
            //初始化 submeshTris
            if (objectsInCombinedMesh.Count == 0)
            {
                if (submeshTris.Length != numResultMats)
                {
                    submeshTris = new SerializableIntArray[numResultMats];
                    for (int i = 0; i < submeshTris.Length; i++)
                        submeshTris[i] = new SerializableIntArray(0);
                }
            }
            //骨骼：初始化源游戏物体动态信息 indexesOfBonesUsed 数据
            if (mbDynamicObjectsInCombinedMesh.Count > 0 &&
                mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 &&
                settings.renderType == RendererType.skinnedMeshRenderer &&
                MeshData.boneWeights.Length > 0)
            {
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                    HashSet<int> idxsOfBonesUsed = new HashSet<int>();
                    for (int j = dgo.vertIdx; j < dgo.vertIdx + dgo.numVerts; j++)
                    {
                        if (MeshData.boneWeights[j].weight0 > 0f)
                            idxsOfBonesUsed.Add(MeshData.boneWeights[j].boneIndex0);
                        if (MeshData.boneWeights[j].weight1 > 0f)
                            idxsOfBonesUsed.Add(MeshData.boneWeights[j].boneIndex1);
                        if (MeshData.boneWeights[j].weight2 > 0f)
                            idxsOfBonesUsed.Add(MeshData.boneWeights[j].boneIndex2);
                        if (MeshData.boneWeights[j].weight3 > 0f)
                            idxsOfBonesUsed.Add(MeshData.boneWeights[j].boneIndex3);
                    }
                    dgo.indexesOfBonesUsed = new int[idxsOfBonesUsed.Count];
                    idxsOfBonesUsed.CopyTo(dgo.indexesOfBonesUsed);
                }
                Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
            }
            Debug.Log(string.Format("合并器初始化完成， 合并物体数量为 = {0}", mbDynamicObjectsInCombinedMesh.Count));
            return true;
        }

        #region 光照贴图

        [SerializeField]
        int lightmapIndex = -1;

        public override int GetLightmapIndex()
        {
            if (settings.lightmapOption == LightmapOptions.generate_new_UV2_layout ||
                settings.lightmapOption == LightmapOptions.preserve_current_lightmapping)
            {
                return lightmapIndex;
            }
            else
            {
                return -1;
            }
        }

        #endregion

        #region 游戏物体

        //已合并游戏物体列表
        [SerializeField]
        protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();

        //已合并游戏物体信息列表
        [SerializeField]
        List<DynamicGameObjectInMeshCombine> mbDynamicObjectsInCombinedMesh = new List<DynamicGameObjectInMeshCombine>();

        //新增未合并游戏物体列表
        Dictionary<GameObject, DynamicGameObjectInMeshCombine> sourceGoToDynamicGoMap = new Dictionary<GameObject, DynamicGameObjectInMeshCombine>();

        //used if user passes null in as parameter to AddOrDelete
        GameObject[] empty = new GameObject[0];
        int[] emptyIDs = new int[0];

        public override GameObject resultSceneObject
        {
            set
            {
                if (_resultSceneObject != value)
                {
                    _targetRenderer = null;
                    if (_mesh != null)
                    {
                        Debug.LogWarning("合并时，该合并器会保存 mesh 的引用，如果合并网格被其他物体引用，需要确保在重新合并前将其置空。");
                    }
                }
                _resultSceneObject = value;
            }
        }

        bool instance2Combined_MapContainsKey(GameObject gameObjectID)
        {
            return sourceGoToDynamicGoMap.ContainsKey(gameObjectID);
        }

        /// <summary>
        /// //根据实例 ID 初始化 MB_DynamicGameObject 对象
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="dgoGameObject"></param>
        /// <returns></returns>
        bool TryGetDynamicGoByInstanceID(int instanceID, out DynamicGameObjectInMeshCombine dgoGameObject)
        {
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                if (mbDynamicObjectsInCombinedMesh[i].instanceID == instanceID)
                {
                    dgoGameObject = mbDynamicObjectsInCombinedMesh[i];
                    return true;
                }
            }
            dgoGameObject = null;
            return false;
        }

        public override int GetNumObjectsInCombined()
        {
            return mbDynamicObjectsInCombinedMesh.Count;
        }

        public override bool CombinedMeshContains(GameObject go)
        {
            return objectsInCombinedMesh.Contains(go);
        }

        public override List<GameObject> GetObjectsInCombined()
        {
            List<GameObject> outObs = new List<GameObject>();
            outObs.AddRange(objectsInCombinedMesh);
            return outObs;
        }

        #region 设置Show or Hide

        public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide)
        {
            if (textureBakeResults == null)
            {
                Debug.LogError("TextureBakeResults 为空.");
                return false;
            }
            if (toShow == null)
                toShow = empty;
            if (toHide == null)
                toHide = empty;
            //calculate amount to hide
            int numResultMats = _textureBakeResults.resultMaterials.Length;
            if (!_Initialize(numResultMats))
            {
                return false;
            }

            for (int i = 0; i < toHide.Length; i++)
            {
                if (!instance2Combined_MapContainsKey(toHide[i]))
                {
                    Debug.LogWarning("Trying to hide an object " + toHide[i] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
                    return false;
                }
            }
            //now to show
            for (int i = 0; i < toShow.Length; i++)
            {
                if (!instance2Combined_MapContainsKey(toShow[i]))
                {
                    Debug.LogWarning("Trying to show an object " + toShow[i] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?");
                    return false;
                }
            }

            //set flags
            for (int i = 0; i < toHide.Length; i++)
                sourceGoToDynamicGoMap[toHide[i]].show = false;
            for (int i = 0; i < toShow.Length; i++)
                sourceGoToDynamicGoMap[toShow[i]].show = true;
            return true;
        }

        #endregion

        #region 创建游戏物体

        public void CreateSceneMeshObject(GameObject[] gos = null, bool createNewChild = false)
        {
            if (_resultSceneObject == null)
            {
                _resultSceneObject = new GameObject("CombinedMeshObject-" + name);
            }
            _targetRenderer = BuildSceneHierarchPreBake(this, _resultSceneObject, GetMesh(), createNewChild, gos);
        }

        internal Renderer BuildSceneHierarchPreBake(MeshCombineHandler meshCombiner, GameObject root, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
        {
            GameObject meshGO;
            MeshFilter meshFilter = null;
            MeshRenderer meshRenderer = null;
            SkinnedMeshRenderer skinMeshRenderer = null;
            Transform trans = null;
            if (root == null)
            {
                Debug.LogError("root 为空.");
                return null;
            }
            if (meshCombiner.textureBakeResults == null)
            {
                Debug.LogError("textureBakeResults 为空.");
                return null;
            }
            if (root.GetComponent<Renderer>() != null)
            {
                Debug.LogError("root game object 不能包含 renderer component");
                return null;
            }
            if (!createNewChild)
            {
                //try to find an existing child
                if (meshCombiner.targetRenderer != null && meshCombiner.targetRenderer.transform.parent == root.transform)
                {
                    trans = meshCombiner.targetRenderer.transform; //good setup
                }
                else
                {
                    Renderer[] rs = root.GetComponentsInChildren<Renderer>();
                    if (rs.Length == 1)
                    {
                        if (rs[0].transform.parent != root.transform)
                        {
                            Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
                        }
                        trans = rs[0].transform;
                    }
                }
            }
            if (trans != null && trans.parent != root.transform)
            { //target renderer must be a child of root
                trans = null;
            }
            if (trans == null)
            {
                meshGO = new GameObject(meshCombiner.name + "-mesh");
                meshGO.transform.parent = root.transform;
                trans = meshGO.transform;
            }
            trans.parent = root.transform;
            meshGO = trans.gameObject;
            if (settings.renderType == RendererType.skinnedMeshRenderer)
            {
                MeshRenderer r = meshGO.GetComponent<MeshRenderer>();
                if (r != null)
                    MeshBakerUtility.Destroy(r);
                MeshFilter f = meshGO.GetComponent<MeshFilter>();
                if (f != null)
                    MeshBakerUtility.Destroy(f);
                skinMeshRenderer = meshGO.GetComponent<SkinnedMeshRenderer>();
                if (skinMeshRenderer == null)
                    skinMeshRenderer = meshGO.AddComponent<SkinnedMeshRenderer>();
                skinMeshRenderer.bones = meshCombiner.GetBones();
                bool origVal = skinMeshRenderer.updateWhenOffscreen;
                skinMeshRenderer.updateWhenOffscreen = true;
                skinMeshRenderer.updateWhenOffscreen = origVal;
            }
            else
            {
                SkinnedMeshRenderer r = meshGO.GetComponent<SkinnedMeshRenderer>();
                if (r != null)
                    MeshBakerUtility.Destroy(r);
                meshFilter = meshGO.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = meshGO.AddComponent<MeshFilter>();
                meshRenderer = meshGO.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    meshRenderer = meshGO.AddComponent<MeshRenderer>();
            }

            _ConfigureSceneHierarch(meshCombiner, root, meshRenderer, meshFilter, skinMeshRenderer, m, objsToBeAdded);

            if (settings.renderType == RendererType.skinnedMeshRenderer)
            {
                return skinMeshRenderer;
            }
            else
            {
                return meshRenderer;
            }
        }

        //设置合并网格游戏物体在 Scene 参数
        private static void _ConfigureSceneHierarch(MeshCombineHandler meshCombiner,GameObject root,MeshRenderer renderer,
            MeshFilter filter,
            SkinnedMeshRenderer smr,
            Mesh m,
            GameObject[] objsToBeAdded = null)
        {
            //assumes everything is set up correctly
            GameObject meshGO;
            if (meshCombiner.settings.renderType == RendererType.skinnedMeshRenderer)
            {
                meshGO = smr.gameObject;
                smr.lightmapIndex = meshCombiner.GetLightmapIndex();
            }
            else
            {
                meshGO = renderer.gameObject;
                filter.sharedMesh = m;
                renderer.lightmapIndex = meshCombiner.GetLightmapIndex();
            }
            if (meshCombiner.settings.lightmapOption == LightmapOptions.preserve_current_lightmapping || meshCombiner.settings.lightmapOption == LightmapOptions.generate_new_UV2_layout)
            {
                meshGO.isStatic = true;
            }

            //set layer and tag of combined object if all source objs have same layer
            // 设置 layer 和 tag
            if (objsToBeAdded != null && objsToBeAdded.Length > 0 && objsToBeAdded[0] != null)
            {
                bool tagsAreSame = true;
                bool layersAreSame = true;
                string tag = objsToBeAdded[0].tag;
                int layer = objsToBeAdded[0].layer;
                for (int i = 0; i < objsToBeAdded.Length; i++)
                {
                    if (objsToBeAdded[i] != null)
                    {
                        if (!objsToBeAdded[i].tag.Equals(tag)) tagsAreSame = false;
                        if (objsToBeAdded[i].layer != layer) layersAreSame = false;
                    }
                }
                if (tagsAreSame)
                {
                    root.tag = tag;
                    meshGO.tag = tag;
                }
                if (layersAreSame)
                {
                    root.layer = layer;
                    meshGO.layer = layer;
                }
            }
        }

        /*
         could be building for a multiMeshBaker or a singleMeshBaker, targetRenderer will be a scene object.
        */
        /// <summary>
        /// 创建预制体实例至场景中
        /// </summary>
        /// <param name="mom"></param>
        /// <param name="instantiatedPrefabRoot"></param>
        /// <param name="m"></param>
        /// <param name="createNewChild"></param>
        /// <param name="objsToBeAdded"></param>
        public static void BuildPrefabHierarchy(MeshCombineHandler mom, GameObject instantiatedPrefabRoot, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
        {
            SkinnedMeshRenderer skinMeshRenderer = null;
            MeshRenderer meshRenderer = null;
            MeshFilter mf = null;
            GameObject meshGO = new GameObject(mom.name + "-mesh");
            meshGO.transform.parent = instantiatedPrefabRoot.transform;
            Transform mt = meshGO.transform;

            mt.parent = instantiatedPrefabRoot.transform;
            meshGO = mt.gameObject;

            //skinnedMeshRenderer
            if (mom.settings.renderType == RendererType.skinnedMeshRenderer)
            {
                MeshRenderer r = meshGO.GetComponent<MeshRenderer>();
                if (r != null)
                    MeshBakerUtility.Destroy(r);
                MeshFilter f = meshGO.GetComponent<MeshFilter>();
                if (f != null)
                    MeshBakerUtility.Destroy(f);
                skinMeshRenderer = meshGO.GetComponent<SkinnedMeshRenderer>();
                if (skinMeshRenderer == null)
                    skinMeshRenderer = meshGO.AddComponent<SkinnedMeshRenderer>();

                skinMeshRenderer.bones = mom.GetBones();
                bool origVal = skinMeshRenderer.updateWhenOffscreen;
                skinMeshRenderer.updateWhenOffscreen = true;
                skinMeshRenderer.updateWhenOffscreen = origVal;
                skinMeshRenderer.sharedMesh = m;

                BlendShape2CombinedMap srcMap = mom._targetRenderer.GetComponent<BlendShape2CombinedMap>();
                if (srcMap != null)
                {
                    BlendShape2CombinedMap targMap = meshGO.GetComponent<BlendShape2CombinedMap>();
                    if (targMap == null) targMap = meshGO.AddComponent<BlendShape2CombinedMap>();
                    targMap.srcToCombinedMap = srcMap.srcToCombinedMap;
                    for (int i = 0; i < targMap.srcToCombinedMap.combinedMeshTargetGameObject.Length; i++)
                    {
                        targMap.srcToCombinedMap.combinedMeshTargetGameObject[i] = meshGO;
                    }
                }
            }
            else
            {
                SkinnedMeshRenderer r = meshGO.GetComponent<SkinnedMeshRenderer>();
                if (r != null)
                    MeshBakerUtility.Destroy(r);
                mf = meshGO.GetComponent<MeshFilter>();
                if (mf == null)
                    mf = meshGO.AddComponent<MeshFilter>();
                meshRenderer = meshGO.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                    meshRenderer = meshGO.AddComponent<MeshRenderer>();
            }

            _ConfigureSceneHierarch(mom, instantiatedPrefabRoot, meshRenderer, mf, skinMeshRenderer, m, objsToBeAdded);

            //First try to get the materials from the target renderer. This is because the mesh may have fewer submeshes than number of result materials if some of the submeshes had zero length tris.
            //If we have just baked then materials on the target renderer will be correct wheras materials on the textureBakeResult may not be correct.
            //首先尝试从目标渲染器获取材质。 这是因为如果某些子网格的长度tris为零，则网格的子网格可能少于结果材料的数量。
            //如果我们刚刚烘焙过，那么目标渲染器上的材质将是正确的，而textureBakeResult上的材质可能是不正确的。
            if (mom.targetRenderer != null)
            {
                Material[] sharedMats = new Material[mom.targetRenderer.sharedMaterials.Length];
                for (int i = 0; i < sharedMats.Length; i++)
                {
                    sharedMats[i] = mom.targetRenderer.sharedMaterials[i];
                }
                if (mom.settings.renderType == RendererType.skinnedMeshRenderer)
                {
                    skinMeshRenderer.sharedMaterial = null;
                    skinMeshRenderer.sharedMaterials = sharedMats;
                }
                else
                {
                    meshRenderer.sharedMaterial = null;
                    meshRenderer.sharedMaterials = sharedMats;
                }
            }
        }

        #endregion

        #endregion

        #region 网格数据

        [SerializeField]
        Mesh _mesh;
        public Mesh GetMesh()
        {
            if (_mesh == null)
            {
                _mesh = NewMesh();
            }
            return _mesh;
        }

        public void SetMesh(Mesh m)
        {
            if (m == null)
            {
                _meshBirth = MeshCreationConditions.AssignedByUser;
            }
            else
            {
                _meshBirth = MeshCreationConditions.NoMesh;
            }

            _mesh = m;
        }

        private Mesh NewMesh()
        {
            if (Application.isPlaying)
            {
                _meshBirth = MeshCreationConditions.CreatedAtRuntime;
            }
            else
            {
                _meshBirth = MeshCreationConditions.CreatedInEditor;
            }
            Mesh m = new Mesh();

            return m;
        }

        [SerializeField]
        MeshCreationConditions _meshBirth = MeshCreationConditions.NoMesh;

        [SerializeField]
        private MeshCombineData combineMeshData;
        /// <summary>
        /// 合并器网格数据
        /// </summary>
        public MeshCombineData MeshData
        {
            get
            {
                if (combineMeshData == null)
                {
                    combineMeshData = new MeshCombineData();
                    combineMeshData.Init();
                }
                return combineMeshData;
            }
        }

        /// <summary>
        /// 二维数组，（子网格，三角形数组）
        /// </summary>
        [SerializeField]
        SerializableIntArray[] submeshTris = new SerializableIntArray[0];

        /*
         * Empties all channels, destroys the mesh and replaces it with a new mesh
         */
        public override void DestroyMesh()
        {
            if (_mesh != null)
            {
                Debug.Log("Destroying Mesh");
                MeshBakerUtility.Destroy(_mesh);
                _meshBirth = MeshCreationConditions.NoMesh;
            }
            ClearBuffers();
        }
        /*
        * Empties all channels and clears the mesh
        */
        public override void ClearMesh()
        {
            if (_mesh != null)
            {
                MeshBakerUtility.MeshClear(_mesh, false);
            }
            else
            {
                _mesh = NewMesh();
            }
            ClearBuffers();
        }

        public override void ClearBuffers()
        {
            MeshData.Init();
            submeshTris = new SerializableIntArray[0];
            blendShapes = new BlendShape[0];
            blendShapesInCombined = new BlendShape[0];
            mbDynamicObjectsInCombinedMesh.Clear();
            objectsInCombinedMesh.Clear();
            sourceGoToDynamicGoMap.Clear();
            if (_usingTemporaryTextureBakeResult)
            {
                MeshBakerUtility.Destroy(_textureBakeResults);
                _textureBakeResults = null;
                _usingTemporaryTextureBakeResult = false;
            }
            Debug.Log("ClearBuffers called");
        }

        public override void DisposeRuntimeCreated()
        {
            if (Application.isPlaying)
            {
                if (_meshBirth == MeshCreationConditions.CreatedAtRuntime)
                {
                    GameObject.Destroy(_mesh);
                }
                else if (_meshBirth == MeshCreationConditions.AssignedByUser)
                {
                    _mesh = null;
                }
                ClearBuffers();
            }
        }

        public override void DestroyMeshEditor(EditorMethodsInterface editorMethods)
        {
            if (_mesh != null && editorMethods != null && !Application.isPlaying)
            {
                Debug.Log("Destroying Mesh");
                editorMethods.Destroy(_mesh);
            }
            ClearBuffers();
        }

        public override int GetNumVerticesFor(GameObject go)
        {
            return GetNumVerticesFor(go.GetInstanceID());
        }

        /// <summary>
        /// 获取合并游戏物体顶点数
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public override int GetNumVerticesFor(int instanceID)
        {
            DynamicGameObjectInMeshCombine dgo = null;
            TryGetDynamicGoByInstanceID(instanceID, out dgo);
            if (dgo != null)
            {
                return dgo.numVerts;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 创建源物体的材质对应合并网格 subMesh 索引（即合并材质索引）字典
        /// </summary>
        OrderedDictionary BuildSourceMatsToSubmeshIdxMap(int numResultMats)
        {
            OrderedDictionary sourceMatsToSubmeshIdx_map = new OrderedDictionary();
            for (int i = 0; i < numResultMats; i++)
            {
                MultiMaterial mm = _textureBakeResults.resultMaterials[i];
                for (int j = 0; j < mm.sourceMaterials.Count; j++)
                {
                    if (mm.sourceMaterials[j] == null)
                    {
                        Debug.LogError("合并材质" + i + "的源材质列表中包含空的材质");
                        return null;
                    }
                    if (!sourceMatsToSubmeshIdx_map.Contains(mm.sourceMaterials[j]))
                    {
                        sourceMatsToSubmeshIdx_map.Add(mm.sourceMaterials[j], i);
                    }
                }
            }
            return sourceMatsToSubmeshIdx_map;
        }

        /// <summary>
        /// 将源物体的材质对应合并网格 subMesh 索引写入对应的 DGo
        /// </summary>
        /// <returns></returns>
        bool CollectMaterialTriangles(Mesh sourceMesh, DynamicGameObjectInMeshCombine dgo, Material[] sharedMaterials, OrderedDictionary sourceMatsToSubmeshIdx_map)
        {
            //everything here applies to the source object being added
            int numTriMeshes = sourceMesh.subMeshCount;
            if (sharedMaterials.Length < numTriMeshes)
                numTriMeshes = sharedMaterials.Length;
            dgo._tmpSubmeshTris = new SerializableIntArray[numTriMeshes];
            dgo.targetSubmeshIdxs = new int[numTriMeshes];
            for (int i = 0; i < numTriMeshes; i++)
            {
                if (_textureBakeResults.doMultiMaterial)
                {
                    if (!sourceMatsToSubmeshIdx_map.Contains(sharedMaterials[i]))
                    {
                        Debug.LogError("游戏物体 " + dgo.name + " 的包含材质 "+ sharedMaterials[i] + " 无法在合并材质中找到");
                        return false;
                    }
                    dgo.targetSubmeshIdxs[i] = (int)sourceMatsToSubmeshIdx_map[sharedMaterials[i]];
                }
                else
                {//合并为单个材质时，源物体所有 subMesh 均对应一个合并材质
                    dgo.targetSubmeshIdxs[i] = 0;
                }
                dgo._tmpSubmeshTris[i] = new SerializableIntArray();
                dgo._tmpSubmeshTris[i].data = sourceMesh.GetTriangles(i);
                Debug.Log("写入 triangles : " + dgo.name + " submesh:" + i + " maps to submesh:" +
                        dgo.targetSubmeshIdxs[i] + " added:" + dgo._tmpSubmeshTris[i].data.Length);
            }
            return true;
        }

        // if adding many copies of the same mesh want to cache obUVsResults
        //如果添加相同网格的多个副本要缓存obUVsResults
        bool _collectOutOfBoundsUVRects2(Mesh m,
            DynamicGameObjectInMeshCombine dgo,
            Material[] sharedMaterials,
            OrderedDictionary sourceMats2submeshIdx_map,
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisResults, MeshChannelsCache meshChannelCache)
        {
            if (_textureBakeResults == null)
            {
                Debug.LogError("textureBakeResults 为空");
                return false;
            }
            MeshAnalysisResult[] res;
            if (meshAnalysisResults.TryGetValue(m.GetInstanceID(), out res))
            {
                dgo.obUVRects = new Rect[sharedMaterials.Length];
                for (int i = 0; i < dgo.obUVRects.Length; i++)
                {
                    dgo.obUVRects[i] = res[i].uvRect;
                }
            }
            else
            {
                int numTriMeshes = m.subMeshCount;
                int numUsedTriMeshes = numTriMeshes;
                if (sharedMaterials.Length < numTriMeshes) numUsedTriMeshes = sharedMaterials.Length;
                dgo.obUVRects = new Rect[numUsedTriMeshes];
                //the mesh analysis result might be longer because we are caching and sharing the result with other
                //renderers which may use more materials
                res = new MeshAnalysisResult[numTriMeshes];
                for (int i = 0; i < numTriMeshes; i++)
                {
                    int idxInResultMats = dgo.targetSubmeshIdxs[i];
                    if (_textureBakeResults.resultMaterials[idxInResultMats].considerMeshUVs)
                    {
                        Vector2[] uvs = meshChannelCache.GetUv0Raw(m);
                        MeshBakerUtility.hasOutOfBoundsUVs(uvs, m, ref res[i], i);
                        Rect r = res[i].uvRect;
                        if (i < numUsedTriMeshes) dgo.obUVRects[i] = r;
                    }
                }
                meshAnalysisResults.Add(m.GetInstanceID(), res);
            }
            return true;
        }

        void _copyUV2unchangedToSeparateRects()
        {
            int uv2Padding = 16; //todo
            //todo meshSize
            List<Vector2> uv2AtlasSizes = new List<Vector2>();
            float minSize = 10e10f;
            float maxSize = 0f;
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                float zz = mbDynamicObjectsInCombinedMesh[i].meshSize.magnitude;
                if (zz > maxSize) maxSize = zz;
                if (zz < minSize) minSize = zz;
            }

            //normalize size so all values lie between these two values
            float MAX_UV_VAL = 1000f;
            float MIN_UV_VAL = 10f;
            float offset = 0;
            float scale = 1;
            if (maxSize - minSize > MAX_UV_VAL - MIN_UV_VAL)
            {
                //need to compress the range. Scale until is MAX_UV_VAL - MIN_UV_VAL in size and shift
                scale = (MAX_UV_VAL - MIN_UV_VAL) / (maxSize - minSize);
                offset = MIN_UV_VAL - minSize * scale;
            }
            else
            {
                scale = MAX_UV_VAL / maxSize;
            }
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {

                float zz = mbDynamicObjectsInCombinedMesh[i].meshSize.magnitude;
                zz = zz * scale + offset;
                Vector2 sz = Vector2.one * zz;
                uv2AtlasSizes.Add(sz);
            }

            //run texture packer on these rects
            TexturePacker tp = new TexturePackerRegular();
            tp.atlasMustBePowerOfTwo = false;
            AtlasPackingResult[] uv2Rects = tp.GetRects(uv2AtlasSizes, 8192, 8192, uv2Padding);
            //Debug.Assert(uv2Rects.Length == 1);
            //adjust UV2s
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                float minx, maxx, miny, maxy;
                minx = maxx = MeshData.uv2s[dgo.vertIdx].x;
                miny = maxy = MeshData.uv2s[dgo.vertIdx].y;
                int endIdx = dgo.vertIdx + dgo.numVerts;
                for (int j = dgo.vertIdx; j < endIdx; j++)
                {
                    if (MeshData.uv2s[j].x < minx) minx = MeshData.uv2s[j].x;
                    if (MeshData.uv2s[j].x > maxx) maxx = MeshData.uv2s[j].x;
                    if (MeshData.uv2s[j].y < miny) miny = MeshData.uv2s[j].y;
                    if (MeshData.uv2s[j].y > maxy) maxy = MeshData.uv2s[j].y;
                }
                //  scale it to fit the rect
                Rect r = uv2Rects[0].rects[i];
                for (int j = dgo.vertIdx; j < endIdx; j++)
                {
                    float width = maxx - minx;
                    float height = maxy - miny;
                    if (width == 0f) width = 1f;
                    if (height == 0f) height = 1f;
                    MeshData.uv2s[j].x = ((MeshData.uv2s[j].x - minx) / width) * r.width + r.x;
                    MeshData.uv2s[j].y = ((MeshData.uv2s[j].y - miny) / height) * r.height + r.y;
                }
            }
        }

        /// <summary>
        /// 获取合并游戏物体渲染器的材质
        /// </summary>
        /// <returns></returns>
        public override List<Material> GetMaterialsOnTargetRenderer()
        {
            List<Material> outMats = new List<Material>();
            if (_targetRenderer != null)
            {
                outMats.AddRange(_targetRenderer.sharedMaterials);
            }
            return outMats;
        }

        #region Bone

        public Transform[] GetBones()
        {
            return MeshData.bones;
        }

        int _getNumBones(Renderer r)
        {
            if (settings.renderType == RendererType.skinnedMeshRenderer)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).bones.Length;
                }
                else if (r is MeshRenderer)
                {
                    return 1;
                }
                else
                {
                    Debug.LogError("Could not _getNumBones. Object does not have a renderer");
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        Transform[] _getBones(Renderer r)
        {
            return MeshBakerUtility.GetBones(r);
        }

        List<DynamicGameObjectInMeshCombine>[] _buildBoneIdx2dgoMap()
        {
            List<DynamicGameObjectInMeshCombine>[] boneIdx2dgoMap = new List<DynamicGameObjectInMeshCombine>[MeshData.bones.Length];
            for (int i = 0; i < boneIdx2dgoMap.Length; i++) boneIdx2dgoMap[i] = new List<DynamicGameObjectInMeshCombine>();
            // build the map of bone indexes to objects that use them
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++)
                {
                    boneIdx2dgoMap[dgo.indexesOfBonesUsed[j]].Add(dgo);
                }
            }
            return boneIdx2dgoMap;
        }

        void _CollectBonesToAddForDGO(DynamicGameObjectInMeshCombine dgo, Dictionary<BoneAndBindpose, int> bone2idx, HashSet<int> boneIdxsToDelete, HashSet<BoneAndBindpose> bonesToAdd, Renderer r, MeshChannelsCache meshChannelCache)
        {
            //compile a list of bone transforms to add
            Matrix4x4[] dgoBindPoses = dgo._tmpCachedBindposes = meshChannelCache.GetBindposes(r);
            BoneWeight[] dgoBoneWeights = dgo._tmpCachedBoneWeights = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
            Transform[] dgoBones = dgo._tmpCachedBones = _getBones(r);

            //find bones that are actually by the vertices in the skinned mesh
            HashSet<int> usedBones = new HashSet<int>();
            for (int j = 0; j < dgoBoneWeights.Length; j++)
            {
                usedBones.Add(dgoBoneWeights[j].boneIndex0);
                usedBones.Add(dgoBoneWeights[j].boneIndex1);
                usedBones.Add(dgoBoneWeights[j].boneIndex2);
                usedBones.Add(dgoBoneWeights[j].boneIndex3);
            }

            int[] usedBoneIdx2srcMeshBoneIdx = new int[usedBones.Count];
            usedBones.CopyTo(usedBoneIdx2srcMeshBoneIdx);

            //for each bone see if it exists in the bones array
            for (int i = 0; i < usedBoneIdx2srcMeshBoneIdx.Length; i++)
            {
                bool foundInBonesList = false;
                int bidx;
                int dgoBoneIdx = usedBoneIdx2srcMeshBoneIdx[i];
                BoneAndBindpose bb = new BoneAndBindpose(dgoBones[dgoBoneIdx], dgoBindPoses[dgoBoneIdx]);
                if (bone2idx.TryGetValue(bb, out bidx))
                {
                    if (dgoBones[dgoBoneIdx] == MeshData.bones[bidx] && !boneIdxsToDelete.Contains(bidx))
                    {
                        if (dgoBindPoses[dgoBoneIdx] == MeshData.bindPoses[bidx])
                        {
                            foundInBonesList = true;
                        }
                    }
                }

                if (!foundInBonesList)
                {
                    if (!bonesToAdd.Contains(bb))
                    {
                        bonesToAdd.Add(bb);
                    }
                }
            }

            dgo._tmpIndexesOfSourceBonesUsed = usedBoneIdx2srcMeshBoneIdx;
        }

        void _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(HashSet<int> boneIdxsToDeleteHS, HashSet<BoneAndBindpose> bonesToAdd, Transform[] nbones, Matrix4x4[] nbindPoses, BoneWeight[] nboneWeights, int totalDeleteVerts)
        {
            // bones are copied separately because some dgos share bones
            if (boneIdxsToDeleteHS.Count > 0)
            {
                int[] boneIdxsToDelete = new int[boneIdxsToDeleteHS.Count];
                boneIdxsToDeleteHS.CopyTo(boneIdxsToDelete);
                Array.Sort(boneIdxsToDelete);
                //bones are being moved in bones array so need to do some remapping
                int[] oldBonesIndex2newBonesIndexMap = new int[MeshData.bones.Length];
                int newIdx = 0;
                int indexInDeleteList = 0;

                //bones were deleted so we need to rebuild bones and bind poses
                //and build a map of old bone indexes to new bone indexes
                //do this by copying old to new skipping ones we are deleting
                for (int i = 0; i < MeshData.bones.Length; i++)
                {
                    if (indexInDeleteList < boneIdxsToDelete.Length &&
                        boneIdxsToDelete[indexInDeleteList] == i)
                    {
                        //we are deleting this bone so skip its index
                        indexInDeleteList++;
                        oldBonesIndex2newBonesIndexMap[i] = -1;
                    }
                    else
                    {
                        oldBonesIndex2newBonesIndexMap[i] = newIdx;
                        nbones[newIdx] = MeshData.bones[i];
                        nbindPoses[newIdx] = MeshData.bindPoses[i];
                        newIdx++;
                    }
                }
                //adjust the indexes on the boneWeights
                int numVertKeeping = MeshData.boneWeights.Length - totalDeleteVerts;
                for (int i = 0; i < numVertKeeping; i++)
                {
                    nboneWeights[i].boneIndex0 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex0];
                    nboneWeights[i].boneIndex1 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex1];
                    nboneWeights[i].boneIndex2 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex2];
                    nboneWeights[i].boneIndex3 = oldBonesIndex2newBonesIndexMap[nboneWeights[i].boneIndex3];
                }
                //adjust the bone indexes on the dgos from old to new
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                    for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++)
                    {
                        dgo.indexesOfBonesUsed[j] = oldBonesIndex2newBonesIndexMap[dgo.indexesOfBonesUsed[j]];
                    }
                }
            }
            else
            { //no bones are moving so can simply copy bones from old to new
                Array.Copy(MeshData.bones, nbones, MeshData.bones.Length);
                Array.Copy(MeshData.bindPoses, nbindPoses, MeshData.bindPoses.Length);
            }
        }

        void _AddBonesToNewBonesArrayAndAdjustBWIndexes(DynamicGameObjectInMeshCombine dgo, Renderer r, int vertsIdx,
                                                         Transform[] nbones, BoneWeight[] nboneWeights, MeshChannelsCache meshChannelCache)
        {
            //Renderer r = MeshBakerUtility.GetRenderer(go);
            Transform[] dgoBones = dgo._tmpCachedBones;
            Matrix4x4[] dgoBindPoses = dgo._tmpCachedBindposes;
            BoneWeight[] dgoBoneWeights = dgo._tmpCachedBoneWeights;

            int[] srcIndex2combinedIndexMap = new int[dgoBones.Length];
            for (int j = 0; j < dgo._tmpIndexesOfSourceBonesUsed.Length; j++)
            {
                int dgoBoneIdx = dgo._tmpIndexesOfSourceBonesUsed[j];

                for (int k = 0; k < nbones.Length; k++)
                {
                    if (dgoBones[dgoBoneIdx] == nbones[k])
                    {
                        if (dgoBindPoses[dgoBoneIdx] == MeshData.bindPoses[k])
                        {
                            srcIndex2combinedIndexMap[dgoBoneIdx] = k;
                            break;
                        }
                    }
                }
            }
            //remap the bone weights for this dgo
            //build a list of usedBones, can't trust dgoBones because it contains all bones in the rig
            for (int j = 0; j < dgoBoneWeights.Length; j++)
            {
                int newVertIdx = vertsIdx + j;
                nboneWeights[newVertIdx].boneIndex0 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex0];
                nboneWeights[newVertIdx].boneIndex1 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex1];
                nboneWeights[newVertIdx].boneIndex2 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex2];
                nboneWeights[newVertIdx].boneIndex3 = srcIndex2combinedIndexMap[dgoBoneWeights[j].boneIndex3];
                nboneWeights[newVertIdx].weight0 = dgoBoneWeights[j].weight0;
                nboneWeights[newVertIdx].weight1 = dgoBoneWeights[j].weight1;
                nboneWeights[newVertIdx].weight2 = dgoBoneWeights[j].weight2;
                nboneWeights[newVertIdx].weight3 = dgoBoneWeights[j].weight3;
            }
            // repurposing the _tmpIndexesOfSourceBonesUsed since
            //we don't need it anymore and this saves a memory allocation . remap the indexes that point to source bones to combined bones.
            for (int j = 0; j < dgo._tmpIndexesOfSourceBonesUsed.Length; j++)
            {
                dgo._tmpIndexesOfSourceBonesUsed[j] = srcIndex2combinedIndexMap[dgo._tmpIndexesOfSourceBonesUsed[j]];
            }
            dgo.indexesOfBonesUsed = dgo._tmpIndexesOfSourceBonesUsed;
            dgo._tmpIndexesOfSourceBonesUsed = null;
            dgo._tmpCachedBones = null;
            dgo._tmpCachedBindposes = null;
            dgo._tmpCachedBoneWeights = null;

            //check original bones and bindPoses
            /*
            for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++) {
                Transform bone = bones[dgo.indexesOfBonesUsed[j]];
                Matrix4x4 bindpose = bindPoses[dgo.indexesOfBonesUsed[j]];
                bool found = false;
                for (int k = 0; k < dgo._originalBones.Length; k++) {
                    if (dgo._originalBones[k] == bone && dgo._originalBindPoses[k] == bindpose) {
                        found = true;
                    }
                }
                if (!found) Debug.LogError("A Mismatch between original bones and bones array. " + dgo.name);
            }
            */
        }

        #endregion

        #region BlendShape

        [SerializeField]
        internal BlendShape[] blendShapes = new BlendShape[0];

        //these blend shapes are not cleared they are used to build the src to combined blend shape map
        [SerializeField]
        internal BlendShape[] blendShapesInCombined = new BlendShape[0];

        //[System.Obsolete("BuildSourceBlendShapeToCombinedIndexMap is deprecated. The map will be now be attached to the combined SkinnedMeshRenderer object as the MB_BlendShape2CombinedMap Component.")]
        //public override Dictionary<MBBlendShapeKey, MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap()
        //{
        //    if (_targetRenderer == null)
        //        return new Dictionary<MBBlendShapeKey, MBBlendShapeValue>();
        //    MB_BlendShape2CombinedMap mapComponent = _targetRenderer.GetComponent<MB_BlendShape2CombinedMap>();
        //    if (mapComponent == null)
        //        return new Dictionary<MBBlendShapeKey, MBBlendShapeValue>();
        //    return mapComponent.srcToCombinedMap.GenerateMapFromSerializedData();
        //}

        internal void BuildSourceBlendShapeToCombinedSerializableIndexMap(SerializableSourceBlendShape2Combined outMap)
        {
            Debug.Assert(_targetRenderer.gameObject != null, "Target Renderer was null.");
            GameObject[] srcGameObjects = new GameObject[blendShapes.Length];
            int[] srcBlendShapeIdxs = new int[blendShapes.Length];
            GameObject[] targGameObjects = new GameObject[blendShapes.Length];
            int[] targBlendShapeIdxs = new int[blendShapes.Length];
            for (int i = 0; i < blendShapesInCombined.Length; i++)
            {
                srcGameObjects[i] = blendShapesInCombined[i].gameObject;
                srcBlendShapeIdxs[i] = blendShapesInCombined[i].indexInSource;
                targGameObjects[i] = _targetRenderer.gameObject;
                targBlendShapeIdxs[i] = i;
            }

            outMap.SetBuffers(srcGameObjects, srcBlendShapeIdxs, targGameObjects, targBlendShapeIdxs);
        }

        #endregion

        #endregion

        #region TextureBakeResults

        public override TextureBakeResults textureBakeResults
        {
            set
            {
                if (mbDynamicObjectsInCombinedMesh.Count > 0 && _textureBakeResults != value && _textureBakeResults != null)
                {
                   Debug.LogWarning("修改  TextureBake Result ，当前合并物体将无效 .");
                }
                _textureBakeResults = value;
            }
        }

        #endregion

        #region 合并网格：添加、删除参与合并游戏物体

        public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
        {
            //移除的游戏物体实例ID数组
            int[] delInstanceIDs = null;
            if (deleteGOs != null)
            {
                delInstanceIDs = new int[deleteGOs.Length];
                for (int i = 0; i < deleteGOs.Length; i++)
                {
                    if (deleteGOs[i] == null)
                    {
                        Debug.LogError("第 " + i + "个在删除列表中的游戏物体为 'Null'");
                    }
                    else
                    {
                        delInstanceIDs[i] = deleteGOs[i].GetInstanceID();
                    }
                }
            }
            return AddDeleteGameObjectsByID(gos, delInstanceIDs, disableRendererInSource);
        }

        public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
        {
            // --- 1、校验
            if (validationLevel > ValidationLevel.none)
            {
                //检测加入合并空项，重复项
                if (gos != null)
                {
                    for (int i = 0; i < gos.Length; i++)
                    {
                        if (gos[i] == null)
                        {
                            Debug.LogError("第 " + i + "个在添加合并列表中的游戏物体为 'Null'. ");
                            return false;
                        }
                        if (validationLevel >= ValidationLevel.robust)
                        {
                            for (int j = i + 1; j < gos.Length; j++)
                            {
                                if (gos[i] == gos[j])
                                {
                                    Debug.LogError("加入合并游戏物体列表中， " + gos[i] + " 出现两次。");
                                    return false;
                                }
                            }
                        }
                    }
                }
                //检测移除合并空项，重复项
                if (deleteGOinstanceIDs != null && validationLevel >= ValidationLevel.robust)
                {
                    for (int i = 0; i < deleteGOinstanceIDs.Length; i++)
                    {
                        for (int j = i + 1; j < deleteGOinstanceIDs.Length; j++)
                        {
                            if (deleteGOinstanceIDs[i] == deleteGOinstanceIDs[j])
                            {
                                Debug.LogError("移除出合并游戏物体列表中 " + deleteGOinstanceIDs[i] + "出现两次。");
                                return false;
                            }
                        }
                    }
                }
            }

            // --- 2、判断是否需要使用临时贴图
            if (_usingTemporaryTextureBakeResult && gos != null && gos.Length > 0)
            {
                MeshBakerUtility.Destroy(_textureBakeResults);
                _textureBakeResults = null;
                _usingTemporaryTextureBakeResult = false;
            }

            //_textureBakeResults 为空时创建临时合并材质贴图
            if (_textureBakeResults == null && gos != null && gos.Length > 0 && gos[0] != null)
            {
                if (!_CreateTemporaryTextrueBakeResult(gos, GetMaterialsOnTargetRenderer()))
                {
                    return false;
                }
            }

            // --- 3、创建合并结果的游戏物体
            CreateSceneMeshObject(gos);

            // --- 4、将物体加入或移除合并列表
            if (!_addToCombined(gos, deleteGOinstanceIDs, disableRendererInSource))
            {
                Debug.LogError("Failed to add/delete objects to combined mesh");
                return false;
            }

            // --- 5、skin 渲染，更新，更新 LightMap
            if (targetRenderer != null)
            {
                if (settings.renderType == RendererType.skinnedMeshRenderer)
                {
                    SkinnedMeshRenderer smr = (SkinnedMeshRenderer)targetRenderer;
                    smr.sharedMesh = _mesh;
                    smr.bones = MeshData.bones;
                    UpdateSkinnedMeshApproximateBoundsFromBounds();
                }
                targetRenderer.lightmapIndex = GetLightmapIndex();
            }
            return true;
        }

        /// <summary>
        /// 合并主流程函数
        /// </summary>
        /// <param name="goToAdd"></param>
        /// <param name="goToDelete"></param>
        /// <param name="disableRendererInSource"></param>
        /// <returns></returns>
        bool _addToCombined(GameObject[] goToAdd, int[] goToDelete, bool disableRendererInSource)
        {
            GameObject[] _goToAdd;
            int[] _goToDeleteInstanceIDs;

            // --- 1、校验，初始化数据
            // 合并材质资源检验
            if (!_validateTextureBakeResults())
            {
                return false;
            }
            // 渲染器，网格检验
            if (!ValidateTargRendererAndMeshAndResultSceneObj())
            {
                return false;
            }
            // 输出格式
            if (outputOption != OutputOptions.bakeMeshAssetsInPlace && settings.renderType == RendererType.skinnedMeshRenderer)
            {
                if (_targetRenderer == null || !(_targetRenderer is SkinnedMeshRenderer))
                {
                    Debug.LogError("渲染合并结果的 Renderer 需设置为 SkinnedMeshRenderer");
                    return false;
                }
            }
            if (settings.doBlendShapes && settings.renderType != RendererType.skinnedMeshRenderer)
            {
                Debug.LogError("doBlendShapes 勾选时， RenderType 必须为 skinnedMeshRenderer.");
                return false;
            }

            if (goToAdd == null)
                _goToAdd = empty;
            else
                _goToAdd = (GameObject[])goToAdd.Clone();
            if (goToDelete == null)
                _goToDeleteInstanceIDs = emptyIDs;
            else
                _goToDeleteInstanceIDs = (int[])goToDelete.Clone();
            if (_mesh == null)
                DestroyMesh(); //cleanup maps and arrays

            //创建材质 Atlas Rect 映射对象
            MaterialToAtlasRectMapper matToRect_map = 
                new MaterialToAtlasRectMapper(textureBakeResults);

            //（合并网格Submesh）合并材质数量
            int numResultMats = _textureBakeResults.resultMaterials.Length;

            //初始化合并器
            if (!_Initialize(numResultMats))
            {
                return false;
            }

            if (submeshTris.Length != numResultMats)
            {
                Debug.LogError("subMesh 数量为 " + submeshTris.Length +"不等于 Texture Bake Result中合并材质数量 " + numResultMats);
                return false;
            }

            if (_mesh.vertexCount > 0 && sourceGoToDynamicGoMap.Count == 0)
            {
                Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. " +
                    "If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
            }

            Debug.Log("==== Calling _addToCombined objs 添加:" + _goToAdd.Length + 
                " objs 删除:" + _goToDeleteInstanceIDs.Length + 
                " fixOutOfBounds:" + textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs() + 
                " doMultiMaterial:" + textureBakeResults.doMultiMaterial + 
                " disableRenderersInSource:" + disableRendererInSource);

            if (_textureBakeResults.resultMaterials == null || _textureBakeResults.resultMaterials.Length == 0)
            {
                Debug.LogError("TextureBakeResults合并材质为空.");
                return false;
            }

            //源材质与各 submesh 索引的映射（多个材质对应一个网格）
            OrderedDictionary sourceMatsToSubmeshIdx_map = BuildSourceMatsToSubmeshIdxMap(numResultMats);
            if (sourceMatsToSubmeshIdx_map == null)
            {
                return false;
            }
            // --- 2、更新要添加和删除的对象的内部信息，跟踪缓冲区大小的变化。
            //update our internal description of objects being added and deleted keep track of changes to buffer sizes as we do.

            //------------ 计算要删除物体信息 -------------------
            // 将删除的顶点数量
            int totalDeleteVerts = 0;
            //将删除网格三角形
            int[] totalDeleteSubmeshTris = new int[numResultMats];
            //将删除混合图像
            int totalDeleteBlendShapes = 0;

            //in order to decide if a bone can be deleted need to know which dgos use it so build a map
            //为了决定是否可以删除骨骼，需要知道哪些动态物体使用它，建立映射关系
            List<DynamicGameObjectInMeshCombine>[] boneIdxToDgoMap = null;
            //将删除的骨骼 Index
            HashSet<int> boneIdxsToDelete = new HashSet<int>();
            //将添加的骨骼点
            HashSet<BoneAndBindpose> bonesToAdd = new HashSet<BoneAndBindpose>();
            if (settings.renderType == RendererType.skinnedMeshRenderer && _goToDeleteInstanceIDs.Length > 0)
            {
                boneIdxToDgoMap = _buildBoneIdx2dgoMap();
            }

            for (int i = 0; i < _goToDeleteInstanceIDs.Length; i++)
            {
                DynamicGameObjectInMeshCombine dgo = null;
                //根据实例 ID 获取 MB_DynamicGameObject 对象
                TryGetDynamicGoByInstanceID(_goToDeleteInstanceIDs[i], out dgo);
                if (dgo != null)
                {
                    totalDeleteVerts += dgo.numVerts;
                    totalDeleteBlendShapes += dgo.numBlendShapes;
                    if (settings.renderType == RendererType.skinnedMeshRenderer)
                    {//skinRender
                        for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++)
                        {
                            if (boneIdxToDgoMap[dgo.indexesOfBonesUsed[j]].Contains(dgo))
                            {
                                boneIdxToDgoMap[dgo.indexesOfBonesUsed[j]].Remove(dgo);
                                if (boneIdxToDgoMap[dgo.indexesOfBonesUsed[j]].Count == 0)
                                {
                                    boneIdxsToDelete.Add(dgo.indexesOfBonesUsed[j]);
                                }
                            }
                        }
                    }
                    for (int j = 0; j < dgo.submeshNumTris.Length; j++)
                    {
                        totalDeleteSubmeshTris[j] += dgo.submeshNumTris[j];
                    }
                }
                else
                {
                    Debug.LogWarning("正尝试删除不存在于已合并的游戏物体的游戏物体");
                }
            }

            //------------ 计算新增合并的物体信息 -------------------
            List<DynamicGameObjectInMeshCombine> toAddDGOs = new List<DynamicGameObjectInMeshCombine>();
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MeshAnalysisResult[]>(); 

            // 创建网格数据缓存
            MeshChannelsCache meshChannelCache = new MeshChannelsCache(this);
            //将添加顶点
            int totalAddVerts = 0;
            //将添加的网格三角形
            int[] totalAddSubmeshTris = new int[numResultMats];
            //将添加的混合形状
            int totalAddBlendShapes = 0;
            //骨骼Index 映射
            Dictionary<BoneAndBindpose, int> bone2idx = new Dictionary<BoneAndBindpose, int>();
            for (int i = 0; i < MeshData.bones.Length; i++)
            {
                BoneAndBindpose bn = new BoneAndBindpose(MeshData.bones[i], MeshData.bindPoses[i]);
                bone2idx.Add(bn, i);
            }

            //通过 GameObject 创建 DynamicGo
            for (int i = 0; i < _goToAdd.Length; i++)
            {
                //检测已合并网格，将删除，重复添加的游戏物体
                // if not already in mesh or we are deleting and re-adding in same operation
                if (!sourceGoToDynamicGoMap.ContainsKey(_goToAdd[i]) || 
                    Array.FindIndex<int>(_goToDeleteInstanceIDs, o => o == _goToAdd[i].GetInstanceID()) != -1)
                {
                    //将添加的游戏物体
                    GameObject go = _goToAdd[i];
                    Material[] sharedMaterials = MeshBakerUtility.GetGOMaterials(go);
                    if (sharedMaterials == null)
                    {
                        Debug.LogError("游戏物体 " + go.name + "没有 Renderer 组件");
                        _goToAdd[i] = null;
                        return false;
                    }

                    Mesh originMesh = MeshBakerUtility.GetMesh(go);
                    if (originMesh == null)
                    {
                        Debug.LogError("游戏物体 " + go.name + " MeshFilter 或 SkinedMeshRenderer 组件没有网格资源");
                        _goToAdd[i] = null;
                        return false;
                    }
                    else if (MeshBakerUtility.IsRunningAndMeshNotReadWriteable(originMesh))
                    {
                        Debug.LogError("游戏物体 " + go.name + " 网格资源文件 read/write flag set 为 'false'.");
                        _goToAdd[i] = null;
                        return false;
                    }

                    DynamicGameObjectInMeshCombine dgo = new DynamicGameObjectInMeshCombine();
                    //对应源游戏物体材质数组的映射信息数组
                    TextureTilingTreatment[] tilingTreatment = new TextureTilingTreatment[sharedMaterials.Length];
                    Rect[] uvRectsInAtlas = new Rect[sharedMaterials.Length];           //在合并材质的 UV Rect
                    Rect[] encapsulatingRect = new Rect[sharedMaterials.Length];        //在合并材质的 Rect
                    Rect[] sourceMaterialTiling = new Rect[sharedMaterials.Length];     //源材质 Tiling
                    string errorMsg = "";
                    //根据源游戏物体材质，获取合并材质映射信息 tiling，uvRect，encapsulatingRect
                    for (int j = 0; j < sharedMaterials.Length; j++)
                    {
                        var subIdx = sourceMatsToSubmeshIdx_map[sharedMaterials[j]];
                        int resMatIndex;
                        if (subIdx == null)
                        {
                            //新增游戏物体材质不存在于已合并材质中
                            Debug.LogError("源游戏物体 " + go.name + " 使用的材质" + sharedMaterials[j] + " 不存在于合并材质中 ");
                            return false;
                        }
                        else
                        {
                            resMatIndex = (int)subIdx;
                        }

                        //获取 UV 映射
                        if (!matToRect_map.TryGetMaterialToUVRectMap(sharedMaterials[j],
                            originMesh, 
                            j, 
                            resMatIndex, 
                            meshChannelCache, 
                            meshAnalysisResultsCache,
                            out tilingTreatment[j], 
                            out uvRectsInAtlas[j], 
                            out encapsulatingRect[j], 
                            out sourceMaterialTiling[j], 
                            ref errorMsg))
                        {
                            Debug.LogError(errorMsg);
                            _goToAdd[i] = null;
                            return false;
                        }
                    }

                    if (_goToAdd[i] != null)
                    {
                        dgo.name = string.Format("{0} {1}", _goToAdd[i].ToString(), _goToAdd[i].GetInstanceID());
                        dgo.instanceID = _goToAdd[i].GetInstanceID();
                        dgo.gameObject = _goToAdd[i];
                        dgo.uvRects = uvRectsInAtlas;
                        dgo.encapsulatingRect = encapsulatingRect;
                        dgo.sourceMaterialTiling = sourceMaterialTiling;
                        dgo.numVerts = originMesh.vertexCount;
                        toAddDGOs.Add(dgo);

                        if (settings.doBlendShapes)
                        {
                            dgo.numBlendShapes = originMesh.blendShapeCount;
                        }
                        Renderer originRenderer = MeshBakerUtility.GetRenderer(go);
                        if (settings.renderType == RendererType.skinnedMeshRenderer)
                        {
                            _CollectBonesToAddForDGO(dgo, bone2idx, boneIdxsToDelete, bonesToAdd, originRenderer, meshChannelCache);
                        }

                        //光照贴图信息设置
                        if (lightmapIndex == -1)
                        {
                            lightmapIndex = originRenderer.lightmapIndex; //initialize	
                        }
                        if (settings.lightmapOption == LightmapOptions.preserve_current_lightmapping)
                        {
                            if (lightmapIndex != originRenderer.lightmapIndex)
                            {
                                Debug.LogWarning("游戏物体 " + go.name + " 有不同的 lightmap 索引");
                            }
                            if (!MeshBakerUtility.GetActive(go))
                            {
                                Debug.LogWarning("游戏物体 " + go.name + "不在场景中显示. 只能获取显示的游戏物体 lightMap 索引");
                            }
                            if (originRenderer.lightmapIndex == -1)
                            {
                                Debug.LogWarning("游戏物体 " + go.name + " 没有 lightmap 索引");
                            }
                        }
                        dgo.lightmapIndex = originRenderer.lightmapIndex;
                        dgo.lightmapTilingOffset = MeshBakerUtility.GetLightmapTilingOffset(originRenderer);

                        //三角形
                        if (!CollectMaterialTriangles(originMesh, dgo, sharedMaterials, sourceMatsToSubmeshIdx_map))
                        {
                            return false;
                        }
                        dgo.meshSize = originRenderer.bounds.size;
                        dgo.submeshNumTris = new int[numResultMats];
                        dgo.submeshTriIdxs = new int[numResultMats];

                        if (textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs())
                        {
                            if (!_collectOutOfBoundsUVRects2(originMesh, dgo, sharedMaterials, sourceMatsToSubmeshIdx_map, meshAnalysisResultsCache, meshChannelCache))
                            {
                                return false;
                            }
                        }

                        totalAddVerts += dgo.numVerts;
                        totalAddBlendShapes += dgo.numBlendShapes;
                        for (int j = 0; j < dgo._tmpSubmeshTris.Length; j++)
                        {
                            totalAddSubmeshTris[dgo.targetSubmeshIdxs[j]] += dgo._tmpSubmeshTris[j].data.Length;
                        }
                        dgo.invertTriangles = IsMirrored(go.transform.localToWorldMatrix);
                    }
                }
                else
                {
                    //已添加
                    Debug.LogWarning("游戏物体 " + _goToAdd[i].name + " 已合并在当前合并网格中 ");
                    _goToAdd[i] = null;
                }
            }
            //隐藏源游戏物体
            for (int i = 0; i < _goToAdd.Length; i++)
            {
                if (_goToAdd[i] != null && disableRendererInSource)
                {
                    MeshBakerUtility.DisableRendererInSource(_goToAdd[i]);
                    Debug.Log("隐藏源游戏物体  " + _goToAdd[i].name + " 的 renderer 组件");
                }
            }

            // --- 3、分配新缓冲区并复制网格信息
            //新网格顶点数
            int newVertSize = MeshData.verts.Length + totalAddVerts - totalDeleteVerts;
            //新骨骼数组尺寸
            int newBonesSize = MeshData.bindPoses.Length + bonesToAdd.Count - boneIdxsToDelete.Count;
            //新网格Submesh三角形数组
            int[] newSubmeshTrisSize = new int[numResultMats];

            int newBlendShapeSize = blendShapes.Length + totalAddBlendShapes - totalDeleteBlendShapes;

            Debug.Log("Verts adding:" + totalAddVerts + " deleting:" + totalDeleteVerts + 
                " submeshes:" + newSubmeshTrisSize.Length + 
                " bones:" + newBonesSize + 
                " blendShapes:" + newBlendShapeSize);
            for (int i = 0; i < newSubmeshTrisSize.Length; i++)
            {
                newSubmeshTrisSize[i] = submeshTris[i].data.Length + totalAddSubmeshTris[i] - totalDeleteSubmeshTris[i];
                //Debug.Log("submesh :" + i + 
                //    " already contains:" + submeshTris[i].data.Length + 
                //    " tris to be Added:" + totalAddSubmeshTris[i] + 
                //    " tris to be Deleted:" + totalDeleteSubmeshTris[i]);
            }

            if (newVertSize >= MeshBakerUtility.MaxMeshVertexCount())
            {
                Debug.LogError("Cannot add objects. Resulting mesh will have more than " + MeshBakerUtility.MaxMeshVertexCount() + " vertices. " +
                    "Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. " +
                    "You don't have to re-configure the MB2_TextureBaker. " +
                    "Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
                return false;
            }

            Vector3[] newNormals = null;
            Vector4[] newTangents = null;
            Vector2[] newuvs = null;
            Vector2[] newuv2s = null;
            Vector2[] newuv3s = null;
            Vector2[] newuv4s = null;
            Color[] newColors = null;
            BlendShape[] newBlendShapes = null;

            Vector3[] newVerts = new Vector3[newVertSize];
            BoneWeight[] newBoneWeights = new BoneWeight[newVertSize];
            Matrix4x4[] newBindPoses = new Matrix4x4[newBonesSize];
            Transform[] newBones = new Transform[newBonesSize];
            SerializableIntArray[] newSubmeshTris = new SerializableIntArray[numResultMats];    //新网格Submesh--三角形二维数组

            if (settings.doNorm)
                newNormals = new Vector3[newVertSize];
            if (settings.doTan)
                newTangents = new Vector4[newVertSize];
            if (settings.doUV)
                newuvs = new Vector2[newVertSize];
            if (doUV2())
                newuv2s = new Vector2[newVertSize];
            if (settings.doUV3)
                newuv3s = new Vector2[newVertSize];
            if (settings.doUV4)
                newuv4s = new Vector2[newVertSize];
            if (settings.doCol)
                newColors = new Color[newVertSize];
            if (settings.doBlendShapes)
                newBlendShapes = new BlendShape[newBlendShapeSize];

            for (int i = 0; i < newSubmeshTris.Length; i++)
            {
                newSubmeshTris[i] = new SerializableIntArray(newSubmeshTrisSize[i]);
            }

            //设置开始删除状态
            for (int i = 0; i < _goToDeleteInstanceIDs.Length; i++)
            {
                DynamicGameObjectInMeshCombine dgo = null;
                TryGetDynamicGoByInstanceID(_goToDeleteInstanceIDs[i], out dgo);
                if (dgo != null)
                {
                    dgo._beingDeleted = true;
                }
            }

            mbDynamicObjectsInCombinedMesh.Sort();

            //copy existing arrays to narrays gameobj by gameobj omitting deleted ones
            // 删除已删除的数组，将现有数组复制到 newArrays gameobj中
            int targVidx = 0;
            int targBlendShapeIdx = 0;
            int[] targSubmeshTidx = new int[numResultMats];
            int triangleIdxAdjustment = 0;

            //复制已合并资源至 新数组
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                if (!dgo._beingDeleted)
                {
                    Debug.Log("Copying obj in combined arrays idx:" + i);
                    Array.Copy(MeshData.verts, dgo.vertIdx, newVerts, targVidx, dgo.numVerts);
                    if (settings.doNorm)
                    {
                        Array.Copy(MeshData.normals, dgo.vertIdx, newNormals, targVidx, dgo.numVerts);
                    }
                    if (settings.doTan)
                    {
                        Array.Copy(MeshData.tangents, dgo.vertIdx, newTangents, targVidx, dgo.numVerts);
                    }
                    if (settings.doUV)
                    {
                        Array.Copy(MeshData.uvs, dgo.vertIdx, newuvs, targVidx, dgo.numVerts);
                    }
                    if (settings.doUV3)
                    {
                        Array.Copy(MeshData.uv3s, dgo.vertIdx, newuv3s, targVidx, dgo.numVerts);
                    }
                    if (settings.doUV4)
                    {
                        Array.Copy(MeshData.uv4s, dgo.vertIdx, newuv4s, targVidx, dgo.numVerts);
                    }
                    if (doUV2())
                    {
                        Array.Copy(MeshData.uv2s, dgo.vertIdx, newuv2s, targVidx, dgo.numVerts);
                    }
                    if (settings.doCol)
                    {
                        Array.Copy(MeshData.colors, dgo.vertIdx, newColors, targVidx, dgo.numVerts);
                    }
                    if (settings.doBlendShapes)
                    {
                        Array.Copy(blendShapes, dgo.blendShapeIdx, newBlendShapes, targBlendShapeIdx, dgo.numBlendShapes);
                    }
                    if (settings.renderType == RendererType.skinnedMeshRenderer)
                    {
                        Array.Copy(MeshData.boneWeights, dgo.vertIdx, newBoneWeights, targVidx, dgo.numVerts);
                    }

                    //adjust triangles, then copy them over
                    for (int subIdx = 0; subIdx < numResultMats; subIdx++)
                    {
                        int[] sTris = submeshTris[subIdx].data;
                        int sTriIdx = dgo.submeshTriIdxs[subIdx];
                        int sNumTris = dgo.submeshNumTris[subIdx];
                        Debug.Log("Adjusting submesh triangles submesh:" + subIdx + 
                            " startIdx:" + sTriIdx + 
                            " num:" + sNumTris +
                            " nsubmeshTris:" + newSubmeshTris.Length +
                            " targSubmeshTidx:" + targSubmeshTidx.Length);
                        for (int j = sTriIdx; j < sTriIdx + sNumTris; j++)
                        {
                            sTris[j] = sTris[j] - triangleIdxAdjustment;
                        }
                        Array.Copy(sTris, sTriIdx, newSubmeshTris[subIdx].data, targSubmeshTidx[subIdx], sNumTris);
                    }

                    dgo.vertIdx = targVidx;
                    dgo.blendShapeIdx = targBlendShapeIdx;

                    for (int j = 0; j < targSubmeshTidx.Length; j++)
                    {
                        dgo.submeshTriIdxs[j] = targSubmeshTidx[j];
                        targSubmeshTidx[j] += dgo.submeshNumTris[j];
                    }
                    targBlendShapeIdx += dgo.numBlendShapes;
                    targVidx += dgo.numVerts;
                }
                else
                {
                    Debug.Log("Not copying obj: " + i);
                    triangleIdxAdjustment += dgo.numVerts;
                }
            }

            if (settings.renderType == RendererType.skinnedMeshRenderer)
            {
                _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(boneIdxsToDelete, bonesToAdd, newBones, newBindPoses, newBoneWeights, totalDeleteVerts);
            }

            //移除将删除物体 DynamicObjects
            for (int i = mbDynamicObjectsInCombinedMesh.Count - 1; i >= 0; i--)
            {
                if (mbDynamicObjectsInCombinedMesh[i]._beingDeleted)
                {
                    sourceGoToDynamicGoMap.Remove(mbDynamicObjectsInCombinedMesh[i].gameObject);
                    objectsInCombinedMesh.RemoveAt(i);
                    mbDynamicObjectsInCombinedMesh.RemoveAt(i);
                }
            }

            MeshData.verts = newVerts;
            if (settings.doNorm)
                MeshData.normals = newNormals;
            if (settings.doTan)
                MeshData.tangents = newTangents;
            if (settings.doUV)
                MeshData.uvs = newuvs;
            if (settings.doUV3)
                MeshData.uv3s = newuv3s;
            if (settings.doUV4)
                MeshData.uv4s = newuv4s;
            if (doUV2())
                MeshData.uv2s = newuv2s;
            if (settings.doCol)
                MeshData.colors = newColors;
            if (settings.doBlendShapes)
                blendShapes = newBlendShapes;
            if (settings.renderType == RendererType.skinnedMeshRenderer)
                MeshData.boneWeights = newBoneWeights;
            int newBonesStartAtIdx = MeshData.bones.Length - boneIdxsToDelete.Count;
            MeshData.bindPoses = newBindPoses;
            MeshData.bones = newBones;
            submeshTris = newSubmeshTris;

            //insert the new bones into the bones array
            int bidx = 0;
            foreach (BoneAndBindpose t in bonesToAdd)
            {
                newBones[newBonesStartAtIdx + bidx] = t.bone;
                newBindPoses[newBonesStartAtIdx + bidx] = t.bindPose;
                bidx++;
            }

            //写入新网格信息
            for (int i = 0; i < toAddDGOs.Count; i++)
            {
                DynamicGameObjectInMeshCombine dgo = toAddDGOs[i];
                GameObject go = _goToAdd[i];
                int vertsIdx = targVidx;
                int blendShapeIdx = targBlendShapeIdx;
                Mesh mesh = MeshBakerUtility.GetMesh(go);
                Matrix4x4 l2wMat = go.transform.localToWorldMatrix;

                //same as l2w with translation removed
                Matrix4x4 l2wRotScale = l2wMat;
                l2wRotScale[0, 3] = l2wRotScale[1, 3] = l2wRotScale[2, 3] = 0f;

                //can't modify the arrays we get from the cache because they will be modified again
                newVerts = meshChannelCache.GetVertices(mesh);
                Vector3[] newNorms = null;
                Vector4[] newTangs = null;
                if (settings.doNorm)
                    newNorms = meshChannelCache.GetNormals(mesh);
                if (settings.doTan)
                    newTangs = meshChannelCache.GetTangents(mesh);
                if (settings.renderType != RendererType.skinnedMeshRenderer)
                { 
                    //for skinned meshes leave in bind pose
                    for (int j = 0; j < newVerts.Length; j++)
                    {
                        int vIdx = vertsIdx + j;
                        MeshData.verts[vertsIdx + j] = l2wMat.MultiplyPoint3x4(newVerts[j]);
                        if (settings.doNorm)
                        {
                            MeshData.normals[vIdx] = go.transform.TransformDirection(newNorms[j]);
                            MeshData.normals[vIdx] = MeshData.normals[vIdx].normalized;
                        }
                        if (settings.doTan)
                        {
                            float w = newTangs[j].w; //need to preserve the w value
                            Vector3 tn = go.transform.TransformDirection(newTangs[j]);
                            tn.Normalize();
                            MeshData.tangents[vIdx] = tn;
                            MeshData.tangents[vIdx].w = w;
                        }
                    }
                }
                else
                {
                    if (settings.doNorm)
                        newNorms.CopyTo(MeshData.normals, vertsIdx);
                    if (settings.doTan)
                        newTangs.CopyTo(MeshData.tangents, vertsIdx);
                    newVerts.CopyTo(MeshData.verts, vertsIdx);
                }

                int numTriSets = mesh.subMeshCount;
                if (dgo.uvRects.Length < numTriSets)
                {
                    Debug.Log("Mesh " + dgo.name + " has more submeshes than materials");
                    numTriSets = dgo.uvRects.Length;
                }
                else if (dgo.uvRects.Length > numTriSets)
                {
                    Debug.LogWarning("Mesh " + dgo.name + " has fewer submeshes than materials");
                }

                if (settings.doUV)
                {
                    _copyAndAdjustUVsFromMesh(dgo, mesh, vertsIdx, meshChannelCache);
                }

                if (doUV2())
                {
                    _copyAndAdjustUV2FromMesh(dgo, mesh, vertsIdx, meshChannelCache);
                }

                if (settings.doUV3)
                {
                    newuv3s = meshChannelCache.GetUv3(mesh);
                    newuv3s.CopyTo(MeshData.uv3s, vertsIdx);
                }

                if (settings.doUV4)
                {
                    newuv4s = meshChannelCache.GetUv4(mesh);
                    newuv4s.CopyTo(MeshData.uv4s, vertsIdx);
                }

                if (settings.doCol)
                {
                    newColors = meshChannelCache.GetColors(mesh);
                    newColors.CopyTo(MeshData.colors, vertsIdx);
                }

                if (settings.doBlendShapes)
                {
                    newBlendShapes = meshChannelCache.GetBlendShapes(mesh, dgo.instanceID, dgo.gameObject);
                    newBlendShapes.CopyTo(blendShapes, blendShapeIdx);
                }

                if (settings.renderType == RendererType.skinnedMeshRenderer)
                {
                    Renderer r = MeshBakerUtility.GetRenderer(go);
                    _AddBonesToNewBonesArrayAndAdjustBWIndexes(dgo, r, vertsIdx, newBones, newBoneWeights, meshChannelCache);
                }

                for (int combinedMeshIdx = 0; combinedMeshIdx < targSubmeshTidx.Length; combinedMeshIdx++)
                {
                    dgo.submeshTriIdxs[combinedMeshIdx] = targSubmeshTidx[combinedMeshIdx];
                }
                for (int j = 0; j < dgo._tmpSubmeshTris.Length; j++)
                {
                    int[] sts = dgo._tmpSubmeshTris[j].data;
                    for (int k = 0; k < sts.Length; k++)
                    {
                        sts[k] = sts[k] + vertsIdx;
                    }
                    if (dgo.invertTriangles)
                    {
                        //need to reverse winding order
                        for (int k = 0; k < sts.Length; k += 3)
                        {
                            int tmp = sts[k];
                            sts[k] = sts[k + 1];
                            sts[k + 1] = tmp;
                        }
                    }
                    int combinedMeshIdx = dgo.targetSubmeshIdxs[j];
                    sts.CopyTo(submeshTris[combinedMeshIdx].data, targSubmeshTidx[combinedMeshIdx]);
                    dgo.submeshNumTris[combinedMeshIdx] += sts.Length;
                    targSubmeshTidx[combinedMeshIdx] += sts.Length;
                }

                dgo.vertIdx = targVidx;
                dgo.blendShapeIdx = targBlendShapeIdx;

                sourceGoToDynamicGoMap.Add(go, dgo);
                // 添加 GameObject 对象至列表
                objectsInCombinedMesh.Add(go);
                // 添加 MB_DynamicGameObject 对象至列表
                mbDynamicObjectsInCombinedMesh.Add(dgo);

                targVidx += newVerts.Length;
                if (settings.doBlendShapes)
                {
                    targBlendShapeIdx += newBlendShapes.Length;
                }
                for (int j = 0; j < dgo._tmpSubmeshTris.Length; j++) dgo._tmpSubmeshTris[j] = null;
                dgo._tmpSubmeshTris = null;
                Debug.Log("Added to combined:" + dgo.name + 
                    " verts:" + newVerts.Length + 
                    " bindPoses:" + newBindPoses.Length);
                
            }
            if (settings.lightmapOption == LightmapOptions.copy_UV2_unchanged_to_separate_rects)
            {
                _copyUV2unchangedToSeparateRects();
            }

            return true;
        }

        void _copyAndAdjustUVsFromMesh(DynamicGameObjectInMeshCombine dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
            Vector2[] nuvs = meshChannelsCache.GetUv0Raw(mesh);

            int[] done = new int[nuvs.Length]; //use this to track uvs that have already been adjusted don't adjust twice
            for (int l = 0; l < done.Length; l++)
                done[l] = -1;
            bool triangleArraysOverlap = false;
            //Rect uvRectInSrc = new Rect (0f,0f,1f,1f);
            //need to address the UVs through the submesh indexes because
            //each submesh has a different UV index
            for (int submeshIdx = 0; submeshIdx < dgo.targetSubmeshIdxs.Length; submeshIdx++)
            {
                int[] subTris;
                if (dgo._tmpSubmeshTris != null)
                {
                    subTris = dgo._tmpSubmeshTris[submeshIdx].data;
                }
                else
                {
                    subTris = mesh.GetTriangles(submeshIdx);
                }

                Debug.Log(string.Format("Build UV transform for mesh {0} submesh {1} encapsulatingRect {2}",
                    dgo.name,submeshIdx, dgo.encapsulatingRect[submeshIdx]));
                Rect rr = TextureCombinerMerging.BuildTransformMeshUV2AtlasRect(
                    textureBakeResults.resultMaterials[dgo.targetSubmeshIdxs[submeshIdx]].considerMeshUVs,
                dgo.uvRects[submeshIdx],
                dgo.obUVRects == null ? new Rect(0,0,1,1) : dgo.obUVRects[submeshIdx],
                dgo.sourceMaterialTiling[submeshIdx],
                dgo.encapsulatingRect[submeshIdx]);

                for (int l = 0; l < subTris.Length; l++)
                {
                    int vidx = subTris[l];
                    if (done[vidx] == -1)
                    {
                        done[vidx] = submeshIdx; //prevents a uv from being adjusted twice. Same vert can be on more than one submesh.
                        Vector2 nuv = nuvs[vidx]; //don't modify nuvs directly because it is cached and we might be re-using
                                                    //if (textureBakeResults.fixOutOfBoundsUVs) {
                                                    //uvRectInSrc can be larger than (out of bounds uvs) or smaller than 0..1
                                                    //this transforms the uvs so they fit inside the uvRectInSrc sample box 

                        // scale, shift to fit in atlas rect
                        nuv.x = rr.x + nuv.x * rr.width;
                        nuv.y = rr.y + nuv.y * rr.height;
                        MeshData.uvs[vertsIdx + vidx] = nuv;
                    }
                    if (done[vidx] != submeshIdx)
                    {
                        triangleArraysOverlap = true;
                    }
                }
            }
            if (triangleArraysOverlap)
            {
                    Debug.LogWarning(dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
            }
            Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", nuvs.Length));
        }

        void _copyAndAdjustUV2FromMesh(DynamicGameObjectInMeshCombine dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
            Vector2[] nuv2s = meshChannelsCache.GetUv2(mesh);
            if (settings.lightmapOption == LightmapOptions.preserve_current_lightmapping)
            { //has a lightmap
                //this does not work in Unity 5. the lightmapTilingOffset is always 1,1,0,0 for all objects
                //lightMap index is always 1
                Vector2 uvscale2;
                Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
                Vector2 uvscale = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
                Vector2 uvoffset = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
                for (int j = 0; j < nuv2s.Length; j++)
                {
                    uvscale2.x = uvscale.x * nuv2s[j].x;
                    uvscale2.y = uvscale.y * nuv2s[j].y;
                    MeshData.uv2s[vertsIdx + j] = uvoffset + uvscale2;
                }
                Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + nuv2s.Length);
            }
            else
            {
                nuv2s.CopyTo(MeshData.uv2s, vertsIdx);
                Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + nuv2s.Length);
            }
        }

        public override void UpdateSkinnedMeshApproximateBounds()
        {
            UpdateSkinnedMeshApproximateBoundsFromBounds();
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBones()
        {
            if (outputOption == OutputOptions.bakeMeshAssetsInPlace)
            {
                Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
                return;
            }
            if (MeshData.bones.Length == 0)
            {
                if (MeshData.verts.Length > 0)
                    Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
                return;
            }
            if (_targetRenderer == null)
            {
                Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
                return;
            }
            if (!_targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
            {
                Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
                return;
            }
            UpdateSkinnedMeshApproximateBoundsFromBonesStatic(MeshData.bones, (SkinnedMeshRenderer)targetRenderer);
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
        {
            if (outputOption == OutputOptions.bakeMeshAssetsInPlace)
            {
                Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
                return;
            }
            if (MeshData.verts.Length == 0 || mbDynamicObjectsInCombinedMesh.Count == 0)
            {
                if (MeshData.verts.Length > 0)
                    Debug.LogWarning("Nothing in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBoundsFromBounds.");
                return;
            }
            if (_targetRenderer == null)
            {
                Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
                return;
            }
            if (!_targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
            {
                Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
                return;
            }

            UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(objectsInCombinedMesh, (SkinnedMeshRenderer)targetRenderer);
        }

        #endregion

        #region 合并网格：更新已合并物体渲染数据

        public override bool UpdateGameObjects(GameObject[] gos, bool recalcBounds = true,
                                        bool updateVertices = true, bool updateNormals = true, bool updateTangents = true,
                                        bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false,
                                        bool updateColors = false, bool updateSkinningInfo = false)
        {
            return _updateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
        }

        bool _updateGameObjects(GameObject[] gos, bool recalcBounds,
            bool updateVertices, bool updateNormals, bool updateTangents,
            bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, 
            bool updateColors, bool updateSkinningInfo)
        {
            int numResultMats = 1;
            if (textureBakeResults.doMultiMaterial)
                numResultMats = textureBakeResults.resultMaterials.Length;
            
            if (!_Initialize(numResultMats))
            {
                return false;
            }
            
            if (_mesh.vertexCount > 0 && sourceGoToDynamicGoMap.Count == 0)
            {
                Debug.LogWarning("合并网格中有残留的顶点数据，如果在 Editor 下合并，且在运行时修改，则确保关闭 'Clear Buffers After Bake' 选项");
            }
            bool success = true;
            MeshChannelsCache meshChannelCache = new MeshChannelsCache(this);
            MaterialToAtlasRectMapper mat2rect_map = null;
            OrderedDictionary sourceMats2submeshIdx_map = null;
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisResultsCache = null;

            if (updateUV)
            {
                sourceMats2submeshIdx_map = BuildSourceMatsToSubmeshIdxMap(numResultMats);
                if (sourceMats2submeshIdx_map == null)
                {
                    return false;
                }
                mat2rect_map = new MaterialToAtlasRectMapper(textureBakeResults);
                meshAnalysisResultsCache = new Dictionary<int, MeshAnalysisResult[]>();
            }

            for (int i = 0; i < gos.Length; i++)
            {
                success = success && _updateGameObject(gos[i], updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo, 
                    meshChannelCache, meshAnalysisResultsCache, sourceMats2submeshIdx_map, mat2rect_map);
            }
            if (recalcBounds)
                _mesh.RecalculateBounds();
            return success;
        }

        bool _updateGameObject(GameObject go, bool updateVertices, bool updateNormals, bool updateTangents,
                                        bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo, 
                                        MeshChannelsCache meshChannelCache, 
                                        Dictionary<int, MeshAnalysisResult[]> meshAnalysisResultsCache,
                                        OrderedDictionary sourceMats2submeshIdx_map, 
                                        MaterialToAtlasRectMapper mat2rect_map)
        {
            DynamicGameObjectInMeshCombine dgo = null;
            if (!sourceGoToDynamicGoMap.TryGetValue(go, out dgo))
            {
                Debug.LogError("游戏物体 " + go.name + " 没有加入合并");
                return false;
            }
            Mesh originMesh = MeshBakerUtility.GetMesh(go);
            if (dgo.numVerts != originMesh.vertexCount)
            {
                Debug.LogError("游戏物体 " + go.name + " 源网格顶点数被修改. _updateGameObject 必须与合并时为相同网格数");
                return false;
            }

            //更新 UV 数据
            if (settings.doUV && updateUV)
            {
                //需找到 UV rect 

                //源物体材质
                Material[] sharedMaterials = MeshBakerUtility.GetGOMaterials(go);
                //源物体每个材质对应每一个数据的一个矩形
                TextureTilingTreatment[] tilingTreatment = new TextureTilingTreatment[sharedMaterials.Length];
                Rect[] uvRectsInAtlas = new Rect[sharedMaterials.Length];
                Rect[] encapsulatingRect = new Rect[sharedMaterials.Length];
                Rect[] sourceMaterialTiling = new Rect[sharedMaterials.Length];
                string errorMsg = "";
                //源物体子网格个数
                int numSrcSubmeshes = Mathf.Min(originMesh.subMeshCount, sharedMaterials.Length);
                if (numSrcSubmeshes != dgo.targetSubmeshIdxs.Length)
                {
                    Debug.LogError(string.Format("更新合并出错，游戏物体 {0} 材质，或子网格与加入合并时不同，尝试重新加入合并", go.name));
                    return false;
                }

                for (int srcSubmeshIdx = 0; srcSubmeshIdx < numSrcSubmeshes; srcSubmeshIdx++)
                {
                    /*
                    Possibilities:

                    Consider a mesh with three submeshes with materials A, B, C that map to
                    different submeshes in the combined mesh, AA,BB,CC. The user is updating the UVs on a 
                    MeshRenderer so that object 'one' now uses material C => CC instead of A => AA. This will mean that the
                    triangle buffers will need to be resized. This is not allowed in UpdateGameObjects.
                    Must map to the same submesh that the old one mapped to.
                    */
                    //比如具有材质为A，B，C的三个子网格的网格，它们映射到组合网格AA，BB，CC中的不同子网格。 
                    //用户正在更新UV上的UV MeshRenderer，因此对象1现在使用材质C => CC而不是A => AA。 
                    //这将意味着三角形缓冲区将需要调整大小。 在UpdateGameObjects中不允许这样做。
                    //必须映射到与旧网格相同的子网格。

                    object subIdx = sourceMats2submeshIdx_map[sharedMaterials[srcSubmeshIdx]];
                    int resMatIdx;
                    if (subIdx == null)
                    {
                        Debug.LogError("源游戏物体 " + go.name + " 使用的材质" + 
                            sharedMaterials[srcSubmeshIdx] + "不在合并材质中");
                        return false;
                    }
                    else
                    {
                        resMatIdx = (int)subIdx;
                        if (resMatIdx != dgo.targetSubmeshIdxs[srcSubmeshIdx])
                        {
                            Debug.LogError(string.Format("Update 合并物体失败 {0}. 其源材质 {1} 在合并时是映射在其他子网格，需重新调用 AddDelete.", go.name, sharedMaterials[srcSubmeshIdx]));
                            return false;
                        }
                    }

                    if (!mat2rect_map.TryGetMaterialToUVRectMap(sharedMaterials[srcSubmeshIdx], 
                        originMesh, 
                        srcSubmeshIdx, 
                        resMatIdx, 
                        meshChannelCache, 
                        meshAnalysisResultsCache,
                        out tilingTreatment[srcSubmeshIdx],
                        out uvRectsInAtlas[srcSubmeshIdx],
                        out encapsulatingRect[srcSubmeshIdx],
                        out sourceMaterialTiling[srcSubmeshIdx],
                        ref errorMsg))
                    {
                        Debug.LogError(errorMsg);
                        return false;
                    }
                }

                dgo.uvRects = uvRectsInAtlas;
                dgo.encapsulatingRect = encapsulatingRect;
                dgo.sourceMaterialTiling = sourceMaterialTiling;
                _copyAndAdjustUVsFromMesh(dgo, originMesh, dgo.vertIdx, meshChannelCache);
            }

            //更新 UV2 LightMap 数据
            if (doUV2() && updateUV2)
            {
                _copyAndAdjustUV2FromMesh(dgo, originMesh, dgo.vertIdx, meshChannelCache);
            }

            //更新 SkinRenderer 数据
            if (settings.renderType == RendererType.skinnedMeshRenderer && updateSkinningInfo)
            {
                //only does BoneWeights. Used to do Bones and BindPoses but it doesn't make sence.
                //if updating Bones and Bindposes should remove and re-add
                Renderer r = MeshBakerUtility.GetRenderer(go);
                BoneWeight[] bws = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
                Transform[] bs = _getBones(r);
                //assumes that the bones and boneweights have not been reeordered
                int bwIdx = dgo.vertIdx; //the index in the verts array
                bool switchedBonesDetected = false;
                for (int i = 0; i < bws.Length; i++)
                {
                    if (bs[bws[i].boneIndex0] != MeshData.bones[MeshData.boneWeights[bwIdx].boneIndex0])
                    {
                        switchedBonesDetected = true;
                        break;
                    }
                    MeshData.boneWeights[bwIdx].weight0 = bws[i].weight0;
                    MeshData.boneWeights[bwIdx].weight1 = bws[i].weight1;
                    MeshData.boneWeights[bwIdx].weight2 = bws[i].weight2;
                    MeshData.boneWeights[bwIdx].weight3 = bws[i].weight3;
                    bwIdx++;
                }
                if (switchedBonesDetected)
                {
                    Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + dgo.name);
                }
            }

            // now do verts, norms, tangents, colors and uv1
            //更新顶点
            Matrix4x4 l2wMat = go.transform.localToWorldMatrix;
            if (updateVertices)
            {
                Vector3[] nverts = meshChannelCache.GetVertices(originMesh);
                for (int j = 0; j < nverts.Length; j++)
                {
                    MeshData.verts[dgo.vertIdx + j] = l2wMat.MultiplyPoint3x4(nverts[j]);
                }
            }

            l2wMat[0, 3] = l2wMat[1, 3] = l2wMat[2, 3] = 0f;
            //更新法线
            if (settings.doNorm && updateNormals)
            {
                Vector3[] nnorms = meshChannelCache.GetNormals(originMesh);
                for (int j = 0; j < nnorms.Length; j++)
                {
                    int vIdx = dgo.vertIdx + j;
                    MeshData.normals[vIdx] = l2wMat.MultiplyPoint3x4(nnorms[j]);
                    MeshData.normals[vIdx] = MeshData.normals[vIdx].normalized;
                }
            }

            //更新切线
            if (settings.doTan && updateTangents)
            {
                Vector4[] ntangs = meshChannelCache.GetTangents(originMesh);
                for (int j = 0; j < ntangs.Length; j++)
                {
                    int midx = dgo.vertIdx + j;
                    float w = ntangs[j].w; //need to preserve the w value
                    Vector3 tn = l2wMat.MultiplyPoint3x4(ntangs[j]);
                    tn.Normalize();
                    MeshData.tangents[midx] = tn;
                    MeshData.tangents[midx].w = w;
                }
            }

            //更新颜色
            if (settings.doCol && updateColors)
            {
                Color[] ncolors = meshChannelCache.GetColors(originMesh);
                for (int j = 0; j < ncolors.Length; j++) MeshData.colors[dgo.vertIdx + j] = ncolors[j];
            }

            //更新 UV3
            if (settings.doUV3 && updateUV3)
            {
                Vector2[] nuv3 = meshChannelCache.GetUv3(originMesh);
                for (int j = 0; j < nuv3.Length; j++) MeshData.uv3s[dgo.vertIdx + j] = nuv3[j];
            }

            //更新 UV4
            if (settings.doUV4 && updateUV4)
            {
                Vector2[] nuv4 = meshChannelCache.GetUv4(originMesh);
                for (int j = 0; j < nuv4.Length; j++) MeshData.uv4s[dgo.vertIdx + j] = nuv4[j];
            }

            return true;
        }

        #endregion

        #region 合并网格：将网格数据应用至场景物体中

        public override void Apply(GenerateUV2Delegate uv2GenerationMethod)
        {
            bool doBones = false;
            if (settings.renderType == RendererType.skinnedMeshRenderer)
                doBones = true;
            Apply(true,
                true,
                settings.doNorm,
                settings.doTan,
                settings.doUV,
                doUV2(),
                settings.doUV3,
                settings.doUV4,
                settings.doCol,
                doBones,
                settings.doBlendShapes,
                uv2GenerationMethod);
        }

        //将合并后的数据应用至网格
        public override void Apply(bool triangles,
                          bool vertices,
                          bool normals,
                          bool tangents,
                          bool uvs,
                          bool uv2,
                          bool uv3,
                          bool uv4,
                          bool colors,
                          bool bones = false,
                          bool blendShapesFlag = false,
                          GenerateUV2Delegate uv2GenerationMethod = null)
        {
            if (_validationLevel >= ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj())
                return;
            if (_mesh == null)
            {
                Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
                return;
            }
            else
            {
                //Debug.Log(string.Format(" tri={0} vert={1} norm={2} tan={3} uv={4} col={5} uv3={6} uv4={7} uv2={8} bone={9} blendShape{10} meshID={11}",
                //        triangles, vertices, normals, tangents, uvs, colors, uv3, uv4, uv2, bones, blendShapes, _mesh.GetInstanceID()));
                //网格有变化，则清空网格数组，设置网格 Index
                if (triangles || _mesh.vertexCount != MeshData.verts.Length)
                {
                    bool justClearTriangles = triangles && !vertices && !normals && !tangents && !uvs && !colors && !uv3 && !uv4 && !uv2 && !bones;
                    MeshBakerUtility.SetMeshIndexFormatAndClearMesh(_mesh, MeshData.verts.Length, vertices, justClearTriangles);
                }

                //顶点数据处理
                if (vertices)
                {
                    Vector3[] vertsToWrite = MeshData.verts;
                    if (MeshData.verts.Length > 0)
                    {
                        //重新定位中心点
                        if (settings.recenterVertsToBoundsCenter && settings.renderType == RendererType.meshRenderer)
                        {
                            vertsToWrite = new Vector3[MeshData.verts.Length];
                            Vector3 max = MeshData.verts[0];
                            Vector3 min = MeshData.verts[0];
                            for (int i = 1; i < MeshData.verts.Length; i++)
                            {
                                Vector3 v = MeshData.verts[i];
                                if (max.x < v.x) max.x = v.x;
                                if (max.y < v.y) max.y = v.y;
                                if (max.z < v.z) max.z = v.z;
                                if (min.x > v.x) min.x = v.x;
                                if (min.y > v.y) min.y = v.y;
                                if (min.z > v.z) min.z = v.z;
                            }
                            Vector3 center = (max + min) / 2f;
                            for (int i = 0; i < MeshData.verts.Length; i++)
                            {
                                vertsToWrite[i] = MeshData.verts[i] - center;
                            }
                            targetRenderer.transform.position = center;
                        }
                        else
                        {
                            targetRenderer.transform.position = Vector3.zero;
                        }
                    }
                    _mesh.vertices = vertsToWrite;
                }
                //三角形数据，材质处理
                if (triangles && _textureBakeResults)
                {
                    if (_textureBakeResults == null)
                    {
                        Debug.LogError("Texture Bake Result 为空.");
                    }
                    else
                    {
                        SerializableIntArray[] submeshTrisToUse = GetSubmeshTrisWithShowHideApplied();
                        //submeshes with zero length tris cause error messages. must exclude these
                        //tris长度为 0 的的子网格会导致错误消息。 必须排除这些
                        int numNonZero = _mesh.subMeshCount = _numNonZeroLengthSubmeshTris(submeshTrisToUse);// submeshTrisToUse.Length;
                        int submeshIdx = 0;
                        for (int i = 0; i < submeshTrisToUse.Length; i++)
                        {
                            if (submeshTrisToUse[i].data.Length != 0)
                            {
                                _mesh.SetTriangles(submeshTrisToUse[i].data, submeshIdx);
                                submeshIdx++;
                            }
                        }
                        _updateMaterialsOnTargetRenderer(submeshTrisToUse, numNonZero);
                    }
                }
                //法线数据处理
                if (normals)
                {
                    if (settings.doNorm)
                    {
                        _mesh.normals = MeshData.normals;
                    }
                    else
                    {
                        Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
                    }
                }
                //切线数据处理
                if (tangents)
                {
                    if (settings.doTan)
                    {
                        _mesh.tangents = MeshData.tangents;
                    }
                    else
                    {
                        Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents");
                    }
                }
                //UV 数据处理
                if (uvs)
                {
                    if (settings.doUV)
                    {
                        _mesh.uv = MeshData.uvs;
                    }
                    else
                    {
                        Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs");
                    }
                }
                //颜色值数据处理
                if (colors)
                {
                    if (settings.doCol)
                    {
                        _mesh.colors = MeshData.colors;
                    }
                    else
                    {
                        Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors");
                    }
                }
                //UV3 数据处理
                if (uv3)
                {
                    if (settings.doUV3)
                    {
                        MeshBakerUtility.MeshAssignUV3(_mesh, MeshData.uv3s);
                    }
                    else
                    {
                        Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s");
                    }
                }
                //UV4 数据处理
                if (uv4)
                {
                    if (settings.doUV4)
                    {
                        MeshBakerUtility.MeshAssignUV4(_mesh, MeshData.uv4s);
                    }
                    else
                    {
                        Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s");
                    }
                }
                //UV2 数据处理
                //烘焙Lightmap以后unity会自动给参与烘焙的所有mesh添加 uv2 的属性
                if (uv2)
                {
                    if (doUV2())
                    {
                        _mesh.uv2 = MeshData.uv2s;
                    }
                    else
                    {
                        Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + settings.lightmapOption);
                    }
                }
                bool do_generate_new_UV2_layout = false;
                if (settings.renderType != RendererType.skinnedMeshRenderer &&
                    settings.lightmapOption == LightmapOptions.generate_new_UV2_layout)
                {
                    if (uv2GenerationMethod != null)
                    {
                        uv2GenerationMethod(_mesh, settings.uv2UnwrappingParamsHardAngle, settings.uv2UnwrappingParamsPackMargin);
                        Debug.Log("为合并网格生成 UV2 布局 ");
                    }
                    else
                    {
                        Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
                    }
                    do_generate_new_UV2_layout = true;
                }
                else if (settings.renderType == RendererType.skinnedMeshRenderer &&
                    settings.lightmapOption == LightmapOptions.generate_new_UV2_layout)
                {
                    Debug.LogWarning(" 无法为SkinnedMeshRenderer 物体生成 UV2 布局 ");
                }
                if (settings.renderType != RendererType.skinnedMeshRenderer &&
                    settings.lightmapOption == LightmapOptions.generate_new_UV2_layout &&
                    do_generate_new_UV2_layout == false)
                {
                    Debug.LogError("生成 UV2 布局 失败.");
                }

                //渲染类型为 skinnedMesh 
                if (settings.renderType == RendererType.skinnedMeshRenderer)
                {
                    if (MeshData.verts.Length == 0)
                    {
                        //disable mesh renderer to avoid skinning warning
                        targetRenderer.enabled = false;
                    }
                    else
                    {
                        targetRenderer.enabled = true;
                    }
                    //needed so that updating local bounds will take affect
                    bool uwos = ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen = true;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen = uwos;
                }

                //骨骼数据处理
                if (bones)
                {
                    _mesh.bindposes = this.MeshData.bindPoses;
                    _mesh.boneWeights = this.MeshData.boneWeights;
                }

                //图像混合处理
                if (blendShapesFlag)
                {
                    if (blendShapesInCombined.Length != blendShapes.Length)
                        blendShapesInCombined = new BlendShape[blendShapes.Length];
                    Vector3[] vs = new Vector3[MeshData.verts.Length];
                    Vector3[] ns = new Vector3[MeshData.verts.Length];
                    Vector3[] ts = new Vector3[MeshData.verts.Length];

                    MeshBakerUtility.ClearBlendShapes(_mesh);
                    for (int i = 0; i < blendShapes.Length; i++)
                    {
                        DynamicGameObjectInMeshCombine dgo = sourceGoToDynamicGoMap[blendShapes[i].gameObject];
                        if (dgo != null)
                        {
                            for (int j = 0; j < blendShapes[i].frames.Length; j++)
                            {
                                MBBlendShapeFrame frame = blendShapes[i].frames[j];
                                int destIdx = dgo.vertIdx;
                                Array.Copy(frame.vertices, 0, vs, destIdx, blendShapes[i].frames[j].vertices.Length);
                                Array.Copy(frame.normals, 0, ns, destIdx, blendShapes[i].frames[j].normals.Length);
                                Array.Copy(frame.tangents, 0, ts, destIdx, blendShapes[i].frames[j].tangents.Length);
                                MeshBakerUtility.AddBlendShapeFrame(_mesh, blendShapes[i].name + blendShapes[i].gameObjectID, frame.frameWeight, vs, ns, ts);
                                _ZeroArray(vs, destIdx, blendShapes[i].frames[j].vertices.Length);
                                _ZeroArray(ns, destIdx, blendShapes[i].frames[j].normals.Length);
                                _ZeroArray(ts, destIdx, blendShapes[i].frames[j].tangents.Length);
                            }
                        }
                        else
                        {
                            Debug.LogError("InstanceID in blend shape that was not in instance2combinedMap");
                        }
                        blendShapesInCombined[i] = blendShapes[i];

                        //this is necessary to get the renderer to refresh its data about the blendshapes.
                        ((SkinnedMeshRenderer)_targetRenderer).sharedMesh = null;
                        ((SkinnedMeshRenderer)_targetRenderer).sharedMesh = _mesh;

                        // Add the map to the target renderer.
                        if (settings.doBlendShapes)
                        {
                            BlendShape2CombinedMap mapComponent = _targetRenderer.GetComponent<BlendShape2CombinedMap>();
                            if (mapComponent == null) mapComponent = _targetRenderer.gameObject.AddComponent<BlendShape2CombinedMap>();
                            SerializableSourceBlendShape2Combined map = mapComponent.GetMap();
                            BuildSourceBlendShapeToCombinedSerializableIndexMap(map);
                        }
                    }
                }

                //计算网格边界
                if (triangles || vertices)
                {
                    _mesh.RecalculateBounds();
                }
                //优化网格（Unity 接口）
                if (settings.optimizeAfterBake && !Application.isPlaying)
                {
                    MeshBakerUtility.OptimizeMesh(_mesh);
                }
            }  
        }

        public virtual void ApplyShowHide()
        {
            if (_validationLevel >= ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj()) return;
            if (_mesh != null)
            {
                if (settings.renderType == RendererType.meshRenderer)
                {
                    //for MeshRenderer meshes this is needed for adding. It breaks skinnedMeshRenderers
                    MeshBakerUtility.MeshClear(_mesh, true);
                    _mesh.vertices = MeshData.verts;
                }
                SerializableIntArray[] submeshTrisToUse = GetSubmeshTrisWithShowHideApplied();
                if (textureBakeResults.doMultiMaterial)
                {
                    //submeshes with zero length tris cause error messages. must exclude these
                    int numNonZero = _mesh.subMeshCount = _numNonZeroLengthSubmeshTris(submeshTrisToUse);// submeshTrisToUse.Length;
                    int submeshIdx = 0;
                    for (int i = 0; i < submeshTrisToUse.Length; i++)
                    {
                        if (submeshTrisToUse[i].data.Length != 0)
                        {
                            _mesh.SetTriangles(submeshTrisToUse[i].data, submeshIdx);
                            submeshIdx++;
                        }
                    }
                    _updateMaterialsOnTargetRenderer(submeshTrisToUse, numNonZero);
                }
                else
                {
                    _mesh.triangles = submeshTrisToUse[0].data;
                }

                if (settings.renderType == RendererType.skinnedMeshRenderer)
                {
                    if (MeshData.verts.Length == 0)
                    {
                        //disable mesh renderer to avoid skinning warning
                        targetRenderer.enabled = false;
                    }
                    else
                    {
                        targetRenderer.enabled = true;
                    }
                    //needed so that updating local bounds will take affect
                    bool uwos = ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen = true;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen = uwos;
                }
                Debug.Log("ApplyShowHide");
            }
            else
            {
                Debug.LogError("Need to add objects to this meshbaker before calling ApplyShowHide");
            }
        }

        public SerializableIntArray[] GetSubmeshTrisWithShowHideApplied()
        {
            bool containsHiddenObjects = false;
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {
                if (mbDynamicObjectsInCombinedMesh[i].show == false)
                {
                    containsHiddenObjects = true;
                    break;
                }
            }
            if (containsHiddenObjects)
            {
                int[] newLengths = new int[submeshTris.Length];
                SerializableIntArray[] newSubmeshTris = new SerializableIntArray[submeshTris.Length];
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                    if (dgo.show)
                    {
                        for (int j = 0; j < dgo.submeshNumTris.Length; j++)
                        {
                            newLengths[j] += dgo.submeshNumTris[j];
                        }
                    }
                }
                for (int i = 0; i < newSubmeshTris.Length; i++)
                {
                    newSubmeshTris[i] = new SerializableIntArray(newLengths[i]);
                }
                int[] idx = new int[newSubmeshTris.Length];
                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                    if (dgo.show)
                    {
                        for (int j = 0; j < submeshTris.Length; j++)
                        { //for each submesh
                            int[] triIdxs = submeshTris[j].data;
                            int startIdx = dgo.submeshTriIdxs[j];
                            int endIdx = startIdx + dgo.submeshNumTris[j];
                            for (int k = startIdx; k < endIdx; k++)
                            {
                                newSubmeshTris[j].data[idx[j]] = triIdxs[k];
                                idx[j] = idx[j] + 1;
                            }
                        }
                    }
                }
                return newSubmeshTris;
            }
            else
            {
                return submeshTris;
            }
        }

        int _numNonZeroLengthSubmeshTris(SerializableIntArray[] subTris)
        {
            int num = 0;
            for (int i = 0; i < subTris.Length; i++)
            {
                if (subTris[i].data.Length > 0)
                    num++;
            }
            return num;
        }

        private void _updateMaterialsOnTargetRenderer(SerializableIntArray[] subTris, int numNonZeroLengthSubmeshTris)
        {
            //zero length triangle arrays in mesh cause errors. have excluded these sumbeshes so must exclude these materials
            if (subTris.Length != textureBakeResults.resultMaterials.Length)
                Debug.LogError("Mismatch between number of submeshes and number of result materials");
            Material[] resMats = new Material[numNonZeroLengthSubmeshTris];
            int submeshIdx = 0;
            for (int i = 0; i < subTris.Length; i++)
            {
                if (subTris[i].data.Length > 0)
                {
                    resMats[submeshIdx] = _textureBakeResults.resultMaterials[i].combinedMaterial;
                    submeshIdx++;
                }
            }
            targetRenderer.materials = resMats;
        }

        void _ZeroArray(Vector3[] arr, int idx, int length)
        {
            int bound = idx + length;
            for (int i = idx; i < bound; i++)
            {
                arr[i] = Vector3.zero;
            }
        }
        #endregion

        #region 验证方法

        /// <summary>
        /// 验证合并材质资源
        /// </summary>
        /// <returns></returns>
        bool _validateTextureBakeResults()
        {
            if (_textureBakeResults == null)
            {
                Debug.LogError("Texture Bake Results Asset 为空无法合并.");
                return false;
            }
            if (_textureBakeResults.materialsAndUVRects == null || _textureBakeResults.materialsAndUVRects.Length == 0)
            {
                Debug.LogError("Texture Bake Results Asset 没有材质 UV Rect 映射，需先合并材质");
                return false;
            }

            if (_textureBakeResults.resultMaterials == null || _textureBakeResults.resultMaterials.Length == 0)
            {
                Debug.LogError("Texture Bake Results Asset 缺少材质，需先合并材质");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 校验游戏物体和渲染器
        /// </summary>
        /// <returns></returns>
        public bool ValidateTargRendererAndMeshAndResultSceneObj()
        {
            if (_resultSceneObject == null)
            {
                Debug.LogError("缺少渲染合并结果的 GameObject");
                return false;
            }
            else
            {
                if (_targetRenderer == null)
                {
                    Debug.LogError("缺少渲染合并结果的 Renderer");
                    return false;
                }
                else
                {
                    if (_targetRenderer.transform.parent != _resultSceneObject.transform)
                    {
                        Debug.LogError("渲染合并结果的 Renderer 所在物体不是 _resultSceneObject 的子物体。");
                        return false;
                    }
                    if (settings.renderType == RendererType.skinnedMeshRenderer)
                    {
                        if (!(_targetRenderer is SkinnedMeshRenderer))
                        {
                            Debug.LogError("渲染类型为 skinned mesh renderer，渲染合并结果的 Renderer 不是 SkinnedMeshRenderer。");
                            return false;
                        }
                    }
                    if (settings.renderType == RendererType.meshRenderer)
                    {
                        if (!(_targetRenderer is MeshRenderer))
                        {
                            Debug.LogError("渲染类型为 mesh renderer，渲染合并结果的 Renderer 不是 MeshRenderer。");
                            return false;
                        }
                        MeshFilter mf = _targetRenderer.GetComponent<MeshFilter>();
                        if (_mesh != mf.sharedMesh)
                        {
                            Debug.LogError("渲染合并结果的 Renderer 渲染网格并非当前网格");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        //tests if a matrix has been mirrored
        bool IsMirrored(Matrix4x4 tm)
        {
            Vector3 x = tm.GetRow(0);
            Vector3 y = tm.GetRow(1);
            Vector3 z = tm.GetRow(2);
            x.Normalize();
            y.Normalize();
            z.Normalize();
            float an = Vector3.Dot(Vector3.Cross(x, y), z);
            return an >= 0 ? false : true;
        }

        public override void CheckIntegrity()
        {
            if (!MeshBakerUtility.DO_INTEGRITY_CHECKS) return;
            //check bones.
            if (settings.renderType == RendererType.skinnedMeshRenderer)
            {

                for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
                {
                    DynamicGameObjectInMeshCombine dgo = mbDynamicObjectsInCombinedMesh[i];
                    HashSet<int> usedBonesWeights = new HashSet<int>();
                    HashSet<int> usedBonesIndexes = new HashSet<int>();
                    for (int j = dgo.vertIdx; j < dgo.vertIdx + dgo.numVerts; j++)
                    {
                        usedBonesWeights.Add(MeshData.boneWeights[j].boneIndex0);
                        usedBonesWeights.Add(MeshData.boneWeights[j].boneIndex1);
                        usedBonesWeights.Add(MeshData.boneWeights[j].boneIndex2);
                        usedBonesWeights.Add(MeshData.boneWeights[j].boneIndex3);
                    }
                    for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++)
                    {
                        usedBonesIndexes.Add(dgo.indexesOfBonesUsed[j]);
                    }

                    usedBonesIndexes.ExceptWith(usedBonesWeights);
                    if (usedBonesIndexes.Count > 0)
                    {
                        Debug.LogError("The bone indexes were not the same. " + usedBonesWeights.Count + " " + usedBonesIndexes.Count);
                    }
                    for (int j = 0; j < dgo.indexesOfBonesUsed.Length; j++)
                    {
                        if (j < 0 || j > MeshData.bones.Length)
                            Debug.LogError("Bone index was out of bounds.");
                    }
                    if (settings.renderType == RendererType.skinnedMeshRenderer && dgo.indexesOfBonesUsed.Length < 1)
                        Debug.Log("DGO had no bones");

                }

            }
            //check blend shapes
            if (settings.doBlendShapes)
            {
                if (settings.renderType != RendererType.skinnedMeshRenderer)
                {
                    Debug.LogError("Blend shapes can only be used with skinned meshes.");
                }
            }
        }

        #endregion

    }
}