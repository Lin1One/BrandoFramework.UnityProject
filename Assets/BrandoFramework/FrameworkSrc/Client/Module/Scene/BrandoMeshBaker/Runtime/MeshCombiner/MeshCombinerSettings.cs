using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWorld
{
    [CreateAssetMenu(fileName = "MeshCombineSetting", menuName = "MeshCombine/MeshCombineSetting")]
    public class MeshCombinerSettings : ScriptableObject, IMeshCombinerSettingHolder
    {
        public MeshCombinerSettingsData data;

        public IMeshCombinerSetting GetMeshBakerSettings()
        {
            return data;
        }

#if UNITY_EDITOR
        public UnityEditor.SerializedProperty GetMeshBakerSettingsAsSerializedProperty()
        {
            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
            return so.FindProperty("data");
        }
#endif
    }

    [System.Serializable]
    public class MeshCombinerSettingsData : IMeshCombinerSetting
    {
        [SerializeField] protected RendererType _renderType;
        public virtual RendererType renderType
        {
            get { return _renderType; }
            set { _renderType = value; }
        }

        [SerializeField] protected OutputOptions _outputOption;
        public virtual OutputOptions outputOption
        {
            get { return _outputOption; }
            set { _outputOption = value; }
        }

        [SerializeField] protected LightmapOptions _lightmapOption = LightmapOptions.ignore_UV2;
        public virtual LightmapOptions lightmapOption
        {
            get { return _lightmapOption; }
            set { _lightmapOption = value; }
        }

        [SerializeField] protected bool _doNorm = true;
        public virtual bool doNorm
        {
            get { return _doNorm; }
            set { _doNorm = value; }
        }

        [SerializeField] protected bool _doTan = true;
        public virtual bool doTan
        {
            get { return _doTan; }
            set { _doTan = value; }
        }

        [SerializeField] protected bool _doCol;
        public virtual bool doCol
        {
            get { return _doCol; }
            set { _doCol = value; }
        }

        [SerializeField] protected bool _doUV = true;
        public virtual bool doUV
        {
            get { return _doUV; }
            set { _doUV = value; }
        }

        [SerializeField] protected bool _doUV3;
        public virtual bool doUV3
        {
            get { return _doUV3; }
            set { _doUV3 = value; }
        }

        [SerializeField] protected bool _doUV4;
        public virtual bool doUV4
        {
            get { return _doUV4; }
            set { _doUV4 = value; }
        }

        [SerializeField]
        protected bool _doBlendShapes;
        public virtual bool doBlendShapes
        {
            get { return _doBlendShapes; }
            set { _doBlendShapes = value; }
        }

        [SerializeField]
        protected bool _recenterVertsToBoundsCenter = false;
        public virtual bool recenterVertsToBoundsCenter
        {
            get { return _recenterVertsToBoundsCenter; }
            set { _recenterVertsToBoundsCenter = value; }
        }

        [SerializeField]
        protected bool _clearBuffersAfterBake = false;
        public bool clearBuffersAfterBake
        {
            get { return _clearBuffersAfterBake; }
            set { _clearBuffersAfterBake = value; }
        }

        [SerializeField]
        public bool _optimizeAfterBake = true;
        public bool optimizeAfterBake
        {
            get { return _optimizeAfterBake; }
            set { _optimizeAfterBake = value; }
        }

        [SerializeField]
        protected float _uv2UnwrappingParamsHardAngle = 60f;
        public float uv2UnwrappingParamsHardAngle
        {
            get { return _uv2UnwrappingParamsHardAngle; }
            set { _uv2UnwrappingParamsHardAngle = value; }
        }

        [SerializeField]
        protected float _uv2UnwrappingParamsPackMargin = .005f;
        public float uv2UnwrappingParamsPackMargin
        {
            get { return _uv2UnwrappingParamsPackMargin; }
            set { _uv2UnwrappingParamsPackMargin = value; }
        }
    }
}
