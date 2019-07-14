using UnityEngine;

namespace Client.UI.EventSystem
{
    public class BaseEventData
    {
        private readonly BrandoEventSystem m_EventSystem;
        public BaseEventData(BrandoEventSystem eventSystem)
        {
            m_EventSystem = eventSystem;
        }

        public BaseInputModule currentInputModule
        {
            get { return m_EventSystem.currentInputModule; }
        }

        private bool m_Used;
        public bool used
        {
            get { return m_Used; }
        }

        public void Reset()
        {
            m_Used = false;
        }

        public void Use()
        {
            m_Used = true;
        }

        public GameObject selectedObject
        {
            get { return m_EventSystem.currentSelectedGameObject; }
            set { m_EventSystem.SetSelectedGameObject(value, this); }
        }
    }
}
