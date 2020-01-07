using System;
using UnityEngine;

namespace GraphicsStudio.PostProcessing
{
    [Serializable]
    public abstract class PostProcessingModel
    {
        [SerializeField, GetSet("enabled")]
        bool m_Enabled;
        public bool enabled
        {
            get { return m_Enabled; }
            set
            {
                if (m_Enabled != value)
                {
                    m_Enabled = value;

                    if (value)
                        OnValidate();
                }
            }
        }

        public abstract void Reset();

        public virtual void OnValidate()
        {}
    }
}
