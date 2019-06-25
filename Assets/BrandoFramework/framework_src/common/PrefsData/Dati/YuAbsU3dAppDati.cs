#region Head

// Author:            LinYuzhou
// CreateDate:        2019/6/25 17:45:19
// Email:             836045613@qq.com

#endregion

using System;

namespace Common.PrefsData
{
    public interface IYuU3dAppId
    {
        string LocAppId { get; }
    }

    [Serializable]
    public abstract class YuAbsU3dAppDati<TActual, TImpl> : GenericMultiDati<TActual, TImpl>, IYuU3dAppId
        where TActual : class, IYuU3dAppId, new()
        where TImpl : class
    {
        protected override bool CheckIsNotAppDati() => false;

        public string LocAppId => ActualSerializableObject.LocAppId;
    }
}

