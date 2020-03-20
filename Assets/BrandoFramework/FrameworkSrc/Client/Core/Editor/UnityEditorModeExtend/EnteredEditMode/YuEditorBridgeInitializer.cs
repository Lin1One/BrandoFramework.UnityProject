#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion
using UnityEngine;

namespace Client.Core.Editor

{
    /// <summary>
    /// 编辑器API桥接构建器。
    /// </summary>
   // [YuOnLoadPriority(2)]
    public class YuEditorBridgeInitializer : IYuEnteredEditModeHandler
    {
        public void HandleStateChange()
        {
            InjectEditorMethod();
        }

        public string LogicDesc => "编辑器API及日志调试API已注入运行时！";

        private static void InjectEditorMethod()
        {
            // 注入Debug函数
            //YuDebugUtility.StaticInit(Debug.Log, Debug.LogWarning, Debug.LogError, Debug.LogException);
            //YuEditorAPIInvoker.DisplayTip = YuEditorUtility.DisplayError;
            //YuEditorAPIInvoker.BuildOrRefreshApp = YuAppStructureInitializer.BuildOrRefreshApp;
            //YuEditorAPIInvoker.PrettifyJsonString = YuEditorUtility.PrettifyJsonString;
        }
    }
}