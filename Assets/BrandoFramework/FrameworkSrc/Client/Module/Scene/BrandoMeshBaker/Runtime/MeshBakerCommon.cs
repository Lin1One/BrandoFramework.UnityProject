
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// Abstract root of the mesh combining classes
    /// </summary>
    public abstract class MeshBakerCommon : MeshBakerRoot
    {
        public List<GameObject> objsToMesh;

        public abstract MeshCombineHandlerBase meshCombiner
        {
            get;
        }

        public bool useObjsToMeshFromTexBaker = true;

        public bool clearBuffersAfterBake = true;

        //t0do put this in the batch baker
        public string bakeAssetsInPlaceFolderPath;

        [HideInInspector]
        public GameObject resultPrefab;

#if UNITY_EDITOR
        [ContextMenu("Create Mesh Baker Settings Asset")]
        public void CreateMeshBakerSettingsAsset()
        {
            string newFilePath = UnityEditor.EditorUtility.SaveFilePanelInProject("New Mesh Baker Settings", "MeshBakerSettings", "asset", "Create a new Mesh Baker Settings Asset");
            if (newFilePath != null)
            {
                MeshCombinerSettings asset = ScriptableObject.CreateInstance<MeshCombinerSettings>();
                UnityEditor.AssetDatabase.CreateAsset(asset, newFilePath);
            }
        }

        [ContextMenu("Copy settings from Shared Settings")]
        public void CopyMySettingsToAssignedSettingsAsset()
        {
            if (meshCombiner.settingsHolder == null)
            {
                Debug.LogError("No Shared Settings Asset Assigned.");
                return;
            }

            UnityEditor.Undo.RecordObject(this, "Undo copy settings");
            _CopySettings(meshCombiner.settingsHolder.GetMeshBakerSettings(), meshCombiner);
            Debug.Log("Copied settings from assigned Shared Settings to this Mesh Baker.");
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Copy settings to Shared Settings")]
        public void CopyAssignedSettingsAssetToMySettings()
        {
            if (meshCombiner.settingsHolder == null)
            {
                Debug.LogError("No Shared Settings Asset Assigned.");
                return;
            }

            if (meshCombiner.settingsHolder is UnityEngine.Object)
                UnityEditor.Undo.RecordObject((UnityEngine.Object)meshCombiner.settingsHolder, "Undo copy settings");
            _CopySettings(meshCombiner, meshCombiner.settingsHolder.GetMeshBakerSettings());
            Debug.Log("Copied settings from this Mesh Baker to the assigned Shared Settings asset.");
            if (meshCombiner.settingsHolder is UnityEngine.Object)
                UnityEditor.EditorUtility.SetDirty((UnityEngine.Object)meshCombiner.settingsHolder);
        }

        void _CopySettings(IMeshCombinerSetting src, IMeshCombinerSetting targ)
        {
            targ.clearBuffersAfterBake = src.clearBuffersAfterBake;
            targ.doBlendShapes = src.doBlendShapes;
            targ.doCol = src.doCol;
            targ.doNorm = src.doNorm;
            targ.doTan = src.doTan;
            targ.doUV = src.doUV;
            targ.doUV3 = src.doUV3;
            targ.doUV4 = src.doUV4;
            targ.optimizeAfterBake = src.optimizeAfterBake;
            targ.recenterVertsToBoundsCenter = src.recenterVertsToBoundsCenter;
            targ.lightmapOption = src.lightmapOption;
            targ.renderType = src.renderType;
            targ.uv2UnwrappingParamsHardAngle = src.uv2UnwrappingParamsHardAngle;
            targ.uv2UnwrappingParamsPackMargin = src.uv2UnwrappingParamsPackMargin;
        }
#endif

        public override TextureBakeResults textureBakeResults
        {
            get { return meshCombiner.textureBakeResults; }
            set { meshCombiner.textureBakeResults = value; }
        }

        public override List<GameObject> GetObjectsToCombine()
        {
            if (useObjsToMeshFromTexBaker)
            {
                TextureCombineEntrance tb = gameObject.GetComponent<TextureCombineEntrance>();
                if (tb == null)
                    tb = gameObject.transform.parent.GetComponent<TextureCombineEntrance>();
                if (tb != null)
                {
                    return tb.GetObjectsToCombine();
                }
                else
                {
                    Debug.LogWarning("Use Objects To Mesh From Texture Baker was checked but no texture baker");
                    return new List<GameObject>();
                }
            }
            else
            {
                if (objsToMesh == null) objsToMesh = new List<GameObject>();
                return objsToMesh;
            }
        }

        public void EnableDisableSourceObjectRenderers(bool show)
        {
            for (int i = 0; i < GetObjectsToCombine().Count; i++)
            {
                GameObject go = GetObjectsToCombine()[i];
                if (go != null)
                {
                    Renderer mr = MeshBakerUtility.GetRenderer(go);
                    if (mr != null)
                    {
                        mr.enabled = show;
                    }

                    LODGroup lodG = mr.GetComponentInParent<LODGroup>();
                    if (lodG != null)
                    {
                        bool isOnlyInGroup = true;
                        LOD[] lods = lodG.GetLODs();
                        for (int j = 0; j < lods.Length; j++)
                        {
                            for (int k = 0; k < lods[j].renderers.Length; k++)
                            {
                                if (lods[j].renderers[k] != mr)
                                {
                                    isOnlyInGroup = false;
                                    break;
                                }
                            }
                        }

                        if (isOnlyInGroup)
                        {
                            lodG.enabled = show;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Clears the meshs and mesh related data but does not destroy it.
        /// </summary>
        public virtual void ClearMesh()
        {
            meshCombiner.ClearMesh();
        }

        /// <summary>
        ///  Clears and desroys the mesh. Clears mesh related data.
        /// </summary>		
        public virtual void DestroyMesh()
        {
            meshCombiner.DestroyMesh();
        }

        public virtual void DestroyMeshEditor(EditorMethodsInterface editorMethods)
        {
            meshCombiner.DestroyMeshEditor(editorMethods);
        }

        public virtual int GetNumObjectsInCombined()
        {
            return meshCombiner.GetNumObjectsInCombined();
        }

        public virtual int GetNumVerticesFor(GameObject go)
        {
            return meshCombiner.GetNumVerticesFor(go);
        }

        /// <summary>
        /// Gets the texture baker on this component or its parent if it exists
        /// </summary>
        /// <returns>The texture baker.</returns>
        public TextureCombineEntrance GetTextureBaker()
        {
            TextureCombineEntrance tb = GetComponent<TextureCombineEntrance>();
            if (tb != null)
                return tb;
            if (transform.parent != null) return transform.parent.GetComponent<TextureCombineEntrance>();
            return null;
        }

        /// <summary>
        /// Adds and deletes objects from the combined mesh. gos and deleteGOs can be null. 
        /// You need to call Apply or ApplyAll to see the changes. 
        /// objects in gos must not include objects already in the combined mesh.
        /// objects in gos and deleteGOs must be the game objects with a Renderer component
        /// This method is slow, so should be called as infrequently as possible.
        /// 从组合的网格中添加和删除对象。 gos和deleteGOs可以为null。
        /// 您需要调用Apply或ApplyAll来查看更改。
        /// gos中的对象不得包含已在组合网格中的对象。
        /// gos和deleteGO中的对象必须是带有Renderer组件的游戏对象
        /// 该方法很慢，因此应尽可能少地调用。
        /// <returns>
        /// The first generated combined mesh
        /// 第一个生成的组合网格
        /// </returns>
        /// <param name='gos'>
        /// 要添加到组合网格中的对象数组。数组可以为null。不得包含对象
        /// 已经在组合网格中。数组必须包含带有渲染组件的游戏对象。
        /// gos. Array of objects to add to the combined mesh. Array can be null. Must not include objects
        /// already in the combined mesh. Array must contain game objects with a render component.
        /// </param>
        /// <param name='deleteGOs'>
        /// deleteGOs. Array of objects to delete from the combined mesh. Array can be null.
        /// 要从组合网格删除的对象数组。数组可以为null。
        /// </param>
        /// <param name='disableRendererInSource'>
        /// Disable renderer component on objects in gos after they have been added to the combined mesh.
        /// 在将对象添加到组合网格中之后，对gos中的对象禁用渲染器组件。
        /// </param>
        /// <param name='fixOutOfBoundUVs'>
        /// Whether to fix out of bounds UVs in meshes as they are being added. This paramater should be set to the same as the combined material.
        /// 是否在添加网格时固定网格中的UV。该参数应设置为与组合材料相同。
        /// </param>
        /// </summary>
        public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

        /// <summary>
        /// This is the best version to use for deleting game objects since the source GameObjects may have been destroyed
        /// Internaly Mesh Baker only stores the instanceID for Game Objects, so objects can be removed after they have been destroyed
        /// </summary>
        public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true);

        /// <summary>
        /// Apply changes to the mesh. All channels set in this instance will be set in the combined mesh.
        /// </summary>	
        public virtual void Apply(MeshCombineHandlerBase.GenerateUV2Delegate uv2GenerationMethod = null)
        {
            meshCombiner.name = name + "-mesh";
            meshCombiner.Apply(uv2GenerationMethod);
        }

        /// <summary>	
        /// Applys the changes to flagged properties of the mesh. This method is slow, and should only be called once per frame. The speed is directly proportional to the number of flags that are true. Only apply necessary properties.	
        /// </summary>	
        public virtual void Apply(bool triangles,
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
                          MeshCombineHandlerBase.GenerateUV2Delegate uv2GenerationMethod = null)
        {
            meshCombiner.name = name + "-mesh";
            meshCombiner.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, blendShapesFlag, uv2GenerationMethod);
        }

        public virtual bool CombinedMeshContains(GameObject go)
        {
            return meshCombiner.CombinedMeshContains(go);
        }

        /// <summary>
        /// 更新已合并的游戏物体
        /// 网格变化，材质变化（材质需为合并材质中的其他材质）
        /// </summary>
        public virtual void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true,
                                            bool updateUV = false, bool updateUV1 = false, bool updateUV2 = false,
                                            bool updateColors = false, bool updateSkinningInfo = false)
        {
            meshCombiner.name = name + "-mesh";
            meshCombiner.UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV1, updateUV2, updateColors, updateSkinningInfo);
        }

        public virtual void UpdateSkinnedMeshApproximateBounds()
        {
            if (_ValidateForUpdateSkinnedMeshBounds())
            {
                meshCombiner.UpdateSkinnedMeshApproximateBounds();
            }
        }

        public virtual void UpdateSkinnedMeshApproximateBoundsFromBones()
        {
            if (_ValidateForUpdateSkinnedMeshBounds())
            {
                meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBones();
            }
        }

        public virtual void UpdateSkinnedMeshApproximateBoundsFromBounds()
        {
            if (_ValidateForUpdateSkinnedMeshBounds())
            {
                meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBounds();
            }
        }

        protected virtual bool _ValidateForUpdateSkinnedMeshBounds()
        {
            if (meshCombiner.outputOption == OutputOptions.bakeMeshAssetsInPlace)
            {
                Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
                return false;
            }
            if (meshCombiner.resultSceneObject == null)
            {
                Debug.LogWarning("Result Scene Object does not exist. No point in calling UpdateSkinnedMeshApproximateBounds.");
                return false;
            }
            SkinnedMeshRenderer smr = meshCombiner.resultSceneObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null)
            {
                Debug.LogWarning("No SkinnedMeshRenderer on result scene object.");
                return false;
            }
            return true;
        }
    }
}
