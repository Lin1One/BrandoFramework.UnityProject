using System;
using System.Collections;
using UnityEngine;

namespace GameWorld
{
    internal class TextureCombinerPackerMeshBakerFast : ITextureCombinerPacker
    {
        public IEnumerator ConvertTexturesToReadableFormats(ProgressUpdateDelegate progressInfo,
            CombineTexturesIntoAtlasesCoroutineResult result,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            EditorMethodsInterface textureEditorMethods)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            yield break;
        }

        public virtual AtlasPackingResult[] CalculateAtlasRectangles(TexturePipelineData data, bool doMultiAtlas)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            return TextureCombinerPackerBase.CalculateAtlasRectanglesStatic(data, doMultiAtlas);
        }

        public IEnumerator CreateAtlases(ProgressUpdateDelegate progressInfo,
            TexturePipelineData data,
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            Rect[] uvRects = packedAtlasRects.rects;

            int atlasSizeX = packedAtlasRects.atlasX;
            int atlasSizeY = packedAtlasRects.atlasY;
            Debug.Log("Generated atlas will be " + atlasSizeX + "x" + atlasSizeY);

            //create a game object
            GameObject renderAtlasesGO = null;
            try
            {
                renderAtlasesGO = new GameObject("MBrenderAtlasesGO");
                AtlasPackerRenderTexture atlasRenderTexture = renderAtlasesGO.AddComponent<AtlasPackerRenderTexture>();
                renderAtlasesGO.AddComponent<Camera>();
                if (data._considerNonTextureProperties)
                    Debug.LogWarning("Blend Non-Texture Properties has limited functionality when used with Mesh Baker Texture Packer Fast.");

                for (int propIdx = 0; propIdx < data.numAtlases; propIdx++)
                {
                    Texture2D atlas = null;
                    if (!TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                    {
                        atlas = null;
                        Debug.Log("Not creating atlas for " + data.texPropertyNames[propIdx].name + " because textures are null and default value parameters are the same.");
                    }
                    else
                    {
                        GC.Collect();

                        TextureCombinerPackerBase.CreateTemporaryTexturesForAtlas(data.distinctMaterialTextures, combiner, propIdx, data);

                        if (progressInfo != null)
                            progressInfo("Creating Atlas '" + data.texPropertyNames[propIdx].name + "'", .01f);
                        // ===========
                        // configure it
                        Debug.Log("About to render " + data.texPropertyNames[propIdx].name + " isNormal=" + data.texPropertyNames[propIdx].isNormalMap);
                        atlasRenderTexture.width = atlasSizeX;
                        atlasRenderTexture.height = atlasSizeY;
                        atlasRenderTexture.padding = data._atlasPadding;
                        atlasRenderTexture.rects = uvRects;
                        atlasRenderTexture.textureSets = data.distinctMaterialTextures;
                        atlasRenderTexture.indexOfTexSetToRender = propIdx;
                        atlasRenderTexture.texPropertyName = data.texPropertyNames[propIdx];
                        atlasRenderTexture.isNormalMap = data.texPropertyNames[propIdx].isNormalMap;
                        atlasRenderTexture.fixOutOfBoundsUVs = data._fixOutOfBoundsUVs;
                        atlasRenderTexture.considerNonTextureProperties = data._considerNonTextureProperties;
                        atlasRenderTexture.resultMaterialTextureBlender = data.nonTexturePropertyBlender;
                        // call render on it
                        atlas = atlasRenderTexture.OnRenderAtlas(combiner);

                        // destroy it
                        // =============
                        Debug.Log("Saving atlas " + data.texPropertyNames[propIdx].name + " w=" + atlas.width + " h=" + atlas.height + " id=" + atlas.GetInstanceID());
                    }
                    atlases[propIdx] = atlas;
                    if (progressInfo != null)
                        progressInfo("Saving atlas: '" + data.texPropertyNames[propIdx].name + "'", .04f);
                    if (data._saveAtlasesAsAssets && textureEditorMethods != null)
                    {
                        textureEditorMethods.SaveAtlasToAssetDatabase(atlases[propIdx], data.texPropertyNames[propIdx], propIdx, data.resultMaterial);
                    }
                    else
                    {
                        data.resultMaterial.SetTexture(data.texPropertyNames[propIdx].name, atlases[propIdx]);
                    }
                    data.resultMaterial.SetTextureOffset(data.texPropertyNames[propIdx].name, Vector2.zero);
                    data.resultMaterial.SetTextureScale(data.texPropertyNames[propIdx].name, Vector2.one);
                    combiner._destroyTemporaryTextures(data.texPropertyNames[propIdx].name); // need to save atlases before doing this				
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError(ex);
                Debug.LogException(ex);
            }
            finally
            {
                if (renderAtlasesGO != null)
                {
                    MeshBakerUtility.Destroy(renderAtlasesGO);
                }
            }
            yield break;
        }
    }
}
