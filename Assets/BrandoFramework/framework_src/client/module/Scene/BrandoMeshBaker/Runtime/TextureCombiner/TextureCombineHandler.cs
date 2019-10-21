//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
    Test different texture packers
    Test lots of multiple material configs
    Try using on Coast scene
*/

/*
  
Notes on Normal Maps in Unity3d

Unity stores normal maps in a non standard format for some platforms. Think of the standard format as being english, unity's as being
french. The raw image files in the project folder are in english, the AssetImporter converts them to french. Texture2D.GetPixels returns 
french. This is a problem when we build an atlas from Texture2D objects and save the result in the project folder.
Unity wants us to flag this file as a normal map but if we do it is effectively translated twice.

Solutions:

    1) convert the normal map to english just before saving to project. Then set the normal flag and let the Importer do translation.
    This was rejected because Unity doesn't translate for all platforms. I would need to check with every version of Unity which platforms
    use which format.

    2) Uncheck "normal map" on importer before bake and re-check after bake. This is the solution I am using.

*/
namespace GameWorld
{
    [System.Serializable]
    public class TextureCombineHandler
    {
        /// <summary>
        /// 临时 Texture
        /// </summary>
        private class TemporaryTexture
        {
            internal string property;
            internal Texture2D texture;

            public TemporaryTexture(string prop, Texture2D tex)
            {
                property = prop;
                texture = tex;
            }
        }


        //Texture 合并结果
        [SerializeField]
        protected TextureBakeResults _textureBakeResults;
        public TextureBakeResults textureBakeResults
        {
            get { return _textureBakeResults; }
            set { _textureBakeResults = value; }
        }

        [SerializeField]
        protected int _atlasPadding = 1;
        public int atlasPadding
        {
            get { return _atlasPadding; }
            set { _atlasPadding = value; }
        }

        [SerializeField]
        protected int _maxAtlasSize = 1;
        public int maxAtlasSize
        {
            get { return _maxAtlasSize; }
            set { _maxAtlasSize = value; }
        }

        [SerializeField]
        protected int _maxAtlasWidthOverride = 4096;
        public virtual int maxAtlasWidthOverride
        {
            get { return _maxAtlasWidthOverride; }
            set { _maxAtlasWidthOverride = value; }
        }

        [SerializeField]
        protected int _maxAtlasHeightOverride = 4096;
        public virtual int maxAtlasHeightOverride
        {
            get { return _maxAtlasHeightOverride; }
            set { _maxAtlasHeightOverride = value; }
        }

        [SerializeField]
        protected bool _useMaxAtlasWidthOverride = false;
        public virtual bool useMaxAtlasWidthOverride
        {
            get { return _useMaxAtlasWidthOverride; }
            set { _useMaxAtlasWidthOverride = value; }
        }

        [SerializeField]
        protected bool _useMaxAtlasHeightOverride = false;
        public virtual bool useMaxAtlasHeightOverride
        {
            get { return _useMaxAtlasHeightOverride; }
            set { _useMaxAtlasHeightOverride = value; }
        }

        [SerializeField]
        protected bool _resizePowerOfTwoTextures = false;
        public bool resizePowerOfTwoTextures
        {
            get { return _resizePowerOfTwoTextures; }
            set { _resizePowerOfTwoTextures = value; }
        }

        [SerializeField]
        protected bool _fixOutOfBoundsUVs = false;
        public bool fixOutOfBoundsUVs
        {
            get { return _fixOutOfBoundsUVs; }
            set { _fixOutOfBoundsUVs = value; }
        }

        [SerializeField]
        protected int _maxTilingBakeSize = 1024;
        public int maxTilingBakeSize
        {
            get { return _maxTilingBakeSize; }
            set { _maxTilingBakeSize = value; }
        }

        [SerializeField]
        protected bool _saveAtlasesAsAssets = false;
        public bool saveAtlasesAsAssets
        {
            get { return _saveAtlasesAsAssets; }
            set { _saveAtlasesAsAssets = value; }
        }

