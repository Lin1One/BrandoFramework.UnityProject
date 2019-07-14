using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Anonym.Isometric
{
    public interface IUpdateSortingOrder
    {
        void UpdateSortingOrder();
    }

    [System.Serializable]
    [RequireComponent(typeof(SpriteMask))]
    [RequireComponent(typeof(IsometricSortingOrder))]
    public class SpriteMaskAssist : MonoBehaviour, IUpdateSortingOrder
    {
        SpriteMask spriteMask;
        SpriteRenderer sprr;
        IsometricSortingOrder order;

        [SerializeField]
        List<SpriteRenderer> sprrList = new List<SpriteRenderer>();

        [SerializeField]
        List<TallCharacterHelper> tchList = new List<TallCharacterHelper>();

        private void Start()
        {
            // Init();
        }

        public void Init(SpriteRenderer _sprr = null, float fAlphaCutoff = 1f)
        {
            if (spriteMask == null)
                spriteMask = GetComponent<SpriteMask>();
            if (sprr == null)            
                sprr = _sprr != null ? _sprr : GetComponentInParent<SpriteRenderer>();
            if (order == null)
                order = GetComponent<IsometricSortingOrder>();
            order.AddUpdateCallBack(this);

            spriteMask.alphaCutoff = fAlphaCutoff;

            UpdateSprite();
        }

        public void UpdateSprite()
        {
            if (sprr != null)
            {
                spriteMask.sprite = sprr.sprite;
                transform.localScale = new Vector3(sprr.flipX ? -1 : 1, sprr.flipY ? -1 : 1, 1);
            }
        }

        public bool IsThis(SpriteRenderer _sprr)
        {
            return (_sprr == null || sprr == null) ? false : sprr == _sprr;
        }

        private void OnWillRenderObject()
        {
            UpdateSprite();
        }

        public void Regist(List<SpriteRenderer> spriteRenderers, TallCharacterHelper _tch = null)
        {
            sprrList.AddRange(spriteRenderers.Where(r => r != null));// && r.enabled && !sprrList.Contains(r)));
            //sprrList = sprrList.Distinct();
            UpdateSortingOrder();

            if (_tch != null && !tchList.Contains(_tch))
                tchList.Add(_tch);
        }

        public bool UnRegist(List<SpriteRenderer> spriteRenderers, TallCharacterHelper _tch = null)
        {
            if (_tch != null && tchList.Contains(_tch))
                tchList.Remove(_tch);

            var gos = spriteRenderers.Select(r => r.gameObject).Distinct();
            spriteRenderers.ForEach(r => sprrList.Remove(r));
            if (sprrList.Any(r => gos.Contains(r.gameObject)))
                return false;

            UpdateSortingOrder();           

            return true;
        }

        public void UpdateSortingOrder()
        {
            bool bResult = sprrList.Count > 0;
            spriteMask.enabled = bResult;
            spriteMask.isCustomRangeActive = bResult;

            if (bResult)
            {
                spriteMask.backSortingOrder = sprrList.Min(r => r.sortingOrder) - 1;
                spriteMask.frontSortingOrder = Mathf.Max(spriteMask.backSortingOrder + 1, sprrList.Max(r => r.sortingOrder) + 1); // order.iLastSortingOrder - 1

                // SpriteRenderer MaskInteraction Out -> order.iLastSortingOrder - 1
                if (sprr.maskInteraction != SpriteMaskInteraction.None)
                    spriteMask.frontSortingOrder = Mathf.Min(spriteMask.frontSortingOrder, order.iLastSortingOrder - 1);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < tchList.Count; ++i)
            {
                if (tchList[i] != null)
                {
                    tchList[i].Remove(this, true);
                }
            }
    }
    }
}