#region Head

// Author:            Yu
// CreateDate:        2018/8/14 21:59:33
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using Common.DataStruct;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using YuCommon;

namespace Client.LegoUI
{
    /// <summary>
    /// RectTransform组件元信息。
    /// </summary>
    [Serializable]
    public class LegoRectTransformMeta
    {
        [LabelText("元素物体名")]
        public string Name;

        public string PropertyId
        {
            get
            {
                if (Name.Contains("@"))
                {
                    if (Name.StartsWith("LegoComponent"))
                    {
                        var id = Name.Replace("@", "_").Replace("=", "_");
                        return id;
                    }

                    var array = Name.Split('@');
                    var codeId = array[0];
                    return codeId;
                }

                return Name;
            }
        }

        /// <summary>
        /// UI物体类型字符串。
        /// 视图、组件或者容器。
        /// </summary>
        public string TypeId
        {
            get
            {
                if (Name.Contains("@"))
                {
                    var array = Name.Split('@');
                    var id = array[0];
                    return id;
                }

                return Name;
            }
        }

        /// <summary>
        /// 是否默认处于激活状态。
        /// </summary>
        [LabelText("是否默认处于激活状态")] public bool IsDefaultActive;

        #region 优化标志位

        public bool IsZeroPosition;
        public bool IsZeroRotation;
        public bool IsScaleOne;

        #endregion

        #region Position

        public float X;
        public float Y;
        public float Z;
        public float Width;
        public float Height;

        #endregion

        #region Rotation

        public float RotationX;
        public float RotationY;
        public float RotationZ;

        #endregion

        #region Scale

        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;

        #endregion

        #region Pivot

        [Serializable]
        public class YuLegoPivotMeta
        {
            public float PivotX;
            public float PivotY;
        }

        public YuLegoPivotMeta PivotMeta = new YuLegoPivotMeta() { PivotX = 0.5f, PivotY = 0.5f };

        #endregion

        #region UI特效元数据

        [LabelText("UI特效信息列表")]
        public List<LegoUIEffectInfo> EffectPrefabIds;

        #endregion

        private const float scaleValue = 1.005f;

        public static LegoRectTransformMeta Create(RectTransform rect)
        {
            var meta = new LegoRectTransformMeta
            {
                Name = rect.name,
                IsDefaultActive = rect.gameObject.activeSelf,
                X = rect.localPosition.x,
                Y = rect.localPosition.y,
                Z = rect.localPosition.z
            };

            if (meta.X.Equal(0f) && meta.Y.Equal(0f) && meta.Z.Equal(0f))
            {
                meta.IsZeroPosition = true;
            }

            meta.Width = rect.sizeDelta.x;
            meta.Height = rect.sizeDelta.y;

            meta.PivotMeta.PivotX = rect.pivot.x;
            meta.PivotMeta.PivotY = rect.pivot.y;

            meta.RotationX = rect.localRotation.eulerAngles.x;
            meta.RotationY = rect.localRotation.eulerAngles.y;
            meta.RotationZ = rect.localRotation.eulerAngles.z;
            if (meta.RotationX.Equal(0f) && meta.RotationY.Equal(0f) && meta.RotationZ.Equal(0f))
            {
                meta.IsZeroRotation = true;
            }

            // unity的缩放值由于是浮点数反序列化后会出现偏差。
            // 因此这里把所有轴的缩放值都和一个常量比较，达不到常量精度的都修正为1f。
            var localScale = rect.localScale;
            meta.ScaleX = localScale.x > scaleValue ? localScale.x : 1f;
            meta.ScaleY = localScale.y > scaleValue ? localScale.y : 1f;
            meta.ScaleZ = localScale.z > scaleValue ? localScale.z : 1f;
            if (meta.ScaleX.Equal(1f) && meta.ScaleY.Equal(1f) && meta.ScaleZ.Equal(1f))
            {
                meta.IsScaleOne = true;
            }

            // 扫描UI特效以构建特效元数据
            meta.CreateUIEffectMeta(rect);

            return meta;
        }

        private const string EFFECTID_START = "effect";

        private void CreateUIEffectMeta(RectTransform control)
        {
            var index = 0;
            var childCount = control.childCount;

            while (index < childCount)
            {
                var child = control.GetChild(index);
                if (!child.name.StartsWith(EFFECTID_START))
                {
                    index++;
                    continue;
                }

                var effectInfo = LegoUIEffectInfo.Create(child);
                if (EffectPrefabIds == null)
                {
                    EffectPrefabIds = new List<LegoUIEffectInfo>();
                }

                EffectPrefabIds.Add(effectInfo);
                Debug.LogError($"在物体{control.name}下发现UI特效{child.name}！");
                index++;
            }
        }

    }
}