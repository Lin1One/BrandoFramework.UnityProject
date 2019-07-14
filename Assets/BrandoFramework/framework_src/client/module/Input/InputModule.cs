#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Client.Module.UnityComponent;
using Common;

namespace Client.InputModule
{
    public class InputModule : IInputModule
    {
        private readonly InputComponent inputComponent;
        private readonly IInputProcesser inputDataProcesser;
        public InputModule()
        {
            inputComponent = YuU3dInjector.MonoInjectorInstance.GetMono<InputComponent>();
            inputDataProcesser = Injector.Instance.Get<IInputProcesser>();
            Init();
        }

        public void Init()
        {
            inputComponent.InputProcess = InputProcess;
        }

        public void InputProcess()
        {
            inputDataProcesser.Process();
        }

    }
}

