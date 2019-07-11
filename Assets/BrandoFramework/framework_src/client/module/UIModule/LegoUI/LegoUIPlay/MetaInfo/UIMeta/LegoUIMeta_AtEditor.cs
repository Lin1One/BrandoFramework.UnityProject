using Client.Extend;
using Common.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/10 22:06:31
// Email:             836045613@qq.com

#endregion

namespace Client.LegoUI
{
    public partial class LegoUIMeta
    {
        [NonSerialized] private Dictionary<YuLegoUIType, Action<Transform>> AddMetaActions;

        private void InitAddMetActions()
        {
            AddMetaActions.Add(YuLegoUIType.Text, AddTextMeta);
            AddMetaActions.Add(YuLegoUIType.Image, AddImageMeta);
            AddMetaActions.Add(YuLegoUIType.RawImage, AddRawImageMeta);
            AddMetaActions.Add(YuLegoUIType.Button, AddButtonMeta);
            AddMetaActions.Add(YuLegoUIType.TButton, AddTButtonMeta);
            AddMetaActions.Add(YuLegoUIType.Toggle, AddToggleMeta);
            AddMetaActions.Add(YuLegoUIType.PlaneToggle, AddPlaneToggleMeta);
            AddMetaActions.Add(YuLegoUIType.InputField, AddInputFieldMeta);
            AddMetaActions.Add(YuLegoUIType.Slider, AddSliderMeta);
            AddMetaActions.Add(YuLegoUIType.Progressbar, AddProgressBarMeta);
            AddMetaActions.Add(YuLegoUIType.Dropdown, AddDropdownMeta);
            AddMetaActions.Add(YuLegoUIType.Component, AddComponentMeta);
            AddMetaActions.Add(YuLegoUIType.Container, AddContainerMeta);
            AddMetaActions.Add(YuLegoUIType.ScrollView, AddScrollViewMeta);
            AddMetaActions.Add(YuLegoUIType.Rocker, AddRockerMeta);
        }

        #region 添加元数据方法

        private void AddTextMeta(Transform transform)
        {
            var textMeta = LegoTextMeta.Create(transform.GetComponent<YuLegoText>());
            TextMetas.Add(textMeta);
        }

        private void AddImageMeta(Transform transform)
        {
            var imageMeta = LegoImageMeta.Create(transform.GetComponent<YuLegoImage>());
            ImageMetas.Add(imageMeta);
        }

        private void AddRawImageMeta(Transform transform)
        {
            var rawImageMeta = LegoRawImageMeta.Create(transform.GetComponent<YuLegoRawImage>());
            RawImageMetas.Add(rawImageMeta);
        }

        private void AddButtonMeta(Transform transform)
        {
            var buttonMeta = LegoButtonMeta.Create(transform.GetComponent<YuLegoButton>());
            ButtonMetas.Add(buttonMeta);
        }

        private void AddTButtonMeta(Transform transform)
        {
            var tButtonMeta = LegoTButtonMeta.Create(transform.GetComponent<YuLegoTButton>());
            TButtonMetas.Add(tButtonMeta);
        }

        private void AddToggleMeta(Transform transform)
        {
            var toggleMeta = LegoToggleMeta.Create(transform.GetComponent<YuLegoToggle>());
            ToggleMetas.Add(toggleMeta);
        }

        private void AddPlaneToggleMeta(Transform transform)
        {
            var planeToggleMeta = LegoPlaneToggleMeta
                .Create(transform.GetComponent<YuLegoPlaneToggle>());
            PlaneToggleMetas.Add(planeToggleMeta);
        }

        private void AddInputFieldMeta(Transform transform)
        {
            var inputFieldMeta = LegoInputFieldMeta.Create(
                transform.GetComponent<YuLegoInputField>());
            InputFieldMetas.Add(inputFieldMeta);
        }

        private void AddDropdownMeta(Transform transform)
        {
            var dropdownMeta = LegoDropdownMeta.Create(
                transform.GetComponent<YuLegoDropdown>());
            DropdownMetas.Add(dropdownMeta);
        }

        private void AddSliderMeta(Transform transform)
        {
            var sliderMeta = LegoSliderMeta.Create(transform.GetComponent<YuLegoSlider>());
            SliderMetas.Add(sliderMeta);
        }

        private void AddProgressBarMeta(Transform transform)
        {
            var progressbarMeta = LegoProgressbarMeta.Create(transform.GetComponent<YuLegoProgressbar>());
            ProgressbarMetas.Add(progressbarMeta);
        }

        private void AddComponentMeta(Transform transform)
        {
            var componentMeta = LegoComponentRef.Create(transform.RectTransform());
            ComponentRefs.Add(componentMeta);
        }

        private void AddContainerMeta(Transform transform)
        {
            var containerMeta = LegoContainerRef.Create(transform.RectTransform());
            ContainerRefs.Add(containerMeta);
        }

