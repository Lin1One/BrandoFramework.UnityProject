using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.U2D;
using UnityEditor;
using System.Linq;

namespace Anonym.Isometric
{
    using Util;

    public class IsoComRemover : EditorWindow
    {
        const string HelpMSG_00 = "Create a Baked object to make it more generic!";
        const string HelpMSG_01 = "Create a new object baked with the selected game object.\n" +
            "Bake objects consist of Unity native components only.\n" +
            "The unique components of the Isometric Builder are removed during the bake process.";

        GameObject origin;
        GameObject baked;
        string MeshForNavmeshRootName = "ForNavMesh";
        Transform rootForNavMesh;
        Transform RootForNavMesh { get
            {
                if (!rootForNavMesh && baked)
                {
                    rootForNavMesh = baked.transform.Find(MeshForNavmeshRootName);
                    if (!rootForNavMesh)
                    {
                        rootForNavMesh = new GameObject(MeshForNavmeshRootName).transform;
                        rootForNavMesh.parent = baked.transform;
                    }
                }
                return rootForNavMesh;
            }
        }
        bool bFoldoutComponentList = false;
        LayerMask layerMask = 1;

        static Dictionary<System.Type, bool> FilterableComponent = new Dictionary<System.Type, bool>() {
            { typeof(IsoTile), false },
            { typeof(IsoTileBulk), false },
            { typeof(IsoMap), false },

            { typeof(Grid), true },
            { typeof(IsometricSortingOrder), false },
            { typeof(GridCoordinates), false },
            { typeof(RegularCollider), false },
            { typeof(ISOBasis), false },

            { typeof(AutoNaming), false },
            { typeof(Iso2DBase), false },
            { typeof(Iso2DObject), false },
            
            { typeof(IsoLight), true },
            { typeof(IsoLightReciver), true },

            { typeof(BoxCollider), true },
            { typeof(SpriteRenderer), true },

            { typeof(MeshForNavmesh), true },
            { typeof(OffMeshLink), true }
        };

        [SerializeField]
        List<System.Type> FilteredComponent = new List<System.Type>() { typeof(BoxCollider), typeof(SpriteRenderer) };

        [SerializeField]
        bool bRemoveAll = false;

        bool isForNavMesh { get { return FilteredComponent.Contains(typeof(MeshForNavmesh)); } }
        bool isRemovableState { get { return FilteredComponent.Count == 0 || (FilteredComponent.Count == 1 && isForNavMesh); } }

        [MenuItem("Window/Anonym/Iso Bakery Window")]
        public static void CreateWindow()
        {            
            EditorWindow window = EditorWindow.CreateInstance<IsoComRemover>();
            window.titleContent.text = "Iso Bakery";
            window.Show();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUILayout.Label("Not available in Play mode!", EditorStyles.boldLabel);
                return;
            }

            GUILayout.Label(HelpMSG_00, EditorStyles.boldLabel);
            GUILayout.Space(4);
            EditorGUILayout.HelpBox(HelpMSG_01, MessageType.Info);
            CustomEditorGUI.DrawSeperator();
            origin = EditorGUILayout.ObjectField(new GUIContent("Source", "Root of Sources"), origin, typeof(GameObject), allowSceneObjects: true) as GameObject;
            CustomEditorGUI.DrawSeperator();

