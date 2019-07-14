using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SubColliderHelper))]
    public class SubColliderHelperEditor : Editor
    {
        bool IsPrefab = false;
        bool bAutoReParent = false;

        TmpTexture2D tmpTexture2D = new TmpTexture2D();

        private void OnEnable()
        {
            if (IsPrefab = PrefabHelper.IsPrefab(targets.Select(r => (r as SubColliderHelper).gameObject).ToArray()))
                return;
        }

        public override void OnInspectorGUI()
        {
            if (IsPrefab)
            {
                base.DrawDefaultInspector();
                return;
            }

            CustomEditorGUI.ColliderControlHelperGUI(tmpTexture2D, targets);
        }

        private void OnSceneGUI()
        {
            if (IsPrefab)
                return;
        }

    }
}
