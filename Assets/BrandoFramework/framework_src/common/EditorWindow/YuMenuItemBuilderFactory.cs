using Common;
using Common.Setting;
using Common.Utility;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.EditorWindow
{
    public static class YuMenuItemBuilderFactory
    {
        private static List<IYuMenuEditorWindowItemBuilder> itemBuilders;

        private static List<IYuMenuEditorWindowItemBuilder> ItemBuilders
        {
            get
            {
                if (itemBuilders != null)
                {
                    return itemBuilders;
                }

                itemBuilders = new List<IYuMenuEditorWindowItemBuilder>();
                foreach (var asmId in YuSetting.Instance.AllAssemblyId)
                {
                    var asm = YuUnityIOUtility.GetUnityAssembly(asmId);
                    if (asm == null)
                    {
                        continue;
                    }

                    var builderTypes = ReflectUtility.GetTypeList<IYuMenuEditorWindowItemBuilder>(
                         false, false, asm);
                    foreach (var type in builderTypes)
                    {
                        var builder = (IYuMenuEditorWindowItemBuilder)Activator.CreateInstance(type);
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
            public YuMenuItemBuilderDescAttribute DescAttribute { get; private set; }

            public YuMenuItemDescCache(Type windowType,
                string builderId, YuMenuItemBuilderDescAttribute descAttribute)
            {
                WindowType = windowType;
                BuilderId = builderId;
                DescAttribute = descAttribute;
            }
        }

        private static Dictionary<IYuMenuEditorWindowItemBuilder, YuMenuItemDescCache> builderDescs;

        private static Dictionary<IYuMenuEditorWindowItemBuilder, YuMenuItemDescCache> BuilderDict
        {
            get
            {
                if (builderDescs != null)
                {
                    return builderDescs;
                }

                builderDescs = new Dictionary<IYuMenuEditorWindowItemBuilder, YuMenuItemDescCache>();
                foreach (var builder in ItemBuilders)
                {
                    var descAttr = builder.GetType().GetAttribute<YuMenuItemBuilderDescAttribute>();
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
            string builderId, YuMenuItemBuilderDescAttribute descAttribute)
        {
            var cache = new YuMenuItemDescCache(windowType, builderId, descAttribute);
            return cache;
        }

        public static YuMenuItemBuilderDescAttribute GetItemDescCache(
            IYuMenuEditorWindowItemBuilder builder)
        {
            if (BuilderDict.ContainsKey(builder))
            {
                return BuilderDict[builder].DescAttribute;
            }

            return null;
        }

        public static List<IYuMenuEditorWindowItemBuilder> GetBuilders(Type windowType)
        {
            var builders = new List<IYuMenuEditorWindowItemBuilder>();

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

        private static List<IYuMenuEditorWindowItemBuilder> OrderBuilderAtPrefs(
            List<IYuMenuEditorWindowItemBuilder> builders)
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
            public IYuMenuEditorWindowItemBuilder Builder { get; private set; }
            public int Priority { get; private set; }

            public YuMenuItemBuilderPriorityResult(IYuMenuEditorWindowItemBuilder builder, int priority)
            {
                Builder = builder;
                Priority = priority;
            }
        }
    }

    
}



