#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "XGame2.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardCore.cginc"
#include "GPUSkinningInclude.cginc"

uniform sampler2D _NormMap;

struct vertexOutput
{
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float4 tSpace0 : TEXCOORD1;
	float4 tSpace1 : TEXCOORD2;
	float4 tSpace2 : TEXCOORD3;
	half3 lightDir : TEXCOORD6; 
	half4 ambientOrLightmapUV : TEXCOORD7;
	UNITY_FOG_COORDS(4)
	
#if SHADOWS_SCREEN
		SHADOW_COORDS(5)
#endif

#if _USEHEIGHT_ON
	half3 heightuv : TEXCOORD8;
#endif

	float3 eyeVec : TEXCOORD9;
	
#if _VERTALPHA_ON
	fixed4 color : COLOR;
#endif

	UNITY_VERTEX_INPUT_INSTANCE_ID 
};

inline vertexOutput vert(appdata_full v)
{
	UNITY_SETUP_INSTANCE_ID(v);
#if _ENABLEBONEBAKE_ON
	float4 normal = float4(v.normal, 0);
	float4 tangent = float4(v.tangent.xyz, 0);

	float4 pos = skin4(v.vertex, v.texcoord2, v.texcoord3);
	normal = skin4(normal, v.texcoord2, v.texcoord3);
	tangent = skin4(tangent, v.texcoord2, v.texcoord3);

	v.vertex = pos;
	v.normal = normal.xyz;
	v.tangent = float4(tangent.xyz, v.tangent.w);
#endif

	vertexOutput o;
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	o.pos = UnityObjectToClipPos(v.vertex);

	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
	float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
	o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
	o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
	o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
	o.uv = 0;
	o.uv.xy = v.texcoord.xy;

#ifdef LIGHTMAP_ON
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if SHADOWS_SCREEN
	TRANSFER_SHADOW(o);
#endif

	o.lightDir = _WorldSpaceLightPos0.xyz - worldPos * _WorldSpaceLightPos0.w;
#ifndef USING_DIRECTIONAL_LIGHT
	o.lightDir = NormalizePerVertexNormal(o.lightDir);
#endif

	VertexInput i = (VertexInput)0;
	i.uv1 = o.uv;
	o.ambientOrLightmapUV = VertexGIForward(i, worldPos, worldNormal);

	UNITY_TRANSFER_FOG(o, o.pos);

#if _USEHEIGHT_ON
	TANGENT_SPACE_ROTATION;
	half3 viewDirForParallax = mul(rotation, ObjSpaceViewDir(v.vertex));
	o.heightuv = viewDirForParallax;
#endif

	o.eyeVec = normalize(worldPos - _WorldSpaceCameraPos);
	
#if _VERTALPHA_ON
	o.color = v.color;
#endif	
	
	return o;
}

inline UnityLight MyMainLight(half3 lightColor, half3 _dir)
{
	UnityLight l;

	l.color = lightColor;
	l.dir = _dir;

	return l;
}

inline half4 caculateAddLight(half3 dir, vertexOutput i, FragmentCommonData s)
{
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	UnityLight light = AdditiveLight(dir, atten);
	UnityIndirect noIndirect = ZeroIndirect();
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);
	return final;
}