
using GameWorld;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Client.Scene
{
    public class TextureCombiner
    {
        public TextureCombiner()
        {
            combineData = new TextureCombinePipelineData();
        }

        #region API

        public static void CombineMaterial(List<GameObject> gameObjectToCombine, GameObjectCombineSetting setting, TextureCombinePipelineData data)
        {
            combineData = data;
            combineSetting = setting;
            CreateCombinedMaterialInfoAsset(gameObjectToCombine);
            CreateAtlases(gameObjectToCombine);
        }

        public static void CombineMaterial(List<GameObject> gameObjectToCombine, GameObjectCombineSetting setting)
        {
            combineData = CreateCombineData(setting);
            combineSetting = setting;
            CreateCombinedMaterialInfoAsset(gameObjectToCombine);
            CreateAtlases(gameObjectToCombine);
        }

        #endregion

        #region CombineConfigAndData

        private static GameObjectCombineSetting combineSetting;
        private static TextureCombinePipelineData combineData;

        private static TextureCombinePipelineData CreateCombineData(GameObjectCombineSetting setting)
        {
            var newCombineData = new TextureCombinePipelineData();
            return newCombineData;
        }

        #endregion

        #region TextureResult

        /// <summary>
        /// 创建合并材质数据资源
        /// </summary>
        public static void CreateCombinedMaterialInfoAsset(List<GameObject> GameObjectsToCombine)
        {
            string baseName = Path.GetFileNameWithoutExtension(combineData.CombinedMaterialInfoPath);
            if (baseName == null || baseName.Length == 0)
                return;
            string folderPath = combineData.CombinedMaterialInfoPath.Substring(0, combineData.CombinedMaterialInfoPath.Length - baseName.Length - 6);

            List<string> matNames = new List<string>();
            //多材质
            if (combineData.DoMultiMaterial)
            {
                for (int i = 0; i < combineData.ResultMaterials.Length; i++)
                {
                    matNames.Add(folderPath + baseName + "-mat" + i + ".mat");
                    AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), matNames[i]);
                    combineData.ResultMaterials[i].combinedMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[i], typeof(Material));
                }
            }
            else
            {
                matNames.Add(folderPath + baseName + "_mat.mat");
                Material newMat = null;
                if (GameObjectsToCombine.Count > 0 && GameObjectsToCombine[0] != null)
                {
                    Renderer r = GameObjectsToCombine[0].GetComponent<Renderer>();
                    if (r == null)
                    {
                        Debug.LogWarning("Object " + GameObjectsToCombine[0] + " does not have a Renderer on it.");
                    }
                    else
                    {
                        if (r.sharedMaterial != null)
                        {
                            newMat = new Material(r.sharedMaterial);
                            ConfigureNewMaterialToMatchOld(newMat, r.sharedMaterial);
                        }
                    }
                }

                if (newMat == null)
                {
                    newMat = new Material(Shader.Find("Diffuse"));
                }
                AssetDatabase.CreateAsset(newMat, matNames[0]);
                combineData.ResultMaterial = (Material)AssetDatabase.LoadAssetAtPath(matNames[0], typeof(Material));
            }
            //create the TextureBakeResults
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TextureBakeResults>(), combineData.CombinedMaterialInfoPath);
            combineData.textureBakeResults = (TextureBakeResults)AssetDatabase.LoadAssetAtPath(combineData.CombinedMaterialInfoPath, typeof(TextureBakeResults));
            AssetDatabase.Refresh();
        }

        #endregion

        #region TextureCombine
        /// <summary>
        /// 创建图集
        /// </summary>
        public static void CreateAtlases(List<GameObject> GameObjectsToCombine,
            bool saveAtlasesAsAssets = false)
        {
            try
            {
                combineData.ResultAtlasesAndRects = null;
                combineData.saveAtlasesAsAssets = saveAtlasesAsAssets;
                //--- 1、合并前检测
                if (!ValidateCombineGameObject(GameObjectsToCombine))
                {
                    return;
                }

                //材质验证
                if (combineData.DoMultiMaterial) 
                {
                    if(!_ValidateResultMaterials(GameObjectsToCombine, combineData.ResultMaterials))
                    {
                        return;
                    }
                }
                else
                {
                    if (!ValidateSingleResultMaterial(GameObjectsToCombine, combineData.ResultMaterial))
                    {
                        return;
                    }
                }


                ////--- 2、初始化存储合并结果的数据结构
                int numResults = 1;
                if (combineData.DoMultiMaterial)
                {
                    numResults = combineData.ResultMaterials.Length;
                }

                combineData.ResultAtlasesAndRects = new AtlasesAndRects[numResults];
                for (int i = 0; i < combineData.ResultAtlasesAndRects.Length; i++)
                {
                    combineData.ResultAtlasesAndRects[i] = new AtlasesAndRects();
                }

                //--- 3、开始合并材质（单个，多个）
                for (int i = 0; i < combineData.ResultAtlasesAndRects.Length; i++)
                {
                    Material resultMat;
                    List<Material> sourceMats;
                    if (combineData.DoMultiMaterial)
                    {
                        sourceMats = combineData.ResultMaterials[i].sourceMaterials;
                        resultMat = combineData.ResultMaterials[i].combinedMaterial;
                        combineData.fixOutOfBoundsUVs = combineData.ResultMaterials[i].considerMeshUVs;
                    }
                    else
                    {
                        resultMat = combineData.ResultMaterial;
                        sourceMats = null;  //为空则为全部合并
                    }

                    CombineTexturesIntoAtlases(combineData.ResultAtlasesAndRects[i],
                        resultMat,
                        GameObjectsToCombine,
                        sourceMats,
                        null);
                }

                //--- 4、TextureBakeResults 保存合并结果
                //unpackMat2RectMap(textureBakeResults);
                //textureBakeResults.doMultiMaterial = _doMultiMaterial;
                //if (_doMultiMaterial)
                //{
                //    textureBakeResults.resultMaterials = resultMaterials;
                //}
                //else
                //{
                //    MultiMaterial[] resMats = new MultiMaterial[1];
                //    resMats[0] = new MultiMaterial();
                //    resMats[0].combinedMaterial = _resultMaterial;
                //    resMats[0].considerMeshUVs = _fixOutOfBoundsUVs;
                //    resMats[0].sourceMaterials = new List<Material>();
                //    for (int i = 0; i < textureBakeResults.materialsAndUVRects.Length; i++)
                //    {
                //        resMats[0].sourceMaterials.Add(textureBakeResults.materialsAndUVRects[i].material);
                //    }
                //    textureBakeResults.resultMaterials = resMats;
                //}

                ////--- 5、传递合并结果到 MeshCombiner 
                //MeshBakerCommon[] mb = GetComponentsInChildren<MeshBakerCommon>();
                //for (int i = 0; i < mb.Length; i++)
                //{
                //    mb[i].textureBakeResults = textureBakeResults;
                //}
                //coroutineResult.isFinished = true;

                ////--- 6、合并材质结束回调
                //if (coroutineResult.success && onBuiltAtlasesSuccess != null)
                //{
                //    onBuiltAtlasesSuccess();
                //}
                //if (!coroutineResult.success && onBuiltAtlasesFail != null)
                //{
                //    onBuiltAtlasesFail();
                //}
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (saveAtlasesAsAssets)
                { //Atlases were saved to project so we don't need these ones
                    if (combineData.ResultAtlasesAndRects!= null)
                    {
                        for (int j = 0; j < combineData.ResultAtlasesAndRects.Length; j++)
                        {
                            AtlasesAndRects mAndA = combineData.ResultAtlasesAndRects[j];
                            if (mAndA != null && mAndA.atlases != null)
                            {
                                for (int i = 0; i < mAndA.atlases.Length; i++)
                                {
                                    if (mAndA.atlases[i] != null)
                                    {
                                        //if (editorMethods != null)
                                        //{
                                        //    editorMethods.Destroy(mAndA.atlases[i]);
                                        //}
                                        //else
                                        //{
                                            MeshBakerUtility.Destroy(mAndA.atlases[i]);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        static void CombineTexturesIntoAtlases(AtlasesAndRects resultAtlasesAndRects,
            Material resultMaterial,
            List<GameObject> objsToMesh,
            List<Material> allowedMaterialsFilter,
            EditorMethodsInterface textureEditorMethods,
            bool splitAtlasWhenPackingIfTooBig = false)
        {
            try
            {
                MaterialPropTexture.readyToBuildAtlases = false;

                // ---- 1.合并材质参数校验
                if (objsToMesh == null || objsToMesh.Count == 0)
                {
                    Debug.LogError("没有游戏物体参与合并");
                    return;
                }

                if (combineData.atlasPadding < 0)
                {
                    Debug.LogError("Atlas padding 必须大于等于零");
                    return;
                }

                if (combineData.maxTilingBakeSize < 2 || combineData.maxTilingBakeSize > 4096)
                {
                    Debug.LogError("无效Tilling尺寸的值Invalid value for max tiling bake size.");
                }

                for (int i = 0; i < objsToMesh.Count; i++)
                {
                    Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                    for (int j = 0; j < ms.Length; j++)
                    {
                        Material m = ms[j];
                        if (m == null)
                        {
                            Debug.LogError("游戏物体" + objsToMesh[i] + " 材质为空 ");
                        }

                    }
                }

                if (combineData.fixOutOfBoundsUVs && (combineData.packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal ||
                            combineData.packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical))
                {
                    Debug.LogWarning("合并算法为 MeshBakerTexturePacker_Horizontal 或 MeshBakerTexturePacker_Vertical，建议不打开 Consider Mesh UVs 选项");
                }

                // ---- 2.将材质的 shader 各参数信息写入管线数据中
                if (!TextureCombinePipeline.CollectPropertyNames(combineData))
                {
                    return;
                }

                // ---- 3.加载 Texture 混合器
                combineData.nonTexturePropertyBlender.LoadTextureBlendersIfNeeded(combineData.ResultMaterial);

                // ---- 4.合并管道
                BeginCombineTexturesIntoAtlases(resultAtlasesAndRects, combineData, splitAtlasWhenPackingIfTooBig, textureEditorMethods);
            }
            finally
            {
                // ---- 6.删除缓存，合并材质完成回调
                //_destroyAllTemporaryTextures();
            }
        }

        // texPropertyNames 是 resultMaterial中纹理属性的列表
        // allowedMaterialsFilter 是材料列表。 没有这些材料的物体将被忽略。这由多种材质过滤器使用
        // textureEditorMethods 仅封装编辑器功能，例如保存资产和跟踪纹理资产谁的格式已更改。 如果在运行时使用，则为null。
        static void BeginCombineTexturesIntoAtlases(AtlasesAndRects resultAtlasesAndRects,
                    TextureCombinePipelineData data,
                    bool splitAtlasWhenPackingIfTooBig,
                    EditorMethodsInterface textureEditorMethods)
        {

            // --- 1、记录各合并物体的源材质的 Prop Texture 信息写入 Data.distinctMaterialTextures
            //每个图集（主图，凹凸等）都将有的 MaterialTextures.Count 个图像。
            //每个 distinctMaterialTextures 对应一个游戏物体的某个材质，记录一组纹理，分别材质的在每个Prop图集一个。 
            List<GameObject> usedObjsToMesh = new List<GameObject>();
            TextureCombinePipeline.Step1_CollectDistinctMatTexturesAndUsedObjects(
                data, textureEditorMethods, usedObjsToMesh);

            // --- 2、计算使每个材质属性中的多个材质的合理尺寸
            TextureCombinePipeline.Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(
                data,textureEditorMethods);


            // --- 3、创建特定打包方式的打包器
            ITextureCombineAtlasPacker texturePaker = CreateAtlasPacker(
                data.IsOnlyOneTextureInAtlasReuseTextures(), 
                data.packingAlgorithm);
            texturePaker.ConvertTexturesToReadableFormats(data,textureEditorMethods);


            // --- 4、计算各源材质在合并材质 Atlas 的排布 
            AtlasPackingResult[] uvRects = texturePaker.CalculateAtlasRectangles(
                data, splitAtlasWhenPackingIfTooBig);

            // --- 5、创建 Atlas 并保存
            TextureCombinePipeline.Step3_BuildAndSaveAtlasesAndStoreResults(
                data,
                null,
                texturePaker,
                uvRects[0],
                textureEditorMethods,
                resultAtlasesAndRects);
        }



        #endregion

        #region 图集打包

        public static bool USE_EXPERIMENTAL_HOIZONTALVERTICAL = true;

        internal static ITextureCombinerPacker CreatePacker(bool onlyOneTextureInAtlasReuseTextures, 
            PackingAlgorithmEnum packingAlgorithm)
        {
            if (onlyOneTextureInAtlasReuseTextures)
            {
                return new TextureCombinerPackerOneTextureInAtlas();
            }
            else if (packingAlgorithm == PackingAlgorithmEnum.UnitysPackTextures)
            {
                return new MB3_TextureCombinerPackerUnity();
            }
            else if (packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
            {
                if (USE_EXPERIMENTAL_HOIZONTALVERTICAL)
                {
                    return new TextureCombinerPackerMeshBakerHorizontalVertical(TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.horizontal);
                }
                else
                {
                    return new TextureCombinerPackerMeshBaker();
                }

            }
            else if (packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
            {
                if (USE_EXPERIMENTAL_HOIZONTALVERTICAL)
                {
                    return new TextureCombinerPackerMeshBakerHorizontalVertical(TextureCombinerPackerMeshBakerHorizontalVertical.AtlasDirection.vertical);
                }
                else
                {
                    return new TextureCombinerPackerMeshBaker();
                }
            }
            else if (packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker)
            {
                return new TextureCombinerPackerMeshBaker();
            }
            else
            {
                return new TextureCombinerPackerMeshBakerFast();
            }
        }

        internal static ITextureCombineAtlasPacker CreateAtlasPacker(bool onlyOneTextureInAtlasReuseTextures,
            PackingAlgorithmEnum packingAlgorithm)
        {
                return new TextureCombineAtlasPacker();
        }

        #endregion

        #region Validate Method

        /// <summary>
        /// 合并游戏物体列表,空物体，重复物体，空材质，网格验证
        /// </summary>
        /// <param name="gameObjectsToMesh"></param>
        /// <returns></returns>
        private static bool ValidateCombineGameObject(List<GameObject> gameObjectsToMesh)
        {
            //网格分析结果缓存
            Dictionary<int, MeshAnalysisResult> meshAnalysisResultCache = 
                new Dictionary<int, MeshAnalysisResult>(); ;
            //获取将合并网格的游戏物体
            List<GameObject> objsToMesh = gameObjectsToMesh;

            for (int i = 0; i < objsToMesh.Count; i++)
            {
                GameObject go = objsToMesh[i];
                if (go == null)
                {
                    Debug.LogError("合并网格游戏物体列表中包含 null 物体，在位置" + i);
                    return false;
                }
                for (int j = i + 1; j < objsToMesh.Count; j++)
                {
                    if (objsToMesh[i] == objsToMesh[j])
                    {
                        Debug.LogError("合并网格游戏物体列表中包含重复游戏物体 " + i + " 和 " + j);
                        return false;
                    }
                }
                if (MeshBakerUtility.GetGOMaterials(go).Length == 0)
                {
                    Debug.LogError("游戏物体 " + go + " 没有材质");
                    return false;
                }
                Mesh m = MeshBakerUtility.GetMesh(go);
                if (m == null)
                {
                    Debug.LogError("合并网格游戏物体列表中， " + go + " 没有网格 ");
                    return false;
                }
                if (m != null)
                {
                    //This check can be very expensive and it only warns so only do this if we are in the editor.
                    if (!Application.isEditor && Application.isPlaying && combineData.DoMultiMaterial)
                    {
                        MeshAnalysisResult mar;
                        if (!meshAnalysisResultCache.TryGetValue(m.GetInstanceID(), out mar))
                        {
                            MeshBakerUtility.doSubmeshesShareVertsOrTris(m, ref mar);
                            meshAnalysisResultCache.Add(m.GetInstanceID(), mar);
                        }
                        //检查重叠的子网格顶点
                        if (mar.hasOverlappingSubmeshVerts)
                        {
                            Debug.LogWarning("游戏物体 " + objsToMesh[i] + " has overlapping submeshes (submeshes share vertices)." +
                                "If the UVs associated with the shared vertices are important then this bake may not work. " +
                                "If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures " +
                                "(each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, " +
                                "available in the asset store can fix overlapping submeshes.");
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 多材质验证
        /// </summary>
        /// <returns></returns>
        static bool _ValidateResultMaterials(List<GameObject> objsToMesh, MultiMaterial[] resultMaterials)
        {
            HashSet<Material> allMatsOnObjs = new HashSet<Material>();
            for (int i = 0; i < objsToMesh.Count; i++)
            {
                if (objsToMesh[i] != null)
                {
                    Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                    for (int j = 0; j < ms.Length; j++)
                    {
                        if (ms[j] != null)
                            allMatsOnObjs.Add(ms[j]);
                    }
                }
            }

            //多材质判断
            HashSet<Material> allMatsInMapping = new HashSet<Material>();
            for (int i = 0; i < resultMaterials.Length; i++)
            {
                //查重
                for (int j = i + 1; j < resultMaterials.Length; j++)
                {
                    if (resultMaterials[i].combinedMaterial == resultMaterials[j].combinedMaterial)
                    {
                        Debug.LogError(string.Format("Source To Combined Mapping: Submesh {0} and Submesh {1} use the same combined material. These should be different", i, j));
                        return false;
                    }
                }

                //判空
                MultiMaterial mm = resultMaterials[i];
                if (mm.combinedMaterial == null)
                {
                    Debug.LogError("Combined Material is null please create and assign a result material.");
                    return false;
                }
                Shader targShader = mm.combinedMaterial.shader;
                for (int j = 0; j < mm.sourceMaterials.Count; j++)
                {
                    if (mm.sourceMaterials[j] == null)
                    {
                        Debug.LogError("There are null entries in the list of Source Materials");
                        return false;
                    }
                    if (targShader != mm.sourceMaterials[j].shader)
                    {
                        Debug.LogWarning("Source material " + mm.sourceMaterials[j] + " does not use shader " + targShader + " it may not have the required textures. If not empty textures will be generated.");
                    }
                    if (allMatsInMapping.Contains(mm.sourceMaterials[j]))
                    {
                        Debug.LogError("A Material " + mm.sourceMaterials[j] + " appears more than once in the list of source materials in the source material to combined mapping. Each source material must be unique.");
                        return false;
                    }
                    allMatsInMapping.Add(mm.sourceMaterials[j]);
                }
            }

            if (allMatsOnObjs.IsProperSubsetOf(allMatsInMapping))
            {
                allMatsInMapping.ExceptWith(allMatsOnObjs);
                ////Debug.LogWarning("There are materials in the mapping that are not used on your source objects: " + PrintSet(allMatsInMapping));
            }
            if (resultMaterials != null && resultMaterials.Length > 0 && allMatsInMapping.IsProperSubsetOf(allMatsOnObjs))
            {
                allMatsOnObjs.ExceptWith(allMatsInMapping);
                ////Debug.LogError("There are materials on the objects to combine that are not in the mapping: " + PrintSet(allMatsOnObjs));
                return false;
            }
            return true;
        }

        static bool ValidateSingleResultMaterial(List<GameObject> objsToMesh, Material singleResultMaterial)
        {
            //合并为单独材质
            if (singleResultMaterial == null)
            {
                Debug.LogError("Combined Material is null please create and assign a result material.");
                return false ;
            }
            Shader targShader = singleResultMaterial.shader;
            for (int i = 0; i < objsToMesh.Count; i++)
            {
                Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                for (int j = 0; j < ms.Length; j++)
                {
                    Material m = ms[j];
                    if (m != null && m.shader != targShader)
                    {
                        Debug.LogWarning("游戏物体" + objsToMesh[i] + " 没有使用 shader " + targShader +
                            " it may not have the required textures. " +
                            "If not small solid color textures will be generated.");
                    }
                }
            }
            return true;
        }

        #endregion

        #region Utility Method



        public static void ConfigureNewMaterialToMatchOld(Material newMat, Material original)
        {
            if (original == null)
            {
                Debug.LogWarning("Original material is null, could not copy properties to " + newMat + ". Setting shader to " + newMat.shader);
                return;
            }
            newMat.shader = original.shader;
            newMat.CopyPropertiesFromMaterial(original);
            ShaderTextureProperty[] texPropertyNames = TextureCombinerPipeline.shaderTexPropertyNames;
            for (int j = 0; j < texPropertyNames.Length; j++)
            {
                Vector2 scale = Vector2.one;
                Vector2 offset = Vector2.zero;
                if (newMat.HasProperty(texPropertyNames[j].name))
                {
                    newMat.SetTextureOffset(texPropertyNames[j].name, offset);
                    newMat.SetTextureScale(texPropertyNames[j].name, scale);
                }
            }
        }


        

        

        internal static float GetSubmeshArea(Mesh m, int submeshIdx)
        {
            if (submeshIdx >= m.subMeshCount || submeshIdx < 0)
            {
                return 0f;
            }
            Vector3[] vs = m.vertices;
            int[] tris = m.GetIndices(submeshIdx);
            float area = 0f;
            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 v0 = vs[tris[i]];
                Vector3 v1 = vs[tris[i + 1]];
                Vector3 v2 = vs[tris[i + 2]];
                Vector3 cross = Vector3.Cross(v1 - v0, v2 - v0);
                area += cross.magnitude / 2f;
            }
            return area;
        }


        /// <summary>
        /// 获取材质属性 Texture 
        /// Some shaders like the Standard shader have texture properties like Emission which can be set on the material
        /// but are disabled using keywords. In these cases the textures should not be returned.
        /// 有些着色器（例如“标准”着色器）具有可以在材质上设置的纹理属性（例如“发射”）
        /// 但使用关键字禁用。 在这些情况下，不应返回纹理。
        /// </summary>
        public static Texture GetTextureConsideringStandardShaderKeywords(string shaderName, Material mat, string propertyName)
        {
            if (shaderName.Equals("Standard") || shaderName.Equals("Standard (Specular setup)") || shaderName.Equals("Standard (Roughness setup"))
            {
                if (propertyName.Equals("_EmissionMap"))
                {
                    if (mat.IsKeywordEnabled("_EMISSION"))
                    {
                        return mat.GetTexture(propertyName);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return mat.GetTexture(propertyName);
        }

         /// <summary>
        /// Returns the tiling scale and offset for a given material.
        /// 
        /// The only reason that this method is necessary is the Standard shader. Each texture in a material has a scale and offset stored with it.
        /// Most shaders use the scale and offset accociated with each texture map. The Standard shader does not do this. It uses the scale and offset
        /// associated with _MainTex for most of the maps.
        /// </summary>
        internal static void GetMaterialScaleAndOffset(Material mat, string propertyName, out Vector2 offset, out Vector2 scale)
        {
            if (mat == null)
            {
                Debug.LogError("Material was null. Should never happen.");
                offset = Vector2.zero;
                scale = Vector2.one;
            }

            if ((mat.shader.name.Equals("Standard") || mat.shader.name.Equals("Standard (Specular setup)")) && mat.HasProperty("_MainTex"))
            {
                offset = mat.GetTextureOffset("_MainTex");
                scale = mat.GetTextureScale("_MainTex");
            } else 
            {
                offset = mat.GetTextureOffset(propertyName);
                scale = mat.GetTextureScale(propertyName);
            }
        }
        #endregion
    }


}