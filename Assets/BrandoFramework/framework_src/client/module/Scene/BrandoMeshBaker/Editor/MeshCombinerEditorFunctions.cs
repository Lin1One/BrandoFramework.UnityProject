using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameWorld.Editor
{
    public class MeshCombinerEditorFunctions
    {
        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static bool BakeIntoCombined(MeshBakerCommon mom, out bool createdDummyTextureBakeResults)
        {
            SerializedObject so = null;
            return BakeIntoCombined(mom, out createdDummyTextureBakeResults, ref so);
        }

        /// <summary>
        ///  Bakes a combined mesh.
        ///  ����Ǵ�Inspector�����е��õģ�����������SerializedObject
        ///  ���ڡ��決��Ԥ�ơ����ܻ���SerializedObject�����Ǳ����
        /// </summary>
        public static bool BakeIntoCombined(MeshBakerCommon mom, out bool createdDummyTextureBakeResults, ref SerializedObject so)
        {
            OutputOptions prefabOrSceneObject = mom.meshCombiner.outputOption;
            createdDummyTextureBakeResults = false;
            if (prefabOrSceneObject != OutputOptions.bakeIntoPrefab && prefabOrSceneObject != OutputOptions.bakeIntoSceneObject)
            {
                Debug.LogError("Paramater prefabOrSceneObject must be bakeIntoPrefab or bakeIntoSceneObject");
                return false;
            }

            //�Ӹ������� ��ͼ�ϲ����������ͼ�ϲ���� Asset
            TextureCombineEntrance tb = mom.GetComponentInParent<TextureCombineEntrance>();
            if (mom.textureBakeResults == null && tb != null)
            {
                mom.textureBakeResults = tb.textureBakeResults;
            }
            //��ͼ�ϲ����Ϊ��ʱ���򴴽�
            if (mom.textureBakeResults == null)
            {
                if (_OkToCreateDummyTextureBakeResult(mom))
                {
                    createdDummyTextureBakeResults = true;
                    List<GameObject> gos = mom.GetObjectsToCombine();
                    if (mom.GetNumObjectsInCombined() > 0)
                    {
                        if (mom.clearBuffersAfterBake)
                        {
                            mom.ClearMesh();
                        }
                        else
                        {
                            Debug.LogError("'Texture Bake Result' must be set to add more objects to a combined mesh that " +
                                "already contains objects. Try enabling 'clear buffers after bake'");
                            return false;
                        }
                    }
                    mom.textureBakeResults = TextureBakeResults.CreateForMaterialsOnRenderer(
                        gos.ToArray(), 
                        mom.meshCombiner.GetMaterialsOnTargetRenderer());
                    Debug.Log("'Texture Bake Result' was not set. " +
                        "Creating a temporary one. Each material will be mapped to a separate submesh.");
                }
            }

            //�ϲ����
            ValidationLevel vl = Application.isPlaying ? ValidationLevel.quick : ValidationLevel.robust;
            if (!MeshBakerRoot.DoCombinedValidate(mom, ObjsToCombineTypes.sceneObjOnly, new EditorMethods(), vl))
            {
                return false;
            }

            //����Ԥ������Դ�Ƿ��Ѵ���
            if (prefabOrSceneObject == OutputOptions.bakeIntoPrefab &&
                    mom.resultPrefab == null)
            {
                Debug.LogError("Need to set the Combined Mesh Prefab field. " +
                    "Create a prefab asset, drag an empty game object into it, and drag it to the 'Combined Mesh Prefab' field.");
                return false;
            }

            if (mom.meshCombiner.resultSceneObject != null &&
                (SceneBakerUtilityInEditor.GetPrefabType(mom.meshCombiner.resultSceneObject) == PrefabType.modelPrefab ||
                 SceneBakerUtilityInEditor.GetPrefabType(mom.meshCombiner.resultSceneObject) == PrefabType.prefab))
            {
                Debug.LogWarning("Result Game Object was a project asset not a scene object instance. Clearing this field.");
                mom.meshCombiner.resultSceneObject = null;
            }

            mom.ClearMesh();

            //�ϲ�
            if (mom.AddDeleteGameObjects(mom.GetObjectsToCombine().ToArray(), null, false))
            {
                mom.Apply(UnwrapUV2);
                if (createdDummyTextureBakeResults)
                {
                    //��ʱ�ϲ�����ͼ
                    Debug.Log(String.Format("Successfully baked {0} meshes each material is mapped to its own submesh.",
                        mom.GetObjectsToCombine().Count));
                }
                else
                {
                    Debug.Log(String.Format("Successfully baked {0} meshes", mom.GetObjectsToCombine().Count));
                }


                if (prefabOrSceneObject == OutputOptions.bakeIntoSceneObject)
                {
                    PrefabType pt = SceneBakerUtilityInEditor.GetPrefabType(mom.meshCombiner.resultSceneObject);
                    if (pt == PrefabType.prefab || pt == PrefabType.modelPrefab)
                    {
                        Debug.LogError("Combined Mesh Object is a prefab asset. " +
                            "If output option bakeIntoSceneObject then this must be an instance in the scene.");
                        return false;
                    }
                }
                else if (prefabOrSceneObject == OutputOptions.bakeIntoPrefab)
                {
                    string prefabPth = AssetDatabase.GetAssetPath(mom.resultPrefab);
                    if (prefabPth == null || prefabPth.Length == 0)
                    {
                        Debug.LogError("�޷����棬�ϲ���Ϸ���岢�Ǵ����ϵ���Դ��");
                        return false;
                    }
                    string baseName = Path.GetFileNameWithoutExtension(prefabPth);
                    string folderPath = prefabPth.Substring(0, prefabPth.Length - baseName.Length - 7);
                    string newFilename = folderPath + baseName + "-mesh";

                    //����������Դ
                    SaveMeshsToAssetDatabase(mom, folderPath, newFilename);

                    if (mom.meshCombiner.renderType == RendererType.skinnedMeshRenderer)
                    {
                        Debug.LogWarning("Render type is skinned mesh renderer. " +
                                "Can't create prefab until all bones have been added to the combined mesh object " + mom.resultPrefab +
                                " Add the bones then drag the combined mesh object to the prefab.");

                    }
                    //���� Prefab
                    RebuildPrefab(mom, ref so);

                    MeshBakerUtility.Destroy(mom.meshCombiner.resultSceneObject);
                }
                else
                {
                    Debug.LogError("�ϲ�������ͳ���");
                    return false;
                }
            }
            else
            {
                //����ϲ�ʧ��
                if (mom.clearBuffersAfterBake)
                {
                    mom.meshCombiner.ClearBuffers();
                }
                if (createdDummyTextureBakeResults)
                    MeshBakerUtility.Destroy(mom.textureBakeResults);
                return false;
            }

            //�����������
            if (mom.clearBuffersAfterBake)
            {
                mom.meshCombiner.ClearBuffers();
            }
            //��ʱTexture
            if (createdDummyTextureBakeResults)
                MeshBakerUtility.Destroy(mom.textureBakeResults);
            return true;
        }

        /// <summary>
        /// ����������Դ�� Asset ��
        /// </summary>
        public static void SaveMeshsToAssetDatabase(MeshBakerCommon mom, string folderPath, string newFileNameBase)
        {
            if (mom is MeshCombinerEntrance)
            {
                MeshCombinerEntrance mb = (MeshCombinerEntrance)mom;
                string newFilename = newFileNameBase + ".asset";
                string ap = AssetDatabase.GetAssetPath(((MeshCombineHandler)mb.meshCombiner).GetMesh());
                if (ap == null || ap.Equals(""))
                {
                    Debug.Log("����������Դ " + newFilename);
                    AssetDatabase.CreateAsset(((MeshCombineHandler)mb.meshCombiner).GetMesh(), newFilename);
                }
                else
                {
                    Debug.Log("Mesh is an asset at " + ap);
                }
            }
            //else if (mom is MB3_MultiMeshBaker)
            //{
            //    MB3_MultiMeshBaker mmb = (MB3_MultiMeshBaker)mom;
            //    List<MB3_MultiMeshCombiner.CombinedMesh> combiners = ((MB3_MultiMeshCombiner)mmb.meshCombiner).meshCombiners;
            //    for (int i = 0; i < combiners.Count; i++)
            //    {
            //        string newFilename = newFileNameBase + i + ".asset";
            //        Mesh mesh = combiners[i].combinedMesh.GetMesh();
            //        string ap = AssetDatabase.GetAssetPath(mesh);
            //        if (ap == null || ap.Equals(""))
            //        {
            //            Debug.Log("Saving mesh asset to " + newFilename);
            //            AssetDatabase.CreateAsset(mesh, newFilename);
            //        }
            //        else
            //        {
            //            Debug.Log("Mesh is an asset at " + ap);
            //        }
            //    }
            //}
            else
            {
                Debug.LogError("Argument was not a MeshCombineEntrance or an MultiMeshBaker.");
            }
        }

        /// <summary>
        /// ����Ԥ����
        /// </summary>
        /// <param name="mom"></param>
        /// <param name="so"></param>
        public static void RebuildPrefab(MeshBakerCommon mom, ref SerializedObject so)
        {
            GameObject prefabRoot = mom.resultPrefab;
            GameObject rootGO = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);

            SceneBakerUtilityInEditor.UnpackPrefabInstance(rootGO, ref so);

            //remove all renderer childeren of rootGO
            Renderer[] rs = rootGO.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rs.Length; i++)
            {
                if (rs[i] != null && rs[i].transform.parent == rootGO.transform)
                {
                    MeshBakerUtility.Destroy(rs[i].gameObject);
                }
            }

            if (mom is MeshCombinerEntrance)
            {
                MeshCombinerEntrance entrance = (MeshCombinerEntrance)mom;
                MeshCombineHandler mbs = (MeshCombineHandler)entrance.meshCombiner;
                MeshCombineHandler.BuildPrefabHierarchy(mbs, rootGO, mbs.GetMesh());
            }
            ////else if (mom is MB3_MultiMeshBaker)
            ////{
            ////    MB3_MultiMeshBaker mmb = (MB3_MultiMeshBaker)mom;
            ////    MB3_MultiMeshCombiner mbs = (MB3_MultiMeshCombiner)mmb.meshCombiner;
            ////    for (int i = 0; i < mbs.meshCombiners.Count; i++)
            ////    {
            ////        MB3_MeshCombinerSingle.BuildPrefabHierarchy(mbs.meshCombiners[i].combinedMesh, rootGO, mbs.meshCombiners[i].combinedMesh.GetMesh(), true);
            ////    }
            ////}
            else
            {
                Debug.LogError("MeshCombiner�ϲ������ʹ���");
            }

            //����Ԥ����
            string prefabPth = AssetDatabase.GetAssetPath(prefabRoot);

            SceneBakerUtilityInEditor.ReplacePrefab(rootGO, prefabPth, ReplacePrefabOption.connectToPrefab);
            if (mom.meshCombiner.renderType != RendererType.skinnedMeshRenderer)
            {
                // For Skinned meshes, leave the prefab instance in the scene so source game objects can moved into the prefab.
                UnityEditor.Editor.DestroyImmediate(rootGO);
            }
        }

        public static void UnwrapUV2(Mesh mesh, float hardAngle, float packingMargin)
        {
            UnwrapParam up = new UnwrapParam();
            UnwrapParam.SetDefaults(out up);
            up.hardAngle = hardAngle;
            up.packMargin = packingMargin;
            Unwrapping.GenerateSecondaryUVSet(mesh, up);
        }

        /// <summary>
        /// �ж��Ƿ�ɴ�����ʱ����
        /// </summary>
        /// <param name="mom"></param>
        /// <returns></returns>
        public static bool _OkToCreateDummyTextureBakeResult(MeshBakerCommon mom)
        {
            List<GameObject> objsToMesh = mom.GetObjectsToCombine();
            if (objsToMesh.Count == 0)
                return false;
            return true;
        }
    }
}
