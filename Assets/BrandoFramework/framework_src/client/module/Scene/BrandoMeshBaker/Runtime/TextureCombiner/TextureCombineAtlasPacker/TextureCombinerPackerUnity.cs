using System;
using System.Collections;
using UnityEngine;

namespace GameWorld
{
    internal class MB3_TextureCombinerPackerUnity : TextureCombinerPackerBase
    {
        public override AtlasPackingResult[] CalculateAtlasRectangles(TexturePipelineData data, bool doMultiAtlas)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            //with Unity texture packer we don't find the rectangles, Unity does. When packer is run
            return new AtlasPackingResult[] { new AtlasPackingResult(new AtlasPadding[0]) };
        }

        public override IEnumerator CreateAtlases(ProgressUpdateDelegate progressInfo,
            TexturePipelineData data, 
            TextureCombineHandler combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, 
            EditorMethodsInterface textureEditorMethods)
        {
            Debug.Assert(!data.IsOnlyOneTextureInAtlasReuseTextures());
            Rect[] uvRects = packedAtlasRects.rects;

            long estArea = 0;
            int atlasSizeX = 1;
            int atlasSizeY = 1;
            uvRects = null;
            for (int propIdx = 0; propIdx < data.numAtlases; propIdx++)
            {
                //-----------------------
                ShaderTextureProperty prop = data.texPropertyNames[propIdx];
                Texture2D atlas = null;
                if (!TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))
                {
                    atlas = null;
                }
                else
                {
                    Debug.LogWarning("Beginning loop " + propIdx + " num temporary textures " + combiner._getNumTemporaryTextures());
                    TextureCombinerPackerBase.CreateTemporaryTexturesForAtlas(data.distinctMaterialTextures, combiner, propIdx, data);
                    Texture2D[] texToPack = new Texture2D[data.distinctMaterialTextures.Count];
                    for (int texSetIdx = 0; texSetIdx < data.distinctMaterialTextures.Count; texSetIdx++)
                    {
                        MaterialPropTexturesSet txs = data.distinctMaterialTextures[texSetIdx];
                        int tWidth = txs.idealWidth;
                        int tHeight = txs.idealHeight;
                        Texture2D tx = txs.ts[propIdx].GetTexture2D();
                        if (progressInfo != null)
                        {
                            progressInfo("Adjusting for scale and offset " + tx, .01f);
                        }

                        if (textureEditorMethods != null)
                        {
                            textureEditorMethods.SetReadWriteFlag(tx, true, true);
                        }

                        tx = GetAdjustedForScaleAndOffset2(prop.name, txs.ts[propIdx], txs.obUVoffset, txs.obUVscale, data, combiner);
                        //create a resized copy if necessary
                        if (tx.width != tWidth || tx.height != tHeight)
                        {
                            if (progressInfo != null) progressInfo("Resizing texture '" + tx + "'", .01f);
                            Debug.LogWarning("Copying and resizing texture " + prop.name + " from " + tx.width + "x" + tx.height + " to " + tWidth + "x" + tHeight);
                            tx = combiner._resizeTexture(prop.name, (Texture2D)tx, tWidth, tHeight);
                        }

                        estArea += tx.width * tx.height;
                        if (data._considerNonTextureProperties)
                        {
                            //combine the tintColor with the texture
                            tx = combiner._createTextureCopy(prop.name, tx);
                            data.nonTexturePropertyBlender.TintTextureWithTextureCombiner(tx, data.distinctMaterialTextures[texSetIdx], prop);
                        }

                        texToPack[texSetIdx] = tx;
                    }

                    if (textureEditorMethods != null) textureEditorMethods.CheckBuildSettings(estArea);

                    if (Math.Sqrt(estArea) > 3500f)
                    {
                        Debug.LogWarning("The maximum possible atlas size is 4096. Textures may be shrunk");
                    }

                    atlas = new Texture2D(1, 1, TextureFormat.ARGB32, true);
                    if (progressInfo != null) progressInfo("Packing texture atlas " + prop.name, .25f);
                    if (propIdx == 0)
                    {
                        if (progressInfo != null) progressInfo("Estimated min size of atlases: " + Math.Sqrt(estArea).ToString("F0"), .1f);
                        Debug.Log("Estimated atlas minimum size:" + Math.Sqrt(estArea).ToString("F0"));
                        int maxAtlasSize = 4096;
                        uvRects = atlas.PackTextures(texToPack, data._atlasPadding, maxAtlasSize, false);
                        Debug.Log("After pack textures atlas size " + atlas.width + " " + atlas.height);
                        atlasSizeX = atlas.width;
                        atlasSizeY = atlas.height;
                        atlas.Apply();
                    }
                    else
                    {
                        if (progressInfo != null) progressInfo("Copying Textures Into: " + prop.name, .1f);
                        atlas = _copyTexturesIntoAtlas(texToPack, data._atlasPadding, uvRects, atlasSizeX, atlasSizeY, combiner);
                    }
                }

                atlases[propIdx] = atlas;
                //----------------------

                if (data._saveAtlasesAsAssets && textureEditorMethods != null)
                {
                    textureEditorMethods.SaveAtlasToAssetDatabase(atlases[propIdx], prop, propIdx, data.resultMaterial);
                }
                data.resultMaterial.SetTextureOffset(prop.name, Vector2.zero);
                data.resultMaterial.SetTextureScale(prop.name, Vector2.one);
                combiner._destroyTemporaryTextures(prop.name);
                GC.Collect();
            }
            packedAtlasRects.rects = uvRects;
            yield break;
        }

        internal static Texture2D _copyTexturesIntoAtlas(Texture2D[] texToPack, int padding, Rect[] rs, int w, int h, TextureCombineHandler combiner)
        {
            Texture2D ta = new Texture2D(w, h, TextureFormat.ARGB32, true);
            MeshBakerUtility.setSolidColor(ta, Color.clear);
            for (int i = 0; i < rs.Length; i++)
            {
                Rect r = rs[i];
                Texture2D t = texToPack[i];
                Texture2D tmpTex = null;
                int x = Mathf.RoundToInt(r.x * w);
                int y = Mathf.RoundToInt(r.y * h);
                int ww = Mathf.RoundToInt(r.width * w);
                int hh = Mathf.RoundToInt(r.height * h);
                if (t.width != ww && t.height != hh)
                {
                    tmpTex = t = MeshBakerUtility.resampleTexture(t, ww, hh);
                }
                ta.SetPixels(x, y, ww, hh, t.GetPixels());
                if (tmpTex != null) MeshBakerUtility.Destroy(tmpTex);
            }
            ta.Apply();
            return ta;
        }

        // used by Unity texture packer to handle tiled textures.
        // may create a new texture that has the correct tiling to handle fix out of bounds UVs
        internal static Texture2D GetAdjustedForScaleAndOffset2(string propertyName, 
            MaterialPropTexture source, 
            Vector2 obUVoffset, 
            Vector2 obUVscale, 
            TexturePipelineData data, 
            TextureCombineHandler combiner)
        {
            Texture2D sourceTex = source.GetTexture2D();
            if (source.matTilingRect.x == 0f && source.matTilingRect.y == 0f && source.matTilingRect.width == 1f && source.matTilingRect.height == 1f)
            {
                if (data._fixOutOfBoundsUVs)
                {
                    if (obUVoffset.x == 0f && obUVoffset.y == 0f && obUVscale.x == 1f && obUVscale.y == 1f)
                    {
                        return sourceTex; //no adjustment necessary
                    }
                }
                else
                {
                    return sourceTex; //no adjustment necessary
                }
            }
            Vector2 dim = TextureCombinerPipeline.GetAdjustedForScaleAndOffset2Dimensions(source, obUVoffset, obUVscale, data);

            Debug.LogWarning("GetAdjustedForScaleAndOffset2: " + sourceTex + " " + obUVoffset + " " + obUVscale);
            float newWidth = dim.x;
            float newHeight = dim.y;
            float scx = (float)source.matTilingRect.width;
            float scy = (float)source.matTilingRect.height;
            float ox = (float)source.matTilingRect.x;
            float oy = (float)source.matTilingRect.y;
            if (data._fixOutOfBoundsUVs)
            {
                scx *= obUVscale.x;
                scy *= obUVscale.y;
                ox = (float)(source.matTilingRect.x * obUVscale.x + obUVoffset.x);
                oy = (float)(source.matTilingRect.y * obUVscale.y + obUVoffset.y);
            }
            Texture2D newTex = combiner._createTemporaryTexture(propertyName, (int)newWidth, (int)newHeight, TextureFormat.ARGB32, true);
            for (int i = 0; i < newTex.width; i++)
            {
                for (int j = 0; j < newTex.height; j++)
                {
                    float u = i / newWidth * scx + ox;
                    float v = j / newHeight * scy + oy;
                    newTex.SetPixel(i, j, sourceTex.GetPixelBilinear(u, v));
                }
            }
            newTex.Apply();
            return newTex;
        }
    }
}
