#region Head

// Author:            LinYuzhou
// CreateDate:        2019/10/19 01:42:04
// Email:             836045613@qq.com

/*
 * 修改日期  ：
 * 修改人    ：
 * 修改内容  ：
*/

#endregion

using UnityEditor;
using UnityEngine;

namespace GameWorld.Editor
{
    [DrawGizmo(GizmoType.Active,typeof(SceneRoot))]
    public class SceneCellGizmosEditor 
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable, typeof(SceneRoot))]
        public static void OnDrawSceneGizmos(SceneRoot test, GizmoType gizmoType)
        {
            Gizmos.color = Color.blue;
            GameObject selectObj = Selection.activeGameObject;
            var sceneInfo = SceneExportEditor.currentSceneInfo;
            if(sceneInfo == null)
            {
                return;
            }
            for (var xIndex = 0; xIndex <= sceneInfo.xCellCount;xIndex++ )
            {
                Vector3 horizontalButton = new Vector3(sceneInfo.xMin + xIndex * sceneInfo.xCellSize, 5, sceneInfo.zMin);
                Vector3 horizontalTop = new Vector3(sceneInfo.xMin + xIndex * sceneInfo.xCellSize, 5, sceneInfo.zMin + sceneInfo.zLenth);
                Gizmos.DrawLine(horizontalButton, horizontalTop);
            }
            for (var zIndex = 0; zIndex <= sceneInfo.zCellCount; zIndex++)
            {
                Vector3 horizontalButton = new Vector3(sceneInfo.xMin, 5, sceneInfo.zMin + zIndex * sceneInfo.zCellSize);
                Vector3 horizontalTop = new Vector3(sceneInfo.xMin + sceneInfo.xLenth, 5, sceneInfo.zMin + +zIndex * sceneInfo.zCellSize);
                Gizmos.DrawLine(horizontalButton, horizontalTop);
            }
        }
    }
}

