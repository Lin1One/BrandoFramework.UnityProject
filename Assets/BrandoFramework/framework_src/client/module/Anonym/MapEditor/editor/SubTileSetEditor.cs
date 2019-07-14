using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;
using System.Linq;
using System.Reflection;

namespace Anonym.Isometric
{
    using Anonym.Util;

    [CustomEditor(typeof(SubTileSet))]
    [CanEditMultipleObjects]
    public class SubTileSetEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This can not be modified here.\nModify this on the containing parent asset or parent component.", MessageType.Info);
        }
    }
}