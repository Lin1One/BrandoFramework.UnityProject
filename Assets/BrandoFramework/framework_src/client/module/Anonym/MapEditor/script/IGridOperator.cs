using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anonym.Isometric
{
    ///  <summary>
    ///  The interface is intended to accommodate changes to the 2018.3 version of the Prefab. 
    ///  The class inheriting from the interface below should be able to compute without error, 
    ///  even if it is IsoMap.isNull.
    ///  </summary>
    public interface IGridOperator
    {
        Vector3 PositionToCoordinates(Vector3 globalPosition, bool bSnap = false);
        Vector3 CoordinatesToPosition(Vector3 coordinates, bool bSnap = false);
        int CoordinatesCountInTile(Vector3 _direction);

        Vector3 TileSize { get; }
        Vector3 GridInterval { get; }
        bool IsInheritGrid { get; }

        // IGridOperator ParentGridOperator { get; }
    }
}