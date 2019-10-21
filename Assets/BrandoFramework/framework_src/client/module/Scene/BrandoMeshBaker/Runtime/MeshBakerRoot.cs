
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
        //����ϲ���֤
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
            //��������������
            Dictionary<int, MeshAnalysisResult> meshAnalysisResultCache = null;

            if (validationLevel == ValidationLevel.robust)
            {
                meshAnalysisResultCache = new Dictionary<int, MeshAnalysisResult>();
            }

            //��ȡ���ϲ��������Ϸ����
            List<GameObject> objsToMesh = mom.GetObjectsToCombine();

            for (int i = 0; i < objsToMesh.Count; i++)
            {
                GameObject go = objsToMesh[i];
                if (go == null)
                {
                    Debug.LogError("�ϲ�������Ϸ�����б��а��� null ���壬��λ��" + i);
                    return false;
                }
                for (int j = i + 1; j < objsToMesh.Count; j++)
                {
                    if (objsToMesh[i] == objsToMesh[j])
                    {
                        Debug.LogError("�ϲ�������Ϸ�����б��а����ظ���Ϸ���� " + i + " �� " + j);
                        return false;
                    }
                }
                if (MeshBakerUtility.GetGOMaterials(go).Length == 0)
                {
                    Debug.LogError("��Ϸ���� " + go + " û�в���");
                    return false;
                }
                Mesh m = MeshBakerUtility.GetMesh(go);
                if (m == null)
                {
                    Debug.LogError("�ϲ�������Ϸ�����б��У� " + go + " û������ ");
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
                        //����ص��������񶥵�
                        if (mar.hasOverlappingSubmeshVerts)
                        {
                            Debug.LogWarning("��Ϸ���� " + objsToMesh[i] + " has overlapping submeshes (submeshes share vertices)." +
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

                //skinned ������֤
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

