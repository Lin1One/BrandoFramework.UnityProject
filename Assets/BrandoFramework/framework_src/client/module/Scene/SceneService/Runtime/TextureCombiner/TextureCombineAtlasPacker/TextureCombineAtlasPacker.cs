using GameWorld;
using System;
using System.Collections;
using UnityEngine;

namespace  Client.Scene
{
    internal class TextureCombineAtlasPacker : TextureCombineAtlasPackerBase
    {
        public override void CreateAtlases(
            TextureCombinePipelineData data, 
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods)
        {
            Rect[] uvRects = packedAtlasRects.rects;

            int atlasSizeX = packedAtlasRects.atlasX;
            int atlasSizeY = packedAtlasRects.atlasY;
            Debug.Log("Generated atlas will be " + atlasSizeX + "x" + atlasSizeY);

            for (int propIdx = 0; propIdx < data.numAtlases; propIdx++)
            {
                Texture2D atlas = null;
                ShaderTextureProperty property = data.texPropertyNames[propIdx];
                if (!TextureCombinePipeline.ShouldWeCreateAtlasForThisProperty(propIdx, data.considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                {
                    atlas = null;
                    Debug.Log("=== Not creating atlas for " + property.name + " because textures are null and default value parameters are the same.");
                }
                else
                {
                    Debug.Log("=== Creating atlas for " + property.name);
                    GC.Collect();
                    CreateTemporaryTexturesForAtlas(data.distinctMaterialTextures, combiner, propIdx, data);

                    //use a jagged array because it is much more efficient in memory
                    Color[][] atlasPixels = new Color[atlasSizeY][];
                    for (int j = 0; j < atlasPixels.Length; j++)
                    {
                        atlasPixels[j] = new Color[atlasSizeX];
                    }

                    bool isNormalMap = false;
                    if (property.isNormalMap)
                        isNormalMap = true;

                    for (int texSetIdx = 0; texSetIdx < data.distinctMaterialTextures.Count; texSetIdx++)
                    {
                        MaterialPropTexturesSet texSet = data.distinctMaterialTextures[texSetIdx];
                        MaterialPropTexture matTex = texSet.ts[propIdx];
                        string s = "Creating Atlas '" + property.name + "' texture " + matTex.GetTexName();

                        Debug.Log(string.Format("Adding texture {0} to atlas {1} for texSet {2} srcMat {3}", matTex.GetTexName(), property.name, texSetIdx, texSet.matsAndGOs.mats[0].GetMaterialName()));

                        Rect r = uvRects[texSetIdx];
                        Texture2D t = texSet.ts[propIdx].GetTexture2D();
                        int x = Mathf.RoundToInt(r.x * atlasSizeX);
                        int y = Mathf.RoundToInt(r.y * atlasSizeY);
                        int ww = Mathf.RoundToInt(r.width * atlasSizeX);
                        int hh = Mathf.RoundToInt(r.height * atlasSizeY);
                        if (ww == 0 || hh == 0)
                            Debug.LogError("Image in atlas has no height or width " + r);

                        if (textureEditorMethods != null)
                            textureEditorMethods.SetReadWriteFlag(t, true, true);
                        DRect samplingRect = texSet.ts[propIdx].GetEncapsulatingSamplingRect();
                        CopyScaledAndTiledToAtlas(texSet.ts[propIdx], 
                            texSet, 
                            property, 
                            samplingRect, 
                            x, 
                            y, 
                            ww, 
                            hh, 
                            packedAtlasRects.padding[texSetIdx], 
                            atlasPixels, 
                            isNormalMap, 
                            data, 
                            combiner);
                    }

                    atlas = new Texture2D(atlasSizeX, atlasSizeY, TextureFormat.ARGB32, true);
                    for (int j = 0; j < atlasPixels.Length; j++)
                    {
                        atlas.SetPixels(0, j, atlasSizeX, 1, atlasPixels[j]);
                    }

                    atlas.Apply();
                    Debug.Log("Saving atlas " + property.name + " w=" + atlas.width + " h=" + atlas.height);
                }

                atlases[propIdx] = atlas;

                if (data.saveAtlasesAsAssets && textureEditorMethods != null)
                {
                    textureEditorMethods.SaveAtlasToAssetDatabase(atlases[propIdx], data.texPropertyNames[propIdx], propIdx, data.ResultMaterial);
                }
                else
                {
                    data.ResultMaterial.SetTexture(data.texPropertyNames[propIdx].name, atlases[propIdx]);
                }

                data.ResultMaterial.SetTextureOffset(data.texPropertyNames[propIdx].name, Vector2.zero);
                data.ResultMaterial.SetTextureScale(data.texPropertyNames[propIdx].name, Vector2.one);
                combiner._destroyTemporaryTextures(data.texPropertyNames[propIdx].name);
            }
        }


        internal static void CopyScaledAndTiledToAtlas(MaterialPropTexture source, 
            MaterialPropTexturesSet sourceMaterial,
            ShaderTextureProperty shaderPropertyName, 
            DRect srcSamplingRect, 
            int targX, int targY, int targW, int targH,
            AtlasPadding padding,
            Color[][] atlasPixels, bool isNormalMap,
            TextureCombinePipelineData data,
            TextureCombineHandler combiner)
        {
            //HasFinished = false;
            Texture2D t = source.GetTexture2D();
            Debug.Log(string.Format("CopyScaledAndTiledToAtlas: {0} inAtlasX={1} inAtlasY={2} inAtlasW={3} inAtlasH={4} paddX={5} paddY={6} srcSamplingRect={7}", 
                t, targX, targY, targW, targH, padding.leftRight, padding.topBottom, srcSamplingRect));
            
            float newWidth = targW;
            float newHeight = targH;
            float scx = (float)srcSamplingRect.width;
            float scy = (float)srcSamplingRect.height;
            float ox = (float)srcSamplingRect.x;
            float oy = (float)srcSamplingRect.y;
            int w = (int)newWidth;
            int h = (int)newHeight;
            if (data.considerNonTextureProperties)
            {
                t = combiner._createTextureCopy(shaderPropertyName.name, t);
                t = data.nonTexturePropertyBlender.TintTextureWithTextureCombiner(t, sourceMaterial, shaderPropertyName);
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float u = i / newWidth * scx + ox;
                    float v = j / newHeight * scy + oy;
                    atlasPixels[targY + j][targX + i] = t.GetPixelBilinear(u, v);
                }
            }

            //bleed the border colors into the padding
            for (int i = 0; i < w; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    //top margin
                    atlasPixels[(targY - j)][targX + i] = atlasPixels[(targY)][targX + i];
                    //bottom margin
                    atlasPixels[(targY + h - 1 + j)][targX + i] = atlasPixels[(targY + h - 1)][targX + i];
                }
            }
            for (int j = 0; j < h; j++)
            {
                for (int i = 1; i <= padding.leftRight; i++)
                {
                    //left margin
                    atlasPixels[(targY + j)][targX - i] = atlasPixels[(targY + j)][targX];
                    //right margin
                    atlasPixels[(targY + j)][targX + w + i - 1] = atlasPixels[(targY + j)][targX + w - 1];
                }
            }
            //corners
            for (int i = 1; i <= padding.leftRight; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    atlasPixels[(targY - j)][targX - i] = atlasPixels[targY][targX];
                    atlasPixels[(targY + h - 1 + j)][targX - i] = atlasPixels[(targY + h - 1)][targX];
                    atlasPixels[(targY + h - 1 + j)][targX + w + i - 1] = atlasPixels[(targY + h - 1)][targX + w - 1];
                    atlasPixels[(targY - j)][targX + w + i - 1] = atlasPixels[targY][targX + w - 1];
                }
            }
        }
    }
}
