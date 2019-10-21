using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// A set of textures one for each "maintex","bump" that one or more materials use. These
    /// Will be baked into a rectangle in the atlas.
    /// 一组纹理针对一种或多种材料使用的每个“主纹理”，“凹凸”使用一个。 这些
    /// 将在Altas中烘焙成矩形。
    /// </summary>
    public class MaterialPropTexturesSet
    {
        /// <summary>
        /// One per "maintex", "bump".
        /// Stores encapsulatingSamplingRect (can be different for maintex, bump...)
        /// Stores materialTiling for mapping materials to atlas rects and transforming UVs not for baking atlases
        /// 
        /// </summary>
        public MaterialPropTexture[] ts;

        public MatsAndGOs matsAndGOs;

        public bool allTexturesUseSameMatTiling { get; private set; }

        public bool thisIsOnlyTexSetInAtlas { get; private set; }

        public TextureTilingTreatment tilingTreatment { get; private set; }

        public Vector2 obUVoffset { get; private set; }
        public Vector2 obUVscale { get; private set; }
        public int idealWidth; //all textures will be resized to this size
        public int idealHeight;

        /// <summary>
        /// 处理方式
        /// </summary>
        private PipelineVariation pipelineVariation;

        internal DRect obUVrect
        {
            get { return new DRect(obUVoffset, obUVscale); }
        }

        public MaterialPropTexturesSet(MaterialPropTexture[] tss, Vector2 uvOffset, Vector2 uvScale, TextureTilingTreatment treatment)
        {
            ts = tss;
            tilingTreatment = treatment;
            obUVoffset = uvOffset;
            obUVscale = uvScale;
            allTexturesUseSameMatTiling = false;
            thisIsOnlyTexSetInAtlas = false;
            matsAndGOs = new MatsAndGOs();
            matsAndGOs.mats = new List<MatAndTransformToMerged>();
            matsAndGOs.gos = new List<GameObject>();
            pipelineVariation = new PipelineVariationSomeTexturesUseDifferentMatTiling(this);
        }

        // The two texture sets are equal if they are using the same 
        // textures/color properties for each map and have the same
        // tiling for each of those color properties
        internal bool IsEqual(object obj, bool fixOutOfBoundsUVs, TextureCombinerNonTextureProperties resultMaterialTextureBlender)
        {
            if (!(obj is MaterialPropTexturesSet))
            {
                return false;
            }
            MaterialPropTexturesSet other = (MaterialPropTexturesSet)obj;
            if (other.ts.Length != ts.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i].matTilingRect != other.ts[i].matTilingRect)
                        return false;
                    if (!ts[i].AreTexturesEqual(other.ts[i]))
                        return false;

                    if (!resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
                    {
                        return false;
                    }
                }

                //IMPORTANT don't use Vector2 != Vector2 because it is only acurate to about 5 decimal places
                //this can lead to tiled rectangles that can't accept rectangles.
                if (fixOutOfBoundsUVs && (obUVoffset.x != other.obUVoffset.x ||
                                            obUVoffset.y != other.obUVoffset.y))
                    return false;
                if (fixOutOfBoundsUVs && (obUVscale.x != other.obUVscale.x ||
                                            obUVscale.y != other.obUVscale.y))
                    return false;
                return true;
            }
        }

        public Vector2 GetMaxRawTextureHeightWidth()
        {
            Vector2 max = new Vector2(0, 0);
            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                MaterialPropTexture tx = ts[propIdx];
                if (!tx.isNull)
                {
                    max.x = Mathf.Max(max.x, tx.width);
                    max.y = Mathf.Max(max.y, tx.height);
                }
            }
            return max;
        }

        public Rect GetEncapsulatingSamplingRectIfTilingSame()
        {
            Debug.Assert(allTexturesUseSameMatTiling, "This should never be called if different properties use different tiling. ");
            if (ts.Length > 0)
            {
                return ts[0].GetEncapsulatingSamplingRect().GetRect();
            }
            return new Rect(0, 0, 1, 1);
        }

        public void SetEncapsulatingSamplingRectWhenMergingTexSets(DRect newEncapsulatingSamplingRect)
        {
            Debug.Assert(allTexturesUseSameMatTiling, "This should never be called if different properties use different tiling. ");
            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                ts[propIdx].SetEncapsulatingSamplingRect(this, newEncapsulatingSamplingRect);
            }
        }

        public void SetEncapsulatingSamplingRectForTesting(int propIdx, DRect newEncapsulatingSamplingRect)
        {
            ts[propIdx].SetEncapsulatingSamplingRect(this, newEncapsulatingSamplingRect);
        }

        public void SetEncapsulatingRect(int propIdx, bool considerMeshUVs)
        {
            if (considerMeshUVs)
            {
                ts[propIdx].SetEncapsulatingSamplingRect(this, obUVrect);
            }
            else
            {
                ts[propIdx].SetEncapsulatingSamplingRect(this, new DRect(0, 0, 1, 1));
            }
        }

        public void CreateColoredTexToReplaceNull(string propName, int propIdx, bool considerMeshUVs, TextureCombineHandler combiner, Color col)
        {
            MaterialPropTexture matTex = ts[propIdx];
            matTex.t = combiner._createTemporaryTexture(propName, 16, 16, TextureFormat.ARGB32, true);
            MeshBakerUtility.setSolidColor(matTex.GetTexture2D(), col);
        }

        public void SetThisIsOnlyTexSetInAtlasTrue()
        {
            Debug.Assert(thisIsOnlyTexSetInAtlas == false);
            thisIsOnlyTexSetInAtlas = true;
        }

        public void SetAllTexturesUseSameMatTilingTrue()
        {
            Debug.Assert(allTexturesUseSameMatTiling == false);
            allTexturesUseSameMatTiling = true;
            pipelineVariation = new PipelineVariationAllTexturesUseSameMatTiling(this);
        }

        public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
        {
            pipelineVariation.AdjustResultMaterialNonTextureProperties(resultMaterial, props);
        }

        public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(TextureTilingTreatment newTilingTreatment)
        {
            tilingTreatment = newTilingTreatment;
            pipelineVariation.SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(newTilingTreatment);
        }


        internal void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect,
                                                    out Rect propsUseDifferntTiling_obUVRect)
        {
            pipelineVariation.GetRectsForTextureBakeResults(out allPropsUseSameTiling_encapsulatingSamplingRect, out propsUseDifferntTiling_obUVRect);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialIndex">Should be an index in matsAndGOs.mats List</param>
        /// <returns></returns>
        internal Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
        {
            return pipelineVariation.GetMaterialTilingRectForTextureBakerResults(materialIndex);
        }

        //assumes all materials use the same obUVrects.
        //假设所有材料都使用相同的obUVrects。
        internal void CalcInitialFullSamplingRects(bool fixOutOfBoundsUVs)
        {
            DRect validFullSamplingRect = new DRect(0, 0, 1, 1);
            if (fixOutOfBoundsUVs)
            {
                validFullSamplingRect = obUVrect;
            }

            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                if (!ts[propIdx].isNull)
                {
                    DRect matTiling = ts[propIdx].matTilingRect;
                    DRect ruv;
                    if (fixOutOfBoundsUVs)
                    {
                        ruv = obUVrect;
                    }
                    else
                    {
                        ruv = new DRect(0.0, 0.0, 1.0, 1.0);
                    }

                    ts[propIdx].SetEncapsulatingSamplingRect(this, UVRectUtility.CombineTransforms(ref ruv, ref matTiling));
                    validFullSamplingRect = ts[propIdx].GetEncapsulatingSamplingRect();
                }
            }

            //if some of the textures were null make them match the sampling of one of the other textures
            for (int propIdx = 0; propIdx < ts.Length; propIdx++)
            {
                if (ts[propIdx].isNull)
                {
                    ts[propIdx].SetEncapsulatingSamplingRect(this, validFullSamplingRect);
                }
            }
        }

        internal void CalcMatAndUVSamplingRects()
        {
            DRect matTiling = new DRect(0f, 0f, 1f, 1f);
            if (allTexturesUseSameMatTiling)
            {
                for (int propIdx = 0; propIdx < ts.Length; propIdx++)
                {
                    if (!ts[propIdx].isNull)
                    {
                        matTiling = ts[propIdx].matTilingRect;
                        break;
                    }
                }
            }

            for (int matIdx = 0; matIdx < matsAndGOs.mats.Count; matIdx++)
            {
                matsAndGOs.mats[matIdx].AssignInitialValuesForMaterialTilingAndSamplingRectMatAndUVTiling(allTexturesUseSameMatTiling, matTiling);
            }
        }

        public bool AllTexturesAreSameForMerge(MaterialPropTexturesSet other, bool considerNonTextureProperties, TextureCombinerNonTextureProperties resultMaterialTextureBlender)
        {
            if (other.ts.Length != ts.Length)
            {
                return false;
            }
            else
            {
                if (!other.allTexturesUseSameMatTiling || !allTexturesUseSameMatTiling)
                {
                    return false;
                }
                // must use same set of textures
                int idxOfFirstNoneNull = -1;
                for (int i = 0; i < ts.Length; i++)
                {
                    if (!ts[i].AreTexturesEqual(other.ts[i]))
                        return false;
                    if (idxOfFirstNoneNull == -1 && !ts[i].isNull)
                    {
                        idxOfFirstNoneNull = i;
                    }
                    if (considerNonTextureProperties)
                    {
                        if (!resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
                        {
                            return false;
                        }
                    }
                }
                if (idxOfFirstNoneNull != -1)
                {
                    //check that all textures are the same. Have already checked all tiling is same
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (!ts[i].AreTexturesEqual(other.ts[i]))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public void DrawRectsToMergeGizmos(Color encC, Color innerC)
        {
            DRect r = ts[0].GetEncapsulatingSamplingRect();
            r.Expand(.05f);
            Gizmos.color = encC;
            Gizmos.DrawWireCube(r.center.GetVector2(), r.size);
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                DRect rr = matsAndGOs.mats[i].samplingRectMatAndUVTiling;
                DRect trans = UVRectUtility.GetShiftTransformToFitBinA(ref r, ref rr);
                Vector2 xy = UVRectUtility.TransformPoint(ref trans, rr.min);
                rr.x = xy.x;
                rr.y = xy.y;
                //Debug.Log("r " + r + " rr" + rr);
                Gizmos.color = innerC;
                Gizmos.DrawWireCube(rr.center.GetVector2(), rr.size);
            }
        }

        internal string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[GAME_OBJS=");
            for (int i = 0; i < matsAndGOs.gos.Count; i++)
            {
                sb.AppendFormat("{0},", matsAndGOs.gos[i].name);
            }
            sb.AppendFormat("MATS=");
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                sb.AppendFormat("{0},", matsAndGOs.mats[i].GetMaterialName());
            }
            sb.Append("]");
            return sb.ToString();
        }

        internal string GetMatSubrectDescriptions()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < matsAndGOs.mats.Count; i++)
            {
                sb.AppendFormat("\n    {0}={1},", matsAndGOs.mats[i].GetMaterialName(), matsAndGOs.mats[i].samplingRectMatAndUVTiling);
            }
            return sb.ToString();
        }
    }

}
