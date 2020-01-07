using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class NewBloomModel : PostProcessingModel
    {
        public enum BloomScreenBlendMode
        {
            Screen = 0,
            Add = 1,
        }

        public enum HDRBloomMode
        {
            Auto = 0,
            On = 1,
            Off = 2,
        }

        [Serializable]
        public struct Settings
        {
            public HDRBloomMode hdr;
            public BloomScreenBlendMode screenBlendMode;
            public float bloomIntensity;
            public float bloomThreshhold;
            public int bloomBlurIterations;
            public float sepBlurSpread;
            public float useSrcAlphaAsMask;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        hdr = HDRBloomMode.Auto,
                        screenBlendMode = BloomScreenBlendMode.Add,
                        bloomIntensity = 1,
                        bloomThreshhold = 0.2f,
                        bloomBlurIterations = 3,
                        sepBlurSpread = 1.58f,
                        useSrcAlphaAsMask = 0.024f,
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
