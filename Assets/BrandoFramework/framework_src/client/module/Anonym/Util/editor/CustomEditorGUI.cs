using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditor.Sprites;

namespace Anonym.Util
{
    public partial class CustomEditorGUI
    {
        public static Color Color_Tile = new Color(0.72f, 1, 0.69f);
        public static Color Color_Overlay = new Color(0.69f, 1, 0.99f);
        public static Color Color_Trigger = new Color(0.8f, 0.69f, 1);
        public static Color Color_Obstacle = new Color(1, 0.69f, 0.87f);
        public static Color Color_Side = new Color(0.8f, 0.79f, 0.82f);

        public static Color Color_LightRed = new Color(0.89f, 0.80f, 0.80f);
        public static Color Color_LightGreen = new Color(0.80f, 0.89f, 0.81f);
        public static Color Color_LightBlue = new Color(0.80f, 0.88f, 0.89f);

        public static Color Color_LightMagenta = new Color(0.84f, 0.407f, 0.45f);
        public static Color Color_LightYellow = new Color(0.945f, 0.835f, 0.305f);
        public static Color Color_LightGray = new Color(0.8f, 0.8f, 0.8f);

        public static Color Color_AlphaBlend(Color foreground, Color background)
        {
            float fAlpha = foreground.a;
            if (fAlpha == 1)
                return foreground;

            float fInvAlpha = 1 - fAlpha;
            return new Color(
                fInvAlpha * background.r + fAlpha * foreground.r,
                fInvAlpha * background.g + fAlpha * foreground.g,
                fInvAlpha * background.b + fAlpha * foreground.b,
                fInvAlpha * background.a + fAlpha * foreground.a);
        }

        public static void ShowPackInfo(Sprite _sprite, GUILayoutOption _option)
        {
            if (EditorSettings.spritePackerMode == SpritePackerMode.Disabled)
                packMSG();
            else if (EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOn
                    || EditorSettings.spritePackerMode == SpritePackerMode.BuildTimeOnly)
                showPackInfo_Legacy(_sprite, _option);
#if UNITY_2017_1_OR_NEWER
            else if (EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOnAtlas
                    || EditorSettings.spritePackerMode == SpritePackerMode.BuildTimeOnlyAtlas)
                showPackInfo_Atlas(_sprite, _option);
#endif
        }
        static void packMSG()
        {
            EditorGUILayout.HelpBox("Sprite packs such as\n[Atlas] or [Legacy Pack]\n" +
                    "can help reduce rendering time.\n" +
                    "File Menu: Edit -> Project Settings -> Editor -> Sprite Packer Mode\n" + 
                    "If it is already set, it will be visible after the first build.", MessageType.None);
        }
        static void showPackInfo_Atlas(Sprite _sprite, GUILayoutOption _option)
        {
            Texture2D _spriteTexture = null;
            if (_sprite.packed)
                _spriteTexture = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(_sprite, true);
            if (_spriteTexture != null)
            {
                EditorGUILayout.LabelField("[SpriteAtlas]", EditorStyles.boldLabel, _option);
                EditorGUI.DrawTextureTransparent(
                    EditorGUILayout.GetControlRect(
                        new GUILayoutOption[] { _option, GUILayout.MaxHeight(150) }),
                        _spriteTexture, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUILayout.LabelField("[No Atlas]", EditorStyles.boldLabel, _option);
                packMSG();
            }

        }
        static void showPackInfo_Legacy(Sprite _sprite, GUILayoutOption _option)
        {
            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_sprite));
            ti.spritePackingTag = EditorGUILayout.TextField(ti.spritePackingTag, _option);

            EditorGUILayout.LabelField("[Packing Tag]", EditorStyles.boldLabel, _option);
            Texture2D _texture = SpriteUtility.GetSpriteTexture(_sprite, _sprite.packed);
            if (_texture != null)
            {
                if (_sprite.packed)
                {
                    EditorGUI.DrawTextureTransparent(
                        EditorGUILayout.GetControlRect(
                            new GUILayoutOption[] { _option, GUILayout.MaxHeight(150) }),
                        _texture, ScaleMode.ScaleToFit);
                }
                else
                {
                    string _msg = !string.IsNullOrEmpty(ti.spritePackingTag)
                        ? "You have to build to see a Packed Texture."
                        : "If you set the same [Packing Tag] to sprites used in the map, render speed will be faster.";
                    EditorGUI.HelpBox(
                        EditorGUILayout.GetControlRect(
                            new GUILayoutOption[] { _option, GUILayout.MaxHeight(75) }), _msg, MessageType.Info);
                }
            }
        }

