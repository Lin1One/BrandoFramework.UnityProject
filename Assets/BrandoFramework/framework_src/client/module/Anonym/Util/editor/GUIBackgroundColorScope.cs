using UnityEngine;

namespace Anonym.Util
{
    public class GUIBackgroundColorScope : GUI.Scope
    {
        private readonly Color _colorBackup;
        public GUIBackgroundColorScope(Color color)
        {
            _colorBackup = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }


        protected override void CloseScope()
        {
            GUI.backgroundColor = _colorBackup;
        }
    }
}