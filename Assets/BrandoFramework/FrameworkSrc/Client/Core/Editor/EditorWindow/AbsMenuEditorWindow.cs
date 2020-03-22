using Common;
using Common.Utility;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Core.Editor
{
    public abstract class AbsMenuEditorWindow : OdinMenuEditorWindow
    {
        protected OdinMenuTree menuTree;
        protected int ItemIndex { get; private set; }

        protected void SwitchToTargetItem(int itemIndex)
        {
            ItemIndex = itemIndex;
            SwitchToTargetItem();
        }

        protected void SwitchToTargetItem()
        {
            if (menuTree == null || ItemIndex < 0 || ItemIndex >= menuTree.MenuItems.Count)
            {
                return;
            }

            var item = menuTree.MenuItems[ItemIndex];
            //item.Value.As<IYuOnActive>()?.OnActive();
            item.Select();
        }
    }

    public abstract class AbsMenuEditorWindow<T> : AbsMenuEditorWindow
        where T : AbsMenuEditorWindow
    {
        protected static T Instance { get; private set; }
        private bool isShow;
        private static Dictionary<Type, bool> showStates;
        private static Dictionary<Type, bool> ShowStates => showStates ?? (showStates = new Dictionary<Type, bool>());

        private static bool GetShowState()
        {
            var type = typeof(T);

            if (ShowStates.ContainsKey(type))
            {
                return ShowStates[type];
            }

            ShowStates.Add(type, false);
            return false;
        }

        protected static void OpenOrCloseWindow()
        {
            var showState = GetShowState();

            if (showState)
            {
                CloseWindow();
            }
            else
            {
                OpenWindow();
            }

            void CloseWindow()
            {
                Instance.Close();
                ShowStates[typeof(T)] = true;
            }
        }

        protected static T OpenWindow()
        {
            if (Instance != null)
            {
                Instance.Focus();
                Instance.Show();
                return Instance;
            }

            Instance = GetWindow<T>();
            InitMinSize();
            InitTitle();
            Instance.Show();
            ShowStates[typeof(T)] = true;
            return Instance;
        }

        private static void InitMinSize()
        {
            var sizeAttr = typeof(T).GetAttribute<EditorWindowSizeAttribute>();
            var min = sizeAttr?.Min ?? 300;
            var max = sizeAttr?.Max ?? 600;
            var size = new Vector2(min, max);
            Instance.minSize = size;
        }

        private static void InitTitle()
        {
            var titleAttr = typeof(T).GetAttribute<EditorWindowTitleAttribute>();
            Instance.titleContent = new GUIContent(titleAttr.Title);
        }

        protected List<IMenuEditorWindowItemBuilder> Builders { get; private set; }
        private bool IsInitMenuItemDatas;

        protected override void OnEnable()
        {
            base.OnEnable();

            var windowType = GetType();
            Builders = MenuItemBuilderFactory.GetBuilders(windowType);

            if (MenuTree == null)
            {
                return;
            }

            foreach (var menuItem in MenuTree.MenuItems)
            {
                foreach (var item in menuItem.ChildMenuItems)
                {
                    //item.As<IYuOnEnable>()?.OnEnable();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var menuItem in MenuTree.MenuItems)
            {
                foreach (var item in menuItem.ChildMenuItems)
                {
                    //item.As<IYuOnClose>()?.OnClose();
                }
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            menuTree = new OdinMenuTree();

            BuildBeforeMenuItems(menuTree);

            foreach (var builder in Builders)
            {
                var isLoadInWindow = Convert.ToBoolean(
                        PlayerPrefs.GetString(builder.TypeName() + "IsLoadIn" + GetType(), "true"));
                if (isLoadInWindow)
                {
                    builder.BuildMenuItem(menuTree);
                }
            }

            SwitchToTargetItem();
            return menuTree;
        }

        protected virtual void BuildBeforeMenuItems(OdinMenuTree tree)
        {

        }
    }
}



