//#region Head

//// Author:            Yu
//// CreateDate:        2018/10/13 20:17:51
//// Email:             35490136@qq.com

///*
// * 修改日期  ：
// * 修改人    ：
// * 修改内容  ：
//*/

//#endregion

//using System.Collections.Generic;
//using UnityEngine;
//using YuCommon;

//namespace YuLegoUIPlay
//{
//    /// <summary>
//    /// 不规则布局滚动视图
//    /// </summary>
//    public class YuLegoLrregularScrollView : MonoBehaviour, IYuLegoControl
//    {
//        private int currentY = 10;
//        private int xOffset = 10;
//        private int yDistance = 5;

//        private int selfWidth;
//        private int selfHeight;
//        private int selfHalfWidth;
//        private int selfHalfHeight;

//        private void OnEnable()
//        {
//            selfWidth = (int) RectTransform.sizeDelta.x;
//            selfHeight = (int) RectTransform.sizeDelta.y;
//            selfHalfWidth = selfWidth / 2;
//            selfHalfHeight = selfHeight / 2;
//        }

//        private readonly Dictionary<string, IYuLegoComponent> _components
//            = new Dictionary<string, IYuLegoComponent>();

//        public void AddComponent(IYuLegoComponent component)
//            => AddComponent(component.UIRect);

//        public void AddComponent(RectTransform rect)
//        {
//            var size = rect.sizeDelta;
//            var xPos = selfHalfWidth - (xOffset + size.x / 2);
//            var yPos = selfHalfHeight - (size.y / 2) - currentY;
//            xPos = xPos * -1;
//            currentY = currentY + (int) size.y + yDistance;
//            rect.SetParent(transform);
//            rect.localScale = Vector3.one;
//            rect.localPosition = new Vector3(xPos, yPos, 0);
//        }

//        #region Lego接口

//        public IYuLegoUI LocUI { get; private set; }
//        private RectTransform rectTransform;

//        public RectTransform RectTransform
//        {
//            get
//            {
//                if (rectTransform != null)
//                {
//                    return rectTransform;
//                }

//                rectTransform = GetComponent<RectTransform>();
//                return rectTransform;
//            }
//        }

//        private GameObject selfGo;

//        public GameObject GameObject
//        {
//            get
//            {
//                if (selfGo != null)
//                {
//                    return selfGo;
//                }

//                selfGo = RectTransform.gameObject;
//                return selfGo;
//            }
//        }

//        public string Name => RectTransform.name;

//        public void Metamorphose(YuLegoUIMeta uiMeta)
//        {
//        }

//        public YuLegoMetamorphoseStage MetamorphoseStage { get; }

//        public void Construct(IYuLegoUI locUI)
//        {
//            LocUI = locUI;
//        }

//        public T GetLocView<T>() where T : class, IYuLegoView
//        {
//            return LocUI.As<T>();
//        }

//        public T GetLocComponent<T>() where T : class, IYuLegoComponent
//        {
//            return LocUI.As<T>();
//        }

//        #endregion
//    }
//}