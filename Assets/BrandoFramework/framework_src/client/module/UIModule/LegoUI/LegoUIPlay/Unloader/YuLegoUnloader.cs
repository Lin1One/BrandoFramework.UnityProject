#region Head

// Author:            Yu
// CreateDate:        2018/10/11 5:10:39
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using System.Collections.Generic;
using UnityEngine;
using YuCommon;
using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI卸载器。
    /// </summary>
    [Singleton]
    public class YuLegoUnloader : MonoBehaviour
    {
        #region 卸载任务栈

        private readonly Stack<YuLegoUnloadTask> _taskStack
            = new Stack<YuLegoUnloadTask>();

        private readonly HashSet<string> _unloadingTask
            = new HashSet<string>();

        #endregion

        private YuLegoUnloadTaskPool _taskPool;

        private YuLegoUnloadTaskPool TaskPool =>
            _taskPool ?? (_taskPool = Injector.Instance.Get<YuLegoUnloadTaskPool>());

        private bool _unloadAble;

        private int _totalUnloadSpeed = 100;

        private int _taskUnloadSpeed = 10;

        /// <summary>
        /// 卸载操作的已执行次数。
        /// </summary>
        private int _unloadExecuteNum;

        private YuLegoUnloadTask _currentTask;

        public void UnloadUI(RectTransform uiRect)
        {
            if (_unloadingTask.Contains(uiRect.name))
            {
                return;
            }

            _unloadingTask.Add(uiRect.name);
            var task = TaskPool.Take(uiRect, _taskUnloadSpeed, UnloadUI, _currentTask);
            _taskStack.Push(task);
            _unloadAble = true;
        }

        public void Update()
        {
            if (!_unloadAble || _taskStack.Count == 0)
            {
                return;
            }

            _unloadExecuteNum++;

            if (_unloadExecuteNum == _totalUnloadSpeed)
            {
                _unloadExecuteNum = 0;
                return;
            }

            while (true)
            {
                _currentTask = _taskStack.Peek();
                if (_currentTask.IsComplete)
                {
                    _unloadingTask.Remove(_currentTask.TaskId);
                    TaskPool.Restore(_currentTask);
                    _taskStack.Pop();
                    if (_taskStack.Count == 0)
                    {
                        return;
                    }

                    _taskStack.Peek().EnableUpdate();
                    continue;
                }

                _currentTask.UnloadAtUpdate();
            }
        }
    }
}