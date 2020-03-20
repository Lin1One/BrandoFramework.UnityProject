using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// Texture合并管线
    /// </summary>
    public class TextureCombinerPipeline
    {
        public static bool USE_EXPERIMENTAL_HOIZONTALVERTICAL = true;
        
        /// <summary>
        /// 材质属性贴图图集设置
        /// </summary>
        public struct CreateAtlasForProperty
        {
            public bool allTexturesAreNull;
            public bool allTexturesAreSame;
            public bool allNonTexturePropsAreSame;

            public override string ToString()
            {
                return string.Format("AllTexturesNull={0} areSame={1} nonTexPropsAreSame={2}", 
                    allTexturesAreNull, allTexturesAreSame, allNonTexturePropsAreSame);
            }
        }

        //shader 材质参数
        public static ShaderTextureProperty[] shaderTexPropertyNames = new ShaderTextureProperty[] 
        {
            new ShaderTextureProperty("_MainTex",false),
            new ShaderTextureProperty("_BumpMap",true),
            new ShaderTextureProperty("_Normal",true),
            new ShaderTextureProperty("_BumpSpecMap",false),
            new ShaderTextureProperty("_DecalTex",false),
            new ShaderTextureProperty("_Detail",false),
            new ShaderTextureProperty("_GlossMap",false),
            new ShaderTextureProperty("_Illum",false),
            new ShaderTextureProperty("_LightTextureB0",false),
            new ShaderTextureProperty("_ParallaxMap",false),
            new ShaderTextureProperty("_ShadowOffset",false),
            new ShaderTextureProperty("_TranslucencyMap",false),
            new ShaderTextureProperty("_SpecMap",false),
            new ShaderTextureProperty("_SpecGlossMap",false),
            new ShaderTextureProperty("_TranspMap",false),
            new ShaderTextureProperty("_MetallicGlossMap",false),
            new ShaderTextureProperty("_OcclusionMap",false),
            new ShaderTextureProperty("_EmissionMap",false),
            new ShaderTextureProperty("_DetailMask",false), 
		};

        /// <summary>
        /// 检查是否需为属性创建纹理图集
        /// </summary>
        /// <param name="propertyIndex"></param>
        /// <param name="considerNonTextureProperties"></param>
        /// <param name="allTexturesAreNullAndSameColor"></param>
        /// <returns></returns>
        internal static bool _ShouldWeCreateAtlasForThisProperty(int propertyIndex, 
            bool considerNonTextureProperties, 
            CreateAtlasForProperty[] allTexturesAreNullAndSameColor)
        {
            CreateAtlasForProperty v = allTexturesAreNullAndSameColor[propertyIndex];
            if (considerNonTextureProperties)
            {
                if (!v.allNonTexturePropsAreSame || !v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 收集材质参数名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool _CollectPropertyNames(TexturePipelineData data)
        {
            //try custom properties remove duplicates
            //尝试删除自定义属性的重复项
            for (int i = 0; i < data.texPropertyNames.Count; i++)
            {
                ShaderTextureProperty s = data._customShaderPropNames.Find(x => x.name.Equals(data.texPropertyNames[i].name));
                if (s != null)
                {
                    data._customShaderPropNames.Remove(s);
                }
            }

            Material m = data.resultMaterial;
            if (m == null)
            {
                Debug.LogError("合并材质为空。");
                return false;
            }

            //Collect the property names for the textures
            for (int i = 0; i < shaderTexPropertyNames.Length; i++)
            {
                if (m.HasProperty(shaderTexPropertyNames[i].name))
                {
                    if (!data.texPropertyNames.Contains(shaderTexPropertyNames[i]))
                    {
                        data.texPropertyNames.Add(shaderTexPropertyNames[i]);
                    }
                    if (m.GetTextureOffset(shaderTexPropertyNames[i].name) != new Vector2(0f, 0f))
                    {
                        Debug.LogWarning("Result material has non-zero offset. This is may be incorrect.");
                    }
                    if (m.GetTextureScale(shaderTexPropertyNames[i].name) != new Vector2(1f, 1f))
                    {
                        Debug.LogWarning("Result material should have tiling of 1,1");
                    }
                }
            }

            for (int i = 0; i < data._customShaderPropNames.Count; i++)
            {
                if (m.HasProperty(data._customShaderPropNames[i].name))
                {
                    data.texPropertyNames.Add(data._customShaderPropNames[i]);
                    if (m.GetTextureOffset(data._customShaderPropNames[i].name) != new Vector2(0f, 0f))
                    {
                        Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
                    }
                    if (m.GetTextureScale(data._customShaderPropNames[i].name) != new Vector2(1f, 1f))
                    {
                        Debug.LogWarning("Result material should probably have tiling of 1,1.");
                    }
                }
                else
                {
                    Debug.LogWarning("Result material shader does not use property " + 
                        data._customShaderPropNames[i].name + " in the list of custom shader property names");
                }
            }
            return true;
        }

        /// <summary>
        /// 判断是否需要为此材质参数创建纹理Atals
        /// </summary>
        /// <param name="propertyIndex"></param>
        /// <param name="allTexturesAreNullAndSameColor"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool _ShouldWeCreateAtlasForThisProperty(int propertyIndex, 
            CreateAtlasForProperty[] allTexturesAreNullAndSameColor, 
            TexturePipelineData data)
        {
            CreateAtlasForProperty v = allTexturesAreNullAndSameColor[propertyIndex];
            if (data._considerNonTextureProperties)
            {
                if (!v.allNonTexturePropsAreSame || !v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!v.allTexturesAreNull)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
                    } else
                    {
                        return null;
                    }
                }
            }
            return mat.GetTexture(propertyName);
        }

        /// <summary>
        /// 第一步：
        ///     写入 TexturePipelineData 的 MaterialPropTexturesSet 列表，和 usedObjsToMesh 列表
        /// 每个TexSet在 Atlas 中都是一个矩形。
        /// 如果 allowedMaterialsFilter （过滤器）为空，那么将收集 allObjsToMesh 上的所有材质，usedObjsToMesh 将与allObjsToMesh相同
        /// 否则，将仅包括allowedMaterialsFilter中的材料，而usedObjsToMesh将是使用这些材料的objs。
        /// </summary>
        internal static IEnumerator __Step1_CollectDistinctMatTexturesAndUsedObjects(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            EditorMethodsInterface textureEditorMethods,
            List<GameObject> usedObjsToMesh)
        {
            // Collect distinct list of textures to combine from the materials on objsToCombine
            // 收集UsedObjects上不同的材质纹理
            bool outOfBoundsUVs = false;
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MeshAnalysisResult[]>(); //cache results
            for (int i = 0; i < data.allObjsToMesh.Count; i++)
            {
                GameObject obj = data.allObjsToMesh[i];
                //报道进度
                if (progressInfo != null)
                {
                    progressInfo("Collecting textures for " + obj, ((float)i) / data.allObjsToMesh.Count / 2f);
                }

                if (obj == null)
                {
                    Debug.LogError("合并游戏物体列表中包含空物体");
                    result.success = false;
                    yield break;
                }

                Mesh sharedMesh = MeshBakerUtility.GetMesh(obj);
                if (sharedMesh == null)
                {
                    Debug.LogError("游戏物体 " + obj.name + " 网格为空");
                    result.success = false;
                    yield break;
                }

                Material[] sharedMaterials = MeshBakerUtility.GetGOMaterials(obj);
                if (sharedMaterials.Length == 0)
                {
                    Debug.LogError("游戏物体 " + obj.name + " 材质为空.");
                    result.success = false;
                    yield break;
                }

                //analyze mesh or grab cached result of previous analysis, stores one result for each submesh
                //处理网格数据
                MeshAnalysisResult[] meshAnalysisResults;//每个游戏物体的主网格子网格数据数组
                if (!meshAnalysisResultsCache.TryGetValue(sharedMesh.GetInstanceID(), out meshAnalysisResults))
                {
                    meshAnalysisResults = new MeshAnalysisResult[sharedMesh.subMeshCount];
                    for (int j = 0; j < sharedMesh.subMeshCount; j++)
                    {
                        MeshBakerUtility.hasOutOfBoundsUVs(sharedMesh, ref meshAnalysisResults[j], j);
                        if (data._normalizeTexelDensity)
                        {
                            meshAnalysisResults[j].submeshArea = GetSubmeshArea(sharedMesh, j);
                        }

                        if (data._fixOutOfBoundsUVs && !meshAnalysisResults[j].hasUVs)
                        {
                            meshAnalysisResults[j].uvRect = new Rect(0, 0, 1, 1);
                            Debug.LogWarning("Mesh for object " + obj + " has no UV channel but 'consider UVs' is enabled." +
                                " Assuming UVs will be generated filling 0,0,1,1 rectangle.");
                        }
                    }
                    meshAnalysisResultsCache.Add(sharedMesh.GetInstanceID(), meshAnalysisResults);
                }

                if (data._fixOutOfBoundsUVs)
                {
                    Debug.Log("Mesh Analysis for object " + obj + 
                        " numSubmesh=" + meshAnalysisResults.Length + 
                        " HasOBUV=" + meshAnalysisResults[0].hasOutOfBoundsUVs + 
                        " UVrectSubmesh0=" + meshAnalysisResults[0].uvRect);
                }


                //处理材质数据
                for (int matIdx = 0; matIdx < sharedMaterials.Length; matIdx++)
                { 
                    ////for each submesh
                    //if (progressInfo != null)
                    //{
                    //    progressInfo(string.Format("Collecting textures for {0} submesh {1}", obj, matIdx), 
                    //        ((float)i) / data.allObjsToMesh.Count / 2f);
                    //}
                    Material mat = sharedMaterials[matIdx];

                    // 材质过滤器
                    if (data.allowedMaterialsFilter != null && !data.allowedMaterialsFilter.Contains(mat))
                    {
                        continue;
                    }

                    outOfBoundsUVs = outOfBoundsUVs || meshAnalysisResults[matIdx].hasOutOfBoundsUVs;

                    if (mat.name.Contains("(Instance)"))
                    {
                        Debug.LogError("The sharedMaterial on object " + obj.name + " has been 'Instanced'." +
                                        " This was probably caused by a script accessing the meshRender.material property in the editor. " +
                                       " The material to UV Rectangle mapping will be incorrect. " +
                                       "To fix this recreate the object from its prefab or re-assign its material from the correct asset.");
                        result.success = false;
                        yield break;
                    }

                    if (data._fixOutOfBoundsUVs)
                    {
                        if (!MeshBakerUtility.AreAllSharedMaterialsDistinct(sharedMaterials))
                        {
                            Debug.LogWarning("游戏物体 " + obj.name + " 使用相同的材质在多个子网格. " +
                                "可能生成奇怪的 resultAtlasesAndRects，尤其是与 _fixOutOfBoundsUVs 为 true 时");
                        }
                    }

                    //材质属性所用到的 Texutre 
                    MaterialPropTexture[] mts = new MaterialPropTexture[data.texPropertyNames.Count];
                    for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
                    {
                        Texture tx = null;
                        Vector2 scale = Vector2.one;
                        Vector2 offset = Vector2.zero;
                        float texelDensity = 0f;
                        if (mat.HasProperty(data.texPropertyNames[propIdx].name))
                        {
                            Texture txx = GetTextureConsideringStandardShaderKeywords(data.resultMaterial.shader.name, mat, data.texPropertyNames[propIdx].name);
                            if (txx != null)
                            {
                                if (txx is Texture2D)
                                {
                                    //TextureFormat 验证
                                    tx = txx;
                                    TextureFormat f = ((Texture2D)tx).format;
                                    bool isNormalMap = false;
                                    if (!Application.isPlaying && textureEditorMethods != null)
                                        isNormalMap = textureEditorMethods.IsNormalMap((Texture2D)tx);
                                    if ((f == TextureFormat.ARGB32 ||
                                        f == TextureFormat.RGBA32 ||
                                        f == TextureFormat.BGRA32 ||
                                        f == TextureFormat.RGB24 ||
                                        f == TextureFormat.Alpha8) && !isNormalMap) //DXT5 does not work
                                    {
                                        //可使用
                                    }
                                    else
                                    {
                                        //TRIED to copy texture using tex2.SetPixels(tex1.GetPixels()) but bug in 3.5 means DTX1 and 5 compressed textures come out skewe
                                        //尝试使用tex2.SetPixels（tex1.GetPixels（））复制纹理，但是3.5中的bug意味着DTX1和5压缩纹理出现扭曲
                                        if (Application.isPlaying && data._packingAlgorithm != PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
                                        {
                                            Debug.LogWarning("合并列表中，游戏物体 " + obj.name + " 所使用的 Texture " + 
                                                tx.name + " 使用的格式 " + f + 
                                                "不是: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 或 DXT. " +
                                                "无法在运行时重新设置尺寸" +
                                                "If format says 'compressed' try changing it to 'truecolor'");
                                            result.success = false;
                                            yield break;
                                        }
                                        else
                                        {
                                            tx = (Texture2D)mat.GetTexture(data.texPropertyNames[propIdx].name);
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogError("合并列表中，游戏物体 " + obj.name + " 渲染网格使用的 Texture 不是 Texture2D. ");
                                    result.success = false;
                                    yield break;
                                }
                            }
                            //像素密度
                            if (tx != null && data._normalizeTexelDensity)
                            {
                                //不考虑平铺和UV采样超出范围
                                if (meshAnalysisResults[propIdx].submeshArea == 0)
                                {
                                    texelDensity = 0f;
                                }
                                else
                                {
                                    texelDensity = (tx.width * tx.height) / (meshAnalysisResults[propIdx].submeshArea);
                                }
                            }
                            //规格，偏移
                            GetMaterialScaleAndOffset(mat, data.texPropertyNames[propIdx].name, out offset, out scale);
                        }

                        mts[propIdx] = new MaterialPropTexture(tx, offset, scale, texelDensity);
                    }

                    // 收集材质参数值的平均值
                    data.nonTexturePropertyBlender.CollectAverageValuesOfNonTextureProperties(data.resultMaterial, mat);

                    Vector2 obUVscale = new Vector2(meshAnalysisResults[matIdx].uvRect.width, meshAnalysisResults[matIdx].uvRect.height);
                    Vector2 obUVoffset = new Vector2(meshAnalysisResults[matIdx].uvRect.x, meshAnalysisResults[matIdx].uvRect.y);

                    //Add to distinct set of textures if not already there
                    TextureTilingTreatment tilingTreatment = TextureTilingTreatment.none;
                    if (data._fixOutOfBoundsUVs)
                    {
                        tilingTreatment = TextureTilingTreatment.considerUVs;
                    }

                    //合并信息 distinctMaterialTextures 数据设置

                    //材质各参数 Texture，及 UV 偏移数据映射 
                    MaterialPropTexturesSet setOfTexs = new MaterialPropTexturesSet(mts, obUVoffset, obUVscale, tilingTreatment);  //one of these per submesh
                    //材质及各变化参数Rect 数据
                    MatAndTransformToMerged matt = new MatAndTransformToMerged(new DRect(obUVoffset, obUVscale), data._fixOutOfBoundsUVs, mat);

                    setOfTexs.matsAndGOs.mats.Add(matt);

                    MaterialPropTexturesSet setOfTexs2 = data.distinctMaterialTextures.Find(x => x.IsEqual(setOfTexs, data._fixOutOfBoundsUVs, data.nonTexturePropertyBlender));
                    if (setOfTexs2 != null)
                    {
                        setOfTexs = setOfTexs2;
                    }
                    else
                    {
                        data.distinctMaterialTextures.Add(setOfTexs);
                    }

                    if (!setOfTexs.matsAndGOs.mats.Contains(matt))
                    {
                        setOfTexs.matsAndGOs.mats.Add(matt);
                    }

                    if (!setOfTexs.matsAndGOs.gos.Contains(obj))
                    {
                        setOfTexs.matsAndGOs.gos.Add(obj);
                        //已使用 游戏物体
                        if (!usedObjsToMesh.Contains(obj))
                            usedObjsToMesh.Add(obj);
                    }
                }
            }

            Debug.Log(string.Format("第一阶段完成;" +
                "参与合并的游戏物体的不同材质，各自包含与shader属性对应的不同的纹理，收集到 {0} 组 textures，即 {0} 个不同的材质，" +
                "fixOutOfBoundsUV:{1} " +
                "considerNonTextureProperties:{2}", 
                data.distinctMaterialTextures.Count, data._fixOutOfBoundsUVs, data._considerNonTextureProperties));

            if (data.distinctMaterialTextures.Count == 0)
            {
                Debug.LogError("None of the source object materials matched any of the allowed materials for submesh with result material: " + data.resultMaterial);
                result.success = false;
                yield break;
            }

            TextureCombinerMerging merger = new TextureCombinerMerging(data._considerNonTextureProperties,
                data.nonTexturePropertyBlender, data._fixOutOfBoundsUVs);
            merger.MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(data.distinctMaterialTextures);

            yield break;
        }
	
        //第二步：计算每个材质的属性对应的 Textures 统一尺寸
        //每种材质（_mainTex，凹凸，Spec ect ...）中的纹理必须大小相同
        //计算要使用的最佳尺寸。 考虑平铺
        //如果图集中只有一种纹理会使用原始大小
        internal static IEnumerator _Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            EditorMethodsInterface textureEditorMethods)
        {
            MaterialPropTexture.readyToBuildAtlases = true;
            data.allTexturesAreNullAndSameColor = CalculateAllTexturesAreNullAndSameColor(data);

            //calculate size of rectangles in atlas
            //计算 atlas 矩形尺寸
            int _padding = data._atlasPadding;
            if (data.distinctMaterialTextures.Count == 1 && data._fixOutOfBoundsUVs == false && data._considerNonTextureProperties == false)
            {
                Debug.Log("所有游戏物体使用相同的材质.将使用 Original textures .");
                _padding = 0;
                data.distinctMaterialTextures[0].SetThisIsOnlyTexSetInAtlasTrue();
                data.distinctMaterialTextures[0].SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(TextureTilingTreatment.edgeToEdgeXY);
            }

            Debug.Assert(data.allTexturesAreNullAndSameColor.Length == data.texPropertyNames.Count,
                "allTexturesAreNullAndSameColor array must be the same length of texPropertyNames.");

            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                Debug.Log("为 TexSet " + i + " of " + data.distinctMaterialTextures.Count + "计算合适的尺寸");
                MaterialPropTexturesSet txs = data.distinctMaterialTextures[i];
                txs.idealWidth = 1;
                txs.idealHeight = 1;
                int tWidth = 1;
                int tHeight = 1;
                Debug.Assert(txs.ts.Length == data.texPropertyNames.Count, 
                    "length of arrays in each element of distinctMaterialTextures must be texPropertyNames.Count");

                //在 MaterialPropTexturesSet 中所有 MaterialPropTextures 应为相同尺寸
                for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
                {
                    if (_ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                    {
                        MaterialPropTexture matTex = txs.ts[propIdx];
                        Debug.Log(string.Format("为 texSet {0} ，property {1} 计算合适尺寸", i, data.texPropertyNames[propIdx].name));
                        if (!matTex.matTilingRect.size.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1)
                        {
                            Debug.LogWarning("Texture " + matTex.GetTexName() + "is tiled by " + 
                                matTex.matTilingRect.size + " tiling will be baked into a texture with maxSize:" + 
                                data._maxTilingBakeSize);
                        }

                        if (!txs.obUVscale.Equals(Vector2.one) && data.distinctMaterialTextures.Count > 1 && data._fixOutOfBoundsUVs)
                        {
                            Debug.LogWarning("Texture " + matTex.GetTexName() +
                                " has out of bounds UVs that effectively tile by " + txs.obUVscale + 
                                " tiling will be baked into a texture with maxSize:" + data._maxTilingBakeSize);
                        }

                        if (matTex.isNull)
                        {
                            txs.SetEncapsulatingRect(propIdx, data._fixOutOfBoundsUVs);
                            Debug.Log(string.Format("No source texture creating a 16x16 texture for {0} texSet {1} srcMat {2}",
                                data.texPropertyNames[propIdx].name, i, txs.matsAndGOs.mats[0].GetMaterialName()));
                        }

                        if (!matTex.isNull)
                        {
                            Vector2 dim = GetAdjustedForScaleAndOffset2Dimensions(matTex, txs.obUVoffset, txs.obUVscale, data);
                            if ((int)(dim.x * dim.y) > tWidth * tHeight)
                            {
                                Debug.Log(" 材质texture " + matTex.GetTexName() + " " + dim + " 需要比" + tWidth + " " + tHeight + "更大的尺寸");
                                tWidth = (int)dim.x;
                                tHeight = (int)dim.y;
                            }
                        }
                    }
                }

                if (data._resizePowerOfTwoTextures)
                {
                    if (tWidth <= _padding * 5)
                    {
                        Debug.LogWarning(String.Format("Some of the textures have widths close to the size of the padding. " +
                            "It is not recommended to use _resizePowerOfTwoTextures with widths this small.", txs.ToString()));
                    }
                    if (tHeight <= _padding * 5)
                    {
                        Debug.LogWarning(String.Format("Some of the textures have heights close to the size of the padding. " +
                            "It is not recommended to use _resizePowerOfTwoTextures with heights this small.", txs.ToString()));
                    }
                    if (IsPowerOfTwo(tWidth))
                    {
                        tWidth -= _padding * 2;
                    }
                    if (IsPowerOfTwo(tHeight))
                    {
                        tHeight -= _padding * 2;
                    }
                    if (tWidth < 1) tWidth = 1;
                    if (tHeight < 1) tHeight = 1;
                }
                Debug.Log("Ideal size is " + tWidth + " " + tHeight);
                txs.idealWidth = tWidth;
                txs.idealHeight = tHeight;
            }
            data._atlasPadding = _padding;
            yield break;
        }

        /// <summary>
        ///  Texture 合并管线第 3 步，创建 Atlas 并保存资源
        /// </summary>
        /// <returns></returns>
        internal static IEnumerator __Step3_BuildAndSaveAtlasesAndStoreResults(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            ITextureCombinerPacker packer,
            AtlasPackingResult atlasPackingResult,
            EditorMethodsInterface textureEditorMethods,
            AtlasesAndRects resultAtlasesAndRects)
        {
            //run the garbage collector to free up as much memory as possible before bake to reduce MissingReferenceException problems
            GC.Collect();
            Texture2D[] atlases = new Texture2D[data.numAtlases];
            //StringBuilder report = GenerateReport(data);

            //创建图集
            yield return packer.CreateAtlases(progressInfo, data, combiner, atlasPackingResult, atlases, textureEditorMethods);

            data.nonTexturePropertyBlender.AdjustNonTextureProperties(data.resultMaterial, data.texPropertyNames, data.distinctMaterialTextures, textureEditorMethods);

            if (data.distinctMaterialTextures.Count > 0)
                data.distinctMaterialTextures[0].AdjustResultMaterialNonTextureProperties(data.resultMaterial, data.texPropertyNames);

            //结果报告
            //if (progressInfo != null)
            //    progressInfo("Building Report", .7f);
            ////report on atlases created
            //StringBuilder atlasMessage = new StringBuilder();
            //atlasMessage.AppendLine("---- Atlases ------");
            //for (int i = 0; i < data.numAtlases; i++)
            //{
            //    if (atlases[i] != null)
            //    {
            //        atlasMessage.AppendLine("Created Atlas For: " + data.texPropertyNames[i].name + " h=" + atlases[i].height + " w=" + atlases[i].width);
            //    }
            //    else if (!_ShouldWeCreateAtlasForThisProperty(i, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
            //    {
            //        atlasMessage.AppendLine("Did not create atlas for " + data.texPropertyNames[i].name + " because all source textures were null.");
            //    }
            //}
            //report.Append(atlasMessage.ToString());

            List<MaterialAndUVRect> mat2rect_map = new List<MaterialAndUVRect>();
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                MaterialPropTexturesSet texSet = data.distinctMaterialTextures[i];
                List<MatAndTransformToMerged> mats = texSet.matsAndGOs.mats;
                Rect allPropsUseSameTiling_encapsulatingSamplingRect;
                Rect propsUseDifferntTiling_obUVRect;
                texSet.GetRectsForTextureBakeResults(out allPropsUseSameTiling_encapsulatingSamplingRect, out propsUseDifferntTiling_obUVRect);
                for (int j = 0; j < mats.Count; j++)
                {
                    Rect allPropsUseSameTiling_sourceMaterialTiling = texSet.GetMaterialTilingRectForTextureBakerResults(j);
                    MaterialAndUVRect key = new MaterialAndUVRect(
                        mats[j].mat,
                        atlasPackingResult.rects[i],
                        texSet.allTexturesUseSameMatTiling,
                        allPropsUseSameTiling_sourceMaterialTiling,
                        allPropsUseSameTiling_encapsulatingSamplingRect,
                        propsUseDifferntTiling_obUVRect,
                        texSet.tilingTreatment,
                        mats[j].objName);
                    if (!mat2rect_map.Contains(key))
                    {
                        mat2rect_map.Add(key);
                    }
                }
            }

            resultAtlasesAndRects.atlases = atlases;                             // one per texture on result shader
            resultAtlasesAndRects.texPropertyNames = ShaderTextureProperty.GetNames(data.texPropertyNames); // one per texture on source shader
            resultAtlasesAndRects.originMatToRect_map = mat2rect_map;

            if (progressInfo != null)
                progressInfo("Restoring Texture Formats & Read Flags", .8f);
            combiner._destroyAllTemporaryTextures();
            if (textureEditorMethods != null)
                textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);

            //if (report != null)
            //    Debug.Log(report.ToString());

            yield break;
        }

        internal static StringBuilder GenerateReport(TexturePipelineData data)
        {
            //generate report want to do this before
            StringBuilder report = new StringBuilder();
            if (data.numAtlases > 0)
            {
                report = new StringBuilder();
                report.AppendLine("Report");
                for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
                {
                    MaterialPropTexturesSet txs = data.distinctMaterialTextures[i];
                    report.AppendLine("----------");
                    report.Append("This set of textures will be resized to:" + txs.idealWidth + "x" + txs.idealHeight + "\n");
                    for (int j = 0; j < txs.ts.Length; j++)
                    {
                        if (!txs.ts[j].isNull)
                        {
                            report.Append("   [" + data.texPropertyNames[j].name + " " + txs.ts[j].GetTexName() + " " + txs.ts[j].width + "x" + txs.ts[j].height + "]");
                            if (txs.ts[j].matTilingRect.size != Vector2.one || txs.ts[j].matTilingRect.min != Vector2.zero) report.AppendFormat(" material scale {0} offset{1} ", txs.ts[j].matTilingRect.size.ToString("G4"), txs.ts[j].matTilingRect.min.ToString("G4"));
                            if (txs.obUVscale != Vector2.one || txs.obUVoffset != Vector2.zero) report.AppendFormat(" obUV scale {0} offset{1} ", txs.obUVscale.ToString("G4"), txs.obUVoffset.ToString("G4"));
                            report.AppendLine("");
                        }
                        else
                        {
                            report.Append("   [" + data.texPropertyNames[j].name + " null ");
                            if (!TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(j, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                            {
                                report.Append("no atlas will be created all textures null]\n");
                            }
                            else
                            {
                                report.AppendFormat("a 16x16 texture will be created]\n");
                            }
                        }
                    }
                    report.AppendLine("");
                    report.Append("Materials using:");
                    for (int j = 0; j < txs.matsAndGOs.mats.Count; j++)
                    {
                        report.Append(txs.matsAndGOs.mats[j].mat.name + ", ");
                    }
                    report.AppendLine("");
                }
            }
            return report;
        }

        /// <summary>
        /// 检测所有 textures 中只有一种纹理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static CreateAtlasForProperty[] CalculateAllTexturesAreNullAndSameColor(TexturePipelineData data)
        {
            // check if all textures are null and use same color for each atlas
            // will not generate an atlas if so
            CreateAtlasForProperty[] shouldWeCreateAtlasForProp = new CreateAtlasForProperty[data.texPropertyNames.Count];
            for (int propIdx = 0; propIdx < data.texPropertyNames.Count; propIdx++)
            {
                MaterialPropTexture firstTexture = data.distinctMaterialTextures[0].ts[propIdx];
                Color firstColor = Color.black;
                if (data._considerNonTextureProperties)
                {
                    firstColor = data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(
                        data.distinctMaterialTextures[0].matsAndGOs.mats[0].mat, 
                        data.texPropertyNames[propIdx]);
                }
                int numTexturesExisting = 0;
                int numTexturesMatchinFirst = 0;
                int numNonTexturePropertiesMatchingFirst = 0;
                for (int j = 0; j < data.distinctMaterialTextures.Count; j++)
                {
                    if (!data.distinctMaterialTextures[j].ts[propIdx].isNull)
                    {
                        numTexturesExisting++;
                    }
                    if (firstTexture.AreTexturesEqual(data.distinctMaterialTextures[j].ts[propIdx]))
                    {
                        numTexturesMatchinFirst++;
                    }
                    if (data._considerNonTextureProperties)
                    {
                        Color colJ = data.nonTexturePropertyBlender.GetColorAsItWouldAppearInAtlasIfNoTexture(data.distinctMaterialTextures[j].matsAndGOs.mats[0].mat, data.texPropertyNames[propIdx]);
                        if (colJ == firstColor)
                        {
                            numNonTexturePropertiesMatchingFirst++;
                        }
                    }

                }
                shouldWeCreateAtlasForProp[propIdx].allTexturesAreNull = numTexturesExisting == 0;
                shouldWeCreateAtlasForProp[propIdx].allTexturesAreSame = numTexturesMatchinFirst == data.distinctMaterialTextures.Count;
                shouldWeCreateAtlasForProp[propIdx].allNonTexturePropsAreSame = numNonTexturePropertiesMatchingFirst == data.distinctMaterialTextures.Count;
                Debug.Log(String.Format("AllTexturesAreNullAndSameColor prop: {0} createAtlas:{1}  val:{2}",
                    data.texPropertyNames[propIdx].name, 
                    _ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, shouldWeCreateAtlasForProp), 
                    shouldWeCreateAtlasForProp[propIdx]));
            }
            return shouldWeCreateAtlasForProp;
        }

        #region 运行时方法
        internal static AtlasPackingResult[] RunTexturePackerOnly(TexturePipelineData data, 
            bool doSplitIntoMultiAtlasIfTooBig, 
            ITextureCombinerPacker texturePacker)
        {
            AtlasPackingResult[] apr = texturePacker.CalculateAtlasRectangles(data, doSplitIntoMultiAtlasIfTooBig); 
            //copy materials PackingResults
            for (int i = 0; i < apr.Length; i++)
            {
                List<MatsAndGOs> matsList = new List<MatsAndGOs>();
                apr[i].data = matsList;
                for (int j = 0; j < apr[i].srcImgIdxs.Length; j++)
                {
                    MaterialPropTexturesSet ts = data.distinctMaterialTextures[apr[i].srcImgIdxs[j]];
                    matsList.Add(ts.matsAndGOs);
                }
            }

            return apr;
        }

        #endregion

        #region 图集打包

        internal static ITextureCombinerPacker CreatePacker(bool onlyOneTextureInAtlasReuseTextures, PackingAlgorithmEnum packingAlgorithm)
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

        #endregion
        
        internal static Vector2 GetAdjustedForScaleAndOffset2Dimensions(MaterialPropTexture source, Vector2 obUVoffset, Vector2 obUVscale, TexturePipelineData data)
        {
            if (source.matTilingRect.x == 0f && source.matTilingRect.y == 0f && source.matTilingRect.width == 1f && source.matTilingRect.height == 1f)
            {
                if (data._fixOutOfBoundsUVs)
                {
                    if (obUVoffset.x == 0f && obUVoffset.y == 0f && obUVscale.x == 1f && obUVscale.y == 1f)
                    {
                        return new Vector2(source.width, source.height); //no adjustment necessary
                    }
                }
                else
                {
                    return new Vector2(source.width, source.height); //no adjustment necessary
                }
            }

            Debug.Log("GetAdjustedForScaleAndOffset2Dimensions: " + source.GetTexName() + " " + obUVoffset + " " + obUVscale);
            Rect encapsulatingSamplingRect = source.GetEncapsulatingSamplingRect().GetRect();
            float newWidth = encapsulatingSamplingRect.width * source.width;
            float newHeight = encapsulatingSamplingRect.height * source.height;

            if (newWidth > data._maxTilingBakeSize) newWidth = data._maxTilingBakeSize;
            if (newHeight > data._maxTilingBakeSize) newHeight = data._maxTilingBakeSize;
            if (newWidth < 1f) newWidth = 1f;
            if (newHeight < 1f) newHeight = 1f;
            return new Vector2(newWidth, newHeight);
        }

        /* 
        Unity uses a non-standard format for storing normals for some platforms. Imagine the standard format is English, Unity's is French
        When the normal-map checkbox is ticked on the asset importer the normal map is translated into french. When we build the normal atlas
        we are reading the french. When we save and click the normal map tickbox we are translating french -> french. A double transladion that
        breaks the normal map. To fix this we need to "unconvert" the normal map to english when saving the atlas as a texture so that unity importer
        can do its thing properly. 
        */
        internal static Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
        {
            Vector3 n = Vector3.zero;
            n.x = c.a * 2f - 1f;
            n.y = c.g * 2f - 1f;
            n.z = Mathf.Sqrt(1 - n.x * n.x - n.y * n.y);
            //now repack in the regular format
            Color32 cc = new Color32();
            cc.a = 1;
            cc.r = (byte)((n.x + 1f) * .5f);
            cc.g = (byte)((n.y + 1f) * .5f);
            cc.b = (byte)((n.z + 1f) * .5f);
            return cc;
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

        internal static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

    }
}
