#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Client.LegoUI
{
    public class YuLegoViewHelper : DevelopHelper
    {
        [LabelText("UI挂载层级")] public LegoViewType ViewType;

        [LabelText("开启时需要隐藏的视图列表")] public List<string> HideTargets;

        [LabelText("是否需要点击空白处关闭")] public bool IsBlankClose;


#if UNITY_EDITOR

        [Button("加载元数据")]
        private void LoadMeta()
        {
            var metaHelper = Injector.Instance.Get<LegoMetaHelper>();
            var uiMeta = metaHelper.GetMeta(name);
            if (uiMeta == null)
            {
                return;
            }

            ViewType = uiMeta.ViewType;
            HideTargets = uiMeta.HideTargets;
            IsBlankClose = uiMeta.IsBlankClose;
        }

#endif

    }
}