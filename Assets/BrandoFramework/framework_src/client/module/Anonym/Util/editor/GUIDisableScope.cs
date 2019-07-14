using UnityEngine;
using UnityEditor;

public class GUIDisableScope : GUI.Scope
{
	public GUIDisableScope()
	{
		EditorGUI.BeginDisabledGroup(true);
	}
	
	protected override void CloseScope()
	{
		EditorGUI.EndDisabledGroup();
	}
}
