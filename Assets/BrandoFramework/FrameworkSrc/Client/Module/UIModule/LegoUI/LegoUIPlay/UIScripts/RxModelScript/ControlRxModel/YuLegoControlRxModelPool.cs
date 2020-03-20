
using Common;


namespace Client.LegoUI
{
    /// <summary>
    /// 乐高UI控件数据模型对象池。
    /// </summary>
    public static class YuLegoControlRxModelPool
    {
        #region Text

        private static IGenericObjectPool<YuLegoTextRxModel> textModelPool;

        private static IGenericObjectPool<YuLegoTextRxModel> TextModelPool
        {
            get
            {
                if (textModelPool != null)
                {
                    return textModelPool;
                }

                textModelPool = new GenericObjectPool<YuLegoTextRxModel>(
                    () => new YuLegoTextRxModel(),
                    100
                );

                return textModelPool;
            }
        }

        public static YuLegoTextRxModel TakeTextModel() => TextModelPool.Take();
        public static void Restore(YuLegoTextRxModel model) => TextModelPool.Restore(model);


        #endregion

        #region Button

        private static IGenericObjectPool<YuLegoButtonRxModel> buttonModelPool;

        private static IGenericObjectPool<YuLegoButtonRxModel> ButtonModelPool
        {
            get
            {
                if (buttonModelPool != null)
                {
                    return buttonModelPool;
                }

                buttonModelPool = new GenericObjectPool<YuLegoButtonRxModel>(
                    () => new YuLegoButtonRxModel(),
                    100
                );

                return buttonModelPool;
            }
        }

        public static YuLegoButtonRxModel TakeButtonModel() => ButtonModelPool.Take();
        public static void Restore(YuLegoButtonRxModel model) => ButtonModelPool.Restore(model);

        #endregion

        #region TButton

        private static IGenericObjectPool<YuLegoTButtonRxModel> tbuttonModelPool;

        private static IGenericObjectPool<YuLegoTButtonRxModel> TbuttonModelPool
        {
            get
            {
                if (tbuttonModelPool != null)
                {
                    return tbuttonModelPool;
                }

                tbuttonModelPool = new GenericObjectPool<YuLegoTButtonRxModel>(
                    () => new YuLegoTButtonRxModel(),
                    100
                );

                return tbuttonModelPool;
            }
        }

        public static YuLegoTButtonRxModel TakeTButtonModel() => TbuttonModelPool.Take();
        public static void Restore(YuLegoTButtonRxModel model) => TbuttonModelPool.Restore(model);

        #endregion

        #region Image

        private static IGenericObjectPool<YuLegoImageRxModel> imageModelPool;

        private static IGenericObjectPool<YuLegoImageRxModel> ImageModelPool
        {
            get
            {
                if (imageModelPool != null)
                {
                    return imageModelPool;
                }

                imageModelPool = new GenericObjectPool<YuLegoImageRxModel>(
                    () => new YuLegoImageRxModel(),
                    100
                );

                return imageModelPool;
            }
        }

        public static YuLegoImageRxModel TakeImageModel() => ImageModelPool.Take();
        public static void Restore(YuLegoImageRxModel model) => ImageModelPool.Restore(model);


        #endregion

        #region RawImage

        private static IGenericObjectPool<YuLegoRawImageRxModel> rawImageModelPool;

        private static IGenericObjectPool<YuLegoRawImageRxModel> RawImageModelPool
        {
            get
            {
                if (rawImageModelPool != null)
                {
                    return rawImageModelPool;
                }

                rawImageModelPool = new GenericObjectPool<YuLegoRawImageRxModel>(
                    () => new YuLegoRawImageRxModel(),
                    100
                );

                return rawImageModelPool;
            }
        }

        public static YuLegoRawImageRxModel TakeRawImageModel() => RawImageModelPool.Take();
        public static void Restore(YuLegoRawImageRxModel model) => RawImageModelPool.Restore(model);

        #endregion

        #region Toggle

        private static IGenericObjectPool<YuLegoToggleRxModel> toggleModelPool;

