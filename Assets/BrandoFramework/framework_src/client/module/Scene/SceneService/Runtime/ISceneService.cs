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


namespace GameWorld
{
    /// <summary>
    /// 场景管理服务
    /// </summary>
    public interface ISceneService
    {
        void Init();
        bool EnterScene(int sceneId);
        //void ExitScene();


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name=""></param>
        //void Init(Dictionary<int, string> mapInfos);



        ///// <summary>
        ///// 打开一个地图
        ///// </summary>
        ///// <param name="mapName">地图名</param>
        ///// <returns>是否成功打开地图</returns>
        //bool OpenMap(string mapName, Point2 coord, bool isPhase = false);

        /// <summary>
        /// 打开一个地图
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="coord">主角初始坐标（服务器二维数组坐标）</param>
        /// <param name="isPhase">是否是位面场景</param>
        /// <returns>是否成功打开地图</returns>
        //bool OpenMap(int mapId, Point2 coord,bool isPhase = false);



        /// <summary>
        /// 关闭当前地图
        /// </summary>


        ///// <summary>
        ///// 关闭当前地图
        ///// </summary>
        //void CloseCurMap();

        ///// <summary>
        ///// 开始以帧循环方式，释放一个指定的地图相关资源
        ///// </summary>
        ///// <param name="mapName">地图名</param>
        ///// <returns>是否有此地图文件的资源</returns>
        //bool StartReleaseMapData(string mapName);

        /// <summary>
        /// 开始以帧循环方式，释放一个指定的地图相关资源
        /// </summary>
        /// <param name="mapName">地图ID</param>
        /// <returns>是否有此地图文件的资源</returns>
        //bool StartReleaseMapData(int mapID);

        /// <summary>
        /// 返回当前地图名
        /// </summary>
        /// <returns></returns>
        //string CurSceneName();

        /// <summary>
        /// 返回当前地图编号
        /// </summary>
        /// <returns></returns>
        //int CurMapId();


        /// <summary>
        /// 获取当前地图的寻路网格
        /// </summary>
        //YuNavMeshData NavMeshData { get; }

        /// <summary>
        /// 服务器二维数组坐标，原点对应的unity坐标
        /// </summary>
        ///Vector2 OriPos { get; }

        /// <summary>
        /// 主角当前所在的服务器二维坐标，
        /// </summary>
        //Point2 CurCoord { get; }

        /// <summary>
        /// 获取一个点的高度
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        ///float GetHeight(Vector2 pos);

        /// <summary>
        /// 获取玩家所在的地图位置（0-1）
        /// </summary>
        /// <returns></returns>
        ///Vector2 PlayerPosPercent { get; }

    }
}

