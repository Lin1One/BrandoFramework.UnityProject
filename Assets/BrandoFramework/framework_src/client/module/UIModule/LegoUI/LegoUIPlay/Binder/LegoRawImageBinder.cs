#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com

#endregion


using Common;

namespace Client.LegoUI
{
    public class LegoRawImageBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var rawImage = control as YuLegoRawImage;
            var rxModel = model as YuLegoRawImageRxModel;
            rxModel.LocControl = rawImage;

            rxModel.TextureId.BindingControl(rawImage, rawImage.ReceiveTextureChange);

            rawImage.gameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            var rxModel = model.As<YuLegoRawImageRxModel>();
            rxModel.TextureId.UnBinding();
        }
    }
}