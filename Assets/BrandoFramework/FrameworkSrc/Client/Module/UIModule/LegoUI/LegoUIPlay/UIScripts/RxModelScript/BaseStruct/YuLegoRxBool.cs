namespace Client.LegoUI
{
    public class YuLegoRxBool : AbsLegoRxStruct<bool>
    {
        protected override bool CheckChange(bool newValue)
        {
            return Value != newValue;
        }
    }
}