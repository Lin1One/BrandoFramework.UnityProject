using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// ���ʣ�ͼ������ӳ��
    /// ���ã�����Դ��Ϸ����Ĳ����ҵ����ںϲ�������Դ�е� Rect ӳ��
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
        /// ���Ի�ȡԴ��������ںϲ������е�ӳ����Ϣ
        /// ���ʹ��fixOutOfBoundsUVs��һ�����ʿ�����ͼ���г��ֶ�Ρ�����������£�����Ҫʹ�������UV�������ҵ���ȷ�ľ��Ρ�
        /// ��������ϵ��������Զ�ʹ����ͬ��ƴ������ encapsulatingRect ���Ը��󣬲��������決��UV�Ͳ���ƽ��
        /// ���mat �Բ�ͬӳ��ʹ�ò�ͬ��tiling���� encapsulatingRect �����ں決ͼ���� uvs �� sourceMaterialTilingOut Ϊ0,0,1,1�� 
        /// ���� tiling �決�� atlas �С�
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
                errorMsg = "Texture Bake Result ��Դ�е� ����UVRect ӳ����ϢΪ�գ������ºϲ�";
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
                errorMsg = string.Format("���� {0} �������� {1} ȱ�ٲ��ʣ��޷���ȡӳ���ϵ", sourceMesh.name, submeshIdx);
                return false;
            }
            if (submeshIdx >= sourceMesh.subMeshCount)
            {
                errorMsg = "�������� ��Submesh index ��������������";
                rectInAtlas = new Rect();
                encapsulatingRectOut = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }

            //Դ������ matsAndSrcUVRect �� ID
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
                errorMsg = string.Format("Material {0} �� Texture Bake Result ���޷��ҵ�", sourceMat.name);
                return false;
            }

            //���������� UVs
            if (!resultAsset.resultMaterials[idxInResultMats].considerMeshUVs)
            {
                if (numTimesMatAppearsInAtlas[idx] != 1)
                {
                    Debug.LogError("TextureBakeResults ��Դ����FixOutOfBoundsUVs is false and a material appears more than once.");
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
                //Դ���������������
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
                //�������δ������決��ʹ�õ����񣬸������UV��ͼ���ں決��rect����̫��
                //�ҵ�һ����¼���ü�¼�����ܹ�������ͼ����uvRect
                bool found = false;
                Rect encapsulatingRect = new Rect(0, 0, 0, 0);
                Rect sourceMaterialTiling = new Rect(0, 0, 0, 0);

                //Debug.Log(string.Format("������ͼ���в����ܹ�ʹ�ò���{1}��������{0}��ƽ�̲���rect�ľ���", 
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
                            Debug.Log("��ͼ�����ҵ�" + "ID Ϊ " + i  + "����" + sourceMesh + "  �� tiled sampling �� Rect");
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