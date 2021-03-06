#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="HideInPrefabAssetsAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HideInPrefabAssetsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HideInPrefabAssetsAttribute"/>.
    /// </summary>

    [DrawerPriority(1000, 0, 0)]
    public sealed class HideInPrefabAssetsAttributeDrawer : OdinAttributeDrawer<HideInPrefabAssetsAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;

            var unityObjectTarget = property.Tree.WeakTargets[0] as UnityEngine.Object;

            if (unityObjectTarget == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            PropertyContext<bool> hide;

            if (property.Context.Get(this, "hide", out hide))
            {
                var type = PrefabUtility.GetPrefabType(unityObjectTarget);
                hide.Value =
                    type == PrefabType.Prefab ||
                    type == PrefabType.ModelPrefab;
            }

            if (hide.Value == false)
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif