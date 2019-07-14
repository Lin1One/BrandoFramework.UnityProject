using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    public interface IISOBasis
    {
        ISOBasis GetISOBasis();

        ISOBasis SetUp(ISOBasis basis = null);
        void Remove();
        void DestroyISOBasis();

        Bounds GetBounds();

        bool IsOnGroundObject();

        void Update_SortingOrder(bool bChildRC);
        void Undo_UpdateDepthFudge(float fFudge, bool bNewFudge = false);

        int CalcSortingOrder(bool bWithBasis);
    }
}