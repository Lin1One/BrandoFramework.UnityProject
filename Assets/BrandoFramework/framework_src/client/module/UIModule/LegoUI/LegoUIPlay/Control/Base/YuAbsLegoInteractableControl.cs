#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HandleDict =
    System.Collections.Generic.Dictionary<Client.LegoUI.LegoInteractableType, Client.LegoUI.IYuLegoActionHandler>;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高可交互视图控件。
    /// </summary>
//    [AddComponentMenu("Yu/LegoUI/Selectable", 70)]
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class YuAbsLegoInteractableControl :
        Selectable,
        ILegoControl,
        IRelease
    {
        #region 交互处理器对象池

        private static IObjectPool<HandleDict> handleDictPool;

        private static IObjectPool<HandleDict> HandleDictPool
        {
            get
            {
                if (handleDictPool != null)
                {
                    return handleDictPool;
                }

                handleDictPool = new ObjectPool<HandleDict>(
                    () => new HandleDict(), 100);
                return handleDictPool;
            }
        }

        #endregion

        #region 字段属性

        private GameObject selfGo;

        public GameObject GameObject
        {
            get
            {
                if (selfGo != null)
                {
                    return selfGo;
                }

                selfGo = RectTransform.gameObject;
                return selfGo;
            }
        }

        public LegoRectTransformMeta RectMeta { get; protected set; }

        public string Name => gameObject.name;

        public string SoundEffectId;

        public bool IsNonRectangularButtonImage;

        public static PointerEventData PointerEventData { get; protected set; }
        public static BaseEventData BaseEventData { get; protected set; }
        public static AxisEventData AxisEventData { get; protected set; }

        private readonly HandleDict handlerDict
            = HandleDictPool.Take();

        private RectTransform m_RectTransform;

        public ILegoUI LocUI { get; private set; }

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


        #endregion

        #region 构造

        public virtual void Construct(ILegoUI locUI, object obj = null)
        {
            LocUI = locUI;
        }

        #endregion

        #region 注册交互行为处理

        public void RegisterHandler
        (
            LegoInteractableType interactableType,
            IYuLegoActionHandler handler
        )
        {
            if (handlerDict.ContainsKey(interactableType))
            {
                handlerDict[interactableType] = handler;
#if DEBUG || DEBUG
                //YuDebugUtility.Log($"控件{name}的{interactableType}处理已经重定向！");
#endif
                return;
            }

            handlerDict.Add(interactableType, handler);
        }

        #endregion

        #region 调用交互处理函数

        protected virtual void InvokeInteractableMethod
        (
            LegoInteractableType interactableType
        )
        {
            if (!interactable)
            {
                return;
            }

            if (handlerDict.ContainsKey(interactableType))
            {
                try
                {
                    handlerDict[interactableType].Execute(this);
                }
                catch(System.Exception e)
                {
#if DEBUG
                    Debug.LogError("调用交互处理异常：" + e.Message + "\n" + e.StackTrace);
#endif
                }
            }
        }
        
        /// <summary>
        /// 调用可配置的 UI 交互表现方法
        /// 如：UI 音效，UI 特效
        /// </summary>
        protected virtual void InvokeConfigurableInteractDisplay(ILegoControl control)
        {
            if (!interactable)
            {
                return;
            }
            var appConfigurableInteractDisplay = Injector.Instance.Get<ILegoUIInteractDisplay>();
            appConfigurableInteractDisplay.Display(control);
        }

        #endregion

        #region 重写基础虚交互处理接口

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            BaseEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnSelect);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            BaseEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnDeselect);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnPointerDown);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnPointerUp);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnPointerEnter);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnPointerExit);
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);

            AxisEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnMove);
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnDrag);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PointerEventData = eventData;
            InvokeInteractableMethod(LegoInteractableType.OnEndDrag);
        }

        #endregion

        #region 释放及处理数据变更

        public virtual void Release()
        {
            handlerDict.Clear();
            HandleDictPool.Restore(handlerDict);
        }

        #endregion

        #region 元数据变形

        public abstract void Metamorphose(LegoUIMeta uiMeta);

        protected void MetamorphoseRect(LegoRectTransformMeta rectMeta)
        {
            YuLegoUtility.MetamorphoseRect(RectTransform, rectMeta);
        }

        public LegoMetamorphoseStage MetamorphoseStage { get; protected set; }

        #endregion

        public bool Interactable
        {
            get
            {
                return interactable;
            }

            set
            {
                interactable = value;
            }
        }
    }
}