#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/2 20:44:51
// Email:             836045613@qq.com

#endregion

using Client.Core.Editor;
using Client.Utility;
using Common.Utility;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Client.Assets.Editor
{
    public class MaterialMenuItemExtend
    {
        [MenuItem("Assets/Material/设置选定目录下所有Mesh文件的材质引用")]
        private static void SetDirsMeshImporterMat()
        {
            var path = EditorUtility.OpenFilePanel("选择Importer Material", "Assets/XTwo/AssetDatabase", "mat");
            path = path.Replace(Application.dataPath, "Assets");
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
            {
                SetMeshImporterMat(mat);
            }
            else
            {
                Debug.LogError("无法找到相应Material文件 " + path);
            }
        }

        private static void SetMeshImporterMat(Material mat)
        {
            var mainDir = UnityEditorUtility.GetSelectDir();
            var dirs = IOUtility.GetAllDir(mainDir);
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    var fullDir = UnityIOUtility.GetFullPath(dir);
                    //Debug.LogError(dir);

                    var fullpaths = Directory.GetFiles(fullDir);
                    foreach (var item in fullpaths)
                    {
                        SetMeshImporterMat(item, mat);
                    }

                }
            }
            //var objs =  AssetDatabase.LoadAllAssetsAtPath(dir);
            //Debug.LogError(objs.Length);
            //foreach (var item in objs)
            //{
            //    Debug.LogError(item.name);
            //}
        }

        private static void SetMeshImporterMat(string fullPath, Material mat)
        {
            if (fullPath.EndsWith(".meta"))
            {
                return;
            }
            var path = UnityIOUtility.GetAssetsPath(fullPath);

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null)
            {
                return;
            }

            var importer = ModelImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as ModelImporter;
            if (importer != null)
            {
                var renders = obj.GetComponentsInChildren<Renderer>();
                string matName = null;
                foreach (var item in renders)
                {
                    foreach (var matItem in item.sharedMaterials)
                    {
                        if (matItem != null)
                        {
                            matName = matItem.name;
                            break;
                        }
                    }
                    if (matName != null)
                    {
                        break;
                    }
                }
                if(matName == null)
                {
                    return;
                }

                importer.importMaterials = true;
                importer.materialLocation = ModelImporterMaterialLocation.InPrefab;

                importer.SaveAndReimport();

                var map = importer.GetExternalObjectMap();
                List<AssetImporter.SourceAssetIdentifier> list =
                    new List<AssetImporter.SourceAssetIdentifier>();
                foreach (var item in map)
                {
                    //Debug.LogError(item.Key.name + " --- " + item.Key.type);
                    if (item.Key.type == mat.GetType())
                    {
                        list.Add(item.Key);

                    }
                }
                foreach (var item in list)
                {
                    importer.RemoveRemap(item);
                    importer.AddRemap(item, mat);
                }

                foreach (var item in renders)
                {
                    foreach (var matItem in item.sharedMaterials)
                    {
                        if (matItem != null)
                        {
                            matName = matItem.name;
                            importer.AddRemap(new AssetImporter.SourceAssetIdentifier(mat.GetType(), matName), mat);
                        }
                    }
                }

                Debug.Log("完成mesh的Importer Materials操作：" + path);

                importer.SaveAndReimport();
            }
        }
    }

}