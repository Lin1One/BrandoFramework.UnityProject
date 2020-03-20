#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using UnityEngine;

namespace Client.Scene
{
    public class SceneModuleBase : ISceneModule
    {
        public void InitModule()
        {
        }

        public Transform SceneRoot => throw new System.NotImplementedException();

        public Transform CameraRoot => throw new System.NotImplementedException();

        public Transform UnitRoot => throw new System.NotImplementedException();

        public virtual void AsyncLoadScene()
        {
            throw new System.NotImplementedException();
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public virtual void KeepSceneNotDestroy()
        {
            throw new System.NotImplementedException();
        }

        public virtual void LoadScene()
        {
            throw new System.NotImplementedException();
        }

        public virtual void RemoveScene()
        {
            throw new System.NotImplementedException();
        }
    }
}

