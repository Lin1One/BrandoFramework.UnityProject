#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InternalAttributeExampleInfoMap.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
/* THIS FILE HAS BEEN AUTOMATICALLY GENERATED. DO NOT EDIT */
/* You can delete the file and/or regenerate it from Sirenix > Utilities > Rebuild Example Info Map. */

//-----------------------------------------------------------------------
// <copyright file="AttributeExamplesInfoMap.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using System.Collections.Generic;

    internal static class InternalAttributeExampleInfoMap
    {
        public static readonly Dictionary<Type, AttributeExampleInfo[]> Map = new Dictionary<Type, AttributeExampleInfo[]>()
        {
            {
                typeof(Sirenix.OdinInspector.AssetListAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(AssetListExamples),
                        Name = "Asset List Examples",
                        Description = "The AssetList attribute work on both lists of UnityEngine.Object types and UnityEngine.Object types, but have different behaviour.",
                        PreviewObject = new AssetListExamples(),
                        Code = "public class AssetListExamples\n{\n    [AssetList]\n    [PreviewField(70, ObjectFieldAlignment.Center)]\n    public Texture2D SingleObject;\n\n    [AssetList(Path = \"/Plugins/Sirenix/\")]\n    public List<ScriptableObject> AssetList;\n\n    [FoldoutGroup(\"Filtered AssetLists examples\", expanded: false)]\n    [AssetList(Path = \"Plugins/Sirenix/\")]\n    public UnityEngine.Object Object;\n\n    [AssetList(AutoPopulate = true)]\n    [FoldoutGroup(\"Filtered AssetLists examples\")]\n    public List<PrefabRelatedAttributesExamples> AutoPopulatedWhenInspected;\n\n    [AssetList(LayerNames = \"MyLayerName\")]\n    [FoldoutGroup(\"Filtered AssetLists examples\")]\n    public GameObject[] AllPrefabsWithLayerName;\n\n    [AssetList(AssetNamePrefix = \"Rock\")]\n    [FoldoutGroup(\"Filtered AssetLists examples\")]\n    public List<GameObject> PrefabsStartingWithRock;\n\n    [FoldoutGroup(\"Filtered AssetLists examples\")]\n    [AssetList(Tags = \"MyTagA, MyTabB\", Path = \"/Plugins/Sirenix/\")]\n    public List<GameObject> GameObjectsWithTag;\n\n    [FoldoutGroup(\"Filtered AssetLists examples\")]\n    [AssetList(CustomFilterMethod = \"HasRigidbodyComponent\")]\n    public List<GameObject> MyRigidbodyPrefabs;\n\n    private bool HasRigidbodyComponent(GameObject obj)\n    {\n        return obj.GetComponent<Rigidbody>() != null;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.BoxGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(BoxGroupExamples),
                        Name = "Box Group Examples",
                        Description = null,
                        PreviewObject = new BoxGroupExamples(),
                        Code = "public class BoxGroupExamples\n{\n    // Box with a title.\n    [BoxGroup(\"Some Title\")]\n    public string A;\n\n    [BoxGroup(\"Some Title\")]\n    public string B;\n\n    // Box with a centered title.\n    [BoxGroup(\"Centered Title\", centerLabel: true)]\n    public string E;\n\n    [BoxGroup(\"Centered Title\")]\n    public string F;\n\n    // Box with a title received from a field.\n    [BoxGroup(\"$G\")]\n    public string G = \"Dynamic box title 2\";\n\n    [BoxGroup(\"$G\")]\n    public string H;\n\n    // No title\n    [BoxGroup]\n    public string C;\n\n    [BoxGroup]\n    public string D;\n\n    // A named box group without a title.\n    [BoxGroup(\"NoTitle\", false)]\n    public string I;\n\n    [BoxGroup(\"NoTitle\")]\n    public string J;\n\n    [BoxGroup(\"A Struct In A Box\"), HideLabel]\n    public SomeStruct BoxedStruct;\n\n    public SomeStruct DefaultStruct;\n\n    [Serializable]\n    public struct SomeStruct\n    {\n        public int One;\n        public int Two;\n        public int Three;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ButtonAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ButtonExamples),
                        Name = "Button Examples",
                        Description = null,
                        PreviewObject = new ButtonExamples(),
                        Code = "public class ButtonExamples\n{\n    public string ButtonName = \"Dynamic button name\";\n\n    public bool Toggle;\n\n    [Button(\"$ButtonName\")]\n    private void DefaultSizedButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [Button(\"Name of button\")]\n    private void NamedButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [Button(ButtonSizes.Small)]\n    private void SmallButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [Button(ButtonSizes.Medium)]\n    private void MediumSizedButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [DisableIf(\"Toggle\")]\n    [HorizontalGroup(\"Split\", 0.5f)]\n    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]\n    private void FanzyButton1()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [HideIf(\"Toggle\")]\n    [VerticalGroup(\"Split/right\")]\n    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]\n    private void FanzyButton2()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [ShowIf(\"Toggle\")]\n    [VerticalGroup(\"Split/right\")]\n    [Button(ButtonSizes.Large), GUIColor(1, 0.2f, 0)]\n    private void FanzyButton3()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [Button(ButtonSizes.Gigantic)]\n    private void GiganticButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n\n    [Button(90)]\n    private void CustomSizedButton()\n    {\n        this.Toggle = !this.Toggle;\n    }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ButtonWithParametersExamples),
                        Name = "Parameters Examples",
                        Description = "You can also use the Button attribute on any method with parameters.\nThis will draw a form in the inspector that lets you fill out the parameters that gets passed to the method when the button is pressed.",
                        PreviewObject = new ButtonWithParametersExamples(),
                        Code = "public class ButtonWithParametersExamples\n{\n    [Button]\n    private void Default(float a, float b, GameObject c)\n    {\n    }\n\n    [Button]\n    private void Default(float t, float b, float[] c)\n    {\n    }\n\n    [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]\n    private int FoldoutButton(int a = 2, int b = 2)\n    {\n        return a + b;\n    }\n\n    [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]\n    private void FoldoutButton(int a, int b, ref int result)\n    {\n        result = a + b;\n    }\n\n    [Button(ButtonStyle.Box)]\n    private void Full(float a, float b, out float c)\n    {\n        c = a + b;\n    }\n\n    [Button(ButtonSizes.Large, ButtonStyle.Box)]\n    private void Full(int a, float b, out float c)\n    {\n        c = a + b;\n    }\n\n    [Button(ButtonStyle.CompactBox, Expanded = true)]\n    private void CompactExpanded(float a, float b, GameObject c)\n    {\n    }\n\n    [Button(ButtonSizes.Medium, ButtonStyle.Box, Expanded = true)]\n    private void FullExpanded(float a, float b)\n    {\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ButtonGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ButtonGroupExamples),
                        Name = "Button Group Examples",
                        Description = null,
                        PreviewObject = new ButtonGroupExamples(),
                        Code = "public class ButtonGroupExamples\n{\n    [ButtonGroup]\n    private void A()\n    {\n    }\n\n    [ButtonGroup]\n    private void B()\n    {\n    }\n\n    [ButtonGroup]\n    private void C()\n    {\n    }\n\n    [ButtonGroup]\n    private void D()\n    {\n    }\n\n    [Button(ButtonSizes.Large)]\n    [ButtonGroup(\"My Button Group\")]\n    private void E()\n    {\n    }\n\n    [GUIColor(0, 1, 0)]\n    [ButtonGroup(\"My Button Group\")]\n    private void F()\n    {\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.OnValueChangedAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(OnValueChangedExamples),
                        Name = "On Value Changed Examples",
                        Description = "OnValueChanged is used here to create a material for a shader, when the shader is changed.",
                        PreviewObject = new OnValueChangedExamples(),
                        Code = "public class OnValueChangedExamples\n{\n    [OnValueChanged(\"CreateMaterial\")]\n    public Shader Shader;\n\n    [ReadOnly, InlineEditor(InlineEditorModes.LargePreview)]\n    public Material Material;\n\n    private void CreateMaterial()\n    {\n        if (this.Material != null)\n        {\n            Material.DestroyImmediate(this.Material);\n        }\n\n        if (this.Shader != null)\n        {\n            this.Material = new Material(this.Shader);\n        }\n    }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ChangingEditorToolExample),
                        Name = "Changing Editor Tool Example",
                        Description = "Example of using EnumPaging together with OnValueChanged.",
                        PreviewObject = new ChangingEditorToolExample(),
                        Code = "public class ChangingEditorToolExample\n{\n    [ShowInInspector]\n    [EnumPaging, OnValueChanged(\"SetCurrentTool\")]\n    [InfoBox(\"Changing this property will change the current selected tool in the Unity editor.\")]\n    private UnityEditor.Tool sceneTool;\n\n    private void SetCurrentTool()\n    {\n        UnityEditor.Tools.current = this.sceneTool;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.EnumPagingAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(EnumPagingExamples),
                        Name = "Enum Paging Examples",
                        Description = null,
                        PreviewObject = new EnumPagingExamples(),
                        Code = "public class EnumPagingExamples\n{\n    [EnumPaging]\n    public SomeEnum SomeEnumField;\n    \n    public enum SomeEnum\n    {\n        A, B, C\n    }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ChangingEditorToolExample),
                        Name = "Changing Editor Tool Example",
                        Description = "Example of using EnumPaging together with OnValueChanged.",
                        PreviewObject = new ChangingEditorToolExample(),
                        Code = "public class ChangingEditorToolExample\n{\n    [ShowInInspector]\n    [EnumPaging, OnValueChanged(\"SetCurrentTool\")]\n    [InfoBox(\"Changing this property will change the current selected tool in the Unity editor.\")]\n    private UnityEditor.Tool sceneTool;\n\n    private void SetCurrentTool()\n    {\n        UnityEditor.Tools.current = this.sceneTool;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ColorPaletteAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ColorPaletteExamples),
                        Name = "Color Palette Examples",
                        Description = null,
                        PreviewObject = new ColorPaletteExamples(),
                        Code = "public class ColorPaletteExamples\n{\n    [ColorPalette]\n    public Color ColorOptions;\n\n    [ColorPalette(\"Underwater\")]\n    public Color UnderwaterColor;\n\n    [ColorPalette(\"My Palette\")]\n    public Color MyColor;\n\n    public string DynamicPaletteName = \"Clovers\";\n\n    [ColorPalette(\"$DynamicPaletteName\")]\n    public Color DynamicPaletteColor;\n\n    [ColorPalette(\"Fall\"), HideLabel]\n    public Color WideColorPalette;\n\n    [ColorPalette(\"Clovers\")]\n    public Color[] ColorArray;\n\n    // ------------------------------------\n    // Color palettes can be accessed and modified from code.\n    // Note that the color palettes will NOT automatically be included in your builds.\n    // But you can easily fetch all color palettes via the ColorPaletteManager \n    // and include them in your game like so:\n    // ------------------------------------\n\n    [FoldoutGroup(\"Color Palettes\", expanded: false)]\n    [ListDrawerSettings(IsReadOnly = true)]\n    [PropertyOrder(9)]\n    public List<ColorPalette> ColorPalettes;\n\n#if UNITY_EDITOR\n\n    [FoldoutGroup(\"Color Palettes\"), Button(ButtonSizes.Large), GUIColor(0, 1, 0), PropertyOrder(8)]\n    private void FetchColorPalettes()\n    {\n        this.ColorPalettes = Sirenix.OdinInspector.Editor.ColorPaletteManager.Instance.ColorPalettes\n            .Select(x => new ColorPalette()\n            {\n                Name = x.Name,\n                Colors = x.Colors.ToArray()\n            })\n            .ToList();\n    }\n\n#endif\n    [Serializable]\n    public class ColorPalette\n    {\n        [HideInInspector]\n        public string Name;\n\n        [LabelText(\"$Name\")]\n        [ListDrawerSettings(IsReadOnly = true, Expanded = false)]\n        public Color[] Colors;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.CustomContextMenuAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(CustomContextMenuExamples),
                        Name = "Custom Context Menu Examples",
                        Description = null,
                        PreviewObject = new CustomContextMenuExamples(),
                        Code = "public class CustomContextMenuExamples\n{\n    [InfoBox(\"A custom context menu is added on this property. Right click the property to view the custom context menu.\")]\n    [CustomContextMenu(\"Say Hello/Twice\", \"SayHello\")]\n    public int MyProperty;\n\n    private void SayHello()\n    {\n        Debug.Log(\"Hello Twice\");\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.CustomValueDrawerAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(CustomValueDrawerExamples),
                        Name = "Custom Value Drawer Examples",
                        Description = null,
                        PreviewObject = new CustomValueDrawerExamples(),
                        Code = "public class CustomValueDrawerExamples\n{\n    public float From = 2, To = 7;\n\n    [CustomValueDrawer(\"MyStaticCustomDrawerStatic\")]\n    public float CustomDrawerStatic;\n\n    [CustomValueDrawer(\"MyStaticCustomDrawerInstance\")]\n    public float CustomDrawerInstance;\n\n    [CustomValueDrawer(\"MyStaticCustomDrawerArray\")]\n    public float[] CustomDrawerArray;\n\n    private static float MyStaticCustomDrawerStatic(float value, GUIContent label)\n    {\n        return EditorGUILayout.Slider(label, value, 0f, 10f);\n    }\n\n    private float MyStaticCustomDrawerInstance(float value, GUIContent label)\n    {\n        return EditorGUILayout.Slider(label, value, this.From, this.To);\n    }\n\n    private float MyStaticCustomDrawerArray(float value, GUIContent label)\n    {\n        return EditorGUILayout.Slider(value, this.From, this.To);\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DictionaryDrawerSettings),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DictionaryExamples),
                        Name = "Dictionary Examples",
                        Description = null,
                        PreviewObject = new DictionaryExamples(),
                        Code = "public class DictionaryExamples\n{\n    [InfoBox(\"In order to serialize dictionaries, all we need to do is to inherit our class from SerializedMonoBehaviour.\")]\n    public Dictionary<int, Material> IntMaterialLookup;\n\n    public Dictionary<string, string> StringStringDictionary;\n\n    [DictionaryDrawerSettings(KeyLabel = \"Custom Key Name\", ValueLabel = \"Custom Value Label\")]\n    public Dictionary<SomeEnum, MyCustomType> CustomLabels;\n\n    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]\n    public Dictionary<string, List<int>> StringListDictionary;\n\n    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]\n    public Dictionary<SomeEnum, MyCustomType> EnumObjectLookup;\n\n    [InlineProperty(LabelWidth = 90)]\n    public struct MyCustomType\n    {\n        public int SomeMember;\n        public GameObject SomePrefab;\n    }\n\n    public enum SomeEnum\n    {\n        First, Second, Third, Fourth, AndSoOn\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableContextMenuAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DisableContextMenuExamples),
                        Name = "Disable Context Menu Examples",
                        Description = null,
                        PreviewObject = new DisableContextMenuExamples(),
                        Code = "public class DisableContextMenuExamples\n{\n    [InfoBox(\"DisableContextMenu disables all right-click context menus provided by Odin. It does not disable Unity's context menu.\", InfoMessageType.Warning)]\n    [DisableContextMenu]\n    public int[] NoRightClickList;\n\n    [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]\n    public int[] NoRightClickListOnListElements;\n\n    [DisableContextMenu(disableForMember: true, disableCollectionElements: true)]\n    public int[] DisableRightClickCompletely;\n\n    [DisableContextMenu]\n    public int NoRightClickField;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.Editor.Examples.DisableIfExamples),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DisableIfExamples),
                        Name = "Disable If Examples",
                        Description = null,
                        PreviewObject = new DisableIfExamples(),
                        Code = "public class DisableIfExamples\n{\n    public UnityEngine.Object SomeObject;\n\n    [EnumToggleButtons]\n    public InfoMessageType SomeEnum;\n\n    public bool IsToggled;\n\n    [DisableIf(\"SomeEnum\", InfoMessageType.Info)]\n    public Vector2 Info;\n\n    [DisableIf(\"SomeEnum\", InfoMessageType.Error)]\n    public Vector2 Error;\n\n    [DisableIf(\"SomeEnum\", InfoMessageType.Warning)]\n    public Vector2 Warning;\n\n    [DisableIf(\"IsToggled\")]\n    public int DisableIfToggled;\n\n    [DisableIf(\"SomeObject\")]\n    public Vector3 EnabledWhenNull;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInEditorModeAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DisableInEditorModeExamples),
                        Name = "Disable In Editor Mode Examples",
                        Description = null,
                        PreviewObject = new DisableInEditorModeExamples(),
                        Code = "public class DisableInEditorModeExamples\n{\n    [Title(\"Disabled in edit mode\")]\n    [DisableInEditorMode]\n    public GameObject A;\n\n    [DisableInEditorMode]\n    public Material B;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInPlayModeAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DisableInPlayModeExamples),
                        Name = "Disable In Play Mode Examples",
                        Description = null,
                        PreviewObject = new DisableInPlayModeExamples(),
                        Code = "public class DisableInPlayModeExamples\n{\n    [Title(\"Disabled in play mode\")]\n    [DisableInPlayMode]\n    public int A;\n\n    [DisableInPlayMode]\n    public Material B;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisplayAsStringAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DisplayAsStringExamples),
                        Name = "Display As String Examples",
                        Description = null,
                        PreviewObject = new DisplayAsStringExamples(),
                        Code = "public class DisplayAsStringExamples\n{\n    [InfoBox(\n        \"Instead of disabling values in the inspector in order to show some information or debug a value. \" +\n        \"You can use DisplayAsString to show the value as text, instead of showing it in a disabled drawer\")]\n    [DisplayAsString]\n    public Color SomeColor;\n\n    [BoxGroup(\"SomeBox\")]\n    [HideLabel]\n    [DisplayAsString]\n    public string SomeText = \"Lorem Ipsum\";\n\n    [InfoBox(\"The DisplayAsString attribute can also be configured to enable or disable overflowing to multiple lines.\")]\n    [HideLabel]\n    [DisplayAsString]\n    public string Overflow = \"A very very very very very very very very very long string that has been configured to overflow.\";\n\n    [HideLabel]\n    [DisplayAsString(false)]\n    public string DisplayAllOfIt = \"A very very very very very very very very long string that has been configured to not overflow.\";\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DrawWithUnityAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(DrawWithUnityExamples),
                        Name = "Draw With Unity Examples",
                        Description = null,
                        PreviewObject = new DrawWithUnityExamples(),
                        Code = "public class DrawWithUnityExamples\n{\n    [InfoBox(\"If you ever experience trouble with one of Odin's attributes, there is a good chance that the DrawWithUnity will come in handy.\")]\n    public GameObject ObjectDrawnWithOdin;\n\n    [DrawWithUnity]\n    public GameObject ObjectDrawnWithUnity;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.EnableIfAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(EnableIfExamples),
                        Name = "Enable If Examples",
                        Description = null,
                        PreviewObject = new EnableIfExamples(),
                        Code = "public class EnableIfExamples\n{\n    public UnityEngine.Object SomeObject;\n\n    [EnumToggleButtons]\n    public InfoMessageType SomeEnum;\n\n    public bool IsToggled;\n\n    [EnableIf(\"SomeEnum\", InfoMessageType.Info)]\n    public Vector2 Info;\n\n    [EnableIf(\"SomeEnum\", InfoMessageType.Error)]\n    public Vector2 Error;\n\n    [EnableIf(\"SomeEnum\", InfoMessageType.Warning)]\n    public Vector2 Warning;\n\n    [EnableIf(\"IsToggled\")]\n    public int EnableIfToggled;\n\n    [EnableIf(\"SomeObject\")]\n    public Vector3 EnabledWhenHasReference;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.EnumToggleButtonsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(EnumToggleButtonsExamples),
                        Name = "Enum Toggle Buttons Examples",
                        Description = null,
                        PreviewObject = new EnumToggleButtonsExamples(),
                        Code = "public class EnumToggleButtonsExamples\n{\n    [Title(\"Default\")]\n    public SomeBitmaskEnum DefaultEnumBitmask;\n\n    [Title(\"Standard Enum\")]\n    [EnumToggleButtons]\n    public SomeEnum SomeEnumField;\n\n    [EnumToggleButtons, HideLabel]\n    public SomeEnum WideEnumField;\n\n    [Title(\"Bitmask Enum\")]\n    [EnumToggleButtons]\n    public SomeBitmaskEnum BitmaskEnumField;\n\n    [EnumToggleButtons, HideLabel]\n    public SomeBitmaskEnum EnumFieldWide;\n\n    public enum SomeEnum\n    {\n        First, Second, Third, Fourth, AndSoOn\n    }\n\n    [System.Flags]\n    public enum SomeBitmaskEnum\n    {\n        A = 1 << 1,\n        B = 1 << 2,\n        C = 1 << 3,\n        All = A | B | C\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.FilePathAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(FilePathExamples),
                        Name = "File Path Examples",
                        Description = "FilePath attribute provides a neat interface for assigning paths to strings.\nIt also supports drag and drop from the project folder.",
                        PreviewObject = new FilePathExamples(),
                        Code = "public class FilePathExamples\n{\n    // By default, FolderPath provides a path relative to the Unity project.\n    [FilePath]\n    public string UnityProjectPath;\n\n    // It is possible to provide custom parent path. Parent paths can be relative to the Unity project, or absolute.\n    [FilePath(ParentFolder = \"Assets/Plugins/Sirenix\")]\n    public string RelativeToParentPath;\n\n    // Using parent path, FilePath can also provide a path relative to a resources folder.\n    [FilePath(ParentFolder = \"Assets/Resources\")]\n    public string ResourcePath;\n\n    // Provide a comma seperated list of allowed extensions. Dots are optional.\n    [FilePath(Extensions = \"cs\")]\n    [BoxGroup(\"Conditions\")]\n    public string ScriptFiles;\n\n    // By setting AbsolutePath to true, the FilePath will provide an absolute path instead.\n    [FilePath(AbsolutePath = true)]\n    [BoxGroup(\"Conditions\")]\n    public string AbsolutePath;\n\n    // FilePath can also be configured to show an error, if the provided path is invalid.\n    [FilePath(RequireExistingPath = true)]\n    [BoxGroup(\"Conditions\")]\n    public string ExistingPath;\n\n    // By default, FilePath will enforce the use of forward slashes. It can also be configured to use backslashes instead.\n    [FilePath(UseBackslashes = true)]\n    [BoxGroup(\"Conditions\")]\n    public string Backslashes;\n\n    // FilePath also supports member references with the $ symbol.\n    [FilePath(ParentFolder = \"$DynamicParent\", Extensions = \"$DynamicExtensions\")]\n    [BoxGroup(\"Member referencing\")]\n    public string DynamicFilePath;\n\n    [BoxGroup(\"Member referencing\")]\n    public string DynamicParent = \"Assets/Plugin/Sirenix\";\n\n    [BoxGroup(\"Member referencing\")]\n    public string DynamicExtensions = \"cs, unity, jpg\";\n\n    // FilePath also supports lists and arrays.\n    [FilePath(ParentFolder = \"Assets/Plugins/Sirenix/Demos/Odin Inspector\")]\n    [BoxGroup(\"Lists\")]\n    public string[] ListOfFiles;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.FolderPathAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(FolderPathExamples),
                        Name = "Folder Path Examples",
                        Description = "FolderPath attribute provides a neat interface for assigning paths to strings.\nIt also supports drag and drop from the project folder.",
                        PreviewObject = new FolderPathExamples(),
                        Code = "public class FolderPathExamples\n{\n    // By default, FolderPath provides a path relative to the Unity project.\n    [FolderPath]\n    public string UnityProjectPath;\n\n    // It is possible to provide custom parent path. Parent paths can be relative to the Unity project, or absolute.\n    [FolderPath(ParentFolder = \"Assets/Plugins/Sirenix\")]\n    public string RelativeToParentPath;\n\n    // Using parent path, FolderPath can also provide a path relative to a resources folder.\n    [FolderPath(ParentFolder = \"Assets/Resources\")]\n    public string ResourcePath;\n\n    // By setting AbsolutePath to true, the FolderPath will provide an absolute path instead.\n    [FolderPath(AbsolutePath = true)]\n    [BoxGroup(\"Conditions\")]\n    public string AbsolutePath;\n\n    // FolderPath can also be configured to show an error, if the provided path is invalid.\n    [FolderPath(RequireExistingPath = true)]\n    [BoxGroup(\"Conditions\")]\n    public string ExistingPath;\n\n    // By default, FolderPath will enforce the use of forward slashes. It can also be configured to use backslashes instead.\n    [FolderPath(UseBackslashes = true)]\n    [BoxGroup(\"Conditions\")]\n    public string Backslashes;\n\n    // FolderPath also supports member references with the $ symbol.\n    [FolderPath(ParentFolder = \"$DynamicParent\")]\n    [BoxGroup(\"Member referencing\")]\n    public string DynamicFolderPath;\n\n    [BoxGroup(\"Member referencing\")]\n    public string DynamicParent = \"Assets/Plugins/Sirenix\";\n\n    // FolderPath also supports lists and arrays.\n    [FolderPath(ParentFolder = \"Assets/Plugins/Sirenix\")]\n    [BoxGroup(\"Lists\")]\n    public string[] ListOfFolders;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.FoldoutGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(FoldoutGroupAttributeExamples),
                        Name = "Foldout Group Attribute Examples",
                        Description = null,
                        PreviewObject = new FoldoutGroupAttributeExamples(),
                        Code = "public class FoldoutGroupAttributeExamples\n{\n    [FoldoutGroup(\"Group 1\")]\n    public int A;\n\n    [FoldoutGroup(\"Group 1\")]\n    public int B;\n\n    [FoldoutGroup(\"Group 1\")]\n    public int C;\n\n    [FoldoutGroup(\"Collapsed group\", expanded: false)]\n    public int D;\n\n    [FoldoutGroup(\"Collapsed group\")]\n    public int E;\n\n    [FoldoutGroup(\"$GroupTitle\", expanded: true)]\n    public int One;\n\n    [FoldoutGroup(\"$GroupTitle\")]\n    public int Two;\n\n    public string GroupTitle = \"Dynamic group title\";\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.GUIColorAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(GUIColorExamples),
                        Name = "GUIColor Examples",
                        Description = null,
                        PreviewObject = new GUIColorExamples(),
                        Code = "public class GUIColorExamples\n{\n    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]\n    public int ColoredInt1;\n\n    [GUIColor(0.3f, 0.8f, 0.8f, 1f)]\n    public int ColoredInt2;\n\n    [ButtonGroup]\n    [GUIColor(0, 1, 0)]\n    private void Apply()\n    {\n    }\n\n    [ButtonGroup]\n    [GUIColor(1, 0.6f, 0.4f)]\n    private void Cancel()\n    {\n    }\n\n    [InfoBox(\"You can also reference a color member to dynamically change the color of a property.\")]\n    [GUIColor(\"GetButtonColor\")]\n    [Button(ButtonSizes.Gigantic)]\n    private static void IAmFabulous()\n    {\n    }\n\n    private static Color GetButtonColor()\n    {\n        Utilities.Editor.GUIHelper.RequestRepaint();\n        return Color.HSVToRGB(Mathf.Cos((float)UnityEditor.EditorApplication.timeSinceStartup + 1f) * 0.225f + 0.325f, 1, 1);\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInEditorModeAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(HideInEditorModeExamples),
                        Name = "Hide In Editor Mode Examples",
                        Description = null,
                        PreviewObject = new HideInEditorModeExamples(),
                        Code = "public class HideInEditorModeExamples\n{\n    [Title(\"Hidden in editor mode\")]\n    [HideInEditorMode]\n    public int C;\n\n    [HideInEditorMode]\n    public int D;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInPlayModeAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(HideInPlayModeExamples),
                        Name = "Hide In Play Mode Examples",
                        Description = null,
                        PreviewObject = new HideInPlayModeExamples(),
                        Code = "public class HideInPlayModeExamples\n{\n    [Title(\"Hidden in play mode\")]\n    [HideInPlayMode]\n    public int A;\n\n    [HideInPlayMode]\n    public int B;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideLabelAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(HideLabelExamples),
                        Name = "Hide Label Examples",
                        Description = null,
                        PreviewObject = new HideLabelExamples(),
                        Code = "public class HideLabelExamples\n{\n    [Title(\"Wide Colors\")]\n    [HideLabel]\n    [ColorPalette(\"Fall\")]\n    public Color WideColor1;\n\n    [HideLabel]\n    [ColorPalette(\"Fall\")]\n    public Color WideColor2;\n\n    [Title(\"Wide Vector\")]\n    [HideLabel]\n    public Vector3 WideVector1;\n\n    [HideLabel]\n    public Vector4 WideVector2;\n\n    [Title(\"Wide String\")]\n    [HideLabel]\n    public string WideString;\n\n    [Title(\"Wide Multiline Text Field\")]\n    [HideLabel]\n    [MultiLineProperty]\n    public string WideMultilineTextField = \"\";\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideReferenceObjectPickerAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(HideReferenceObjectPickerExamples),
                        Name = "Hide Reference Object Picker Examples",
                        Description = "When the object picker is hidden, you can right click and set the instance to null, in order to set a new value.\n\nIf you don't want this behavior, you can use DisableContextMenu attribute to ensure people can't change the value.",
                        PreviewObject = new HideReferenceObjectPickerExamples(),
                        Code = "public class HideReferenceObjectPickerExamples\n{\n    [Title(\"Hidden Object Pickers\")]\n    [HideReferenceObjectPicker]\n    public MyCustomReferenceType OdinSerializedProperty1;\n\n    [HideReferenceObjectPicker]\n    public MyCustomReferenceType OdinSerializedProperty2;\n\n    [Title(\"Shown Object Pickers\")]\n    public MyCustomReferenceType OdinSerializedProperty3;\n\n    [InfoBox(\"Protip: You can also put the HideInInspector attribute on the class definition itself to hide it globally for all members.\")]\n    public MyCustomReferenceType OdinSerializedProperty4;\n\n    // [HideReferenceObjectPicker]\n    public class MyCustomReferenceType\n    {\n        public int A;\n        public int B;\n        public int C;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HorizontalGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(HorizontalGroupAttributeExamples),
                        Name = "Horizontal Group Attribute Examples",
                        Description = null,
                        PreviewObject = new HorizontalGroupAttributeExamples(),
                        Code = "public class HorizontalGroupAttributeExamples\n{\n    [HorizontalGroup]\n    public int A;\n\n    [HideLabel, LabelWidth (150)]\n    [HorizontalGroup(150)]\n    public LayerMask B;\n\n    // LabelWidth can be helpfull when dealing with HorizontalGroups.\n    [HorizontalGroup(\"Group 1\", LabelWidth = 20)]\n    public int C;\n\n    [HorizontalGroup(\"Group 1\")]\n    public int D;\n\n    [HorizontalGroup(\"Group 1\")]\n    public int E;\n\n    // Having multiple properties in a column can be achived using multiple groups. Checkout the \"Combining Group Attributes\" example.\n    [HorizontalGroup(\"Split\", 0.5f, LabelWidth = 20)]\n    [BoxGroup(\"Split/Left\")]\n    public int L;\n\n    [BoxGroup(\"Split/Right\")]\n    public int M;\n\n    [BoxGroup(\"Split/Left\")]\n    public int N;\n\n    [BoxGroup(\"Split/Right\")]\n    public int O;\n\n    // Horizontal Group also has supprot for: Title, MarginLeft, MarginRight, PaddingLeft, PaddingRight, MinWidth and MaxWidth.\n    [HorizontalGroup(\"MyButton\", MarginLeft = 0.25f, MarginRight = 0.25f)]\n    public void SomeButton()\n    {\n        // ...\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.IndentAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(IndentExamples),
                        Name = "Indent Examples",
                        Description = null,
                        PreviewObject = new IndentExamples(),
                        Code = "public class IndentExamples\n{\n    [Title(\"Nicely organize your properties.\")]\n    [Indent]\n    public int A;\n\n    [Indent(2)]\n    public int B;\n\n    [Indent(3)]\n    public int C;\n\n    [Indent(4)]\n    public int D;\n\n    [Title(\"Using the Indent attribute\")]\n    [Indent]\n    public int E;\n\n    [Indent(0)]\n    public int F;\n\n    [Indent(-1)]\n    public int G;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.InfoBoxAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(InfoBoxExamples),
                        Name = "Info Box Examples",
                        Description = "This example demonstrates the use of the InfoBox attribute.\nAny info box with a warning or error drawn in the inspector will also be found by the Scene Validation tool.",
                        PreviewObject = new InfoBoxExamples(),
                        Code = "public class InfoBoxExamples\n{\n    [Title(\"InfoBox message types\")]\n    [InfoBox(\"Default info box.\")]\n    public int A;\n\n    [InfoBox(\"Warning info box.\", InfoMessageType.Warning)]\n    public int B;\n\n    [InfoBox(\"Error info box.\", InfoMessageType.Error)]\n    public int C;\n\n    [InfoBox(\"Info box without an icon.\", InfoMessageType.None)]\n    public int D;\n\n    [Title(\"Conditional info boxes\")]\n    public bool ToggleInfoBoxes;\n\n    [InfoBox(\"This info box is only shown while in editor mode.\", InfoMessageType.Error, \"IsInEditMode\")]\n    public float G;\n\n    [InfoBox(\"This info box is hideable by a static field.\", \"ToggleInfoBoxes\")]\n    public float E;\n\n    [InfoBox(\"This info box is hideable by a static field.\", \"ToggleInfoBoxes\")]\n    public float F;\n\n    [Title(\"Info box member reference\")]\n    [InfoBox(\"$InfoBoxMessage\")]\n    public string InfoBoxMessage = \"My dynamic info box message\";\n\n    private static bool IsInEditMode()\n    {\n        return !Application.isPlaying;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.InlineButtonAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(InlineButtonExamples),
                        Name = "Inline Button Examples",
                        Description = null,
                        PreviewObject = new InlineButtonExamples(),
                        Code = "public class InlineButtonExamples\n{\n    // Inline Buttons:\n    [InlineButton(\"A\")]\n    public int InlineButton;\n\n    [InlineButton(\"A\")]\n    [InlineButton(\"B\", \"Custom Button Name\")]\n    public int ChainedButtons;\n\n    private void A()\n    {\n        Debug.Log(\"A\");\n    }\n\n    private void B()\n    {\n        Debug.Log(\"B\");\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.InlineEditorAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(InlineEditorExamples),
                        Name = "Inline Editor Examples",
                        Description = null,
                        PreviewObject = new InlineEditorExamples(),
                        Code = "public class InlineEditorExamples\n{\n    [DisableInInlineEditors]\n    public Vector3 DisabledInInlineEditors;\n\n    [HideInInlineEditors]\n    public Vector3 HiddenInInlineEditors;\n\n    [InlineEditor]\n    public InlineEditorExamples Self;\n\n    [InlineEditor]\n    public Transform InlineComponent;\n\n    [InlineEditor(InlineEditorModes.FullEditor)]\n    public Material FullInlineEditor;\n\n    [InlineEditor(InlineEditorModes.GUIAndHeader)]\n    public Material InlineMaterial;\n\n    [InlineEditor(InlineEditorModes.SmallPreview)]\n    public Material[] InlineMaterialList;\n\n    [InlineEditor(InlineEditorModes.LargePreview)]\n    public Mesh InlineMeshPreview;\n\n    [Header(\"Boxed / Default\")]\n    [InlineEditor(InlineEditorObjectFieldModes.Boxed)]\n    public Transform Boxed;\n\n    [Header(\"Foldout\")]\n    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]\n    public MinMaxSliderExamples Foldout;\n\n    [Header(\"Hide ObjectField\")]\n    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]\n    public Transform CompletelyHidden;\n\n    [Header(\"Show ObjectField if null\")]\n    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]\n    public Transform OnlyHiddenWhenNotNull;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.InlinePropertyAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(InlinePropertyExamples),
                        Name = "Inline Property Examples",
                        Description = null,
                        PreviewObject = new InlinePropertyExamples(),
                        Code = "public class InlinePropertyExamples\n{\n    public Vector3 Vector3;\n\n    public Vector3Int MyVector3Int;\n\n    [InlineProperty(LabelWidth = 13)]\n    public Vector2Int MyVector2Int;\n\n    [Serializable]\n    [InlineProperty(LabelWidth = 13)]\n    public struct Vector3Int\n    {\n        [HorizontalGroup]\n        public int X;\n\n        [HorizontalGroup]\n        public int Y;\n\n        [HorizontalGroup]\n        public int Z;\n    }\n\n    [Serializable]\n    public struct Vector2Int\n    {\n        [HorizontalGroup]\n        public int X;\n\n        [HorizontalGroup]\n        public int Y;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.LabelTextAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(LabelTextExamples),
                        Name = "Label Text Examples",
                        Description = "Specify a different label text for your properties.",
                        PreviewObject = new LabelTextExamples(),
                        Code = "public class LabelTextExamples\n{\n    [LabelText(\"1\")]\n    public int MyInt1;\n\n    [LabelText(\"2\")]\n    public int MyInt2;\n\n    [LabelText(\"3\")]\n    public int MyInt3;\n\n    [InfoBox(\"Use $ to refer to a member string.\")]\n    [LabelText(\"$MyInt3\")]\n    public string LabelText = \"Dynamic label text\";\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ListDrawerSettingsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ListExamples),
                        Name = "List Examples",
                        Description = null,
                        PreviewObject = new ListExamples(),
                        Code = "public class ListExamples\n{\n    [PropertyOrder(int.MinValue), OnInspectorGUI]\n    private void DrawIntroInfoBox()\n    {\n        SirenixEditorGUI.InfoMessageBox(\"Out of the box, Odin significantly upgrades the drawing of lists and arrays in the inspector - across the board, without you ever lifting a finger.\");\n    }\n\n    [Title(\"List Basics\")]\n    [InfoBox(\"List elements can now be dragged around to reorder them and deleted individually, and lists have paging (try adding a lot of elements!). You can still drag many assets at once into lists from the project view - just drag them into the list itself and insert them where you want to add them.\")]\n    public List<float> FloatList;\n\n    [InfoBox(\"Applying a [Range] attribute to this list instead applies it to all of its float entries.\")]\n    [Range(0, 1)]\n    public float[] FloatRangeArray;\n\n    [InfoBox(\"Lists can be made read-only in different ways.\")]\n    [ListDrawerSettings(IsReadOnly = true)]\n    public int[] ReadOnlyArray1 = new int[] { 1, 2, 3 };\n\n    [ReadOnly]\n    public int[] ReadOnlyArray2 = new int[] { 1, 2, 3 };\n\n    public SomeOtherStruct[] SomeStructList;\n\n    [Title(\"Advanced List Customization\")]\n    [InfoBox(\"Using [ListDrawerSettings], lists can be customized in a wide variety of ways.\")]\n    [ListDrawerSettings(NumberOfItemsPerPage = 5)]\n    public int[] FiveItemsPerPage;\n\n    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = \"SomeString\")]\n    public SomeStruct[] IndexLabels;\n\n    [ListDrawerSettings(DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = false, HideRemoveButton = true)]\n    public int[] MoreListSettings = new int[] { 1, 2, 3 };\n\n    [ListDrawerSettings(OnBeginListElementGUI = \"BeginDrawListElement\", OnEndListElementGUI = \"EndDrawListElement\")]\n    public SomeStruct[] InjectListElementGUI;\n\n    [ListDrawerSettings(OnTitleBarGUI = \"DrawRefreshButton\")]\n    public List<int> CustomButtons;\n\n    [ListDrawerSettings(CustomAddFunction = \"CustomAddFunction\")]\n    public List<int> CustomAddBehaviour;\n\n    private void BeginDrawListElement(int index)\n    {\n        SirenixEditorGUI.BeginBox(this.InjectListElementGUI[index].SomeString);\n    }\n\n    private void EndDrawListElement(int index)\n    {\n        SirenixEditorGUI.EndBox();\n    }\n\n    private void DrawRefreshButton()\n    {\n        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))\n        {\n            Debug.Log(this.CustomButtons.Count.ToString());\n        }\n    }\n\n    private int CustomAddFunction()\n    {\n        return this.CustomAddBehaviour.Count;\n    }\n\n    [Serializable]\n    public struct SomeStruct\n    {\n        public string SomeString;\n        public int One;\n        public int Two;\n        public int Three;\n    }\n\n    [Serializable]\n    public struct SomeOtherStruct\n    {\n        [HorizontalGroup(\"Split\", 55), PropertyOrder(-1)]\n        [PreviewField(50, OdinInspector.ObjectFieldAlignment.Left), HideLabel]\n        public UnityEngine.MonoBehaviour SomeObject;\n\n        [FoldoutGroup(\"Split/$Name\", false)]\n        public int A, B, C;\n\n        [FoldoutGroup(\"Split/$Name\", false)]\n        public int Two;\n\n        [FoldoutGroup(\"Split/$Name\", false)]\n        public int Three;\n\n        private string Name { get { return this.SomeObject ? this.SomeObject.name : \"Null\"; } }\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.MinMaxSliderAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(MinMaxSliderExamples),
                        Name = "Min Max Slider Examples",
                        Description = "Uses a Vector2 where x is the min knob and y is the max knob.",
                        PreviewObject = new MinMaxSliderExamples(),
                        Code = "public class MinMaxSliderExamples\n{\n    [MinMaxSlider(-10, 10)]\n    public Vector2 MinMaxValueSlider;\n\n    [MinMaxSlider(-10, 10, true)]\n    public Vector2 WithFields;\n\n    [InfoBox(\"You can also assign the min max values dynamically by refering to members.\")]\n    [MinMaxSlider(\"DynamicRange\", true)]\n    public Vector2 DynamicMinMax;\n\n    [MinMaxSlider(\"Min\", 10f, true)]\n    public Vector2 DynamicMin;\n\n    [MinMaxSlider(0f, \"Max\", true)]\n    public Vector2 DynamicMax;\n\n    public Vector2 DynamicRange;\n\n    public float Min { get { return this.DynamicRange.x; } }\n\n    public float Max { get { return this.DynamicRange.y; } }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.MaxValueAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(MinMaxValueValueExamples),
                        Name = "Min Max Value Value Examples",
                        Description = null,
                        PreviewObject = new MinMaxValueValueExamples(),
                        Code = "public class MinMaxValueValueExamples\n{\n    // Ints\n    [Title(\"Int\")]\n    [MinValue(0)]\n    public int IntMinValue0;\n\n    [MaxValue(0)]\n    public int IntMaxValue0;\n\n    // Floats\n    [Title(\"Float\")]\n    [MinValue(0)]\n    public float FloatMinValue0;\n\n    [MaxValue(0)]\n    public float FloatMaxValue0;\n\n    // Vectors\n    [Title(\"Vectors\")]\n    [MinValue(0)]\n    public Vector3 Vector3MinValue0;\n\n    [MaxValue(0)]\n    public Vector3 Vector3MaxValue0;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.MinValueAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(MinMaxValueValueExamples),
                        Name = "Min Max Value Value Examples",
                        Description = null,
                        PreviewObject = new MinMaxValueValueExamples(),
                        Code = "public class MinMaxValueValueExamples\n{\n    // Ints\n    [Title(\"Int\")]\n    [MinValue(0)]\n    public int IntMinValue0;\n\n    [MaxValue(0)]\n    public int IntMaxValue0;\n\n    // Floats\n    [Title(\"Float\")]\n    [MinValue(0)]\n    public float FloatMinValue0;\n\n    [MaxValue(0)]\n    public float FloatMaxValue0;\n\n    // Vectors\n    [Title(\"Vectors\")]\n    [MinValue(0)]\n    public Vector3 Vector3MinValue0;\n\n    [MaxValue(0)]\n    public Vector3 Vector3MaxValue0;\n}",
                    },
                }
            },
            {
                typeof(UnityEngine.MultilineAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(MultiLinePropertyExamples),
                        Name = "Multi Line Property Examples",
                        Description = null,
                        PreviewObject = new MultiLinePropertyExamples(),
                        Code = "public class MultiLinePropertyExamples\n{\n    [Multiline(10)]\n    public string UnityMultilineField = \"\";\n\n    [Title(\"Wide Multiline Text Field\", bold: false)]\n    [HideLabel]\n    [MultiLineProperty(10)]\n    public string WideMultilineTextField = \"\";\n\n    [InfoBox(\"Odin supports properties, but Unity's own Multiline attribute only works on fields.\")]\n    [ShowInInspector]\n    [MultiLineProperty(10)]\n    public string OdinMultilineProperty { get; set; }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.OnInspectorGUIAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(OnInspectorGUIExamples),
                        Name = "On Inspector GUIExamples",
                        Description = null,
                        PreviewObject = new OnInspectorGUIExamples(),
                        Code = "public class OnInspectorGUIExamples\n{\n    [OnInspectorGUI(\"DrawPreview\", append: true)]\n    public Texture2D Texture;\n\n    private void DrawPreview()\n    {\n        if (this.Texture == null) return;\n\n        GUILayout.BeginVertical(GUI.skin.box);\n        GUILayout.Label(this.Texture);\n        GUILayout.EndVertical();\n    }\n\n    [OnInspectorGUI]\n    private void OnInspectorGUI()\n    {\n        UnityEditor.EditorGUILayout.HelpBox(\"OnInspectorGUI can also be used on both methods and properties\", UnityEditor.MessageType.Info);\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInNonPrefabsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInPrefabsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInPrefabInstancesAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.DisableInPrefabAssetsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInNonPrefabsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInPrefabsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInPrefabInstancesAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideInPrefabAssetsAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PrefabRelatedAttributesExamples),
                        Name = "Prefab Related Attributes Examples",
                        Description = null,
                        PreviewObject = new PrefabRelatedAttributesExamples(),
                        Code = "public class PrefabRelatedAttributesExamples\n{\n    [HideInPrefabAssets]\n    public GameObject HiddenInPrefabAssets;\n\n    [HideInPrefabInstances]\n    public GameObject HiddenInPrefabInstances;\n\n    [HideInPrefabs]\n    public GameObject HiddenInPrefabs;\n\n    [HideInNonPrefabs]\n    public GameObject HiddenInNonPrefabs;\n\n    [DisableInPrefabAssets]\n    public GameObject DisabledInPrefabAssets;\n\n    [DisableInPrefabInstances]\n    public GameObject DisabledInPrefabInstances;\n\n    [DisableInPrefabs]\n    public GameObject DisabledInPrefabs;\n\n    [DisableInNonPrefabs]\n    public GameObject DisabledInNonPrefabs;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.PreviewFieldAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PreviewFieldsExamples),
                        Name = "Preview Fields Examples",
                        Description = null,
                        PreviewObject = new PreviewFieldsExamples(),
                        Code = "public class PreviewFieldsExamples\n{\n    [PreviewField]\n    public Object RegularPreviewField;\n\n    [VerticalGroup(\"row1/left\")]\n    public string A, B, C;\n\n    [HideLabel]\n    [PreviewField(50, ObjectFieldAlignment.Right)]\n    [HorizontalGroup(\"row1\", 50), VerticalGroup(\"row1/right\")]\n    public Object D;\n\n    [HideLabel]\n    [PreviewField(50, ObjectFieldAlignment.Left)]\n    [HorizontalGroup(\"row2\", 50), VerticalGroup(\"row2/left\")]\n    public Object E;\n\n    [VerticalGroup(\"row2/right\"), LabelWidth(-54)]\n    public string F, G, H;\n\n    [InfoBox(\n        \"These object fields can also be selectively enabled and customized globally \" +\n        \"from the Odin preferences window.\\n\\n\" +\n        \" - Hold Ctrl + Click = Delete Instance\\n\" +\n        \" - Drag and drop = Move / Swap.\\n\" +\n        \" - Ctrl + Drag = Replace.\\n\" +\n        \" - Ctrl + drag and drop = Move and override.\")]\n    [PropertyOrder(-1)]\n    [Button(ButtonSizes.Large)]\n    private void ConfigureGlobalPreviewFieldSettings()\n    {\n        Sirenix.OdinInspector.Editor.GeneralDrawerConfig.Instance.OpenInEditor();   \n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ProgressBarAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ProgressBarExamples),
                        Name = "Progress Bar Examples",
                        Description = "The ProgressBar attribute draws a horizontal colored bar, which can also be clicked to change the value.\n\nIt can be used to show how full an inventory might be, or to make a visual indicator for a healthbar. It can even be used to make fighting game style health bars, that stack multiple layers of health.",
                        PreviewObject = new ProgressBarExamples(),
                        Code = "public class ProgressBarExamples\n{\n    [ProgressBar(0, 100)]\n    public int ProgressBar = 50;\n\n    [HideLabel]\n    [ProgressBar(-100, 100, r: 1, g: 1, b: 1, Height = 30)]\n    public short BigColoredProgressBar = 50;\n\n    [ProgressBar(0, 10, 0, 1, 0, Segmented = true)]\n    public int SegmentedColoredBar = 5;\n\n    [ProgressBar(0, 100, ColorMember = \"GetHealthBarColor\")]\n    public float DynamicHealthBarColor = 50;\n\n    [BoxGroup(\"Dynamic Range\")]\n    [ProgressBar(\"Min\", \"Max\")]\n    public float DynamicProgressBar = 50;\n    [BoxGroup(\"Dynamic Range\")]\n    public float Min;\n    [BoxGroup(\"Dynamic Range\")]\n    public float Max = 100;\n\n    [Range(0, 300)]\n    [BoxGroup(\"Stacked\")]\n    public float StackedHealth;\n\n    [HideLabel, ShowInInspector]\n    [ProgressBar(0, 100, ColorMember = \"GetStackedHealthColor\", BackgroundColorMember = \"GetStackHealthBackgroundColor\", DrawValueLabel = false)]\n    [BoxGroup(\"Stacked\")]\n    private float StackedHealthProgressBar\n    {\n        get { return this.StackedHealth % 100.01f; }\n    }\n\n    private Color GetHealthBarColor(float value)\n    {\n        return Color.Lerp(Color.red, Color.green, Mathf.Pow(value / 100f, 2));\n    }\n\n    private Color GetStackedHealthColor()\n    {\n        return\n            this.StackedHealth > 200 ? Color.white :\n            this.StackedHealth > 100 ? Color.green :\n            Color.red;\n    }\n\n    private Color GetStackHealthBackgroundColor()\n    {\n        return\n            this.StackedHealth > 200 ? Color.green :\n            this.StackedHealth > 100 ? Color.red :\n            new Color(0.16f, 0.16f, 0.16f, 1f);\n    }\n\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.PropertyOrderAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PropertyOrderExamples),
                        Name = "Property Order Examples",
                        Description = null,
                        PreviewObject = new PropertyOrderExamples(),
                        Code = "public class PropertyOrderExamples\n{\n    [PropertyOrder(1)]\n    public int Second;\n\n    [InfoBox(\"PropertyOrder is used to change the order of properties in the inspector.\")]\n    [PropertyOrder(-1)]\n    public int First;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.PropertyTooltipAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(PropertyTooltipExamples),
                        Name = "Property Tooltip Examples",
                        Description = "PropertyTooltip is used to add tooltips to properties in the inspector.\nPropertyTooltip can also be applied to properties and methods, unlike Unity's Tooltip attribute.",
                        PreviewObject = new PropertyTooltipExamples(),
                        Code = "public class PropertyTooltipExamples\n{\n    [PropertyTooltip(\"This is tooltip on an int property.\")]\n    public int MyInt;\n\n    [InfoBox(\"Use $ to refer to a member string.\")]\n    [PropertyTooltip(\"$Tooltip\")]\n    public string Tooltip = \"Dynamic tooltip.\";\n\n    [Button, PropertyTooltip(\"Button Tooltip\")]\n    private void ButtonWithTooltip()\n    {\n        // ...\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ReadOnlyAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ReadOnlyExamples),
                        Name = "Read Only Examples",
                        Description = "ReadOnly disables properties in the inspector.",
                        PreviewObject = new ReadOnlyExamples(),
                        Code = "public class ReadOnlyExamples\n{\n    [ReadOnly]\n    public string MyString = \"This is displayed as text\";\n\n    [ReadOnly]\n    public int MyInt = 9001;\n\n    [ReadOnly]\n    public int[] MyIntList = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.RequiredAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(RequiredExamples),
                        Name = "Required Examples",
                        Description = "Required displays an error when objects are missing.",
                        PreviewObject = new RequiredExamples(),
                        //Code = "public class RequiredExamples\n{\n    [Required]\n    public GameObject MyGameObject;\n\n    [Required(\"Custom error message.\")]\n    public Rigidbody MyRigidbody;\n\n    [InfoBox(\"Use $ to indicate a member string as message.\")]\n    [Required(\"$DynamicMessage\")]\n    public GameObject GameObject;\n\n    public string DynamicMessage = \"Dynamic error message\";\n}",
                        Code = "<color=#3987D6FF>public</color> <color=#3987D6FF>class</color> <color=#4EC9B0FF>RequiredExamples</color>\n{\n    [<color=#4EC9B0FF>Required</color>]\n    <color=#3987D6FF>public</color> <color=#4EC9B0FF>GameObject</color> MyGameObject;\n\n    [<color=#4EC9B0FF>Required</color>(<color=#D69D85FF>\"Custom error message.\"</color>)]\n    <color=#3987D6FF>public</color> <color=#4EC9B0FF>Rigidbody</color> MyRigidbody;\n\n    <color=#57A64AFF>// Use $ to indicate a member string as message.</color>\n    [<color=#4EC9B0FF>Required</color>(<color=#D69D85FF>\"$DynamicMessage\"</color>)]\n    <color=#3987D6FF>public</color> <color=#4EC9B0FF>GameObject</color> GameObject;\n\n    <color=#3987D6FF>public</color> <color=#3987D6FF>string</color> DynamicMessage = <color=#D69D85FF>\"Dynamic error message\"</color>;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ResponsiveButtonGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ResponsiveButtonGroupExample),
                        Name = "Responsive Button Group Example",
                        Description = null,
                        PreviewObject = new ResponsiveButtonGroupExample(),
                        Code = "public class ResponsiveButtonGroupExample\n{\n    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]\n    private void OpenDockableWindowExample()\n    {\n        var window = UnityEditor.EditorWindow.GetWindow<MyDockableGameDashboard>();\n        window.WindowPadding = new Vector4();\n    }\n\n    [OnInspectorGUI] private void Space1() { GUILayout.Space(20); }\n\n    [ResponsiveButtonGroup] public void Foo() { }\n    [ResponsiveButtonGroup] public void Bar() { }\n    [ResponsiveButtonGroup] public void Baz() { }\n\n    [OnInspectorGUI] private void Space2() { GUILayout.Space(20); }\n\n    [ResponsiveButtonGroup(\"UniformGroup\", UniformLayout = true)] public void Foo1() { }\n    [ResponsiveButtonGroup(\"UniformGroup\")]                       public void Foo2() { }\n    [ResponsiveButtonGroup(\"UniformGroup\")]                       public void LongesNameWins() { }\n    [ResponsiveButtonGroup(\"UniformGroup\")]                       public void Foo4() { }\n    [ResponsiveButtonGroup(\"UniformGroup\")]                       public void Foo5() { }\n    [ResponsiveButtonGroup(\"UniformGroup\")]                       public void Foo6() { }\n\n    [OnInspectorGUI] private void Space3() { GUILayout.Space(20); }\n\n    [ResponsiveButtonGroup(\"DefaultButtonSize\", DefaultButtonSize = ButtonSizes.Small)] public void Bar1() { }\n    [ResponsiveButtonGroup(\"DefaultButtonSize\")]                                        public void Bar2() { }\n    [ResponsiveButtonGroup(\"DefaultButtonSize\")]                                        public void Bar3() { }\n    [Button(ButtonSizes.Large), ResponsiveButtonGroup(\"DefaultButtonSize\")]             public void Bar4() { }\n    [Button(ButtonSizes.Large), ResponsiveButtonGroup(\"DefaultButtonSize\")]             public void Bar5() { }\n    [ResponsiveButtonGroup(\"DefaultButtonSize\")]                                        public void Bar6() { }\n\n    [OnInspectorGUI] private void Space4() { GUILayout.Space(20); }\n\n    [FoldoutGroup(\"SomeOtherGroup\")]\n    [ResponsiveButtonGroup(\"SomeOtherGroup/SomeBtnGroup\")] public void Baz1() { }\n    [ResponsiveButtonGroup(\"SomeOtherGroup/SomeBtnGroup\")] public void Baz2() { }\n    [ResponsiveButtonGroup(\"SomeOtherGroup/SomeBtnGroup\")] public void Baz3() { }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.SceneObjectsOnlyAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(SceneAndAssetsOnlyExamples),
                        Name = "Scene And Assets Only Examples",
                        Description = null,
                        PreviewObject = new SceneAndAssetsOnlyExamples(),
                        Code = "public class SceneAndAssetsOnlyExamples\n{\n    [Title(\"Assets only\")]\n    [AssetsOnly]\n    public List<GameObject> OnlyPrefabs;\n\n    [AssetsOnly]\n    public GameObject SomePrefab;\n\n    [AssetsOnly]\n    public Material MaterialAsset;\n\n    [AssetsOnly]\n    public MeshRenderer SomeMeshRendererOnPrefab;\n\n    [Title(\"Scene Objects only\")]\n    [SceneObjectsOnly]\n    public List<GameObject> OnlySceneObjects;\n\n    [SceneObjectsOnly]\n    public GameObject SomeSceneObject;\n\n    [SceneObjectsOnly]\n    public MeshRenderer SomeMeshRenderer;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.AssetsOnlyAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(SceneAndAssetsOnlyExamples),
                        Name = "Scene And Assets Only Examples",
                        Description = null,
                        PreviewObject = new SceneAndAssetsOnlyExamples(),
                        Code = "public class SceneAndAssetsOnlyExamples\n{\n    [Title(\"Assets only\")]\n    [AssetsOnly]\n    public List<GameObject> OnlyPrefabs;\n\n    [AssetsOnly]\n    public GameObject SomePrefab;\n\n    [AssetsOnly]\n    public Material MaterialAsset;\n\n    [AssetsOnly]\n    public MeshRenderer SomeMeshRendererOnPrefab;\n\n    [Title(\"Scene Objects only\")]\n    [SceneObjectsOnly]\n    public List<GameObject> OnlySceneObjects;\n\n    [SceneObjectsOnly]\n    public GameObject SomeSceneObject;\n\n    [SceneObjectsOnly]\n    public MeshRenderer SomeMeshRenderer;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.HideIfAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ShowAndHideIfExamples),
                        Name = "Show And Hide If Examples",
                        Description = null,
                        PreviewObject = new ShowAndHideIfExamples(),
                        Code = "public class ShowAndHideIfExamples\n{\n    public UnityEngine.Object SomeObject;\n\n    [EnumToggleButtons]\n    public InfoMessageType SomeEnum;\n\n    public bool IsToggled;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Info)]\n    public Vector3 Info;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Warning)]\n    public Vector2 Warning;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Error)]\n    public Vector3 Error;\n\n    [ShowIf(\"IsToggled\")]\n    public Vector2 VisibleWhenToggled;\n\n    [HideIf(\"IsToggled\")]\n    public Vector3 HiddenWhenToggled;\n\n    [HideIf(\"SomeObject\")]\n    public Vector3 ShowWhenNull;\n\n    [ShowIf(\"SomeObject\")]\n    public Vector3 HideWhenNull;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ShowIfAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ShowAndHideIfExamples),
                        Name = "Show And Hide If Examples",
                        Description = null,
                        PreviewObject = new ShowAndHideIfExamples(),
                        Code = "public class ShowAndHideIfExamples\n{\n    public UnityEngine.Object SomeObject;\n\n    [EnumToggleButtons]\n    public InfoMessageType SomeEnum;\n\n    public bool IsToggled;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Info)]\n    public Vector3 Info;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Warning)]\n    public Vector2 Warning;\n\n    [ShowIf(\"SomeEnum\", InfoMessageType.Error)]\n    public Vector3 Error;\n\n    [ShowIf(\"IsToggled\")]\n    public Vector2 VisibleWhenToggled;\n\n    [HideIf(\"IsToggled\")]\n    public Vector3 HiddenWhenToggled;\n\n    [HideIf(\"SomeObject\")]\n    public Vector3 ShowWhenNull;\n\n    [ShowIf(\"SomeObject\")]\n    public Vector3 HideWhenNull;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ShowDrawerChainAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ShowDrawerChainExamples),
                        Name = "Show Drawer Chain Examples",
                        Description = null,
                        PreviewObject = new ShowDrawerChainExamples(),
                        Code = "public class ShowDrawerChainExamples\n{\n#if UNITY_EDITOR\n    [HorizontalGroup(PaddingRight = -1)]\n    [ShowInInspector, PropertyOrder(-1)]\n    public bool ToggleHideIf { get { Utilities.Editor.GUIHelper.RequestRepaint(); return UnityEditor.EditorApplication.timeSinceStartup % 3 < 1.5f; } }\n\n    [HorizontalGroup]\n    [ShowInInspector, HideLabel, ProgressBar(0, 1.5f)]\n    private double Animate { get { return Math.Abs(UnityEditor.EditorApplication.timeSinceStartup % 3 - 1.5f); } }\n#endif\n    [InfoBox(\n        \"Any drawer not used in the draw chain will be greyed out in the drawer chain so that you can more easily debug the draw chain. You can see this by toggling the above toggle field.\\n\\n\" +\n        \"If you have any custom drawers they will show up with green names in the drawer chain.\")]\n    [ShowDrawerChain]\n    [HideIf(\"ToggleHideIf\")]\n    public GameObject SomeObject;\n\n    [Range(0, 10)]\n    [ShowDrawerChain]\n    public float SomeRange;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ShowInInspectorAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ShowInInspectorExamples),
                        Name = "Show In Inspector Examples",
                        Description = "ShowInInspector is used to display properties that otherwise wouldn't be shown in the inspector. Such as non-serialized fields or properties.",
                        PreviewObject = new ShowInInspectorExamples(),
                        Code = "public class ShowInInspectorExamples\n{\n    [ShowInInspector]\n    private int myPrivateInt;\n\n    [ShowInInspector]\n    public int MyPropertyInt { get; set; }\n\n    [ShowInInspector]\n    public int ReadOnlyProperty\n    {\n        get { return this.myPrivateInt; }\n    }\n\n    [ShowInInspector]\n    public static bool StaticProperty { get; set; }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ShowPropertiesInTheInspectorExamples),
                        Name = "Show Properties In The Inspector Examples",
                        Description = null,
                        PreviewObject = new ShowPropertiesInTheInspectorExamples(),
                        Code = "public class ShowPropertiesInTheInspectorExamples\n{\n    [SerializeField, HideInInspector]\n    private int evenNumber;\n\n    [ShowInInspector]\n    public int EvenNumber\n    {\n        get { return this.evenNumber; }\n        set { this.evenNumber = value + value % 2; }\n    }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(StaticInspectorsExample),
                        Name = "Static Inspectors Example",
                        Description = "You can use the ShowInInspector attribute on static members to make them appear in the inspector as well.",
                        PreviewObject = new StaticInspectorsExample(),
                        Code = "public class StaticInspectorsExample\n{\n    [ShowInInspector]\n    public static List<MySomeStruct> SomeStaticField = new List<MySomeStruct>();\n\n    [ShowInInspector, PropertyRange(0, 0.1f)]\n    public static float FixedDeltaTime\n    {\n        get { return Time.fixedDeltaTime; }\n        set { Time.fixedDeltaTime = value; }\n    }\n\n    [Button(ButtonSizes.Large), PropertyOrder(-1)]\n    public static void AddToList()\n    {\n        int count = SomeStaticField.Count + 1000;\n        while (SomeStaticField.Count < count)\n        {\n            SomeStaticField.Add(new MySomeStruct());\n        }\n    }\n\n    [Serializable]\n    public struct MySomeStruct\n    {\n        [HideLabel, PreviewField(45)]\n        [HorizontalGroup(\"Split\", width: 45)]\n        public Texture2D Icon;\n\n        [FoldoutGroup(\"Split/$Icon\")]\n        [HorizontalGroup(\"Split/$Icon/Properties\", LabelWidth = 40)]\n        public int Foo;\n\n        [HorizontalGroup(\"Split/$Icon/Properties\")]\n        public int Bar;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.SuffixLabelAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(SuffixLabelExamples),
                        Name = "Suffix Label Examples",
                        Description = "The SuffixLabel attribute draws a label at the end of a property. It's useful for conveying intend about a property.",
                        PreviewObject = new SuffixLabelExamples(),
                        Code = "public class SuffixLabelExamples\n{\n    [SuffixLabel(\"Prefab\")]\n    public GameObject GameObject;\n\n    [Space(15)]\n    [InfoBox(\n        \"Using the Overlay property, the suffix label will be drawn on top of the property instead of behind it.\\n\" +\n        \"Use this for a neat inline look.\")]\n    [SuffixLabel(\"ms\", Overlay = true)]\n    public float Speed;\n\n    [SuffixLabel(\"radians\", Overlay = true)]\n    public float Angle;\n\n    [Space(15)]\n    [InfoBox(\"The SuffixAttribute also supports referencing a member string field, property, or method by using $.\")]\n    [SuffixLabel(\"$Suffix\", Overlay = true)]\n    public string Suffix = \"Dynamic suffix label\";\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TabGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TabGroupExamples),
                        Name = "Tab Group Examples",
                        Description = null,
                        PreviewObject = new TabGroupExamples(),
                        Code = "public class TabGroupExamples\n{\n    [TabGroup(\"Tab A\")]\n    public int One;\n\n    [TabGroup(\"Tab A\")]\n    public int Two;\n\n    [TabGroup(\"Tab A\")]\n    public int Three;\n\n    [TabGroup(\"Tab B\")]\n    public string MyString;\n\n    [TabGroup(\"Tab B\")]\n    public float MyFloat;\n\n    [TabGroup(\"Tab C\")]\n    [HideLabel]\n    public MyTabObject TabC;\n\n    [TabGroup(\"New Group\", \"Tab A\")]\n    public int A;\n\n    [TabGroup(\"New Group\", \"Tab A\")]\n    public int B;\n\n    [TabGroup(\"New Group\", \"Tab A\")]\n    public int C;\n\n    [TabGroup(\"New Group\", \"Tab B\")]\n    public string D;\n\n    [TabGroup(\"New Group\", \"Tab B\")]\n    public float E;\n\n    [TabGroup(\"New Group\", \"Tab C\")]\n    [HideLabel]\n    public MyTabObject F;\n\n    [Serializable]\n    public class MyTabObject\n    {\n        public int A;\n        public int B;\n        public int C;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TableListAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TableListExamples),
                        Name = "Table List Examples",
                        Description = null,
                        PreviewObject = new TableListExamples(),
                        Code = "public class TableListExamples\n{\n    [TableList(ShowIndexLabels = true)]\n    public List<SomeCustomClass> TableListWithIndexLabels = new List<SomeCustomClass>();\n\n    [TableList(DrawScrollView = true, MaxScrollViewHeight = 200, MinScrollViewHeight = 100)]\n    public List<SomeCustomClass> MinMaxScrollViewTable = new List<SomeCustomClass>();\n\n    [TableList(DrawScrollView = false)]\n    public List<SomeCustomClass> AlwaysExpandedTable = new List<SomeCustomClass>();\n\n    [TableList(ShowPaging = true)]\n    public List<SomeCustomClass> TableWithPaging = new List<SomeCustomClass>();\n\n    [Serializable]\n    public class SomeCustomClass\n    {\n        [TableColumnWidth(57, Resizable = false)]\n        [PreviewField(Alignment = ObjectFieldAlignment.Center)]\n        public Texture Icon;\n\n        [TextArea]\n        public string Description;\n\n        [VerticalGroup(\"Combined Column\"), LabelWidth(22)]\n        public string A, B, C;\n\n        [TableColumnWidth(60)]\n        [Button, VerticalGroup(\"Actions\")]\n        public void Test1() { }\n    \n        [TableColumnWidth(60)]\n        [Button, VerticalGroup(\"Actions\")]\n        public void Test2() { }\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TableMatrixAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TableMatrixExamples),
                        Name = "Table Matrix Examples",
                        Description = "Right-click and drag the column and row labels in order to modify the tables.",
                        PreviewObject = new TableMatrixExamples(),
                        Code = "public class TableMatrixExamples\n{\n    [TableMatrix(SquareCells = true)]\n    public GameObject[,] PrefabMatrix = new GameObject[8, 4];\n\n    [TableMatrix(HorizontalTitle = \"Square Celled Matrix\", SquareCells = true)]\n    public Texture2D[,] SquareCelledMatrix = new Texture2D[8, 4];\n\n    [TableMatrix(HorizontalTitle = \"Read Only Matrix\", IsReadOnly = true)]\n    public int[,] ReadOnlyMatrix = new int[5, 5];\n\n    [TableMatrix(HorizontalTitle = \"X axis\", VerticalTitle = \"Y axis\")]\n    public InfoMessageType[,] LabledMatrix = new InfoMessageType[6, 6];\n\n    [TableMatrix(HorizontalTitle = \"Custom Cell Drawing\", DrawElementMethod = \"DrawColoredEnumElement\", ResizableColumns = false, RowHeight = 16)]\n    public bool[,] CustomCellDrawing = new bool[30, 30];\n\n    private static bool DrawColoredEnumElement(Rect rect, bool value)\n    {\n        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))\n        {\n            value = !value;\n            GUI.changed = true;\n            Event.current.Use();\n        }\n\n        UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));\n\n        return value;\n    }\n}",
                    },
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TwoDimensionalArrayExamples),
                        Name = "Two Dimensional Array Examples",
                        Description = null,
                        PreviewObject = new TwoDimensionalArrayExamples(),
                        Code = "public class TwoDimensionalArrayExamples// : SerializedMonoBehaviour\n{\n    // Unity does not serialize multi-dimensional arrays.\n    // By inheriting from something like SerializedMonoBehaviour you can have Odin serialize multi-dimensional arrays for you.\n    // If you prefer doing that yourself, you can still make Odin show them in the inspector using the ShowInInspector attribute.\n\n    public bool[,] BooleanMatrix = new bool[15, 6];\n\n    [TableMatrix(SquareCells = true)]\n    public Texture2D[,] TextureMatrix = new Texture2D[8, 6];\n\n    public InfoMessageType[,] EnumMatrix = new InfoMessageType[4, 4];\n\n    public string[,] StringMatrix = new string[4, 4];\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TitleAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TitleExamples),
                        Name = "Title Examples",
                        Description = "The Title attribute has the same purpose as Unity's Header attribute,but it also supports properties, and methods.\n\nTitle also offers more features such as subtitles, options for horizontal underline, bold text and text alignment.\n\nBoth attributes, with Odin, supports either static strings, or refering to members strings by adding a $ in front.",
                        PreviewObject = new TitleExamples(),
                        Code = "public class TitleExamples\n{\n    [Title(\"Titles and Headers\")]\n    public string MyTitle = \"My Dynamic Title\";\n    public string MySubtitle = \"My Dynamic Subtitle\";\n\n    [Title(\"Static title\")]\n    public int C;\n    public int D;\n\n    [Title(\"Static title\", \"Static subtitle\")]\n    public int E;\n    public int F;\n\n    [Title(\"$MyTitle\", \"$MySubtitle\")]\n    public int G;\n    public int H;\n\n    [Title(\"Non bold title\", \"$MySubtitle\", bold: false)]\n    public int I;\n    public int J;\n\n    [Title(\"Non bold title\", \"With no line seperator\", horizontalLine: false, bold: false)]\n    public int K;\n    public int L;\n\n    [Title(\"$MyTitle\", \"$MySubtitle\", TitleAlignments.Right)]\n    public int M;\n    public int N;\n\n    [Title(\"$MyTitle\", \"$MySubtitle\", TitleAlignments.Centered)]\n    public int O;\n    public int P;\n\n    [Title(\"$Combined\", titleAlignment: TitleAlignments.Centered)]\n    public int Q;\n    public int R;\n\n    [ShowInInspector]\n    [Title(\"Title on a Property\")]\n    public int S { get; set; }\n\n    [Title(\"Title on a Method\")]\n    [Button]\n    public void DoNothing()\n    { }\n\n    public string Combined { get { return this.MyTitle + \" - \" + this.MySubtitle; } }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TitleGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TitleGroupExamples),
                        Name = "Title Group Examples",
                        Description = null,
                        PreviewObject = new TitleGroupExamples(),
                        Code = "public class TitleGroupExamples\n{\n    [TitleGroup(\"Ints\")]\n    public int SomeInt1;\n\n    [TitleGroup(\"$SomeString1\", \"Optional subtitle\")]\n    public string SomeString1;\n\n    [TitleGroup(\"Vectors\", \"Optional subtitle\", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: false)]\n    public Vector2 SomeVector1;\n\n    [TitleGroup(\"Ints\",\"Optional subtitle\", alignment: TitleAlignments.Split)]\n    public int SomeInt2;\n\n    [TitleGroup(\"$SomeString1\", \"Optional subtitle\")]\n    public string SomeString2;\n\n    [TitleGroup(\"Vectors\")]\n    public Vector2 SomeVector2 { get; set; }\n    \n    [TitleGroup(\"Ints/Buttons\", indent: false)]\n    private void IntButton() { }\n\n    [TitleGroup(\"$SomeString1/Buttons\", indent: false)]\n    private void StringButton() { }\n\n    [TitleGroup(\"Vectors\")]\n    private void VectorButton() { }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ToggleGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ToggleExamples),
                        Name = "Toggle Examples",
                        Description = null,
                        PreviewObject = new ToggleExamples(),
                        Code = "public class ToggleExamples\n{\n    // Simple Toggle Group\n    [ToggleGroup(\"MyToggle\")]\n    public bool MyToggle;\n\n    [ToggleGroup(\"MyToggle\")]\n    public float A;\n\n    [ToggleGroup(\"MyToggle\")]\n    [HideLabel, Multiline]\n    public string B;\n\n    // Toggle for custom data.\n    [ToggleGroup(\"EnableGroupOne\", \"$GroupOneTitle\")]\n    public bool EnableGroupOne;\n\n    [ToggleGroup(\"EnableGroupOne\")]\n    public string GroupOneTitle = \"One\";\n\n    [ToggleGroup(\"EnableGroupOne\")]\n    public float GroupOneA;\n\n    [ToggleGroup(\"EnableGroupOne\")]\n    public float GroupOneB;\n\n    // Toggle for individual objects.\n    [Toggle(\"Enabled\")]\n    public MyToggleObject Three = new MyToggleObject();\n\n    [Toggle(\"Enabled\")]\n    public MyToggleA Four = new MyToggleA();\n\n    [Toggle(\"Enabled\")]\n    public MyToggleB Five = new MyToggleB();\n\n    public MyToggleC[] ToggleList;\n\n    [Serializable]\n    public class MyToggleObject\n    {\n        public bool Enabled;\n\n        [HideInInspector]\n        public string Title;\n\n        public int A;\n        public int B;\n    }\n\n    [Serializable]\n    public class MyToggleA : MyToggleObject\n    {\n        public float C;\n        public float D;\n        public float F;\n    }\n\n    [Serializable]\n    public class MyToggleB : MyToggleObject\n    {\n        public string Text;\n    }\n\n    [Serializable]\n    public class MyToggleC\n    {\n        [ToggleGroup(\"Enabled\", \"$Label\")]\n        public bool Enabled;\n\n        public string Label { get { return this.Test.ToString(); } }\n\n        [ToggleGroup(\"Enabled\")]\n        public float Test;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ToggleLeftAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ToggleLeftExamples),
                        Name = "Toggle Left Examples",
                        Description = null,
                        PreviewObject = new ToggleLeftExamples(),
                        Code = "public class ToggleLeftExamples\n{\n    [InfoBox(\"Draws the toggle button before the label for a bool property.\")]\n    [ToggleLeft]\n    public bool LeftToggled;\n\n    [EnableIf(\"LeftToggled\")]\n    public int A;\n\n    [EnableIf(\"LeftToggled\")]\n    public bool B;\n\n    [EnableIf(\"LeftToggled\")]\n    public bool C;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.TypeFilterAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(TypeFilterExamples),
                        Name = "Type Filter Examples",
                        Description = "The TypeFilter will instantiate the given type directly, It will also draw all child members in a foldout below the dropdown.",
                        PreviewObject = new TypeFilterExamples(),
                        Code = "public class TypeFilterExamples\n{\n    [TypeFilter(\"GetFilteredTypeList\")]\n    public BaseClass A, B, C, D;\n\n    [TypeFilter(\"GetFilteredTypeList\")]\n    public BaseClass[] CustomFilter;\n\n    public IEnumerable<Type> GetFilteredTypeList()\n    {\n        var q = typeof(BaseClass).Assembly.GetTypes()\n            .Where(x => !x.IsAbstract)                                          // Excludes BaseClass\n            .Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>\n            .Where(x => typeof(BaseClass).IsAssignableFrom(x));                 // Excludes classes not inheriting from BaseClass\n\n        // Adds various C1<T> type variants.\n        q = q.AppendWith(typeof(C1<>).MakeGenericType(typeof(GameObject)));\n        q = q.AppendWith(typeof(C1<>).MakeGenericType(typeof(AnimationCurve)));\n        q = q.AppendWith(typeof(C1<>).MakeGenericType(typeof(List<float>)));\n\n        return q;\n    }\n\n    public abstract class BaseClass\n    {\n        public int BaseField;\n    }\n\n    public class A1 : BaseClass { public int _A1; }\n    public class A2 : A1 { public int _A2; }\n    public class A3 : A2 { public int _A3; }\n    public class B1 : BaseClass { public int _B1; }\n    public class B2 : B1 { public int _B2; }\n    public class B3 : B2 { public int _B3; }\n    public class C1<T> : BaseClass { public T C; }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ValidateInputAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ValidateInputExamples),
                        Name = "Validate Input Examples",
                        Description = "ValidateInput is used to display error boxes in case of invalid values.\nIn this case the GameObject must have a MeshRenderer component.",
                        PreviewObject = new ValidateInputExamples(),
                        Code = "public class ValidateInputExamples\n{\n    [HideLabel]\n    [Title(\"Default message\", \"You can just provide a default message that is always used\")]\n    [ValidateInput(\"HasMeshRendererDefaultMessage\", \"Prefab must have a MeshRenderer component\")]\n    public GameObject DefaultMessage;\n\n    [Space(12), HideLabel]\n    [Title(\"Dynamic message\", \"Or the validation method can dynamically provide a custom message\")]\n    [ValidateInput(\"HasMeshRendererDynamicMessage\", \"Prefab must have a MeshRenderer component\")]\n    public GameObject DynamicMessage;\n\n    [Space(12), HideLabel]\n    [Title(\"Dynamic message type\", \"The validation method can also control the type of the message\")]\n    [ValidateInput(\"HasMeshRendererDynamicMessageAndType\", \"Prefab must have a MeshRenderer component\")]\n    public GameObject DynamicMessageAndType;\n\n    [Space(8), HideLabel]\n    [InfoBox(\"Change GameObject value to update message type\", InfoMessageType.None)]\n    public InfoMessageType MessageType;\n\n    [Space(12), HideLabel]\n    [Title(\"Dynamic default message\", \"Use $ to indicate a member string as default message\")]\n    [ValidateInput(\"AlwaysFalse\", \"$Message\", InfoMessageType.Warning)]\n    public string Message = \"Dynamic ValidateInput message\";\n\n    private bool AlwaysFalse(string value)\n    {\n        return false;\n    }\n\n    private bool HasMeshRendererDefaultMessage(GameObject gameObject)\n    {\n        if (gameObject == null) return true;\n\n        return gameObject.GetComponentInChildren<MeshRenderer>() != null;\n    }\n\n    private bool HasMeshRendererDynamicMessage(GameObject gameObject, ref string errorMessage)\n    {\n        if (gameObject == null) return true;\n\n        if (gameObject.GetComponentInChildren<MeshRenderer>() == null)\n        {\n            // If errorMessage is left as null, the default error message from the attribute will be used\n            errorMessage = \"\\\"\" + gameObject.name + \"\\\" must have a MeshRenderer component\";\n\n            return false;\n        }\n\n        return true;\n    }\n\n    private bool HasMeshRendererDynamicMessageAndType(GameObject gameObject, ref string errorMessage, ref InfoMessageType? messageType)\n    {\n        if (gameObject == null) return true;\n\n        if (gameObject.GetComponentInChildren<MeshRenderer>() == null)\n        {\n            // If errorMessage is left as null, the default error message from the attribute will be used\n            errorMessage = \"\\\"\" + gameObject.name + \"\\\" should have a MeshRenderer component\";\n\n            // If messageType is left as null, the default message type from the attribute will be used\n            messageType = this.MessageType;\n\n            return false;\n        }\n\n        return true;\n    }\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.ValueDropdownAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(ValueDropdownExamples),
                        Name = "Value Dropdown Examples",
                        Description = null,
                        PreviewObject = new ValueDropdownExamples(),
                        Code = "public class ValueDropdownExamples\n{\n    [ValueDropdown(\"TextureSizes\")]\n    public int SomeSize1;\n\n    [ValueDropdown(\"FriendlyTextureSizes\")]\n    public int SomeSize2;\n\n    [ValueDropdown(\"FriendlyTextureSizes\", AppendNextDrawer = true, DisableGUIInAppendedDrawer = true)]\n    public int SomeSize3;\n\n    [ValueDropdown(\"GetListOfMonoBehaviours\", AppendNextDrawer = true)]\n    public MonoBehaviour SomeMonoBehaviour;\n\n    [ValueDropdown(\"KeyCodes\")]\n    public KeyCode FilteredEnum;\n\n    [ValueDropdown(\"TreeViewOfInts\", ExpandAllMenuItems = true)]\n    public List<int> IntTreview;\n\n    [ValueDropdown(\"GetAllSceneObjects\", IsUniqueList = true)]\n    public List<GameObject> UniqueGameobjectList;\n\n    [ValueDropdown(\"GetAllSceneObjects\", IsUniqueList = true, DropdownTitle = \"Select Scene Object\", DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)]\n    public List<GameObject> UniqueGameobjectListMode2;\n\n    private IEnumerable TreeViewOfInts = new ValueDropdownList<int>()\n    {\n        { \"Node 1/Node 1.1\", 1 },\n        { \"Node 1/Node 1.2\", 2 },\n        { \"Node 2/Node 2.1\", 3 },\n        { \"Node 3/Node 3.1\", 4 },\n        { \"Node 3/Node 3.2\", 5 },\n        { \"Node 1/Node 3.1/Node 3.1.1\", 6 },\n        { \"Node 1/Node 3.1/Node 3.1.2\", 7 },\n    };\n\n    private IEnumerable<MonoBehaviour> GetListOfMonoBehaviours()\n    {\n        return GameObject.FindObjectsOfType<MonoBehaviour>();\n    }\n\n    private static IEnumerable<KeyCode> KeyCodes = Enumerable.Range((int)KeyCode.Alpha0, 10).Cast<KeyCode>();\n\n    private static IEnumerable GetAllSceneObjects()\n    {\n        Func<Transform, string> getPath = null;\n        getPath = x => (x ? getPath(x.parent) + \"/\" + x.gameObject.name : \"\");\n        return GameObject.FindObjectsOfType<GameObject>().Select(x => new ValueDropdownItem(getPath(x.transform), x));\n    }\n\n    private static IEnumerable GetAllScriptableObjects()\n    {\n        return UnityEditor.AssetDatabase.FindAssets(\"t:ScriptableObject\")\n            .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))\n            .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));\n    }\n\n    private static IEnumerable GetAllSirenixAssets()\n    {\n        var root = \"Assets/Plugins/Sirenix/\";\n\n        return UnityEditor.AssetDatabase.GetAllAssetPaths()\n            .Where(x => x.StartsWith(root))\n            .Select(x => x.Substring(root.Length))\n            .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(root + x)));\n    }\n\n    private static IEnumerable FriendlyTextureSizes = new ValueDropdownList<int>()\n    {\n        { \"Small\", 256 },\n        { \"Medium\", 512 },\n        { \"Large\", 1024 },\n    };\n\n    private static int[] TextureSizes = new int[] { 256, 512, 1024 };\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.VerticalGroupAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(VerticalGroupExamples),
                        Name = "Vertical Group Examples",
                        Description = "VerticalGroup, similar to HorizontalGroup, groups properties together vertically in the inspector.\nBy itself it doesn't do much, but combined with other groups, like HorizontalGroup, it can be very useful. It can also be used in TableLists to create columns.",
                        PreviewObject = new VerticalGroupExamples(),
                        Code = "public class VerticalGroupExamples\n{\n    [HorizontalGroup(\"Split\")]\n    [VerticalGroup(\"Split/Left\")]\n    public InfoMessageType First;\n\n    [VerticalGroup(\"Split/Left\")]\n    public InfoMessageType Second;\n\n    [HideLabel]\n    [VerticalGroup(\"Split/Right\")]\n    public int A;\n\n    [HideLabel]\n    [VerticalGroup(\"Split/Right\")]\n    public int B;\n}",
                    },
                }
            },
            {
                typeof(Sirenix.OdinInspector.WrapAttribute),
                new AttributeExampleInfo[]
                {
                    new AttributeExampleInfo()
                    {
                        ExampleType = typeof(WrapExamples),
                        Name = "Wrap Examples",
                        Description = null,
                        PreviewObject = new WrapExamples(),
                        Code = "public class WrapExamples\n{\n    [Wrap(0f, 100f)]\n    public int IntWrapFrom0To100;\n    \n    [Wrap(0f, 100f)]\n    public float FloatWrapFrom0To100;\n    \n    [Wrap(0f, 100f)]\n    public Vector3 Vector3WrapFrom0To100;\n\n    [Wrap(0f, 360)]\n    public float AngleWrap;\n\n    [Wrap(0f, Mathf.PI * 2)]\n    public float RadianWrap;\n}",
                    },
                }
            },
        };
    }
}
#endif