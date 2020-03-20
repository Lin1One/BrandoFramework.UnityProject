#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Common;
using Sirenix.OdinInspector;
using System;

namespace Client.Core
{
    [Serializable]
    public class EventModuleConfig : BaseModuleConfig<IU3DEventModule>
    {
        [LabelText("≤‚ ‘ ID")]
        public int eventTestID;
    }
}

