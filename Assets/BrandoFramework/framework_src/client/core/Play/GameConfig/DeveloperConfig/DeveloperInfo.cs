#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using Sirenix.OdinInspector;
using System;

namespace Client.Core
{
    [YuDatiDetailHelpDesc("��������Ϣ")]
    [DatiInEditor]
    public class DeveloperInfoDati: GenericSingleDati<DeveloperInfo, DeveloperInfoDati>
    {

    }

    [Serializable]
    public class DeveloperInfo
    {
        [Title("��������Ϣ����",titleAlignment:TitleAlignments.Centered)]
        [LabelWidth(120)]
        [LabelText("����������")]
        public string DeveloperName;

        [LabelWidth(120)]
        [LabelText("����������")]
        public string DeveloperEmail;

        public string CurrentTime => DateTime.Now.ToString();
    }
}

