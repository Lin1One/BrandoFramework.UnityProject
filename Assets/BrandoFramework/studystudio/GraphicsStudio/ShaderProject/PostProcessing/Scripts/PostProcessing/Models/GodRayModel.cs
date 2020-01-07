/************************************************************
//     文件名      : GodRayModel.cs
//     功能描述    : Godray后处理参数模型
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/10/22
//     Copyright   : Copyright EZFun Inc.
**************************************************************/

using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class GodRayModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            public Color threshold;
            public float intensity;
            public Color lightColor;
            public int blurIterations;
            public float blurOffset;
            public float lightRadius;
            public float lightPow;
            public float decay;
            public float exposure;
            public float depthThreshold;
            public Vector3 targetViewPos;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        threshold = new Color(0.176f, 0.176f, 0.176f),
                        lightColor = Color.white,
                        intensity = 0.48f,
                        blurOffset = 4.0f,
                        blurIterations = 2,
                        lightRadius = 3.2f,
                        lightPow = 1.2f,
                        decay = 0.6f,
                        exposure = 2.2f,
                        depthThreshold = 0.06f,
                        targetViewPos = Vector3.zero,
                    };
                }
            }
        }

        [SerializeField]
        public Settings m_Settings = Settings.defaultSettings;

        public Settings settings
        {
            get { return m_Settings; }
            set { m_Settings = value; }
        }

        public override void Reset()
        {
            m_Settings = Settings.defaultSettings;
        }
    }
}
