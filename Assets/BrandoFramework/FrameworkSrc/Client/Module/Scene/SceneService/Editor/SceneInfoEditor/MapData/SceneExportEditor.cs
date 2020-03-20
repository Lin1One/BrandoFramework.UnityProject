#region Head

// Author:            LinYuzhou
// CreateDate:        2019/9/18 01:42:04
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.Scene;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace GameWorld.Editor
{
    public class SceneExportEditor
    {
        private static string s_curMapName;
        private static string s_mapDataPath = Application.dataPath + "/GameWorld/TestScene/MapData/";
        private static int mapCellSize = 100;
        public static SceneInfo currentSceneInfo;

        //--------------------- 处理地图基本（表现层）物体数据 ------------------------

        //导出地图主函数
        [MenuItem("Export/生成地图数据")]
        public static void ExportMap()
        {
            GameObject selectObj = Selection.activeGameObject;
            if (selectObj == null)
            {
                Debug.LogError("未选择地图的根物体");
                return;
            }

            if (!selectObj.name.Contains("#"))
            {
                EditorUtility.DisplayDialog("提示", "没有选择正确命名的地图gameobject,格式：地图名#x格数_z格数_地图id", "确定");
                return;
            }

            s_curMapName = selectObj.name.Split('#')[0];
            string setString = selectObj.name.Split('#')[1];
            string[] settings = setString.Split('_');
            if (settings.Length != 3)
            {
                EditorUtility.DisplayDialog("提示",
                    "没有选择正确命名的地图gameobject,格式：地图名#格数_格数_地图id", "确定");
                return;
            }

            //if (!EditorUtility.DisplayDialog("提示", "是否开始打包地图资源", "确定", "取消"))
            //    return;

            //初始化地图数据对象
            currentSceneInfo = new SceneInfo();

            //获取天空盒数据
            if (RenderSettings.skybox == null)
            {
                currentSceneInfo.skyboxMaterial = "";
            }
            else
            {
                currentSceneInfo.skyboxMaterial = RenderSettings.skybox.name;
            }

            //场景雾
            if (RenderSettings.fog)
            {
                currentSceneInfo.hasFog = true;
                currentSceneInfo.fogColor = new MyVector4(RenderSettings.fogColor);
                currentSceneInfo.fogMode = (int)RenderSettings.fogMode;
                currentSceneInfo.fogStart = RenderSettings.fogStartDistance;
                currentSceneInfo.fogEnd = RenderSettings.fogEndDistance;
                currentSceneInfo.fogDensity = RenderSettings.fogDensity;
            }
            else
            {
                currentSceneInfo.hasFog = false;
                currentSceneInfo.fogColor = new MyVector4(1, 1, 1, 1);
                currentSceneInfo.fogMode = (int)FogMode.Linear;
                currentSceneInfo.fogStart = 0;
                currentSceneInfo.fogEnd = 20;
            }

            //读取记录所有场景尺寸的文件
            string mapSizeInfoPath = string.Format("{0}/MapSizeInfo/", s_mapDataPath);
            string mapSizeInfoFilePath = string.Format("{0}/MapSizeInfo/MapSizeInfo.json", s_mapDataPath);

            //地图总信息
            YuAllMapSize allMapSizeInfo = null;
            if (File.Exists(mapSizeInfoFilePath))
            {
                var allMapSizeFile = File.ReadAllText(mapSizeInfoFilePath);
                //allMapSizeInfo = YuJsonUtility.FromJson<YuAllMapSize>(allMapSizeFile);
            }

            if (allMapSizeInfo == null)
            {
                allMapSizeInfo = new YuAllMapSize();
            }
            if (allMapSizeInfo.MapNameList == null)
            {
                allMapSizeInfo.MapNameList = new List<string>();
                allMapSizeInfo.MapSizeList = new List<SceneSizeInfo>();
            }
            if (!allMapSizeInfo.MapNameList.Contains(s_curMapName))
            {
                allMapSizeInfo.MapNameList.Add(s_curMapName);
                allMapSizeInfo.MapSizeList.Add(new SceneSizeInfo());
            }
            var mapSizeInfo = allMapSizeInfo.MapSizeList[allMapSizeInfo.MapNameList.IndexOf(s_curMapName)];


            currentSceneInfo.mapId = int.Parse(settings[2]);

            //SceneVO voData = ExportMapLogicData(info, selectObj.transform, mapSizeInfo);
            //CreateMonsterTeamData(voData, info);
            //CreateCollectionTeamData(voData, info);

            //计算场景大小
            var currentSceneRect = GetRect(selectObj.transform);
            float xMin = currentSceneRect.x, xMax = currentSceneRect.z, zMin = currentSceneRect.y, zMax = currentSceneRect.w;
            Debug.Log("xMin = " + currentSceneRect.x + ", zMin = " + currentSceneRect.y + ", xMax = " + currentSceneRect.z + "zMax = " + currentSceneRect.w);

            //计算格子高宽
            //info.xCellCount = Mathf.Max(1, ((int)(xMax - xMin)) / mapCellSize);
            //info.zCellCount = Mathf.Max(1, ((int)(zMax - zMin)) / mapCellSize);
            currentSceneInfo.xCellCount = int.Parse(settings[0]);
            currentSceneInfo.zCellCount = int.Parse(settings[1]);
            Debug.Log("格子总数为 x ：" + currentSceneInfo.xCellCount + " z ： " + currentSceneInfo.zCellCount);
            currentSceneInfo.mapName = s_curMapName;

            SceneCellInfo[] cellsInfo = new SceneCellInfo[currentSceneInfo.xCellCount * currentSceneInfo.zCellCount];
            currentSceneInfo.cells = cellsInfo;
            for (int i = 0; i < cellsInfo.Length; i++)
            {
                cellsInfo[i] = new SceneCellInfo();
                cellsInfo[i].cellId = i;
                cellsInfo[i].itemsNum = new List<int>();
            }

            //初始化节点、预制物容器
            List<SceneLayerInfo> layerList = new List<SceneLayerInfo>();
            List<SceneItemInfo> prefabList = new List<SceneItemInfo>();

            mapSizeInfo.UnityMin = new MyVector2(xMin, zMin);
            mapSizeInfo.UnityMax = new MyVector2(xMax, zMax);

            //设置原点、划分格子
            currentSceneInfo.originPos = new MyVector2(xMin, zMin);
            currentSceneInfo.xCellSize = (xMax - xMin) / currentSceneInfo.xCellCount;
            currentSceneInfo.zCellSize = (zMax - zMin) / currentSceneInfo.zCellCount;

            //记录lightmap图片资源（不参与资源打包）
            if (LightmapSettings.lightmaps.Length > 0)
            {
                List<Texture2D> lmTexs = new List<Texture2D>();
                List<int> dataIndex = new List<int>();
                List<SceneLightInfo.ELightmapType> lmType = new List<SceneLightInfo.ELightmapType>();

                for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
                {
                    LightmapData lmdata = LightmapSettings.lightmaps[i];

                    if (lmdata.lightmapColor != null)
                    {
                        lmTexs.Add(lmdata.lightmapColor);
                        dataIndex.Add(i);
                        lmType.Add(SceneLightInfo.ELightmapType.color);
                    }
                    if (lmdata.lightmapDir != null)
                    {
                        lmTexs.Add(lmdata.lightmapDir);
                        dataIndex.Add(i);
                        lmType.Add(SceneLightInfo.ELightmapType.dir);
                    }
                    if (lmdata.shadowMask != null)
                    {
                        lmTexs.Add(lmdata.shadowMask);
                        dataIndex.Add(i);
                        lmType.Add(SceneLightInfo.ELightmapType.shadowmask);
                    }
                }

                string[] texNames = new string[lmTexs.Count];
                string[] texPaths = new string[lmTexs.Count];
                for (int i = 0; i < lmTexs.Count; i++)
                {
                    if (lmTexs[i] != null)
                    {
                        texNames[i] = lmTexs[i].name;
                        texPaths[i] = AssetDatabase.GetAssetPath(lmTexs[i]);
                    }
                    else
                    {
                        texNames[i] = null;
                        //texPaths[i] = AssetDatabase.GetAssetPath(lmTexs[i]);
                    }
                        
                }

                SceneLightInfo lmInfo = currentSceneInfo.lightmapInfo = new SceneLightInfo();
                lmInfo.lightmapTexNames = texNames;
                lmInfo.lightmapTexPath = texPaths;
                lmInfo.dataIndexes = dataIndex.ToArray();
                lmInfo.types = lmType.ToArray();
                lmInfo.lightmapMode = LightmapSettings.lightmapsMode;
            }

            //光照信息
            CreateRoleLight(selectObj.transform, currentSceneInfo);

            //算出prefab总数
            int prefabNum = 0;
            TraverseHierarchy(selectObj.transform,
                (Transform trans) =>
                {
                    UnityEngine.Object objPrefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);

                    if (objPrefab != null)  //如果对象是有预制物的对象(objPrefab为预制物对象，谨慎使用)
                    {
                        prefabNum++;
                        return true;
                    }

                    return false;
                });

            int itemNum = 0;

            //前序遍历所有节点，视prefab为叶子
            TraverseHierarchy(selectObj.transform,
                (Transform trans) =>
                {
                    EditorUtility.DisplayProgressBar("Title", string.Format("正在构建地图分块加载数据：{0}/{1}",
                        itemNum, prefabNum), (float)itemNum / prefabNum);

                    bool isPrefab = false;
                    UnityEngine.Object objPrefab = PrefabUtility.GetCorrespondingObjectFromSource(trans.gameObject);

                    if (objPrefab != null)  //如果对象是有预制物的对象(objPrefab为预制物对象，谨慎使用)
                    {

                        isPrefab = true;
                        string prefabPath = AssetDatabase.GetAssetPath(objPrefab);

                        //Todo 可能需要添加获取资源其他信息
                        //prefabPath = CopyFileToResources(prefabPath);

                        SceneItemInfo itemInfo = new SceneItemInfo();
                        itemInfo.parentLayer = MyMapTool.GetELaterByName(trans.parent.name, selectObj.name);

                        List<int> numList;
                        if (itemInfo.parentLayer == EMapLayer.Animation)
                        {
                            //如果是animation类型，则全地图加载
                            numList = new List<int>(currentSceneInfo.CellCount);
                            for (int cellNum = 0; cellNum < currentSceneInfo.CellCount; cellNum++)
                            {
                                numList.Add(cellNum);
                            }
                        }
                        else
                        {
                            numList = GetCellNumByVertex(trans.gameObject, currentSceneInfo); //算出此游戏对象所在格子编号集合
                        }

                        if (numList != null && numList.Count > 0 && itemInfo.parentLayer != EMapLayer.None)
                        {
                            //初始化物体info
                            prefabList.Add(itemInfo);
                            itemInfo.pos = new MyVector3(trans.position);
                            itemInfo.scal = new MyVector3(trans.lossyScale);
                            itemInfo.rot = new MyVector3(trans.eulerAngles);
                            itemInfo.objNum = itemNum;
                            itemInfo.objName = trans.name;
                            itemInfo.prefabName = objPrefab.name;
                            itemInfo.prefabAssetPath = prefabPath;

                            //保存lightmap信息
                            Renderer[] renders = trans.gameObject.GetComponentsInChildren<Renderer>();
                            if (renders.Length > 0)
                            {
                                itemInfo.lightmapIndexes = new int[renders.Length];
                                itemInfo.lightmapScaleOffsets = new MyVector4[renders.Length];
                                itemInfo.realtimeLightmapIndexes = new int[renders.Length];
                                itemInfo.realtimeLightmapScaleOffsets = new MyVector4[renders.Length];
                                for (int i = 0; i < renders.Length; i++)
                                {
                                    itemInfo.lightmapIndexes[i] = renders[i].lightmapIndex;
                                    itemInfo.lightmapScaleOffsets[i] = new MyVector4(renders[i].lightmapScaleOffset);
                                    itemInfo.realtimeLightmapIndexes[i] = renders[i].realtimeLightmapIndex;
                                    itemInfo.realtimeLightmapScaleOffsets[i] = new MyVector4(renders[i].realtimeLightmapScaleOffset);
                                }

                            }

                            foreach (int num in numList)     //遍历对应编号的格子info，存入物体info
                            {
                                if (num >= 0 && num < cellsInfo.Length)
                                {
                                    cellsInfo[num].itemsNum.Add(itemInfo.objNum);
                                }
                            }

                            itemNum++;
                        }
                        return true;
                    }
                    else  //如果对象不是预制物，则认为是层对象
                    {
                        string layerName = trans.name;
                        if (trans.gameObject == selectObj)
                        {
                            layerName = layerName.Split('#')[0];
                        }

                        SceneLayerInfo layerInfo = new SceneLayerInfo();
                        layerInfo.layerName = layerName;
                        layerInfo.pos = new MyVector3(trans.position);
                        layerInfo.rot = new MyVector3(trans.eulerAngles);
                        layerInfo.scal = new MyVector3(trans.lossyScale);
                        if (trans.parent != null && trans != selectObj.transform)
                            layerInfo.parentLayer = MyMapTool.GetELaterByName(trans.parent.name, selectObj.name);

                        layerList.Add(layerInfo);
                    }
                    return isPrefab;
                    }
                );
            EditorUtility.ClearProgressBar();

            ////创建navMesh数据
            //NavMeshTriangulation navMeshTri = NavMesh.CalculateTriangulation();
            //info.navMeshData = YuNavMeshDataCreate.CreateNavMeshData(navMeshTri);

            if (!Directory.Exists(mapSizeInfoPath))
            {
                Directory.CreateDirectory(mapSizeInfoPath);
            }

            string curMapPath = string.Format("{0}/{1}", s_mapDataPath, s_curMapName);

            if (!Directory.Exists(curMapPath))
            {
                Directory.CreateDirectory(curMapPath);
            }

            //创建高度图
            CreateHeightMap(selectObj.transform, currentSceneInfo,
                string.Format("{0}/{1}/{2}_heightmap.png", s_mapDataPath, s_curMapName, s_curMapName));

            currentSceneInfo.layers = layerList.ToArray();
            currentSceneInfo.items = prefabList.ToArray();
            //string fullPaht = string.Format("{0}/{1}_scriptable.asset", s_mapDataPath, s_curMapName);
            //AssetDatabase.CreateAsset(info, fullPaht);

            string fullPath = string.Format("{0}/{1}/{2}.json", s_mapDataPath, s_curMapName, s_curMapName);
            Common.Utility.JsonUtility.WriteAsJson(fullPath, currentSceneInfo);
            Common.Utility.JsonUtility.WriteAsJson(mapSizeInfoFilePath, allMapSizeInfo);
            AssetDatabase.Refresh();
        }

        [MenuItem("GameWorld/合并地形")]
        public static void CombineMeshMap()
        {
            GameObject selectObj = Selection.activeGameObject;
            MeshFilter[] mfChildren = selectObj.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[mfChildren.Length];

            //获取所有子物体的渲染器和材质
            MeshRenderer[] mrChildren = selectObj.GetComponentsInChildren<MeshRenderer>();
            Material[] materials = new Material[mrChildren.Length];

            //合并子纹理
            Texture2D[] textures = new Texture2D[mrChildren.Length];

            List<Material> materialsList = new List<Material>();
            List<Texture2D> texturesList = new List<Texture2D>();

            Dictionary<Material, List<MeshFilter>> sameMatMeshFiletersDic = new Dictionary<Material, List<MeshFilter>>();
            Dictionary<Material, List<MeshRenderer>> sameMatMeshRenderersDic = new Dictionary<Material, List<MeshRenderer>>();
            for (int i = 0; i < mrChildren.Length; i++)
            {
                var mat = mrChildren[i].sharedMaterial;
                if (!sameMatMeshFiletersDic.ContainsKey(mat))
                {
                    sameMatMeshFiletersDic.Add(mat, new List<MeshFilter>());
                    sameMatMeshRenderersDic.Add(mat, new List<MeshRenderer>());
                }
                sameMatMeshRenderersDic[mat].Add(mrChildren[i]);
                sameMatMeshFiletersDic[mat].Add(mfChildren[i]);
            }
            for (int i = 0; i < mrChildren.Length; i++)
            {
                materials[i] = mrChildren[i].sharedMaterial;
                if (materialsList.Contains(mrChildren[i].sharedMaterial))
                {
                    textures[i] = textures[i - 1];
                    continue;
                }
                materialsList.Add(mrChildren[i].sharedMaterial);
                materials[i] = mrChildren[i].sharedMaterial;
                Texture2D tx = materials[i].GetTexture("_MainTex") as Texture2D;
                if (tx == null)
                {
                    Debug.Log(materials[i].ToString() + "_____" + i);
                    continue;
                }
                if (texturesList.Contains(tx))
                {
                    continue;
                }
                Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
                tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
                tx2D.Apply();
                textures[i] = tx2D;
                texturesList.Add(tx2D);
            }
            //设置新材质的主纹理
            Texture2D texture = new Texture2D(2048, 2048);

            Rect[] rects = texture.PackTextures(texturesList.ToArray(), 5, 2048);

            foreach (var met in materialsList)
            {
                Material sameMaterial = new Material(met.shader);
                sameMaterial.CopyPropertiesFromMaterial(met);
                sameMaterial.SetTexture("_MainTex", texture);
                GameObject newSameMaterialGo = new GameObject(met.ToString());
                //生成新的渲染器和网格组件
                MeshRenderer mr = newSameMaterialGo.AddComponent<MeshRenderer>();
                MeshFilter mf = newSameMaterialGo.AddComponent<MeshFilter>();
                mr.material = sameMaterial;
                //mr.sharedMaterial = sameMaterial;
                newSameMaterialGo.transform.SetParent(selectObj.transform);

                var indexOfRect = materialsList.IndexOf(met);
                var combines = new List<CombineInstance>();
                for (int i = 0; i < mfChildren.Length; i++)
                {
                    if (materials[i] != met)
                        continue;
                    Rect rect = rects[indexOfRect];
                    Mesh meshCombine = mfChildren[i].sharedMesh;
                    Vector2[] uvs = new Vector2[meshCombine.uv.Length];
                    //把网格的uv根据贴图的rect刷一遍  
                    for (int j = 0; j < uvs.Length; j++)
                    {
                        uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                        uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
                    }
                    meshCombine.uv = uvs;
                    CombineInstance newconbine = new CombineInstance();
                    newconbine.mesh = meshCombine;
                    newconbine.transform = mfChildren[i].transform.localToWorldMatrix;
                    //mfChildren[i].gameObject.SetActive(false);
                    combines.Add(newconbine);
                }

                //生成新的网格，赋值给新的网格渲染组件
                Mesh newMesh = new Mesh();
                newMesh.CombineMeshes(combines.ToArray(), true, true);//合并网格  
                mf.mesh = newMesh;
                var combineAssetPath = Application.dataPath + "/GameWorld/TestScene/originAsset/CombineAsset/"/* + mf.gameObject.name + ".fbx"*/;
                //AssetDatabase.CreateAsset(newMesh, combineAssetPath);
                CreatCombineMeshAsset(mf.gameObject, combineAssetPath);
            }
        }

        [MenuItem("GameWorld/合并相同材质的物体2")]
        public static void CombineMeshMap2()
        {
            var combineAssetPath = Application.dataPath + "/GameWorld/TestScene/originAsset/CombineAsset/"/* + mf.gameObject.name + ".fbx"*/;

            GameObject selectObj = Selection.activeGameObject;
            MeshFilter[] mfChildren = selectObj.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[mfChildren.Length];

            //获取所有子物体的渲染器和材质
            MeshRenderer[] mrChildren = selectObj.GetComponentsInChildren<MeshRenderer>();
            Material[] materials = new Material[mrChildren.Length];



            List<Material> materialsList = new List<Material>();
            Dictionary<Shader, List<MeshFilter>> sameShaderGameObject = new Dictionary<Shader, List<MeshFilter>>();
            Dictionary<Shader,List<Material>> sameShaderMat = new Dictionary<Shader,List<Material>>();
            Dictionary<Material, List<MeshFilter>> sameMatMeshFiletersDic = new Dictionary<Material, List<MeshFilter>>();
            Dictionary<Material, List<MeshRenderer>> sameMatMeshRenderersDic = new Dictionary<Material, List<MeshRenderer>>();
            for (int i = 0; i < mrChildren.Length; i++)
            {
                var mat = mrChildren[i].material;
                var shader = mat.shader;
                if (!sameShaderMat.ContainsKey(shader))
                {
                    sameShaderMat.Add(shader, new List<Material>());
                    sameShaderGameObject.Add(shader, new List<MeshFilter>());
                }
                sameShaderMat[shader].Add(mat);
                sameShaderGameObject[shader].Add(mfChildren[i]);
                //if (!sameMatMeshFiletersDic.ContainsKey(mat))
                //{
                //    sameMatMeshFiletersDic.Add(mat, new List<MeshFilter>());
                //    sameMatMeshRenderersDic.Add(mat, new List<MeshRenderer>());
                //}
                //sameMatMeshRenderersDic[mat].Add(mrChildren[i]);
                //sameMatMeshFiletersDic[mat].Add(mfChildren[i]);
            }
            foreach(var sameMetList in sameShaderMat.Values)
            {
                //合并子纹理
                List<Texture2D> textures = new List<Texture2D>();
                Dictionary<string, int> textureIndex = new Dictionary<string, int>();
                List<Material> existMaterials = new List<Material>();
                
                for (int i = 0; i < sameMetList.Count; i++)
                {
                    Texture2D tx = sameMetList[i].GetTexture("_MainTex") as Texture2D;
                    if (tx == null)
                    {
                        Debug.Log(materials[i].ToString() + "_____" + i);
                        continue;
                    }
                    if(textureIndex.ContainsKey(tx.name))
                    {
                        continue;
                    }

                    Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.RGBA32, false);
                    tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
                    tx2D.Apply();
                    textures.Add(tx2D);
                    textureIndex.Add(tx.name, textures.Count - 1);
                }
                //设置新材质的主纹理
                Texture2D texture = new Texture2D(512, 2048);
                Rect[] rects = texture.PackTextures(textures.ToArray(), 5, 1024);
                Material combineMaterial = new Material(sameMetList[0].shader);
                combineMaterial.CopyPropertiesFromMaterial(sameMetList[0]);
                
                
                GameObject newSameMaterialGo = new GameObject(sameMetList[0].ToString());
                //生成新的渲染器和网格组件
                MeshRenderer mr = newSameMaterialGo.AddComponent<MeshRenderer>();
                MeshFilter mf = newSameMaterialGo.AddComponent<MeshFilter>();

                //AssetDatabase.CreateAsset(texture, "Assets/GameWorld/TestScene/originAsset/CombineAsset/" + newSameMaterialGo.name + ".png");
                File.WriteAllBytes(combineAssetPath + newSameMaterialGo.name + ".png", texture.EncodeToPNG());
                AssetDatabase.CreateAsset(combineMaterial, "Assets/GameWorld/TestScene/originAsset/CombineAsset/" + newSameMaterialGo.name + ".mat");
                AssetDatabase.Refresh();
                mr.material = combineMaterial;
                combineMaterial.SetTexture("_MainTex", texture);
                //mr.sharedMaterial = combineMaterial;
                newSameMaterialGo.transform.SetParent(selectObj.transform);

                //var indexOfRect = materialsList.IndexOf(met);
                var combines = new List<CombineInstance>();
                for (int i = 0; i < sameMetList.Count; i++)
                {
                    //if (materials[i] != met)
                    //    continue;
                    //var indexOfRect = materialsList.IndexOf(met);
                    var mat = sameMetList[i];
                    var textureName = mat.mainTexture.name;
                    var index = textureIndex[textureName];
                    Rect rect = rects[index];
                    Debug.Log(rect.ToString() + "UV " + i);
                    sameShaderGameObject[sameMetList[0].shader][i].gameObject.name = i.ToString();
                    Mesh meshCombine = sameShaderGameObject[sameMetList[0].shader][i].mesh;
                    Vector2[] uvs = new Vector2[meshCombine.uv.Length];
                    //把网格的uv根据贴图的rect刷一遍  
                    for (int j = 0; j < uvs.Length; j++)
                    {
                        uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                        uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
                    }
                    meshCombine.uv = uvs;
                    CombineInstance newconbine = new CombineInstance();
                    newconbine.mesh = meshCombine;
                    newconbine.transform = mfChildren[i].transform.localToWorldMatrix;
                    //mfChildren[i].gameObject.SetActive(false);
                    combines.Add(newconbine);
                }

                //生成新的网格，赋值给新的网格渲染组件
                Mesh newMesh = new Mesh();
                newMesh.CombineMeshes(combines.ToArray(), true, true);//合并网格  
                mf.mesh = newMesh;
                
                //AssetDatabase.CreateAsset(newMesh, combineAssetPath);
                CreatCombineMeshAsset(mf.gameObject, combineAssetPath);
            }
            




            

            //foreach (var met in materialsList)
            //{
            //    Material sameMaterial = new Material(met.shader);
            //    sameMaterial.CopyPropertiesFromMaterial(met);
            //    sameMaterial.SetTexture("_MainTex", texture);
            //    GameObject newSameMaterialGo = new GameObject(met.ToString());
            //    //生成新的渲染器和网格组件
            //    MeshRenderer mr = newSameMaterialGo.AddComponent<MeshRenderer>();
            //    MeshFilter mf = newSameMaterialGo.AddComponent<MeshFilter>();
            //    mr.material = sameMaterial;
            //    //mr.sharedMaterial = sameMaterial;
            //    newSameMaterialGo.transform.SetParent(selectObj.transform);

            //    var indexOfRect = materialsList.IndexOf(met);
            //    var combines = new List<CombineInstance>();
            //    for (int i = 0; i < mfChildren.Length; i++)
            //    {
            //        if (materials[i] != met)
            //            continue;
            //        Rect D = rects[indexOfRect];
            //        Mesh meshCombine = mfChildren[i].sharedMesh;
            //        Vector2[] uvs = new Vector2[meshCombine.uv.Length];
            //        //把网格的uv根据贴图的rect刷一遍  
            //        for (int j = 0; j < uvs.Length; j++)
            //        {
            //            uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
            //            uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
            //        }
            //        meshCombine.uv = uvs;
            //        CombineInstance newconbine = new CombineInstance();
            //        newconbine.mesh = meshCombine;
            //        newconbine.transform = mfChildren[i].transform.localToWorldMatrix;
            //        //mfChildren[i].gameObject.SetActive(false);
            //        combines.Add(newconbine);
            //    }

            //    //生成新的网格，赋值给新的网格渲染组件
            //    Mesh newMesh = new Mesh();
            //    newMesh.CombineMeshes(combines.ToArray(), true, true);//合并网格  
            //    mf.mesh = newMesh;
            //    var combineAssetPath = Application.dataPath + "/GameWorld/TestScene/originAsset/CombineAsset/"/* + mf.gameObject.name + ".fbx"*/;
            //    //AssetDatabase.CreateAsset(newMesh, combineAssetPath);
            //    CreatCombineMeshAsset(mf.gameObject, combineAssetPath);
            //}
        }

        private static void CreatCombineMeshAsset(GameObject meshGo,string meshAssetPath)
        {
            using (StreamWriter streamWriter = new StreamWriter(string.Format("{0}{1}.obj", meshAssetPath, meshGo.name)))
            {
                var mf = meshGo.GetComponent<MeshFilter>();
                streamWriter.Write(MeshToString(mf, new Vector3(-1f, 1f, 1f)));
                streamWriter.Close();
            }
            AssetDatabase.Refresh();
        }

        private static string MeshToString(MeshFilter mf, Vector3 scale)
        {
            Mesh mesh = mf.sharedMesh;
            Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
            Vector2 textureOffset = mf.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex");
            Vector2 textureScale = mf.GetComponent<Renderer>().sharedMaterial.GetTextureScale("_MainTex");

            StringBuilder stringBuilder = new StringBuilder().Append("mtllib design.mtl")
                .Append("\n")
                .Append("g ")
                .Append(mf.name)
                .Append("\n");

            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector = vertices[i];
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale.x, vector.y * scale.y, vector.z * scale.z));
            }

            stringBuilder.Append("\n");

            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            if (mesh.subMeshCount > 1)
            {
                int[] triangles = mesh.GetTriangles(1);

                for (int j = 0; j < triangles.Length; j += 3)
                {
                    if (!dictionary.ContainsKey(triangles[j]))
                    {
                        dictionary.Add(triangles[j], 1);
                    }

                    if (!dictionary.ContainsKey(triangles[j + 1]))
                    {
                        dictionary.Add(triangles[j + 1], 1);
                    }

                    if (!dictionary.ContainsKey(triangles[j + 2]))
                    {
                        dictionary.Add(triangles[j + 2], 1);
                    }
                }
            }

            for (int num = 0; num != mesh.uv.Length; num++)
            {
                Vector2 vector2 = Vector2.Scale(mesh.uv[num], textureScale) + textureOffset;

                if (dictionary.ContainsKey(num))
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", mesh.uv[num].x, mesh.uv[num].y));
                }
                else
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", vector2.x, vector2.y));
                }
            }

            for (int k = 0; k < mesh.subMeshCount; k++)
            {
                stringBuilder.Append("\n");

                if (k == 0)
                {
                    stringBuilder.Append("usemtl ").Append("Material_design").Append("\n");
                }

                if (k == 1)
                {
                    stringBuilder.Append("usemtl ").Append("Material_logo").Append("\n");
                }

                int[] triangles2 = mesh.GetTriangles(k);

                for (int l = 0; l < triangles2.Length; l += 3)
                {
                    stringBuilder.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n", triangles2[l] + 1, triangles2[l + 2] + 1, triangles2[l + 1] + 1));
                }
            }
            return stringBuilder.ToString();
        }

        //人物光源数据
        private static void CreateRoleLight(Transform trans, SceneInfo mapInfo)
        {
            if (trans.parent == null)
            {
                mapInfo.hasLight = false;
                return;
            }
            Transform lightTrans = trans.parent.Find("RoleLight");
            if (lightTrans == null)
            {
                mapInfo.hasLight = false;
                return;
            }
            Light light = lightTrans.GetComponent<Light>();
            if (light == null)
            {
                mapInfo.hasLight = false;
                return;
            }

            mapInfo.hasLight = true;
            mapInfo.lightColor = new MyVector4(light.color);
            mapInfo.lightIntensity = light.intensity;
            mapInfo.lightPos = new MyVector3(lightTrans.position);
            mapInfo.lightDir = new MyVector3(lightTrans.forward);
        }


        //判断一个物体对应的格子编号集合 
        //Todo 
        // 1、需要考虑子物体的mesh，
        // 2、需要考虑少量顶点组成的大面片所处所有格子 (已完成，需要研究是否可以改进算法逻辑)
        static List<int> GetCellNumByVertex(GameObject obj, SceneInfo mapInfo)
        {
            if (obj == null || mapInfo == null)
                return null;

            List<int> numList = new List<int>();
            Mesh mesh = null;
            if (obj.GetComponent<MeshFilter>() != null)
                mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            if (mesh != null)
            {
                //提出地图边界
                float xMin = mapInfo.xMin;
                float zMin = mapInfo.zMin;

                int cellCount = mapInfo.xCellCount * mapInfo.zCellCount; //格子数量

                List<Vector3> vertexList = new List<Vector3>();

                //缩放变换后的xz坐标size
                Vector2 meshSize = new Vector2(mesh.bounds.size.x * obj.transform.lossyScale.x,
                    mesh.bounds.size.z * obj.transform.lossyScale.z);

                //如果尺寸不超过一个格子的长或宽,则进行简易检测
                if (meshSize.magnitude <= Mathf.Min(mapInfo.xCellSize, mapInfo.zCellSize))
                {
                    //遍历每个顶点
                    foreach (var vertex in mesh.vertices)
                    {
                        //---将顶点世界变换后存入list---
                        Vector3 worldPos = obj.transform.TransformPoint(vertex);

                        //算出所在xz行列,进而得出所在格子编号
                        int xNum = (int)((worldPos.x - xMin) / mapInfo.xCellSize);
                        int zNum = (int)((worldPos.z - zMin) / mapInfo.zCellSize);

                        //越界规范
                        if (xNum < 0)
                            xNum = 0;
                        else if (xNum >= mapInfo.xCellCount)
                            xNum = mapInfo.xCellCount - 1;
                        if (zNum < 0)
                            zNum = 0;
                        else if (zNum > mapInfo.zCellCount)
                            zNum = mapInfo.zCellCount - 1;

                        int num = xNum + zNum * mapInfo.xCellCount;

                        if (!numList.Contains(num))
                            numList.Add(num);
                    }
                }
                else//如果尺寸超过一个格子的长或宽,则进行复杂运算，检测每个三角形面片所包裹的格子
                {
                    //遍历每个顶点
                    foreach (var vertex in mesh.vertices)
                    {
                        //---将顶点世界变换后存入list---
                        Vector3 worldPos = obj.transform.TransformPoint(vertex);
                        vertexList.Add(worldPos);
                    }

                    //遍历每个三角形面片
                    for (int i = 0; i < mesh.triangles.Length / 3; i++)
                    {
                        //---算出此面所覆盖的所有格子编号---
                        Vector3[] verticesWorld = new Vector3[3]
                        {
                        vertexList[mesh.triangles[i * 3]],
                        vertexList[mesh.triangles[i * 3 + 1]],
                        vertexList[mesh.triangles[i * 3 + 2]]
                        };

                        GetCellsByTriangle(verticesWorld, mapInfo, ref numList);
                    }
                }

            }
            else // 如果预制物不含mesh，则视为一个点
            {
                //提出地图边界
                float xMin = mapInfo.xMin;
                float zMin = mapInfo.zMin;

                int cellCount = mapInfo.xCellCount * mapInfo.zCellCount; //格子数量

                Vector3 worldPos = obj.transform.position;

                //算出所在xz行列,进而得出所在格子编号
                int xNum = (int)((worldPos.x - xMin) / mapInfo.xCellSize);
                int zNum = (int)((worldPos.z - zMin) / mapInfo.zCellSize);

                //越界规范
                if (xNum < 0)
                    xNum = 0;
                else if (xNum >= mapInfo.xCellCount)
                    xNum = mapInfo.xCellCount - 1;
                if (zNum < 0)
                    zNum = 0;
                else if (zNum > mapInfo.zCellCount)
                    zNum = mapInfo.zCellCount - 1;

                int num = xNum + zNum * mapInfo.xCellCount;

                if (!numList.Contains(num))
                    numList.Add(num);

            }

            return numList;
        }

        //计算一个三角形面所在地图格子编号集合,顶点信息为世界坐标（类似光栅化算法）
        private static void GetCellsByTriangle(Vector3[] worldPosArr, SceneInfo mapInfo,
            ref List<int> cellNums)
        {
            if (mapInfo == null)
            {
                Debug.LogWarning("mapInfo为Null！");
                return;
            }

            if (worldPosArr == null || worldPosArr.Length != 3)
            {
                Debug.LogWarning("传入的三角面数据错误，无数据或顶点数不是3！");
                return;
            }

            Vector2[] worldPos2D = new Vector2[3]
            {
            new Vector2(worldPosArr[0].x,worldPosArr[0].z),
            new Vector2(worldPosArr[1].x,worldPosArr[1].z),
            new Vector2(worldPosArr[2].x,worldPosArr[2].z)
            };

            if (cellNums == null)
                cellNums = new List<int>();

            //先算出每个顶点所在格子
            foreach (var worldPos in worldPos2D)
            {
                //算出所在xz行列,进而得出所在格子编号
                int xNum = (int)((worldPos.x - mapInfo.xMin) / mapInfo.xCellSize);
                int zNum = (int)((worldPos.y - mapInfo.zMin) / mapInfo.zCellSize);

                //越界规范
                if (xNum < 0)
                    xNum = 0;
                else if (xNum >= mapInfo.xCellCount)
                    xNum = mapInfo.xCellCount - 1;
                if (zNum < 0)
                    zNum = 0;
                else if (zNum >= mapInfo.zCellCount)
                    zNum = mapInfo.zCellCount - 1;

                int num = xNum + zNum * mapInfo.xCellCount;

                if (!cellNums.Contains(num))
                    cellNums.Add(num);
            }

            //如果面片较小，则不进行后面运算
            for (int i = 0; i < worldPos2D.Length; i++)
            {
                int j = i + 1;
                if (j >= worldPos2D.Length)
                    j = 0;

                float len = (worldPos2D[i] - worldPos2D[j]).magnitude;
                if (len < Mathf.Min(mapInfo.xCellSize, mapInfo.zCellSize))
                    return;
            }

            //---通过格子边框与三角形的边是否相交计算---

            List<int> otherAxis = new List<int>();   //交点列表;

            //遍历行边框
            for (int i = 0; i < mapInfo.zCellCount - 1; i++)
            {
                float zVal = mapInfo.zMin + (i + 1) * mapInfo.zCellSize;
                otherAxis.Clear();
                for (int j1 = 0; j1 < worldPos2D.Length; j1++)//遍历3条边，求与此边框线是否相交
                {
                    int j2 = j1 + 1;
                    if (j2 >= worldPos2D.Length)
                        j2 = 0;

                    if (zVal > worldPos2D[j1].y && zVal < worldPos2D[j2].y
                        || zVal > worldPos2D[j2].y && zVal < worldPos2D[j1].y)
                    {
                        float xVal = (zVal - worldPos2D[j1].y) * (worldPos2D[j2].x - worldPos2D[j1].x) /
                            (worldPos2D[j2].y - worldPos2D[j1].y) + worldPos2D[j1].x;
                        int xNum = (int)(xVal / mapInfo.xCellSize);     //交点所在位置对应的格子列数
                        if (xNum < 0)
                            xNum = 0;
                        else if (xNum >= mapInfo.xCellCount)
                            xNum = mapInfo.xCellCount - 1;

                        if (!otherAxis.Contains(xNum))
                            otherAxis.Add(xNum);
                    }
                }
                if (otherAxis.Count == 0)
                    continue;
                if (otherAxis.Count > 2)
                {
                    Debug.LogWarning("计算错误，一条直线与三角形不可能相交于3个点");
                    continue;
                }

                int xNumMin, xNumMax;
                if (otherAxis.Count == 2)
                {
                    xNumMin = Mathf.Min(otherAxis[0], otherAxis[1]);
                    xNumMax = Mathf.Max(otherAxis[0], otherAxis[1]);
                }
                else
                    xNumMin = xNumMax = otherAxis[0];

                for (int xNum = xNumMin; xNum <= xNumMax; xNum++)
                {
                    //交点上下两个格子都要添加
                    int num = xNum + i * mapInfo.xCellCount;
                    if (!cellNums.Contains(num))
                        cellNums.Add(num);
                    num = xNum + (i + 1) * mapInfo.xCellCount;
                    if (!cellNums.Contains(num))
                        cellNums.Add(num);
                }
            }

            //遍历列边框
            for (int i = 0; i < mapInfo.xCellCount - 1; i++)
            {
                float xVal = mapInfo.xMin + (i + 1) * mapInfo.xCellSize;
                otherAxis.Clear();
                for (int j1 = 0; j1 < worldPos2D.Length; j1++)//遍历3条边，求与此边框线是否相交
                {
                    int j2 = j1 + 1;
                    if (j2 >= worldPos2D.Length)
                        j2 = 0;

                    if (xVal > worldPos2D[j1].x && xVal < worldPos2D[j2].x
                        || xVal > worldPos2D[j2].x && xVal < worldPos2D[j1].x)
                    {
                        float zVal = (xVal - worldPos2D[j1].x) * (worldPos2D[j2].y - worldPos2D[j1].y) /
                            (worldPos2D[j2].x - worldPos2D[j1].x) + worldPos2D[j1].y;
                        int zNum = (int)(zVal / mapInfo.zCellSize);  //交点所在位置对应的格子行数
                        if (zNum < 0)
                            zNum = 0;
                        else if (zNum >= mapInfo.zCellCount)
                            zNum = mapInfo.zCellCount - 1;

                        if (!otherAxis.Contains(zNum))
                            otherAxis.Add(zNum);
                    }
                }
                if (otherAxis.Count == 0)
                    continue;
                if (otherAxis.Count > 2)
                {
                    Debug.LogWarning("计算错误，一条直线与三角形不可能相交于3个点");
                    continue;
                }

                int zNumMin, zNumMax;
                if (otherAxis.Count == 2)
                {
                    zNumMin = Mathf.Min(otherAxis[0], otherAxis[1]);
                    zNumMax = Mathf.Max(otherAxis[0], otherAxis[1]);
                }
                else
                    zNumMin = zNumMax = otherAxis[0];

                for (int zNum = zNumMin; zNum <= zNumMax; zNum++)
                {
                    //交点左右两个格子都要添加
                    int num = i + zNum * mapInfo.xCellCount;
                    if (!cellNums.Contains(num))
                        cellNums.Add(num);

                    num = i + 1 + zNum * mapInfo.xCellCount;
                    if (!cellNums.Contains(num))
                        cellNums.Add(num);
                }
            }
        }

        //计算xz坐标矩形
        static Vector4 GetRect(Transform root)
        {
            if (root == null)
                return Vector4.zero;

            float xMin = 0, zMin = 0, xMax = 0, zMax = 0;
            //如果地图带SceneMapPoints，指示大小
            if (root.parent != null)
            {
                Transform sceneMapPoints = root.parent.Find("SceneMapPoints");
                if (sceneMapPoints != null)
                {
                    Transform bottomLeft = sceneMapPoints.Find("BottomLeft");
                    Transform topRight = sceneMapPoints.Find("TopRight");
                    if (bottomLeft != null && topRight != null)
                    {
                        xMin = bottomLeft.position.x;
                        zMin = bottomLeft.position.z;
                        xMax = topRight.position.x;
                        zMax = topRight.position.z;
                    }
                }
            }
            else
            {
                foreach (var meshFilter in root.GetComponentsInChildren<MeshFilter>())
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    Transform trans = meshFilter.transform;
                    foreach (var vertex in mesh.vertices)
                    {
                        Vector3 worldPos = trans.TransformPoint(vertex);
                        if (worldPos.x < xMin)
                            xMin = worldPos.x;
                        else if (worldPos.x > xMax)
                            xMax = worldPos.x;
                        if (worldPos.z < zMin)
                            zMin = worldPos.z;
                        else if (worldPos.z > zMax)
                            zMax = worldPos.z;
                    }
                }
            }

            Vector4 rect = new Vector4(xMin, zMin, xMax, zMax);

            return rect;
        }

        //递归遍历节点(func返回true则不继续递归)
        static void TraverseHierarchy(Transform root, Func<Transform,bool> func)
        {
            if (root == null || func == null)
                return;

            if (!func(root))
            {
                for (int i = 0; i < root.childCount; i++)
                {
                    TraverseHierarchy(root.GetChild(i), func);
                }
            }
        }


        //-------------------- 处理地图逻辑数据(给服务器的数据) ---------------------------------
        private static SceneVO ExportMapLogicData(SceneInfo mapInfo, Transform rootTrans, SceneSizeInfo mapSizeInfo)
        {
            NavMeshTriangulation navMeshTri = NavMesh.CalculateTriangulation();

            if (navMeshTri.vertices.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有获取到该场景的navMesh烘焙数据", "确定");
                return null;
            }

            SceneVO mapData = new SceneVO();

            //算出导航图最小点和最大点（aabb方式）
            Vector2 minPoint = new Vector2(navMeshTri.vertices[0].x, navMeshTri.vertices[0].z);
            Vector2 maxPoint = minPoint;
            foreach (var point in navMeshTri.vertices)
            {
                if (point.x < minPoint.x)
                {
                    minPoint.x = point.x;
                }
                else if (point.x > maxPoint.x)
                {
                    maxPoint.x = point.x;
                }

                if (point.z < minPoint.y)
                {
                    minPoint.y = point.z;
                }
                else if (point.z > maxPoint.y)
                {
                    maxPoint.y = point.z;
                }
            }

            //xz轴格子数量（服务器用）
            int xCellNum = Mathf.CeilToInt(maxPoint.x) - Mathf.FloorToInt(minPoint.x);
            int zCellNum = Mathf.CeilToInt(maxPoint.y) - Mathf.FloorToInt(minPoint.y);

            //原点
            Vector2 oriPos = new Vector2(Mathf.Floor(minPoint.x), Mathf.Ceil(minPoint.y));
            mapInfo.oriPosVO = new MyVector2(oriPos);
            mapInfo.xCellCountVO = xCellNum;
            mapInfo.zCellCountVO = zCellNum;

            mapSizeInfo.ServerOriPos = mapInfo.oriPosVO;
            mapSizeInfo.xCellCountServer = xCellNum;
            mapSizeInfo.zCellCountServer = zCellNum;

            int cellCount = xCellNum * zCellNum;
            int[] blockArr = new int[cellCount];
            for (int i = 0; i < blockArr.Length; i++)
            {
                blockArr[i] = 1;
            }

            EditorUtility.DisplayProgressBar("提示", "正在构建可行走区域", 1.0f);
            //遍历navMesh三角面，找出所有可行走的格子
            for (int i = 0; i < navMeshTri.indices.Length / 3; i++)
            {
                Vector3 point1_3D = navMeshTri.vertices[navMeshTri.indices[i * 3]];
                Vector3 point2_3D = navMeshTri.vertices[navMeshTri.indices[i * 3 + 1]];
                Vector3 point3_3D = navMeshTri.vertices[navMeshTri.indices[i * 3 + 2]];

                //找出三角面的3个点的2D坐标(相对坐下角原点)
                Vector2[] points = new Vector2[3];
                points[0] = new Vector2(point1_3D.x, point1_3D.z) - oriPos;
                points[1] = new Vector2(point2_3D.x, point2_3D.z) - oriPos;
                points[2] = new Vector2(point3_3D.x, point3_3D.z) - oriPos;

                //算出此面经过的所有格子,并赋值到blockArr中
                GetUnitCellInTriangle(points, blockArr, xCellNum, zCellNum);
            }

            //Todo 寻找所有的安全区物体，计算其对应的格子
            int[] safeArr = new int[cellCount];
            Transform safeAreaRoot = rootTrans.parent.Find("SafeArea");
            EditorUtility.DisplayProgressBar("提示", "正在构建安全区域", 1.0f);
            if (safeAreaRoot != null)
            {
                List<Transform> safeList = new List<Transform>(safeAreaRoot.GetComponentsInChildren<Transform>());
                for (int z = 0; z < zCellNum; z++)
                {
                    for (int x = 0; x < xCellNum; x++)   //二维格子采样
                    {
                        Vector2 samplePos = oriPos + new Vector2(x + 0.5f, z + 0.5f); //因为格子永远是单位长度的
                        RaycastHit[] hits = Physics.RaycastAll(new Vector3(samplePos.x, 500.0f, samplePos.y), Vector3.down, 1000);
                        if (hits != null)
                        {
                            foreach (var hit in hits)
                            {
                                if (safeList.Contains(hit.transform))
                                {
                                    int finalNum = z * xCellNum + x;
                                    if ((blockArr[finalNum] & 1) == 0)
                                    {
                                        safeArr[finalNum] = 1;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            mapData.id = mapInfo.mapId;
            mapData.name = s_curMapName + "_SceneVO";
            mapData.width = xCellNum;
            mapData.height = zCellNum;
            mapData.tileWH = 1;
            mapData.tiltTH = 1;
            mapData.backWH = 0;
            mapData.backTH = 0;
            mapData.moveWH = 0;
            mapData.moveTH = 0;
            mapData.block = new int[cellCount];
            mapData.box = new int[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                mapData.block[i] = mapData.block[i] | blockArr[i];
                mapData.block[i] = mapData.block[i] | (safeArr[i] << 5);

                //王城战区域  << 1
            }
            CreateBoxPointData(rootTrans, xCellNum, zCellNum, mapData, mapInfo);    //创建box点数据

            //mapData.flag = new int[cellCount];
            //mapData.alpha = new int[cellCount];
            //mapData.safe = safeArr;
            //mapData.path = new int[cellCount];

            //string fullPaht = string.Format("{0}/{1}.asset", "Assets/_Yu/Example/Game/SecenDemo/AssetDatabase/Map", mapData.name);
            //AssetDatabase.CreateAsset(mapData, fullPaht);
            string fullPaht = string.Format("{0}/{1}.json", s_mapDataPath, mapData.name);
            //YuJsonUtility.WriteAsJson(fullPaht, mapData);

            return mapData;
        }

        //求出单位为1长宽的格子合集，所有经过一个三角面的
        private static void GetUnitCellInTriangle(Vector2[] points, int[] arr, int xCount, int zCount)
        {
            if (points.Length != 3)
            {
                Debug.LogError("三角形的顶点数量不是3");
                return;
            }

            //先算出每个顶点所在格子
            foreach (var pos in points)
            {
                //算出所在xz行列,进而得出所在格子编号
                int xNum = (int)pos.x;
                int zNum = (int)pos.y;

                //越界规范
                if (xNum < 0)
                    xNum = 0;
                else if (xNum >= xCount)
                    xNum = xCount - 1;
                if (zNum < 0)
                    zNum = 0;
                else if (zNum >= zCount)
                    zNum = zCount - 1;

                int num = xNum + zNum * xCount;

                arr[num] = 0;
            }

            //如果面片较小，则不进行后面运算
            for (int i = 0; i < points.Length; i++)
            {
                int j = i + 1;
                if (j >= points.Length)
                    j = 0;

                float len = (points[i] - points[j]).magnitude;
                if (len < 1.0f)
                    return;
            }

            int xMin = (int)points[0].x;
            int xMax = xMin + 1;
            int zMin = (int)points[0].y;
            int zMax = zMin + 1;

            for (int i = 0; i < 3; i++)
            {
                if (points[i].x < xMin)
                {
                    xMin = (int)points[i].x;
                }
                else if (points[i].x + 1 > xMax)
                {
                    xMax = (int)points[i].x + 1;
                }

                if (points[i].y < zMin)
                {
                    zMin = (int)points[i].y;
                }
                else if (points[i].y + 1 > zMax)
                {
                    zMax = (int)points[i].y + 1;
                }
            }

            List<int> otherAxis = new List<int>();   //交点列表;

            //遍历行边框
            for (int zVal = zMin; zVal <= zMax; zVal++)
            {
                otherAxis.Clear();
                for (int j1 = 0; j1 < points.Length; j1++)//遍历3条边，求与此边框线是否相交
                {
                    int j2 = j1 + 1;
                    if (j2 >= points.Length)
                        j2 = 0;

                    if (zVal > points[j1].y && zVal < points[j2].y
                        || zVal > points[j2].y && zVal < points[j1].y)
                    {
                        float xVal = (zVal - points[j1].y) * (points[j2].x - points[j1].x) /
                            (points[j2].y - points[j1].y) + points[j1].x;
                        int xNum = (int)xVal;     //交点所在位置对应的格子列数
                        if (xNum < 0)
                            xNum = 0;
                        else if (xNum >= xCount)
                            xNum = xCount - 1;

                        if (!otherAxis.Contains(xNum))
                            otherAxis.Add(xNum);
                    }
                }
                if (otherAxis.Count == 0)
                    continue;
                if (otherAxis.Count > 2)
                {
                    Debug.LogWarning("计算错误，一条直线与三角形不可能相交于3个点");
                    continue;
                }

                int xNumMin, xNumMax;
                if (otherAxis.Count == 2)
                {
                    xNumMin = Mathf.Min(otherAxis[0], otherAxis[1]);
                    xNumMax = Mathf.Max(otherAxis[0], otherAxis[1]);
                }
                else
                    xNumMin = xNumMax = otherAxis[0];

                for (int xNum = xNumMin; xNum <= xNumMax; xNum++)
                {
                    //交点上下两个格子都要添加
                    int num = xNum + zVal * xCount;
                    if (num >= 0 && num < arr.Length)
                        arr[num] = 0;
                    num = xNum + (zVal + 1) * xCount;
                    if (num >= 0 && num < arr.Length)
                        arr[num] = 0;
                }
            }

            //遍历列边框
            for (int xVal = xMin; xVal < xMax; xVal++)
            {
                otherAxis.Clear();
                for (int j1 = 0; j1 < points.Length; j1++)//遍历3条边，求与此边框线是否相交
                {
                    int j2 = j1 + 1;
                    if (j2 >= points.Length)
                        j2 = 0;

                    if (xVal > points[j1].x && xVal < points[j2].x
                        || xVal > points[j2].x && xVal < points[j1].x)
                    {
                        float zVal = (xVal - points[j1].x) * (points[j2].y - points[j1].y) /
                            (points[j2].x - points[j1].x) + points[j1].y;
                        int zNum = (int)zVal;  //交点所在位置对应的格子行数
                        if (zNum < 0)
                            zNum = 0;
                        else if (zNum >= zCount)
                            zNum = zCount - 1;

                        if (!otherAxis.Contains(zNum))
                            otherAxis.Add(zNum);
                    }
                }
                if (otherAxis.Count == 0)
                    continue;
                if (otherAxis.Count > 2)
                {
                    Debug.LogWarning("计算错误，一条直线与三角形不可能相交于3个点");
                    continue;
                }

                int zNumMin, zNumMax;
                if (otherAxis.Count == 2)
                {
                    zNumMin = Mathf.Min(otherAxis[0], otherAxis[1]);
                    zNumMax = Mathf.Max(otherAxis[0], otherAxis[1]);
                }
                else
                    zNumMin = zNumMax = otherAxis[0];

                for (int zNum = zNumMin; zNum <= zNumMax; zNum++)
                {
                    //交点左右两个格子都要添加
                    int num = xVal + zNum * xCount;
                    if (num >= 0 && num < arr.Length)
                        arr[num] = 0;

                    num = xVal + 1 + zNum * xCount;
                    if (num >= 0 && num < arr.Length)
                        arr[num] = 0;
                }

            }
        }


        //----------------------------------编辑刷怪数据----------------------------------
        private static void CreateMonsterTeamData(SceneVO voData, SceneInfo mapInfo)
        {
            //if (voData == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Monster";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}
            //Transform rootParent = Selection.activeGameObject.transform.parent;
            //if (rootParent == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Monster";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}
            //Transform root = rootParent.Find("MonsterTeams");
            //if (root == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Monster";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}

            ////获取各个刷怪点的设置数据
            //List<Transform> settings = new List<Transform>();
            //foreach (var trans in root.GetComponentsInChildren<Transform>())
            //{
            //    if (trans != root)
            //    {
            //        settings.Add(trans);
            //    }
            //}
            //List<YuMonsterTeamData> teamList = new List<YuMonsterTeamData>();
            //int teamNum = 0;
            //foreach (var setting in settings)
            //{
            //    string[] nameSplits = setting.name.Split('#');
            //    if (nameSplits != null && nameSplits.Length != 2)
            //    {
            //        continue;
            //    }
            //    string monsterName = nameSplits[0];
            //    string monsterId = nameSplits[1].Split('_')[0];
            //    int maxNum = int.Parse(nameSplits[1].Split('_')[1]);


            //    bool exist = false;
            //    Point2 pos = YuGraphAlgorithm.GetCoordByPosition(setting.transform.position,
            //                mapInfo.oriPosVO.ToVector2(), voData.width, voData.height);
            //    foreach (var team in teamList) //遍历已有怪物队伍数据，是否已有队伍信息与此设置一样
            //    {
            //        if (team.monsterId == monsterId
            //            && team.maxCount == maxNum)
            //        {
            //            //如果有，则添加这个坐标
            //            exist = true;
            //            List<Point2> posList = new List<Point2>(team.createPoints);


            //            posList.Add(pos);
            //            team.createPoints = posList.ToArray();
            //            break;
            //        }
            //    }
            //    if (!exist) //如果不存在，则新建怪物队伍数据
            //    {

            //        YuMonsterTeamData team = new YuMonsterTeamData();
            //        team.teamNum = teamNum;
            //        teamNum++;
            //        team.monsterId = monsterId;
            //        team.monsterName = monsterName;
            //        team.maxCount = maxNum;
            //        team.createPoints = new Point2[1];
            //        team.createPoints[0] = pos;
            //        teamList.Add(team);
            //    }
            //}

            //MonsterDataBag dataBag = new MonsterDataBag();
            //dataBag.monsterTeams = teamList.ToArray();

            //string fileName = s_curMapName + "_Monster";
            //string fullPaht = string.Format("{0}/{1}.json", s_mapDataPath, fileName);
            //YuJsonUtility.WriteAsJson(fullPaht, dataBag);
        }


        //----------------------------------编辑收集物数据----------------------------------
        private static void CreateCollectionTeamData(SceneVO voData, SceneInfo mapInfo)
        {
            //if (voData == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Collection";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}
            //Transform rootParent = Selection.activeGameObject.transform.parent;
            //if (rootParent == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Collection";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}
            //Transform root = rootParent.Find("CollectionTeams");
            //if (root == null)
            //{
            //    MonsterDataBag dataBagTemp = new MonsterDataBag();
            //    string fileNameTemp = s_curMapName + "_Collection";
            //    string fullPahtTemp = string.Format("{0}/{1}.json", s_mapDataPath, fileNameTemp);
            //    //YuJsonUtility.WriteAsJson(fullPahtTemp, dataBagTemp);
            //    return;
            //}

            ////获取各个刷怪点的设置数据
            //List<Transform> settings = new List<Transform>();
            //foreach (var trans in root.GetComponentsInChildren<Transform>())
            //{
            //    if (trans != root)
            //    {
            //        settings.Add(trans);
            //    }
            //}
            //List<YuMonsterTeamData> teamList = new List<YuMonsterTeamData>();
            //int teamNum = 0;
            //foreach (var setting in settings)
            //{
            //    string[] nameSplits = setting.name.Split('#');
            //    if (nameSplits != null && nameSplits.Length != 2)
            //    {
            //        continue;
            //    }
            //    string collectionName = nameSplits[0];
            //    string collectionId = nameSplits[1].Split('_')[0];
            //    int maxNum = int.Parse(nameSplits[1].Split('_')[1]);


            //    bool exist = false;
            //    Point2 pos = YuGraphAlgorithm.GetCoordByPosition(setting.transform.position,
            //                mapInfo.oriPosVO.ToVector2(), voData.width, voData.height);
            //    foreach (var team in teamList) //遍历已有怪物队伍数据，是否已有队伍信息与此设置一样
            //    {
            //        if (team.monsterId == collectionId
            //            && team.maxCount == maxNum)
            //        {
            //            //如果有，则添加这个坐标
            //            exist = true;
            //            List<Point2> posList = new List<Point2>(team.createPoints);


            //            posList.Add(pos);
            //            team.createPoints = posList.ToArray();
            //            break;
            //        }
            //    }
            //    if (!exist) //如果不存在，则新建收集物队伍数据
            //    {

            //        YuMonsterTeamData team = new YuMonsterTeamData();
            //        team.teamNum = teamNum;
            //        teamNum++;
            //        team.monsterId = collectionId;
            //        team.monsterName = collectionName;
            //        team.maxCount = maxNum;
            //        team.createPoints = new Point2[1];
            //        team.createPoints[0] = pos;
            //        teamList.Add(team);
            //    }
            //}

            //MonsterDataBag dataBag = new MonsterDataBag();
            //dataBag.monsterTeams = teamList.ToArray();

            //string fileName = s_curMapName + "_Collection";
            //string fullPaht = string.Format("{0}/{1}.json", s_mapDataPath, fileName);
            //YuJsonUtility.WriteAsJson(fullPaht, dataBag);
        }

        //------------------------------编辑刷宝箱点数据----------------------------------
        private static void CreateBoxPointData(Transform rootTrans, int xCellNum, int zCellNum,
           SceneVO voData, SceneInfo mapInfo)
        {
            Transform boxAreaRoot = rootTrans.parent.Find("BoxArea");
            EditorUtility.DisplayProgressBar("提示", "正在构建安全区域", 1.0f);
            if (boxAreaRoot != null)
            {
                List<Transform> boxList = new List<Transform>(boxAreaRoot.GetComponentsInChildren<Transform>());
                for (int z = 0; z < zCellNum; z++)
                {
                    for (int x = 0; x < xCellNum; x++)   //二维格子采样
                    {
                        Vector2 samplePos = mapInfo.oriPosVO.ToVector2() + new Vector2(x + 0.5f, z + 0.5f); //因为格子永远是单位长度的
                        RaycastHit[] hits = Physics.RaycastAll(new Vector3(samplePos.x, 500.0f, samplePos.y), Vector3.down, 1000);
                        if (hits != null)
                        {
                            foreach (var hit in hits)
                            {
                                if (boxList.Contains(hit.transform))
                                {
                                    int finalNum = z * xCellNum + x;
                                    if ((voData.block[finalNum] & 1) == 0)
                                    {
                                        voData.box[finalNum] = 1;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //创建高度图
        private static void CreateHeightMap(Transform rootTrans, SceneInfo mapInfo, string fullPathName)
        {
            EditorUtility.DisplayProgressBar("提示", "正在生成高度图", 1.0f);

            Transform mapRoot = GameObject.Find("HeightMap").transform;
            if (mapRoot == null)
            {
                EditorUtility.DisplayDialog("提示", "没有高度物体", "确定");
                return;
            }

            int count = 1024;

            Texture2D heightMap = new Texture2D(count, count, TextureFormat.RG16, false, false);
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;
            float zMin = float.MaxValue;
            float zMax = float.MinValue;

            List<Transform> transList = new
                List<Transform>(mapRoot.GetComponentsInChildren<Transform>());

            foreach (var meshFilter in mapRoot.GetComponentsInChildren<MeshCollider>())
            {
                Mesh mesh = meshFilter.sharedMesh;
                if (mesh == null)
                {
                    continue;
                }
                Transform trans = meshFilter.transform;
                foreach (var vertex in mesh.vertices)
                {
                    Vector3 worldPos = trans.TransformPoint(vertex);
                    if (worldPos.x < xMin)
                        xMin = worldPos.x;
                    else if (worldPos.x > xMax)
                        xMax = worldPos.x;
                    if (worldPos.y < yMin)
                        yMin = worldPos.y;
                    else if (worldPos.y > yMax)
                        yMax = worldPos.y;
                    if (worldPos.z < zMin)
                        zMin = worldPos.z;
                    else if (worldPos.z > zMax)
                        zMax = worldPos.z;
                }
            }
            foreach (var collider in mapRoot.GetComponentsInChildren<Collider>())
            {
                if (collider is MeshCollider)
                {
                    continue;
                }
                Vector3 minPos = collider.bounds.min;
                Vector3 maxPos = collider.bounds.max;

                if (minPos.x < xMin)
                    xMin = minPos.x;
                if (maxPos.x > xMax)
                    xMax = maxPos.x;
                if (minPos.y < yMin)
                    yMin = minPos.y;
                if (maxPos.y > yMax)
                    yMax = maxPos.y;
                if (minPos.z < zMin)
                    zMin = minPos.z;
                if (maxPos.z > zMax)
                    zMax = maxPos.z;
            }


            float deltaX = (xMax - xMin) / count;
            float deltaZ = (zMax - zMin) / count;
            float height = yMax - yMin;

            if (mapInfo != null)
            {
                mapInfo.heightMapOriX = xMin;
                mapInfo.heightMapOriZ = zMin;
                mapInfo.heightMapSizeX = xMax - xMin;
                mapInfo.heightMapSizeZ = zMax - zMin;
                mapInfo.heightMapMinY = yMin;
                mapInfo.heightMapSizeY = height;
            }

            bool[] isFindArr = new bool[count * count];
            Color[] resetArr = new Color[count * count];
            for (int z = 0; z < count; z++)
            {
                for (int x = 0; x < count; x++)
                {
                    Vector3 pos = new Vector3(xMin + (x + 0.5f) * deltaX,
                        yMax + 1.0f, zMin + (z + 0.5f) * deltaZ);

                    heightMap.SetPixel(x, z, new Color(0, 0, 0));

                    RaycastHit[] hitArr = Physics.RaycastAll(pos, Vector3.down, height + 2.0f);
                    isFindArr[x + z * count] = false;
                    if (hitArr != null)
                    {
                        float maxHeight = yMin;
                        foreach (var hit in hitArr)
                        {
                            if (transList.Contains(hit.transform))
                            {
                                if (hit.point.y > maxHeight)
                                {
                                    maxHeight = hit.point.y;
                                    isFindArr[x + z * count] = true;
                                }
                            }
                        }

                        float heightValue = (maxHeight - yMin) / height;
                        ushort shortValue = (ushort)(heightValue * 65535.0f);
                        byte[] byteArr = BitConverter.GetBytes(shortValue);

                        //float rValue = (((int)(heightValue * 65536)) / 256) / 256.0f;
                        //float gValue = (((int)(heightValue * 65536)) % 256) / 256.0f;
                        heightMap.SetPixel(x, z, new Color32(byteArr[0], byteArr[1], 0,0));
                    }

                }
            }

            //没有采样到高度信息的，值设置为周围2格有高度信息的，以避免边界误差
            for (int z = 0; z < count; z++)
            {
                for (int x = 0; x < count; x++)
                {
                    if (!isFindArr[x + z * count])       //如果这个像素没获取到高度
                    {
                        Vector4 heightValue = Vector4.zero;
                        int findCount = 0;
                        for (int zNear = z - 5; zNear <= z + 5; zNear++)
                        {
                            if (zNear < 0 || zNear >= count)
                            {
                                continue;
                            }

                            for (int xNear = x - 5; xNear <= x + 5; xNear++)
                            {
                                if (xNear < 0 || xNear >= count)
                                {
                                    continue;
                                }
                                if (isFindArr[xNear + zNear * count])
                                {
                                    heightValue += new Vector4(heightMap.GetPixel(xNear, zNear).r, 
                                        heightMap.GetPixel(xNear, zNear).g, heightMap.GetPixel(xNear, zNear).b, 1) ;
                                    findCount ++;
                                }
                            }
                        }
                        if(findCount >1)
                        {
                            heightValue = heightValue / (float)findCount;
                        }

                        resetArr[x + z * count] = heightValue;
                    }
                }
            }

            for (int z = 0; z < count; z++)
            {
                for (int x = 0; x < count; x++)
                {
                    if (!isFindArr[x + z * count] &&
                        resetArr[x + z * count].r > 0.001f)
                    {
                        heightMap.SetPixel(x, z, resetArr[x + z * count]);
                    }

                }
            }

            byte[] bytes = heightMap.EncodeToPNG();
            string fullPath = Application.dataPath;
            fullPath = fullPath.Remove(fullPath.IndexOf("Assets"));
            fullPath = fullPath + fullPathName;
            File.WriteAllBytes(fullPathName, bytes);

            AssetDatabase.Refresh();

            TextureImporter texImporter = TextureImporter.GetAtPath(fullPathName) as TextureImporter;
            if (texImporter != null)
            {
                TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                setting.format = TextureImporterFormat.RG16;

                texImporter.SetPlatformTextureSettings(setting);
                texImporter.isReadable = true;
                texImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(fullPathName);
            }

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Export/单独生成高度图")]
        private static void CreateHeightMap()
        {
            try
            {
                GameObject selectObj = Selection.activeGameObject;

                s_mapDataPath = "AssetDatabase/Package_00/MapData";

                if (selectObj != null && //selectObj.GetComponent<MapExportSetting>() != null)
                    selectObj.name.Contains("#"))
                {
                    s_curMapName = selectObj.name.Split('#')[0];
                    string setString = selectObj.name.Split('#')[1];
                    string[] settings = setString.Split('_');
                    if (settings.Length != 3)
                    {
                        EditorUtility.DisplayDialog("提示",
                            "没有选择正确命名的地图gameobject,格式：地图名#x格数_z格数_地图id", "确定");
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示",
                            "没有选择正确命名的地图gameobject,格式：地图名#x格数_z格数_地图id", "确定");
                    return;
                }

                string fullPaht = string.Format("{0}/{1}/{2}.json", s_mapDataPath, s_curMapName, s_curMapName);
                SceneInfo mapInfo = null;
                var jsContent = File.ReadAllText(fullPaht);
                if (jsContent != null)
                {
                    //mapInfo = YuJsonUtility.FromJson<SceneInfo>(jsContent);
                }


                CreateHeightMap(selectObj.transform, mapInfo,
                        string.Format("{0}/{1}/{2}_heightmap.png", s_mapDataPath, s_curMapName, s_curMapName));

                //YuJsonUtility.WriteAsJson(fullPaht, mapInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("提示", "生成高度图失败", "确认");
            }
        }

        [MenuItem("Export/重置高度图的格式/所有")]
        private static void ResetMapTextureFormat()
        {
            string path = //YuU3dAppSettingDati.CurrentActual.AppRootDir
                    "AssetDatabase/Package_00/MapData";
            path = Application.dataPath + path.Remove(path.IndexOf("Assets"),6);

            DirectoryInfo direction = new DirectoryInfo(path);
            var directions = direction.GetDirectories();
            int count = 0;
            foreach (var dir in directions)
            {
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)
                {
                    string assetPath = file.FullName.Replace("\\", "/");
                    assetPath = assetPath.Remove(0, Application.dataPath.Length - 6);

                    TextureImporter texImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
                    if (texImporter != null)
                    {
                        TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                        setting.format = TextureImporterFormat.RG16;

                        texImporter.SetPlatformTextureSettings(setting);
                        texImporter.isReadable = true;
                        texImporter.SaveAndReimport();
                        AssetDatabase.ImportAsset(assetPath);
                        count++;
                    }
                }
            }
         
           
           
            EditorUtility.DisplayDialog("完成", "完成了 " + count + " 个图片的格式修改", "确认");

            AssetDatabase.Refresh();
        }

        [MenuItem("Export/重置高度图的格式/选定目标")]
        private static void ResetMapTextureFormat2()
        {
            var select = Selection.activeObject;
            if(select == null)
            {
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(select);
            TextureImporter texImporter = TextureImporter.GetAtPath(assetPath) as TextureImporter;
            if (texImporter != null)
            {
                TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                setting.format = TextureImporterFormat.RG16;

                texImporter.SetPlatformTextureSettings(setting);
                texImporter.isReadable = true;
                texImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(assetPath);
            }

            AssetDatabase.Refresh();
        }
    }

    [Serializable]
    public class MonsterDataBag
    {
        //public YuMonsterTeamData[] monsterTeams;
    }

}

