using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Util
{
    using Isometric;

    [ExecuteInEditMode]
    public class GameObjectSelector : MethodBTN_MonoBehaviour
    { 
        [SerializeField]
        Color colliderColor = new Color32(232, 129, 255, 74);

        [SerializeField]
        Color gizmoColor = new Color32(128, 198, 212, 162);

        [SerializeField]
        LayerMask layerMask = -1;

        [SerializeField]
        List<Collider> colliders = new List<Collider>();

        List<Bounds> boundsForGizmo = new List<Bounds>();

        [SerializeField]
        List<GameObject> gameObjects = new List<GameObject>();
        public List<GameObject> Selected { get { return gameObjects; } }

        [SerializeField]
        List<System.Type> ComponentsForSelection = new List<System.Type>() {
            { typeof(IsoTile)},
        };

        void updateCollider()
        {
            colliders.Clear();
            colliders.AddRange(GetComponentsInChildren<Collider>());
        }

        void check()
        {
            gameObjects.Clear();
            boundsForGizmo.Clear();
            colliders.RemoveAll(r => r == null);

            foreach (var one in colliders)
            {
                Collider[] results = OverlapCollider(one);
                foreach (var result in results)
                {
                    var go = result.gameObject;
                    Component com = null;
                    if (ComponentsForSelection.Any(r => ((com = go.GetComponentInParent(r)) != null)))
                    {
                        if (!gameObjects.Contains(com.gameObject))
                        {
                            gameObjects.Add(com.gameObject);
                            boundsForGizmo.Add(result.bounds);
                        }
                    }
                }
            }
        }

        private Collider[] OverlapCollider(Collider collider)
        {
            if (collider is BoxCollider)
            {
                var box = collider as BoxCollider;
                return Physics.OverlapBox(box.transform.position + box.center, box.size * 0.5f, Quaternion.identity, layerMask);
            }
            else if (collider is SphereCollider)
            {
                var sphere = collider as SphereCollider;
                return Physics.OverlapSphere(sphere.transform.position + sphere.center, sphere.radius, layerMask);
            }
            Debug.LogError("Only BoxCollider, SphereCollider are supported.");
            return null;
        }

        private void DrawGizmoCollider(Collider collider)
        {
            if (collider is BoxCollider)
            {
                var box = collider as BoxCollider;
                Gizmos.DrawCube(box.transform.position + box.center, box.size);
            }
            else if (collider is SphereCollider)
            {
                var sphere = collider as SphereCollider;
                Gizmos.DrawSphere(sphere.transform.position + sphere.center, sphere.radius);
            }
            else
                Debug.LogError("Only BoxCollider, SphereCollider are supported.");
        }

        private void Awake()
        {
            updateCollider();

            foreach (var one in colliders)
                one.isTrigger = true;
        }

        private void OnDrawGizmosSelected()
        {
            check();

            Gizmos.color = gizmoColor;
            foreach (var one in boundsForGizmo)
            {
                Gizmos.DrawWireCube(one.center, one.size);
            }

            Gizmos.color = colliderColor;
            foreach (var one in colliders)
            {
                DrawGizmoCollider(one);                
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }

#if UNITY_EDITOR
        [MethodBTN(false)]
        public void SelectGameObjects()
        {
            UnityEditor.Selection.objects = gameObjects.ToArray();
        }

        [MethodBTN(false)]
        public void UpdateCollider()
        {
            updateCollider();
            check();
        }

        [MethodBTN(false)]
        public BoxCollider Add_BoxColliderChild()
        {
            var go = new GameObject("Box Collider");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            var result = go.AddComponent<BoxCollider>();
            result.isTrigger = true;
            UpdateCollider();
            return result;
        }

        [MethodBTN(false)]
        public SphereCollider Add_SphereColliderChild()
        {
            var go = new GameObject("Sphere Collider");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            var result = go.AddComponent<SphereCollider>();
            result.isTrigger = true;
            UpdateCollider();
            return result;
        }
#endif
    }
}