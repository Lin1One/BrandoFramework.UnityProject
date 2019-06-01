#region Head

// Author:            Yu
// CreateDate:        2018/10/24 19:52:57
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Client.GamePlaying.AI;
using client_common;
using client_module_event;

namespace Client.GamePlaying.Unit
{
    /// <summary>
    /// AI控制器
    /// </summary>
    public class XTwoUnitAIControl : UnitComponent, IYuUnitAIControl
    {
        private YuAIBehaviorTree m_behaviorTree;    //行为树

        private uint updateCount;      //更新AI计数
        private const uint currentFrameCount = 3;           //几帧更新一次AI



        private IU3DEventModule eventModule;
        private IU3DEventModule EvnetModule
        {
            get
            {
                if (eventModule == null)
                {
                    eventModule = Injector.Instance.Get<IU3DEventModule>();
                }
                return eventModule;
            }
        }
                

        public object BehaviorParam    //用于切换AI行为，记录的参数
        {
            get;
            set;
        }

        public XTwoUnitAIControl()
        {
            //OnInit();
        }

        protected override void OnInit()
        {
            EvnetModule.WatchUnityEvent(YuUnityEventType.Update, OnFixedUpdate);
        }

        protected override void OnRelease()
        {
            if (m_behaviorTree != null)
            {
                var temp = m_behaviorTree;
                m_behaviorTree = null;
                temp.Release();
            }

            EvnetModule.RemoveUnityEvent(
                YuUnityEventType.Update, OnFixedUpdate);
        }

        private void OnFixedUpdate()
        {
            updateCount++;
            if (updateCount < currentFrameCount)
            {
                return;
            }
            updateCount = 0;

            if (m_behaviorTree != null)
            {
                if (m_behaviorTree.Tick() != AIBehaviorState.Running)
                {
                    var temp = m_behaviorTree;
                    m_behaviorTree = null;
                    temp.Release();
                }
            }
        }

        /// <summary>
        /// 判断是否正在执行此条行为树
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public bool CheckIsExecuteTree(YuAIBehaviorTree tree)
        {
            return m_behaviorTree == tree;
        }

        /// <summary>
        /// 设置当前行为数，（原有行为数将被结束）
        /// </summary>
        /// <param name="tree"></param>
        public void ResetBehaviorTree(YuAIBehaviorTree tree)
        {
            if (m_behaviorTree != null)
            {
                var temp = m_behaviorTree;
                m_behaviorTree = null;
                temp.Release();
            }
            m_behaviorTree = tree;
        }

        public bool CheckBehaviorTreeMatch(YuAIBehaviorTree tree)
        {
            return m_behaviorTree == tree;
        }


        //#region 设置具体行为

        ///// <summary>
        ///// 移动到某处
        ///// </summary>
        ///// <param name="coord">服务器二维数组坐标</param>
        ///// <param name="callBack">寻路回调，成功到达返回true，获取道路失败或是中途中止，返回false</param>
        ///// <param name="isPath">true：使用寻路方式移动   false：强制直线移动</param>
        //public void MoveTo(Point2 coord, Action<bool> callBack = null, bool isPath = true, float stopDis = 0.0f)
        //{
        //    //Vector2 pos = YuGraphAlgorithm.GetPositionByCoord(coord,
        //    //        m_mapLoadService.OriPos);
        //    //MoveTo(pos, callBack, isPath, stopDis);
        //}

        //private Action m_moveRecord;

        ///// <summary>
        ///// 移动到某处
        ///// </summary>
        ///// <param name="coord">服务器二维数组坐标</param>
        ///// <param name="callBack">寻路回调，成功到达返回true，获取道路失败或是中途中止，返回false</param>
        ///// <param name="isPath">true：使用寻路方式移动   false：强制直线移动</param>
        //public void MoveTo(Vector2 pos, Action<bool> callBack = null, bool isPath = true, float stopDis = 0.0f)
        //{
        //    //if (m_threadNum > 0)      //如果已有一个正在进行的寻路计算任务，手动中止它
        //    //{
        //    //    m_threadModule.StopTask(m_threadNum);
        //    //    m_threadNum = -1;
        //    //}

