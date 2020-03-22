using Common;
using Common.Setting;
using Client.Utility;
using Common.Utility;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client.Core.Editor
{
    public static class MenuItemBuilderFactory
    {
        private static List<IMenuEditorWindowItemBuilder> itemBuilders;

        private static List<IMenuEditorWindowItemBuilder> ItemBuilders
        {
            get
            {
                if (itemBuilders != null)
                {
                    return itemBuilders;
                }

                itemBuilders = new List<IMenuEditorWindowItemBuilder>();
                foreach (var asmId in YuSetting.Instance.AllAssemblyId)
                {
                    var asm = UnityIOUtility.GetUnityAssembly(asmId);
                    if (asm == null)
                    {
                        continue;
                    }

                    var builderTypes = ReflectUtility.GetTypeList<IMenuEditorWindowItemBuilder>(
                         false, false, asm);
                    foreach (var type in builderTypes)
                    {
                        var builder = (IMenuEditorWindowItemBuilder)Activator.CreateInstance(type);
                        itemBuilders.Add(builder);
                    }
                }

                return itemBuilders;
            }
        }

        private class YuMenuItemDescCache
        {
            public Type WindowType { get; private set; }
            public string BuilderId { get; private set; }
            public MenuItemBuilderDescAttribute DescAttribute { get; private set; }

            public YuMenuItemDescCache(Type windowType,
                string builderId, MenuItemBuilderDescAttribute descAttribute)
            {
                WindowType = windowType;
                BuilderId = builderId;
                DescAttribute = descAttribute;
            }
        }

        private static Dictionary<IMenuEditorWindowItemBuilder, YuMenuItemDescCache> builderDescs;

        private static Dictionary<IMenuEditorWindowItemBuilder, YuMenuItemDescCache> BuilderDict
        {
            get
            {
                if (builderDescs != null)
                {
                    return builderDescs;
                }

                builderDescs = new Dictionary<IMenuEditorWindowItemBuilder, YuMenuItemDescCache>();
                foreach (var builder in ItemBuilders)
                {
                    var descAttr = builder.GetType().GetAttribute<MenuItemBuilderDescAttribute>();
                    if (descAttr == null)
                    {
                        throw new Exception($"菜单项构建器{builder.GetType().Name}没有附加描述特性！");
                    }

                    builderDescs.Add(builder, CreateDescCache(descAttr.WindowType, builder.GetType().Name,
                        descAttr));
                }

                return builderDescs;
            }
        }

        private static YuMenuItemDescCache CreateDescCache(Type windowType,
            string builderId, MenuItemBuilderDescAttribute descAttribute)
        {
            var cache = new YuMenuItemDescCache(windowType, builderId, descAttribute);
            return cache;
        }

        public static MenuItemBuilderDescAttribute GetItemDescCache(
            IMenuEditorWindowItemBuilder builder)
        {
            if (BuilderDict.ContainsKey(builder))
            {
                return BuilderDict[builder].DescAttribute;
            }

            return null;
        }

        public static List<IMenuEditorWindowItemBuilder> GetBuilders(Type windowType)
        {
            var builders = new List<IMenuEditorWindowItemBuilder>();

            foreach (var kv in BuilderDict)
            {
                if (kv.Value.WindowType != windowType)
                {
                    continue;
                }

                builders.Add(kv.Key);
            }

            var orderedBuilders = OrderBuilderAtPrefs(builders);
            return orderedBuilders;
        }

        private static List<IMenuEditorWindowItemBuilder> OrderBuilderAtPrefs(
            List<IMenuEditorWindowItemBuilder> builders)
        {
            var priorityResults = new List<YuMenuItemBuilderPriorityResult>();

            foreach (var builder in builders)
            {
                var typeId = builder.GetType().Name;
                var priority = PlayerPrefs.GetInt(typeId, 0);
                var newResult = new YuMenuItemBuilderPriorityResult(builder, priority);
                priorityResults.Add(newResult);
            }

            var buildResults = priorityResults.OrderBy(r => r.Priority)
                .Select(r => r.Builder).ToList();
            return buildResults;
        }

        private struct YuMenuItemBuilderPriorityResult
        {
            public IMenuEditorWindowItemBuilder Builder { get; private set; }
            public int Priority { get; private set; }

            public YuMenuItemBuilderPriorityResult(IMenuEditorWindowItemBuilder builder, int priority)
            {
                Builder = builder;
                Priority = priority;
            }
        }
    }

    
}



