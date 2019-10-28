using GameWorld;
using System.Collections.Generic;
using UnityEngine;


namespace Client.Scene
{
    /// <summary>
    /// 材质合并管道数据
    /// 每个实例对应一个材质合并的流程。
    /// </summary>
    public class TextureCombinePipelineData
    {
        internal TextureBakeResults textureBakeResults;    //贴图合并结果Asset
        public Material ResultMaterial { get; set; }        //合并为单个材质时的合并材质
        internal int atlasPadding = 1;
        internal int maxAtlasWidth = 1;
        internal int maxAtlasHeight = 1;
        internal bool useMaxAtlasHeightOverride = false;
        internal bool useMaxAtlasWidthOverride = false;
        internal bool resizePowerOfTwoTextures = false;    //POT texture 重新设置尺寸（算上 padding）
        internal bool fixOutOfBoundsUVs = false;
        internal int maxTilingBakeSize = 1024;
        internal bool saveAtlasesAsAssets = false;          //是否保存合并资源
        internal PackingAlgorithmEnum packingAlgorithm = PackingAlgorithmEnum.UnitysPackTextures;
        internal bool meshBakerTexturePackerForcePowerOfTwo = true;              //贴图 Atlas 为 POT
        internal bool normalizeTexelDensity = false;
        internal bool considerNonTextureProperties = false;

        public MultiMaterial[] ResultMaterials = new MultiMaterial[0];
        

        public string CombinedMaterialInfoPath { get; set; }

        public string CombinerName { get; set; }

        public bool DoMultiMaterial{ get;set; }

        /// <summary>
        /// 材质合并的图集及Rect映射结果数组,每项对应一个 shader 的材质
        /// </summary>
        public AtlasesAndRects[] ResultAtlasesAndRects { get; set; }

        internal TextureCombinerNonTextureProperties nonTexturePropertyBlender;
        internal List<MaterialPropTexturesSet> distinctMaterialTextures;            //合并材质各属性所要合并的 Texture 集合
        internal List<GameObject> allObjsToMesh;
        internal List<Material> allowedMaterialsFilter;
        internal List<ShaderTextureProperty> texPropertyNames;
        internal List<ShaderTextureProperty> customShaderPropNames = new List<ShaderTextureProperty>();
        internal TextureCombinePipeline.CreateAtlasForProperty[] allTexturesAreNullAndSameColor;

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
                !fixOutOfBoundsUVs &&
                !considerNonTextureProperties)
            {
                return true;
            }
            return false;
        }
    }


}

