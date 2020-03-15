namespace Client.LegoUI
{
    public class YuLegoRxInt : AbsLegoRxStruct<int>
    {
        protected override bool CheckChange(int newValue)
        {
            return Value != newValue;
        }
    }
}