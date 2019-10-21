using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameWorld
{
    internal abstract class TextureCombinerPackerBase : ITextureCombinerPacker
    {
        public virtual IEnumerator ConvertTexturesToReadableFormats(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            EditorMethodsInterface textureEditorMethods)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            //MakeProceduralTexturesReadable(progressInfo, result, data, combiner, textureEditorMethods, LOG_LEVEL);
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
                            if (progressInfo != null) progressInfo(String.Format("Convert texture {0} to readable format ", tx), .5f);
                            textureEditorMethods.AddTextureFormat((Texture2D)tx, data.texPropertyNames[j].isNormalMap);
                        }
                    }
                }
            }
            yield break;
        }

        public virtual AtlasPackingResult[] CalculateAtlasRectangles(TexturePipelineData data, bool doMultiAtlas)
        {
            return CalculateAtlasRectanglesStatic(data, doMultiAtlas);
        }

        public abstract IEnumerator CreateAtlases(ProgressUpdateDelegate progressInfo,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods);

        internal static void CreateTemporaryTexturesForAtlas(List<MaterialPropTexturesSet> distinctMaterialTextures, TextureCombineHandler combiner, int propIdx, TexturePipelineData data)
        {
            for (int texSetIdx = 0; texSetIdx < data.distinctMaterialTextures.Count; texSetIdx++)
            {
                MaterialPropTexturesSet txs = data.distinctMaterialTextures[texSetIdx];
                MaterialPropTexture matTex = txs.ts[propIdx];
                if (matTex.isNull)
                {
                    //create a small 16 x 16 texture to use in the atlas
                    Color col = data.nonTexturePropertyBlender.GetColorForTemporaryTexture(txs.matsAndGOs.mats[0].mat, data.texPropertyNames[propIdx]);
                    txs.CreateColoredTexToReplaceNull(data.texPropertyNames[propIdx].name, propIdx, data._fixOutOfBoundsUVs, combiner, col);
                }
            }
        }

        /// <summary>
        /// 计算合并贴图 Atlas Rect 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="doMultiAtlas"></param>
        /// <returns></returns>
        public static AtlasPackingResult[] CalculateAtlasRectanglesStatic(TexturePipelineData data, bool doMultiAtlas)
        {
            List<Vector2> imageSizes = new List<Vector2>();
            for (int i = 0; i < data.distinctMaterialTextures.Count; i++)
            {
                imageSizes.Add(new Vector2(data.distinctMaterialTextures[i].idealWidth, data.distinctMaterialTextures[i].idealHeight));
            }

            TexturePacker tp = CreateTexturePacker(data._packingAlgorithm);
            tp.atlasMustBePowerOfTwo = data._meshBakerTexturePackerForcePowerOfTwo;

            List<AtlasPadding> paddings = new List<AtlasPadding>();
            for (int i = 0; i < imageSizes.Count; i++)
            {
                AtlasPadding padding = new AtlasPadding();
                padding.topBottom = data._atlasPadding;
                padding.leftRight = data._atlasPadding;
                if (data._packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal)
                    padding.leftRight = 0;
                if (data._packingAlgorithm == PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical)
                    padding.topBottom = 0;
                paddings.Add(padding);
            }

            return tp.GetRects(imageSizes, paddings, data._maxAtlasWidth, data._maxAtlasHeight, doMultiAtlas);
        }

        public static void MakeProceduralTexturesReadable(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            EditorMethodsInterface textureEditorMethods)
        {
            Debug.LogError("TODO this should be done as close to textures being used as possible due to memory issues.");
            //make procedural materials readable
            /*
            for (int i = 0; i < combiner._proceduralMaterials.Count; i++)
            {
                if (!combiner._proceduralMaterials[i].proceduralMat.isReadable)
                {
                    combiner._proceduralMaterials[i].originalIsReadableVal = combiner._proceduralMaterials[i].proceduralMat.isReadable;
                    combiner._proceduralMaterials[i].proceduralMat.isReadable = true;
                    //textureEditorMethods.AddProceduralMaterialFormat(_proceduralMaterials[i].proceduralMat);
                    combiner._proceduralMaterials[i].proceduralMat.RebuildTexturesImmediately();
                }
            }
            //convert procedural textures to RAW format
            
            for (int i = 0; i < distinctMaterialTextures.Count; i++)
            {
                for (int j = 0; j < texPropertyNames.Count; j++)
                {
                    if (distinctMaterialTextures[i].ts[j].IsProceduralTexture())
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Converting procedural texture to Textur2D:" + distinctMaterialTextures[i].ts[j].GetTexName() + " property:" + texPropertyNames[i]);
                        Texture2D txx = distinctMaterialTextures[i].ts[j].ConvertProceduralToTexture2D(_temporaryTextures);
                        distinctMaterialTextures[i].ts[j].t = txx;
                    }
                }
            }
            */
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
