using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameWorld;

namespace  Client.Scene
{
    internal abstract class TextureCombineAtlasPackerBase : ITextureCombineAtlasPacker
    {
        public virtual void ConvertTexturesToReadableFormats(
            TextureCombinePipelineData data,
            EditorMethodsInterface textureEditorMethods)
        {
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                for (int j = 0; j < data.texPropertyNames.Count; j++)
                {
                    MaterialPropTexture ts = data.distinctMaterialTextures[i].ts[j];
                    if (!ts.isNull)
                    {
                        if (textureEditorMethods != null)
                        {
                            Texture tx = ts.GetTexture2D();
                            textureEditorMethods.AddTextureFormat(
                                (Texture2D)tx, data.texPropertyNames[j].isNormalMap);
                        }
                    }
                }
            }
        }

        public abstract void CreateAtlases(TextureCombinePipelineData data,
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases,
            EditorMethodsInterface textureEditorMethods);

        

        public virtual AtlasPackingResult[] CalculateAtlasRectangles(TextureCombinePipelineData data, bool doMultiAtlas)
        {
            return CalculateAtlasRectanglesStatic(data, doMultiAtlas);
        }

        /// <summary>
        /// 计算合并贴图 Atlas Rect 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="doMultiAtlas"></param>
        /// <returns></returns>
        public static AtlasPackingResult[] CalculateAtlasRectanglesStatic(TextureCombinePipelineData data, bool doMultiAtlas)
        {
            List<Vector2> imageSizes = new List<Vector2>();
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                imageSizes.Add(new Vector2(data.distinctMaterialTextures[i].idealWidth, data.distinctMaterialTextures[i].idealHeight));
            }

            TexturePacker tp = CreateTexturePacker(data.packingAlgorithm);
            tp.atlasMustBePowerOfTwo = data.meshBakerTexturePackerForcePowerOfTwo;

            List<AtlasPadding> paddings = new List<AtlasPadding>();
            for (int i = 0; i < imageSizes.Count; i++)
            {
                AtlasPadding padding = new AtlasPadding();
                padding.topBottom = data.atlasPadding;
                padding.leftRight = data.atlasPadding;
                if (data.packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
                    padding.leftRight = 0;
                if (data.packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
                    padding.topBottom = 0;
                paddings.Add(padding);
            }

            return tp.GetRects(imageSizes, paddings, data.maxAtlasWidth, data.maxAtlasHeight, doMultiAtlas);
        }

        internal static void CreateTemporaryTexturesForAtlas(List<MaterialPropTexturesSet> distinctMaterialTextures, 
            TextureCombineHandler combiner, 
            int propIdx,
            TextureCombinePipelineData data)
        {
            for (int texSetIdx = 0; texSetIdx < data.distinctMaterialTextures.Count; texSetIdx++)
            {
                MaterialPropTexturesSet txs = data.distinctMaterialTextures[texSetIdx];
                MaterialPropTexture matTex = txs.ts[propIdx];
                if (matTex.isNull)
                {
                    //create a small 16 x 16 texture to use in the atlas
                    Color col = data.nonTexturePropertyBlender.GetColorForTemporaryTexture(txs.matsAndGOs.mats[0].mat, data.texPropertyNames[propIdx]);
                    txs.CreateColoredTexToReplaceNull(data.texPropertyNames[propIdx].name, propIdx, data.fixOutOfBoundsUVs, combiner, col);
                }
            }
        }

        internal static TexturePacker CreateTexturePacker(PackingAlgorithmEnum _packingAlgorithm)
        {
            if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker)
            {
                return new TexturePackerRegular();
            }
            else if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
            {
                return new TexturePackerRegular();
            }
            else if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
            {
                TexturePackerHorizontalVert tp = new TexturePackerHorizontalVert();
                tp.packingOrientation = TexturePackerHorizontalVert.TexturePackingOrientation.horizontal;
                return tp;
            }
            else if (_packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
            {
                TexturePackerHorizontalVert tp = new TexturePackerHorizontalVert();
                tp.packingOrientation = TexturePackerHorizontalVert.TexturePackingOrientation.vertical;
                return tp;
            }
            else
            {
                Debug.LogError("packing algorithm must be one of the MeshBaker options to create a Texture Packer");
            }
            return null;
        }
    }
}
