#region Head

// Author:            LinYuzhou
// CreateDate:        2/3/2019 10:07:15 AM
// Email:             836045613@qq.com

#endregion



namespace Client.Core.Editor
{
    public class OdinIEnteredEditModeHandler //: IYuEnteredEditModeHandler
    {
        public void HandleStateChange()
        {
            //EditorAPIInvoker.OpenFeatureHubItemAtId = YuFeatureHub.OpenItem;
            //EditorAPIInvoker.OpenFeatureHubItemAtIndex = YuFeatureHub.OpenItem;
        }

        public string LogicDesc => "功能中心编辑器API已注入运行时！";
    }
}