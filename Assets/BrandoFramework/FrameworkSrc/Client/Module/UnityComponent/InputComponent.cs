
using Client.Core.UnityComponent;
using System;
using UnityEngine;

namespace Client.Module.UnityComponent
{
    public class InputComponent : AbsSingletonMonoComponent<InputComponent>
    {
        public Action InputProcess;

        #region 帧循环事件

        private void FixedUpdate()
        {
            
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                InputProcess();
            }
        }

        private void LateUpdate()
        {
            
        }

        private void OnGUI()
        {
            
        }

        #endregion
    }
}