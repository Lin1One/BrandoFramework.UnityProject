﻿using Common;


namespace Client.LegoUI
{
    public class YuLegoRxFloat : AbsLegoRxStruct<float>
    {
        protected override bool CheckChange(float newValue)
        {
            return !Value.Equal(newValue);
        }
    }
}