using Common.Utility;
using GameWorld;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Client.Scene.Editor
{
    [Serializable]
    public class SceneEditor
    {
        #region 可视化界面

        [Title("场景编辑窗口", TitleAlignment = TitleAlignments.Centered)]
        [LabelText("场景根节点")]
        [LabelWidth(70)]
        [PropertyTooltip("场景中挂载 SceneRoot 组件的游戏物体")]
        [InlineButton("AnalyzeCurrentScene", "解析当前场景")]
        public GameObject SceneRoot;

        [TabGroup("场景解析")]
        [HideLabel]
        public SceneInfoInEditor CurrentSceneInfo;

        [TabGroup("场景解析")]
        [Space(10)]
        [LabelText("场景数据保存路径")]
        [InlineButton("SaveSceneAnalysisInfo", "保存")]
        [LabelWidth(100)]
        [FolderPath]
        public string SceneInfoStorePath;
        // = Application.dataPath + "/GameWorld/TestScene/MapData/"

        [TabGroup("场景格")]
        [HideLabel]
        public SceneCellEditor CellEditor;
        #endregion

        #region 解析场景信息

        private void AnalyzeCurrentScene()
        {
            if (SceneRoot == null)
            {
                SceneRoot = GameObject.FindObjectOfType<SceneRoot>().gameObject;
            }
            if (SceneRoot == null)
            {
                Debug.LogError("未选择地图的根物体");
                return;
            }
            string[] curSceneParams = SceneRoot.name.Split('#');
            if (curSceneParams.Length != 4)
            {
                Debug.LogError("没有选择正确命名的地图gameobject,格式：地图名#x格数_z格数_地图id");
                return;
            }

            CurrentSceneInfo = new SceneInfoInEditor();
            CurrentSceneInfo.mapName = curSceneParams[0];
            int.TryParse(curSceneParams[1], out CurrentSceneInfo.xCellCount);
            int.TryParse(curSceneParams[2], out CurrentSceneInfo.zCellCount);
            int.TryParse(curSceneParams[3], out CurrentSceneInfo.mapId);

            //计算场景大小
            var currentSceneRect = GetRect(SceneRoot.transform);
            float xMin = currentSceneRect.x, xMax = currentSceneRect.z, zMin = currentSceneRect.y, zMax = currentSceneRect.w;

            //mapSizeInfo.UnityMin = new MyVector2(xMin, zMin);
            //mapSizeInfo.UnityMax = new MyVector2(xMax, zMax);

            //设置原点、划分格子
            CurrentSceneInfo.originPos = new Vector2(xMin, zMin);
            CurrentSceneInfo.xCellSize = (xMax - xMin) / CurrentSceneInfo.xCellCount;
            CurrentSceneInfo.zCellSize = (zMax - zMin) / CurrentSceneInfo.zCellCount;
            CurrentSceneInfo.skyboxMaterial = RenderSettings.skybox;
            SetSceneCellObjectInfo(SceneRoot.transform, CurrentSceneInfo);
            SetLightMapInfo(CurrentSceneInfo);
            SetLightInfo(SceneRoot.transform, CurrentSceneInfo);
            SetFogInfo(CurrentSceneInfo);
            GetHeightMap(SceneRoot.transform, CurrentSceneInfo);
            ////创建navMesh数据
            //NavMeshTriangulation navMeshTri = NavMesh.CalculateTriangulation();
            //info.navMeshData = YuNavMeshDataCreate.CreateNavMeshData(navMeshTri);

            SetCellEditorTab();
        }

        private Vector4 GetRect(Transform root)
        {
            if (root == null)
                return Vector4.zero;

            float xMin = 0, zMin = 0, xMax = 0, zMax = 0;
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
            Vector4 rect = new Vector4(xMin, zMin, xMax, zMax);
            return rect;
        }

        private void SetLightMapInfo(SceneInfoInEditor sceneInfo)
        {
            if (LightmapSettings.lightmaps.Length == 0)
            {
                sceneInfo = null;
                return ;
            }
            //lightmap图片资源（不参与资源打包）
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
                }

            }

            SceneLightInfo lmInfo = new SceneLightInfo();
            lmInfo.lightmapTexNames = texNames;
            lmInfo.lightmapTexPath = texPaths;
            lmInfo.dataIndexes = dataIndex.ToArray();
            lmInfo.types = lmType.ToArray();
            lmInfo.lightmapMode = LightmapSettings.lightmapsMode;

            sceneInfo.lightmapInfo = lmInfo;
        }

        private void SetLightInfo(Transform trans, SceneInfoInEditor sceneInfo)
        {
            if (trans.parent == null)
            {
                sceneInfo.hasLight = false;
                return;
            }
            Transform lightTrans = trans.parent.Find("RoleLight");
            if (lightTrans == null)
            {
                sceneInfo.hasLight = false;
                return;
            }
            Light light = lightTrans.GetComponent<Light>();
            if (light == null)
            {
                sceneInfo.hasLight = false;
                return;
            }

            sceneInfo.hasLight = true;
            sceneInfo.lightColor = light.color;
            sceneInfo.lightIntensity = light.intensity;
            sceneInfo.lightPos =  lightTrans.position;
            sceneInfo.lightDir =  lightTrans.forward;
        }

        private void SetFogInfo(SceneInfoInEditor sceneInfo)
        {
            //场景雾
            if (RenderSettings.fog)
            {
                sceneInfo.hasFog = true;
                sceneInfo.fogColor = RenderSettings.fogColor;
                sceneInfo.fogMode = (int)RenderSettings.fogMode;
                sceneInfo.fogStart = RenderSettings.fogStartDistance;
                sceneInfo.fogEnd = RenderSettings.fogEndDistance;
                sceneInfo.fogDensity = RenderSettings.fogDensity;
            }
            else
            {
                sceneInfo.hasFog = false;
                sceneInfo.fogColor = Color.white;
                sceneInfo.fogMode = (int)FogMode.Linear;
                sceneInfo.fogStart = 0;
                sceneInfo.fogEnd = 20;
            }
        }

        private void SetSceneCellObjectInfo(Transform sceneRoot, SceneInfoInEditor sceneInfo)
        {
            //初始化节点、预制物容器
            List<SceneLayerInfo> layerList = new List<SceneLayerInfo>();
            List<SceneItemInfoInEditor> prefabList = new List<SceneItemInfoInEditor>();
            SceneCellInfoInEditor[] cellsInfo = new SceneCellInfoInEditor[CurrentSceneInfo.xCellCount * CurrentSceneInfo.zCellCount];
            for (int i = 0; i < cellsInfo.Length; i++)
            {
                cellsInfo[i] = new SceneCellInfoInEditor();
                cellsInfo[i].cellId = i;
                cellsInfo[i].itemsNum = new List<int>();
                cellsInfo[i].OnlyInThisCellItemIds = new List<int>();
            }

            //prefab总数
            int prefabNum = 0;
            TraverseHierarchy(sceneRoot.transform,(Transform trans) =>
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

            //前序遍历所有节点，视prefab为叶
            TraverseHierarchy(sceneRoot.transform,(Transform trans) =>
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

                        SceneItemInfoInEditor itemInfo = new SceneItemInfoInEditor();
                        itemInfo.parentLayer = MyMapTool.GetELaterByName(trans.parent.name, sceneRoot.name);

                        List<int> numList;
                        if (itemInfo.parentLayer == EMapLayer.Animation)
                        {
                            //如果是animation类型，则每个格子均加载
                            numList = new List<int>(sceneInfo.CellCount);
                            for (int cellNum = 0; cellNum < sceneInfo.CellCount; cellNum++)
                            {
                                numList.Add(cellNum);
                            }
                        }
                        else
                        {
                            numList = GetCellIdByGameObjectMesh(trans.gameObject, sceneInfo); //算出此游戏对象所在格子集合
                        }

                        if (numList != null && numList.Count > 0 && itemInfo.parentLayer != EMapLayer.None)
                        {

                            //初始化物体info
                            prefabList.Add(itemInfo);
                            itemInfo.obj = trans.gameObject;
                            itemInfo.pos = trans.position;
                            itemInfo.scale = trans.lossyScale;
                            itemInfo.rot = trans.eulerAngles;
                            itemInfo.objNum = itemNum;
                            itemInfo.objName = trans.name;
                            itemInfo.prefabName = objPrefab.name;
                            itemInfo.prefabAssetPath = prefabPath;

                            //保存Item lightmap信息
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

                            if(numList.Count == 1)
                            {
                                cellsInfo[numList[0]].OnlyInThisCellItemIds.Add(itemInfo.objNum);
                                cellsInfo[numList[0]].itemsNum.Add(itemInfo.objNum);
                            }
                            else
                            {
                                foreach (int num in numList)     //遍历对应编号的格子info，存入物体info
                                {
                                    if (num >= 0 && num < cellsInfo.Length)
                                    {
                                        cellsInfo[num].itemsNum.Add(itemInfo.objNum);
                                    }
                                }
                            }
                            itemNum++;
                        }
                        return true;
                    }
                    else  //如果对象不是预制物，则认为是层对象
                    {
                        string layerName = trans.name;
                        if (trans.gameObject == sceneRoot)
                        {
                            layerName = layerName.Split('#')[0];
                        }

                        SceneLayerInfo layerInfo = new SceneLayerInfo();
                        layerInfo.layerName = layerName;
                        layerInfo.pos = new MyVector3(trans.position);
                        layerInfo.rot = new MyVector3(trans.eulerAngles);
                        layerInfo.scal = new MyVector3(trans.lossyScale);
                        if (trans.parent != null && trans != sceneRoot.transform)
                            layerInfo.parentLayer = MyMapTool.GetELaterByName(trans.parent.name, sceneRoot.name);

                        layerList.Add(layerInfo);
                    }
                    return isPrefab;
                }
                );
            EditorUtility.ClearProgressBar();
            CurrentSceneInfo.layers = layerList.ToArray();
            CurrentSceneInfo.items = prefabList.ToArray();
            CurrentSceneInfo.cells = cellsInfo;
        }

        //递归遍历节点(func返回true则不继续递归)
        private void TraverseHierarchy(Transform root, Func<Transform, bool> func)
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

        //判断一个物体对应的格子编号集合 
        //Todo 
        // 1、需要考虑子物体的mesh，
        // 2、需要考虑少量顶点组成的大面片所处所有格子 (已完成，需要研究是否可以改进算法逻辑)
        private List<int> GetCellIdByGameObjectMesh(GameObject obj, SceneInfoInEditor mapInfo)
        {
            if (obj == null || mapInfo == null)
                return null;

            List<int> insideCellIdList = new List<int>();
            Mesh mesh = null;
            if (obj.GetComponent<MeshFilter>() != null)
                mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            if (mesh != null)
            {
                //地图边界
                float xMin = mapInfo.xMin;
                float zMin = mapInfo.zMin;

                //格子数量
                int cellCount = mapInfo.xCellCount * mapInfo.zCellCount; 

                List<Vector3> vertexList = new List<Vector3>();

                //缩放变换后的xz坐标size
                Vector2 meshSize = new Vector2(mesh.bounds.size.x * obj.transform.lossyScale.x,
                    mesh.bounds.size.z * obj.transform.lossyScale.z);

                //如果尺寸不超过一个格子的长或宽,则进行简易检测
                if (true)//meshSize.magnitude <= Mathf.Min(mapInfo.xCellSize, mapInfo.zCellSize))
                {
                    //遍历顶点
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
                        else if (zNum >= mapInfo.zCellCount)
                            zNum = mapInfo.zCellCount - 1;

                        int num = xNum + zNum * mapInfo.xCellCount;

                        if (!insideCellIdList.Contains(num))
                        {
                            insideCellIdList.Add(num);
                        }
                    }
                }
                else//如果尺寸超过一个格子的长或宽,则检测每个三角形面片所包裹的格子
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
                        GetCellsByTriangle(verticesWorld, mapInfo, ref insideCellIdList);
                    }
                }
            }
            else // 如果预制物不含mesh，则视为一个点
            {
                //地图边界
                float xMin = mapInfo.xMin;
                float zMin = mapInfo.zMin;

                int cellCount = mapInfo.xCellCount * mapInfo.zCellCount; //格子数

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

                if (!insideCellIdList.Contains(num))
                    insideCellIdList.Add(num);
            }

            return insideCellIdList;
        }

        //计算一个三角形面所在场景格子编号集合,顶点信息为世界坐标（类似光栅化算法）
        private void GetCellsByTriangle(Vector3[] worldPosArr, SceneInfoInEditor mapInfo,ref List<int> cellNums)
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

        //获取高度图
        private void GetHeightMap(Transform rootTrans, SceneInfoInEditor mapInfo)
        {
            Transform SceneHeightRoot = GameObject.Find("HeightMap").transform;
            if (SceneHeightRoot == null)
            {
                Debug.LogError("没有高度物体");
                mapInfo.ConsiderHeightMap = false;
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
                List<Transform>(SceneHeightRoot.GetComponentsInChildren<Transform>());

            foreach (var meshFilter in SceneHeightRoot.GetComponentsInChildren<MeshCollider>())
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
            foreach (var collider in SceneHeightRoot.GetComponentsInChildren<Collider>())
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
                mapInfo.ConsiderHeightMap = true;
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
                        heightMap.SetPixel(x, z, new Color32(byteArr[0], byteArr[1], 0, 0));
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
                                        heightMap.GetPixel(xNear, zNear).g, heightMap.GetPixel(xNear, zNear).b, 1);
                                    findCount++;
                                }
                            }
                        }
                        if (findCount > 1)
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
        }

        public void SetSceneGameObjectInCell(int cellID)
        {
            SceneCellInfoInEditor cellInfo = new SceneCellInfoInEditor();

            foreach(var cell in CurrentSceneInfo.cells)
            {
                if (cell.cellId == cellID)
                {
                    cellInfo = cell;
                }
            }
            if(cellInfo == null)
            {
                return;
            }

            var itemIds = cellInfo.itemsNum;
            var allItemIds = CurrentSceneInfo.items;
            List<GameObject> gameObjectIncell = new List<GameObject>();
            foreach(var targetItemId in itemIds)
            {
                foreach(var item in allItemIds)
                {
                    if(targetItemId == item.objNum && !gameObjectIncell.Contains(item.obj))
                    {
                        gameObjectIncell.Add(item.obj);
                    }
                }
            }
            if(gameObjectIncell.Count > 0)
            {
                Selection.objects = gameObjectIncell.ToArray();
                EditorGUIUtility.PingObject(gameObjectIncell[0]);
            }
        }

        #endregion

        #region 保存场景信息

        private void SaveSceneAnalysisInfo()
        {
            string fullPath = string.Format("{0}/{1}/{2}.json", SceneInfoStorePath, CurrentSceneInfo.mapName, CurrentSceneInfo.mapName);
            YuJsonUtility.WriteAsJson(fullPath, CurrentSceneInfo);
            ////YuJsonUtility.WriteAsJson(mapSizeInfoFilePath, allMapSizeInfo);
            AssetDatabase.Refresh();
        }

        private void SaveScenesTotalInfo()
        {
            //读取记录所有场景尺寸的文件
            string mapSizeInfoPath = string.Format("{0}/MapSizeInfo/", SceneInfoStorePath);
            string mapSizeInfoFilePath = string.Format("{0}/MapSizeInfo/MapSizeInfo.json", SceneInfoStorePath);

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
            if (!allMapSizeInfo.MapNameList.Contains(CurrentSceneInfo.mapName))
            {
                allMapSizeInfo.MapNameList.Add(CurrentSceneInfo.mapName);
                allMapSizeInfo.MapSizeList.Add(new SceneSizeInfo());
            }
            var mapSizeInfo = allMapSizeInfo.MapSizeList[allMapSizeInfo.MapNameList.IndexOf(CurrentSceneInfo.mapName)];

            if (!Directory.Exists(mapSizeInfoPath))
            {
                Directory.CreateDirectory(mapSizeInfoPath);
            }

            string curMapPath = string.Format("{0}/{1}", SceneInfoStorePath, CurrentSceneInfo.mapName);

            if (!Directory.Exists(curMapPath))
            {
                Directory.CreateDirectory(curMapPath);
            }
        }

        //获取高度图
        private static void SaveHeightMap(Transform rootTrans, SceneInfoInEditor mapInfo, string fullPathName)
        {
            EditorUtility.DisplayProgressBar("提示", "正在生成高度图", 1.0f);

            Transform SceneHeightRoot = GameObject.Find("HeightMap").transform;
            if (SceneHeightRoot == null)
            {
                Debug.LogError("没有高度物体");
                mapInfo.ConsiderHeightMap = false;
                return;
            }

            int count = 1024;

            Texture2D heightMap = new Texture2D(count, count, TextureFormat.RG16, false, false);
            float xMin = mapInfo.heightMapOriX;
            float xMax = mapInfo.heightMapSizeX + xMin;
            float yMin = mapInfo.heightMapMinY;
            float yMax = float.MinValue;
            float zMin = mapInfo.heightMapOriZ;
            float zMax = zMin + mapInfo.heightMapSizeZ;

            List<Transform> transList = new
                List<Transform>(SceneHeightRoot.GetComponentsInChildren<Transform>());

            foreach (var meshCollider in SceneHeightRoot.GetComponentsInChildren<MeshCollider>())
            {
                Mesh mesh = meshCollider.sharedMesh;
                if (mesh == null)
                {
                    continue;
                }
                Transform trans = meshCollider.transform;
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
            foreach (var collider in SceneHeightRoot.GetComponentsInChildren<Collider>())
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
                        heightMap.SetPixel(x, z, new Color32(byteArr[0], byteArr[1], 0, 0));
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
                                        heightMap.GetPixel(xNear, zNear).g, heightMap.GetPixel(xNear, zNear).b, 1);
                                    findCount++;
                                }
                            }
                        }
                        if (findCount > 1)
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

        #endregion

        #region 场景格信息

        public void SetCellEditorTab()
        {
            CellEditor.cellInfos = CurrentSceneInfo.cells.ToList();
        }

        #endregion

    }
}