using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// Component that handles baking materials into a combined material.
    /// 材质合并入口组件
    /// 合并逻辑于 TextureCombineHandler 实现
    /// </summary>
    public class TextureCombineEntrance : MeshBakerRoot
    {
        public delegate void OnCombinedTexturesCoroutineSuccess();
        public delegate void OnCombinedTexturesCoroutineFail();

        [SerializeField]
        protected TextureBakeResults _textureBakeResults;
        public override TextureBakeResults textureBakeResults
        {
            get { return _textureBakeResults; }
            set { _textureBakeResults = value; }
        }

        /// <summary>
        /// 图集内贴图间隙
        /// </summary>
        [SerializeField]
        protected int _atlasPadding = 1;
        public virtual int atlasPadding
        {
            get { return _atlasPadding; }
            set { _atlasPadding = value; }
        }

        /// <summary>
        /// 材质贴图图集最大尺寸
        /// </summary>
        [SerializeField]
        protected int _maxAtlasSize = 4096;
        public virtual int maxAtlasSize
        {
            get { return _maxAtlasSize; }
            set { _maxAtlasSize = value; }
        }

        [SerializeField]
        protected bool _useMaxAtlasWidthOverride = false;
        public virtual bool useMaxAtlasWidthOverride
        {
            get { return _useMaxAtlasWidthOverride; }
            set { _useMaxAtlasWidthOverride = value; }
        }

        [SerializeField]
        protected int _maxAtlasWidthOverride = 4096;
        public virtual int maxAtlasWidthOverride
        {
            get { return _maxAtlasWidthOverride; }
            set { _maxAtlasWidthOverride = value; }
        }

        [SerializeField]
        protected bool _useMaxAtlasHeightOverride = false;
        public virtual bool useMaxAtlasHeightOverride
        {
            get { return _useMaxAtlasHeightOverride; }
            set { _useMaxAtlasHeightOverride = value; }
        }

        [SerializeField]
        protected int _maxAtlasHeightOverride = 4096;
        public virtual int maxAtlasHeightOverride
        {
            get { return _maxAtlasHeightOverride; }
            set { _maxAtlasHeightOverride = value; }
        }

        //POT
        [SerializeField]
        protected bool _resizePowerOfTwoTextures = false;
        public virtual bool resizePowerOfTwoTextures
        {
            get { return _resizePowerOfTwoTextures; }
            set { _resizePowerOfTwoTextures = value; }
        }

        [SerializeField]
        protected bool _fixOutOfBoundsUVs = false; //is considerMeshUVs but can't change because it would break all existing bakers
        public virtual bool fixOutOfBoundsUVs
        {
            get { return _fixOutOfBoundsUVs; }
            set { _fixOutOfBoundsUVs = value; }
        }

        [SerializeField]
        protected int _maxTilingBakeSize = 1024;
        public virtual int maxTilingBakeSize
        {
            get { return _maxTilingBakeSize; }
            set { _maxTilingBakeSize = value; }
        }

        /// <summary>
        /// 合并贴图方式算法
        /// </summary>
        [SerializeField]
        protected PackingAlgorithmEnum _packingAlgorithm = PackingAlgorithmEnum.MeshBakerTexturePacker;
        public virtual PackingAlgorithmEnum packingAlgorithm
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
        protected List<ShaderTextureProperty> _customShaderProperties = new List<ShaderTextureProperty>();
        public virtual List<ShaderTextureProperty> customShaderProperties
        {
            get { return _customShaderProperties; }
            set { _customShaderProperties = value; }
        }

        //this is depricated
        //不推荐使用
        [SerializeField]
        protected List<string> _customShaderPropNames_Depricated = new List<string>();
        public virtual List<string> customShaderPropNames
        {
            get { return _customShaderPropNames_Depricated; }
            set { _customShaderPropNames_Depricated = value; }
        }

        /// <summary>
        /// 是否合并为多个材质
        /// </summary>
        [SerializeField]
        protected bool _doMultiMaterial;
        public virtual bool doMultiMaterial
        {
            get { return _doMultiMaterial; }
            set { _doMultiMaterial = value; }
        }

        /// <summary>
        /// 如果 Atlas 过大是否合并为多个材质
        /// </summary>
        [SerializeField]
        protected bool _doMultiMaterialSplitAtlasesIfTooBig = true;
        public virtual bool doMultiMaterialSplitAtlasesIfTooBig
        {
            get { return _doMultiMaterialSplitAtlasesIfTooBig; }
            set { _doMultiMaterialSplitAtlasesIfTooBig = value; }
        }

        [SerializeField]
        protected bool _doMultiMaterialSplitAtlasesIfOBUVs = true;
        public virtual bool doMultiMaterialSplitAtlasesIfOBUVs
        {
            get { return _doMultiMaterialSplitAtlasesIfOBUVs; }
            set { _doMultiMaterialSplitAtlasesIfOBUVs = value; }
        }

        [SerializeField]
        protected bool _considerNonTextureProperties = false;
        public bool considerNonTextureProperties
        {
            get { return _considerNonTextureProperties; }
            set { _considerNonTextureProperties = value; }
        }

        [SerializeField]
        protected bool _doSuggestTreatment = true;
        public bool doSuggestTreatment
        {
            get { return _doSuggestTreatment; }
            set { _doSuggestTreatment = value; }
        }

        private CreateAtlasesCoroutineResult _coroutineResult;
        public CreateAtlasesCoroutineResult CoroutineResult
        {
            get{ return _coroutineResult; }
        }

        /// <summary>
        /// 合并后材质
        /// </summary>
        [SerializeField]
        protected Material _resultMaterial;
        public virtual Material resultMaterial
        {
            get { return _resultMaterial; }
            set { _resultMaterial = value; }
        }

        public MultiMaterial[] resultMaterials = new MultiMaterial[0];

        public List<GameObject> objsToMesh;

        public override List<GameObject> GetObjectsToCombine()
        {
            if (objsToMesh == null)
                objsToMesh = new List<GameObject>();
            return objsToMesh;
        }

        #region 创建合并材质贴图图集

        public OnCombinedTexturesCoroutineSuccess onBuiltAtlasesSuccess;
        public OnCombinedTexturesCoroutineFail onBuiltAtlasesFail;
        
        public AtlasesAndRects[] OnCombinedTexturesCoroutineAtlasesAndRects;

        public AtlasesAndRects[] CreateAtlases()
        {
            return CreateAtlases(null, false, null);
        }

        /// <summary>
        /// Creates the atlases.创建图集
        /// saveAtlasesAsAssets 创建图集并保存至项目资源目录，或者内存中
        /// editorMethods 贴图格式编辑器方法
        /// </summary>
        public AtlasesAndRects[] CreateAtlases(ProgressUpdateDelegate progressInfo,
            bool saveAtlasesAsAssets = false,
            EditorMethodsInterface editorMethods = null)
        {
            AtlasesAndRects[] resultAtlasesAndRects = null;
            try
            {
                _coroutineResult = new CreateAtlasesCoroutineResult();
                
                TextureCombineHandler.RunCorutineWithoutPause(
                    CreateAtlasesCoroutine(progressInfo, _coroutineResult, saveAtlasesAsAssets, editorMethods, 1000f), 0);
                if (_coroutineResult.success && textureBakeResults != null)
                {
                    resultAtlasesAndRects = this.OnCombinedTexturesCoroutineAtlasesAndRects;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                if (saveAtlasesAsAssets)
                { //Atlases were saved to project so we don't need these ones
                    if (resultAtlasesAndRects != null)
                    {
                        for (int j = 0; j < resultAtlasesAndRects.Length; j++)
                        {
                            AtlasesAndRects mAndA = resultAtlasesAndRects[j];
                            if (mAndA != null && mAndA.atlases != null)
                            {
                                for (int i = 0; i < mAndA.atlases.Length; i++)
                                {
                                    if (mAndA.atlases[i] != null)
                                    {
                                        if (editorMethods != null)
                                        {
                                            editorMethods.Destroy(mAndA.atlases[i]);
                                        }
                                        else
                                        {
                                            MeshBakerUtility.Destroy(mAndA.atlases[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return resultAtlasesAndRects;
        }

        /// <summary>
        /// 创建贴图 Atlas 协程
        /// </summary>
        /// <returns></returns>
        public IEnumerator CreateAtlasesCoroutine(ProgressUpdateDelegate progressInfo,
            CreateAtlasesCoroutineResult coroutineResult,
            bool saveAtlasesAsAssets = false,
            EditorMethodsInterface editorMethods = null,
            float maxTimePerFrame = .01f)
        {
            OnCombinedTexturesCoroutineAtlasesAndRects = null;

            //--- 1、合并前检测
            if (maxTimePerFrame <= 0f)
            {
                Debug.LogError("maxTimePerFrame must be a value greater than zero");
                coroutineResult.isFinished = true;
                yield break;
            }

            //验证等级
            ValidationLevel vl = Application.isPlaying ? ValidationLevel.quick : ValidationLevel.robust;

            //验证
            if (!DoCombinedValidate(this, ObjsToCombineTypes.dontCare, null, vl))
            {
                coroutineResult.isFinished = true;
                yield break;
            }

                //合并为多材质验证
            if (_doMultiMaterial && !_ValidateResultMaterials())
            {
                coroutineResult.isFinished = true;
                yield break;
            }
            else if (!_doMultiMaterial)
            {
                //合并为单独材质
                if (_resultMaterial == null)
                {
                    Debug.LogError("Combined Material is null please create and assign a result material.");
                    coroutineResult.isFinished = true;
                    yield break;
                }
                Shader targShader = _resultMaterial.shader;
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
            }

            TextureCombineHandler combiner = CreateAndConfigureTextureCombiner();
            combiner.saveAtlasesAsAssets = saveAtlasesAsAssets;

            ////--- 2、初始化存储合并结果的数据结构
            int numResults = 1;
            if (_doMultiMaterial)
            {
                numResults = resultMaterials.Length;
            }
                
            OnCombinedTexturesCoroutineAtlasesAndRects = new AtlasesAndRects[numResults];
            for (int i = 0; i < OnCombinedTexturesCoroutineAtlasesAndRects.Length; i++)
            {
                OnCombinedTexturesCoroutineAtlasesAndRects[i] = new AtlasesAndRects();
            }

            //--- 3、开始合并材质（单个，多个）
            for (int i = 0; i < OnCombinedTexturesCoroutineAtlasesAndRects.Length; i++)
            {
                Material resMatToPass;
                List<Material> sourceMats;
                if (_doMultiMaterial)
                {
                    sourceMats = resultMaterials[i].sourceMaterials;
                    resMatToPass = resultMaterials[i].combinedMaterial;
                    combiner.fixOutOfBoundsUVs = resultMaterials[i].considerMeshUVs;
                }
                else
                {
                    resMatToPass = _resultMaterial;
                    sourceMats = null;
                }

                //TextureHandler 材质合并协程结果
                CombineTexturesIntoAtlasesCoroutineResult coroutineResult2 = new CombineTexturesIntoAtlasesCoroutineResult();
                yield return combiner.CombineTexturesIntoAtlasesCoroutine(progressInfo,
                    OnCombinedTexturesCoroutineAtlasesAndRects[i],
                    resMatToPass,
                    objsToMesh,
                    sourceMats,
                    editorMethods,
                    coroutineResult2,
                    maxTimePerFrame);

                coroutineResult.success = coroutineResult2.success;
                if (!coroutineResult.success)
                {
                    coroutineResult.isFinished = true;
                    yield break;
                }
            }

            //--- 4、TextureBakeResults 保存合并结果
            unpackMat2RectMap(textureBakeResults);
            textureBakeResults.doMultiMaterial = _doMultiMaterial;
            if (_doMultiMaterial)
            {
                textureBakeResults.resultMaterials = resultMaterials;
            }
            else
            {
                MultiMaterial[] resMats = new MultiMaterial[1];
                resMats[0] = new MultiMaterial();
                resMats[0].combinedMaterial = _resultMaterial;
                resMats[0].considerMeshUVs = _fixOutOfBoundsUVs;
                resMats[0].sourceMaterials = new List<Material>();
                for (int i = 0; i < textureBakeResults.materialsAndUVRects.Length; i++)
                {
                    resMats[0].sourceMaterials.Add(textureBakeResults.materialsAndUVRects[i].material);
                }
                textureBakeResults.resultMaterials = resMats;
            }

            //--- 5、传递合并结果到 MeshCombiner 
            MeshBakerCommon[] mb = GetComponentsInChildren<MeshBakerCommon>();
            for (int i = 0; i < mb.Length; i++)
            {
                mb[i].textureBakeResults = textureBakeResults;
            }
            coroutineResult.isFinished = true;

            //--- 6、合并材质结束回调
            if (coroutineResult.success && onBuiltAtlasesSuccess != null)
            {
                onBuiltAtlasesSuccess();
            }
            if (!coroutineResult.success && onBuiltAtlasesFail != null)
            {
                onBuiltAtlasesFail();
            }
        }

        /// <summary>
        /// 多材质验证
        /// </summary>
        /// <returns></returns>
        bool _ValidateResultMaterials()
        {
            HashSet<Material> allMatsOnObjs = new HashSet<Material>();
            for (int i = 0; i < objsToMesh.Count; i++)
            {
                if (objsToMesh[i] != null)
                {
                    Material[] ms = MeshBakerUtility.GetGOMaterials(objsToMesh[i]);
                    for (int j = 0; j < ms.Length; j++)
                    {
                        if (ms[j] != null) allMatsOnObjs.Add(ms[j]);
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
                        Debug.LogError(String.Format("Source To Combined Mapping: Submesh {0} and Submesh {1} use the same combined material. These should be different", i, j));
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
                Debug.LogWarning("There are materials in the mapping that are not used on your source objects: " + PrintSet(allMatsInMapping));
            }
            if (resultMaterials != null && resultMaterials.Length > 0 && allMatsInMapping.IsProperSubsetOf(allMatsOnObjs))
            {
                allMatsOnObjs.ExceptWith(allMatsInMapping);
                Debug.LogError("There are materials on the objects to combine that are not in the mapping: " + PrintSet(allMatsOnObjs));
                return false;
            }
            return true;
        }

        string PrintSet(HashSet<Material> s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Material m in s)
            {
                sb.Append(m + ",");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 初始化贴图合并处理器
        /// </summary>
        /// <returns></returns>
        public TextureCombineHandler CreateAndConfigureTextureCombiner()
        {
            TextureCombineHandler combiner = new TextureCombineHandler();
            combiner.atlasPadding = _atlasPadding;
            combiner.maxAtlasSize = _maxAtlasSize;
            combiner.maxAtlasHeightOverride = _maxAtlasHeightOverride;
            combiner.maxAtlasWidthOverride = _maxAtlasWidthOverride;
            combiner.useMaxAtlasHeightOverride = _useMaxAtlasHeightOverride;
            combiner.useMaxAtlasWidthOverride = _useMaxAtlasWidthOverride;
            combiner.customShaderPropNames = _customShaderProperties;
            combiner.fixOutOfBoundsUVs = _fixOutOfBoundsUVs;
            combiner.maxTilingBakeSize = _maxTilingBakeSize;
            combiner.packingAlgorithm = _packingAlgorithm;
            combiner.meshBakerTexturePackerForcePowerOfTwo = _meshBakerTexturePackerForcePowerOfTwo;
            combiner.resizePowerOfTwoTextures = _resizePowerOfTwoTextures;
            combiner.considerNonTextureProperties = _considerNonTextureProperties;
            return combiner;
        }

        void unpackMat2RectMap(TextureBakeResults tbr)
        {
            List<Material> ms = new List<Material>();
            List<MaterialAndUVRect> mss = new List<MaterialAndUVRect>();
            List<Rect> rs = new List<Rect>();
            for (int i = 0; i < OnCombinedTexturesCoroutineAtlasesAndRects.Length; i++)
            {
                AtlasesAndRects newMesh = OnCombinedTexturesCoroutineAtlasesAndRects[i];
                List<MaterialAndUVRect> map = newMesh.originMatToRect_map;
                if (map != null)
                {
                    for (int j = 0; j < map.Count; j++)
                    {
                        mss.Add(map[j]);
                        ms.Add(map[j].material);
                        rs.Add(map[j].atlasRect);
                    }
                }
            }
            //tbr.version = MB2_TextureBakeResults.VERSION;
            tbr.materialsAndUVRects = mss.ToArray();
        }

        #endregion

        //复制材质
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
        
    }
}

