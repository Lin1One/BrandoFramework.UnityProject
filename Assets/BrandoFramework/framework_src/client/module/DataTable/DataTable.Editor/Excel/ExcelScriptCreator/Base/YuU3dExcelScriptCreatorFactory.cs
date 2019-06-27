#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/27 10:56:39
// Email:             836045613@qq.com

#endregion

using Common.ScriptCreate;
using Common.Utility;
using System;
using System.Collections.Generic;

namespace Client.DataTable.Editor
{
    public static class YuU3dExcelScriptCreatorFactory
    {
        private static Dictionary<ScriptType, List<IYuU3dExcelScriptCreator>> creators;

        private static Dictionary<ScriptType, List<IYuU3dExcelScriptCreator>> Creators
        {
            get
            {
                if (creators != null)
                {
                    return creators;
                }

                creators = new Dictionary<ScriptType, List<IYuU3dExcelScriptCreator>>();
                var asm = typeof(IYuExcelFieldHandler).Assembly;
                var creatorTypes = ReflectUtility.GetTypeList<IYuU3dExcelScriptCreator>(
                    false, false, asm);
                foreach (var creatorType in creatorTypes)
                {
                    var creator = (IYuU3dExcelScriptCreator) Activator.CreateInstance(creatorType);
                    if (!creators.ContainsKey(creator.ScriptType))
                    {
                        creators.Add(creator.ScriptType, new List<IYuU3dExcelScriptCreator>());
                    }

                    var targetCreators = creators[creator.ScriptType];
                    targetCreators.Add(creator);
                }

                return creators;
            }
        }

        public static List<IYuU3dExcelScriptCreator> GetScriptCreators(ScriptType scriptType)
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