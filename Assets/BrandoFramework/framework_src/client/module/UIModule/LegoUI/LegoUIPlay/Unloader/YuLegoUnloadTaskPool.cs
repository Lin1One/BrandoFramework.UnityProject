#region Head

// Author:            Yu
// CreateDate:        2018/10/11 6:01:07
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using System;
using UnityEngine;
using YuCommon;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高卸载任务对象池。
    /// </summary>
    [Singleton]
    public class YuLegoUnloadTaskPool
    {
        private IObjectPool<YuLegoUnloadTask> taskPool;

        private IObjectPool<YuLegoUnloadTask> TaskPool
        {
            get
            {
                if (taskPool != null)
                {
                    return taskPool;
                }

                taskPool = new ObjectPool<YuLegoUnloadTask>(
                    () => new YuLegoUnloadTask(), 30);
                return taskPool;
            }
        }

        public YuLegoUnloadTask Take(RectTransform uiRect, int unloadSpped,
            Action<RectTransform> pushSonTask, YuLegoUnloadTask parentTask)
        {
            var task = TaskPool.Take();
            task.Init(uiRect, unloadSpped, pushSonTask, parentTask);
            return task;
        }

        public void Restore(YuLegoUnloadTask task) => TaskPool.Restore(task);
    }
}