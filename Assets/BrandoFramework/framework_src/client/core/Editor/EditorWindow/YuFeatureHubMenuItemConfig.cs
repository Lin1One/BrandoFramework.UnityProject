using Common;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.EditorWindow
{
    /// <summary>
    /// 功能中心菜单构建器配置可视化工具。
    /// </summary>
    [Serializable]
    public class YuFeatureHubMenuItemConfig
    {
        [ShowInInspector]
        [ListDrawerSettings(Expanded = true,DraggableItems = true,HideRemoveButton = true)]
        [TableList(IsReadOnly = true,CellPadding = 3)]
        [LabelText("菜单选项配置列表")]
        private readonly List<YuMenuItemPriorityItem> PriorityItems = new List<YuMenuItemPriorityItem>();

        private List<IYuMenuEditorWindowItemBuilder> builders;

        public void SetMenuPriorityUp(YuMenuItemPriorityItem item)
        {
            if(PriorityItems.Contains(item))
            {
                var originalIndex =  PriorityItems.IndexOf(item);
                if (originalIndex == 0)
                {
                    return;
                }
                var temp = PriorityItems[originalIndex];
                PriorityItems[originalIndex] = PriorityItems[originalIndex - 1];
                PriorityItems[originalIndex - 1] = temp;
            }
        }

        #region 功能菜单设置

        //[HorizontalGroup("功能菜单项操作按钮")]
        //[Button("读取菜单项配置", ButtonSizes.Medium)]
        //private void InitBuilderDescs()
        //{
        //    PriorityItems.Clear();
        //    builders = YuMenuItemBuilderFactory.GetBuilders(typeof(EditorFunctionMenuWindow));
        //    foreach (var builder in builders)
        //    {
        //        var cache = YuMenuItemBuilderFactory.GetItemDescCache(builder);
        //        var Item = new YuMenuItemPriorityItem(cache.Desc, builder.TypeId())
        //        {
        //            ItemUpAction = SetMenuPriorityUp,
        //            isLoadDefault = Convert.ToBoolean(
        //                PlayerPrefs.GetString(builder.TypeId() + "IsLoadIn" + typeof(EditorFunctionMenuWindow), "true"))
        //        };
        //        PriorityItems.Add(Item);
        //    }
        //}

        //[HorizontalGroup("功能菜单项操作按钮")]
        //[Button("保存菜单项配置", ButtonSizes.Medium)]
        //private void SaveBuildPriorityPrefs()
        //{
        //    for (int index = 0; index < PriorityItems.Count; index++)
        //    {
        //        var menuItem = PriorityItems[index];
        //        PlayerPrefs.SetInt(menuItem.BuilderId, index);
        //        PlayerPrefs.SetString(
        //            menuItem.BuilderId + "IsLoadIn" + typeof(EditorFunctionMenuWindow),menuItem.isLoadDefault.ToString());
        //    }
        //    //YuEditorAPIInvoker.DisplayTip("功能中心的菜单项优先级设置已更新！");
        //}

        #endregion
    }

    [Serializable]
    public class YuMenuItemPriorityItem
    {
        [LabelWidth(120)]
        [HorizontalGroup("功能菜单")]
        [HideLabel]
        public string BuilderDesc;

        [HorizontalGroup("功能菜单",Width  = 10)]
        [HideLabel]
       
        public bool isLoadDefault;

        [HideInInspector]
        public string BuilderId { get; private set; }

        [HideInInspector]
        public Action<YuMenuItemPriorityItem> ItemUpAction;

        [HorizontalGroup("功能菜单", Width = 25)]
        [Button("⇧")]
        public void OnClickItemUp()
        {
            ItemUpAction.Invoke(this);
        }

        public YuMenuItemPriorityItem(string buidlerDesc, string builderId)
        {
            BuilderDesc = buidlerDesc;
            BuilderId = builderId;
        }

    }
}

