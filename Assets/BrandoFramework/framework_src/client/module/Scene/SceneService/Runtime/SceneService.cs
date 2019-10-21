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

using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    public class SceneService : ISceneService
    {
        private string mapDataDirPath = Application.dataPath + "/GameWorld/TestScene/MapData/";
        private const int c_maxKeepResource = 2;
        
        private Dictionary<int, string> m_dicMapId = new Dictionary<int, string>();   //地图配置id对应的地图数据资源id
        private Dictionary<string, SceneUnit> m_dicMapLoader = new Dictionary<string, SceneUnit>();   //所有地图加载器
        private LinkedList<SceneUnit> m_linkOpenOrder = new LinkedList<SceneUnit>(); //记录开启顺序,用于回收资源的选择
        private List<SceneUnit> m_listClosingMap = new List<SceneUnit>();   //正在关闭的地图（回收GameObject）
        private List<SceneUnit> m_listReleaseMap = new List<SceneUnit>();   //正在释放资源的地图
        //private Material m_blockMaterial;     //阻挡物所用的透明材质

        //private readonly IYuU3DEventModule m_eventModule = 
        //    YuU3dAppUtility.Injector.Get<IYuU3DEventModule>();

        private SceneUnit m_curMapLoader;     //当前活动的地图对应的加载器
        public SceneUnit CurSceneUnit => m_curMapLoader;


        /// <summary>
        /// 获取当前地图的寻路网格
        /// </summary>
        public SceneNavMeshData NavMeshData
        {
            get
            {
                return m_curMapLoader != null ? 
                    m_curMapLoader.NavMeshData : null;
            }
        }

        ///// <summary>
        ///// 服务器二维坐标原点对应的unity坐标
        ///// </summary>
        //public Vector2 OriPos
        //{
        //    get
        //    {
        //        if(m_curMapLoader !=null)
        //        {
        //            return m_curMapLoader.OriPos;
        //        }
        //        return Vector2.zero;
        //    }
        //}

        public void Init()
        {
            string testSceneName = "TestScene";
            string testSceneInfoFile = "TestScene/TestScene.json";
            int mapAssetId = 1;
            
            SceneUnit sceneUnit = new SceneUnit();
            sceneUnit.Init(mapDataDirPath + testSceneInfoFile);
            m_dicMapId.Add(mapAssetId, testSceneName);
            m_dicMapLoader.Add(testSceneName, sceneUnit);
        }

        public void Init(Dictionary<int, string> mapInfos)
        {
            foreach (var mapInfo in mapInfos)
            {
                //Todo 通过读表方式获取路径
                string mapAssetId = mapInfo.Value;
            
                if (m_dicMapId.ContainsKey(mapInfo.Key))
                {
#if DEBUG
                    Debug.LogError(string.Format("读取到重复的地图信息 ID：{0}    ，path：{1}", mapInfo.Key, mapAssetId));
#endif
                    continue;
                }
                //地图配置编号 映射 地图数据资源id
                m_dicMapId.Add(mapInfo.Key, mapInfo.Value);

                //如果资源id没有对应的地图加载器，则创建
                if(!m_dicMapLoader.ContainsKey(mapInfo.Value))
                {
                    SceneUnit mapLoader = new SceneUnit();
                    mapLoader.Init(mapAssetId);
                    //mapLoader.BlockMaterial = m_blockMaterial;
                    m_dicMapLoader.Add(mapInfo.Value, mapLoader);
                }
                
            }
            //注册事件
            //m_eventModule.WatchUnityEvent(
            //    YuUnityEventType.FixedUpdate, OnFixedUpdate);
            //m_eventModule.WatchEvent(
            //    YuUnityEventCode.NetDisconnect, CloseCurMap);

            //YuMapTools.SetMapIDMapping(m_dicMapId);

            //m_eventModule.WatchEvent(YuUnityEventCode.Scene_CameraTranslate, OnCameraTranslate);
        }

        public bool EnterScene(int mapID)
        {
            return true;
        }

        public bool OpenMap(int mapID)//, Point2 coord,bool isPhase = false)
        {
            //if (CurMapId() == mapID)
            //{
            //    return true;
            //}

            if (m_dicMapId.ContainsKey(mapID) && 
                m_dicMapLoader.ContainsKey(m_dicMapId[mapID]))
            {
                ExitCurMap();                          //尝试关闭旧地图

                m_curMapLoader = m_dicMapLoader[m_dicMapId[mapID]];
                if (!m_curMapLoader.TryEnterScene())
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("地图开启失败,ID：{0}", mapID));
#endif
                    //return false;
                }
                //---成功打开地图后---
               

                //---判断是否需要释放之前的地图资源---
                if (m_linkOpenOrder.Contains(m_curMapLoader))
                {
                    m_linkOpenOrder.Remove(m_curMapLoader);
                    m_linkOpenOrder.AddLast(m_curMapLoader);
                }
                else
                {
                    m_linkOpenOrder.AddLast(m_curMapLoader);
                }
                if (m_linkOpenOrder.Count > c_maxKeepResource)
                {
                    StartReleaseMapData(m_linkOpenOrder.First.Value);
                    m_linkOpenOrder.RemoveFirst();
                }
                return true;
            }