        public static void NewParagraph(string _msg, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField(_msg, EditorStyles.boldLabel, options);
            EditorGUI.indentLevel++;
        }

        public static void NewParagraph(string _msg, string _tooltip, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField(new GUIContent(_msg, _tooltip), EditorStyles.boldLabel, options);
            EditorGUI.indentLevel++;
        }

        public static bool NewParagraphWithHideToggle(string _msg, string _hideMSG, bool bHide, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.Separator();
            Rect rt_Title = EditorGUILayout.GetControlRect(options);

            GUIStyle style = EditorStyles.boldLabel;
            GUIContent _label = new GUIContent(_msg);
            Vector2 labelSize = style.CalcSize(_label);
            Rect rt_Label = rt_Title;
            rt_Label.width = labelSize.x + EditorGUIUtility.singleLineHeight;
            rt_Title.xMin = rt_Label.xMax;

            EditorGUI.LabelField(rt_Label, _label, style);
            return EditorGUI.ToggleLeft(rt_Title, _hideMSG, bHide);
        }

        public static void NewHelpBox(string _msg, MessageType _type, int _indextLevel)
        {
            EditorGUI.indentLevel += _indextLevel;
            EditorGUILayout.HelpBox(_msg, _type);
            EditorGUI.indentLevel -= _indextLevel;
        }

        //public static bool Undo_Change_Sprite(SpriteRenderer _sprr, Sprite _newSprite)
        //{
        //    if (_newSprite != _sprr.sprite)
        //    {
        //        Undo.RecordObject(_sprr, "Sprite Changed");
        //        _sprr.sprite = _newSprite;
        //        return true;
        //    }
        //    return false;
        //}

        public static GameObject Undo_Instantiate(GameObject _prefab, Transform _transform, string _actionName, bool _buttonAction)
        {
            if (!_buttonAction || GUILayout.Button(_actionName))
            {
                return Undo_Instantiate(_prefab, _transform, _actionName);
            }
            return null;
        }

        static GameObject Undo_Instantiate(GameObject _prefab, Transform _transform, string _actionName)
        {
            GameObject _obj = GameObject.Instantiate(_prefab, _transform, false);
            Undo.RegisterCreatedObjectUndo(_obj, _actionName);
            return _obj;
        }
        // public static void DrawTexture(Rect _DrawRect, SpriteRenderer sprr, bool _bSimpleDraw = false)
        // {
        //     if (sprr == null)
        //     {
        //         EditorGUI.LabelField(_DrawRect, "No Texture!");
        //         return;
        //     }

        //     DrawSprite(_DrawRect, sprr.sprite, _bSimpleDraw);
        // }
        
        public static void DrawSprite(Rect _DrawRect, Sprite _sprite, Color _color, bool _bSquare, bool _bSimpleDraw)
        {
            if (_sprite == null || _sprite.texture == null)
            {
                EditorGUI.LabelField(_DrawRect, "No Texture!");
                return;
            }

            if (_bSquare)
                _DrawRect.width = _DrawRect.height = Mathf.Min(_DrawRect.width, _DrawRect.height);
            Texture texture = _sprite.texture;

            bool bRectPacked = _sprite.packed && _sprite.packingMode == SpritePackingMode.Rectangle;
            Rect tr = bRectPacked ? _sprite.textureRect : _sprite.rect;
            Rect _SourceRect = new Rect(tr.x / texture.width, tr.y / texture.height,
                tr.width / texture.width, tr.height / texture.height);

            if (_bSimpleDraw)
            {
                float fRatio = tr.width / tr.height;
                if (fRatio > 1)
                    _DrawRect.height /= fRatio;
                else if (fRatio < 1)
                    _DrawRect.width *= fRatio;
            }

            bool bDrawColorRect = !_SourceRect.Equals(new Rect(0,0,1,1));

#if !UNITY_2017_1_OR_NEWER
            bDrawColorRect = true;
#endif

            if (!_color.Equals(Color.clear))
            {
                if (bDrawColorRect)
                {
                    GUI.DrawTextureWithTexCoords(_DrawRect, texture, _SourceRect);
                    Rect _rt = new Rect();
                    float _size = Mathf.Min(_DrawRect.width, _DrawRect.height) * 0.25f;
                    _rt.x = (_DrawRect.xMin + _DrawRect.xMax - _size) * 0.5f;
                    _rt.y = (_DrawRect.yMin + _DrawRect.yMax - _size) * 0.5f;
                    _rt.width = _rt.height = _size;
                    EditorGUI.DrawRect(_rt, _color);
                }
#if UNITY_2017_1_OR_NEWER
                else
                    GUI.DrawTexture(_DrawRect, texture, ScaleMode.StretchToFill, true, 1, _color, 0, 0);
#endif
            }
            else
            {
                GUI.DrawTextureWithTexCoords(_DrawRect, texture, _SourceRect);
            }

            if(EditorApplication.isPlaying && _sprite.packed)
                GUI.Label(_DrawRect, "Playing Mode\nPacked Texture");
        }

