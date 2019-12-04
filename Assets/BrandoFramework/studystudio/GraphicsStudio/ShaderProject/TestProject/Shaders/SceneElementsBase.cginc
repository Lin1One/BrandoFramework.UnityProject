#ifndef __SCENEELEMENTS_BASE_SHADER__
#define __SCENEELEMENTS_BASE_SHADER__

#include "SceneStandardSpecular.cginc"

uniform half _PaintScale;
uniform half4 _PaintColor;
uniform half _LightStrength;

inline half4 SceneCaculateLight(vertexOutput i, FragmentCommonData s)
{
#if _REALTIME_ON && !_FORCEBAKE_ON
	UnityLight mainLight = MainLight();
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, mainLight);
	gi.indirect.diffuse = ShadeSHPerPixel(s.normalWorld, 0, s.posWorld);
#else
	UnityLight mainLight;
	mainLight.color = _GlobalLightColor.rgb * _LightMapWeight.x;
	mainLight.dir = normalize(_GlobalLightDir.xyz);
	half shadowMask;
	half3 lightMapDiffuse = GetLightMapDiffuse(i.uv.zw, shadowMask);
	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, shadowMask, mainLight);
	gi.indirect.diffuse = lightMapDiffuse;
#endif

	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	return final;
}

inline half3 CalcLightMapColor(vertexOutput i, half3 diffuseColor)
{
	half3 lightMapColor = DecodeLightmap(tex2D(_LightColorMap, i.uv.zw));
	half tempValue = _LightMapWeight.y * _NightMultiplier * _PointLightColor * _PointLightIntensity;
	return lightMapColor * diffuseColor * tempValue;
}

#endif // __SCENEELEMENTS_BASE_SHADER__