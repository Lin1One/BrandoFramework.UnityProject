#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Common.Editor
{
    /// <summary>
    /// 层次面板扩展入口核心类。
    /// </summary>
    public class YuHierarchyExtension : IYuEnteredEditModeHandler, IYuEnteredPlayMode
    {
        #region 字段

        private static GameObject selectGo;
        private static Event currentEvent;

        private readonly List<IHierarchyItemChangeHandler> handlers
            = new List<IHierarchyItemChangeHandler>();

        #endregion

        public void HandleStateChange()
        {
            InitAllHandler();
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
        }

        public string LogicDesc => "层次面板扩展已启动！";

        private void InitAllHandler()
        {
            ////var asmIds = YuSetting.Instance.AllAssemblyId.Where(id => id.EndsWith("Editor")).ToList();
            ////foreach (var asmId in asmIds)
            ////{
            ////    var asm = YuUnityIOUtility.GetUnityAssembly(asmId);
            ////    var handlerTypes = ReflectUtility.GetTypeList<IHierarchyItemChangeHandler>(
            ////        false, false, asm);
            ////    foreach (var handlerType in handlerTypes)
            ////    {
            ////        var handler = (IHierarchyItemChangeHandler) Activator.CreateInstance(handlerType);
            ////        handlers.Add(handler);
            ////    }
            ////}
        }

        private void HierarchyWindowItemOnGui(int instanceid, Rect selectionrect)
        {
            currentEvent = Event.current;

            if (!selectionrect.Contains(currentEvent.mousePosition)
                || currentEvent.button != 1
                || currentEvent.type > EventType.MouseUp)
            {
                return;
            }

            selectGo = EditorUtility.InstanceIDToObject(instanceid) as GameObject;
            if (selectGo == null)
            {
                return;
            }

            // 创建特定业务的固定菜单
            if (currentEvent.command)
            {
                foreach (var handler in handlers)
                {
                    if (!handler.SpecialCheck(selectGo))
                    {
                        continue;
                    }

                    handler.MakeFixedMenu();
                    return;
                }
            }


            // 创建动态菜单
            if (currentEvent.alt)
            {
                MakeDynamicMenu();
            }
        }

        private void MakeDynamicMenu()
        {
            var menu = new GenericMenu();

            foreach (var handler in handlers)
            {
                handler.MakeDynamicMenu(menu);
            }

            menu.ShowAsContext();
            currentEvent.Use();
        }
    }
}