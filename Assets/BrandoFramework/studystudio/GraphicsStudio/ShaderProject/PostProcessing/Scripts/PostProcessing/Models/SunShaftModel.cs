using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class SunShaftModel : PostProcessingModel
    {
        public enum Resolution
        {
            Low,
            Normal,
            High
        }

        public enum BlendMode
        {
            Screen,
            Add,
        }

        [Serializable]
        public struct Settings
        {
            public Resolution resolution;
            public BlendMode blendMode;
            public Vector2 sunPos;
            public int radialBlurIterations;
            public Color sunColor;
            public float sunShaftBlurRadius;
            public float sunShaftIntensity;
            public float useSkyBoxAlpha;
            public float maxRadius;
            public bool useDepthTexture;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        resolution = Resolution.Normal,
                        blendMode = BlendMode.Screen,
                        sunPos = new Vector2(0.4f, 0.6f),
                        radialBlurIterations = 2,
                        sunColor = new Color(0.471f, 0.439f, 0.431f, 1.0f),
                        sunShaftBlurRadius = 2.5f,
                        sunShaftIntensity = 0.9f,
                        useSkyBoxAlpha = 0.75f,
                        maxRadius = 0.75f,
                        useDepthTexture = true,
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
