#region Head

// Author:            Yu
// CreateDate:        2018/8/17 8:15:22
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

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
    public class LegoComponentRef
    {
        [LabelText("组件名")] public string RefComponent;

        [LabelText("挂载路径")] public string MountPath;

        [LabelText("逻辑Id")] public string LogicId;

        [LabelText("挂载位置")] public LegoRectTransformMeta MountPosition;

        public static LegoComponentRef Create(RectTransform rect)
        {
            string path = null;
            var view = rect.FindParent<YuLegoViewHelper>(out path);
            if (view == null)
            {
                throw new Exception("乐高组件必须置于乐高视图下");
            }

            var meta = new LegoComponentRef
            {
                RefComponent = GetComponentId(rect),
                MountPath = path,
                LogicId = rect.name,
                MountPosition = LegoRectTransformMeta.Create(rect),
            };

            return meta;
        }

        private static string GetComponentId(RectTransform rect)
        {
            if (rect.name.Contains("@"))
            {
                return rect.name.Split('@')[0];
            }
            else
            {
                return rect.name;
            }
        }
    }
}