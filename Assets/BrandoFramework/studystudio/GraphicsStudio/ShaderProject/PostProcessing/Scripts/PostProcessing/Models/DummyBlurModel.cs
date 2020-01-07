/************************************************************
//     文件名      : DummyBlurPostprocess.cs
//     功能描述    : 保留之前版本的 BlurPostprocess.cs
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/06/20.
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/
using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    [Serializable]
    public class DummyBlurModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            [Range(0, 2)]
            public int m_Downsample;

            public BlurType m_BlurType;

            [Range(0.0f, 10.0f)]
            public float m_BlurSize;

            [Range(1, 4)]
            public int m_BlurIterations;

            public float m_Process;
            public bool m_IsForward;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        m_Downsample = 2,
                        m_BlurType = BlurType.StandardGauss,
                        m_BlurSize = 1.0f,
                        m_BlurIterations = 2,
                        m_Process = 2,
                        m_IsForward = true,
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
