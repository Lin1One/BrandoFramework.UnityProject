#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion


using UnityEngine;

namespace Client.LegoUI
{
    public abstract class AbsLegoView :
        AbsLegoUI,
        ILegoView
    {
        private float depthZ;
        
        public float DepthZ
        {
            get { return depthZ; }
            set
            {
                depthZ = value;
                var oldP = UIRect.localPosition;
                var newP = new Vector3(oldP.x, oldP.y, value);
                UIRect.localPosition = newP;
            }
        }
    }
}