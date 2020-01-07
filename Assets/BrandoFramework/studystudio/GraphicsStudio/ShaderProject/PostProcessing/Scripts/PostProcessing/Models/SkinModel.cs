using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class SkinModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            public float sssWidth;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {              
                        sssWidth = 0.01f,
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
