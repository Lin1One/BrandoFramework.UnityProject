/************************************************************
//     文件名      : DummyMotionBlurPostprocess.cs
//     功能描述    : 保留之前版本的 MotionBlurPostprocess.cs
//     负责人      : xufang
//     参考文档    : 无
//     创建日期    : 2019/06/20.
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/
using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public enum RadialBlurType
    {
        enhance,//增强径向模糊直到最大值
        reduce, //减少径向模糊直到为零
        vary,    //径向模糊到最大值后迅速回复
        all     //没有用这个实现，在这里不用管
    }

    [Serializable]
    public class DummyMotionBlurModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            public float m_Strength;
            public float m_DurationTime;
            public float m_toZeroTime;
            public float m_MaxBlurDist;
            public bool m_ExtraBlur;

            public static Settings DefaultSettings
            {
                get
                {
                    return new Settings
                    {
                        m_MaxBlurDist = 1.0f,
                        m_DurationTime = 0.0f,
                        m_Strength = 6.0f,
                        m_ExtraBlur = false,
                    };
                }
            }
        }

        [SerializeField]
        public Settings m_Settings = Settings.DefaultSettings;

        public override void Reset()
        {
            m_Settings = Settings.DefaultSettings;
        }
    }
}
