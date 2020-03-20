using UnityEditor;

namespace GameWorld.Editor
{
    [CustomEditor(typeof(TextureCombineEntrance))]
    [CanEditMultipleObjects]
    public class TextureCombinerEditor : UnityEditor.Editor
    {

        TextureBakerEditorInternal tbe = new TextureBakerEditorInternal();

        void OnEnable()
        {
            tbe.OnEnable(serializedObject);
        }

        void OnDisable()
        {
            tbe.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            tbe.DrawGUI(serializedObject, (TextureCombineEntrance)target, typeof(MeshCombineEditorWindow));
        }

    }
}