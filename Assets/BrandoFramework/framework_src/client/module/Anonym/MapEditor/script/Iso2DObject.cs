using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
	using Util;
	[DisallowMultipleComponent]
	[System.Serializable]
	[ExecuteInEditMode]
    public class Iso2DObject : Iso2DBase
    {
        public enum Type
        {
            Overlay,
            Obstacle,
            Side_Union,
            Side_X,
            Side_Y,
            Side_Z,
            Through,
        }

        IsoTile _tile;
        public IsoTile Tile
        {
            get
            {
                if (_tile == null) 
                {
                    _tile = GetComponentInParent<IsoTile>();
                }
                if (_tile == null)
                {
                    string names = "";
                    Transform _t = transform;
                    bool isAllActived = true;
                    while (_t != null)
                    {
                        isAllActived &= _t.gameObject.activeSelf;                            
                        names = _t.name + " : " + names;
                        _t = _t.parent; 
                    }

                    if (isAllActived)
                    {
                        Debug.LogWarning("There is Iso2D Object without Sprite.\nCheck this Object : " + names);
                        Debug.LogWarning("Iso2DObject Must be a descendant of RegularCollider\n" + names);
                    }
                }
                return _tile;
            }
        }

        RegularCollider _rc = null;
        public RegularCollider RC
        {
            get
            {
                if (_rc == null)
                    _rc = gameObject.FindComponentInParent<RegularCollider>();
                return _rc;
            }
        }
        public SubColliderHelper SC
        {
            get
            {
                return gameObject.FindComponentInParent<RegularCollider>();
            }
        }

        [SerializeField]
        public bool bGroundOffsetMark = false;

        [SerializeField]
        float fDepthFudge = -0.01f;
        public float FDepthFudge { get { return fDepthFudge; } }
        public Vector2 IsometricAngle
        {
            get
            {
                if (IsoMap.IsNull)
                {
                    Transform _transform = transform;
                    if (!IsSideOfTile && Tile)
                    {
                        var sides = Tile.GetSideObjects(Type.Side_Union, Type.Side_X, Type.Side_Y, Type.Side_Z);
                        if (sides.Length > 0)
                            _transform = sides[0].transform;
                    }

                    return new Vector2(_transform.localEulerAngles.x, _transform.localEulerAngles.y);
                }
                return IsoMap.instance.TileAngle;
            }
        }

        public Vector3 GetPosition_WithoutFudge()
        {
            return transform.position - IsoMap.vDepthFudge(fDepthFudge, IsometricAngle);
        }

        public Type _Type = Type.Overlay;
        public bool IsTileRCAttachment { get {
                return RC != null && RC.gameObject == RC.Tile.gameObject;    }   }
        public bool IsColliderAttachment{get{
                return !IsSideOfTile && RC != null && RC.gameObject != RC.Tile.gameObject;    }   }
        public bool IsAttachment{get{
			return IsColliderAttachment || IsOverlay;    }   }
        public bool IsOverlay { get { return _Type == Type.Overlay; } }
		public bool IsSideOfTile{get{	return IsUnionSide || IsXYZSide;}}
		public bool IsUnionSide{get{
			return _Type == Type.Side_Union;}}
		public bool IsXYZSide{get{
			return _Type == Type.Side_X 
				|| _Type == Type.Side_Y
				|| _Type == Type.Side_Z;}}

        #region RuntimeEtc
        public GameObject GetDestoryParentObject(bool bJustDoIt)
        {
            GameObject _DestroyGameObject = gameObject;
            if (RC != null)
            {
                if (Tile.gameObject != RC.gameObject)
                {
                    bool bDestroyRC = true;
                    if (!bJustDoIt)
                    {
                        for (int i = 0; i < RC.Iso2Ds.Length; ++i)
                        {
                            if (RC.Iso2Ds[i] != null && RC.Iso2Ds[i] != this)
                            {
                                bDestroyRC = false;
                                break;
                            }
                        }
                    }
                    if (bDestroyRC)
                        _DestroyGameObject = RC.gameObject;
                }
            }
            else
            {
                var sub = SC;
                if (sub != null)
                    _DestroyGameObject = sub.gameObject;
            }
            return _DestroyGameObject;
        }

        public bool DestoryGameObject(bool bCanUndo, bool bJustDoIt)
        {
            GameObject _DestroyGameObject = GetDestoryParentObject(bJustDoIt);
#if UNITY_EDITOR
            if (bCanUndo)
                return UndoUtil.Delete(_DestroyGameObject);
            else
#endif
                DestroyImmediate(_DestroyGameObject);

            return true;
        }

        public bool ChangeSprite(Sprite _newSprite, bool _bKeepChildLoosyScale = false, bool bRecordForUndo = true, string undoName = "Changed: Sprites")
        {
            if (sprr.sprite == _newSprite)
                return false;

#if UNITY_EDITOR
            if (bRecordForUndo)
                UnityEditor.Undo.RecordObject(sprr, undoName);

            if (_bApplyPPUScale)
            {
                Toggle_ApplyPPUScale();
                sprr.sprite = _newSprite;
                Toggle_ApplyPPUScale();
            }
            else
#endif
            {
                sprr.sprite = _newSprite;
            }
            return true;
        }
        #endregion

#if UNITY_EDITOR
        GridCoordinates _coordinates;
		[HideInInspector]
		public GridCoordinates coordinates{get{
			return _coordinates == null ?
				_coordinates = (Tile == null ? null : Tile.coordinates) : _coordinates;
		}}		

        public void UpdateIsometricRotationScale()
        {
            if (Tile.bAutoFit_SpriteSize)
            {
                IsometricRotationScale = IsoMap.fScale_TA_Y(IsometricAngle, coordinates.TileSize);
            }
            else
            {
                IsometricRotationScale = 0f;
            }
        }

		void OnTransformParentChanged()
		{
			_rc = null;
			_tile = null;
		}		

		public void Copycat(Iso2DObject _target, bool bUndoable = true)
		{
			SpriteRenderer sprr = GetComponent<SpriteRenderer>();
			if (bUndoable)
			{
				UnityEditor.Undo.RecordObject(sprr, "IsoTile:Copycat:Sprite");
				UnityEditor.Undo.RecordObject(this, "IsoTile:Copycat:CustomTransform");
			}
			sprr.sprite = _target.GetComponent<SpriteRenderer>().sprite;			
			localRotation = _target.localRotation;
			localScale = _target.localScale;
			if (coordinates != null)
				coordinates.bSnapFree = _target.coordinates.bSnapFree;	
			UnityEditor.EditorUtility.SetDirty(sprr);	
		}

		public static List<Iso2DObject>  GetSideListOfTileSelection(params Type[] _types)
		{
			List<Iso2DObject> _result = new List<Iso2DObject>();
			foreach(GameObject _go in UnityEditor.Selection.gameObjects)
			{
				if (_go == null)
					continue;
				
				IsoTile _t = _go.GetComponent<IsoTile>();
				if (_t == null)
					continue;
				
				if (_types[0] == Type.Side_Union)
				{
					Iso2DObject _Iso2D = _t.GetSideObject(Type.Side_Union);
					if (_Iso2D != null)
						_result.Add(_Iso2D);
				}
				else
				{
					_result.AddRange(_t.GetSideObjects(_types));
				}
			}
			return _result;
		}
		
        public void Undo_LocalScale(Vector3 _newScale)
		{
			// UnityEditor.Undo.RecordObject(this, "Iso2DObject : LocalScale");
			// localScale = Vector3.Scale(localScale, _newScale);
			UnityEditor.Undo.RecordObject(transform, "Iso2DObject : LocalScale");
			adjustScale(_newScale);
		}
		public void Undo_PositionOffset(Vector3 _newOffset)
		{
			UnityEditor.Undo.RecordObject(transform, "Iso2DObject : LocalOffset");
            transform.position = _newOffset + IsoMap.vDepthFudge(fDepthFudge, IsometricAngle);
		}
        public void Undo_AddDepthFudge(float fAdd)
        {
            Undo_NewDepthFudge(fDepthFudge + fAdd);
        }
		public void Undo_NewDepthFudge(float _newDepthFudge)
		{
			UnityEditor.Undo.RecordObject(this, "DepthFudge changed");
			Vector3 _vTmp = GetPosition_WithoutFudge();
			fDepthFudge = _newDepthFudge;
            Undo_PositionOffset(_vTmp);
		}
        public void Undo_AddDepthFudgeOnly(float fAdd)
        {
            UnityEditor.Undo.RecordObject(this, "DepthFudge changed");
            fDepthFudge += fAdd;
        }

        public override void AdjustScale()
		{
			if (RC != null)
				adjustScale(RC.Iso2DScaleMultiplier);
			else
				base.AdjustScale();
		}

        private void OnValidate()
        {
            if (PrefabHelper.IsPrefab(gameObject) == true || Tile == null)
                return;

            UpdateIsometricRotationScale();
            AdjustScale();
        }

        static Color gizmoColor = new Color(0.95f, 0.5f, 0.05f, 0.65f);
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoColor;
            if (UnityEditor.Selection.activeObject == transform.gameObject)
            {
                if (SC)
                    SC.Gizmo_SimpleBounds();

                if (transform.parent != null)
                {
                    var gizmo = transform.parent.GetComponent<IGizmoDraw>();
                    if (gizmo != null)
                        gizmo.GizmoDraw();
                }
            }
        }
#endif
    }
}