            if (bFoldoutComponentList = EditorGUILayout.Foldout(bFoldoutComponentList, 
                string.Format("The remaining components({0})", FilteredComponent.Count)))
            {
                var enumerator = FilterableComponent.GetEnumerator();
                EditorGUI.indentLevel++;
                while(enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    using (new EditorGUI.DisabledGroupScope(!current.Value))
                    {
                        bool bFiltered = FilteredComponent.Contains(current.Key);
                        EditorGUI.BeginChangeCheck();
                        bool bToggle = EditorGUILayout.ToggleLeft(current.Key.ToString(), bFiltered);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (bToggle && !bFiltered)
                                FilteredComponent.Add(current.Key);
                            else if (bFiltered)
                                FilteredComponent.Remove(current.Key);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }

            if (isForNavMesh)
            {
                EditorGUI.indentLevel += 2;
                layerMask = CustomEditorGUI.LayerMaskField("LayerMask", layerMask);
                EditorGUI.indentLevel -= 2;
            }

            if (isRemovableState)
            {
                bRemoveAll = EditorGUILayout.ToggleLeft("Remove All(except for Nav Mesh)", bRemoveAll);
            }

            using (new EditorGUI.DisabledGroupScope(!origin))
            {
                if (GUILayout.Button("Bake!", GUILayout.Height(30)))
                {
                    Bake();
                }
            }
            if (CustomEditorGUI.DestroyOBJBtn(baked, "Baked", GUILayout.Height(30)))
            {
                baked = null;
            }
        }        

        void Bake()
        {
            string remainComponentNames = "";
            rootForNavMesh = null;
            FilteredComponent.ForEach(r => remainComponentNames += r.Name + " ");

            baked = GameObject.Instantiate(origin, null);
            Undo.RegisterCreatedObjectUndo(baked, "IsoBake!");
            baked.name = string.Format("[Baked] {0} with {1}", origin.name, remainComponentNames);
            RecurciveChild(baked, removeComponents);
            RemoveAllExceptForNavMesh();            
            Selection.activeGameObject = baked;

            Debug.Log(string.Format("[Complete!] {0}", baked.name));
        }

        void RemoveAllExceptForNavMesh()
        {
            if (isRemovableState && bRemoveAll)
            {
                RootForNavMesh.SetAsFirstSibling();
                Transform _child = null;
                while(baked.transform.childCount > 1)
                {
                    _child = baked.transform.GetChild(1);
                    if (_child != null)
                        DestroyImmediate(_child.gameObject);
                }
            }
        }

        void RecurciveChild(GameObject obj, System.Action<GameObject> Do)
        {
            int childCount = obj.transform.childCount;
            for(int i = 0; i < childCount; ++i)
            {
                RecurciveChild(obj.transform.GetChild(i).gameObject, removeComponents);
            }

            Do(obj);
        }

        void removeComponents(GameObject target)
        {
            var comEnumerator = FilterableComponent.GetEnumerator();
                        
            if (isForNavMesh && CustomEditorGUI.IsMaskedLayer(layerMask, target))
            {
                var col = target.GetComponent<Collider>();
                if (col && col.enabled && !col.isTrigger)
                {
                    var meshObj = MeshForNavmesh.Bake(col);
                    meshObj.transform.parent = RootForNavMesh;
                    meshObj.isStatic = true;
                    IsoTile tile = target.GetComponentInParent<IsoTile>();
                    if (tile != null)
                        meshObj.name = tile.name;
                }
            }

            while (comEnumerator.MoveNext())
            {
                var type = comEnumerator.Current.Key;
                if ((type.IsSubclassOf(typeof(Component)) || type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsInterface)
                    && !FilteredComponent.Contains(type))
                {
                    var com = target.GetComponent(type);
                    if (com)
                        DestroyImmediate(com);
                }
            }
        }

        static void SetEnable(GameObject target, bool bEnable)
        {
            // root 이하의 모든 Iso Coms에 대해 bEnable를 적용한다.
            var comEnumerator = FilterableComponent.GetEnumerator();

            while (comEnumerator.MoveNext())
            {
                var type = comEnumerator.Current.Key;
                if ((type.IsSubclassOf(typeof(Component)) || type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsInterface))
                {
                    var hv = target.GetComponent(type) as Behaviour;
                    if (hv)
                        hv.enabled = bEnable;
                }
            }
        }

        public static void SetEnable(bool bEnable, GameObject root)
        {
            int childCount = root.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                SetEnable(bEnable, root.transform.GetChild(i).gameObject);
            }

            SetEnable(root, bEnable);
        }
    }
}