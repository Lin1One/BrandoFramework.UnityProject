using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public class GlowModel : PostProcessingModel
    {
        [Serializable]
        public struct Settings
        {
            /// The brightness of the glow. Values larger than one give extra "boost".
            public float glowIntensity;

            /// Blur iterations - larger number means more blur.
            public int blurIterations;

            /// Blur spread for each iteration. Lower values
            /// give better looking blur, but require more iterations to
            /// get large blurs. Value is usually between 0.5 and 1.0.
            public float blurSpread;

            /// Tint glow with this color. Alpha adds additional glow everywhere.
            public Color glowTint;

            public static Settings defaultSettings
            {
                get
                {
                    return new Settings
                    {
                        glowIntensity = 0.1f,
                        blurIterations = 1,
                        blurSpread = 0.5f,
                        glowTint = new Color(0.272f, 0.235f, 0.218f, 0),
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