#if UNITY_EDITOR
            Debug.LogError(string.Format("未找到对应的地图加载器,ID：{0}", mapID));
#endif
            return false;
        }

        public void ExitCurMap()
        {
            if (m_curMapLoader != null)
            {
                m_curMapLoader.ExitScene();

                if (!m_listClosingMap.Contains(m_curMapLoader))
                {
                    m_listClosingMap.Add(m_curMapLoader);
                }
                m_curMapLoader = null;
            }
        }

        public string CurMapName()
        {
            //if (m_curMapLoader != null)
            //    return m_curMapLoader.MapName;

            return null;
        }

        public bool StartReleaseMapData(int mapID)
        {
            if (m_dicMapId.ContainsKey(mapID))
            {
                var assetId = m_dicMapId[mapID];
                if(m_dicMapLoader.ContainsKey(assetId))
                {
                    return StartReleaseMapData(m_dicMapLoader[assetId]);
                }
            }
            return false;
        }

        private void OnFixedUpdate()
        {
            ReleaseObjectFrame();
            //CheckCameraTranslate();
        }

        //public bool StartReleaseMapData(string mapName)
        //{
        //    int mapID = GetMapID(mapName);
        //    return StartReleaseMapData(mapID);
        //}

        //开始回收指定地图资源
        private bool StartReleaseMapData(SceneUnit mapLoader)
        {
            if (m_listReleaseMap.Contains(mapLoader))
            {
#if DEBUG
                Debug.LogWarning("该地图已经在回收列表中");
#endif
                return false;
            }
            mapLoader.ReleaseSceneObject();
            m_listReleaseMap.Add(mapLoader);
            //注册事件
            //m_eventModule.WatchUnityEvent(
            //    YuUnityEventType.FixedUpdate, ReleaseResourceFrame);
            return true;
        }

        //帧循环事件监控，回收地图GameObject，常驻
        private void ReleaseObjectFrame()
        {
            if(m_curMapLoader !=null)
            {
                //用于地图动态回收游戏对象
                m_curMapLoader.ReleaseSceneObjectFrame();
            }
            //用于回收已关闭的地图所有游戏对象
            for (int i = 0; i < m_listClosingMap.Count;)
            {
                if (m_listClosingMap[i].ReleaseSceneObjectFrame())
                {
                    m_listClosingMap.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        //帧循环事件监控，回收地图资源，有地图需要释放资源时监控
        private void ReleaseResourceFrame()
        {
            for (int i = 0; i < m_listReleaseMap.Count;)
            {
                if (m_listReleaseMap[i].ReleaseSceneObjectFrame())
                {
                    m_listReleaseMap.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            //没有任务则移除事件监控
            if (m_listReleaseMap.Count == 0)
            {
                //m_eventModule.RemoveUnityEvent(
                //    YuUnityEventType.FixedUpdate, ReleaseResourceFrame);
            }
        }

//        //通过地图名找到对应的地图ID  (无法实现，因为一个地图可能对应多个地图id)
//        private int GetMapID(string mapName)
//        {
//            foreach (var mapInfo in m_dicMapLoader)
//            {
//                if (mapInfo.Value != null && mapInfo.Value.MapName == mapName)
//                {
//                    return mapInfo.Key;
//                }
//            }
//#if UNITY_EDITOR
//            Debug.LogWarning(string.Format("未找到对应的地图ID：{0}", mapName));
//#endif

//            return -1;
//        }

        //public int CurMapId()
        //{

        //    if (m_curMapLoader != null)
        //        return m_curMapLoader.MapId;

        //    return -1;
        //}

        //public float GetHeight(Vector2 pos)
        //{
        //    if(m_curMapLoader !=null)
        //        return m_curMapLoader.GetHeight(pos);
        //    return 0.0f;
        //}

        //public Point2 CurCoord
        //{
        //    get
        //    {
        //        return m_curMapLoader.PlayerCoord;
        //    }
        //}

        //public Vector2 PlayerPosPercent
        //{
        //    get
        //    {
        //        if (m_curMapLoader != null)
        //            return m_curMapLoader.PlayerPosPercent;
        //        return Vector2.zero;
        //    }
        //}

        #region 摄像机改变位置方向，判断地形透视的处理

        //private bool m_isCameraTranslate;
        //private Vector3 m_cameraPos;
        //private Vector3 m_cameraFocus;
        //private float m_lastCameraTime;
        //private const float c_cameraCycle = 0.5f;

        //private void OnCameraTranslate(object param)
        //{
        //    m_isCameraTranslate = true;
        //    object[] paramArr = (object[])param;
        //    m_cameraPos = (Vector3)paramArr[0];
        //    m_cameraFocus = (Vector3)paramArr[1];
        //}

        //private void CheckCameraTranslate()
        //{
        //    if(m_isCameraTranslate &&
        //        m_curMapLoader != null && 
        //        Time.time - m_lastCameraTime > c_cameraCycle)
        //    {
        //        m_curMapLoader.CheckCameraTranslate(m_cameraPos, m_cameraFocus);
        //        m_lastCameraTime = Time.time;
        //        m_isCameraTranslate = false;
        //    }
        //}

        #endregion
    }
}

