using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// There is different handing of how things are baked into atlases depending on:
    ///   do all TexturesUseSameMaterialTiling
    ///   are the textures edge to edge.
    /// We try to capture those differences a clearly defined way.
    /// </summary>
    ///将事物烘焙为图集的方式有所不同，具体取决于：
    ///     是否所有TexturesUseSameMaterialTiling
    ///     边缘到边缘的纹理。
    ///我们尝试以明确定义的方式捕获这些差异。
    public interface PipelineVariation
    {
        void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect,
                                           out Rect propsUseDifferntTiling_obUVRect);

        /// <summary>
        /// 设置平铺处理方式，调整封装采样矩形
        /// </summary>
        /// <param name="newTilingTreatment"></param>
        void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(TextureTilingTreatment newTilingTreatment);

        /// <summary>
        /// 获取材质平铺矩形
        /// </summary>
        /// <param name="materialIndex"></param>
        /// <returns></returns>
        Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex);


        /// <summary>
        /// 调整结果材质，非纹理属性
        /// </summary>
        /// <param name="resultMaterial"></param>
        /// <param name="props"></param>
        void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props);
    }

    /// <summary>
    /// 所有texture 使用相同材质球平铺方式
    /// </summary>
    public class PipelineVariationAllTexturesUseSameMatTiling : PipelineVariation
    {
        private MaterialPropTexturesSet texSet;

        public PipelineVariationAllTexturesUseSameMatTiling(MaterialPropTexturesSet ts)
        {
            texSet = ts;
            Debug.Assert(texSet.allTexturesUseSameMatTiling == true);
        }

        public void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect,
                                                    out Rect propsUseDifferntTiling_obUVRect)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == true);
            propsUseDifferntTiling_obUVRect = new Rect(0, 0, 0, 0);
            allPropsUseSameTiling_encapsulatingSamplingRect = texSet.GetEncapsulatingSamplingRectIfTilingSame();
            //adjust for tilingTreatment
            if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeX)
            {
                allPropsUseSameTiling_encapsulatingSamplingRect.x = 0;
                allPropsUseSameTiling_encapsulatingSamplingRect.width = 1;
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeY)
            {
                allPropsUseSameTiling_encapsulatingSamplingRect.y = 0;
                allPropsUseSameTiling_encapsulatingSamplingRect.height = 1;
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeXY)
            {
                allPropsUseSameTiling_encapsulatingSamplingRect = new Rect(0, 0, 1, 1);
            }
        }

        public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(TextureTilingTreatment newTilingTreatment)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == true);
            if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeX)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.width = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeY)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.height = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeXY)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.height = 1;
                    r.width = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
        }

        public Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == true);
            return texSet.matsAndGOs.mats[materialIndex].materialTiling.GetRect();
        }

        public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == true);
        }
    }

    public class PipelineVariationSomeTexturesUseDifferentMatTiling : PipelineVariation
    {
        private MaterialPropTexturesSet texSet;

        public PipelineVariationSomeTexturesUseDifferentMatTiling(MaterialPropTexturesSet ts)
        {
            texSet = ts;
            Debug.Assert(texSet.allTexturesUseSameMatTiling == false);
        }

        public void GetRectsForTextureBakeResults(out Rect allPropsUseSameTiling_encapsulatingSamplingRect,
                                                    out Rect propsUseDifferntTiling_obUVRect)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == false);
            allPropsUseSameTiling_encapsulatingSamplingRect = new Rect(0, 0, 0, 0);
            propsUseDifferntTiling_obUVRect = texSet.obUVrect.GetRect();
            //adjust for tilingTreatment
            if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeX)
            {
                propsUseDifferntTiling_obUVRect.x = 0;
                propsUseDifferntTiling_obUVRect.width = 1;
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeY)
            {
                propsUseDifferntTiling_obUVRect.y = 0;
                propsUseDifferntTiling_obUVRect.height = 1;
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeXY)
            {
                propsUseDifferntTiling_obUVRect = new Rect(0, 0, 1, 1);
            }
        }

        public void SetTilingTreatmentAndAdjustEncapsulatingSamplingRect(TextureTilingTreatment newTilingTreatment)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == false);
            if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeX)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.width = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeY)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.height = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
            else if (texSet.tilingTreatment == TextureTilingTreatment.edgeToEdgeXY)
            {
                foreach (MaterialPropTexture t in texSet.ts)
                {
                    DRect r = t.GetEncapsulatingSamplingRect();
                    r.height = 1;
                    r.width = 1;
                    t.SetEncapsulatingSamplingRect(texSet, r);
                }
            }
        }

        public Rect GetMaterialTilingRectForTextureBakerResults(int materialIndex)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == false);
            return new Rect(0, 0, 0, 0);
        }

        public void AdjustResultMaterialNonTextureProperties(Material resultMaterial, List<ShaderTextureProperty> props)
        {
            Debug.Assert(texSet.allTexturesUseSameMatTiling == false);
            if (texSet.thisIsOnlyTexSetInAtlas)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    if (resultMaterial.HasProperty(props[i].name))
                    {

                        resultMaterial.SetTextureOffset(props[i].name, texSet.ts[i].matTilingRect.min);
                        resultMaterial.SetTextureScale(props[i].name, texSet.ts[i].matTilingRect.size);
                    }
                }
            }
        }
    }

}
