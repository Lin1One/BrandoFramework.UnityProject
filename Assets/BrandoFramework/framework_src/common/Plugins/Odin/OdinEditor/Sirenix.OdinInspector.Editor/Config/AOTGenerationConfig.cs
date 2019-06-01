#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AOTGenerationConfig.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AOTGenerationConfig.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using Sirenix.Serialization.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Contains configuration for generating an assembly that provides increased AOT support in Odin.
    /// </summary>
    [SirenixEditorConfig]
    public class AOTGenerationConfig : GlobalConfig<AOTGenerationConfig>
    {
        private static readonly TwoWaySerializationBinder TypeBinder = new DefaultSerializationBinder();

        [Serializable]
        private class TypeEntry
        {
            [NonSerialized]
            public bool IsInitialized;

            [NonSerialized]
            public bool IsNew;

            [NonSerialized]
            public string NiceTypeName;

            public string TypeName;

            public bool IsCustom;

            public bool Emit;

            public Type Type;
        }

        private string AutomateBeforeBuildsSuffix { get { return this.EnableAutomateBeforeBuilds ? "Experimental" : "The automation feature is only available in Unity 5.6 and up"; } }
        private bool EnableAutomateBeforeBuilds { get { return UnityVersion.IsVersionOrGreater(5, 6); } }
        private bool ShowAutomateConfig { get { return this.EnableAutomateBeforeBuilds && this.automateBeforeBuilds; } }

        [SerializeField, ToggleLeft, EnableIf("EnableAutomateBeforeBuilds"), SuffixLabel("$AutomateBeforeBuildsSuffix", false)]
        private bool automateBeforeBuilds = false;

        [SerializeField, ToggleLeft, ShowIf("ShowAutomateConfig")]
        private bool deleteDllAfterBuilds = true;

        [SerializeField, ShowIf("ShowAutomateConfig")]
        private List<BuildTarget> automateForPlatforms = new List<BuildTarget>()
        {
            BuildTarget.iOS,
            BuildTarget.WebGL,
        };

        [SerializeField, HideInInspector]
        private long lastScan;

        [SerializeField, PropertyOrder(4)]
        [ListDrawerSettings(DraggableItems = false, OnTitleBarGUI = "GenericVariantsTitleGUI", HideAddButton = true)]
        private List<TypeEntry> supportSerializedTypes;

        /// <summary>
        /// <para>
        /// Whether to automatically scan the project and generate an AOT dll, right before builds. This will only affect platforms that are in the <see cref="AutomateForPlatforms"/> list.
        /// </para>
        /// <para>
        /// **This will only work on Unity 5.6 and higher!**
        /// </para>
        /// </summary>
        public bool AutomateBeforeBuilds
        {
            get { return this.automateBeforeBuilds; }
            set { this.automateBeforeBuilds = value; }
        }

        /// <summary>
        /// Whether to automatically delete the generated AOT dll after a build has completed.
        /// </summary>
        public bool DeleteDllAfterBuilds
        {
            get { return this.deleteDllAfterBuilds; }
            set { this.deleteDllAfterBuilds = value; }
        }

        /// <summary>
        /// A list of platforms to automatically scan the project and generate an AOT dll for, right before builds. This will do nothing unless <see cref="AutomateBeforeBuilds"/> is true.
        /// </summary>
        public List<BuildTarget> AutomateForPlatforms
        {
            get { return this.automateForPlatforms; }
        }

        /// <summary>
        /// The path to the AOT folder that the AOT .dll and linker file is created in, relative to the current project folder.
        /// </summary>
        public string AOTFolderPath { get { return "Assets/" + SirenixAssetPaths.SirenixAssembliesPath + "AOT/"; } }

        //[ToggleLeft]
        //[TitleGroup("Generate AOT DLL"), PropertyOrder(9)]
        //[InfoBox("If 'Emit AOT Formatters' is enabled, Odin will also generate serialization formatters for types which need it. This removes the need for reflection on AOT platforms, and can significantly speed up serialization.")]
        //[SerializeField]
        //private bool emitAOTFormatters = true;

        [OnInspectorGUI, PropertyOrder(-10000)]
        private void DrawExperimentalText()
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rect = GUIHelper.GetCurrentLayoutRect();
                rect.height = 20;
                rect.y -= 35;
                GUI.Label(rect, "Experimental", new GUIStyle(SirenixGUIStyles.RightAlignedGreyMiniLabel) { fontSize = 16 });
            }
        }

        private void GenericVariantsTitleGUI()
        {
            SirenixEditorGUI.VerticalLineSeparator();
            GUILayout.Label("Last scan: " + DateTime.FromBinary(this.lastScan).ToString(), EditorStyles.centeredGreyMiniLabel);

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("  Sort  ")))
            {
                this.SortTypes();
            }

            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                this.supportSerializedTypes.Insert(0, new TypeEntry() { IsCustom = true, Emit = true });
            }
        }

        private void SortTypes()
        {
            Comparison<TypeEntry> sorter = (TypeEntry a, TypeEntry b) =>
            {
                bool aType = a.Type != null;
                bool bType = b.Type != null;

                if (aType != bType)
                {
                    return aType ? 1 : -1;
                }
                else if (!aType)
                {
                    return (a.TypeName ?? "").CompareTo(b.TypeName ?? "");
                }

                if (a.IsCustom != b.IsCustom)
                {
                    return a.IsCustom ? -1 : 1;
                }

                if (a.IsNew != b.IsNew)
                {
                    return a.IsNew ? -1 : 1;
                }

                return (a.NiceTypeName ?? "").CompareTo(b.NiceTypeName ?? "");
            };

            this.supportSerializedTypes.Sort(sorter);
            //this.emitFormattersForTypes.Sort(sorter);
        }

        [Button("Scan Project", 36), HorizontalGroup("ButtonMargin", 0.2f, PaddingRight = -4), PropertyOrder(2)]
        private void ScanProjectButton()
        {
            EditorApplication.delayCall += ScanProject;
        }

        /// <summary>
        /// Scans the entire project for types to support AOT serialization for.
        /// </summary>
        public void ScanProject()
        {
            List<Type> serializedTypes;

            if (AOTSupportUtilities.ScanProjectForSerializedTypes(out serializedTypes))
            {
                this.RegisterTypes(this.supportSerializedTypes, serializedTypes);
                this.SortTypes();

                this.lastScan = DateTime.Now.Ticks;
                EditorUtility.SetDirty(this);
            }
        }

        private void RegisterTypes(List<TypeEntry> typeEntries, List<Type> types)
        {
            var preExistingNonCustomTypes = typeEntries
                .Where(n => !n.IsCustom && n.Type != null)
                .Select(n => n.Type)
                .ToHashSet();

            typeEntries.RemoveAll(n => !n.IsCustom);

            var preExistingCustomTypes = typeEntries
                .Where(n => n.Type != null)
                .Select(n => n.Type)
                .ToHashSet();

            typeEntries.AddRange(types
                .Where(type => !preExistingCustomTypes.Contains(type))
                .Select(type => new TypeEntry()
                {
                    Type = type,
                    TypeName = TypeBinder.BindToName(type),
                    NiceTypeName = type.GetNiceName(),
                    IsCustom = false,
                    Emit = true,
                    IsNew = !preExistingNonCustomTypes.Contains(type),
                    IsInitialized = false
                }));

            this.InitializeTypeEntries();
        }

        [OnInspectorGUI, PropertyOrder(-1)]
        private void DrawTopInfoBox()
        {
            SirenixEditorGUI.InfoMessageBox(
                "On AOT-compiled platforms, Unity's code stripping can remove classes that the serialization system needs, " +
                "or fail to generate code for needed variants of generic types. Therefore, Odin can create an assembly that " +
                "directly references all functionality that is needed at runtime, to ensure it is available.");
        }

        [OnInspectorGUI, HorizontalGroup("ButtonMargin"), PropertyOrder(1)]
        private void DrawWarning()
        {
            EditorGUILayout.HelpBox(
                "Scanning the entire project might take a while. It will scan the entire project for relevant types including " +
                "ScriptableObjects, prefabs and scenes. Modified type entries will not be touched.",
                MessageType.Warning);
        }

        /// <summary>
        /// Generates an AOT DLL, using the current configuration of the AOTGenerationConfig instance.
        /// </summary>
        [TitleGroup("Generate AOT DLL", "Sirenix/Assemblies/AOT/" + FormatterEmitter.PRE_EMITTED_ASSEMBLY_NAME + ".dll", indent: false), PropertyOrder(9)]
        [Button(ButtonSizes.Large)]
        public void GenerateDLL()
        {
            var generateForTypes = this.supportSerializedTypes.Where(n => n.Emit && n.Type != null).Select(n => n.Type).ToList();

            FixUnityAboutWindowBeforeEmit.Fix();
            AOTSupportUtilities.GenerateDLL(this.AOTFolderPath, FormatterEmitter.PRE_EMITTED_ASSEMBLY_NAME, generateForTypes, true);
        }

        public void GenerateDLL(string folderPath, bool generateLinkXML = true)
        {
            var generateForTypes = this.supportSerializedTypes.Where(n => n.Emit && n.Type != null).Select(n => n.Type).ToList();

            FixUnityAboutWindowBeforeEmit.Fix();
            AOTSupportUtilities.GenerateDLL(folderPath, FormatterEmitter.PRE_EMITTED_ASSEMBLY_NAME, generateForTypes, generateLinkXML);
        }

        [OnInspectorGUI, PropertyOrder(-1000)]
        private void OnGUIInitializeTypeEntries()
        {
            if (Event.current.type != EventType.Layout)
            {
                return;
            }

            this.InitializeTypeEntries();
        }

        private void InitializeTypeEntries()
        {
            this.supportSerializedTypes = this.supportSerializedTypes ?? new List<TypeEntry>();

            //this.emitFormattersForTypes = this.emitFormattersForTypes ?? new List<TypeEntry>();

            // Type is not serialized by Unity. Deserialize on the editor is not always called when reloading, neither is OnEnabled...
            // So right now we do it like this. if you have better ideas I'm all ears.
            foreach (var item in this.supportSerializedTypes/*.Concat(this.emitFormattersForTypes)*/)
            {
                if (!item.IsInitialized)
                {
                    if (item.Type == null)
                    {
                        if (item.TypeName != null)
                        {
                            item.Type = TypeBinder.BindToType(item.TypeName);
                        }

                        if (item.Type != null)
                        {
                            item.NiceTypeName = item.Type.GetNiceName();
                        }
                    }
                    item.IsInitialized = true;
                }
            }
        }

        private class TypeEntryDrawer : OdinValueDrawer<TypeEntry>
        {
            // private static readonly GUIStyle deletedLabelStyle = new GUIStyle("sv_label_3") { margin = new RectOffset(3, 3, 2, 0), alignment = TextAnchor.MiddleCenter };
            private static readonly GUIStyle MissingLabelStyle = new GUIStyle("sv_label_6") { margin = new RectOffset(3, 3, 2, 0), alignment = TextAnchor.MiddleCenter };

            private static readonly GUIStyle NewLabelStyle = new GUIStyle("sv_label_3") { margin = new RectOffset(3, 3, 2, 0), alignment = TextAnchor.MiddleCenter };
            private static readonly GUIStyle ChangedLabelStyle = new GUIStyle("sv_label_4") { margin = new RectOffset(3, 3, 2, 0), alignment = TextAnchor.MiddleCenter };

            protected override void DrawPropertyLayout(GUIContent label)
            {
                var entry = this.ValueEntry;
                var value = entry.SmartValue;
                var valueChanged = false;
                var rect = EditorGUILayout.GetControlRect();
                var toggleRect = rect.SetWidth(20);
                rect.xMin += 20;

                // Init
                if (string.IsNullOrEmpty(value.NiceTypeName) && value.Type != null)
                {
                    value.NiceTypeName = value.Type.GetNiceName();
                }

                if (value.IsNew || value.IsCustom || value.Type == null)
                {
                    rect.xMax -= 78;
                }

                // Toggle
                GUIHelper.PushGUIEnabled(value.Type != null);
                valueChanged = value.Emit != (value.Emit = EditorGUI.Toggle(toggleRect, value.Emit));
                GUIHelper.PopGUIEnabled();

                // TextField
                rect.y += 2;
                GUI.SetNextControlName(entry.Property.Path);
                bool isEditing = GUI.GetNameOfFocusedControl() == entry.Property.Path || value.Type == null;
                GUIHelper.PushColor(isEditing ? Color.white : new Color(0, 0, 0, 0));
                var newName = EditorGUI.TextField(rect, value.TypeName, EditorStyles.textField);
                GUIHelper.PopColor();

                // TextField overlay
                if (!isEditing)
                {
                    GUI.Label(rect, value.NiceTypeName);
                }

                if (value.IsNew || value.IsCustom || value.Type == null)
                {
                    rect.xMin = rect.xMax + 3;
                    rect.width = 75;
                }

                // Labels
                if (value.Type == null)
                {
                    EditorGUI.LabelField(rect, GUIHelper.TempContent("MISSING"), MissingLabelStyle);
                }
                else if (value.IsNew)
                {
                    EditorGUI.LabelField(rect, GUIHelper.TempContent("NEW"), NewLabelStyle);
                }
                else if (value.IsCustom)
                {
                    EditorGUI.LabelField(rect, GUIHelper.TempContent("MODIFIED"), ChangedLabelStyle);
                }

                // Set values
                if ((newName ?? "") != (value.TypeName ?? ""))
                {
                    value.TypeName = newName;
                    value.IsCustom = true;
                    value.Type = TypeBinder.BindToType(value.TypeName);
                    value.NiceTypeName = value.Type == null ? value.TypeName : value.Type.GetNiceName();
                    valueChanged = true;
                }

                if (valueChanged)
                {
                    value.IsCustom = true;
                    entry.Values.ForceMarkDirty();
                }
            }
        }
    }
}
#endif