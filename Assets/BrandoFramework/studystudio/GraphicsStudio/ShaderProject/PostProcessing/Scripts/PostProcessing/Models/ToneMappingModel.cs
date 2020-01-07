using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class ToneMappingModel : PostProcessingModel
    {
        public enum Method
        {
            ReinHard,
            AutoWhite
        }

        [Serializable]
        public struct Settings
        {
            public Method method;
            public float middleGray;
            public float white;
            public float adaptionSpeed;
            public int textureSize;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        method = Method.AutoWhite,
                        middleGray = 0.22f,
                        white = 0.8f,
                        adaptionSpeed = 1.5f,
                        textureSize = 256
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
