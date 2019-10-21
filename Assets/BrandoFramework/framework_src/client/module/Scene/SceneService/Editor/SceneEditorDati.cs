#region Head

// Author:            LinYuzhou
// CreateDate:        2019/10/21 20:44:51
// Email:             836045613@qq.com

#endregion

using Common.PrefsData;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Scene.Editor
{
    [Serializable]
    [YuDatiDesc(YuDatiSaveType.Single, typeof(SceneEditorDati),
        "应用配置及资料/场景编辑")]
    public class SceneEditorDati : GenericSingleDati<SceneEditor, SceneEditorDati>
    {
        public override void OnEnable()
        {
            SceneManager.sceneLoaded += CallBack;
        }

        public override void OnActive()
        {
            base.OnActive();
        }


        public void CallBack(UnityEngine.SceneManagement.Scene scene, LoadSceneMode sceneType)
        {
            Debug.Log(scene.name + "is load complete!");
        }

    }
}