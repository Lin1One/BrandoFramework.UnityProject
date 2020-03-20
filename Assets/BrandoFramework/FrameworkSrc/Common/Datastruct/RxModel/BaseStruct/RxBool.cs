#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/11 22:06:31
// Email:             836045613@qq.com

#endregion

namespace Common.DataStruct
{
    [System.Serializable]
    public class RxBool : AbsRxStruct<bool>
    {
        protected override bool CheckChange(bool newValue)
        {
            return Value != newValue;
        }
    }
}

