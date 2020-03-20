
using UnityEditor;

namespace GameWorld.Editor
{
    [CustomEditor(typeof(MeshCombinerSettings))]
    public class MeshCombineSettingsAssetEditor : UnityEditor.Editor
    {
        private SerializedObject settingsSerializedObj;
        private SerializedProperty mbSettings;
        private MeshCombineSettingEditor meshBakerSettingsEditor;

        public void OnEnable()
        {
            settingsSerializedObj = new SerializedObject(target);
            mbSettings = settingsSerializedObj.FindProperty("data");
            meshBakerSettingsEditor = new MeshCombineSettingEditor();
            meshBakerSettingsEditor.OnEnable(mbSettings);
        }

        public override void OnInspectorGUI()
        {
            MeshCombinerSettings tbg = (MeshCombinerSettings)target;
            settingsSerializedObj.Update();
            EditorGUILayout.HelpBox("合并设置配置可由多个MeshCombiner共享", MessageType.Info,true);
            meshBakerSettingsEditor.DrawGUI(tbg.data, true);
            settingsSerializedObj.ApplyModifiedProperties();
        }
    }
}
