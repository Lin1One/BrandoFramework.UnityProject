#region Head

// Author:            Yu
// CreateDate:        2018/8/28 10:49:57
// Email:             35490136@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

namespace Common.DataStruct
{
    [System.Serializable]
    public class RxByte : AbsRxStruct<byte>
    {
        protected override bool CheckChange(byte newValue)
        {
            return Value != newValue;
        }
    }
}