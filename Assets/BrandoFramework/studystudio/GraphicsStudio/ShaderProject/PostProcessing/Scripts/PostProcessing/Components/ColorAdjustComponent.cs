/************************************************************
//     文件名      : ColorAdjustComponent.cs
//     功能描述    : 用于实现一些常用的屏幕空间颜色需求：如死亡置灰
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019-9-5
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine.Rendering;

namespace GraphicsStudio.PostProcessing
{
    using System;
    using UnityEngine;
    using DebugMode = BuiltinDebugViewsModel.Mode;

    public sealed class ColorAdjustComponent : PostProcessingComponentRenderTexture<ColorAdjustModel>
    {
        private int m_grayColorID = -1;
        private int m_grayItensityID = -1;

        public override bool active
        {
            get
            {
                return model.enabled && !context.interrupted;
            }
        }

        public override void Prepare(Material uberMaterial)
        {
            if (uberMaterial == null)
            {
                return;
            }
            
            if (m_grayColorID == -1)
            {
                m_grayColorID = Shader.PropertyToID("_grayClor");
            }

            if (m_grayItensityID == -1)
            {
                m_grayItensityID = Shader.PropertyToID("_grayItensity");
            }

            if (model.settings.setGray)
            {
                uberMaterial.EnableKeyword("SCREEN_SPACE_GRAY");
                uberMaterial.SetColor(m_grayColorID, model.settings.grayColor);
                uberMaterial.SetFloat(m_grayItensityID, model.settings.grayItensity);
            }
        }
    }
}
