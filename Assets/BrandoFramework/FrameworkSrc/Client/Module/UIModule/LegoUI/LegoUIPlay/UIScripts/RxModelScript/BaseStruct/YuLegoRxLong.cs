namespace Client.LegoUI
{
    public class YuLegoRxLong : AbsLegoRxStruct<long>
    {
        protected override bool CheckChange(long newValue)
        {
            return Value != newValue;
        }
    }
}