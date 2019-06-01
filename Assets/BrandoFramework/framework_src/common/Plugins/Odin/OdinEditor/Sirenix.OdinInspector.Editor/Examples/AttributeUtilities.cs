#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AttributeUtilities.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AttributeExampleInfo.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    public static class AttributeUtilities
    {
        private static readonly Type[] AttributeTypes;
        private static readonly Dictionary<Type, string[]> AttributeCategoryMap;

        static AttributeUtilities()
        {
            AttributeCategoryMap = AssemblyUtilities.GetAllAssemblies()
                .SelectMany(a => a.GetCustomAttributes(typeof(RegisterAttributeAttribute), true))
                .Cast<RegisterAttributeAttribute>()
                .ToDictionary(x => x.AttributeType, x => x.Categories);

            AttributeTypes = AttributeCategoryMap.Keys.ToArray();
        }

        public static IEnumerable<Type> FindAllAttributes()
        {
            return AttributeTypes;
        }

        public static Texture GetAttributeIcon(Type attributeType)
        {
            return EditorIcons.ArrowRight.Active;
        }

        public static IEnumerable<string> GetAttributeCategories(Type attributeType)
        {
            string[] categories;
            if (AttributeCategoryMap.TryGetValue(attributeType, out categories))
            {
                return categories;
            }
            else
            {
                return new string[] { "Uncategorized" };
            }
        }

        public static string GetAttributeDescription(Type attributeType)
        {
            return "Replace this: " + attributeType.GetNiceName().SplitPascalCase();
        }

        public static AttributeExampleInfo[] GetAttributeExamples(Type attributeType)
        {
            if (attributeType == null)  
            {
                throw new ArgumentNullException("attributeType");
            }

            AttributeExampleInfo[] examples;
            if (InternalAttributeExampleInfoMap.Map.TryGetValue(attributeType, out examples) == false)
            {
                examples = new AttributeExampleInfo[0];
            }

            return examples;
        }

        public static string GetOnlineDocumentationUrl(Type attributeType)
        {
            return null;
        }
    }
}
#endif