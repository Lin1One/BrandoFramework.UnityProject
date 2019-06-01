#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="EditableKeyValuePairResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="EditableKeyValuePairResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Serialization;
    using System;

    [ResolverPriority(-1)]
    public class EditableKeyValuePairResolver<TKey, TValue> : OdinPropertyResolver<EditableKeyValuePair<TKey, TValue>>, IHasSpecialPropertyPaths, IMaySupportPrefabModifications
    {
        private static InspectorPropertyInfo[] ChildInfos;

        public bool MaySupportPrefabModifications { get { return DictionaryKeyUtility.KeyTypeSupportsPersistentPaths(typeof(TKey)); } }

        public string GetSpecialChildPath(int childIndex)
        {
            if (this.Property.Parent != null && this.Property.Parent.ChildResolver is IKeyValueMapResolver)
            {
                var key = this.ValueEntry.SmartValue.Key;
                var keyStr = DictionaryKeyUtility.GetDictionaryKeyString(key);

                if (childIndex == 0) // Key
                {
                    return this.Property.Parent.Path + "." + keyStr + "#key";
                }
                else if (childIndex == 1) // Value
                {
                    return this.Property.Parent.Path + "." + keyStr;
                }
            }
            else
            {
                if (childIndex == 0)
                {
                    return this.Property.Path + ".Key";
                }
                else if (childIndex == 1)
                {
                    return this.Property.Path + ".Value";
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        protected override void Initialize()
        {
            if (ChildInfos == null)
            {
                ChildInfos = InspectorPropertyInfoUtility.GetDefaultPropertiesForType(this.Property, typeof(EditableKeyValuePair<TKey, TValue>), false);
            }
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            return ChildInfos[childIndex];
        }

        protected override int GetChildCount(EditableKeyValuePair<TKey, TValue> value)
        {
            return ChildInfos.Length;
        }

        public override int ChildNameToIndex(string name)
        {
            if (name == "Key") return 0;
            if (name == "Value") return 1;
            if (name == "#Value") return 1;
            return -1;
        }
    }
}
#endif