using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
	using Util;
	
	[System.Serializable]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[ExecuteInEditMode]
    public class Iso2DBase : IsoTransform
    {
        protected SpriteRenderer _sprr;
        public SpriteRenderer sprr
        {
            get { return _sprr != null ? _sprr : _sprr = GetComponent<SpriteRenderer>(); }
        }

        Iso2DBase[] _childIso2Ds = null;
        protected Iso2DBase[] ChildIso2Ds
        {
            get
            {
                return _childIso2Ds != null ? _childIso2Ds :
                    (_childIso2Ds = findChildIso2D());
            }
        }

        Iso2DBase[] findChildIso2D()
        {
            List<Iso2DBase> _list = new List<Iso2DBase>();
            Iso2DBase _tmp;
            for (int i = 0; i < transform.childCount; ++i)
            {
                if ((_tmp = transform.GetChild(i).GetComponent<Iso2DBase>()))
                    _list.Add(_tmp);
            }

            return _list.ToArray();
        }

#if UNITY_EDITOR
        [HideInInspector]
		float _lastRefPPu;
		[HideInInspector]
		float _lastSpritePPU;
		[HideInInspector]
		float _lastPPURefScale = 1;
		public float PPURefScale{
			get{
				if (sprr == null || sprr.sprite == null)
					return 1f;
				if (!IsoMap.IsNull 
                    && (_lastSpritePPU != sprr.sprite.pixelsPerUnit ||
                    _lastRefPPu != IsoMap.instance.ReferencePPU))
				{
					_lastSpritePPU = sprr.sprite.pixelsPerUnit;
					_lastRefPPu = IsoMap.instance.ReferencePPU;
					return _lastPPURefScale = sprr.sprite.ReferencePPUScale(_lastRefPPu);
				}
				return _lastPPURefScale;
			}
		}
		[SerializeField]
		protected bool _bApplyPPUScale = true;
		public bool bApplyPPUScale{get{return _bApplyPPUScale;}}     		
		
		void OnTransformChildrenChanged()
		{
			_childIso2Ds = null;
		}

		override protected void adjustScale(Vector3 vMultiplier)
		{
			Vector3 v3Tmp = vMultiplier;
			if (bApplyPPUScale)
				v3Tmp *= PPURefScale;
            base.adjustScale(v3Tmp);
		}

		public void Toggle_ApplyPPUScale()
		{
			UnityEditor.Undo.RecordObject(this, "ApplyPPU");
			_bApplyPPUScale = !_bApplyPPUScale;
			applyPPUScaleToChild();
		}

		void applyPPUScaleToChild()
		{
			float fMultiplier = bApplyPPUScale ? 1f / PPURefScale : PPURefScale;
			for (int i = 0 ; i < transform.childCount; ++i)
			{
				Transform _t = transform.GetChild(i);
				UnityEditor.Undo.RecordObject(_t, "ApplyPPUScale : Adjust Child Scale");
				_t.localScale *= fMultiplier;
			}
		}
#endif

		public int Update_SortingOrder(int _newSortingOrder, bool bUndoable)
		{
#if UNITY_EDITOR
            if (bUndoable)
                UnityEditor.Undo.RecordObject(sprr, "SortingOrder update");
#endif
            sprr.sortingOrder = _newSortingOrder;
            if (IsoMap.isAutoISOMode)
                _newSortingOrder++;

            foreach (var r in ChildIso2Ds)
				_newSortingOrder = r.Update_SortingOrder(_newSortingOrder, bUndoable);
			return _newSortingOrder;
		}
    }
}