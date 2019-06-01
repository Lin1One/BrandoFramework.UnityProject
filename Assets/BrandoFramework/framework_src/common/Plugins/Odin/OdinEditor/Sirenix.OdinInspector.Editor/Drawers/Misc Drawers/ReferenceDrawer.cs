#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ReferenceDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ReferenceDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;

    /// <summary>
    /// Draws all reference type properties, which has already been drawn elsewhere. This drawer adds an additional foldout to prevent infinite draw depth.
    /// </summary>
    [AllowGUIEnabledForReadonly]
    [DrawerPriority(90, 0, 0)]
    public sealed class ReferenceDrawer<T> : OdinValueDrawer<T> where T : class
    {
        private bool drawAsReference;

        /// <summary>
        /// Prevents the drawer from being applied to UnityEngine.Object references since they are shown as an object field, and is not drawn in-line.
        /// </summary>
        public override bool CanDrawTypeFilter(Type type)
        {
            return
                !typeof(MemberInfo).IsAssignableFrom(type) &&
                !typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;

            if (Event.current.type == EventType.Layout)
            {
                this.drawAsReference = entry.ValueState == PropertyValueState.Reference;
            }

            if (this.drawAsReference)
            {
                var isToggled = this.GetPersistentValue("is_Toggled", false);
                var targetProp = entry.Property.Tree.GetPropertyAtPath(entry.TargetReferencePath);

                if (targetProp == null)
                {
                    GUILayout.Label("Reference to " + entry.TargetReferencePath + ". But no property was found at path, which is a problem.");
                    return;
                }
                SirenixEditorGUI.BeginToolbarBox();
                SirenixEditorGUI.BeginToolbarBoxHeader();
                Rect valueRect;
                isToggled.Value = SirenixEditorGUI.Foldout(isToggled.Value, label, out valueRect);
                GUI.Label(valueRect, "Reference to " + targetProp.Path, SirenixGUIStyles.LeftAlignedGreyMiniLabel);
                SirenixEditorGUI.EndToolbarBoxHeader();
                if (SirenixEditorGUI.BeginFadeGroup(entry.Context.Get(this, "k", 0), isToggled.Value))
                {
                    var isInReference = targetProp.Context.GetGlobal("is_in_reference", false);
                    bool previous = isInReference.Value;
                    isInReference.Value = true;
                    targetProp.Draw(targetProp.Label);
                    isInReference.Value = previous;
                }
                SirenixEditorGUI.EndFadeGroup();
                SirenixEditorGUI.EndToolbarBox();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif