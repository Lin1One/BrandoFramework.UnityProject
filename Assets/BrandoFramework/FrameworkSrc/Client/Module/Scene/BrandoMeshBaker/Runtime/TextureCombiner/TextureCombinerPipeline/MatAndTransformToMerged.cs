using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    /// <summary>
    /// 材质，游戏物体映射
    /// </summary>
    public class MatsAndGOs
    {
        public List<MatAndTransformToMerged> mats;
        public List<GameObject> gos;
    }

    /// <summary>
    /// 合并 Mat 及所属 Transform 
    /// </summary>
    public class MatAndTransformToMerged
    {
        public Material mat;

        /// <summary>
        /// If considerUVs = true is set to the UV rect of the source mesh
        /// otherwise if considerUVs = false is set to 0,0,1,1
        /// </summary>
        public DRect obUVRectIfTilingSame { get; private set; }

        /// <summary>
        /// 采样的矩形 Mat 和 UV 
        /// </summary>
        public DRect samplingRectMatAndUVTiling { get; private set; }

        /// <summary>
        /// IMPORTANT: There are two materialTilingRects. These ones are stored per source material.
        /// For mapping materials to atlas rects and transforming UVs, NOT for baking atlases.    
        /// Is set to materialTiling if allTexturesUseSameMatTiling otherwise
        /// is set to 0,0,1,1
        /// </summary>
        public DRect materialTiling { get; private set; }
        public string objName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obUVrect"></param>
        /// <param name="fixOutOfBoundsUVs">是否修复越界 UV </param>
        /// <param name="m"></param>
        private void _init(DRect obUVrect, bool fixOutOfBoundsUVs, Material m)
        {
            if (fixOutOfBoundsUVs)
            {
                obUVRectIfTilingSame = obUVrect;
            }
            else
            {
                obUVRectIfTilingSame = new DRect(0, 0, 1, 1);
            }
            mat = m;
        }

        public MatAndTransformToMerged(DRect obUVrect, bool fixOutOfBoundsUVs)
        {
            _init(obUVrect, fixOutOfBoundsUVs, null);
        }

        public MatAndTransformToMerged(DRect obUVrect, bool fixOutOfBoundsUVs, Material m)
        {
            _init(obUVrect, fixOutOfBoundsUVs, m);
        }

        public override bool Equals(object obj)
        {
            if (obj is MatAndTransformToMerged)
            {
                MatAndTransformToMerged o = (MatAndTransformToMerged)obj;


                if (o.mat == mat && o.obUVRectIfTilingSame == obUVRectIfTilingSame)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return mat.GetHashCode() ^ obUVRectIfTilingSame.GetHashCode() ^ samplingRectMatAndUVTiling.GetHashCode();
        }

        public string GetMaterialName()
        {
            if (mat != null)
            {
                return mat.name;
            }
            else if (objName != null)
            {
                return string.Format("[matFor: {0}]", objName);
            }
            else
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// 为材料平铺和采样矩形和UV平铺分配初始值
        /// </summary>
        /// <param name="allTexturesUseSameMatTiling"></param>
        /// <param name="matTiling"></param>
        public void AssignInitialValuesForMaterialTilingAndSamplingRectMatAndUVTiling(bool allTexturesUseSameMatTiling, DRect matTiling)
        {
            if (allTexturesUseSameMatTiling)
            {
                materialTiling = matTiling;
            }
            else
            {
                materialTiling = new DRect(0f, 0f, 1f, 1f);
            }
            DRect tmpMatTiling = materialTiling;
            DRect obUVrect = obUVRectIfTilingSame;
            samplingRectMatAndUVTiling = UVRectUtility.CombineTransforms(ref obUVrect, ref tmpMatTiling);
        }
    }
}
