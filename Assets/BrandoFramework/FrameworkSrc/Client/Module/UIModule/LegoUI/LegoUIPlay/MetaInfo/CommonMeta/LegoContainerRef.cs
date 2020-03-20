#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion


using Client.Extend;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图组件引用元数据。
    /// </summary>
    [Serializable]
    public class LegoContainerRef
    {
        [LabelText("容器名")] public string ContainerName;

        [LabelText("挂载路径")] public string MountPath;

        [LabelText("挂载位置")] public LegoRectTransformMeta MountPosition;

        public static LegoContainerRef Create(RectTransform rect)
        {
            string path = null;
            var view = rect.FindParent<YuLegoViewHelper>(out path);
            if (view == null)
            {
                var component = rect.FindParent<YuLegoComponentHelper>(out path);
                throw new Exception("乐高容器必须置于乐高视图或者乐高组件下");
            }

            var meta = new LegoContainerRef
            {
                ContainerName = rect.name,
                MountPath = path,
                MountPosition = LegoRectTransformMeta.Create(rect)
            };

            return meta;
        }
    }
}