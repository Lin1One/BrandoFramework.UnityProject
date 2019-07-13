using Common.DataStruct;
using YuCommon;

namespace Client.LegoUI
{
    public class YuLegoRxDouble : AbsLegoRxStruct<double>
    {
        protected override bool CheckChange(double newValue)
        {
            return !Value.Equal(newValue);
        }
    }
}