        public static Vector2 Vector2Slider(Vector2 _v2Value, Vector2 _v2ResetValue,
            string _label, Vector2 _vMin, Vector2 _vMax, float _Editor_MAXWidth)
        {
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel, GUILayout.MaxWidth(_Editor_MAXWidth));

            using (new EditorGUILayout.VerticalScope())
            {
                float _fSpcaeHeight = EditorGUIUtility.singleLineHeight / 3f;
                GUILayout.Space(_fSpcaeHeight);                
                _v2Value.x = FloatSlider("X", _v2Value.x, _vMin.x, _vMax.x, _Editor_MAXWidth);
                GUILayout.Space(_fSpcaeHeight);                
                _v2Value.y = FloatSlider("Y", _v2Value.y, _vMin.y, _vMax.y, _Editor_MAXWidth);
                GUILayout.Space(_fSpcaeHeight);
            }
            _v2ResetValue = Vector3.Max(Vector3.Min(_v2ResetValue, _vMax), _vMin);

            EditorGUILayout.Separator();
            using (new GUIBackgroundColorScope(Color_LightBlue))
            if (GUILayout.Button("Reset " + _v2ResetValue, GUILayout.MaxWidth(_Editor_MAXWidth)))
            {
                _v2Value = _v2ResetValue;
            }
            return _v2Value;
        }
        public static Vector3 Vector3Slider(Vector3 _v3Value, Vector3 _v3ResetValue,
            string _label, Vector3 _vMin, Vector3 _vMax, float _Editor_MAXWidth)
        {
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel, GUILayout.MaxWidth(_Editor_MAXWidth));

            using (new EditorGUILayout.VerticalScope())
            {
                _v3Value.x = FloatSlider("X", _v3Value.x, _vMin.x, _vMax.x, _Editor_MAXWidth);
                _v3Value.y = FloatSlider("Y", _v3Value.y, _vMin.y, _vMax.y, _Editor_MAXWidth);
                _v3Value.z = FloatSlider("Z", _v3Value.z, _vMin.z, _vMax.z, _Editor_MAXWidth);
            }
            _v3ResetValue = Vector3.Max(Vector3.Min(_v3ResetValue, _vMax), _vMin);

            EditorGUILayout.Separator();
            using (new GUIBackgroundColorScope(Color_LightBlue))
            if (GUILayout.Button("Reset " + _v3ResetValue, GUILayout.MaxWidth(_Editor_MAXWidth)))
            {
                _v3Value = _v3ResetValue;
            }
            return _v3Value;
        }
        public static float FloatSlider(string sLabel, float fValue, float fMin, float fMax, float _fMaxWidth, bool _bIndent = false)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Rect _rt = EditorGUILayout.GetControlRect(GUILayout.MaxWidth(_fMaxWidth));
                if (_bIndent)
                    _rt = EditorGUI.IndentedRect(_rt);
                fValue = FloatSlider(_rt, sLabel, fValue, fMin, fMax);
            }
            return fValue;
        }

        public static float FloatSlider(Rect _rt, string sLabel, float fValue, float fMin, float fMax)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIContent _label = new GUIContent(sLabel);
                Vector2 textSize = GUI.skin.label.CalcSize(_label);
                Rect _rtLabel = _rt;
                _rtLabel.width = textSize.x;
                _rt.xMin = _rtLabel.xMax;
                Rect[] _rts = _rt.Division(new float[] { 0.025f, 0.7f, 0.025f, 0.25f }, null);

                GUI.Label(_rtLabel, _label);
                fValue = GUI.HorizontalSlider(_rts[1], fValue, fMin, fMax);
                fValue = Mathf.Clamp(EditorGUI.FloatField(_rts[3], fValue), fMin, fMax);
            }
            return fValue;
        }

        public static void SceneViewAligne()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (SceneView.lastActiveSceneView == null)
                return;

            Camera cam = sceneView.camera;

            if (cam.orthographic == false)
            {
                cam.orthographic = true;
                cam.orthographicSize = Screen.height / 2;
            }

            Vector3 position = sceneView.pivot;
            position.x = 0;
            position.y = 0;
            position.z = -Screen.height / 2f / Mathf.Tan(cam.fieldOfView / 2 * Mathf.PI / 180);

            sceneView.rotation = new Quaternion(0, 0, 0, 1);

            sceneView.Repaint();
        }

        public static float FrameCollider(Camera cam, GameObject target)
        {
            MeshCollider c = target.GetComponent<MeshCollider>();
            return calc_FrameDistance(cam, (c.bounds.max - c.bounds.center).magnitude);
        }
        public static float FrameRenderer(Camera cam, GameObject target)
        {
            Bounds bounds = new Bounds(target.transform.position, Vector3.zero);
            foreach (var renderer in target.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return FrameBounds(cam, bounds);
        }
        public static float FrameBounds(Camera cam, Bounds bounds)
        {
            return calc_FrameDistance(cam, bounds.size.magnitude);
        }
        static float calc_FrameDistance(Camera cam, float radius)
        {
            // D = R / sin( FOV/2 );
            var fov = cam.fieldOfView;
            var d = radius / Mathf.Sin(Mathf.Deg2Rad * (fov * 0.5f));
            return d + cam.nearClipPlane;
        }
        public static void AddToSelection(GameObject _obj)
        {
            GameObject[] _gameObjects = new GameObject[Selection.gameObjects.Length + 1];
            Selection.gameObjects.CopyTo(_gameObjects, 0);
            _gameObjects[_gameObjects.Length - 1] = _obj;
            Selection.objects = _gameObjects;
        }
        public static void ComSelector<T>(T _target, string _msg) where T : Component
        {
            if (_target == null || _target.gameObject == null)
                return;

            float iWidth = EditorGUIUtility.currentViewWidth / 2 - 4;
            using (new EditorGUILayout.HorizontalScope())
            {
                //using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(_target, typeof(GameObject), allowSceneObjects: true,
                        options: GUILayout.MaxWidth(iWidth * 1.25f));
                }
                using (new GUIBackgroundColorScope(CustomEditorGUI.Color_LightMagenta))
                {
                    if (GUILayout.Button(_msg, GUILayout.MinWidth(iWidth * 0.5f)))
                        Selection.activeGameObject = _target.gameObject;
                }
            }
        }

        public static void DrawBordereddRect(Rect _rectBorder, Color _colorBorder, Rect _rectInside, Color _colorInside)
        {
            EditorGUI.DrawRect(_rectBorder, _colorBorder);
            if (_colorInside.Equals(Color.clear))
                _colorInside = EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.8f, 0.8f, 0.8f);
            EditorGUI.DrawRect(_rectInside, _colorInside);
        }

        /// https://github.com/garcialuigi/NavMeshHelper/blob/master/NavMeshHelper/Assets/Scripts/CustomEditorExtension.cs
        /// <summary>
        /// Creates a LayerMask field in an editor(EditorWindow, Editor).
        /// Unity is missing it, so there is the need to implement this handmade.
        /// Use example:
        /// private LayerMask layerMask = 0; // this has global scope
        /// 
        /// layerMask = CustomEditorExtension.LayerMaskField("Layer Mask: ", layerMask);
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                {
                    maskWithoutEmpty |= (1 << i);
                }
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                {
                    mask |= (1 << layerNumbers[i]);
                }
            }
            layerMask.value = mask;
            return layerMask;
        }

        public static bool IsMaskedLayer(LayerMask mask, GameObject obj)
        {
            return (mask.value & 1 << obj.layer) > 0;
        }

        public static void DrawSeperator()
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1.0f) });
        }

        public static bool DestroyOBJBtn<T>(T obj, string fieldName, params GUILayoutOption[] options) where T : class
        {
            bool bResult = false;
            if (obj != null)
            {
                CustomEditorGUI.DrawSeperator();
                EditorGUILayout.ObjectField(fieldName, obj as Object, typeof(T), allowSceneObjects: true);
                if (GUILayout.Button("Destroy", options))
                {
                    Object.DestroyImmediate(obj as Object);
                    bResult = true;
                }
                GUILayout.Space(8);
            }
            return bResult;
        }

        public static void FitLabel(GUIContent gUIContent)
        {
            Vector2 textSize = GUI.skin.label.CalcSize(gUIContent);
            GUI.Label(EditorGUILayout.GetControlRect(GUILayout.Width(textSize.x), GUILayout.Height(textSize.y)), gUIContent);
        }

        [System.Serializable]
        public class SimpleGrid<T> where T : Object
        {
            public List<T> elementList;
            public List<T> selection;

            [SerializeField]
            float fCellSize;

            [SerializeField]
            float fScrollHeight;

            [SerializeField]
            string fieldName;

            // System.Action<T, Rect> action;
            string BtnName;
            System.Action BtnCallbackF;

            [SerializeField]
            bool bSelectable;
            [SerializeField]
            bool bDragable;
            [SerializeField]
            bool bMultiSelectable;
            [SerializeField]
            bool bObjectField;
            [SerializeField]
            public bool bFoldOUt;
            Vector2 vScrollPos;
            [SerializeField]
            Color cSelectedColor;

            public void Init(string name, float fSize, 
                List<T> list, // System.Action<T, Rect> act, 
                string btnName, System.Action btnF, Color selectedColor,
                bool selectable = true, bool objectField = true, bool multiSelectable = true, bool dragable = false)
            {
                fieldName = name;
                // action = act;
                BtnName = btnName;
                BtnCallbackF = btnF;
                fCellSize = fSize;
                bFoldOUt = false;
                vScrollPos = Vector2.zero;
                UpdateList(list);
                bSelectable = selectable;
                if (bSelectable)
                    selection = new List<T>();
                bObjectField = objectField;
                bMultiSelectable = multiSelectable;
                cSelectedColor = selectedColor;
                bDragable = dragable;
            }
            public void UpdateList(List<T> list)
            {
                elementList = list;
            }

            public void DeselectAll()
            {
                selection.Clear();
            }

            string FieldInfoStr { get { return string.Format("[{0}] {1}({2})", fieldName, typeof(T).Name, elementList.Count); } }
            public bool ShowGrid(System.Action<T, Rect> drawCall)
            {
                if (this == null || elementList == null || elementList.Where(e => e != null).Count() == 0)
                    return false;

                return ShowGrid(this, drawCall);
            }

            static bool ShowGrid(SimpleGrid<T> grid, System.Action<T, Rect> drawCall)
            {
                return ShowGrid(grid.FieldInfoStr, drawCall, grid.elementList.GetEnumerator(), 
                    grid.BtnName, grid.BtnCallbackF, 
                    ref grid.fCellSize, ref grid.bFoldOUt, ref grid.vScrollPos, 
                    grid.selection, grid.bObjectField, grid.bMultiSelectable, grid.bDragable, grid.cSelectedColor);
            }

            public static bool ShowGrid(string fieldName, System.Action<T, Rect> action,
                IEnumerator<T> enumerator, string BtnName, System.Action updateCallbackF,
                ref float fCellSize, ref bool bFoldOUt, ref Vector2 vScrollPos,
                List<T> selection, bool bObjectField, bool bMultiSelectable, bool bDragable,
                Color selectedColor, bool isFoldable = true)
            {
                float fViewWidth = EditorGUIUtility.currentViewWidth;
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayoutOption option = GUILayout.Width(fViewWidth * 0.25f);

                    if (!isFoldable || (bFoldOUt = EditorGUILayout.Foldout(bFoldOUt, fieldName)))
                    {
                        if (!isFoldable)
                            EditorGUILayout.LabelField(fieldName);

                        fCellSize = EditorGUILayout.Slider(fCellSize, 10, fViewWidth, option);
                        if (updateCallbackF != null && GUILayout.Button(BtnName, option))
                        {
                            updateCallbackF();
                            return true;
                        }
                    }
                }

                int xCount = Mathf.Max(1, Mathf.FloorToInt((fViewWidth - 34f * EditorGUIUtility.pixelsPerPoint) / fCellSize)), xTmp = 0;
                int yCount = 0, yScrollStart = 0, yScrollEnd = 1, yTmp = 0;
                while (enumerator.MoveNext())
                {
                    yCount++;
                }
                enumerator.Reset();
                yCount = Mathf.CeilToInt((float)yCount / xCount);

                bool bResult = false;
                if (bFoldOUt)
                {
                    float fMaxHeight = yCount * fCellSize + 48f * EditorGUIUtility.pixelsPerPoint;
                    using (var scroll = new EditorGUILayout.ScrollViewScope(vScrollPos, GUILayout.MaxHeight(fMaxHeight)))
                    {
                        vScrollPos = scroll.scrollPosition;                        
                        yScrollStart = Mathf.FloorToInt(vScrollPos.y / (fCellSize * 1.25f));
                        yScrollEnd = Mathf.Min(yCount - 1, yScrollStart + Mathf.CeilToInt(fMaxHeight / fCellSize));

                        while (enumerator.MoveNext())
                        {
                            bool bContinue = true;
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                for (int i = 0; i < xCount; ++i)
                                {
                                    if (bContinue = i == 0 ? true : enumerator.MoveNext())
                                    {
                                        var current = enumerator.Current;
                                        if (yTmp >= yScrollStart && yTmp <= yScrollEnd)
                                        {
                                            if (current != null)
                                                bResult |= DrawCell(action, current, selection, fCellSize, bObjectField, bMultiSelectable, selectedColor, bDragable);
                                        }
                                        else if (i == 0)
                                            EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.Height(fCellSize));
                                    }
                                    else
                                        break;
                                }
                            }
                            if (!bContinue)
                                break;
                            yTmp++;
                        }
                    }
                }

                return bResult;
            }

            static bool DrawCell(System.Action<T, Rect> drawCell, T element, List<T> selection, float fWidth, bool bObjectField, bool bMultiSelectable, Color selectedColor, bool bDragable)
            {
                const string sOnGUI = "OnGUI";
                string dragDropIdentifier = typeof(T).Name + "Drag ID";
                bool bResult = false;

                if (drawCell == null)
                {
                    Debug.LogError("null DrawCell Method!");
                    return bResult;
                }

                var evt = Event.current;
                if (evt.type == EventType.ScrollWheel)
                {
                    return bResult;
                }

                ITag elementTag = element as ITag;
                if (elementTag != null && elementTag.Tag(sOnGUI))
                {
                    return bResult;
                }

                GUILayoutOption wOption = GUILayout.Width(fWidth), hOption = GUILayout.Height(bObjectField ? (fWidth - EditorGUIUtility.singleLineHeight) : fWidth);

                bool bSelectable = selection != null;
                bool bSelected = bSelectable && selection.Contains(element);

                Rect ImgRT = Rect.zero, obRT = Rect.zero;
                try
                {
                    using (new EditorGUILayout.VerticalScope(wOption))
                    {
                        if (elementTag != null)
                            elementTag.AddTags(sOnGUI);

                        ImgRT = EditorGUILayout.GetControlRect(wOption, hOption);
                        obRT = EditorGUILayout.GetControlRect(wOption);
                    }
                }
                catch
                {

                }

                if (evt.type == EventType.Repaint || evt.type == EventType.Layout)
                {
                    if (bSelected)
                        EditorGUI.DrawRect(ImgRT, selectedColor);

                    drawCell(element, ImgRT);
                    EditorGUI.ObjectField(obRT, element, typeof(Sprite), allowSceneObjects: false);
                }
                else if (bSelectable && evt.type == EventType.MouseUp && evt.button == 0)
                {
                    if (obRT.Contains(evt.mousePosition))
                    {
                        EditorGUIUtility.PingObject(element);
                        evt.Use();
                    }
                    else if (ImgRT.Contains(evt.mousePosition))
                    {
                        if (!bSelected)
                        {
                            if (!bMultiSelectable)
                                selection.Clear();
                            selection.Add(element);
                            bResult = true;
                        }
                        else
                        {
                            selection.Remove(element);
                            bResult = true;
                        }
                        evt.Use();
                    }
                }
                else if (bDragable
                    && (evt.type == EventType.MouseDown || evt.type == EventType.MouseUp || evt.type == EventType.MouseDrag)
                    && (evt.button == 0 && ImgRT.Contains(evt.mousePosition)))
                {
                    switch (evt.type)
                    {
                        case EventType.MouseDown:
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.SetGenericData(dragDropIdentifier, new object());
                            DragAndDrop.objectReferences = selection.ToArray();
                            evt.Use();
                            break;
                        case EventType.MouseDrag:
                            if (DragAndDrop.GetGenericData(dragDropIdentifier) != null)
                            {
                                DragAndDrop.StartDrag("Dragging List ELement");
                                evt.Use();
                            }
                            break;
                        case EventType.MouseUp:
                            DragAndDrop.PrepareStartDrag();
                            evt.Use();
                            break;
                    }
                }

                // finish
                if (elementTag != null)
                    elementTag.RemoveTags(sOnGUI);

                return bResult;
            }            
        }
    }
}