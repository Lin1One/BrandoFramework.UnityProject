using UnityEngine;
using UnityEditor;

namespace Anonym.Util
{
    public class EditorGUIIndentLevelScope : GUI.Scope
    {
        private readonly int _iLvBackup;
        public EditorGUIIndentLevelScope()
        {
            _iLvBackup = EditorGUI.indentLevel;
        }


        protected override void CloseScope()
        {
            EditorGUI.indentLevel = _iLvBackup;
        }
    }
}