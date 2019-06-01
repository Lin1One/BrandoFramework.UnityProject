#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="SyncVarAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="SyncVarAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;

    /// <summary>
    /// SyncVar attribute drawer.
    /// </summary>
    public class SyncVarAttributeDrawer : OdinAttributeDrawer<SyncVarAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    this.CallNextDrawer(label);
                }
                GUILayout.EndVertical();

                GUILayout.Label("SyncVar", EditorStyles.miniLabel, GUILayoutOptions.Width(52f));
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif