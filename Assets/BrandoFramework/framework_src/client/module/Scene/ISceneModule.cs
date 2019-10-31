#region Head

// Author:            LinYuzhou
// Email:             836045613@qq.com

#endregion

using Common;
using UnityEngine;

namespace Client.Scene
{
    public interface ISceneModule : IModule
    {
        void LoadScene();

        void AsyncLoadScene();

        void RemoveScene();

        void KeepSceneNotDestroy();

        Transform SceneRoot { get; }

        Transform CameraRoot { get; }

        Transform UnitRoot { get; }
    }
}

