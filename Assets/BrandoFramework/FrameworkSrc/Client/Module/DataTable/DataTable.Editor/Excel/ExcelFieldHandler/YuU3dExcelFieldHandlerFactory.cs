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
    public static class YuU3dExcelFieldHandlerFactory
    {
        private static Dictionary<FieldTypeEnum, List<IYuExcelFieldHandler>> handlers;

        private static Dictionary<FieldTypeEnum, List<IYuExcelFieldHandler>> Handlers
            => handlers ?? (handlers = new Dictionary<FieldTypeEnum, List<IYuExcelFieldHandler>>());

        static YuU3dExcelFieldHandlerFactory()
        {
            InitAllFieldHandler();
        }

        private static void InitAllFieldHandler()
        {
            var asm = typeof(IYuExcelFieldHandler).Assembly;
            var handlerTypes = ReflectUtility.GetTypeList<IYuExcelFieldHandler>(
                false, false, asm);
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IYuExcelFieldHandler) Activator.CreateInstance(handlerType);
                var fieldType = handler.FieldType;
                if (!Handlers.ContainsKey(fieldType))
                {
                    Handlers.Add(fieldType, new List<IYuExcelFieldHandler>());
                }

                var targetHandlers = Handlers[fieldType];
                targetHandlers.Add(handler);
            }
        }

        public static IYuExcelFieldHandler GetFieldHandler(FieldTypeEnum fieldType,
            ScriptType scriptType)
        {
            if (!Handlers.ContainsKey(fieldType))
            {
                return null;
            }

            var targetHandlers = Handlers[fieldType];
            var handler = targetHandlers.Find(h => h.ScriptType == scriptType);
            return handler;
        }
    }
}