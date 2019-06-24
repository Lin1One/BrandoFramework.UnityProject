#region Head

// Author:            liuruoyu1981
// CreateDate:        2019/1/26 8:35:53
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using System;

namespace YuU3dPlay
{
    public interface IYuU3dAppId
    {
        string LocAppId { get; }
    }

    [Serializable]
    public abstract class YuAbsU3dAppDati<TActual, TImpl> : YuAbsU3dGenericMultiDati<TActual, TImpl>, IYuU3dAppId
        where TActual : class, IYuU3dAppId, new()
        where TImpl : class
    {
        protected override bool CheckIsNotAppDati() => false;

        public string LocAppId => ActualSerializableObject.LocAppId;
    }
}