        private void AddScrollViewMeta(Transform transform)
        {
            var scrollViewMeta = LegoScrollViewMeta.Create(transform.GetComponent<YuLegoScrollView>());
            ScrollViewMetas.Add(scrollViewMeta);
        }

        private void AddRockerMeta(Transform transform)
        {
            var rockerMeta = LegoRockerMeta.Create(transform.GetComponent<YuLegoRocker>());
            RockerMetas.Add(rockerMeta);
        }

        public void AddOperateMeta(YuLegoUIType elementType,
            LegoRectTransformMeta rectMeta, Transform transform)
        {
            ElementTypes.Add(elementType);
            RectMetas.Add(rectMeta);
            if (!AddMetaActions.ContainsKey(elementType))
            {
                Debug.LogError($"目标UI类型{elementType}当前没有对应的元数据构建委托！");
            }

            var action = AddMetaActions[elementType];
            action(transform);
        }

        #endregion

        private List<LegoImageMeta> allImageMetas;

        public List<LegoImageMeta> AllImageMetas
        {
            get
            {
                if (allImageMetas != null)
                {
                    return allImageMetas;
                }

                allImageMetas = new List<LegoImageMeta>();
                allImageMetas.AddRange(ImageMetas);

                foreach (var buttonMeta in ButtonMetas)
                {
                    allImageMetas.Add(buttonMeta.ButtonImageMeta);
                }

                foreach (var tButtonMeta in TButtonMetas)
                {
                    allImageMetas.Add(tButtonMeta.IconImageMeta);
                    allImageMetas.Add(tButtonMeta.ButtonImageMeta);
                }

                foreach (var toggleMeta in ToggleMetas)
                {
                    allImageMetas.Add(toggleMeta.BackgroundImageMeta);
                    allImageMetas.Add(toggleMeta.CheckMarkImageMeta);
                }

                foreach (var planeToggleMeta in PlaneToggleMetas)
                {
                    allImageMetas.Add(planeToggleMeta.ImageFrontPointImageMeta);
                    allImageMetas.Add(planeToggleMeta.ToggleImageMeta);
                }

                foreach (var inputFieldMeta in InputFieldMetas)
                {
                    allImageMetas.Add(inputFieldMeta.RootImageMeta);
                }

                foreach (var progressbarMeta in ProgressbarMetas)
                {
                    allImageMetas.Add(progressbarMeta.BackgroundImageMeta);
                    allImageMetas.Add(progressbarMeta.FillImageMeta);
                }

                foreach (var sliderMeta in SliderMetas)
                {
                    allImageMetas.Add(sliderMeta.BackgroundImageMeta);
                    allImageMetas.Add(sliderMeta.FillImageMeta);
                    allImageMetas.Add(sliderMeta.HandleImageMeta);
                }

                foreach (var scrollViewMeta in ScrollViewMetas)
                {
                    allImageMetas.Add(scrollViewMeta.ScrollRectImageMeta);
                    allImageMetas.Add(scrollViewMeta.ScrollViewImageMeta);
                }

                return allImageMetas;
            }
        }

        public void Init()
        {
            RootMeta = new LegoRectTransformMeta();
            ElementTypes = new List<YuLegoUIType>();
            ComponentRefs = new List<LegoComponentRef>();
            ContainerRefs = new List<LegoContainerRef>();
            RectMetas = new List<LegoRectTransformMeta>();
            TextMetas = new List<LegoTextMeta>();
            ButtonMetas = new List<LegoButtonMeta>();
            TButtonMetas = new List<LegoTButtonMeta>();
            ImageMetas = new List<LegoImageMeta>();
            RawImageMetas = new List<LegoRawImageMeta>();
            SliderMetas = new List<LegoSliderMeta>();
            ProgressbarMetas = new List<LegoProgressbarMeta>();
            ToggleMetas = new List<LegoToggleMeta>();
            PlaneToggleMetas = new List<LegoPlaneToggleMeta>();
            InputFieldMetas = new List<LegoInputFieldMeta>();
            DropdownMetas = new List<LegoDropdownMeta>();
            ScrollViewMetas = new List<LegoScrollViewMeta>();
            RockerMetas = new List<LegoRockerMeta>();

            AddMetaActions = new Dictionary<YuLegoUIType, Action<Transform>>();
            InitAddMetActions();
        }

        [Button("保存元数据", ButtonSizes.Medium)]
        public void Save()
        {
            //var currentApp = YuU3dAppSettingDati.CurrentActual;
            //var metaDir = currentApp.Helper.AssetDatabaseLegoMetaDir;
            //var uiType = RootMeta.Name.Split('_')[0];
            //var path = metaDir + uiType + "/" + RootMeta.Name + ".txt";
            //var jsonContent = JsonUtility.ToJson(this);
            //IOUtility.WriteAllText(path, jsonContent);
        }
    }
}


#endif