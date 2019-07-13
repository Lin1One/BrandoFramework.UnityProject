#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Client.Extend;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using YuU3dPlay;
using Object = UnityEngine.Object;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI扩展包通用工具。
    /// </summary>
    public static class YuLegoUtility
    {
        #region 静态引用

        ////private static IYuU3dAppEntity _appEntity;

        ////private static IYuU3dAppEntity AppEntity
        ////{
        ////    get { return _appEntity ?? (_appEntity = YuU3dAppUtility.Injector.Get<IYuU3dAppEntity>()); }
        ////}

        ////private static YuU3dAppSetting u3DApp;

        ////private static YuU3dAppSetting U3DApp
        ////{
        ////    get { return u3DApp ?? (u3DApp = AppEntity.CurrentRuningU3DApp); }
        ////}

#if DEBUG
        private static readonly RectTransform rxModelRoot;

        private static RectTransform scRoot;
        private static RectTransform ScRoot
        {
            get
            {
                if (scRoot != null)
                {
                    return scRoot;
                }

                scRoot = rxModelRoot.Find("ScrollView")?.RectTransform();
                if (scRoot != null)
                {
                    return scRoot;
                }

                var tmpGo = new GameObject("ScrollView");
                tmpGo.transform.SetParent(rxModelRoot);
                scRoot = tmpGo.AddComponent<RectTransform>();
                return scRoot;
            }
        }
#endif

        #endregion

        #region 公共API

        public static void MetamorphoseRect(RectTransform rect, LegoRectTransformMeta rectMeta)
        {
            rect.name = rectMeta.Name;

            // 设置Pivot
            //rect.pivot = new Vector2(rectMeta.PivotMeta.PivotX,
            //    rectMeta.PivotMeta.PivotY);

            if (!rectMeta.IsZeroPosition)
            {
                rect.localPosition = new Vector3(
                    rectMeta.X,
                    rectMeta.Y,
                    rectMeta.Z
                );
            }

            if (!rectMeta.IsZeroRotation)
            {
                rect.localRotation = Quaternion.Euler(
                    rectMeta.RotationX,
                    rectMeta.RotationY,
                    rectMeta.RotationZ
                );
            }

            if (!rectMeta.IsScaleOne)
            {
                rect.localScale = new Vector3(
                    rectMeta.ScaleX,
                    rectMeta.ScaleY,
                    rectMeta.ScaleZ
                );
            }
            else
            {
                rect.localScale = Vector3.one;
            }

            rect.sizeDelta = new Vector2(rectMeta.Width, rectMeta.Height);

            if (!rectMeta.IsDefaultActive)
            {
                rect.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}