using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Isometric
{
    using Util;

    // [CreateAssetMenu(fileName = "SubTileSet", menuName = "Anonym/SubTileSet", order = 601)]
    [System.Serializable] 
    public class SubTileSet : ScriptableObject
    {
        public enum Type
        {
            Normal = 0,
            OneWay = 1,
            TwoWay = 2,
            ThreeWay = 3,

            Ex_3_Diagonal = 4,
            Ex_2_Diagonal_Together = 5,
            Ex_2_Diagonal_Apart_H = 6,
            Ex_2_Diagonal_Apart_V = 7,
            Ex_1_Diagonal = 8,

            Custom = 10,
        }

        public enum matchType
        {
            Null = 0,
            BaseField = 1,
            OutField = 2,
            OtherField = 3,
        }

        public static readonly InGameDirection[] AllDirections = {
            InGameDirection.Top_Move, InGameDirection.TR_Move, InGameDirection.Right_Move, InGameDirection.DR_Move,
            InGameDirection.Down_Move, InGameDirection.DL_Move, InGameDirection.Left_Move, InGameDirection.TL_Move };
        public static readonly InGameDirection[] CrossDirections = { InGameDirection.Top_Move, InGameDirection.Right_Move, InGameDirection.Down_Move, InGameDirection.Left_Move };
        public static readonly InGameDirection[] DiagonalDirections = { InGameDirection.TL_Move, InGameDirection.TR_Move, InGameDirection.DR_Move, InGameDirection.DL_Move };

        const string _defaultName = "[Sub]";

        [SerializeField]
        public Type tileSetType = Type.Normal;

        [SerializeField]
        TileSetSprites outField = null;
        public TileSetSprites OutField {
            get { return outField; }
            set { outField = value; }
        }

        [SerializeField, HideInInspector]
        public Sprite GetParentBaseSprite
        {
            get { return parent ? parent.baseSprite : null ; }
        }

        #region Relative Shape
        // type
        public enum RelativeShape
        {
            Sprite = 0,
            Tile = 1,

            Count = 2,
        }

        const int iFieldCount = 9;

        // Field Type
        [SerializeField]
        RelativeShape[] FieldTypes = new RelativeShape[iFieldCount] {
            RelativeShape.Sprite , RelativeShape.Sprite , RelativeShape.Sprite ,
            RelativeShape.Sprite , RelativeShape.Sprite , RelativeShape.Sprite ,
            RelativeShape.Sprite , RelativeShape.Sprite , RelativeShape.Sprite };

        // Common
        public TileSetSprites getDirectionalTileSetSprites(InGameDirection _dir, out InGameDirection _outDir)
        {
            _outDir = _dir;

            if (parent != null && (
                // (tileSetType == Type.Normal && _dir == InGameDirection.BaseField) ||
                (tileSetType == Type.ThreeWay && CrossDirections.Contains(_dir)) ||
                (tileSetType == Type.Ex_1_Diagonal && DiagonalDirections.Contains(_dir)) ||
                (tileSetType == Type.Ex_2_Diagonal_Together && (CrossDirections.Contains(_dir) || _dir == InGameDirection.BaseField)) ||
                (tileSetType == Type.Ex_2_Diagonal_Apart_H && InGameDirection.Left_Move.Get(rights:true).Contains(_dir)) ||
                (tileSetType == Type.Ex_2_Diagonal_Apart_V && InGameDirection.Top_Move.Get(rights:true).Contains(_dir)) ||
                (tileSetType == Type.Ex_3_Diagonal && (DiagonalDirections.Contains(_dir) || _dir == InGameDirection.BaseField))))
                return parent;

            switch (tileSetType)
            {
                case Type.OneWay:
                    if (CrossDirections.Contains(_dir))
                        _dir = InGameDirection.OutField;
                    break;

                case Type.TwoWay:
                    if (_dir == InGameDirection.BaseField)
                        _dir = InGameDirection.OutField;
                    break;

                case Type.ThreeWay:
                    if (_dir == InGameDirection.BaseField)
                        _dir = InGameDirection.OutField;
                    break;

                case Type.Ex_1_Diagonal:
                    if (_dir == InGameDirection.BaseField)
                        _dir = InGameDirection.OutField;
                    break;

                case Type.Ex_2_Diagonal_Apart_H:
                    if (_dir == InGameDirection.BaseField)
                        _dir = InGameDirection.OutField;
                    break;

                case Type.Ex_2_Diagonal_Apart_V:
                    if (_dir == InGameDirection.BaseField)
                        _dir = InGameDirection.OutField;
                    break;

                case Type.Ex_2_Diagonal_Together:
                    break;

                case Type.Ex_3_Diagonal:
                    break;
            }

            _outDir = _dir;
            if (_dir == InGameDirection.OutField && OutField != null)
                return OutField;

            return null;
        }
        public int getFillteredOutDirectionalIndex(InGameDirection _dir)
        {

            if ((tileSetType == Type.TwoWay && (_dir == InGameDirection.DL_Move || _dir == InGameDirection.DR_Move)) ||
                (tileSetType == Type.Ex_2_Diagonal_Apart_V && _dir == InGameDirection.Down_Move) ||
                (tileSetType == Type.Ex_2_Diagonal_Apart_H && _dir == InGameDirection.Right_Move))
            {
                _dir = _dir.Get(opposite: true).First();
            }

            return (int)_dir;
        }
        public bool ApplyShape(IsoTile _target, InGameDirection _outDir, bool bNullable = false, bool bUndoable = true, string UndoName = "Apply: TilsSet Shape")
        {
            RelativeShape _type = RelativeShape.Sprite;
            int index = (int)_outDir;
            if (index >= 0 && index < FieldTypes.Length)
                _type = FieldTypes[index];
            else
                return false;

            switch (_type)
            { 
                case RelativeShape.Sprite:
                    _target.ChangeBaseSprite(getDirectionalSprite(_outDir), bNullable, bUndoable, UndoName);
                    break;
                case RelativeShape.Tile:
                    _target.Copycat(getDirectionalTile(_outDir), true, true, true, bUndoable, UndoName);
                    _target.tileSetSprites = parent;
                    break;
                default:
                    return false;
            }

            return true;
        }

        // Sprite Field
        [SerializeField, HideInInspector]
        Sprite[] DirectionalSprites = new Sprite[iFieldCount] { null, null, null, null, null, null, null, null, null };
        Sprite getDirectionalSprite(InGameDirection _dir)
        {
            int index = getFillteredOutDirectionalIndex(_dir);
            if (index < 0 || index >= iFieldCount)
                return null;

            return DirectionalSprites[index];
        }
        InGameDirection GetDirectionalSprite_withTileSetSprites(InGameDirection _dir)
        {
            InGameDirection outDIr;
            TileSetSprites _tileset = getDirectionalTileSetSprites(_dir, out outDIr);

            if (_tileset != null)
            {
                if (_tileset == parent)
                    return InGameDirection.ParentField;
                else if (_tileset == outField)
                    return InGameDirection.OutField;
            }

            return outDIr;
        }
        
        // Tile Field
        [SerializeField, HideInInspector]
        IsoTile[] DirectionalTiles = new IsoTile[iFieldCount] { null, null, null, null, null, null, null, null, null };
        IsoTile getDirectionalTile(InGameDirection _dir)
        {
            int index = getFillteredOutDirectionalIndex(_dir);

            if (index < 0 || index >= iFieldCount)
                return null;

            return DirectionalTiles[index];
        }

        // Specipic Neighbors
        [SerializeField, HideInInspector]
        TileSetSprites[] CustomOutField = new TileSetSprites[9] { null, null, null, null, null, null, null, null, null};
        public TileSetSprites GetCustomDirectionalTileSet(InGameDirection _dir)
        {
            int index = (int)_dir;
            if (index < 0 || index >= CustomOutField.Length)
                return null;
            return CustomOutField[index];
        }
        public bool IsRelative(TileSetSprites _compareWith)
        {
            if (_compareWith == null)
                return false;

            if (tileSetType != SubTileSet.Type.Custom)
                return _compareWith == OutField;

            return CustomOutField.Contains(_compareWith);
        }
        #endregion

        [SerializeField, HideInInspector]
        public TileSetSprites parent;

        [SerializeField]
        [Tooltip("When the other conditions are the same, the SubTileSet of the higher value of iPriority is used.")]
        public int iPriority = 0;

        [SerializeField]
        [Tooltip("When the other conditions are the same, the SubTileSet of the higher value of iPriority is used.")]
        public bool bAllowNullOutField = true;

#if UNITY_EDITOR
        [SerializeField]
        public Vector2 vIsometricAngle = IsoMap.DefaultTileAngle;

        [SerializeField, HideInInspector]
        public bool bSimpleView = false;

        [SerializeField, HideInInspector]
        public bool bFlodOut = false;

        [SerializeField, HideInInspector]
        public Texture2D bakedTileSetImg;

        public static SubTileSet CreateAsset(Object parentAsset, string postName = null)
        {
            if (parentAsset == null || !AssetDatabase.Contains(parentAsset))
            {
                Debug.LogError("No ParentAsset");
                return null;
            }

            var _instance = ScriptableObject.CreateInstance<SubTileSet>();
            _instance.name = string.IsNullOrEmpty(postName) ? _defaultName : string.Format("{0} {1}", _defaultName, postName);
            _instance.parent = parentAsset as TileSetSprites;
            _instance.InitBaseSprite();

            AssetDatabase.AddObjectToAsset(_instance, parentAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return _instance;
        }
#endif

        public Dictionary<InGameDirection, matchType> ConvertNeighbourInfo(Dictionary<InGameDirection, TileSetSprites> tilesetSprites)
        {
            Dictionary<InGameDirection, matchType> NeighborInfo = new Dictionary<InGameDirection, matchType>();
            foreach (var one in tilesetSprites)
            {
                TileSetSprites value = one.Value;
                if (value == null)
                    NeighborInfo.Add(one.Key, matchType.Null);
                else if (value == parent)
                    NeighborInfo.Add(one.Key, matchType.BaseField);
                else if (one.Value == OutField)
                    NeighborInfo.Add(one.Key, matchType.OutField);
                else
                    NeighborInfo.Add(one.Key, matchType.OtherField);
            }
            return NeighborInfo;
        }

        /// <summary>
        /// Send Tags of neighborhood Tiles as parameter.
        /// (T_Tag, TL_Tag, TR_Tag, L_Tag, R_Tag, DL_Tag, DR_Tag, D_Tag)
        /// Most proper sprite will be returned.
        /// </summary>
        public InGameDirection FindProperDir(Dictionary<InGameDirection, TileSetSprites> tilesetSprites)
        {
            Dictionary<InGameDirection, matchType> NeighborInfo = ConvertNeighbourInfo(tilesetSprites);

            switch (tileSetType)
            {
                case Type.Normal:
                    return GetProperSprite_Normal(NeighborInfo);

                case Type.TwoWay:
                    return GetProperSprite_TwoWay(NeighborInfo);

                case Type.OneWay:
                    return GetProperSprite_OneWay(NeighborInfo);

                case Type.ThreeWay:
                    return GetProperSprite_ThreeWay(NeighborInfo);

                case Type.Ex_1_Diagonal:
                    return GetProperSprite_Ex_1_Diagonal(NeighborInfo);

                case Type.Ex_2_Diagonal_Apart_H:
                    return GetProperSprite_Ex_2_Diagonal_Apart_H(NeighborInfo);

                case Type.Ex_2_Diagonal_Apart_V:
                    return GetProperSprite_Ex_2_Diagonal_Apart_V(NeighborInfo);

                case Type.Ex_2_Diagonal_Together:
                    return GetProperSprite_Ex_2_Diagonal_Together(NeighborInfo);

                case Type.Ex_3_Diagonal:
                    return GetProperSprite_Ex_3_Diagonal(NeighborInfo);

                case Type.Custom:
                    return GetProperSprite_Custom(NeighborInfo);
            }

            if (NeighborInfo.Where(n => DiagonalDirections.Contains(n.Key)).All(n => n.Value == matchType.Null))
                return InGameDirection.ParentField;

            return InGameDirection.None;
        }

        InGameDirection GetProperSprite_Normal(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();
            foreach(var one in CrossDirections)
            {
                var sides = one.Get(sides: true);
                var opposites = one.Get(sides_of_opposite: true, opposite: true);

                if (!Neighbors_Check_Any(matchType.OutField, false, neighbors, opposites) &&
                    ((Neighbors_Check_All(matchType.OutField, false, neighbors, sides) && Neighbors_Check_All(matchType.BaseField, bAllowNullOutField, neighbors, opposites) ||
                    (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, sides) && Neighbors_Check_All(matchType.BaseField, false, neighbors, opposites)))))
                    results.Add(one);
            }

            foreach (var one in DiagonalDirections)
            {
                var opposites = one.Get(rights: true, opposite: true); // sides_of_opposite: true <-이 옵션이 있으면 대각선 없으면 3갈래 길과 3갈래 + 1대각선 길에 우서순위가 필요하다.
                if (Neighbors_Check_All(matchType.OutField, false, neighbors, one) && Neighbors_Check_All(matchType.BaseField, bAllowNullOutField, neighbors, opposites) ||
                    Neighbors_Check_All(matchType.OutField, bAllowNullOutField, neighbors, one) && Neighbors_Check_All(matchType.BaseField, false, neighbors, opposites))
                    results.Add(one);
            }

            if (Neighbors_Check_All(matchType.BaseField, false, neighbors, AllDirections))
                results.Add(InGameDirection.BaseField);

            return multiResult(results);
        }

        InGameDirection GetProperSprite_TwoWay(Dictionary<InGameDirection, matchType> neighbors)
        {
            //if (!Neighbors_Check_Any(matchType.BaseField, false, neighbors, DiagonalDirections))
            //    return InGameDirection.None;

            var results = new List<InGameDirection>();

            foreach (var one in CrossDirections)
            {
                ////// var sides = one.Get(sides: true);
                ////// var opposites = one.Get(sides_of_opposite: true, opposite: true);

                if (Neighbors_Check_All(matchType.OutField, bAllowNullOutField, neighbors, one.Get(opposite:true)) &&
                    Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(sides:true)) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(sides_of_opposite: true)))
                    results.Add(one);
            }

            foreach (var one in InGameDirection.Top_Move.Get(sides:true))
            {
                var dirLine = one.Get(self: true, opposite: true);
                var crossLine = one.Get(rights: true);

                bool isAtleastHalfLine = Neighbors_Check_Any(matchType.BaseField, false, neighbors, crossLine);   

                if ((isAtleastHalfLine && Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, dirLine))
                    || (!isAtleastHalfLine && Neighbors_Check_All(matchType.OutField, false, neighbors, dirLine)))
                {
                    if (Neighbors_Check_All(matchType.BaseField, bAllowNullOutField, neighbors, crossLine)) 
                        // && !Neighbors_Check_Any(matchType.BaseField, false, neighbors, crossLine))
                        results.Add(one);
                }
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_OneWay(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();
            if (Neighbors_Check_All(matchType.BaseField, false, neighbors, DiagonalDirections) && !Neighbors_Check_Any(matchType.BaseField, false, neighbors, CrossDirections))
                results.Add(InGameDirection.BaseField);
            else
            {
                if (neighbors.Count((n) => { return DiagonalDirections.Contains(n.Key) ? n.Value == matchType.BaseField : false; }) != 1 &&
                    neighbors.Count((n) => { return DiagonalDirections.Contains(n.Key) ? n.Value == matchType.OutField : false; }) != 3)
                    return InGameDirection.None;

                // Peninsular type
                foreach (var one in DiagonalDirections)
                {
                    if (Neighbors_Check_All(matchType.BaseField, bAllowNullOutField, neighbors, one.Get(opposite: true)) &&
                        Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(self: true, rights: true)))
                        results.Add(one);
                }
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_ThreeWay(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();

            foreach(var one in DiagonalDirections)
            {
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(opposite:true, sides:true)) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(rights:true, self:true)))
                    results.Add(one);
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Ex_1_Diagonal(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();
            // Cross 방향 중 하나만 OuterField이고 나머지는 모두 BaseField인 경우
            foreach (var one in CrossDirections)
            {
                if (Neighbors_Check_All(matchType.OutField, bAllowNullOutField, neighbors, one.Get(opposite:true)) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(self:true, sides:true, rights: true, sides_of_opposite:true)))
                    results.Add(one);
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Ex_2_Diagonal_Apart_H(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();

            InGameDirection[] leftNright = InGameDirection.Left_Move.Get(self: true, opposite: true);
            foreach (var one in leftNright)
            {
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, leftNright) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(sides: true, rights: true, sides_of_opposite: true)))
                    results.Add(one);
            }

            foreach (var one in DiagonalDirections)
            {
                InGameDirection[] disrForOut = one.Get(sides: true).Where(d => !leftNright.Contains(d)).ToArray();
                InGameDirection[] disrForBase = one.Get(sides: true).Where(d => leftNright.Contains(d)).ToArray();
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(opposite: true).Concat(disrForOut).ToArray()) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(self:true, rights: true).Concat(disrForBase).ToArray()))
                {
                    results.Add(one);
                }
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Ex_2_Diagonal_Apart_V(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();

            InGameDirection[] upNdown = InGameDirection.Top_Move.Get(self: true, opposite: true);
            foreach (var one in upNdown)
            {
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, upNdown) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(sides: true, rights: true, sides_of_opposite: true)))
                    results.Add(one);
            }

            foreach (var one in DiagonalDirections)
            {
                var sides = one.Get(sides: true);
                InGameDirection[] disrForOut = sides.Where(d => !upNdown.Contains(d)).ToArray();
                InGameDirection[] disrForBase = sides.Where(d => upNdown.Contains(d)).ToArray();
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(opposite: true).Concat(disrForOut).ToArray()) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(self:true, rights: true).Concat(disrForBase).ToArray()))
                {
                    results.Add(one);
                }
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Ex_2_Diagonal_Together(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();

            foreach (var one in DiagonalDirections)
            {
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(sides: true)) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(self: true, rights: true, sides_of_opposite:true, opposite: true)))
                    results.Add(one);
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Ex_3_Diagonal(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();

            foreach (var one in CrossDirections)
            {
                if (Neighbors_Check_All_Individually(matchType.OutField, bAllowNullOutField, neighbors, one.Get(self: true, rights: true)) &&
                    Neighbors_Check_All(matchType.BaseField, false, neighbors, one.Get(sides: true, sides_of_opposite:true, opposite: true)))
                    results.Add(one);
            }

            return multiResult(results);
        }

        InGameDirection GetProperSprite_Custom(Dictionary<InGameDirection, matchType> neighbors)
        {
            var results = new List<InGameDirection>();
            foreach (var one in neighbors)
            {
                if (one.Key == InGameDirection.BaseField)
                    continue;

                var _target = GetCustomDirectionalTileSet(one.Key);
                switch(one.Value)
                {
                    case matchType.BaseField:
                        if (_target != parent)
                            return InGameDirection.None;
                        break;
                    case matchType.Null:
                        if (_target != null)
                            return InGameDirection.None;
                        break;
                    case matchType.OtherField:
                        if (_target == parent || _target == null || _target == OutField)
                            return InGameDirection.None;
                        break;
                    case matchType.OutField:
                        if (_target != OutField)
                            return InGameDirection.None;
                        break;
                }
            }
            results.Add(InGameDirection.BaseField);
            return multiResult(results);
        }

        //static bool Neighbors_Check(matchType type, bool bAllowNullOutField, Dictionary<InGameDirection, matchType> neighbors, params InGameDirection[] _dirs)
        //{
        //    return Neighbors_Check_All(type, bAllowNullOutField, neighbors, _dirs) &&
        //        !Neighbors_Check_Any(type, false, neighbors, AllDirections.Where(d => !_dirs.Contains(d)).ToArray());
        //}

        static bool Neighbors_Check_All(matchType type, bool bAllowNullOutField, Dictionary<InGameDirection, matchType> data, params InGameDirection[] _dirs)
        {
            if (_dirs.All(d => data[d] == type))
                return true;
            return bAllowNullOutField ? _dirs.All(d => (data[d] == matchType.Null)) : false;
        }

        static bool Neighbors_Check_All_Individually(matchType type, bool bAllowNullOutField, Dictionary<InGameDirection, matchType> data, params InGameDirection[] _dirs)
        {
            return _dirs.All(d => Neighbors_Check_All(type, bAllowNullOutField, data, d));
        }

        public static bool Neighbors_Check_Any(matchType type, bool bAllowNullOutField, Dictionary<InGameDirection, matchType> data, params InGameDirection[] _dirs)
        {
            return _dirs.Any(d => (data[d] == type || (bAllowNullOutField ? data[d] == matchType.Null : false)));
        }

        InGameDirection multiResult(List<InGameDirection> results)
        {
            if (results != null && results.Count > 0)
            {
#if UNITY_EDITOR
                if (TileSetSprites.bLogWhenMultiProperSprite && results.Count > 1)
                    Debug.LogWarning("Two or more satisfied conditions have been detected in the SubTileSet. : " + string.Join(" ", results.Select(r => r.ToString()).ToArray()));
#endif
                return GetDirectionalSprite_withTileSetSprites(results.First());
            }
            return InGameDirection.None;
        }

        public string UpdateName()
        {
            return name = string.Format("{0}_{1}_{2}", _defaultName, OutField != null ? OutField.name : "Null", tileSetType);
        }

        void InitBaseSprite()
        {
            if (parent == null)
                return;

            for(int i = 0; i < DirectionalSprites.Length; ++i)
            {
                DirectionalSprites[i] = parent.baseSprite;
            }
        }
    }
}