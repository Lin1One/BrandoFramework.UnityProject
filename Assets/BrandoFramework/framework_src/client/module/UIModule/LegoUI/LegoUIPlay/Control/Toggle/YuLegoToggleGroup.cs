#region Head

// Author:            LinYuzhou
// CreateDate:        2019/7/12 19:38:48
// Email:             836045613@qq.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client.LegoUI
{
    /// <summary>
    /// 乐高视图开关组控件。
    /// </summary>
    [AddComponentMenu("Yu/LegoUI/YuLego Toggle Group", 32)]
    [DisallowMultipleComponent]
    public class YuLegoToggleGroup : UIBehaviour
    {
        [SerializeField] private bool m_AllowSwitchOff = false;

        public bool allowSwitchOff
        {
            get { return m_AllowSwitchOff; }
            set { m_AllowSwitchOff = value; }
        }

        private List<YuLegoToggle> m_Toggles = new List<YuLegoToggle>();

        protected YuLegoToggleGroup()
        {
        }

        private void ValidateToggleIsInGroup(YuLegoToggle toggle)
        {
            if (toggle == null || !m_Toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}",
                    new object[] {toggle, this}));
        }

        public void NotifyToggleOn(YuLegoToggle toggle)
        {
            ValidateToggleIsInGroup(toggle);

            // disable all toggles in the group
            for (var i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i] == toggle)
                    continue;

                m_Toggles[i].isOn = false;
            }
        }

        public void UnregisterToggle(YuLegoToggle toggle)
        {
            if (m_Toggles.Contains(toggle))
                m_Toggles.Remove(toggle);
        }

        public void RegisterToggle(YuLegoToggle toggle)
        {
            if (!m_Toggles.Contains(toggle))
                m_Toggles.Add(toggle);
        }

        public bool AnyTogglesOn()
        {
            return m_Toggles.Find(x => x.isOn) != null;
        }

        public IEnumerable<YuLegoToggle> ActiveToggles()
        {
            return m_Toggles.Where(x => x.isOn);
        }

        public void SetAllTogglesOff()
        {
            bool oldAllowSwitchOff = m_AllowSwitchOff;
            m_AllowSwitchOff = true;

            for (var i = 0; i < m_Toggles.Count; i++)
                m_Toggles[i].isOn = false;

            m_AllowSwitchOff = oldAllowSwitchOff;
        }
    }
}