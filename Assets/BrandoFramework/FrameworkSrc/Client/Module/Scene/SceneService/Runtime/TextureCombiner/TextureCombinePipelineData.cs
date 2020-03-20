using GameWorld;
using System.Collections.Generic;
using UnityEngine;


namespace Client.Scene
{
    /// <summary>
    /// ���ʺϲ��ܵ�����
    /// ÿ��ʵ����Ӧһ�����ʺϲ������̡�
    /// </summary>
    public class TextureCombinePipelineData
    {
        internal TextureBakeResults textureBakeResults;    //��ͼ�ϲ����Asset
        public Material ResultMaterial { get; set; }        //�ϲ�Ϊ��������ʱ�ĺϲ�����
        internal int atlasPadding = 1;
        internal int maxAtlasWidth = 1;
        internal int maxAtlasHeight = 1;
        internal bool useMaxAtlasHeightOverride = false;
        internal bool useMaxAtlasWidthOverride = false;
        internal bool resizePowerOfTwoTextures = false;    //POT texture �������óߴ磨���� padding��
        internal bool fixOutOfBoundsUVs = false;
        internal int maxTilingBakeSize = 1024;
        internal bool saveAtlasesAsAssets = false;          //�Ƿ񱣴�ϲ���Դ
        internal PackingAlgorithmEnum packingAlgorithm = PackingAlgorithmEnum.UnitysPackTextures;
        internal bool meshBakerTexturePackerForcePowerOfTwo = true;              //��ͼ Atlas Ϊ POT
        internal bool normalizeTexelDensity = false;
        internal bool considerNonTextureProperties = false;

        public MultiMaterial[] ResultMaterials = new MultiMaterial[0];
        

        public string CombinedMaterialInfoPath { get; set; }

        public string CombinerName { get; set; }

        public bool DoMultiMaterial{ get;set; }

        /// <summary>
        /// ���ʺϲ���ͼ����Rectӳ��������,ÿ���Ӧһ�� shader �Ĳ���
        /// </summary>
        public AtlasesAndRects[] ResultAtlasesAndRects { get; set; }

        internal TextureCombinerNonTextureProperties nonTexturePropertyBlender;
        internal List<MaterialPropTexturesSet> distinctMaterialTextures;            //�ϲ����ʸ�������Ҫ�ϲ��� Texture ����
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
        /// �Ƿ�ϲ�Texture �� Atlas ��ֻ��һ����ͼ
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

