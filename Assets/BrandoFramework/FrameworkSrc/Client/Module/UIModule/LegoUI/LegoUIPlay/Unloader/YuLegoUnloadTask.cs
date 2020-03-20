#region Head

// Author:            Yu
// CreateDate:        2018/10/11 5:11:11
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common;
using System;
using System.Linq;
using UnityEngine;

namespace Client.LegoUI
{
    public class YuLegoUnloadTask : IReset
    {
        #region 静态构造

        private static readonly LegoMetaHelper metaHelper;

        static YuLegoUnloadTask()
        {
            InitControlPool();
            metaHelper = Injector.Instance.Get<LegoMetaHelper>();
        }

        #endregion

        #region 静态控件池引用

        private static YuLegoRectTransformPool rectPool;
        private static YuLegoTextPool textPool;
        private static YuLegoImagePool imagePool;
        private static YuLegoRawImagePool rawImagePool;
        private static YuLegoButtonPool buttonPool;
        private static YuLegoTButtonPool _tButtonPool;
        private static YuLegoTogglePool togglePool;
        private static YuLegoSliderPool sliderPool;
        private static YuLegoProgressbarPool progressbarPool;
        private static YuLegoInputFieldPool inputFieldPool;
        private static YuLegoDropdownPool dropdownPool;
        private static YuLegoScrollViewPool scrollViewPool;

        private static void InitControlPool()
        {
            rectPool = Injector.Instance.Get<YuLegoRectTransformPool>();
            textPool = Injector.Instance.Get<YuLegoTextPool>();
            imagePool = Injector.Instance.Get<YuLegoImagePool>();
            rawImagePool = Injector.Instance.Get<YuLegoRawImagePool>();
            buttonPool = Injector.Instance.Get<YuLegoButtonPool>();
            _tButtonPool = Injector.Instance.Get<YuLegoTButtonPool>();
            togglePool = Injector.Instance.Get<YuLegoTogglePool>();
            sliderPool = Injector.Instance.Get<YuLegoSliderPool>();
            progressbarPool = Injector.Instance.Get<YuLegoProgressbarPool>();
            inputFieldPool = Injector.Instance.Get<YuLegoInputFieldPool>();
            dropdownPool = Injector.Instance.Get<YuLegoDropdownPool>();
            scrollViewPool = Injector.Instance.Get<YuLegoScrollViewPool>();
        }

        #endregion

        private LegoUIMeta taskMeta;
        private RectTransform taskRect;
        public string TaskId => taskRect.name;

        private YuLegoUnloadTask _parentTask;

        /// <summary>
        /// 已卸载的控件数量。
        /// </summary>
        private int unloadedCount;

        private int willUnloadCount;

        protected int SonComponentUnloadIndex;
        protected int SonContainerUnloadIndex;

        private int sonComponentNum;
        private int sonContainerNum;

        private int unloadSpeed;

        private LegoUIType unloadingType;

        public bool IsComplete { get; private set; }

        private Action<RectTransform> m_PushTaskAction;

        public void Init(RectTransform tUi, int speed, Action<RectTransform> pushTaskAction,
            YuLegoUnloadTask parentTask)
        {
            taskRect = tUi;
            var uiTypeId = GetUiTypeId();
            taskMeta = metaHelper.GetMeta(uiTypeId);
            unloadSpeed = speed;
            m_PushTaskAction = pushTaskAction;
            _parentTask = parentTask;
            willUnloadCount = taskMeta.WillUnloadCount;
            sonComponentNum = taskMeta.ComponentRefs.Count;
            sonContainerNum = taskMeta.ContainerRefs.Count;
            _executeAble = true;

            string GetUiTypeId()
            {
                if (!tUi.name.Contains("@"))
                {
                    return tUi.name;
                }

                var finalId = tUi.name.Split('@').First();
                return finalId;
            }
        }

        private void OnSonCompleted(string uiId)
        {
            if (_parentTask == null)
            {
                return;
            }

            if (uiId.StartsWith("LegoComponent"))
            {
                _parentTask.SonComponentUnloadIndex++;
            }
            else
            {
                _parentTask.SonContainerUnloadIndex++;
            }
        }

        public void EnableUpdate() => _executeAble = true;

