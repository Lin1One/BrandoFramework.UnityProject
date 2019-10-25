
using Common;

namespace Client.LegoUI
{
    public class LegoRockerBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var rocker = control as YuLegoRocker;
            var rxModel = model as YuLegoRockerRxModel;
            rxModel.LocControl = rocker;

            // 数据响应绑定
            rxModel.BgSpriteId.BindingControl(rocker.BgImage, rocker.OnBackGroundChange);
            rxModel.RockerSpriteId.BindingControl(rocker.RockerImage, rocker.OnRockerChange);

            rocker.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoRockerRxModel>();
            rxModel.BgSpriteId.UnBinding();
            rxModel.RockerSpriteId.UnBinding();
        }
    }
}
