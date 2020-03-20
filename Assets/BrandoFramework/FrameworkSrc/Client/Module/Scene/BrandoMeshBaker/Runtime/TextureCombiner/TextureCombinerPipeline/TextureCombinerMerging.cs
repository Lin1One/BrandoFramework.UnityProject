using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameWorld
{
    public class TextureCombinerMerging
    {
        private bool _considerNonTextureProperties = false;
        private bool fixOutOfBoundsUVs = true;
        private static bool LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS = false;

        private TextureCombinerNonTextureProperties resultMaterialTextureBlender;

        public TextureCombinerMerging(bool considerNonTextureProps, TextureCombinerNonTextureProperties resultMaterialTexBlender, bool fixObUVs)
        {
            _considerNonTextureProperties = considerNonTextureProps;
            resultMaterialTextureBlender = resultMaterialTexBlender;
            fixOutOfBoundsUVs = fixObUVs;
        }

        public static Rect BuildTransformMeshUV2AtlasRect(
            bool considerMeshUVs,
            Rect _atlasRect,
            Rect _obUVRect,
            Rect _sourceMaterialTiling,
            Rect _encapsulatingRect)
        {
            DRect atlasRect = new DRect(_atlasRect);
            DRect obUVRect;
            if (considerMeshUVs)
            {
                obUVRect = new DRect(_obUVRect); //this is the uvRect in src mesh
            }
            else
            {
                obUVRect = new DRect(0.0, 0.0, 1.0, 1.0);
            }

            DRect sourceMaterialTiling = new DRect(_sourceMaterialTiling);

            //封装材质， UV 参数
            DRect encapsulatingRectMatAndUVTiling = new DRect(_encapsulatingRect);

            DRect encapsulatingRectMatAndUVTilingInverse = UVRectUtility.InverseTransform(ref encapsulatingRectMatAndUVTiling);

            DRect toNormalizedUVs = UVRectUtility.InverseTransform(ref obUVRect);

            DRect meshFullSamplingRect = UVRectUtility.CombineTransforms(ref obUVRect, ref sourceMaterialTiling);

            DRect shiftToFitInEncapsulating = UVRectUtility.GetShiftTransformToFitBinA(ref encapsulatingRectMatAndUVTiling, ref meshFullSamplingRect);
            meshFullSamplingRect = UVRectUtility.CombineTransforms(ref meshFullSamplingRect, ref shiftToFitInEncapsulating);

            //transform between full sample rect and encapsulating rect
            DRect relativeTrans = UVRectUtility.CombineTransforms(ref meshFullSamplingRect, ref encapsulatingRectMatAndUVTilingInverse);

            DRect trans = UVRectUtility.CombineTransforms(ref toNormalizedUVs, ref relativeTrans);
            trans = UVRectUtility.CombineTransforms(ref trans, ref atlasRect);
            Rect rr = trans.GetRect();
            return rr;
        }


        public void MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(List<MaterialPropTexturesSet> distinctMaterialTextures)
        {
            int numMerged = 0;

            // IMPORTANT: Note that the verts stored in the mesh are NOT Normalized UV Coords. They are normalized * [UVTrans]. To get normalized UV
            // coords we must multiply them by [invUVTrans]. Need to do this to the verts in the mesh before we do any transforms with them.
            // Also check that all textures use same tiling. This is a prerequisite for merging.
            // Mark MB3_TexSet that are mergable (allTexturesUseSameMatTiling)
            //存储在网格中的顶点不是归一化UV坐标。 它们已归一化 * [UVTrans]。 
            //获得归一化的 UV 坐标必须将其乘以[invUVTrans]。 
            //在对它们进行任何转换之前，需要对网格中的顶点执行此操作。


            //检查所有纹理是否使用相同的拼贴。 这是合并的先决条件。
            //标记可合并的TexSet（allTexturesUseSameMatTiling）
            for (int i = 0; i < distinctMaterialTextures.Count; i++)
            {
                MaterialPropTexturesSet matPropsTextureSet = distinctMaterialTextures[i];
                int idxOfFirstNotNull = -1;
                bool allAreSame = true;
                DRect firstRect = new DRect();

                //判断不是相同的 texture 资源
                for (int propIdx = 0; propIdx < matPropsTextureSet.ts.Length; propIdx++)
                {
                    if (idxOfFirstNotNull != -1)
                    {
                        if (!matPropsTextureSet.ts[propIdx].isNull && firstRect != matPropsTextureSet.ts[propIdx].matTilingRect)
                        {
                            allAreSame = false;
                        }
                    }
                    //先找第一个非空纹理，设置为 idxOfFirstNotNull
                    else if (!matPropsTextureSet.ts[propIdx].isNull)
                    {
                        idxOfFirstNotNull = propIdx;
                        firstRect = matPropsTextureSet.ts[propIdx].matTilingRect;
                    }
                }
                if (LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS == true)
                {
                    if (allAreSame)
                    {
                        Debug.LogFormat("TextureSet {0} allTexturesUseSameMatTiling = {1}", i, allAreSame);
                    }
                    else
                    {
                        Debug.Log(string.Format("Textures in material(s) do not all use the same material tiling. " +
                            "This set of textures will not be considered for merge: {0} ", matPropsTextureSet.GetDescription()));
                    }
                }
                if (allAreSame)
                {
                    matPropsTextureSet.SetAllTexturesUseSameMatTilingTrue();
                }
            }

            //设置 材质Obj 名称
            for (int i = 0; i < distinctMaterialTextures.Count; i++)
            {
                MaterialPropTexturesSet matPropsTextureSet = distinctMaterialTextures[i];
                for (int matIdx = 0; matIdx < matPropsTextureSet.matsAndGOs.mats.Count; matIdx++)
                {
                    if (matPropsTextureSet.matsAndGOs.gos.Count > 0)
                    {
                        matPropsTextureSet.matsAndGOs.mats[matIdx].objName = matPropsTextureSet.matsAndGOs.gos[0].name;
                    }
                    else if (matPropsTextureSet.ts[0] != null)
                    {
                        matPropsTextureSet.matsAndGOs.mats[matIdx].objName = string.Format("[objWithTx:{0} atlasBlock:{1} matIdx{2}]", matPropsTextureSet.ts[0].GetTexName(), i, matIdx);
                    }
                    else
                    {
                        matPropsTextureSet.matsAndGOs.mats[matIdx].objName = string.Format("[objWithTx:{0} atlasBlock:{1} matIdx{2}]", "Unknown", i, matIdx);
                    }
                }

                matPropsTextureSet.CalcInitialFullSamplingRects(fixOutOfBoundsUVs);
                matPropsTextureSet.CalcMatAndUVSamplingRects();
            }

            // need to calculate the srcSampleRect for the complete tiling in the atlas
            // for each material need to know what the subrect would be in the atlas if material UVRect was 0,0,1,1 and Merged uvRect was full tiling
            //需要为图集中的完整切片计算src SampleRect
            //对于每种材料，如果材料UVRect为0,0,1,1并且合并的uvRect为完整平铺，则需要知道该图集在图集中的位置
            List<int> MarkedForDeletion = new List<int>();
            for (int i = 0; i < distinctMaterialTextures.Count; i++)
            {
                MaterialPropTexturesSet tx2 = distinctMaterialTextures[i];
                for (int j = i + 1; j < distinctMaterialTextures.Count; j++)
                {
                    MaterialPropTexturesSet tx1 = distinctMaterialTextures[j];
                    if (tx1.AllTexturesAreSameForMerge(tx2, _considerNonTextureProperties, resultMaterialTextureBlender))
                    {
                        double accumulatedAreaCombined = 0f;
                        double accumulatedAreaNotCombined = 0f;
                        DRect encapsulatingRectMerged = new DRect();
                        int idxOfFirstNotNull = -1;
                        for (int propIdx = 0; propIdx < tx2.ts.Length; propIdx++)
                        {
                            if (!tx2.ts[propIdx].isNull)
                            {
                                if (idxOfFirstNotNull == -1)
                                    idxOfFirstNotNull = propIdx;
                            }
                        }

                        if (idxOfFirstNotNull != -1)
                        {
                            // only in here if all properties use the same tiling so don't need to worry about which propIdx we are dealing with
                            //Get the rect that encapsulates all material and UV tiling for materials and meshes in tx1
                            //如果所有属性都使用相同的tiling，因此无需担心我们正在处理哪个propIdx
                            //获取将所有材料和UV平铺封装为rect的rect在tx1中
                            DRect encapsulatingRect1 = tx1.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
                            for (int matIdx = 1; matIdx < tx1.matsAndGOs.mats.Count; matIdx++)
                            {
                                DRect tmpSsamplingRectMatAndUVTilingTx1 = tx1.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling;
                                encapsulatingRect1 = UVRectUtility.GetEncapsulatingRectShifted(ref encapsulatingRect1, ref tmpSsamplingRectMatAndUVTilingTx1);
                            }
                            //same for tx2
                            DRect encapsulatingRect2 = tx2.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
                            for (int matIdx = 1; matIdx < tx2.matsAndGOs.mats.Count; matIdx++)
                            {
                                DRect tmpSsamplingRectMatAndUVTilingTx2 = tx2.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling;
                                encapsulatingRect2 = UVRectUtility.GetEncapsulatingRectShifted(ref encapsulatingRect2, ref tmpSsamplingRectMatAndUVTilingTx2);
                            }

                            encapsulatingRectMerged = UVRectUtility.GetEncapsulatingRectShifted(ref encapsulatingRect1, ref encapsulatingRect2);
                            accumulatedAreaCombined += encapsulatingRectMerged.width * encapsulatingRectMerged.height;
                            accumulatedAreaNotCombined += encapsulatingRect1.width * encapsulatingRect1.height + encapsulatingRect2.width * encapsulatingRect2.height;
                        }
                        else
                        {
                            encapsulatingRectMerged = new DRect(0f, 0f, 1f, 1f);
                        }

                        //the distinct material textures may overlap.
                        //if the area of these rectangles combined is less than the sum of these areas of these rectangles then merge these distinctMaterialTextures
                        if (accumulatedAreaCombined < accumulatedAreaNotCombined)
                        {
                            // merge tx2 into tx1
                            numMerged++;
                            StringBuilder sb = null;

                            sb = new StringBuilder();
                            sb.AppendFormat("About To Merge:\n   TextureSet1 {0}\n   TextureSet2 {1}\n", tx1.GetDescription(), tx2.GetDescription());

                            for (int matIdx = 0; matIdx < tx1.matsAndGOs.mats.Count; matIdx++)
                            {
                                sb.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n",
                                    tx1.matsAndGOs.mats[matIdx].mat, tx1.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling, tx1.ts[0].GetEncapsulatingSamplingRect());
                            }
                            for (int matIdx = 0; matIdx < tx2.matsAndGOs.mats.Count; matIdx++)
                            {
                                sb.AppendFormat("tx2 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n",
                                    tx2.matsAndGOs.mats[matIdx].mat, tx2.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling, tx2.ts[0].GetEncapsulatingSamplingRect());
                            }

                            //copy game objects over
                            for (int k = 0; k < tx2.matsAndGOs.gos.Count; k++)
                            {
                                if (!tx1.matsAndGOs.gos.Contains(tx2.matsAndGOs.gos[k]))
                                {
                                    tx1.matsAndGOs.gos.Add(tx2.matsAndGOs.gos[k]);
                                }
                            }

                            //copy materials over from tx2 to tx1
                            for (int matIdx = 0; matIdx < tx2.matsAndGOs.mats.Count; matIdx++)
                            {
                                tx1.matsAndGOs.mats.Add(tx2.matsAndGOs.mats[matIdx]);
                            }

                            tx1.SetEncapsulatingSamplingRectWhenMergingTexSets(encapsulatingRectMerged);
                            if (!MarkedForDeletion.Contains(i))
                            {
                                MarkedForDeletion.Add(i);
                            }

                            sb.AppendFormat("=== After Merge TextureSet {0}\n", tx1.GetDescription());
                            for (int matIdx = 0; matIdx < tx1.matsAndGOs.mats.Count; matIdx++)
                            {
                                sb.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n",
                                    tx1.matsAndGOs.mats[matIdx].mat, tx1.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling, tx1.ts[0].GetEncapsulatingSamplingRect());
                            }
                            //Integrity check that sampling rects fit into enapsulating rects
                            if (MeshBakerRoot.DO_INTEGRITY_CHECKS)
                            {
                                if (MeshBakerRoot.DO_INTEGRITY_CHECKS) { DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures); }
                            }

                            Debug.Log(sb.ToString());

                            break;


                            Debug.Log(string.Format("Considered merging {0} and {1} but there was not enough overlap. It is more efficient to bake these to separate rectangles.", tx1.GetDescription(), tx2.GetDescription()));

                        }
                    }
                }
                //remove distinctMaterialTextures that were merged
                for (int j = MarkedForDeletion.Count - 1; j >= 0; j--)
                {
                    distinctMaterialTextures.RemoveAt(MarkedForDeletion[j]);
                }
                MarkedForDeletion.Clear();

                Debug.Log(string.Format("MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects complete merged {0} now have {1}", numMerged, distinctMaterialTextures.Count));

                if (MeshBakerRoot.DO_INTEGRITY_CHECKS)
                {
                    DoIntegrityCheckMergedEncapsulatingSamplingRects(distinctMaterialTextures);
                }
            }
        }

            //进行完整性检查合并
        public void DoIntegrityCheckMergedEncapsulatingSamplingRects(List<MaterialPropTexturesSet> distinctMaterialTextures)
            {
                if (MeshBakerRoot.DO_INTEGRITY_CHECKS)
                {
                    for (int i = 0; i < distinctMaterialTextures.Count; i++)
                    {
                        MaterialPropTexturesSet tx1 = distinctMaterialTextures[i];
                        if (!tx1.allTexturesUseSameMatTiling)
                        {
                            continue;
                        }
                        for (int matIdx = 0; matIdx < tx1.matsAndGOs.mats.Count; matIdx++)
                        {
                            MatAndTransformToMerged mat = tx1.matsAndGOs.mats[matIdx];
                            DRect uvR = mat.obUVRectIfTilingSame;
                            DRect matR = mat.materialTiling;
                            //if (!MB2_TextureBakeResults.IsMeshAndMaterialRectEnclosedByAtlasRect(tx1.tilingTreatment, 
                            //    uvR.GetRect(), matR.GetRect(), tx1.ts[0].GetEncapsulatingSamplingRect().GetRect(),MB2_LogLevel.info))
                            //{
                            //    Debug.LogErrorFormat("mesh " + tx1.matsAndGOs.mats[matIdx].objName + "\n" +
                            //                        " uv=" + uvR + "\n" +
                            //                        " mat=" + matR.GetRect().ToString("f5") + "\n" +
                            //                        " samplingRect=" + tx1.matsAndGOs.mats[matIdx].samplingRectMatAndUVTiling.GetRect().ToString("f4") + "\n" +
                            //                        " encapsulatingRect " + tx1.ts[0].GetEncapsulatingSamplingRect().GetRect().ToString("f4") + "\n");
                            //    Debug.LogErrorFormat(string.Format("Integrity check failed. " + tx1.matsAndGOs.mats[matIdx].objName + " Encapsulating sampling rect failed to contain potentialRect\n"));
                            //    MB2_TextureBakeResults.IsMeshAndMaterialRectEnclosedByAtlasRect(tx1.tilingTreatment, uvR.GetRect(), matR.GetRect(), tx1.ts[0].GetEncapsulatingSamplingRect().GetRect(), MB2_LogLevel.trace);
                            //    Debug.Assert(false);
                            //}
                            //}
                        }

                    }
                }
            }
    }
}
