using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameWorld
{
    /// <summary>
    /// 创建贴图 Atlas 协程结果
    /// </summary>
    public class CreateAtlasesCoroutineResult
    {
        public bool success = true;
        public bool isFinished = false;
    }

    /// <summary>
    /// 合并贴图至图集结果
    /// </summary>
    public class CombineTexturesIntoAtlasesCoroutineResult
    {
        public bool success = true;
        public bool isFinished = false;
    }

    /// <summary>
    /// This class stores the results from an MB2_TextureBaker when materials are combined into atlases. It stores
    /// a list of materials and the corresponding UV rectangles in the atlases. It also stores the configuration
    /// options that were used to generate the combined material.
    /// 
    /// It can be saved as an asset in the project so that textures can be baked in one scene and used in another.
    /// 当将材质合并为图集时，此类存储 TextureBaker 的结果，
    /// 存储 Atlas 中的材质列表和相应的UV矩形。 
    /// 存储配置用于生成合并材料的选项。
    ///
    /// 可将其另存为项目中的资产，以便可以在一个场景中烘焙纹理并在另一个场景中使用它。
    /// </summary>
    public class TextureBakeResults : ScriptableObject
    {
        /// <summary>
        /// 合并材质数组
        /// </summary>
        public MultiMaterial[] resultMaterials;

        /// <summary>
        /// 源材质在合并材质 Atlas 的 Rect 映射数组（在各prop 的 Atlas 都相同）
        /// </summary>
        public MaterialAndUVRect[] materialsAndUVRects;
        
        public bool doMultiMaterial;

        private void OnEnable()
        {
            for (int i = 0; i < materialsAndUVRects.Length; i++)
            {
                materialsAndUVRects[i].allPropsUseSameTiling = true;
            }
        }

        /// <summary>
        /// Creates for materials on renderer.
        /// 生成 TextureBakeResult，如果要合并的所有游戏物体都使用相同的材质，则可以使用该 TextureBakeResult。
        /// 将游戏物体的渲染器所使用的所有材质映射到 Atlas 中的矩形0,0..1,1 中。
        /// 用于创建临时 TextureCombineResult，Mesh 合并时，TextureCombineResult 为空时调用
        /// </summary>
        public static TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos, List<Material> matsOnTargetRenderer)
        {
            HashSet<Material> fullMaterialList = new HashSet<Material>(matsOnTargetRenderer);
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i] == null)
                {
                    Debug.LogError(string.Format("列表中第 {0} 个游戏物体为空", i));
                    return null;
                }
                Material[] oMats = MeshBakerUtility.GetGOMaterials(gos[i]);
                if (oMats.Length == 0)
                {
                    Debug.LogError(string.Format("列表中第 {0} 个游戏物体没有 renderer 组件", i));
                    return null;
                }
                for (int j = 0; j < oMats.Length; j++)
                {
                    if (!fullMaterialList.Contains(oMats[j]))
                    {
                        fullMaterialList.Add(oMats[j]);
                    }
                }
            }
            //所有游戏位图的源材质
            Material[] rms = new Material[fullMaterialList.Count];
            fullMaterialList.CopyTo(rms);

            TextureBakeResults textureCombineResult = (TextureBakeResults)CreateInstance(typeof(TextureBakeResults));
            List<MaterialAndUVRect> sourceMatUVRects = new List<MaterialAndUVRect>();
            for (int i = 0; i < rms.Length; i++)
            {
                if (rms[i] != null)
                {
                    MaterialAndUVRect matAndUVRect = new MaterialAndUVRect(rms[i], 
                        new Rect(0f, 0f, 1f, 1f), 
                        true, 
                        new Rect(0f, 0f, 1f, 1f), 
                        new Rect(0f, 0f, 1f, 1f), 
                        new Rect(0, 0, 0, 0), 
                        TextureTilingTreatment.none, 
                        "");
                    if (!sourceMatUVRects.Contains(matAndUVRect))
                    {
                        sourceMatUVRects.Add(matAndUVRect);
                    }
                }
            }

            textureCombineResult.resultMaterials = new MultiMaterial[sourceMatUVRects.Count];
            for (int i = 0; i < sourceMatUVRects.Count; i++)
            {
                textureCombineResult.resultMaterials[i] = new MultiMaterial();
                List<Material> sourceMats = new List<Material>();
                sourceMats.Add(sourceMatUVRects[i].material);
                textureCombineResult.resultMaterials[i].sourceMaterials = sourceMats;
                textureCombineResult.resultMaterials[i].combinedMaterial = sourceMatUVRects[i].material;
                textureCombineResult.resultMaterials[i].considerMeshUVs = false;
            }
            if (rms.Length == 1)
            {
                textureCombineResult.doMultiMaterial = false;
            }
            else
            {
                textureCombineResult.doMultiMaterial = true;
            }

            textureCombineResult.materialsAndUVRects = sourceMatUVRects.ToArray();
            return textureCombineResult;
        }

        public bool DoAnyResultMatsUseConsiderMeshUVs()
        {
            if (resultMaterials == null)
                return false;
            for (int i = 0; i < resultMaterials.Length; i++)
            {
                if (resultMaterials[i].considerMeshUVs)
                    return true;
            }
            return false;
        }

        public bool ContainsMaterial(Material m)
        {
            for (int i = 0; i < materialsAndUVRects.Length; i++)
            {
                if (materialsAndUVRects[i].material == m)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取材质合并后描述（Inspector）
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            //shader 信息
            sb.Append("Shaders:\n");
            HashSet<Shader> shaders = new HashSet<Shader>();
            if (materialsAndUVRects != null)
            {
                for (int i = 0; i < materialsAndUVRects.Length; i++)
                {
                    if (materialsAndUVRects[i].material != null)
                    {
                        shaders.Add(materialsAndUVRects[i].material.shader);
                    }
                }
            }

            foreach (Shader m in shaders)
            {
                sb.Append("  ").Append(m.name).AppendLine();
            }

            //材质信息
            sb.Append("Materials:\n");
            if (materialsAndUVRects != null)
            {
                for (int i = 0; i < materialsAndUVRects.Length; i++)
                {
                    if (materialsAndUVRects[i].material != null)
                    {
                        sb.Append("  ").Append(materialsAndUVRects[i].material.name).AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
    }
}