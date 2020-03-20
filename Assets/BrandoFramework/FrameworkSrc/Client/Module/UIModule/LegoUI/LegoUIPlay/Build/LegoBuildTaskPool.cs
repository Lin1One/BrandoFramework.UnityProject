#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion

using Common;
using System;
using UnityEngine;


namespace Client.LegoUI
{
    [Singleton]
    public class LegoBuildTaskPool
    {
        private IGenericObjectPool<LegoBuildTask> taskPool;

        private IGenericObjectPool<LegoBuildTask> TaskPool
        {
            get
            {
                if (taskPool != null)
                {
                    return taskPool;
                }

                taskPool = new GenericObjectPool<LegoBuildTask>(
                    () => new LegoBuildTask(), 30);
                return taskPool;
            }
        }

        public LegoBuildTask GetTask
        (
            string id,
            Action<LegoBuildTask> onLoaded,
            RectTransform parent
        )
        {
            var task = TaskPool.Take();
            task.Init(id, onLoaded, parent);
            return task;
        }

        public void RestoreTask(LegoBuildTask task)
        {
            TaskPool.Restore(task);
        }
    }
}