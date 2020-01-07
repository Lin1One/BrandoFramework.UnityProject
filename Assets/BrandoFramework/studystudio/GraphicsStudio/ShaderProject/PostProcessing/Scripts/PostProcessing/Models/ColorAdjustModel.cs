using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class ColorAdjustModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            [Tooltip("screen space set gray?")]
            public bool setGray;

            [Tooltip("gray color")]
            public Color grayColor;

            [Range(0f, 1f), Tooltip("gray itensity")]
            public float grayItensity;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        setGray = false,
                        grayColor = new Color(0.299f, 0.587f, 0.114f, 1.0f),
                        grayItensity = 0.0f,
                    };
                }
            }
        }

        [SerializeField]
        Settings m_Settings = Settings.defaultSettings;
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