        [SerializeField]
        protected PackingAlgorithmEnum _packingAlgorithm = PackingAlgorithmEnum.UnitysPackTextures;
        public PackingAlgorithmEnum packingAlgorithm
        {
            get { return _packingAlgorithm; }
            set { _packingAlgorithm = value; }
        }

        [SerializeField]
        protected bool _meshBakerTexturePackerForcePowerOfTwo = true;
        public bool meshBakerTexturePackerForcePowerOfTwo
        {
            get { return _meshBakerTexturePackerForcePowerOfTwo; }
            set { _meshBakerTexturePackerForcePowerOfTwo = value; }
        }

        [SerializeField]
        protected List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();
        public List<ShaderTextureProperty> customShaderPropNames
        {
            get { return _customShaderPropNames; }
            set { _customShaderPropNames = value; }
        }

        [SerializeField]
        protected bool _normalizeTexelDensity = false;

        [SerializeField]
        protected bool _considerNonTextureProperties = false;
        public bool considerNonTextureProperties
        {
            get { return _considerNonTextureProperties; }
            set { _considerNonTextureProperties = value; }
        }

        // 为贴图合并为 Atlas 创建的纹理副本
        // 最终将销毁
        private List<TemporaryTexture> _temporaryTextures = new List<TemporaryTexture>();
        

        //This runs a coroutine without pausing it is used to build the textures from the editor
        public static bool _RunCorutineWithoutPauseIsRunning = false;

        /// <summary>
        /// //不分帧运行协程方法
        /// </summary>
        /// <param name="cor"></param>
        /// <param name="recursionDepth"></param>
        public static void RunCorutineWithoutPause(IEnumerator cor, int recursionDepth)
        {
            //递归深度
            if (recursionDepth == 0)
            {
                _RunCorutineWithoutPauseIsRunning = true;
            }
            if (recursionDepth > 20)
            {
                Debug.LogError("Recursion Depth Exceeded.");
                return;
            }
            while (cor.MoveNext())
            {
                object retObj = cor.Current;
                if (retObj is YieldInstruction)
                {
                    //do nothing
                }
                else if (retObj == null)
                {
                    //do nothing
                }
                else if (retObj is IEnumerator)
                {
                    RunCorutineWithoutPause((IEnumerator)cor.Current, recursionDepth + 1);
                }
            }
            if (recursionDepth == 0)
            {
                _RunCorutineWithoutPauseIsRunning = false;
            }
        }


        /// <summary>
        /// 合并贴图形成纹理图集
        /// </summary>
        public bool CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, 
            AtlasesAndRects resultAtlasesAndRects, 
            Material resultMaterial, 
            List<GameObject> objsToMesh, 
            List<Material> allowedMaterialsFilter, 
            EditorMethodsInterface textureEditorMethods = null, 
            List<AtlasPackingResult> packingResults = null, 
            bool onlyPackRects = false, 
            bool splitAtlasWhenPackingIfTooBig = false)
        {
            CombineTexturesIntoAtlasesCoroutineResult result = new CombineTexturesIntoAtlasesCoroutineResult();
            RunCorutineWithoutPause(
                _CombineTexturesIntoAtlases(
                    progressInfo, 
                    result, 
                    resultAtlasesAndRects, 
                    resultMaterial, 
                    objsToMesh, 
                    allowedMaterialsFilter, 
                    textureEditorMethods, 
                    packingResults, 
                    onlyPackRects, 
                    splitAtlasWhenPackingIfTooBig
                    ),
                  0);
            if (result.success == false) Debug.LogError("Failed to generate atlases.");
            return result.success;
        }

        #region 合并材质流程