        //    if (!Role.ActStateMachine.CanMove())  //不能移动的情况下，不处理
        //    {
        //        m_moveRecord = () =>
        //        {
        //            MoveTo(pos, callBack, isPath, stopDis);
        //        };

        //        return;
        //    }

        //    if (isPath)
        //    {
        //        //m_threadNum = m_threadModule.CreateFuncTask(EForeverThread.thread1,
        //        //    FindPathThread, new object[] { pos, stopDis }, CheckFindPathResult);

        //        m_onMoveTo = callBack;
        //    }
        //    else
        //    {
        //        MoveToPos(pos, callBack);
        //    }
        //}

        ////private readonly YuThreadModule m_threadModule = YuU3dAppUtility.Injector.Get<YuThreadModule>();
        //private int m_threadNum = -1;

        //private Action<bool> m_onMoveTo;                    //寻路结果回调
        ////private Thread m_findPathThread;                    //寻路线程
        ////private int m_findPathState;                        //是否找到寻路(0为寻路中，1为找到，-1为失败)
        ////private Vector2[][] m_pathArr;                        //寻路结果
        //private readonly object m_locker = new object();    //加锁对象 
        ////private bool m_isWatchEvent;

        //private void CheckFindPathResult(int taskNum, object param)
        //{
        //    if (Role?.ActStateMachine == null)
        //    {
        //        return;
        //    }

        //    if (m_threadNum != taskNum ||        //如果回调编号不等于当前记录的编号，说明此回调的操作已被顶替，不处理
        //       !Role.ActStateMachine.CanMove()) //不能移动的情况下，不处理
        //    {
        //        return;
        //    }
        //    var paramArr = (object[])param;
        //    bool isFind = (bool)paramArr[0];
        //    Vector2[][] pathArr = paramArr[1] as Vector2[][];

        //    //if (m_findPathState != 0)
        //    //{
        //    //m_eventModule.RemoveUnityEvent(YuUnityEventType.FixedUpdate, CheckFindPathResult);
        //    //m_isWatchEvent = false;
        //    if (isFind)
        //    {
        //        if (Role == null)
        //        {
        //            return;
        //        }

        //        if (m_behaviorTree != null)
        //        {
        //            var temp = m_behaviorTree;
        //            m_behaviorTree = null;
        //            temp.Release();
        //        }

        //        YuAIBehaviorTreeBuilder builder = new YuAIBehaviorTreeBuilder();
        //        builder.SetAISubject(Role);
        //        m_behaviorTree = builder.
        //            Sequence().                           //主动选择
        //                //Action<XTwoAIActionPathTo>(new object[] { pathArr, m_onMoveTo }).          //移动到某地
        //            End();

        //        //if (Role.Type == YuUnitManager.UnitType.leadPlayer)
        //        //{
        //        //    GetOnMount();

        //        //    List<Vector2> coordArr = new List<Vector2>();
        //        //    if (pathArr != null)
        //        //    {
        //        //        foreach (var arr in pathArr)
        //        //        {
        //        //            foreach (var item in arr)
        //        //            {
        //        //                coordArr.Add(YuGraphAlgorithm.RemapWorldPosToUnitCoord(
        //        //                   item, YuMapTools.MapInfo.MapMinPos, YuMapTools.MapInfo.MapSize));
        //        //            }
        //        //        }
        //        //    }
        //        //    m_eventModule.TriggerEvent(YuUnityEventCode.Scene_FindPathPoints,
        //        //        null, new List<Vector2>(coordArr));
        //        //}
        //    }
        //    else
        //    {
        //        //if (Role.Type == YuUnitManager.UnitType.leadPlayer)
        //        //{
        //        //    m_eventModule.TriggerEvent(YuUnityEventCode.Scene_FindPathPoints,
        //        //    null, new List<Vector2>());
        //        //}
        //        //if (m_onMoveTo != null)
        //        //{
        //        //    m_onMoveTo(false);
        //        //}
        //    }
        //    //m_findPathState = 0;
        //    //}
        //}

