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
    public static class ExcelFieldHandlerFactory
    {
        private static Dictionary<FieldTypeEnum, List<IExcelFieldHandler>> handlers;

        private static Dictionary<FieldTypeEnum, List<IExcelFieldHandler>> Handlers
            => handlers ?? (handlers = new Dictionary<FieldTypeEnum, List<IExcelFieldHandler>>());

        static ExcelFieldHandlerFactory()
        {
            InitAllFieldHandler();
        }

        private static void InitAllFieldHandler()
        {
            var asm = typeof(IExcelFieldHandler).Assembly;
            var handlerTypes = ReflectUtility.GetTypeList<IExcelFieldHandler>(
                false, false, asm);
            foreach (var handlerType in handlerTypes)
            {
                var handler = (IExcelFieldHandler) Activator.CreateInstance(handlerType);
                var fieldType = handler.FieldType;
                if (!Handlers.ContainsKey(fieldType))
                {
                    Handlers.Add(fieldType, new List<IExcelFieldHandler>());
                }

                var targetHandlers = Handlers[fieldType];
                targetHandlers.Add(handler);
            }
        }

        public static IExcelFieldHandler GetFieldHandler(FieldTypeEnum fieldType,
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