        #region 合并材质协程
        //float _maxTimePerFrameForCoroutine;
        public IEnumerator CombineTexturesIntoAtlasesCoroutine(ProgressUpdateDelegate progressInfo, 
            AtlasesAndRects resultAtlasesAndRects, 
            Material resultMaterial, 
            List<GameObject> objsToMesh, 
            List<Material> allowedMaterialsFilter, 
            EditorMethodsInterface textureEditorMethods = null, 
            CombineTexturesIntoAtlasesCoroutineResult coroutineResult = null, 
            float maxTimePerFrame = .01f, 
            List<AtlasPackingResult> packingResults = null, 
            bool onlyPackRects = false, 
            bool splitAtlasWhenPackingIfTooBig = false)
        {
            coroutineResult.success = true;
            coroutineResult.isFinished = false;
            if (maxTimePerFrame <= 0f)
            {
                Debug.LogError("maxTimePerFrame must be a value greater than zero");
                coroutineResult.isFinished = true;
                yield break;
            }
            //_maxTimePerFrameForCoroutine = maxTimePerFrame;
            yield return _CombineTexturesIntoAtlases(progressInfo, 
                coroutineResult, 
                resultAtlasesAndRects, 
                resultMaterial, 
                objsToMesh, 
                allowedMaterialsFilter, 
                textureEditorMethods, 
                packingResults, 
                onlyPackRects, 
                splitAtlasWhenPackingIfTooBig);
            coroutineResult.isFinished = true;
            yield break;
        }

