
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public abstract class MeshBakerRoot:MonoBehaviour
    {
        public static bool DO_INTEGRITY_CHECKS = false;
        [HideInInspector]
        public abstract TextureBakeResults textureBakeResults { get; set; }

        public Vector3 sortAxis;

        /**
         * Transparent shaders often require objects to be sorted along 
         */
        public class ZSortObjects
        {
            public Vector3 sortAxis;
            public class Item
            {
                public GameObject go;
                public Vector3 point;
            }

            public class ItemComparer : IComparer<Item>
            {
                public int Compare(Item a, Item b)
                {
                    return (int)Mathf.Sign(b.point.z - a.point.z);
                }
            }

            public void SortByDistanceAlongAxis(List<GameObject> gos)
            {
                if (sortAxis == Vector3.zero)
                {
                    Debug.LogError("The sort axis cannot be the zero vector.");
                    return;
                }
                Debug.Log("Z sorting meshes along axis numObjs=" + gos.Count);
                List<Item> items = new List<Item>();
                Quaternion q = Quaternion.FromToRotation(sortAxis, Vector3.forward);
                for (int i = 0; i < gos.Count; i++)
                {
                    if (gos[i] != null)
                    {
                        Item item = new Item();
                        item.point = gos[i].transform.position;
                        item.go = gos[i];
                        item.point = q * item.point;
                        items.Add(item);
                    }
                }
                items.Sort(new ItemComparer());

                for (int i = 0; i < gos.Count; i++)
                {
                    gos[i] = items[i].go;
                }
            }
        }
        //网格合并验证
        public static bool DoCombinedValidate(MeshBakerRoot mom, ObjsToCombineTypes objToCombineType, EditorMethodsInterface editorMethods, ValidationLevel validationLevel)
        {
            if (mom.textureBakeResults == null)
            {
                Debug.LogError("Need to set Texture Bake Result on " + mom);
                return false;
            }
            if (mom is MeshBakerCommon)
            {
                //MB3_MeshBakerCommon momMB = (MB3_MeshBakerCommon)mom;
                //MB3_TextureBaker tb = momMB.GetTextureBaker();
                //if (tb != null && tb.textureBakeResults != mom.textureBakeResults)
                //{
                //    Debug.LogWarning("Texture Bake Result on this component is not the same as the Texture Bake Result on the MB3_TextureBaker.");
                //}
            }
            //网格分析结果缓存
            Dictionary<int, MeshAnalysisResult> meshAnalysisResultCache = null;

            if (validationLevel == ValidationLevel.robust)
            {
                meshAnalysisResultCache = new Dictionary<int, MeshAnalysisResult>();
            }

            //获取将合并网格的游戏物体
            List<GameObject> objsToMesh = mom.GetObjectsToCombine();

            for (int i = 0; i < objsToMesh.Count; i++)
            {
                GameObject go = objsToMesh[i];
                if (go == null)
                {
                    Debug.LogError("合并网格游戏物体列表中包含 null 物体，在位置" + i);
                    return false;
                }
                for (int j = i + 1; j < objsToMesh.Count; j++)
                {
                    if (objsToMesh[i] == objsToMesh[j])
                    {
                        Debug.LogError("合并网格游戏物体列表中包含重复游戏物体 " + i + " 和 " + j);
                        return false;
                    }
                }
                if (MeshBakerUtility.GetGOMaterials(go).Length == 0)
                {
                    Debug.LogError("游戏物体 " + go + " 没有材质");
                    return false;
                }
                Mesh m = MeshBakerUtility.GetMesh(go);
                if (m == null)
                {
                    Debug.LogError("合并网格游戏物体列表中， " + go + " 没有网格 ");
                    return false;
                }
                if (m != null)
                {
                    //This check can be very expensive and it only warns so only do this if we are in the editor.
                    if (!Application.isEditor && Application.isPlaying && mom.textureBakeResults.doMultiMaterial &&
                        validationLevel >= ValidationLevel.robust)
                    {
                        MeshAnalysisResult mar;
                        if (!meshAnalysisResultCache.TryGetValue(m.GetInstanceID(), out mar))
                        {
                            MeshBakerUtility.doSubmeshesShareVertsOrTris(m, ref mar);
                            meshAnalysisResultCache.Add(m.GetInstanceID(), mar);
                        }
                        //检查重叠的子网格顶点
                        if (mar.hasOverlappingSubmeshVerts)
                        {
                            Debug.LogWarning("游戏物体 " + objsToMesh[i] + " has overlapping submeshes (submeshes share vertices)." +
                                "If the UVs associated with the shared vertices are important then this bake may not work. " +
                                "If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures " +
                                "(each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, " +
                                "available in the asset store can fix overlapping submeshes.");
                        }
                    }
                }
            }


            List<GameObject> objs = objsToMesh;

            if (mom is MeshCombinerEntrance)
            {
                objs = mom.GetObjectsToCombine();
                if (objs == null || objs.Count == 0)
                {
                    Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
                    return false;
                }

                //skinned 网格验证
                //if (mom is MB3_MeshBaker && ((MB3_MeshBaker)mom).meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer)
                //{
                //    if (!editorMethods.ValidateSkinnedMeshes(objs))
                //    {
                //        return false;
                //    }
                //}
            }

            if (editorMethods != null)
            {
                editorMethods.CheckPrefabTypes(objToCombineType, objsToMesh);
            }
            return true;
        }

        //todo switch this to List<Renderer>
        public virtual List<GameObject> GetObjectsToCombine()
        {
            return null;
        }
    }
}

