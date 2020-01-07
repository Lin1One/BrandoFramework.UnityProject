/************************************************************
//     �ļ���      : DummyMotionBlurPostprocess.cs
//     ��������    : ����֮ǰ�汾�� MotionBlurPostprocess.cs
//     ������      : xufang
//     �ο��ĵ�    : ��
//     ��������    : 2019/06/20.
//     Copyright   : Copyright 2014-2017 EZFun Inc.
**************************************************************/
using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    public enum RadialBlurType
    {
        enhance,//��ǿ����ģ��ֱ�����ֵ
        reduce, //���پ���ģ��ֱ��Ϊ��
        vary,    //����ģ�������ֵ��Ѹ�ٻظ�
        all     //û�������ʵ�֣������ﲻ�ù�
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
