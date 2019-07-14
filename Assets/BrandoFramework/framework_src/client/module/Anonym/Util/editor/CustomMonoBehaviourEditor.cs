using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace Anonym.Util
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MethodBTN_MonoBehaviour), true)]
    public class MethodBTN_MonoBehaviour_Editor : Editor
    {
        public bool bDrawDefaultInspector = true;
        public override void OnInspectorGUI()
        {
            if (bDrawDefaultInspector)
                DrawDefaultInspector();

            MonoBehaviour mb = target as MonoBehaviour;
            if (PrefabHelper.IsPrefab(mb.gameObject))
                return;

            var type = target.GetType();

            bool bPrintLabel = false;
            foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] parameters = method.GetParameters();
                var attributes = method.GetCustomAttributes(typeof(MethodBTN_MonoBehaviour.MethodBTNAttribute), true);
                if (attributes.Length > 0)
                {
                    if (!bPrintLabel)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("[Method Button]", EditorStyles.boldLabel);
                        bPrintLabel = true;
                    }
                    MethodBTN_MonoBehaviour.MethodBTNAttribute attribute = ((MethodBTN_MonoBehaviour.MethodBTNAttribute)attributes[0]);
                    bool bEnable = !attribute.bPlayModeOnly || Application.isPlaying;
                    using (new EditorGUI.DisabledGroupScope(!bEnable))
                    {
                        if (GUILayout.Button(string.Format("{0} {1}()", bEnable ? "" : "[allowed during Playing]", method.Name)))
                        {
                            Debug.Log(string.Format("MethodBTNAttribute Launch Detected! : {0} {1}", target, method.Name));
                            method.Invoke(target, parameters.Length == 0 ? null : attribute.args);
                        } 
                    }
                }
            }
        }
    }
}