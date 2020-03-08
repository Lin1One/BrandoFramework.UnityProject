using Client.Core;
using Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using YuU3dPlay;

namespace Client.LegoUI
{
    /// <summary>
    /// 摇杆控件。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Rocker", 40)]
    public class YuLegoRocker : ScrollRect, IYuLegoRocker
    {
        #region Lego

        public ILegoUI LocUI { get; private set; }

        private RectTransform m_RectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform != null)
                {
                    return m_RectTransform;
                }

                m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        public LegoRectTransformMeta RectMeta { get; private set; }

        public GameObject GameObject => RectTransform.gameObject;
        public string Name => RectTransform.name;

        public void Construct(ILegoUI locUI, object obj = null)
        {
            LocUI = locUI;
            //rockerPosition = RockerImage.RectTransform.localPosition;
        }

        protected override void Start()
        {
            base.Start();
            //计算摇杆块的半径
            radius = RectTransform.sizeDelta.x * 0.5f;
        }

        private static IU3DEventModule eventModule;

        private static IU3DEventModule EventModule
        {
            get
            {
                if (eventModule != null)
                {
                    return eventModule;
                }

                eventModule = Injector.Instance.Get<IU3DEventModule>();
                return eventModule;
            }
        }

        #endregion

        #region 摇杆核心逻辑

        protected float radius = 0f;

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            // 抛出摇杆被拖动事件
            EventModule.TriggerEvent(ProjectCoreEventCode.Input_DragRocker);
            var contentPostion = content.anchoredPosition;
            EventModule.TriggerEvent(ProjectCoreEventCode.Input_RockerMove, contentPostion,null);

            if (contentPostion.magnitude > radius)
            {
                contentPostion = contentPostion.normalized * radius;
                SetContentAnchoredPosition(contentPostion);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            // 抛出摇杆停止拖拽事件
            EventModule.TriggerEvent(ProjectCoreEventCode.Input_EndDragRocker);
        }

        #endregion

        #region 子组件

        private YuLegoImage imageBg;

        public YuLegoImage BgImage
        {
            get
            {
                if (imageBg != null)
                {
                    return imageBg;
                }

                imageBg = transform.Find("Image_Background").GetComponent<YuLegoImage>();
                return imageBg;
            }
        }

        private YuLegoImage rockerImage;

        public YuLegoImage RockerImage
        {
            get
            {
                if (rockerImage != null)
                {
                    return rockerImage;
                }

                rockerImage = transform.Find("Image_Rocker").GetComponent<YuLegoImage>();
                return rockerImage;
            }
        }

        #endregion

        #region 元数据变形

        private enum MetamorphoseStatus
        {
            Rocker,
            BackgroundImage,
            RockerImage
        }

        private MetamorphoseStatus metamorphose = MetamorphoseStatus.Rocker;

        private LegoRockerMeta rockerMeta;

        public void Metamorphose(LegoUIMeta uiMeta)
        {
            if (MetamorphoseStage == LegoMetamorphoseStage.Completed)
            {
                MetamorphoseStage = LegoMetamorphoseStage.Metamorphosing;
            }

            if (rockerMeta == null)
            {
                rockerMeta = uiMeta.NextRocker;
                RectMeta = uiMeta.CurrentRect;
            }

            switch (metamorphose)
            {
                case MetamorphoseStatus.Rocker:
                    YuLegoUtility.MetamorphoseRect(RectTransform, RectMeta);

                    metamorphose = MetamorphoseStatus.BackgroundImage;
                    break;
                case MetamorphoseStatus.BackgroundImage:
                    BgImage.Metamorphose(rockerMeta.BgImageMeta);

                    metamorphose = MetamorphoseStatus.RockerImage;
                    break;
                case MetamorphoseStatus.RockerImage:
                    RockerImage.Metamorphose(rockerMeta.RockerImageMeta);

                    metamorphose = MetamorphoseStatus.Rocker;
                    MetamorphoseStage = LegoMetamorphoseStage.Completed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public LegoMetamorphoseStage MetamorphoseStage { get; private set; }
            = LegoMetamorphoseStage.Completed;

        #endregion

        #region 数据响应

        public void OnBackGroundChange(string newValue) => BgImage.SpriteId = newValue;
        public void OnRockerChange(string newValue) => RockerImage.SpriteId = newValue;


        #endregion

    }
}
