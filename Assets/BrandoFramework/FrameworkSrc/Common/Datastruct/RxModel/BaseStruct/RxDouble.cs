#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/11 22:06:31
// Email:             836045613@qq.com


#endregion

using System;


namespace Common.DataStruct
{
    [Serializable]
    public class YuRxDouble : AbsRxStruct<double>
    {
        protected override bool CheckChange(double newValue)
        {
            return !Value.Equal(newValue);
        }
    }
}