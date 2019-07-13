namespace Client.LegoUI
{
    public class YuLegoRxShort : AbsLegoRxStruct<short>
    {
        protected override bool CheckChange(short newValue)
        {
            return Value != newValue;
        }
    }
}