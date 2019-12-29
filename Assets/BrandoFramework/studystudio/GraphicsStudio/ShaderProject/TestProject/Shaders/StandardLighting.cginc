#include "UnityStandardCore.cginc"

uniform sampler2D _LightColorMap;

uniform half4 _GlobalLightColor;
uniform float4 _GlobalLightDir;

uniform half4 _PointLightColor;
uniform half _PointLightIntensity;


uniform half _NightMultiplier;



inline UnityLight MyMainLight(half3 lightColor, half3 _dir)
{
	UnityLight l;

	l.color = lightColor;
	l.dir = _dir;

	return l;
}

inline half3 CalcSpeculorColorFromMetalic(inout half3 albedo, half metalic)
{
	half tmp = 0.220916301;
	half3 specColor = lerp(tmp.xxx, albedo, metalic);
	return specColor;
}

// inline half4 caculateAddLight(half3 dir, vertexOutput i, FragmentCommonData s)
// {
// 	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
// 	UnityLight light = AdditiveLight(dir, atten);
// 	UnityIndirect noIndirect = ZeroIndirect();
// 	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);
// 	return final;
// }