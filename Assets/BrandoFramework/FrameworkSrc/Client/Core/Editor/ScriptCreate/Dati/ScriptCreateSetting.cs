
using Sirenix.OdinInspector;
using System;

namespace Client.ScriptCreate
{
    [Serializable]
    public class YuScriptCreateSetting
    {
        [LabelText("脚本Id")]
        public string ScriptId;

        #region 脚本类型及自动创建接口

        [HorizontalGroup("脚本类型", 0.5f)]
        [LabelText("脚本类型")]
        public Inherits inherits;

        [HorizontalGroup("脚本类型")]
        [ShowIf("CheckIsAotuCreateInterface")]
        [LabelText("是否自动创建对应的接口")]
        public bool IsAotuCreateInterface;

        private bool CheckIsAotuCreateInterface()
        {
            var result = inherits == Inherits.Class || inherits == Inherits.AbstractClass;
            return result;
        }

        #endregion

        [LabelText("预编译指令")]
        public string PreComplie;

        [LabelText("父类或者接口")]
        public string Inherit;

        [LabelText("命名空间")]
        public string NameSpace;
    }
}