#ifndef SCENE_FRAGDATA
#define SCENE_FRAGDATA

#include "StandardLighting.cginc"
#include "StandardVert.cginc"

uniform sampler2D _MaskTex;
uniform float  _Roughness;
uniform float _CutOff;
uniform float _Metalic;

inline half4 ScenePerPixelWorldNormal(vertexOutput vertextO, half2 uv)
{
	half3 normal = half3(vertextO.tSpace0.z, vertextO.tSpace1.z, vertextO.tSpace2.z);

#if _USENORMALMAP_ON
	half3 tangent = half3(vertextO.tSpace0.x, vertextO.tSpace1.x, vertextO.tSpace2.x);
	half3 binormal = half3(vertextO.tSpace0.y, vertextO.tSpace1.y, vertextO.tSpace2.y);
	half3 xxx = tex2D(_NormalMap, uv);
	half3 normalTangent;
	normalTangent.xy = xxx.xy * 2 - 1;
	normalTangent.z = sqrt(1 - saturate(dot(normalTangent.xy, normalTangent.xy)));
	half3 normalWorld = normalize(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z);
	return half4(normalWorld, xxx.z);
#else
	return half4(normalize(normal),0);
#endif
}

inline FragmentCommonData SceneSpecularSetup(float4 i_uv, half3 diffuseColor, half3 specColor)
{
	half2 uv = i_uv.xy;
	half oneMinusReflectivity = 1 - SpecularStrength(specColor);
	half3 diffColor = diffuseColor * oneMinusReflectivity;
	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	return o;
}


inline FragmentCommonData SceneBuildingFragmentSetup(vertexOutput vertextO, out half emisScale)
{
	half4 i_tex = vertextO.uv;

#if _HEIGHTMAP_ON
	half h = tex2D(_MaskTex, i_tex).b;
	half2 offset = ParallaxOffset1Step(h, _Parallax, NormalizePerPixelNormal(vertextO.heightuv));
	i_tex = half4(i_tex.xy + offset, i_tex.zw + offset);
#endif

	half3 mask = tex2D(_MaskTex, i_tex);

#if _ALPHATEST_ON
	clip(mask.g - _CutOff);
#endif

	emisScale = mask.b;
	half3 diffuse = tex2D(_MainTex, i_tex);
	half3 specularColor = CalcSpeculorColorFromMetalic(diffuse, _Metalic);
	//高光遮蔽
#if _SPECULARMASK_ON
	specularColor *= mask.r;
#endif

	FragmentCommonData o = SceneSpecularSetup(i_tex, diffuse, specularColor);
	half4 normal = ScenePerPixelWorldNormal(vertextO, i_tex.xy);
	o.normalWorld = normal.rgb;
	o.smoothness = normal.w;
	o.alpha = 1 - o.oneMinusReflectivity + mask.g * o.oneMinusReflectivity;
	o.diffColor = o.diffColor * o.alpha * _Color;
	o.specColor = specularColor;
	o.posWorld = float3(vertextO.tSpace0.w, vertextO.tSpace1.w, vertextO.tSpace2.w);
	o.eyeVec = vertextO.eyeVec; 
	return o;
}

#endif