using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace GameWorld
{
    /// <summary>
    /// Used internally during the material baking process
    /// 相同shader ，不同材质在 Atals 的对应映射Rect 信息
    /// shader 有多个属性对应多个 texture ，则有多个 Atlas，在各个 Atlas 映射 Rect 相同
    /// </summary>
    [Serializable]
    public class AtlasesAndRects
    {
        /// <summary>
        /// 合并材质 Atlas 列表
        /// </summary>
        public Texture2D[] atlases;
        /// <summary>
        /// 源材质与合并材质 Atlas 的 Rect 映射关系列表（在各 Atlas 都相同）
        /// </summary>
        [NonSerialized]
        public List<MaterialAndUVRect> originMatToRect_map;
        /// <summary>
        /// shader 属性数组
        /// </summary>
        public string[] texPropertyNames;   
    }

    [System.Serializable]
    public class MaterialAndUVRect
    {
        /// <summary>
        /// The source material that was baked into the atlas.
        /// </summary>
        public Material material;

        /// <summary>
        /// The rectangle in the atlas where the texture (including all tiling) was copied to.
        /// </summary>
        public Rect atlasRect;

        /// <summary>
        /// For debugging. The name of the first srcObj that uses this MaterialAndUVRect.
        /// </summary>
        public string srcObjName;

        public bool allPropsUseSameTiling = true;

        /// <summary>
        /// Only valid if allPropsUseSameTiling = true. Else should be 0,0,0,0
        /// The material tiling on the source material
        /// </summary>
        [FormerlySerializedAs("sourceMaterialTiling")]
        public Rect allPropsUseSameTiling_sourceMaterialTiling;

        /// <summary>
        /// Only valid if allPropsUseSameTiling = true. Else should be 0,0,0,0
        /// The encapsulating sampling rect that was used to sample for the atlas. Note that the case
        /// of dont-considerMeshUVs is the same as do-considerMeshUVs where the uv rect is 0,0,1,1 
        /// </summary>
        [FormerlySerializedAs("samplingEncapsulatinRect")]
        public Rect allPropsUseSameTiling_samplingEncapsulatinRect;

        /// <summary>
        /// Only valid if allPropsUseSameTiling = false.
        /// The UVrect of the source mesh that was baked. We are using a trick here.
        /// Instead of storing the material tiling for each
        /// texture property here, we instead bake all those tilings into the atlases and here we pretend
        /// that all those tilings were 0,0,1,1. Then all we need is to store is the 
        /// srcUVsamplingRect
        /// </summary>
        public Rect propsUseDifferntTiling_srcUVsamplingRect;

        /// <summary>
        /// The tilling type for this rectangle in the atlas.
        /// </summary>
        public TextureTilingTreatment tilingTreatment = TextureTilingTreatment.unknown;

        /// <param name="mat">The Material</param>
        /// <param name="destRect">The rect in the atlas this material maps to</param>
        /// <param name="allPropsUseSameTiling">If true then use sourceMaterialTiling and samplingEncapsulatingRect.
        /// if false then use srcUVsamplingRect. None used values should be 0,0,0,0.</param>
        /// <param name="sourceMaterialTiling">allPropsUseSameTiling_sourceMaterialTiling</param>
        /// <param name="samplingEncapsulatingRect">allPropsUseSameTiling_samplingEncapsulatinRect</param>
        /// <param name="srcUVsamplingRect">propsUseDifferntTiling_srcUVsamplingRect</param>
        public MaterialAndUVRect(Material mat,
            Rect destRect,
            bool allPropsUseSameTiling,
            Rect sourceMaterialTiling,
            Rect samplingEncapsulatingRect,
            Rect srcUVsamplingRect,
            TextureTilingTreatment treatment,
            string objName)
        {
            if (allPropsUseSameTiling)
            {
                Debug.Assert(srcUVsamplingRect == new Rect(0, 0, 0, 0));
            }

            if (!allPropsUseSameTiling)
            {
                Debug.Assert(samplingEncapsulatingRect == new Rect(0, 0, 0, 0));
                Debug.Assert(sourceMaterialTiling == new Rect(0, 0, 0, 0));
            }

            material = mat;
            atlasRect = destRect;
            tilingTreatment = treatment;
            this.allPropsUseSameTiling = allPropsUseSameTiling;
            allPropsUseSameTiling_sourceMaterialTiling = sourceMaterialTiling;
            allPropsUseSameTiling_samplingEncapsulatinRect = samplingEncapsulatingRect;
            propsUseDifferntTiling_srcUVsamplingRect = srcUVsamplingRect;
            srcObjName = objName;
        }

        public override int GetHashCode()
        {
            return material.GetInstanceID() ^ allPropsUseSameTiling_samplingEncapsulatinRect.GetHashCode() ^ propsUseDifferntTiling_srcUVsamplingRect.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MaterialAndUVRect)) return false;
            MaterialAndUVRect b = (MaterialAndUVRect)obj;
            return material == b.material &&
                allPropsUseSameTiling_samplingEncapsulatinRect == b.allPropsUseSameTiling_samplingEncapsulatinRect &&
                allPropsUseSameTiling_sourceMaterialTiling == b.allPropsUseSameTiling_sourceMaterialTiling &&
                allPropsUseSameTiling == b.allPropsUseSameTiling &&
                propsUseDifferntTiling_srcUVsamplingRect == b.propsUseDifferntTiling_srcUVsamplingRect;
        }

        public Rect GetEncapsulatingRect()
        {
            if (allPropsUseSameTiling)
            {
                return allPropsUseSameTiling_samplingEncapsulatinRect;
            }
            else
            {
                return propsUseDifferntTiling_srcUVsamplingRect;
            }
        }

        public Rect GetMaterialTilingRect()
        {
            if (allPropsUseSameTiling)
            {
                return allPropsUseSameTiling_sourceMaterialTiling;
            }
            else
            {
                return new Rect(0, 0, 1, 1);
            }
        }
    }
}