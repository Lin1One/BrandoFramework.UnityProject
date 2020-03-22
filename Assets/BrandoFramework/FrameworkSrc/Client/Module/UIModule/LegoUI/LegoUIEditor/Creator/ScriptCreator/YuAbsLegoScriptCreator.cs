#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/13 14:44:21
// Email:             836045613@qq.com

#endregion

using Common;

namespace Client.LegoUI.Editor
{
    public abstract class YuAbsLegoScriptCreator
    {
        protected readonly StringAppender Appender
            = new StringAppender();

        protected LegoUIMeta UiMeta { get; private set; }
        //protected YuU3dAppSetting U3DAppSetting { get; }

        protected YuAbsLegoScriptCreator(LegoUIMeta uiMeta)
        {
            UiMeta = uiMeta;
        }
    }
}