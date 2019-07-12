#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com


#endregion

using Common;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Client.LegoUI
{
    [Singleton]
    [YuMonoPath("Other/", null)]
    public class LegoBuilder : MonoBehaviour
    {
        #region 构建任务栈

        //For 构建任务耗费帧数
        private int buildFrameCount = 0;

        private readonly Stack<LegoBuildTask> taskStack
            = new Stack<LegoBuildTask>();

        #endregion

        #region 基础字段及属性

        private LegoBuildTaskPool taskPool;

        private LegoBuildTaskPool TaskPool =>
            taskPool ?? (taskPool = Injector.Instance.Get<LegoBuildTaskPool>());

        /// <summary>
        /// 是否进行构建。
        /// </summary>
        private bool buildAble;

        /// <summary>
        /// 当前设置的构建速度，该速度为所有乐高构建任务所共享，消耗完毕则会跳出当前帧。
        /// </summary>
        private int totalBuildSpeed = 50;

        private int taskDefaultSpeed;

        private DateTime starTime;

        private int CostTime => (DateTime.Now - starTime).Milliseconds;

        public RectTransform RootRect { get; private set; }

        private const int BuildCountOut = 500;

        #endregion

        #region 启动构建

        public LegoBuildTask CreateTask
        (
            string id,
            Action<LegoBuildTask> onBuilded,
            RectTransform parent,
            int buildSpeed = -1,
            Action allCompleted = null
        )
        {
            AllCompleted = allCompleted;
            var finalSpeed = buildSpeed == -1 ? taskDefaultSpeed : buildSpeed;
            var task = TaskPool.GetTask(id, onBuilded, parent)
                .SetPushTaskDel(PushSonTask)
                .SetBuildSpeed(finalSpeed);
            RootRect = task.RootRect;
            return task;
        }

        public void StartBuild()
        {
            buildFrameCount = 0;
            buildAble = true;
            starTime = DateTime.Now;
        }

        public void PushSonTask(LegoBuildTask task)
        {
            taskStack.Push(task);
        }

        #endregion

        #region 回调

        public Action AllCompleted { get; private set; }

        #endregion

        #region 帧循环调用构建任务

        /// <summary>
        /// 当前帧已执行构建工作量。
        /// </summary>
        private int buildedCount;

        private LegoBuildTask currentTask;

        public void Update()
        {
            if (!buildAble)
            {
                return;
            }

            buildFrameCount++;
            int buildFrameCountOut = 0; 
            while (true)
            {
                buildFrameCountOut++;
                if (buildFrameCountOut > BuildCountOut)
                {
#if DEBUG
                    Debug.LogError($"该界面构建帧超时退出！");
#endif
                    return;
                }

                if (taskStack.Count == 0)
                {
                    buildAble = false;
                    AllCompleted?.Invoke();
                    AllCompleted = null; // 每次都置空完全构建完毕回调
                    //Debug.Log($"该界面构造帧数为 : {buildFrameCount}");
                    //Debug.Log($"该界面构造耗时为 : {CostTime}毫秒！");

                    buildFrameCount = 0;
                    return;
                }

                currentTask = taskStack.Peek();
                if (currentTask.IsComplete)
                {
                    currentTask.Reset();
                    TaskPool.RestoreTask(currentTask);
                    taskStack.Pop();
                    continue;
                }

                try
                {
                    currentTask.BuildAtUpdate();
                    buildedCount += currentTask.BuildedFrameCount;
                    if (buildedCount >= totalBuildSpeed)
                    {
                        buildedCount = 0;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //DestroySelf();

#if DEBUG
                    Debug.LogError($"UI{currentTask.TaskMeta.RootMeta.Name}构建失败！");
                    Debug.LogError(ex.Message + ex.StackTrace);
#endif
                    currentTask.Reset();
                    TaskPool.RestoreTask(currentTask);
                    taskStack.Pop();

#if UNITY_EDITOR
                    //Application.Quit();
#endif
                }
            }
        }

        private void DestroySelf()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        #endregion

        #region 构造

        private void AutoInject()
        {
            ////var buildSetting = YuU3dAppLegoUISettingDati.CurrentActual.BuildSetting;
            ////totalBuildSpeed = buildSetting.TotalBuildSpeed;
            ////taskDefaultSpeed = buildSetting.TaskBuildSpeed;
        }

        #endregion
    }
}