        IEnumerator _CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, 
            CombineTexturesIntoAtlasesCoroutineResult result, 
            AtlasesAndRects resultAtlasesAndRects, 
            Material resultMaterial, 
            List<GameObject> objsToMesh, 
            List<Material> allowedMaterialsFilter, 
            EditorMethodsInterface textureEditorMethods, 
            List<AtlasPackingResult> atlasPackingResult, 
            bool onlyPackRects, 
            bool splitAtlasWhenPackingIfTooBig)
        {
            try
            {
                _temporaryTextures.Clear();
                MaterialPropTexture.readyToBuildAtlases = false;

                // ---- 0.合并材质前回调
                if (textureEditorMethods != null)
                {
                    textureEditorMethods.Clear();
                    textureEditorMethods.OnPreTextureBake();
                }

                // ---- 1.合并材质参数校验
                if (objsToMesh == null || objsToMesh.Count == 0)
                {
                    Debug.LogError("没有游戏物体参与合并");
                    result.success = false;
                    yield break;
                }

                if (_atlasPadding < 0)
                {
                    Debug.LogError("Atlas padding 必须大于等于零");
                    result.success = false;
                    yield break;
                }

                if (_maxTilingBakeSize < 2 || _maxTilingBakeSize > 4096)
                {
                    Debug.LogError("无效Tilling尺寸的值Invalid value for max tiling bake size.");
                    result.success = false;
                    yield break;
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
                            result.success = false;
                            yield break;
                        }

                    }
                }

                if (_fixOutOfBoundsUVs && (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal ||
                            _packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical))
                {
                    Debug.LogWarning("合并算法为 MeshBakerTexturePacker_Horizontal 或 MeshBakerTexturePacker_Vertical，建议不打开 Consider Mesh UVs 选项");
                }

                if (progressInfo != null)
                    progressInfo("Collecting textures for " + objsToMesh.Count + " meshes.", .01f);

                // ---- 2.创建材质合并管线数据
                TexturePipelineData data = CreatePipelineData(resultMaterial, 
                    new List<ShaderTextureProperty>(), 
                    objsToMesh, 
                    allowedMaterialsFilter, 
                    new List<MaterialPropTexturesSet>());

                // ---- 3.将材质的 shader 各参数信息写入管线数据中
                if (!TextureCombinerPipeline._CollectPropertyNames(data))
                {
                    result.success = false;
                    yield break;
                }

                // ---- 4.加载 Texture 混合器
                data.nonTexturePropertyBlender.LoadTextureBlendersIfNeeded(data.resultMaterial);

                // ---- 5.选择本地合并，或运行时合并
                if (onlyPackRects)
                {
                    yield return __RunTexturePackerOnly(result, data, splitAtlasWhenPackingIfTooBig, textureEditorMethods, atlasPackingResult);
                }
                else
                {
                    yield return __CombineTexturesIntoAtlases(progressInfo, result, resultAtlasesAndRects, data, splitAtlasWhenPackingIfTooBig, textureEditorMethods);
                }
            }
            finally
            {
                // ---- 6.删除缓存，合并材质完成回调
                _destroyAllTemporaryTextures();
                if (textureEditorMethods != null)
                {
                    textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
                    textureEditorMethods.OnPostTextureBake();
                }
            }
        }

        // texPropertyNames 是 resultMaterial中纹理属性的列表
        // allowedMaterialsFilter 是材料列表。 没有这些材料的物体将被忽略。这由多种材质过滤器使用
        // textureEditorMethods 仅封装编辑器功能，例如保存资产和跟踪纹理资产谁的格式已更改。 如果在运行时使用，则为null。
        IEnumerator __CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            AtlasesAndRects resultAtlasesAndRects,
            TexturePipelineData data,
            bool splitAtlasWhenPackingIfTooBig,
            EditorMethodsInterface textureEditorMethods)
        {
            if (progressInfo != null)
                progressInfo("Collecting textures ", .01f);

            // --- 1、记录各合并物体的源材质的 Prop Texture 信息写入 Data.distinctMaterialTextures
            //每个图集（主图，凹凸等）都将有的 MaterialTextures.Count 个图像。
            //每个 distinctMaterialTextures 对应一个游戏物体的某个材质，记录一组纹理，分别材质的在每个Prop图集一个。 
            List<GameObject> usedObjsToMesh = new List<GameObject>();
            yield return TextureCombinerPipeline.__Step1_CollectDistinctMatTexturesAndUsedObjects(progressInfo, result, data, textureEditorMethods, usedObjsToMesh);
            if (!result.success)
            {
                yield break;
            }

            // --- 2、计算使每个材质属性中的多个材质的合理尺寸
            yield return TextureCombinerPipeline._Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(progressInfo, result, data, textureEditorMethods);
            if (!result.success)
            {
                yield break;
            }

            // --- 3、创建特定打包方式的打包器
            ITextureCombinerPacker texturePaker = TextureCombinerPipeline.CreatePacker(data.IsOnlyOneTextureInAtlasReuseTextures(), data._packingAlgorithm);

            yield return texturePaker.ConvertTexturesToReadableFormats(progressInfo, result, data, this, textureEditorMethods);
            if (!result.success)
            {
                yield break;
            }

            // --- 4、计算各源材质在合并材质 Atlas 的排布 
            AtlasPackingResult[] uvRects = texturePaker.CalculateAtlasRectangles(data, splitAtlasWhenPackingIfTooBig);

            // --- 5、创建 Atlas 并保存
            yield return TextureCombinerPipeline.__Step3_BuildAndSaveAtlasesAndStoreResults(progressInfo,
                result,
                data,
                this,
                texturePaker,
                uvRects[0],
                textureEditorMethods,
                resultAtlasesAndRects);
        }

        /// <summary>
        /// 运行时图片打包
        /// </summary>
        /// <param name="result"></param>
        /// <param name="data"></param>
        /// <param name="splitAtlasWhenPackingIfTooBig"></param>
        /// <param name="textureEditorMethods"></param>
        /// <param name="packingResult"></param>
        /// <returns></returns>
        IEnumerator __RunTexturePackerOnly(CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            bool splitAtlasWhenPackingIfTooBig,
            EditorMethodsInterface textureEditorMethods,
            List<AtlasPackingResult> packingResult)
        {
            Debug.Log("__RunTexturePacker texture properties in shader:" + data.texPropertyNames.Count + " objsToMesh:" + data.allObjsToMesh.Count + " _fixOutOfBoundsUVs:" + data._fixOutOfBoundsUVs);
            List<GameObject> usedObjsToMesh = new List<GameObject>();
            yield return TextureCombinerPipeline.__Step1_CollectDistinctMatTexturesAndUsedObjects(null, result, data, textureEditorMethods, usedObjsToMesh);
            if (!result.success)
            {
                yield break;
            }

            data.allTexturesAreNullAndSameColor = new TextureCombinerPipeline.CreateAtlasForProperty[data.texPropertyNames.Count];
            yield return TextureCombinerPipeline._Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(null, result, data, textureEditorMethods);
            if (!result.success)
            {
                yield break;
            }

            ITextureCombinerPacker texturePaker = TextureCombinerPipeline.CreatePacker(data.IsOnlyOneTextureInAtlasReuseTextures(), data._packingAlgorithm);
            //    run the texture packer only
            AtlasPackingResult[] aprs = TextureCombinerPipeline.RunTexturePackerOnly(data, splitAtlasWhenPackingIfTooBig, texturePaker);
            for (int i = 0; i < aprs.Length; i++)
            {
                packingResult.Add(aprs[i]);
            }
        }

        #endregion

        TexturePipelineData CreatePipelineData(Material resultMaterial,
            List<ShaderTextureProperty> texPropertyNames, 
            List<GameObject> objsToMesh, 
            List<Material> allowedMaterialsFilter,
            List<MaterialPropTexturesSet> distinctMaterialTextures)
        {
            TexturePipelineData data = new TexturePipelineData();
            data._textureBakeResults = _textureBakeResults;
            data._atlasPadding = _atlasPadding;
            //合并贴图方式
            if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical && _useMaxAtlasHeightOverride)
            {
                data._maxAtlasHeight = _maxAtlasHeightOverride;
                data._useMaxAtlasHeightOverride = true;
            }
            else
            {
                data._maxAtlasHeight = _maxAtlasSize;
            }

            if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal && _useMaxAtlasWidthOverride)
            {
                data._maxAtlasWidth = _maxAtlasWidthOverride;
                data._useMaxAtlasWidthOverride = true;
            } else
            {
                data._maxAtlasWidth = _maxAtlasSize;
            }

            data._resizePowerOfTwoTextures = _resizePowerOfTwoTextures;
            data._fixOutOfBoundsUVs = _fixOutOfBoundsUVs;
            data._maxTilingBakeSize = _maxTilingBakeSize;
            data._saveAtlasesAsAssets = _saveAtlasesAsAssets;
            data._packingAlgorithm = _packingAlgorithm;
            data._meshBakerTexturePackerForcePowerOfTwo = _meshBakerTexturePackerForcePowerOfTwo;
            data._customShaderPropNames = _customShaderPropNames;
            data._normalizeTexelDensity = _normalizeTexelDensity;
            data._considerNonTextureProperties = _considerNonTextureProperties;
            data.nonTexturePropertyBlender = new TextureCombinerNonTextureProperties( _considerNonTextureProperties);
            data.resultMaterial = resultMaterial;
            data.distinctMaterialTextures = distinctMaterialTextures;
            data.allObjsToMesh = objsToMesh;
            data.allowedMaterialsFilter = allowedMaterialsFilter;
            data.texPropertyNames = texPropertyNames;
            return data;
        }

        #endregion

        internal int _getNumTemporaryTextures()
        {
            return _temporaryTextures.Count;
        }

        //used to track temporary textures that were created so they can be destroyed
        public Texture2D _createTemporaryTexture(string propertyName, int w, int h, TextureFormat texFormat, bool mipMaps)
        {
            Texture2D t = new Texture2D(w, h, texFormat, mipMaps);
            t.name = string.Format("tmp{0}_{1}x{2}", _temporaryTextures.Count, w, h);
            MeshBakerUtility.setSolidColor(t, Color.clear);
            TemporaryTexture txx = new TemporaryTexture(propertyName, t);
            _temporaryTextures.Add(txx);
            return t;
        }

        internal Texture2D _createTextureCopy(string propertyName, Texture2D t)
        {
            Texture2D tx = MeshBakerUtility.createTextureCopy(t);
            tx.name = string.Format("tmpCopy{0}_{1}x{2}", _temporaryTextures.Count, tx.width, tx.height);
            TemporaryTexture txx = new TemporaryTexture(propertyName, tx);
            _temporaryTextures.Add(txx);
            return tx;
        }

        internal Texture2D _resizeTexture(string propertyName, Texture2D t, int w, int h)
        {
            Texture2D tx = MeshBakerUtility.resampleTexture(t, w, h);
            tx.name = string.Format("tmpResampled{0}_{1}x{2}", _temporaryTextures.Count, w, h);
            TemporaryTexture txx = new TemporaryTexture(propertyName, tx);
            _temporaryTextures.Add(txx);
            return tx;
        }

        internal void _destroyAllTemporaryTextures()
        {
            Debug.Log("Destroying " + _temporaryTextures.Count + " temporary textures");
            for (int i = 0; i < _temporaryTextures.Count; i++)
            {
                MeshBakerUtility.Destroy(_temporaryTextures[i].texture);
            }
            _temporaryTextures.Clear();
        }

        internal void _destroyTemporaryTextures(string propertyName)
        {
            int numDestroyed = 0;
            for (int i = _temporaryTextures.Count - 1; i >= 0; i--)
            {
                if (_temporaryTextures[i].property.Equals(propertyName))
                {
					numDestroyed++;
                    MeshBakerUtility.Destroy(_temporaryTextures[i].texture);
                    _temporaryTextures.RemoveAt(i);
                }
            }
            Debug.Log("Destroying " + numDestroyed + " temporary textures " + propertyName + " num remaining " + _temporaryTextures.Count);
        }

        public void SuggestTreatment(List<GameObject> objsToMesh, Material[] resultMaterials, List<ShaderTextureProperty> _customShaderPropNames)
        {
            this._customShaderPropNames = _customShaderPropNames;
            StringBuilder sb = new StringBuilder();
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MeshAnalysisResult[]>(); //cache results
            for (int i = 0; i < objsToMesh.Count; i++)
            {
                GameObject obj = objsToMesh[i];
                if (obj == null) continue;
                Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                if (ms.Length > 1)
                { // and each material is not mapped to its own layer
                    sb.AppendFormat("\nObject {0} uses {1} materials. Possible treatments:\n", objsToMesh[i].name, ms.Length);
                    sb.AppendFormat("  1) Collapse the submeshes together into one submesh in the combined mesh. Each of the original submesh materials will map to a different UV rectangle in the atlas(es) used by the combined material.\n");
                    sb.AppendFormat("  2) Use the multiple materials feature to map submeshes in the source mesh to submeshes in the combined mesh.\n");
                }
                Mesh m = MeshBakerUtility.GetMesh(obj);

                MeshAnalysisResult[] mar;
                if (!meshAnalysisResultsCache.TryGetValue(m.GetInstanceID(), out mar))
                {
                    mar = new MeshAnalysisResult[m.subMeshCount];
                    MeshBakerUtility.doSubmeshesShareVertsOrTris(m, ref mar[0]);
                    for (int j = 0; j < m.subMeshCount; j++)
                    {
                        MeshBakerUtility.hasOutOfBoundsUVs(m, ref mar[j], j);
                        //DRect outOfBoundsUVRect = new DRect(mar[j].uvRect);
                        mar[j].hasOverlappingSubmeshTris = mar[0].hasOverlappingSubmeshTris;
                        mar[j].hasOverlappingSubmeshVerts = mar[0].hasOverlappingSubmeshVerts;
                    }
                    meshAnalysisResultsCache.Add(m.GetInstanceID(), mar);
                }

                for (int j = 0; j < ms.Length; j++)
                {
                    if (mar[j].hasOutOfBoundsUVs)
                    {
                        DRect r = new DRect(mar[j].uvRect);
                        sb.AppendFormat("\nObject {0} submesh={1} material={2} uses UVs outside the range 0,0 .. 1,1 to create tiling that tiles the box {3},{4} .. {5},{6}. This is a problem because the UVs outside the 0,0 .. 1,1 " +
                                        "rectangle will pick up neighboring textures in the atlas. Possible Treatments:\n", obj, j, ms[j], r.x.ToString("G4"), r.y.ToString("G4"), (r.x + r.width).ToString("G4"), (r.y + r.height).ToString("G4"));
                        sb.AppendFormat("    1) Ignore the problem. The tiling may not affect result significantly.\n");
                        sb.AppendFormat("    2) Use the 'fix out of bounds UVs' feature to bake the tiling and scale the UVs to fit in the 0,0 .. 1,1 rectangle.\n");
                        sb.AppendFormat("    3) Use the Multiple Materials feature to map the material on this submesh to its own submesh in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n");
                        sb.AppendFormat("    4) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n");
                    }
                }
                if (mar[0].hasOverlappingSubmeshVerts)
                {
                    sb.AppendFormat("\nObject {0} has submeshes that share vertices. This is a problem because each vertex can have only one UV coordinate and may be required to map to different positions in the various atlases that are generated. Possible treatments:\n", objsToMesh[i]);
                    sb.AppendFormat(" 1) Ignore the problem. The vertices may not affect the result.\n");
                    sb.AppendFormat(" 2) Use the Multiple Materials feature to map the submeshs that overlap to their own submeshs in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n");
                    sb.AppendFormat(" 3) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n");
                }
            }
            Dictionary<Material, List<GameObject>> m2gos = new Dictionary<Material, List<GameObject>>();
            for (int i = 0; i < objsToMesh.Count; i++)
            {
                if (objsToMesh[i] != null)
                {
                    Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                    for (int j = 0; j < ms.Length; j++)
                    {
                        if (ms[j] != null)
                        {
                            List<GameObject> lgo;
                            if (!m2gos.TryGetValue(ms[j], out lgo))
                            {
                                lgo = new List<GameObject>();
                                m2gos.Add(ms[j], lgo);
                            }
                            if (!lgo.Contains(objsToMesh[i])) lgo.Add(objsToMesh[i]);
                        }
                    }
                }
            }

            for (int i = 0; i < resultMaterials.Length; i++)
            {
                string resultMatShaderName = resultMaterials[i] != null ? "None" : resultMaterials[i].shader.name;
                TexturePipelineData data = CreatePipelineData(resultMaterials[i], new List<ShaderTextureProperty>(), objsToMesh, new List<Material>(), new List<MaterialPropTexturesSet>());
                TextureCombinerPipeline._CollectPropertyNames(data);
                foreach (Material m in m2gos.Keys)
                {
                    for (int j = 0; j < data.texPropertyNames.Count; j++)
                    {
                        if (m.HasProperty(data.texPropertyNames[j].name))
                        {
                            Texture txx = TextureCombinerPipeline.GetTextureConsideringStandardShaderKeywords(resultMatShaderName, m, data.texPropertyNames[j].name);
                            if (txx != null)
                            {
                                Vector2 o = m.GetTextureOffset(data.texPropertyNames[j].name);
                                Vector3 s = m.GetTextureScale(data.texPropertyNames[j].name);
                                if (o.x < 0f || o.x + s.x > 1f ||
                                    o.y < 0f || o.y + s.y > 1f)
                                {
                                    sb.AppendFormat("\nMaterial {0} used by objects {1} uses texture {2} that is tiled (scale={3} offset={4}). If there is more than one texture in the atlas " +
                                                        " then Mesh Baker will bake the tiling into the atlas. If the baked tiling is large then quality can be lost. Possible treatments:\n", m, PrintList(m2gos[m]), txx, s, o);
                                    sb.AppendFormat("  1) Use the baked tiling.\n");
                                    sb.AppendFormat("  2) Use the Multiple Materials feature to map the material on this object/submesh to its own submesh in the combined mesh. No other materials should map to this submesh. The original material can be applied to this submesh.\n");
                                    sb.AppendFormat("  3) Combine only meshes that use the same (or subset of) the set of textures on this mesh. The original material can be applied to the result.\n");
                                }
                            }
                        }
                    }
                }
            }
            string outstr = "";
            if (sb.Length == 0)
            {
                outstr = "====== No problems detected. These meshes should combine well ====\n  If there are problems with the combined meshes please report the problem to digitalOpus.ca so we can improve Mesh Baker.";
            }
            else
            {
                outstr = "====== There are possible problems with these meshes that may prevent them from combining well. TREATMENT SUGGESTIONS (copy and paste to text editor if too big) =====\n" + sb.ToString();
            }
            Debug.Log(outstr);
        }

        string PrintList(List<GameObject> gos)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < gos.Count; i++)
            {
                sb.Append(gos[i] + ",");
            }
            return sb.ToString();
        }

    }
}
