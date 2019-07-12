#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 21:16:31
// Email:             836045613@qq.com


#endregion



using Common.DataStruct;

namespace Client.LegoUI
{
    public class LegoImageBinder : ILegoBinder
    {
        public void Binding(object control, object model, LegoRectTransformMeta rectMeta)
        {
            var image = (YuLegoImage)control;
            var rxModel = (YuLegoImageRxModel)model;
            rxModel.LocControl = image;

            rxModel.SpriteId.BindingControl(image, image.ReceiveSpriteChange);

            image.GameObject.SetActive(rectMeta.IsDefaultActive);
        }

        public void UnBinding(object model)
        {
            model.As<YuLegoImageRxModel>().SpriteId.UnBinding();
        }
    }
}