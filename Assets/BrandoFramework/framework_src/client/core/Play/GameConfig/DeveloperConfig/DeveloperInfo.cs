#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using Sirenix.OdinInspector;
using System;

namespace Client.Core
{
    [YuDatiDetailHelpDesc("开发者信息")]
    [DatiInEditor]
    public class DeveloperInfoDati: GenericSingleDati<DeveloperInfo, DeveloperInfoDati>
    {

    }

    [Serializable]
    public class DeveloperInfo
    {
        [Title("开发者信息配置",titleAlignment:TitleAlignments.Centered)]
        [LabelWidth(120)]
        [LabelText("开发者姓名")]
        public string DeveloperName;

        [LabelWidth(120)]
        [LabelText("开发者邮箱")]
        public string DeveloperEmail;

        public string CurrentTime => DateTime.Now.ToString();
    }
}

