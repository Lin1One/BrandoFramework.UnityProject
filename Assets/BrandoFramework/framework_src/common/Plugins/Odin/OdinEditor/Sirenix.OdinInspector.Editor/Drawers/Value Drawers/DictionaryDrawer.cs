#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DictionaryDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.Serialization;

    /// <summary>
    /// Property drawer for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public class DictionaryDrawer<TDictionary, TKey, TValue> : OdinValueDrawer<TDictionary> where TDictionary : IDictionary<TKey, TValue>
    {
        private const string CHANGE_ID = "DICTIONARY_DRAWER";
        private static readonly bool KeyIsValueType = typeof(TKey).IsValueType;
        private static GUIStyle addKeyPaddingStyle;

        private static GUIStyle AddKeyPaddingStyle
        {
            get
            {
                if (addKeyPaddingStyle == null)
                {
                    addKeyPaddingStyle = new GUIStyle("CN Box")
                    {
                        overflow = new RectOffset(0, 0, 1, 0),
                        fixedHeight = 0,
                        stretchHeight = false,
                        padding = new RectOffset(10, 10, 10, 10)
                    };
                }

                return addKeyPaddingStyle;
            }
        }

        private class Context
        {
            public GUIPagingHelper Paging = new GUIPagingHelper();
            public GeneralDrawerConfig Config;
            public LocalPersistentContext<bool> Toggled;
            public float KeyWidthOffset;
            public bool ShowAddKeyGUI = false;
            public bool? NewKewIsValid;
            public string NewKeyErrorMessage;
            public TKey NewKey;
            public TValue NewValue;
            public StrongDictionaryPropertyResolver<TDictionary, TKey, TValue> DictionaryResolver;
            public GUIContent Label;
            public DictionaryDrawerSettings AttrSettings;
            public bool DisableAddKey;

            public TempKeyValuePair<TKey, TValue> TempKeyValue;
            public IPropertyValueEntry<TKey> TempKeyEntry;
            public IPropertyValueEntry<TValue> TempValueEntry;

            public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
            {
                padding = new RectOffset(7, 20, 3, 3)
            };
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var context = entry.Property.Context.Get(this, "context", (Context)null);
            if (context.Value == null)
            {
                context.Value = new Context();
                context.Value.Toggled = entry.Context.GetPersistent(this, "Toggled", GeneralDrawerConfig.Instance.OpenListsByDefault);
                context.Value.KeyWidthOffset = 130;
                context.Value.Label = label ?? new GUIContent(typeof(TDictionary).GetNiceName());
                context.Value.AttrSettings = entry.Property.Info.GetAttribute<DictionaryDrawerSettings>() ?? new DictionaryDrawerSettings();
                context.Value.DisableAddKey = entry.Property.Tree.PrefabModificationHandler.HasPrefabs && !entry.Property.SupportsPrefabModifications;

                if (!context.Value.DisableAddKey)
                {
                    context.Value.TempKeyValue = new TempKeyValuePair<TKey, TValue>();

                    var tree = PropertyTree.Create(context.Value.TempKeyValue);
                    tree.UpdateTree();

                    context.Value.TempKeyEntry = (IPropertyValueEntry<TKey>)tree.GetPropertyAtPath("Key").ValueEntry;
                    context.Value.TempValueEntry = (IPropertyValueEntry<TValue>)tree.GetPropertyAtPath("Value").ValueEntry;
                }
            }

            context.Value.DictionaryResolver = entry.Property.ChildResolver as StrongDictionaryPropertyResolver<TDictionary, TKey, TValue>;
            context.Value.Config = GeneralDrawerConfig.Instance;
            context.Value.Paging.NumberOfItemsPerPage = context.Value.Config.NumberOfItemsPrPage;
            context.Value.ListItemStyle.padding.right = !entry.IsEditable || context.Value.AttrSettings.IsReadOnly ? 4 : 20;

            SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            {
                context.Value.Paging.Update(elementCount: entry.Property.Children.Count);
                this.DrawToolbar(entry, context.Value);
                context.Value.Paging.Update(elementCount: entry.Property.Children.Count);

                if (!context.Value.DisableAddKey && context.Value.AttrSettings.IsReadOnly == false)
                {
                    this.DrawAddKey(entry, context.Value);
                }

                float t;
                GUIHelper.BeginLayoutMeasuring();
                if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(entry.Property, this), context.Value.Toggled.Value, out t))
                {
                    var rect = SirenixEditorGUI.BeginVerticalList(false);
                    if (context.Value.AttrSettings.DisplayMode == DictionaryDisplayOptions.OneLine)
                    {
                        var maxWidth = rect.width - 90;
                        rect.xMin = context.Value.KeyWidthOffset + 22;
                        rect.xMax = rect.xMin + 10;
                        context.Value.KeyWidthOffset = context.Value.KeyWidthOffset + SirenixEditorGUI.SlideRect(rect).x;

                        if (Event.current.type == EventType.Repaint)
                        {
                            context.Value.KeyWidthOffset = Mathf.Clamp(context.Value.KeyWidthOffset, 90, maxWidth);
                        }

                        if (context.Value.Paging.ElementCount != 0)
                        {
                            var headerRect = SirenixEditorGUI.BeginListItem(false);
                            {
                                GUILayout.Space(14);
                                if (Event.current.type == EventType.Repaint)
                                {
                                    GUI.Label(headerRect.SetWidth(context.Value.KeyWidthOffset), context.Value.AttrSettings.KeyLabel, SirenixGUIStyles.LabelCentered);
                                    GUI.Label(headerRect.AddXMin(context.Value.KeyWidthOffset), context.Value.AttrSettings.ValueLabel, SirenixGUIStyles.LabelCentered);
                                    SirenixEditorGUI.DrawSolidRect(headerRect.AlignBottom(1), SirenixGUIStyles.BorderColor);
                                }
                            }
                            SirenixEditorGUI.EndListItem();
                        }
                    }

                    this.DrawElements(entry, label, context.Value);
                    SirenixEditorGUI.EndVerticalList();
                }
                SirenixEditorGUI.EndFadeGroup();

                // Draw borders
                var outerRect = GUIHelper.EndLayoutMeasuring();
                if (t > 0.01f && Event.current.type == EventType.Repaint)
                {
                    Color col = SirenixGUIStyles.BorderColor;
                    outerRect.yMin -= 1;
                    SirenixEditorGUI.DrawBorders(outerRect, 1, col);
                    col.a *= t;
                    if (context.Value.AttrSettings.DisplayMode == DictionaryDisplayOptions.OneLine)
                    {
                        // Draw Slide Rect Border
                        outerRect.width = 1;
                        outerRect.x += context.Value.KeyWidthOffset + 13;
                        SirenixEditorGUI.DrawSolidRect(outerRect, col);
                    }
                }
            }
            SirenixEditorGUI.EndIndentedVertical();
        }

        private void DrawAddKey(IPropertyValueEntry<TDictionary> entry, Context context)
        {
            if (entry.IsEditable == false || context.AttrSettings.IsReadOnly)
            {
                return;
            }

            if (SirenixEditorGUI.BeginFadeGroup(context, context.ShowAddKeyGUI))
            {
                GUILayout.BeginVertical(AddKeyPaddingStyle);
                {
                    if (typeof(TKey) == typeof(string) && context.NewKey == null)
                    {
                        context.NewKey = (TKey)(object)"";
                        context.NewKewIsValid = null;
                    }

                    if (context.NewKewIsValid == null)
                    {
                        context.NewKewIsValid = CheckKeyIsValid(entry, context.NewKey, out context.NewKeyErrorMessage);
                    }

                    InspectorUtilities.BeginDrawPropertyTree(context.TempKeyEntry.Property.Tree, false);

                    // Key
                    {
                        //context.TempKeyValue.key = context.NewKey;
                        context.TempKeyEntry.Property.Update();

                        EditorGUI.BeginChangeCheck();

                        context.TempKeyEntry.Property.Draw(new GUIContent(context.AttrSettings.KeyLabel));

                        bool changed1 = EditorGUI.EndChangeCheck();
                        bool changed2 = context.TempKeyEntry.ApplyChanges();

                        if (changed1 || changed2)
                        {
                            context.NewKey = context.TempKeyValue.Key;
                            EditorApplication.delayCall += () => context.NewKewIsValid = null;
                            GUIHelper.RequestRepaint();
                        }
                    }

                    // Value
                    {
                        //context.TempKeyValue.value = context.NewValue;
                        context.TempValueEntry.Property.Update();
                        context.TempValueEntry.Property.Draw(new GUIContent(context.AttrSettings.ValueLabel));
                        context.TempValueEntry.ApplyChanges();
                        context.NewValue = context.TempKeyValue.Value;
                    }

                    context.TempKeyEntry.Property.Tree.InvokeDelayedActions();
                    var changed = context.TempKeyEntry.Property.Tree.ApplyChanges();

                    if (changed)
                    {
                        context.NewKey = context.TempKeyValue.Key;
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();
                    }

                    InspectorUtilities.EndDrawPropertyTree(context.TempKeyEntry.Property.Tree);

                    GUIHelper.PushGUIEnabled(GUI.enabled && context.NewKewIsValid.Value);
                    if (GUILayout.Button(context.NewKewIsValid.Value ? "Add" : context.NewKeyErrorMessage))
                    {
                        context.DictionaryResolver.QueueSet(Enumerable.Repeat<object>(context.NewKey, entry.ValueCount).ToArray(), Enumerable.Repeat<object>(context.NewValue, entry.ValueCount).ToArray());
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();

                        entry.Property.Tree.DelayActionUntilRepaint(() =>
                        {
                            context.NewValue = default(TValue);
                            context.TempKeyValue.Value = default(TValue);
                            context.TempValueEntry.Update();
                        });
                    }
                    GUIHelper.PopGUIEnabled();
                }
                GUILayout.EndVertical();
            }
            SirenixEditorGUI.EndFadeGroup();
        }

        private void DrawToolbar(IPropertyValueEntry<TDictionary> entry, Context context)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                if (entry.ListLengthChangedFromPrefab) GUIHelper.PushIsBoldLabel(true);

                if (context.Config.HideFoldoutWhileEmpty && context.Paging.ElementCount == 0)
                {
                    GUILayout.Label(context.Label, GUILayoutOptions.ExpandWidth(false));
                }
                else
                {
                    context.Toggled.Value = SirenixEditorGUI.Foldout(context.Toggled.Value, context.Label);
                }

                if (entry.ListLengthChangedFromPrefab) GUIHelper.PopIsBoldLabel();

                GUILayout.FlexibleSpace();

                // Item Count
                if (context.Config.ShowItemCount)
                {
                    if (entry.ValueState == PropertyValueState.CollectionLengthConflict)
                    {
                        int min = entry.Values.Min(x => x.Count);
                        int max = entry.Values.Max(x => x.Count);
                        GUILayout.Label(min + " / " + max + " items", EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        GUILayout.Label(context.Paging.ElementCount == 0 ? "Empty" : context.Paging.ElementCount + " items", EditorStyles.centeredGreyMiniLabel);
                    }
                }

                bool hidePaging =
                        context.Config.HidePagingWhileCollapsed && context.Toggled.Value == false ||
                        context.Config.HidePagingWhileOnlyOnePage && context.Paging.PageCount == 1;

                if (!hidePaging)
                {
                    var wasEnabled = GUI.enabled;
                    bool pagingIsRelevant = context.Paging.IsEnabled && context.Paging.PageCount != 1;

                    GUI.enabled = wasEnabled && pagingIsRelevant && !context.Paging.IsOnFirstPage;
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowLeft, true))
                    {
                        if (Event.current.button == 0)
                        {
                            context.Paging.CurrentPage--;
                        }
                        else
                        {
                            context.Paging.CurrentPage = 0;
                        }
                    }

                    GUI.enabled = wasEnabled && pagingIsRelevant;
                    var width = GUILayoutOptions.Width(10 + context.Paging.PageCount.ToString().Length * 10);
                    context.Paging.CurrentPage = EditorGUILayout.IntField(context.Paging.CurrentPage + 1, width) - 1;
                    GUILayout.Label(GUIHelper.TempContent("/ " + context.Paging.PageCount));

                    GUI.enabled = wasEnabled && pagingIsRelevant && !context.Paging.IsOnLastPage;
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.ArrowRight, true))
                    {
                        if (Event.current.button == 0)
                        {
                            context.Paging.CurrentPage++;
                        }
                        else
                        {
                            context.Paging.CurrentPage = context.Paging.PageCount - 1;
                        }
                    }

                    GUI.enabled = wasEnabled && context.Paging.PageCount != 1;
                    if (context.Config.ShowExpandButton)
                    {
                        if (SirenixEditorGUI.ToolbarButton(context.Paging.IsEnabled ? EditorIcons.ArrowDown : EditorIcons.ArrowUp, true))
                        {
                            context.Paging.IsEnabled = !context.Paging.IsEnabled;
                        }
                    }
                    GUI.enabled = wasEnabled;
                }
                if (!context.DisableAddKey && context.AttrSettings.IsReadOnly != true)
                {
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
                    {
                        context.ShowAddKeyGUI = !context.ShowAddKeyGUI;
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private static GUIStyle oneLineMargin;

        private static GUIStyle OneLineMargin
        {
            get
            {
                if (oneLineMargin == null)
                {
                    oneLineMargin = new GUIStyle() { margin = new RectOffset(8, 0, 0, 0) };
                }
                return oneLineMargin;
            }
        }

        private static GUIStyle headerMargin;

        private static GUIStyle HeaderMargin
        {
            get
            {
                if (headerMargin == null)
                {
                    headerMargin = new GUIStyle() { margin = new RectOffset(40, 0, 0, 0) };
                }
                return headerMargin;
            }
        }

        private void DrawElements(IPropertyValueEntry<TDictionary> entry, GUIContent label, Context context)
        {
            for (int i = context.Paging.StartIndex; i < context.Paging.EndIndex; i++)
            {
                var keyValuePairProperty = entry.Property.Children[i];
                var keyValuePairValue = (keyValuePairProperty.ValueEntry as IPropertyValueEntry<EditableKeyValuePair<TKey, TValue>>).SmartValue;

                Rect rect = SirenixEditorGUI.BeginListItem(false, context.ListItemStyle);
                {
                    if (context.AttrSettings.DisplayMode != DictionaryDisplayOptions.OneLine)
                    {
                        bool defaultExpanded;
                        switch (context.AttrSettings.DisplayMode)
                        {
                            case DictionaryDisplayOptions.CollapsedFoldout:
                                defaultExpanded = false;
                                break;

                            case DictionaryDisplayOptions.ExpandedFoldout:
                                defaultExpanded = true;
                                break;

                            default:
                                defaultExpanded = SirenixEditorGUI.ExpandFoldoutByDefault;
                                break;
                        }
                        var isExpanded = keyValuePairProperty.Context.GetPersistent(this, "Expanded", defaultExpanded);

                        SirenixEditorGUI.BeginBox();
                        SirenixEditorGUI.BeginToolbarBoxHeader();
                        {
                            if (keyValuePairValue.IsInvalidKey)
                            {
                                GUIHelper.PushColor(Color.red);
                            }
                            var btnRect = GUIHelper.GetCurrentLayoutRect().AlignLeft(HeaderMargin.margin.left);
                            btnRect.y += 1;
                            GUILayout.BeginVertical(HeaderMargin);
                            GUIHelper.PushIsDrawingDictionaryKey(true);

                            GUIHelper.PushLabelWidth(10);

                            keyValuePairProperty.Children[0].Draw(null);

                            GUIHelper.PopLabelWidth();

                            GUIHelper.PopIsDrawingDictionaryKey();
                            GUILayout.EndVertical();
                            if (keyValuePairValue.IsInvalidKey)
                            {
                                GUIHelper.PopColor();
                            }
                            isExpanded.Value = SirenixEditorGUI.Foldout(btnRect, isExpanded.Value, GUIHelper.TempContent("Key"));
                        }
                        SirenixEditorGUI.EndToolbarBoxHeader();

                        if (SirenixEditorGUI.BeginFadeGroup(isExpanded, isExpanded.Value))
                        {
                            keyValuePairProperty.Children[1].Draw(null);
                        }
                        SirenixEditorGUI.EndFadeGroup();

                        SirenixEditorGUI.EndToolbarBox();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical(GUILayoutOptions.Width(context.KeyWidthOffset));
                        {
                            var keyProperty = keyValuePairProperty.Children[0];

                            if (keyValuePairValue.IsInvalidKey)
                            {
                                GUIHelper.PushColor(Color.red);
                            }

                            if (context.AttrSettings.IsReadOnly) GUIHelper.PushGUIEnabled(false);

                            GUIHelper.PushIsDrawingDictionaryKey(true);
                            GUIHelper.PushLabelWidth(10);

                            keyProperty.Draw(null);

                            GUIHelper.PopLabelWidth();
                            GUIHelper.PopIsDrawingDictionaryKey();

                            if (context.AttrSettings.IsReadOnly) GUIHelper.PopGUIEnabled();

                            if (keyValuePairValue.IsInvalidKey)
                            {
                                GUIHelper.PopColor();
                            }
                        }
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(OneLineMargin);
                        {
                            GUIHelper.PushHierarchyMode(false);
                            var valueEntry = keyValuePairProperty.Children[1];
                            var tmp = GUIHelper.ActualLabelWidth;
                            GUIHelper.BetterLabelWidth = 150;
                            valueEntry.Draw(null);
                            GUIHelper.BetterLabelWidth = tmp;
                            GUIHelper.PopHierarchyMode();
                        }
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                    
                    if (entry.IsEditable && !context.AttrSettings.IsReadOnly && SirenixEditorGUI.IconButton(new Rect(rect.xMax - 24 + 5, rect.y + 4 + ((int)rect.height - 23) / 2, 14, 14), EditorIcons.X))
                    {
                        context.DictionaryResolver.QueueRemoveKey(Enumerable.Range(0, entry.ValueCount).Select(n => context.DictionaryResolver.GetKey(n, i)).ToArray());
                        EditorApplication.delayCall += () => context.NewKewIsValid = null;
                        GUIHelper.RequestRepaint();
                    }
                }
                SirenixEditorGUI.EndListItem();
            }

            if (context.Paging.IsOnLastPage && entry.ValueState == PropertyValueState.CollectionLengthConflict)
            {
                SirenixEditorGUI.BeginListItem(false);
                GUILayout.Label(GUIHelper.TempContent("------"), EditorStyles.centeredGreyMiniLabel);
                SirenixEditorGUI.EndListItem();
            }
        }

        private static bool CheckKeyIsValid(IPropertyValueEntry<TDictionary> entry, TKey key, out string errorMessage)
        {
            if (!KeyIsValueType && object.ReferenceEquals(key, null))
            {
                errorMessage = "Key cannot be null.";
                return false;
            }

            var keyStr = DictionaryKeyUtility.GetDictionaryKeyString(key);

            if (entry.Property.Children[keyStr] == null)
            {
                errorMessage = "";
                return true;
            }
            else
            {
                errorMessage = "An item with the same key already exists.";
                return false;
            }
        }
    }
}
#endif