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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameWorld
{

    public class SceneAreaInfo
    {

    }

    [Serializable]
    public class SceneNavMeshData
    {
        public int i;
    }


    /// <summary>
    /// 场景单位
    /// </summary>
    public class SceneUnit
    {
        private const int c_FrameReleaseObjCount = 3; //一帧回收游戏对象数量
        private const int c_FrameReleaseResourceCount = 1; //一帧回收资源数量

        //private readonly IYuU3DEventModule m_eventModule =
        //    YuU3dAppUtility.Injector.Get<IYuU3DEventModule>();

        //private ScriptableObject m_assetRef;
        private string m_assetId;
        private int m_moveEventId = -1; //记录事件id(用于注销事件)
        private int m_getJumpPointEventId = -1; //记录事件id(用于注销事件)
        private int m_loadMapEventId = -1; //记录事件id(用于注销事件)

        private bool isOpen;

        private int m_maxCellOpenCount = 16; //最多打开多少格地图块
        private int startCellId = 0;

        private SceneInfo m_mapInfo;
        private Light m_mainLight;
        private SceneAreaInfo m_areaInfo; //区域跳跃点信息

        public SceneNavMeshData NavMeshData
        {
            get { return m_mapInfo != null ? m_mapInfo.navMeshData : new SceneNavMeshData(); }
        }


        public void ReleaseSceneObject()
        {

        }
        public bool ReleaseSceneObjectFrame()
        {
            return true;
        }
        //loading页模块
        //private static IYuLoadingModule s_loadingModule;

        //private static IYuLoadingModule LoadingModule
        //{
        //    get
        //    {
        //        if (s_loadingModule != null)
        //        {
        //            return s_loadingModule;
        //        }

        //        s_loadingModule = YuU3dAppUtility.Injector.Get<IYuLoadingModule>();
        //        return s_loadingModule;
        //    }
        //}

        //private static YuThreadModule s_threadModule;

        //private static YuThreadModule ThreadModule
        //{
        //    get
        //    {
        //        if (s_threadModule == null)
        //        {
        //            s_threadModule = YuU3dAppUtility.Injector.Get<YuThreadModule>();
        //        }

        //        return s_threadModule;
        //    }
        //}

        //高度图
        private Texture2D m_heightMap;

        private Vector3 m_playerPos; //玩家unity坐标
        //private Point2 m_playerCoord; //玩家服务器坐标

        //public Point2 PlayerCoord
        //{
        //    get { return m_playerCoord; }
        //}

        private GameObject m_rootObj;

        private string m_curMapName; //当前使用的地图名

        public string MapName
        {
            get { return m_curMapName; }
        }

        private int m_curMapId;

        public int MapId
        {
            get { return m_curMapId; }
        }

        private int m_xCell; //当前地图x格子数量
        private int m_zCell; //当前地图z格子数量
        private float m_xCellSize;
        private float m_zCellSize;
        private float m_xMin;
        private float m_zMin;

        private float m_lastMapTime;

        private LightmapData[] m_lightmDatas; //用于设置Unity场景Lightmap

        private LinkedList<SceneCellInfo> m_linkOpenedCell = new LinkedList<SceneCellInfo>(); //记录已开启的格子

        #region 游戏对象管理

        private Dictionary<string, GameObject> m_dicLayer = new Dictionary<string, GameObject>(); //管理层级信息
        private Dictionary<int, GameObject> m_dicItemObject = new Dictionary<int, GameObject>(); //管理实例化的物体游戏对象

        private Stack<GameObject> m_stackReleaseObject = new Stack<GameObject>(); //等待被回收的游戏对象
        //private Dictionary<int, CheckBlockItem> m_dicCheckBlockItem = new Dictionary<int, CheckBlockItem>(); //管理阻挡视线检测的物品

        #endregion

        #region 资源管理

        private int m_waitLoadNum; //等待加载的资源数量

        private List<Texture2D> m_listLightmapTex =
            new List<Texture2D>(); //管理lightmap纹理

        private Material m_skyBoxMat;
        private Material m_skyBoxMatRef;

        private Dictionary<int, GameObject> m_dicItemPrefab =
            new Dictionary<int, GameObject>(); //管理预制物资源

        private Dictionary<string, UnityEngine.Object> originAssetDic =
            new Dictionary<string, UnityEngine.Object>(); //管理预制物资源

        //private IAssetModule m_assetModule; //资源加载模块

        //public Material BlockMaterial;     //阻挡物所用的透明材质

        #endregion

        /// <summary>
        /// 初始化地图信息
        /// </summary>
        /// <param name="mapInfoPath"></param>
        /// <returns></returns>
        public void Init(string mapInfoAsset)
        {
            //m_curMapId = mapID;
            m_waitLoadNum = 0;
            isOpen = false;
            //m_assetModule = YuU3dAppUtility.Injector.Get<IAssetModule>();
            m_assetId = mapInfoAsset;
            m_mainLight = GameObject.Find("Directional Light").GetComponent<Light>();
        }

        //加载地图资源
        private bool LoadMapInfo(bool isCheckRes)
        {
            if (isCheckRes)
            {
                //bool needSubDownload = YuU3dAppUtility.Injector.
                //    Get<IYuHotUpdateModule>().CheckCurMapNeed(m_assetId);
                //if(needSubDownload)
                //{
                //    return false;
                //}
            }

            if (m_mapInfo == null)
            {
                var mapInfoStr = File.ReadAllText(m_assetId);
//                var text = YuJsonUtility..Load<TextAsset>(m_assetId);
//                if (text == null)
//                {
//#if UNITY_EDITOR
//                    Debug.LogError(string.Format("读取mapInfo文件失败：{0}", m_assetId));
//#endif
//                    return false;
//                }
                m_mapInfo = YuJsonUtility.FromJson<SceneInfo>(mapInfoStr);
                if (m_mapInfo == null)
                {
                    return false;
                }
            }

            //m_heightMap = m_assetModule.Load<Texture2D>(string.Format("{0}_heightmap", m_assetId));
            //if(m_heightMap == null)
            //{
            //    return false;
            //}

            InitData();
            return true;
        }

        //按读取到的m_mapInfo初始化各成员数据
        private void InitData()
        {
            if (m_mapInfo == null)
                return;

            if (string.IsNullOrEmpty(m_mapInfo.skyboxMaterial))
            {
                m_skyBoxMat = null;
            }
            else
            {
                //m_assetModule.LoadAsync<Material>(m_mapInfo.skyboxMaterial, material =>
                //{
                //    m_skyBoxMatRef = material;
                //    m_skyBoxMat = material;
                //    if (isOpen)
                //    {
                //        RenderSettings.skybox = m_skyBoxMat;
                //    }
                //});
            }

            //成员数据赋值
            m_curMapId = m_mapInfo.mapId;
            m_curMapName = m_mapInfo.mapName;
            m_xCell = m_mapInfo.xCellCount;
            m_zCell = m_mapInfo.zCellCount;
            m_xMin = m_mapInfo.xMin;
            m_zMin = m_mapInfo.zMin;
            m_xCellSize = m_mapInfo.xCellSize;
            m_zCellSize = m_mapInfo.zCellSize;

            //初始化m_mapInfo非序列化数据
            foreach (var cell in m_mapInfo.cells)
            {
                cell.isOpen = false;
            }

            foreach (var item in m_mapInfo.items)
            {
                item.useCount = 0;
            }
        }

        /// <summary>
        /// 释放地图资源时的一次性执行部分
        /// </summary>
        public void Release()
        {
            if (m_heightMap != null)
            {
                m_heightMap = null;
                //Todo
                //m_assetModule.GetTexture2D(string.Format("{0}_heightmap", m_assetId)).Release();
            }

            if (m_skyBoxMat != null)
            {
                m_skyBoxMat = null;
                //Todo 
                //m_skyBoxMatRef.Release();
                m_skyBoxMatRef = null;
            }

            m_xCell = 0;
            m_zCell = 0;
            m_xMin = 0;
            m_zMin = 0;
            m_xCellSize = 0;
            m_zCellSize = 0;
            m_curMapName = null;
            m_mapInfo = null;
        }

        private int m_downLoadEventId = -1;

        public bool TryEnterScene()
        {
            if (m_mapInfo == null)
            {
                bool MapInfoloaded = LoadMapInfo(false);
                if (!MapInfoloaded)
                {
                    //m_downLoadEventId = m_eventModule.WatchEvent(YuUnityEventCode.Scene_SubDownloadEnd, () =>
                    // {
                    //     if (m_downLoadEventId > 0)
                    //     {
                    //         m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.Scene_SubDownloadEnd, m_downLoadEventId);
                    //         m_downLoadEventId = -1;
                    //     }

                    //     OpenMap(coord, curMapID, false);
                    // });

                    return false;
                }
            }

            //m_curMapId = curMapID;

            //m_playerCoord = coord;
            //Vector2 pos2D = YuGraphAlgorithm.GetPositionByCoord(coord, m_mapInfo.oriPosVO.ToVector2());
            //m_playerPos = new Vector3(pos2D.x, 0.0f, pos2D.y);

            //设置地图工具信息
            //YuMapTools.ResetMapInfo(m_mapInfo, m_areaInfo);

            //构建必要的Unity对象
            BuildMap();
            //立马更新地图
            m_lastMapTime = -1.0f;
            

            //var loadHandler = LoadingModule.GetFreeHandler("XTwo", "加载地图");
            //LoadingModule.RegisterHandler("XTwo", loadHandler);

            //int openCellNum = GetCurCellNum(m_playerPos);
            //int openCellNum = 7;
            OpenCellDataSync(m_mapInfo.cells[startCellId]);
            UpdateMap();
            //LoadingModule.StartLoading("legoview_loadingframe");

            //if (m_loadMapEventId > 0)
            //{
            //    m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.View_CompleteLoading, m_loadMapEventId);
            //    m_loadMapEventId = -1;
            //}

            //m_loadMapEventId = m_eventModule.WatchEvent(YuUnityEventCode.View_CompleteLoading,
            //    () =>
            //    {
            //        m_moveEventId = m_eventModule.WatchEvent(
            //            YuUnityEventCode.Scene_SelfOnMove, OnPlayerMover);
            //        m_eventModule.TriggerEvent
            //            (YuUnityEventCode.Scene_OpenMap, null, m_curMapId);
            //        UpdateMap();

            //        if (m_loadMapEventId > 0)
            //        {
            //            m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.View_CompleteLoading, m_loadMapEventId);
            //            m_loadMapEventId = -1;
            //        }
            //    });

            //if(!isCheckRes)
            //{
            //    YuU3dAppUtility.Injector.Get<YuUnitManager>().LeadPlayer?.U3DData?.SetCoord(coord);
            //}

            isOpen = true;
            return true;
        }

        public void ExitScene()
        {

        }
        /// <summary>
        /// 关闭地图
        /// </summary>
        public void CloseMap()
        {
            isOpen = false;

            //把地图根节点隐藏
            if (m_rootObj != null)
                m_rootObj.SetActive(false);
            else
            {
#if UNITY_EDITOR
                Debug.LogError("严重警告，关闭地图时，未找到地图根GameObject！");
#endif
            }

            //把所有格子设置为关闭
            foreach (var cell in m_linkOpenedCell)
            {
                //cell.isOpen = false;
            }

            //把所有游戏对象放入回收列表
            foreach (var item in m_dicLayer.Values)
            {
                m_stackReleaseObject.Push(item);
            }

            foreach (var item in m_dicItemObject.Values)
            {
                m_stackReleaseObject.Push(item);
            }

            m_dicLayer.Clear();
            m_dicItemObject.Clear();
            m_linkOpenedCell.Clear();
            //m_dicCheckBlockItem.Clear();

            //重置lightmap数据
            LightmapSettings.lightmaps = null;
            //清空天空盒
            RenderSettings.skybox = null;

            //注销事件
            if (m_moveEventId > 0)
            {
                //m_eventModule.RemoveSpecifiedHandler(
                //    YuUnityEventCode.Scene_SelfOnMove, m_moveEventId);
                m_moveEventId = -1;
            }

            if (m_getJumpPointEventId > 0)
            {
                //m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.Scene_SendJumpPoint,
                //    m_getJumpPointEventId);
                m_getJumpPointEventId = -1;
            }

            if (m_loadMapEventId > 0)
            {
                //m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.View_CompleteLoading, m_loadMapEventId);
                m_loadMapEventId = -1;
            }
        }

        /// <summary>
        /// 获取主角(0-1)位置
        /// </summary>
        //public Vector2 PlayerPosPercent
        //{
        //    get
        //    {
        //        if (m_mapInfo == null)
        //        {
        //            return Vector2.zero;
        //        }

        //        Vector2 mapOri = m_mapInfo.originPos.ToVector2();
        //        return new Vector2((m_playerPos.x - mapOri.x) / m_mapInfo.xLenth,
        //            (m_playerPos.z - mapOri.y) / m_mapInfo.zLenth);
        //    }
        //}

        //public float GetHeight(Vector2 pos)
        //{
        //    if (m_mapInfo == null || m_heightMap == null)
        //    {
        //        return 0.0f;
        //    }

        //    float xf = ((pos.x - m_mapInfo.heightMapOriX) / m_mapInfo.heightMapSizeX);
        //    float yf = ((pos.y - m_mapInfo.heightMapOriZ) / m_mapInfo.heightMapSizeZ);
        //    //xf = Mathf.Clamp(xf, 0.0f, 1.0f);
        //    //yf = Mathf.Clamp(yf, 0.0f, 1.0f);
        //    //xf = xf * 1024 - 0.5f;
        //    //yf = yf * 1024 - 0.5f;
        //    //int x = (int)xf;
        //    //int y = (int)yf;
        //    //xf -= x;
        //    //yf -= y;
        //    //int x2 = Mathf.Clamp(x + 1, 0, 1023);
        //    //int y2 = Mathf.Clamp(y + 1, 0, 1023);
        //    //x = Mathf.Clamp(x, 0, 1023);
        //    //y = Mathf.Clamp(y, 0, 1023);

        //    //float[] valueArr = new float[4];
        //    //valueArr[0] = YuGraphAlgorithm.SamplarRG(m_heightMap, x, y);// m_heightMap.GetPixel(x, y).r;
        //    //valueArr[1] = YuGraphAlgorithm.SamplarRG(m_heightMap, x2, y);//m_heightMap.GetPixel(x2, y).r;
        //    //valueArr[2] = YuGraphAlgorithm.SamplarRG(m_heightMap, x, y2);//m_heightMap.GetPixel(x, y2).r;
        //    //valueArr[3] = YuGraphAlgorithm.SamplarRG(m_heightMap, x2, y2);//m_heightMap.GetPixel(x2, y2).r;

        //    //float valueFin = YuGraphAlgorithm.LinearSamplar(new Vector2(xf, yf), valueArr);

        //    float valueFin = YuGraphAlgorithm.LinearSamplarRG(m_heightMap, xf, yf);

        //    return valueFin * m_mapInfo.heightMapSizeY + m_mapInfo.heightMapMinY + 0.01f;
        //}


        //打开地图时，构建马上需要的场景对象、数据
        private void BuildMap()
        {
            //初始化lightmap
            if (m_mapInfo.lightmapInfo != null)
            {
                SceneLightInfo lmInfo = m_mapInfo.lightmapInfo;

                //初始化lightmap纹理资源
                if (m_listLightmapTex == null)
                    m_listLightmapTex = new List<Texture2D>();
                m_listLightmapTex.Clear();
                if (m_listLightmapTex.Count == 0 && lmInfo.DataCount > 0)
                {
                    for (int i = 0; i < lmInfo.lightmapTexPath.Length; i++)
                    {
                        m_listLightmapTex.Add(null);
                        //---读取lightmap纹理资源的操作
                        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(lmInfo.lightmapTexPath[i]);
                            if (texture == null)
                            {
#if UNITY_EDITOR
                                Debug.LogError("lightMap纹理读取失败：" + lmInfo.lightmapTexNames[i]);
#endif
                            }

                            m_listLightmapTex[i] = texture;

                            //最后一个图片加载完成则初始化LightmapData数组
                            if (i == lmInfo.lightmapTexPath.Length - 1)
                            {
                                OnLoadLightmap(lmInfo);
                            }
                        
                    }
                }

                //设置天空盒
                RenderSettings.skybox = m_skyBoxMat;
            }

            //设置主平行光
            if (m_mapInfo.hasLight)
            {
                m_mainLight.enabled = true;
                m_mainLight.color = m_mapInfo.lightColor.ToVector4();
                m_mainLight.intensity = m_mapInfo.lightIntensity;
                m_mainLight.transform.position = m_mapInfo.lightPos.ToVector3();
                m_mainLight.transform.forward = m_mapInfo.lightDir.ToVector3();
            }
            else
            {
                m_mainLight.enabled = false;
            }

            //设置雾
            if (m_mapInfo.hasFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = m_mapInfo.fogColor.ToVector4();
                Camera.main.backgroundColor = m_mapInfo.fogColor.ToVector4();
                RenderSettings.fogMode = (FogMode)m_mapInfo.fogMode;
                RenderSettings.fogStartDistance = m_mapInfo.fogStart;
                RenderSettings.fogEndDistance = m_mapInfo.fogEnd;
                RenderSettings.fogDensity = m_mapInfo.fogDensity;
            }
            else
            {
                RenderSettings.fog = false;
            }

            //创建的游戏场景对象父节点
            if (m_mapInfo.layers != null)
            {
                foreach (var info in m_mapInfo.layers) //创建
                {
                    if (!m_dicLayer.ContainsKey(info.layerName))
                    {
                        GameObject gameObj = new GameObject(info.layerName);
                        gameObj.transform.position = info.pos.ToVector3();
                        gameObj.transform.eulerAngles = info.rot.ToVector3();
                        gameObj.transform.localScale = info.scal.ToVector3();
                        m_dicLayer.Add(info.layerName, gameObj);

                        if (info.layerName == m_curMapName)
                        {
                            m_rootObj = gameObj;
                            m_rootObj.transform.SetParent(GameObject.Find("SceneRoot").transform);
                        }
                    }
                }

                if (m_rootObj == null)
                {
                    Debug.LogError("错误，没有找到地图根节点！");
                }

                foreach (var info in m_mapInfo.layers) //设置父级
                {
                    string parentName = MyMapTool.GetNameByELater(info.parentLayer, m_curMapName);

                    if (!string.IsNullOrEmpty(parentName) && m_dicLayer.ContainsKey(info.layerName))
                    {
                        GameObject gameObj = m_dicLayer[info.layerName];
                        GameObject parentObj = m_dicLayer[parentName];
                        if (gameObj != null)
                            gameObj.transform.parent = parentObj != null ? parentObj.transform : null;
                    }
                }
            }

            //    //请求跳跃点数据
            //    if (m_areaInfo == null)
            //    {
            //        m_getJumpPointEventId = m_eventModule.WatchEvent(
            //            YuUnityEventCode.Scene_SendJumpPoint, OnGetJumpPoint);
            //        m_eventModule.TriggerEvent(YuUnityEventCode.Scene_RequestJumpPoint,
            //            null, m_curMapId);
            //    }
        }

        //lightmap纹理加载全部完成后的回调，初始化LightmapData数组
        private void OnLoadLightmap(SceneLightInfo lmInfo)
        {
            //初始化LightmapData数组
            if (m_lightmDatas == null ||
                (m_lightmDatas.Length == 0 && lmInfo.DataCount > 0))
            {
                {
                    m_lightmDatas = new LightmapData[lmInfo.DataCount];
                    for (int i = 0; i < m_lightmDatas.Length; i++)
                    {
                        m_lightmDatas[i] = new LightmapData();
                    }
                }

                for (int i = 0; i < lmInfo.lightmapTexNames.Length; i++)
                {
                    LightmapData corData = m_lightmDatas[lmInfo.dataIndexes[i]];
                    Texture2D lightmapTex = null;
                    if (m_listLightmapTex[i] != null)
                    {
                        lightmapTex = m_listLightmapTex[i];
                    }

                    switch (lmInfo.types[i])
                    {
                        case SceneLightInfo.ELightmapType.color:
                            corData.lightmapColor = lightmapTex;
                            break;
                        case SceneLightInfo.ELightmapType.dir:
                            corData.lightmapDir = lightmapTex;
                            break;
                        case SceneLightInfo.ELightmapType.shadowmask:
                            corData.shadowMask = lightmapTex;
                            break;
                    }
                }
            }

            LightmapSettings.lightmaps = m_lightmDatas;
            LightmapSettings.lightmapsMode = lmInfo.lightmapMode;
        }

        //主角移动事件触发
        private void OnPlayerMover(object param)
        {
            //获取主角当前位置
            try
            {
                m_playerPos = (Vector3) param;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            UpdateMap();
        }

        //获取跳跃点信息响应
        private void OnGetJumpPoint(object param)
        {
            object[] paramArr = (object[]) param;
            int mapId = (int) paramArr[0];
            if (mapId != m_curMapId)
            {
                return;
            }

            //Point2[] startArr = (Point2[]) paramArr[1];
            //Point2[] endArr = (Point2[]) paramArr[2];

            //List<YuMapJumpPoint> jumpList = new List<YuMapJumpPoint>();

            //for (int i = 0; i < startArr.Length; i++)
            //{
            //    //计算跳点起点信息
            //    Vector2 pos = YuMapTools.MapInfo.ServerToUnityPos(startArr[i]);
            //    int areaID, nodeId;
            //    if (!YuNavMeshTools.GetNodeIdByPos(NavMeshData, pos, out areaID, out nodeId))
            //    {
            //        continue;
            //    }

            //    YuMapJumpPoint jumpPoint = new YuMapJumpPoint();
            //    jumpPoint.StartAreaId = areaID;
            //    jumpPoint.StartPosition = pos;

            //    //计算跳点终点信息
            //    pos = YuMapTools.MapInfo.ServerToUnityPos(endArr[i]);
            //    if (!YuNavMeshTools.GetNodeIdByPos(NavMeshData, pos, out areaID, out nodeId))
            //    {
            //        continue;
            //    }

            //    jumpPoint.EndAreaId = areaID;
            //    jumpPoint.EndPosition = pos;

            //    jumpList.Add(jumpPoint);
            //}

            //m_areaInfo = new YuMapAreaInfo(jumpList.ToArray());
            //YuMapTools.ResetMapInfo(m_mapInfo, m_areaInfo);

            //m_eventModule.RemoveSpecifiedHandler(YuUnityEventCode.Scene_SendJumpPoint,
            //    m_getJumpPointEventId);
            //m_getJumpPointEventId = -1;
        }

        //更新地图
        private void UpdateMap()
        {
            int[] openNums = { 1};
            //更新流式加载地图信息
            foreach (int num in openNums)
            {
                if (num >= 0 && num < m_mapInfo.cells.Length)
                {
                    OpenCellDataSync(m_mapInfo.cells[num]);
                }
            }



            //            if (m_mapInfo == null)
            //            {
            //#if UNITY_EDITOR
            //                Debug.LogWarning("m_mapInfo 为 Null");
            //#endif
            //                return;
            //            }

            //            if (Time.time - m_lastMapTime < 0.1f) //0.1秒最小间隔
            //                return;

            //            m_lastMapTime = Time.time;

            //多线程执行更新地图判断
            //ThreadModule.CreateFuncTask(EForeverThread.thread1,
            //    CheckOpenCells, null, (taskNum, param) =>
            //    {
            //        if (!isOpen)
            //        {
            //            return;
            //        }

            //        object[] paramArr = param as object[];
            //        int[] openNums = paramArr[1] as int[];
            //        Point2 point = (Point2)paramArr[0];

            //        if (openNums != null)
            //        {
            //            //更新流式加载地图信息
            //            foreach (int num in openNums)
            //            {
            //                if (num >= 0 && num < m_mapInfo.cells.Length)
            //                {
            //                    OpenCellData(m_mapInfo.cells[num]);
            //                }
            //            }

            //            //跨格
            //            //if (point != m_playerCoord)
            //            //{
            //            //    m_playerCoord = point;

            //            //    YuU3dAppUtility.Injector.Get<YuU3DEventModule>().TriggerEvent(
            //            //        YuUnityEventCode.Scene_SelfOverCell, null, m_playerCoord);
            //            //}
            //        }
            //    });

            //object CheckOpenCells(object param) //检查打开格子的函数
            //{
            //    ////判断是否跨服务器坐标二维数组格子，如果是则触发跨格事件
            //    Point2 point = YuGraphAlgorithm.GetCoordByPosition(m_playerPos,
            //        m_mapInfo.oriPosVO.ToVector2(), m_mapInfo.xCellCountVO, m_mapInfo.zCellCountVO);

            //    return new object[] { point, GetOpenCellNums(m_playerPos, m_mapInfo) };
            //}
        }

        //更新地图
        public void UpdateMap_InEditor(int[] openIndexs)
        {
            int[] openNums = openIndexs;
            //更新流式加载地图信息
            foreach (int num in openNums)
            {
                if (num >= 0 && num < m_mapInfo.cells.Length)
                {
                    OpenCellDataSync(m_mapInfo.cells[num]);
                }
            }
        }
            //读取一个cell的数据
        private void OpenCellData(SceneCellInfo cellInfo)
        {
            if (cellInfo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("cellInfo 为 Null");
#endif
                return;
            }

            if (cellInfo.isOpen)
                return;

            while (m_linkOpenedCell.Count > m_maxCellOpenCount)
            {
                int temp = m_linkOpenedCell.Count;

                CloseCellData();

                if (m_linkOpenedCell.Count >= temp)
                {
#if UNITY_EDITOR
                    Debug.LogError("严重警告！地图块隐藏失败！");
#endif
                    break;
                }
            }

            cellInfo.isOpen = true;
            m_linkOpenedCell.AddLast(cellInfo);
            foreach (var itemNum in cellInfo.itemsNum)
            {
                CreateItem(itemNum);
            }
        }


        private void OpenCellDataSync(SceneCellInfo cellInfo)
        {
            if (cellInfo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("cellInfo 为 Null");
#endif
                return;
            }

            if (cellInfo.isOpen)
            {
                return;
            }
            while (m_linkOpenedCell.Count > m_maxCellOpenCount)
            {
                int temp = m_linkOpenedCell.Count;
                CloseCellData();
                if (m_linkOpenedCell.Count >= temp)
                {
#if UNITY_EDITOR
                    Debug.LogError("严重警告！地图块隐藏失败！");
#endif
                    break;
                }
            }

            cellInfo.isOpen = true;
            m_linkOpenedCell.AddLast(cellInfo);
            foreach (var item in cellInfo.itemsNum)
            {
                CreateItem(item, true);
            }
        }

        //创建一个物体
        private void CreateItem(int itemNum, bool isSync = false)
        {
            Debug.Log(string.Format("编号为{0}的itemInfo", itemNum));
            SceneItemInfo itemInfo = m_mapInfo.items[itemNum];
            if (itemInfo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("无法找到编号为{0}的itemInfo", itemNum));
#endif
                return;
            }
            itemInfo.useCount++;

            if (m_dicItemObject.ContainsKey(itemNum))
            {
                return;
            }

            m_dicItemObject.Add(itemNum, null);

            UnityEngine.Object prefabResource = null;
            if (m_dicItemPrefab.ContainsKey(itemNum))
            {
                prefabResource = m_dicItemPrefab[itemNum];
            }
            if(originAssetDic.ContainsKey(itemInfo.prefabAssetPath))
            {
                prefabResource = originAssetDic[itemInfo.prefabAssetPath];
            }

            if (prefabResource == null) //如果字典里获取不到
            {
                m_waitLoadNum++; //等待加载的资源数++
                if (isSync)
                {
                    m_waitLoadNum--;
                    var prefabRef = AssetDatabase.LoadAssetAtPath<GameObject>(itemInfo.prefabAssetPath);
                    if (prefabRef == null)
                    {
#if DEBUG
                        Debug.LogError("无法加载prefab： " + itemInfo.prefabName);
#endif
                        return;
                    }

                    var prefabObj = prefabRef;
                    if (prefabObj == null)
                    {
#if DEBUG
                        Debug.LogWarning("无法加载prefab： " + itemInfo.prefabName);
#endif
                        return;
                    }

                    if (m_dicItemPrefab.ContainsKey(itemNum))
                        m_dicItemPrefab.Remove(itemNum);
                    m_dicItemPrefab.Add(itemNum, prefabObj);
                    if(!originAssetDic.ContainsKey(itemInfo.prefabAssetPath))
                    {
                        originAssetDic.Add(itemInfo.prefabAssetPath, prefabObj);
                    }
                    InstanceGameObj(prefabObj, itemInfo);
                }
                else
                {
//                    AssetDatabase.LoadAssetAtPath<GameObject>(TestScenePrefabsDirPath + itemInfo.prefabName, prefab =>
//                    {
//                        m_waitLoadNum--;
//                        if (prefab == null)
//                        {
//#if DEBUG
//                            Debug.LogWarning("无法加载prefab： " + itemInfo.prefabName);
//#endif
//                            return;
//                        }

//                        if (m_dicItemPrefab.ContainsKey(itemNum))
//                            m_dicItemPrefab.Remove(itemNum);
//                        m_dicItemPrefab.Add(itemNum, prefab);
//                        InstanceGameObj(prefab, itemInfo);
//                    });
                }
            }
            else //如果字典里获取到了
            {
                InstanceGameObj(prefabResource, itemInfo);
                //m_instanceList.Enqueue(() => {  });
                //if (!m_isWatchUpdate)
                //{
                //    m_eventModule.WatchUnityEvent(YuUnityEventType.Update, InstanceByUpdate);
                //    m_isWatchUpdate = true;
                //}
            }
        }

        private bool m_isWatchUpdate;

        private readonly Queue<Action> m_instanceList =
            new Queue<Action>();

        private void InstanceByUpdate()
        {
            if (m_instanceList.Count == 0)
            {
                //m_eventModule.RemoveUnityEvent(YuUnityEventType.Update, InstanceByUpdate);
                m_isWatchUpdate = false;
                return;
            }

            var action = m_instanceList.Dequeue();
            action();
        }

        //实例化一个GameObject
        private void InstanceGameObj(UnityEngine.Object prefabResource, SceneItemInfo itemInfo)
        {
            //如果已经要求被销毁，则跳出
            //if (!m_dicItemObject.ContainsKey(itemInfo.objNum))
            //{
            //    return;
            //}

            UnityEngine.Object newObj = GameObject.Instantiate(prefabResource);

            if (newObj != null && newObj is GameObject)
            {
                GameObject gameObj = (GameObject) newObj;

                //if ((int)itemInfo.parentLayer > 1)
                //{
                //    foreach (var item in gameObj.GetComponentsInChildren<Transform>())
                //    {
                //        switch (itemInfo.parentLayer)
                //        {
                //            case EMapLayer.AdornEffect:
                //                item.gameObject.layer = YuUnityConstant.LAYER_PARTICLE;
                //                break;
                //            case EMapLayer.Building:
                //                item.gameObject.layer = YuUnityConstant.LAYER_BUILDING;
                //                break;
                //            case EMapLayer.Terrain:
                //                item.gameObject.layer = YuUnityConstant.LAYER_TERRAIN;
                //                break;
                //            case EMapLayer.Shadow:
                //                item.gameObject.layer = YuUnityConstant.LAYER_SHADOW;
                //                break;
                //            case EMapLayer.Tree:
                //                item.gameObject.layer = YuUnityConstant.LAYER_TREE;
                //                break;
                //            case EMapLayer.Animation:
                //                item.gameObject.layer = YuUnityConstant.LAYER_DEFAULT;
                //                break;
                //        }
                //    }
                //}

                gameObj.transform.position = itemInfo.pos.ToVector3();
                gameObj.transform.eulerAngles = itemInfo.rot.ToVector3();
                gameObj.transform.localScale = itemInfo.scal.ToVector3();
                gameObj.name = itemInfo.objName;

                Renderer[] renders = gameObj.GetComponentsInChildren<Renderer>();
                if (renders.Length == itemInfo.lightmapIndexes.Length)
                {
                    for (int i = 0; i < renders.Length; i++)
                    {
                        if (itemInfo.lightmapIndexes[i] > -1)
                        {
                            renders[i].lightmapIndex = itemInfo.lightmapIndexes[i];
                            renders[i].lightmapScaleOffset = itemInfo.lightmapScaleOffsets[i].ToVector4();
                        }

                        if (itemInfo.realtimeLightmapIndexes[i] > -1)
                        {
                            renders[i].realtimeLightmapIndex = itemInfo.realtimeLightmapIndexes[i];
                            renders[i].realtimeLightmapScaleOffset =
                                itemInfo.realtimeLightmapScaleOffsets[i].ToVector4();
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Renderer数量与itemInfo的lightmap数据数量不一致，将导致lightmap无法正常工作");
                }

                //m_dicItemObject[itemInfo.objNum] = gameObj;
                //if ((itemInfo.parentLayer == EMapLayer.Building || 
                //    itemInfo.parentLayer == EMapLayer.Tree) &&
                //    !m_dicCheckBlockItem.ContainsKey(itemInfo.objNum))
                //{
                //    var blockItem = new CheckBlockItem(gameObj);
                //    blockItem.obj = gameObj;
                //    m_dicCheckBlockItem.Add(itemInfo.objNum, blockItem);
                //}

                string parentName = MyMapTool.GetNameByELater(itemInfo.parentLayer, m_curMapName);
                Transform parentTrans = m_dicLayer.ContainsKey(parentName) ? m_dicLayer[parentName].transform : null;
                gameObj.transform.parent = parentTrans;
            }
            else
            {
                Debug.LogWarning(prefabResource.name + "实例化异常");
            }
        }

        //关闭一个最远的cell地图块
        private void CloseCellData()
        {
            if (m_mapInfo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("m_mapInfo 为 Null");
#endif
                return;
            }

            int xNum = (int) ((m_playerPos.x - m_xMin) / m_xCellSize);
            int zNum = (int) ((m_playerPos.z - m_zMin) / m_zCellSize);

            int farthest = 0;
            SceneCellInfo closeCell = null;
            foreach (var cell in m_linkOpenedCell)
            {
                int xCellNum = cell.cellId % m_xCell;
                int zCellNum = cell.cellId / m_xCell;

                int temp = Mathf.Max(Mathf.Abs(xCellNum - xNum),
                    Mathf.Abs(zCellNum - zNum));
                if (temp > farthest)
                {
                    farthest = temp;
                    closeCell = cell;
                }
            }

            if (closeCell == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("没有找到可删除地图块!");
#endif
                return;
            }

            m_linkOpenedCell.Remove(closeCell);
            closeCell.isOpen = false;
            foreach (var item in closeCell.itemsNum)
            {
                DeleteItem(item);
            }
        }

        //删除一个prefab实例化出来的物体
        private void DeleteItem(int itemNum)
        {
            SceneItemInfo itemInfo = m_mapInfo.items[itemNum];
            if (itemInfo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("无法找到编号为{0}的itemInfo", itemNum));
#endif
                return;
            }

            if (!m_dicItemObject.ContainsKey(itemNum))
            {
#if UNITY_EDITOR
                Debug.LogWarning("m_dicPrefabObject中，无法找到编号为 " + itemNum + " 的prefab实例化对象");
#endif
                itemInfo.useCount = 0;
                return;
            }

            itemInfo.useCount--;
            //物体在各个格子被开启的总数为0，可以回收了
            if (itemInfo.useCount <= 0)
            {
                GameObject gameObj = m_dicItemObject[itemNum];
                m_dicItemObject.Remove(itemNum);
                //if (m_dicCheckBlockItem.ContainsKey(itemNum))
                //{
                //    m_dicCheckBlockItem.Remove(itemNum);
                //}

                m_stackReleaseObject.Push(gameObj);

                if (itemInfo.useCount < 0)
                    itemInfo.useCount = 0;
            }
        }

        [ThreadStatic] private static List<int> ts_openList = new List<int>();

        //获取当前位置，需要开启和关闭的cell编号(可能在多线程下运行)
        private int[] GetOpenCellNums(Vector3 curPos, SceneInfo mapInfo)
        {
            if (ts_openList == null)
            {
                ts_openList = new List<int>();
            }

            ts_openList.Clear();

            //当前主角所在的格子xz编号，可越界
            int xNum = (int) ((curPos.x - m_xMin) / m_xCellSize);
            int zNum = (int) ((curPos.z - m_zMin) / m_zCellSize);

            //九宫格内
            for (int z = zNum, zDelta = -1; zDelta < 6; z += zDelta, zDelta += 3)
            {
                if (z < 0 || z >= m_zCell) //越界判断
                    continue;
                for (int x = xNum, xDelta = -1; xDelta < 6; x += xDelta, xDelta += 3)
                {
                    if (x < 0 || x >= m_xCell) //越界判断
                        continue;

                    int num = x + z * m_xCell;
                    if (num >= 0 && num < m_mapInfo.cells.Length
                                 && !m_mapInfo.cells[num].isOpen)
                        ts_openList.Add(num);
                }
            }

            return ts_openList.ToArray();
        }

        private int GetCurCellNum(Vector3 curPos)
        {
            int xNum = (int) ((curPos.x - m_xMin) / m_xCellSize);
            int zNum = (int) ((curPos.z - m_zMin) / m_zCellSize);

            xNum = Mathf.Clamp(xNum, 0, m_xCell);
            zNum = Mathf.Clamp(zNum, 0, m_zCell);

            return xNum + zNum * m_xCell;
        }

        //一帧中回收部分游戏对象，全部回收完则返回true
        public bool ReleaseObjectFrame()
        {
            for (int i = 0; i < c_FrameReleaseObjCount; i++)
            {
                if (m_stackReleaseObject.Count > 0)
                {
                    GameObject obj = m_stackReleaseObject.Pop();
                    if (obj != null)
                        GameObject.Destroy(obj);
                }
                else
                    return true;
            }

            if (m_stackReleaseObject.Count > 0)
                return false;

            return true;
        }

        //一帧中回收部分资源，全部回收完则返回true
        public bool ReleaseResourceFrame()
        {
            for (int i = 0; i < c_FrameReleaseResourceCount; i++)
            {
                if (m_listLightmapTex.Count > 0)
                {
                    int num = m_listLightmapTex.Count - 1;
                    string assetName = m_listLightmapTex[num].name;
                    m_listLightmapTex.RemoveAt(num);
                    //Texture2D obj = m_assetModule.Load<Texture2D>(assetName);
                    //if (obj == null)
                    //{
                    //    Debug.LogError(string.Format("回收lightmap纹理资源时，" +
                    //                                 "无法找到该资源的IYuAssetRef：{0}", assetName));
                    //}

                    //if (obj != null)
                    //{
                    //    //Todo
                    //    //obj.Release();
                    //}
                }
                else if (m_dicItemPrefab.Count > 0)
                {
                    KeyValuePair<int, GameObject> pair = m_dicItemPrefab.ElementAt(0);
                    string assetName = pair.Value.name;
                    m_dicItemPrefab.Remove(pair.Key);
                    GameObject obj = null;
                    //m_assetModule.LoadAsync<GameObject>(assetName, (assetRef) =>
                    //{
                    //    obj = assetRef;
                    //    if (obj != null)
                    //    {
                    //        //Todo

                    //        //obj.Release();
                    //    }

                    //    if (obj == null)
                    //    {
                    //        Debug.LogError(string.Format("回收prefab资源时，" +
                    //                                     "无法找到该资源的IYuAssetRef：{0}", assetName));
                    //    }
                    //});
                }
                else if (m_waitLoadNum == 0)
                {
                    return true;
                }
            }

            if (m_dicItemPrefab.Count > 0 || m_listLightmapTex.Count > 0 || m_waitLoadNum > 0)
                return false;

            return true;
        }

        ////通过摄像机位置与焦点坐标，判断需要透明化的物体
        //public void CheckCameraTranslate(Vector3 vec1,Vector3 vec2)
        //{


        //    foreach(var item in m_dicCheckBlockItem.Values)
        //    {
        //        for(int i =0;i< item.saves.Length;i++)
        //        {
        //            if(item.saves[i].isReplace)
        //            {
        //                if (!YuGraphAlgorithm.CheckSegmentCollideAABB(vec1, vec2,
        //                    item.saves[i].render.bounds.min, item.saves[i].render.bounds.max))
        //                {
        //                    item.saves[i].render.sharedMaterials = item.saves[i].mats;
        //                    item.saves[i].isReplace = false;
        //                }
        //            }
        //            else
        //            {
        //                if (YuGraphAlgorithm.CheckSegmentCollideAABB(vec1, vec2,
        //                    item.saves[i].render.bounds.min, item.saves[i].render.bounds.max))
        //                {
        //                    Material[] materials = new Material[item.saves[i].render.sharedMaterials.Length];
        //                    for (int j = 0; j < materials.Length; j++)
        //                    {
        //                        materials[j] = BlockMaterial;
        //                    }

        //                    item.saves[i].render.sharedMaterials = materials;
        //                    item.saves[i].isReplace = true;
        //                }
        //            }

        //        }
        //    }
        //}


        //private struct CheckBlockItem
        //{
        //    public CheckBlockItem(GameObject go)
        //    {
        //        obj = go;
        //        var renders = go.GetComponentsInChildren<Renderer>();
        //        saves = new SaveMat[renders.Length];
        //        for(int i =0;i<renders.Length;i++)
        //        {
        //            saves[i].render = renders[i];
        //            saves[i].mats = renders[i].sharedMaterials;
        //        }
        //    }

        //    public GameObject obj;
        //    public SaveMat[] saves;

        //    public struct SaveMat
        //    {
        //        public bool isReplace;
        //        public Renderer render;
        //        public Material[] mats;
        //    }
        //}
    }
}