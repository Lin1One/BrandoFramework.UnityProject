//----------------------------------------------
//            MeshBaker
// Copyright © 2011-2012 Ian Deane
//----------------------------------------------
using System.Collections.Generic;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// 贴图合并管道数据
    /// 每个实例对应一个材质合并的流程。
    /// </summary>
    public class TexturePipelineData
    {
        internal TextureBakeResults _textureBakeResults;    //贴图合并结果Asset
        internal Material resultMaterial;                   //合并为单个材质时的合并材质

        internal int _atlasPadding = 1;
        internal int _maxAtlasWidth = 1;                    //
        internal int _maxAtlasHeight = 1;
        internal bool _useMaxAtlasHeightOverride = false;
        internal bool _useMaxAtlasWidthOverride = false;
        internal bool _resizePowerOfTwoTextures = false;    //POT texture 重新设置尺寸（算上 padding）
        internal bool _fixOutOfBoundsUVs = false;
        internal int _maxTilingBakeSize = 1024;
        internal bool _saveAtlasesAsAssets = false;
        internal PackingAlgorithmEnum _packingAlgorithm = PackingAlgorithmEnum.UnitysPackTextures;
        internal bool _meshBakerTexturePackerForcePowerOfTwo = true;              //贴图 Atlas 为 POT
        internal bool _normalizeTexelDensity = false;
        internal bool _considerNonTextureProperties = false;
        internal TextureCombinerNonTextureProperties nonTexturePropertyBlender;
        internal List<MaterialPropTexturesSet> distinctMaterialTextures;            //合并材质各属性所要合并的 Texture 集合
        internal List<GameObject> allObjsToMesh;
        internal List<Material> allowedMaterialsFilter;
        internal List<ShaderTextureProperty> texPropertyNames;
        internal List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();
        internal TextureCombinerPipeline.CreateAtlasForProperty[] allTexturesAreNullAndSameColor;
        internal int numAtlases
        {
            get
            {
                if (texPropertyNames != null) return texPropertyNames.Count;
                else return 0;
            }
        }
        

        /// <summary>
        /// 是否合并Texture 的 Atlas 中只有一个张图
        /// </summary>
        /// <returns></returns>
        internal bool IsOnlyOneTextureInAtlasReuseTextures()
        {
            if (distinctMaterialTextures != null &&
                distinctMaterialTextures.Count == 1 &&
                distinctMaterialTextures[0].thisIsOnlyTexSetInAtlas == true &&
                !_fixOutOfBoundsUVs &&
                !_considerNonTextureProperties)
            {
                return true;
            }
            return false;
        }
    }

}
