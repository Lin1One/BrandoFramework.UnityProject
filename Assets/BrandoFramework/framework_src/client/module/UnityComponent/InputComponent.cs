
using client_module_event;
using System;
using System.Collections.Generic;
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