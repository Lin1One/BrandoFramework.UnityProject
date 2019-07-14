using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Anonym.Util
{
    using Isometric;

    public class TouchManager : Singleton<TouchManager>
    {
        [SerializeField]
        TouchUtility.EventType eventType = TouchUtility.EventType.Mouse;

        [SerializeField, Range(0.001f, 0.1f)]
        float fScreenDragSensitivity = 0.01f;

        [SerializeField]
        Camera cam;
        public Camera Cam { get { return cam == null ? Camera.main : cam; } }

        [SerializeField]
        UnityEngine.UI.Toggle toggle;

        [SerializeField]
        UnityEngine.UI.Toggle toggle2;

        [SerializeField]
        bool bAutoStack = true;

        [SerializeField]
        bool bAttachmentOnly = false;

        [SerializeField]
        bool bAutoDrop = true;

        [SerializeField, Tooltip("Only tiles with Y coordinate higher than BaseFloor can be dragged.")]
        int iBaseFloor = 0;

        [SerializeField]
        Gradient selectedGradient = new Gradient();
        [SerializeField]
        Gradient ghostGradient = new Gradient();
        [SerializeField]
        Gradient destinationGradient = new Gradient();

        List<IsoTile> exceptionList = new List<IsoTile>();
        bool bOnScreenDrag = false;

        float fTileHeight = 1f;
        IsoTile fromTile = null;
        IsoTile toTile = null;
        IsoTile ghostTileInstance = null;

        Vector3 lastMousePos = Vector3.zero;

        IsoTile FindTopTile(IsoTile tile)
        {
            if (tile == null || !bAutoStack)
                return tile;

            var result = tile.Bulk.GetTiles_At(tile.coordinates._xyz, Vector3.up, true, true);
            result.RemoveAll(r => exceptionList.Contains(r));
            return result.Count == 0 ? tile : result.Last();
        }

        #region FromTile
        void FromTile_Select(IsoTile tile)
        {
            if (tile != null && (bAttachmentOnly || tile.coordinates._xyz.y > iBaseFloor))// && fromTile != tile)
            {
                toTile = fromTile = tile;
                exceptionList.Add(fromTile);
                GhostTile_Create();
                ColoredObject.Start(fromTile.gameObject, selectedGradient);
                fTileHeight = fromTile.GetBounds_SideOnly().size.y;
            }

            if (fromTile != null && lastMousePos != Input.mousePosition)
            {
                ToTile_Set();
            }
        }
        void FromTile_UnSelectTile()
        {
            bool unSelect = false;
            switch (eventType)
            {
                case TouchUtility.EventType.Mouse:
                    unSelect = Input.GetMouseButtonUp(0);
                    break;
                case TouchUtility.EventType.Touch:
                    unSelect = Input.touchCount == 0;
                    break;
            }

            if (unSelect)
            {
                if (fromTile != null)
                    ColoredObject.End(fromTile.gameObject);

                if (fromTile != toTile)
                    FromTile_Move();

                if (ghostTileInstance != null)
                {
                    exceptionList.Remove(ghostTileInstance);
                    Destroy(ghostTileInstance.gameObject);
                    ghostTileInstance = null;
                }

                ToTile_Reset();
                exceptionList.Clear();
                toTile = fromTile = null;
            }
        }
        void FromTile_Move()
        {
            if (fromTile == toTile)
                return;

            if (bAttachmentOnly)
            {
                toTile.Copycat(fromTile, false, true, false, false, "", false);
                fromTile.Clear_Attachment(false);
            }
            else
            {
                if (bAutoDrop)
                    ghostTileInstance.DropToFloor(queryTriggerInteraction: QueryTriggerInteraction.Ignore);

                fromTile.coordinates.Move(ghostTileInstance.coordinates._xyz);
            }
        }
        #endregion

        #region GhostTile
        void GhostTile_Create()
        {
            ghostTileInstance = fromTile.Duplicate();
            ghostTileInstance.gameObject.isStatic = false;
            ColoredObject.Start(ghostTileInstance.gameObject, ghostGradient);

            var cols = ghostTileInstance.GetComponentsInChildren<Collider>();
            foreach (var col in cols)
                col.enabled = false;
            GhostTile_Update(fromTile.transform.position);

            exceptionList.Add(ghostTileInstance);
        }
        void GhostTile_Update()
        {
            Vector3 position;
            if (toTile != null)
                position = toTile.transform.position;
            else if (!TouchUtility.Raycast_Plane(Cam, Input.mousePosition,
                new Plane(Vector3.down, fromTile.transform.position.y), out position))
                return;

            GhostTile_Update(position);
        }
        void GhostTile_Update(Vector3 position)
        {
            GhostTile_Toggle(toTile != fromTile);

            if (toTile != null)
            {
                if (bAttachmentOnly)
                    position.y = toTile.transform.position.y;
                else if (bAutoStack)
                    position.y += fTileHeight;
                else
                    position.y = fromTile.transform.position.y;

                if (!bAutoStack)
                {
                    var cols = Physics.OverlapSphere(position, 0.1f, -1, QueryTriggerInteraction.Ignore);
                    foreach (var col in cols)
                    {
                        if (col == null || exceptionList.Contains(col.GetComponentInParent<IsoTile>()))
                            continue;

                        if (!bAttachmentOnly)
                        {
                            var isdo2D = col.GetComponentInChildren<Iso2DObject>();
                            if (isdo2D != null && isdo2D.IsSideOfTile)
                                return;
                        }
                    }
                }

                ghostTileInstance.coordinates.MoveToWorldPosition(position);
            }
        }
        void GhostTile_Toggle(bool bFlag)
        {
            ghostTileInstance.gameObject.SetActive(bFlag);
        }
        #endregion

        #region ToTile
        void ToTile_Set()
        {
            var newTarget = TouchUtility.GetTile_ScreenPos(Cam, Input.mousePosition, exceptionList);
            newTarget = FindTopTile(newTarget);

            if (newTarget != toTile)
            {
                ToTile_Reset();
                toTile = newTarget;
                if (toTile != null && toTile != fromTile)
                    ColoredObject.Start(toTile.gameObject, destinationGradient);
            }

            GhostTile_Update();
        }
        void ToTile_Reset()
        {
            if (toTile != null && toTile != fromTile)
            {
                ColoredObject.End(toTile.gameObject);
                toTile = null;
            }
        }
        #endregion

        public void AutoStackToggle(bool bFlag)
        {
            if (bAttachmentOnly)
            {
                Debug.Log("This is not available, when bAttachmentOnly is True.");
                return;
            }
            bAutoStack = bFlag;
        }

        public void AutoStackToggle2(bool bFlag)
        {
            bAttachmentOnly = bFlag;
            toggle.interactable = !bFlag;
        }

        void UpdateCameraDrag()
        {
            // Camera Drag
            if (!bOnScreenDrag)
            {
                bOnScreenDrag = Input.GetMouseButtonDown(1);
                lastMousePos = Input.mousePosition;
            }
            else if (bOnScreenDrag)
            {
                bOnScreenDrag = !Input.GetMouseButtonUp(1);

                var vDiff = lastMousePos;
                lastMousePos = Input.mousePosition;
                vDiff -= lastMousePos;

                Cam.transform.Translate(vDiff * fScreenDragSensitivity);
            }
        }
        private void Update()
        {
            IsoTile _selecedTile = TouchUtility.GetTile(eventType, Cam, exceptionList);
            _selecedTile = FindTopTile(_selecedTile);

            FromTile_Select(_selecedTile);
            FromTile_UnSelectTile();

            UpdateCameraDrag();
        }
        override protected void Awake()
        {
            if (toggle != null)
                toggle.isOn = bAutoStack;

            if (toggle2 != null)
                toggle2.isOn = bAttachmentOnly;
        }
    }
}