        private static IGenericObjectPool<YuLegoToggleRxModel> ToggleModelPool
        {
            get
            {
                if (toggleModelPool != null)
                {
                    return toggleModelPool;
                }

                toggleModelPool = new GenericObjectPool<YuLegoToggleRxModel>(
                    () => new YuLegoToggleRxModel(),
                    100
                );

                return toggleModelPool;
            }
        }

        public static YuLegoToggleRxModel TakeToggleModel() => ToggleModelPool.Take();
        public static void Restore(YuLegoToggleRxModel model) => ToggleModelPool.Restore(model);

        #endregion

        #region PlaneToggle


        private static IGenericObjectPool<YuLegoPlaneToggleRxModel> planeToggleModelPool;

        private static IGenericObjectPool<YuLegoPlaneToggleRxModel> PlaneToggleModelPool
        {
            get
            {
                if (planeToggleModelPool != null)
                {
                    return planeToggleModelPool;
                }

                planeToggleModelPool = new GenericObjectPool<YuLegoPlaneToggleRxModel>(
                    () => new YuLegoPlaneToggleRxModel(),
                    100
                );

                return planeToggleModelPool;
            }
        }

        public static YuLegoPlaneToggleRxModel TakePlaneToggleModel() => PlaneToggleModelPool.Take();
        public static void Restore(YuLegoPlaneToggleRxModel model) => PlaneToggleModelPool.Restore(model);

        #endregion

        #region Slider

        private static IGenericObjectPool<YuLegoSliderRxModel> sliderModelPool;

        private static IGenericObjectPool<YuLegoSliderRxModel> SliderModelPool
        {
            get
            {
                if (sliderModelPool != null)
                {
                    return sliderModelPool;
                }

                sliderModelPool = new GenericObjectPool<YuLegoSliderRxModel>(
                    () => new YuLegoSliderRxModel(),
                    100
                );

                return sliderModelPool;
            }
        }

        public static YuLegoSliderRxModel TakeSliderModel() => SliderModelPool.Take();
        public static void Restore(YuLegoSliderRxModel model) => SliderModelPool.Restore(model);

        #endregion

        #region InputField

        private static IGenericObjectPool<YuLegoInputFieldRxModel> inputFieldModelPool;

        private static IGenericObjectPool<YuLegoInputFieldRxModel> InputFieldModelPool
        {
            get
            {
                if (inputFieldModelPool != null)
                {
                    return inputFieldModelPool;
                }

                inputFieldModelPool = new GenericObjectPool<YuLegoInputFieldRxModel>(
                    () => new YuLegoInputFieldRxModel(),
                    100
                );

                return inputFieldModelPool;
            }
        }

        public static YuLegoInputFieldRxModel TakeInputFieldModel() => InputFieldModelPool.Take();
        public static void Restore(YuLegoInputFieldRxModel model) => InputFieldModelPool.Restore(model);

        #endregion

        #region Progressbar

        private static IGenericObjectPool<YuLegoProgressbarRxModel> progressbarModelPool;

        private static IGenericObjectPool<YuLegoProgressbarRxModel> ProgressbarModelPool
        {
            get
            {
                if (progressbarModelPool != null)
                {
                    return progressbarModelPool;
                }

                progressbarModelPool = new GenericObjectPool<YuLegoProgressbarRxModel>(
                    () => new YuLegoProgressbarRxModel(),
                    100
                );

                return progressbarModelPool;
            }
        }

        public static YuLegoProgressbarRxModel TakeProgressbarModel() => ProgressbarModelPool.Take();
        public static void Restore(YuLegoProgressbarRxModel model) => ProgressbarModelPool.Restore(model);

        #endregion

        #region Progressbar

        private static IGenericObjectPool<YuLegoRockerRxModel> rockerModelPool;

        private static IGenericObjectPool<YuLegoRockerRxModel> RockerModelPool
        {
            get
            {
                if (rockerModelPool != null)
                {
                    return rockerModelPool;
                }

                rockerModelPool = new GenericObjectPool<YuLegoRockerRxModel>(
                    () => new YuLegoRockerRxModel(),
                    100
                );

                return rockerModelPool;
            }
        }

        public static YuLegoRockerRxModel TakeRockerModel() => RockerModelPool.Take();
        public static void Restore(YuLegoRockerRxModel model) => RockerModelPool.Restore(model);

        #endregion

    }
}
