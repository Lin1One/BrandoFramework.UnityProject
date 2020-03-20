#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Client.ScriptCreate;
using Common.Utility;
using System;
using System.Collections.Generic;

namespace Client.DataTable.Editor
{
    public static class YuU3dExcelScriptCreatorFactory
    {
        private static Dictionary<ScriptType, List<IExcelScriptCreator>> creators;

        private static Dictionary<ScriptType, List<IExcelScriptCreator>> Creators
        {
            get
            {
                if (creators != null)
                {
                    return creators;
                }

                creators = new Dictionary<ScriptType, List<IExcelScriptCreator>>();
                var asm = typeof(IYuExcelFieldHandler).Assembly;
                var creatorTypes = ReflectUtility.GetTypeList<IExcelScriptCreator>(
                    false, false, asm);
                foreach (var creatorType in creatorTypes)
                {
                    var creator = (IExcelScriptCreator) Activator.CreateInstance(creatorType);
                    if (!creators.ContainsKey(creator.ScriptType))
                    {
                        creators.Add(creator.ScriptType, new List<IExcelScriptCreator>());
                    }

                    var targetCreators = creators[creator.ScriptType];
                    targetCreators.Add(creator);
                }

                return creators;
            }
        }

        public static List<IExcelScriptCreator> GetScriptCreators(ScriptType scriptType)
        {
            if (!Creators.ContainsKey(scriptType))
            {
                return null;
            }

            var targetCreators = Creators[scriptType];
            return targetCreators;
        }
    }
}