        ////private IYuSocketDispatcher m_socket;

        ////private void GetOnMount()
        ////{
        ////    if (m_socket == null)
        ////    {
        ////        m_socket = YuU3dAppUtility.Injector.Get<IYuSocketDispatcher>();
        ////    }
        ////    m_socket.RequestSend(
        ////               "Game", 14008, new XTwo_ShouFeiDianHideRequest
        ////               {
        ////                   type = (int)PointChargesType.Mount,
        ////                   hide = 0,
        ////               });
        ////}

        ////private object FindPathThread(object obj)
        ////{
        ////    var navMeshData = m_mapLoadService.NavMeshData;
        ////    var objArr = (object[])obj;
        ////    Vector2 pos = (Vector2)objArr[0];
        ////    float stopDis = (float)objArr[1];

        ////    //判断双方是否在同一个寻路区域
        ////    int startAreaId, endAreaId, startPoly, endPoly;
        ////    if (!YuNavMeshTools.GetNodeIdByPos(navMeshData, Role.U3DData.Position2D,
        ////        out startAreaId, out startPoly))
        ////    {
        ////        //Todo 需要对出发点不在可行走范围做处理
        ////        //m_findPathState = -1;
        ////        return new object[] { false, null };
        ////    }
        ////    if (!YuNavMeshTools.GetNodeIdByPos(navMeshData, pos,
        ////       out endAreaId, out endPoly))
        ////    {
        ////        //目的地不在可行走范围，寻在主角方向最近点
        ////        YuNavMeshTools.FindNearestNode(navMeshData,
        ////            Role.U3DData.Position2D, pos, out endAreaId, out pos);

        ////        if (endAreaId < 0)
        ////        {
        ////            return new object[] { false, null };
        ////        }
        ////    }
        ////    Vector2[][] pathArr2 = YuMapTools.MapInfo.FindPathJumpPoint(startAreaId,
        ////            endAreaId, Role.U3DData.Position2D, pos, navMeshData.areas);

        ////    if (pathArr2 == null || pathArr2.Length == 0)
        ////    {
        ////        return new object[] { false, null };
        ////    }

        ////    if (stopDis > 0.01f) //如果设置了停止距离
        ////    {
        ////        List<Vector2> lastPath = new List<Vector2>(pathArr2[pathArr2.Length - 1]);

        ////        for (int i = lastPath.Count - 1; i > 0; i--)
        ////        {
        ////            Vector2 lastPos = lastPath[i - 1];
        ////            Vector2 dir = lastPath[i] - lastPos;
        ////            float dis = dir.magnitude;

        ////            if (dis < stopDis)
        ////            {
        ////                lastPath.RemoveAt(lastPath.Count - 1);
        ////                stopDis -= dis;
        ////            }
        ////            else
        ////            {
        ////                lastPath[i] = lastPos + (dis - stopDis) / dis * dir;
        ////                break;
        ////            }

        ////        }
        ////        pathArr2[pathArr2.Length - 1] = lastPath.ToArray();
        ////    }

        ////    return new object[] { true, pathArr2 };
        ////}

        //private void MoveToPos(Vector2 pos, Action<bool> callback)
        //{
        //    //if (callback == null)
        //    //{
        //    //    callback = (isSuc) => { Role.Motion.StartIdle(); };
        //    //}
        //    //else
        //    //{
        //    //    callback += (isSuc) => { Role.Motion.StartIdle(); };
        //    //}

        //    //如果距离过长，则加速移动(处理网络延迟导致的滞后)
        //    float dis = Vector2.Distance(Role.U3DData.Position2D, pos);
        //    float speedScale = 1.0f;
        //    if (dis > 1.5f)
        //    {
        //        speedScale = dis / 1.5f;
        //    }

        //    //Role.Motion.SetMovePos(pos, callback, speedScale);
        //}


        //#endregion

    }
}

