using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    [System.Serializable]
    public struct MinMaxGradientData : ISerializationCallbackReceiver
    {
        public ParticleSystemGradientMode _mode;
        public Color _color1;
        public Color _color2;
        public Gradient _gradient1;
        public Gradient _gradient2;
        public ParticleSystem.MinMaxGradient _startColor;
        public MinMaxGradientData(Color c1)
        {
            _mode = ParticleSystemGradientMode.Color;
            _color1 = c1;
            _color2 = Color.clear;
            _gradient1 = _gradient2 = null;
            _startColor = MakeMinMaxGradient(_mode, _color1, _color2, _gradient1, _gradient2);
        }
        public MinMaxGradientData(Color c1, Color c2)
        {
            _mode = ParticleSystemGradientMode.TwoColors;
            _color1 = c1;
            _color2 = c2;
            _gradient1 = _gradient2 = null;
            _startColor = MakeMinMaxGradient(_mode, _color1, _color2, _gradient1, _gradient2);

        }
        public MinMaxGradientData(Gradient gradient, ParticleSystemGradientMode mode)
        {
            _mode = mode; // ParticleSystemGradientMode.RandomColor or Gradient
            _color1 = _color2 = Color.clear;
            _gradient1 = gradient;
            _gradient2 = null;
            _startColor = MakeMinMaxGradient(_mode, _color1, _color2, _gradient1, _gradient2);
        }
        public MinMaxGradientData(Gradient gradient1, Gradient gradient2)
        {
            _mode = ParticleSystemGradientMode.TwoGradients;
            _color1 = _color2 = Color.clear;
            _gradient1 = gradient1;
            _gradient2 = gradient2;
            _startColor = MakeMinMaxGradient(_mode, _color1, _color2, _gradient1, _gradient2);
        }
        static ParticleSystem.MinMaxGradient MakeMinMaxGradient(ParticleSystemGradientMode mode,
            Color col1, Color col2, Gradient gradient1, Gradient gradient2)
        {
            ParticleSystem.MinMaxGradient _mmGradient;
            if (mode == ParticleSystemGradientMode.Gradient
                || mode == ParticleSystemGradientMode.RandomColor)
            {
                _mmGradient = new ParticleSystem.MinMaxGradient(gradient1);
            }
            else if (mode == ParticleSystemGradientMode.TwoColors)
            {
                _mmGradient = new ParticleSystem.MinMaxGradient(col1, col2);
            }
            else if (mode == ParticleSystemGradientMode.TwoGradients)
            {
                _mmGradient = new ParticleSystem.MinMaxGradient(gradient1, gradient2);
            }
            else // if (ps.main.startColor.mode == ParticleSystemGradientMode.Color)
                _mmGradient = new ParticleSystem.MinMaxGradient(col1);
            _mmGradient.mode = mode;
            return _mmGradient;
        }
        public Color GetColor(ParticleSystem _ps, ParticleSystem.Particle _pc)
        {
            if (_startColor.mode == ParticleSystemGradientMode.Color)
                return _startColor.color;
            else if (_startColor.mode == ParticleSystemGradientMode.RandomColor)
                return _startColor.Evaluate(UnityEngine.Random.Range(0, 1f));
            // ParticleSystemGradientMode.Gradient .TwoColors .TwoGradients)
            float fStartTime = ((_ps.time + _pc.remainingLifetime) % _ps.main.duration) / _ps.main.duration;
            return _startColor.Evaluate(fStartTime);
        }
        void ApplyChange()
        {
            // _startColor = MakeMinMaxGradient(_mode, _color1, _color2, _gradient1, _gradient2);
            _startColor.mode = _mode;
            if (_startColor.mode == ParticleSystemGradientMode.Gradient
                || _startColor.mode == ParticleSystemGradientMode.RandomColor)
            {
                _startColor.gradient = _gradient1;
            }
            else if (_startColor.mode == ParticleSystemGradientMode.TwoColors)
            {
                _startColor.colorMin = _color1;
                _startColor.colorMax = _color2;
            }
            else if (_startColor.mode == ParticleSystemGradientMode.TwoGradients)
            {
                _startColor.gradientMin = _gradient1;
                _startColor.gradientMax = _gradient2;               
            }
            else
                _startColor.color = _startColor.colorMin = _startColor.colorMax = _color1;
        }

        public void OnBeforeSerialize()
        {

        }
        public void OnAfterDeserialize()
        {
            ApplyChange();
        }
    }
	[System.Serializable]
	public struct ParticleTrickData
	{
		public ParticleSystem _ps;
		public ParticleSystemRenderer _prr;
        public MinMaxGradientData startColorData;
		public bool bDynamicParticle_SelfColliderOn;
        public int layerMask;
        public bool bOverrideColor;
        public Color hiddenColor;
        public float fParticleSortingFudge;
		public bool bUseTrick;
        public int sortingLayer;
		public ParticleTrickData(ParticleSystem ps)
		{
			bDynamicParticle_SelfColliderOn = true;
			bUseTrick = true;
            bOverrideColor = true;
            sortingLayer = 0;
            layerMask = 1 << LayerMask.NameToLayer("Default");
            startColorData = CopyColor(ps.main.startColor);

            var main = ps.main;
            hiddenColor = Color.clear;
            main.startColor = hiddenColor;

			if (ps == null)
			{
				_ps = null;
				_prr = null;
				fParticleSortingFudge = 0f;
				return;
			}

			_ps = ps;
			_prr = _ps.GetComponent<ParticleSystemRenderer>();
			fParticleSortingFudge = _prr.sortingFudge;
		}
        
        static MinMaxGradientData CopyColor(ParticleSystem.MinMaxGradient _startColor)
        {
            MinMaxGradientData _sc;
            if (_startColor.mode == ParticleSystemGradientMode.Gradient
                || _startColor.mode == ParticleSystemGradientMode.RandomColor)
            {
                _sc = new MinMaxGradientData(_startColor.gradient, _startColor.mode);
            }
            else if (_startColor.mode == ParticleSystemGradientMode.TwoColors)
            {
                _sc = new MinMaxGradientData(_startColor.colorMin, _startColor.colorMax);
            }
            else if (_startColor.mode == ParticleSystemGradientMode.TwoGradients)
            {
                _sc = new MinMaxGradientData(_startColor.gradientMin, _startColor.gradientMax);
            }
            else // if (ps.main.startColor.mode == ParticleSystemGradientMode.Color)
                _sc = new MinMaxGradientData(_startColor.color);
            return _sc;
        }
        public Color GetColor(ParticleSystem.Particle _pc)
        {
            return startColorData.GetColor(_ps, _pc);
        }

        public Vector3 GetBasePosition()
        {
            if (_ps.main.simulationSpace == ParticleSystemSimulationSpace.World)
                return Vector3.zero;
            else if (_ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                return _ps.transform.position;
            // else if (_ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            return _ps.main.customSimulationSpace.position;
        }

        public static void Apply_SortingData(ParticleSystemRenderer _prr, 
            float fParticleSortingFudge, int sortingLayer)
        {
            if (_prr != null)
            {
                _prr.sortingFudge = fParticleSortingFudge;
                _prr.sortingLayerID = SortingLayer.layers.Length > sortingLayer ? SortingLayer.layers[sortingLayer].id : 0;
            }
        }

        public void Sync_ColorData(bool bUpside)
        {
            if (_ps != null)
            {
                if (bUpside)
                {
                    var _main = _ps.main;
                    _main.startColor = startColorData._startColor;
                }
                else
                {
                    this = new ParticleTrickData(_ps);
                }
            }
        }

	}

    [DisallowMultipleComponent]
	[ExecuteInEditMode]
	[System.Serializable]
	public class ParticleSortingTrick : MonoBehaviour {
		static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[256];

        [SerializeField]
        public float fFudgeMax = 10f;

		[SerializeField]
		List<ParticleTrickData> _pdArray = new List<ParticleTrickData>();
        public List<ParticleTrickData> Childs{get{return _pdArray;}}
        public bool Contains(ParticleSystem _ps)
        {
            return _pdArray.Exists(r => r._ps == _ps);
        }
        public void Clear()
        {
            _pdArray.ForEach((r) => {r.Sync_ColorData(true);});
            _pdArray.Clear();
        }
        public void Sync(bool bUpside)
        {
            _pdArray.ForEach((r) => {r.Sync_ColorData(bUpside);});
        }

        int setParticleArray(ParticleSystem _ps)
        {
            if (particles == null || particles.Length < _ps.main.maxParticles)
                particles = new ParticleSystem.Particle[_ps.main.maxParticles];
            return _ps.GetParticles(particles);
        }

        void update_ParticleTrick(bool bJustDoIt = false)
        {
            RaycastHit _hit;
            ParticleSystem _ps;
			ParticleSystemRenderer _prr;
            Vector3 _particlePosition;
            Vector3 _particlePosition_base = Vector3.zero;
            Vector3 _rendererPosition;
            Vector3 _dir;
            Color32 _hiddenColor;
            float _distance;
            int iCount;
            bool bCorrupted = false;

            for (int i = 0 ; i < _pdArray.Count ; ++i)
            {
                if (_pdArray[i]._prr == null)
                {
                    bCorrupted = true;
                    continue;
                }
				_prr =  _pdArray[i]._prr;
                _rendererPosition = _prr.transform.position;

                if (_pdArray[i].bUseTrick)
                {
                    _ps = _prr.GetComponent<ParticleSystem>();
                    _hiddenColor = _pdArray[i].hiddenColor;
                    _particlePosition_base = _pdArray[i].GetBasePosition();

                    // _distance = _prr.bounds.size.magnitude;
                    _distance = Vector3.Distance(_rendererPosition, Camera.main.transform.position);
                    iCount = setParticleArray(_ps);

                    for (int j = 0; j < iCount; j++)
                    {
                        _particlePosition = _particlePosition_base + (Vector3) (_ps.transform.localToWorldMatrix * particles[j].position);
                        if (_pdArray[i].bDynamicParticle_SelfColliderOn)
                        {
                            _dir = _particlePosition - _rendererPosition;
                            if (Physics.Raycast(_rendererPosition, _dir, out _hit, _dir.magnitude, _pdArray[i].layerMask))
                            {
                                particles[j].startColor = _hiddenColor;
                                particles[j].remainingLifetime = 0;
                                continue;
                            }
                        }

                        if (_pdArray[i].bOverrideColor)
                        {
                            if (Physics.Raycast(_particlePosition - Camera.main.transform.forward * _distance,
                                Camera.main.transform.forward, out _hit, _distance, _pdArray[i].layerMask))
                            {
                                particles[j].startColor = Color.Lerp(_pdArray[i].GetColor(particles[j]), _hiddenColor, 
                                    2f * (_distance - _hit.distance) / particles[j].GetCurrentSize(_pdArray[i]._ps));
                            }
                            else if (particles[j].startColor.Equals(_hiddenColor))
                            {
                                particles[j].startColor = _pdArray[i].GetColor(particles[j]);
                            }
                        }
                    }
                    _ps.SetParticles(particles, iCount);
                }
            }

            if (bCorrupted)
                Update_Child(false);
        }

        void OnDrawGizmosSelected() {
            Vector3 _start = transform.position;
            Vector3 _end = _start - Vector3.Distance(_start, Camera.main.transform.position) * Camera.main.transform.forward;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_start, _end);

            for (int i = 0 ; i < _pdArray.Count ; ++i)
            {
                if (_pdArray[i]._prr == null)
                {
                    continue;
                }

                if (_pdArray[i].bUseTrick)
                {
                    ParticleSystemRenderer _prr = _pdArray[i]._prr;;
                    ParticleSystem _ps = _prr.GetComponent<ParticleSystem>();
                    Vector3 _particlePosition_base = _pdArray[i].GetBasePosition();
                    float _distance = Vector3.Distance(_particlePosition_base, Camera.main.transform.position);

                    int iCount = setParticleArray(_ps);

                    for (int j = 0; j < iCount; j++)
                    {
                        Vector3 _particlePosition = _particlePosition_base + (Vector3) (_ps.transform.localToWorldMatrix * particles[j].position);
                        Gizmos.DrawLine(_particlePosition, _particlePosition + _distance * -Camera.main.transform.forward);
                    }
                }
            }
        }

		public void Update_Child(bool bRemoveOnly)
        {
			ParticleSystem[] _particlesSystems = transform.GetComponentsInChildren<ParticleSystem>();

            ParticleSortingTrick[] _exList = transform.GetComponentsInChildren<ParticleSortingTrick>();
            if (_exList != null)
                _particlesSystems = _particlesSystems.Where(r => !_exList.Any(ex => ex != this && ex.Contains(r))).ToArray();

			if (_particlesSystems != null)
			{
				for (int i = _pdArray.Count - 1 ; i >= 0 ; --i)
				{
					if (!_particlesSystems.Contains(_pdArray[i]._ps))
						_pdArray.RemoveAt(i);
                    // else
                    //     _pdArray[i].SyncColor(true);
				}

                if (!bRemoveOnly)
                {
                    for (int i = 0 ; i < _particlesSystems.Length ; ++i)
                    {
                        if (!_pdArray.Exists(r => r._ps == _particlesSystems[i]))
                        {
                            ParticleTrickData _pd = new ParticleTrickData(_particlesSystems[i]);
                            _pdArray.Add(_pd);
                        }
                    }
                }
			}
        }

        void OnTransformChildrenChanged()
        {
            Update_Child(false);
            update_ParticleTrick();
        }
        void OnEnable()
        {
            Update_Child(false);
            update_ParticleTrick(true);
        }
        void Update()
        {
            update_ParticleTrick();
        }
	}
}