        public void UnloadAtUpdate()
        {
            for (int index = 0; index < unloadSpeed; index++)
            {
                if (!_executeAble)
                {
                    return;
                }

                var taskName = taskMeta.RootMeta.Name;
                Debug.Log($"{taskName} : {index}");

                if (unloadedCount == willUnloadCount
                    && SonComponentUnloadIndex == sonComponentNum
                    && SonContainerUnloadIndex == sonContainerNum)
                {
                    IsComplete = true;
                    taskMeta.Reset();
                    OnSonCompleted(taskMeta.RootMeta.Name);
                    if (taskMeta.RootMeta.Name.StartsWith("LegoView")
                        || taskMeta.RootMeta.Name.StartsWith("LegoComponent"))
                    {
                        var rootButton = taskRect.Find("Button_Root").GetComponent<YuLegoButton>();
                        buttonPool.Restore(rootButton);
                    }

                    rectPool.Restore(taskRect);

                    return;
                }

                UnloadNext();
            }
        }

        private void UnloadNext()
        {
            unloadingType = taskMeta.NextElement;
            if (unloadingType == LegoUIType.Component || unloadingType == LegoUIType.Container)
            {
                PushSonTask();
            }
            else
            {
                RestoreControl();
            }
        }

        private bool _executeAble = true;

        private void PushSonTask()
        {
            _executeAble = false; // 暂停当前任务执行。
            var nextRectRef = taskMeta.NextRect;
            var sonUi = taskRect.Find(nextRectRef.Name).GetComponent<RectTransform>();
            m_PushTaskAction(sonUi);
        }

        private void RestoreControl()
        {
            var nextControlId = taskMeta.NextRect.Name;

            switch (unloadingType)
            {
                case LegoUIType.None:
                    break;
                case LegoUIType.Text:
                    var text = taskRect.Find(nextControlId).GetComponent<YuLegoText>();
                    textPool.Restore(text);
                    break;
                case LegoUIType.InlineText:
                    break;
                case LegoUIType.Image:
                    var image = taskRect.Find(nextControlId).GetComponent<YuLegoImage>();
                    imagePool.Restore(image);
                    break;
                case LegoUIType.RawImage:
                    var rawImage = taskRect.Find(nextControlId).GetComponent<YuLegoRawImage>();
                    rawImagePool.Restore(rawImage);
                    break;
                case LegoUIType.Button:
                    var button = taskRect.Find(nextControlId).GetComponent<YuLegoButton>();
                    buttonPool.Restore(button);
                    break;
                case LegoUIType.InputField:
                    var inputField = taskRect.Find(nextControlId).GetComponent<YuLegoInputField>();
                    inputFieldPool.Restore(inputField);
                    break;
                case LegoUIType.Slider:
                    var slider = taskRect.Find(nextControlId).GetComponent<YuLegoSlider>();
                    sliderPool.Restore(slider);
                    break;
                case LegoUIType.Progressbar:
                    var progressbar = taskRect.Find(nextControlId).GetComponent<YuLegoProgressbar>();
                    progressbarPool.Restore(progressbar);
                    break;
                case LegoUIType.Toggle:
                    var toggle = taskRect.Find(nextControlId).GetComponent<YuLegoToggle>();
                    togglePool.Restore(toggle);
                    break;
                case LegoUIType.PlaneToggle:
                    break;
                case LegoUIType.Tab:
                    break;
                case LegoUIType.Dropdown:
                    var dropdown = taskRect.Find(nextControlId).GetComponent<YuLegoDropdown>();
                    dropdownPool.Restore(dropdown);
                    break;
                case LegoUIType.Rocker:
                    break;
                case LegoUIType.Grid:
                    break;
                case LegoUIType.ScrollView:
                    var scrollView = taskRect.Find(nextControlId).GetComponent<YuLegoScrollView>();
                    scrollViewPool.Restore(scrollView);
                    break;
                case LegoUIType.Component:
                    break;
                case LegoUIType.View:
                    break;
                case LegoUIType.TButton:
                    var tButton = taskRect.Find(nextControlId).GetComponent<YuLegoTButton>();
                    _tButtonPool.Restore(tButton);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            unloadedCount++;
        }

        public void Reset()
        {
            _executeAble = false;
            IsComplete = false;
            unloadedCount = 0;
            unloadSpeed = 0;
            willUnloadCount = 0;
            sonComponentNum = 0;
            sonContainerNum = 0;
            SonComponentUnloadIndex = 0;
            SonContainerUnloadIndex = 0;
        }
    }
}