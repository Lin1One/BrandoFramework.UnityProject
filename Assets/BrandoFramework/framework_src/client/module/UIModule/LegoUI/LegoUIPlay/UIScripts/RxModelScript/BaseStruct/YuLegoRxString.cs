namespace Client.LegoUI
{
    public class YuLegoRxString : AbsLegoRxStruct<string>
    {
        protected override bool CheckChange(string newValue)
        {
            return Value != newValue;
        }
    }
}