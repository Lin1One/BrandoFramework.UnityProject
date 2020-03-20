namespace Client.LegoUI
{
    public class YuLegoRxByte : AbsLegoRxStruct<byte>
    {
        protected override bool CheckChange(byte newValue)
        {
            return Value != newValue;
        }
    }
}