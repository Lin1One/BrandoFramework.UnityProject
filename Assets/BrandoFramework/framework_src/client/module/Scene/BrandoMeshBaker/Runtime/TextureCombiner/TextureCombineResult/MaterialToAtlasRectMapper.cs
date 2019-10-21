using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// 材质，图集矩形映射
    /// 作用：根据源游戏物体的材质找到其在合并材质资源中的 Rect 映射
    /// </summary>
    public class MaterialToAtlasRectMapper
    {
        TextureBakeResults resultAsset;
        int[] numTimesMatAppearsInAtlas;
        MaterialAndUVRect[] matsAndSrcUVRect;

        public MaterialToAtlasRectMapper(TextureBakeResults res)
        {
            resultAsset = res;
            matsAndSrcUVRect = res.materialsAndUVRects;

            //count the number of times a material appears in the atlas. used for fast lookup
            numTimesMatAppearsInAtlas = new int[matsAndSrcUVRect.Length];
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (numTimesMatAppearsInAtlas[i] > 1)
                {
                    continue;
                }
                int count = 1;
                for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                {
                    if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                    {
                        count++;
                    }
                }
                numTimesMatAppearsInAtlas[i] = count;
                if (count > 1)
                {
                    for (int j = i + 1; j < matsAndSrcUVRect.Length; j++)
                    {
                        if (matsAndSrcUVRect[i].material == matsAndSrcUVRect[j].material)
                        {
                            numTimesMatAppearsInAtlas[j] = count;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A material can appear more than once in an atlas if using fixOutOfBoundsUVs.
        /// in this case you need to use the UV rect of the mesh to find the correct rectangle.
        /// If the all properties on the mat use the same tiling then 
        /// encapsulatingRect can be larger and will include baked UV and material tiling
        /// If mat uses different tiling for different maps then encapsulatingRect is the uvs of
        /// source mesh used to bake atlas and sourceMaterialTilingOut is 0,0,1,1. This works because
        /// material tiling was baked into the atlas.
        /// 尝试获取源物体材质在合并材质中的映射信息
        /// 如果使用fixOutOfBoundsUVs，一个材质可以在图集中出现多次。在这种情况下，您需要使用网格的UV矩形来找到正确的矩形。
        /// 如果材质上的所有属性都使用相同的拼贴，则 encapsulatingRect 可以更大，并将包含烘焙的UV和材质平铺
        /// 如果mat 对不同映射使用不同的tiling，则 encapsulatingRect 是用于烘焙图集的 uvs 且 sourceMaterialTilingOut 为0,0,1,1。 
        /// 材质 tiling 烘焙到 atlas 中。
        /// </summary>
        public bool TryGetMaterialToUVRectMap(Material sourceMat,
            Mesh sourceMesh,
            int submeshIdx,
            int idxInResultMats,
            MeshChannelsCache meshChannelCache,
            Dictionary<int, MeshAnalysisResult[]> meshAnalysisCache,
            out TextureTilingTreatment tilingTreatment,
            out Rect rectInAtlas,
            out Rect encapsulatingRectOut,
            out Rect sourceMaterialTilingOut,
            ref string errorMsg)
        {
            for (int i = 0; i < resultAsset.materialsAndUVRects.Length; i++)
            {
                resultAsset.materialsAndUVRects[i].allPropsUseSameTiling = true;
            }

            tilingTreatment = TextureTilingTreatment.unknown;
            if (resultAsset.materialsAndUVRects.Length == 0)
            {
                errorMsg = "Texture Bake Result 资源中的 材质UVRect 映射信息为空，需重新合并";
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            if (sourceMat == null)
            {
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = string.Format("网格 {0} 的子网格 {1} 缺少材质，无法获取映射关系", sourceMesh.name, submeshIdx);
                return false;
            }
            if (submeshIdx >= sourceMesh.subMeshCount)
            {
                errorMsg = "参数错误 ：Submesh index 大于子网格数量";
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }

            //源材质在 matsAndSrcUVRect 的 ID
            int idx = -1;
            for (int i = 0; i < matsAndSrcUVRect.Length; i++)
            {
                if (sourceMat == matsAndSrcUVRect[i].material)
                {
                    idx = i;
                    break;
                }
            }
            if (idx == -1)
            {
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                errorMsg = string.Format("Material {0} 在 Texture Bake Result 中无法找到", sourceMat.name);
                return false;
            }

            //不处理网格 UVs
            if (!resultAsset.resultMaterials[idxInResultMats].considerMeshUVs)
            {
                if (numTimesMatAppearsInAtlas[idx] != 1)
                {
                    Debug.LogError("TextureBakeResults 资源错误，FixOutOfBoundsUVs is false and a material appears more than once.");
                }
                MaterialAndUVRect mr = matsAndSrcUVRect[idx];
                rectInAtlas = mr.atlasRect;
                tilingTreatment = mr.tilingTreatment;
                encapsulatingRectOut = mr.GetEncapsulatingRect();
                sourceMaterialTilingOut = mr.GetMaterialTilingRect();
                return true;
            }
            else
            {
                //todo what if no UVs
                //Find UV rect in source mesh
                //源网格分析，并缓存
                MeshAnalysisResult[] meshAnalysisInfo;
                if (!meshAnalysisCache.TryGetValue(sourceMesh.GetInstanceID(), out meshAnalysisInfo))
                {
                    meshAnalysisInfo = new MeshAnalysisResult[sourceMesh.subMeshCount];
                    for (int j = 0; j < sourceMesh.subMeshCount; j++)
                    {
                        Vector2[] uvss = meshChannelCache.GetUv0Raw(sourceMesh);
                        MeshBakerUtility.hasOutOfBoundsUVs(uvss, sourceMesh, ref meshAnalysisInfo[j], j);
                    }
                    meshAnalysisCache.Add(sourceMesh.GetInstanceID(), meshAnalysisInfo);
                }

                //this could be a mesh that was not used in the texture baking that has huge UV tiling too big for the rect that was baked
                //find a record that has an atlas uvRect capable of containing this
                //这可能是未在纹理烘焙中使用的网格，该网格的UV贴图对于烘焙的rect而言太大
                //找到一条记录，该记录具有能够包含此图集的uvRect
                bool found = false;
                Rect encapsulatingRect = new Rect(0, 0, 0, 0);
                Rect sourceMaterialTiling = new Rect(0, 0, 0, 0);

                //Debug.Log(string.Format("尝试在图集中查找能够使用材质{1}保持网格{0}的平铺采样rect的矩形", 
                //    m, sourceMat, meshAnalysisInfo[submeshIdx].uvRect.ToString("f5")));

                for (int i = idx; i < matsAndSrcUVRect.Length; i++)
                {
                    MaterialAndUVRect matAndUVrect = matsAndSrcUVRect[i];
                    if (matAndUVrect.material == sourceMat)
                    {
                        if (matAndUVrect.allPropsUseSameTiling)
                        {
                            encapsulatingRect = matAndUVrect.allPropsUseSameTiling_samplingEncapsulatinRect;
                            sourceMaterialTiling = matAndUVrect.allPropsUseSameTiling_sourceMaterialTiling;
                        }
                        else
                        {
                            encapsulatingRect = matAndUVrect.propsUseDifferntTiling_srcUVsamplingRect;
                            sourceMaterialTiling = new Rect(0, 0, 1, 1);
                        }

                        if (UVRectUtility.IsMeshAndMaterialRectEnclosedByAtlasRect(
                                matAndUVrect.tilingTreatment,
                                meshAnalysisInfo[submeshIdx].uvRect,
                                sourceMaterialTiling,
                                encapsulatingRect))
                        {
                            Debug.Log("在图集中找到" + "ID 为 " + i  + "包含" + sourceMesh + "  的 tiled sampling 的 Rect");
                            idx = i;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                    MaterialAndUVRect mr = matsAndSrcUVRect[idx];
                    rectInAtlas = mr.atlasRect;
                    tilingTreatment = mr.tilingTreatment;
                    encapsulatingRectOut = mr.GetEncapsulatingRect();
                    sourceMaterialTilingOut = mr.GetMaterialTilingRect();
                    return true;
                }
                else
                {
                    rectInAtlas = new Rect();
                    encapsulatingRectOut = new Rect();
                    sourceMaterialTilingOut = new Rect();
                    errorMsg = string.Format("Could not find a tiled rectangle in the atlas capable of containing the uv and material tiling on mesh {0} for material {1}. " +
                        "Was this mesh included when atlases were baked?", sourceMesh.name, sourceMat);
                    return false;
                }
            }
        }
    }
}