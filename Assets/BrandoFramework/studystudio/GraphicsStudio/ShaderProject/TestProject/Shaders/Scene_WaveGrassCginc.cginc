#ifndef REALTIME_GRASS_SHADER
#define REALTIME_GRASS_SHADER

#include "UnityStandardCore.cginc"
#include "RealTimeLightBase.cginc"

uniform sampler2D _Diffuse; 
uniform float4 _Diffuse_ST;
uniform half _CutOff;
uniform half4 _WavesControl;

uniform half _LightStrength = 1.0f;

struct vertexOutput
{
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float4 ambientOrLightmapUV : TEXCOORD1;
	float posWorld : TEXCOORD2;
	float3 eyeVec : TEXCOORD3;
	UNITY_SHADOW_COORDS(4)
	UNITY_FOG_COORDS(5)
	half4 color : COLOR;
	float3 normalWorld : NORMAL;
};

void FastSinCos(half4 val, out half4 s, out half4 c)
{
	val = val * 6.408849 - 3.1415927;
	// powers for taylor series
	half4 r5 = val * val;					// wavevec ^ 2
	half4 r6 = r5 * r5;						// wavevec ^ 4;
	half4 r7 = r6 * r5;						// wavevec ^ 6;
	half4 r8 = r6 * r5;						// wavevec ^ 8;

	half4 r1 = r5 * val;					// wavevec ^ 3
	half4 r2 = r1 * r5;						// wavevec ^ 5;
	half4 r3 = r2 * r5;						// wavevec ^ 7;

	//Vectors for taylor's series expansion of sin and cos
	half4 sin7 = { 1, -0.16161616, 0.0083333, -0.00019841 };
	half4 cos8 = { -0.5, 0.041666666, -0.0013888889, 0.000024801587 };
	// sin
	s = val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
	// cos
	c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
}

float4 Wave(appdata_full v)
{
	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	float4 _waveXSize = half4(0.012, 0.02, 0.06, 0.024) * _WavesControl.y;
	float4 _waveZSize = half4 (0.006, .02, 0.02, 0.05) * _WavesControl.y;
	//half4 waveSpeed = half4 (0.3, .5, .4, 1.2) * 4;
	half4 waveSpeed = half4 (1.2, 2, 1.6, 4.8);
	//float4 _waveXmove = float4(0.012, 0.02, -0.06, 0.048) * 2;
	half4 _waveXmove = half4(0.024, 0.04, -0.12, 0.096);
	half4 _waveZmove = half4 (0.006, .02, -0.02, 0.1);
	float4 tempValue = _WavesControl.x * waveSpeed;
	float4 waves = posWorld.x * _waveXSize + posWorld.z * _waveZSize + tempValue * _Time.y;
	half4 halfWaves = frac(waves);
   
	half4 s, c;
	FastSinCos(halfWaves, s, c);
	half waveAmount = v.color.a * _WavesControl.z;

	s = s * s;
	s = s * s;
	s = s * waveAmount;
	half3 waveMove = half3 (dot(s, _waveXmove), 0, dot(s, _waveZmove));
	posWorld.xz -= waveMove.xz * _WavesControl.z;

	return posWorld;
}

vertexOutput vertCore(appdata_full v)
{
	vertexOutput o;
	UNITY_INITIALIZE_OUTPUT(vertexOutput, o);
	o.uv.xy = TRANSFORM_TEX(v.texcoord,_Diffuse);
	#ifdef LIGHTMAP_ON
		o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif

	float4 posWorld;
	#ifdef _WAVE_ON
		posWorld = Wave(v);
	#else
		posWorld = mul(unity_ObjectToWorld,v.vertex);
	#endif

	float distance = length(posWorld.xyz - _WorldSpaceCameraPos.xyz);
	half alpha = 1.0 - saturate((distance - _WavesControl.w)/(_ProjectionParams.z - _WavesControl.w));
	v.vertex = mul(unity_WorldToObject, posWorld);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.color = half4(_Color.rgb,alpha);
	o.eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos);
	o.normalWorld = UnityObjectToWorldNormal(v.normal);
	o.posWorld = posWorld;
	o.ambientOrLightmapUV = EZVertexGIForward(v, posWorld, o.normalWorld);
	UNITY_TRANSFER_SHADOW(o, v.texcoord1);
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

vertexOutput waveVert(appdata_full v)
{
	return vertCore(v);
}

FragmentCommonData FragmentDataSetup(float4 albedo, vertexOutput i)
{
	FragmentCommonData o = (FragmentCommonData)0;
	o.specColor = _SpecColor.rgb;
	half oneMinusReflectivity;
	o.diffColor = EnergyConservationBetweenDiffuseAndSpecular (
		albedo.rgb, o.specColor, oneMinusReflectivity);
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.smoothness = 1.0h;
	o.eyeVec = NormalizePerPixelNormal(i.eyeVec);
	o.posWorld = i.posWorld;
	o.normalWorld = i.normalWorld;	
	
	o.diffColor = PreMultiplyAlpha (o.diffColor, albedo.a, o.oneMinusReflectivity, o.alpha);
	return o;
}

//----------------- ShadowCaster ------------------ //

struct vertexShadowCasterOutput
{
	V2F_SHADOW_CASTER_NOPOS
	float2 uv : TEXCOORD1;
	float4 pos : SV_POSITION;
	half alpha : TEXCOORD2;
};


vertexShadowCasterOutput vertShawdowCasterCore(appdata_full v)
{
	vertexShadowCasterOutput o;
	TRANSFER_SHADOW_CASTER_NOPOS(o,o.pos)
	o.uv = TRANSFORM_TEX(v.texcoord, _Diffuse);

	float4 posWorld;
	#ifdef _WAVE_ON
		posWorld = Wave(v);
	#else
		posWorld = mul(unity_ObjectToWorld, v.vertex);
	#endif

	float distance = length(posWorld.xyz - _WorldSpaceCameraPos.xyz);
	half alpha = 1.0 - saturate(distance - _WavesControl.w / _ProjectionParams.z - _WavesControl.w);
	v.vertex = mul(unity_WorldToObject, posWorld);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.alpha = alpha;
	return o;
}

vertexShadowCasterOutput waveVertShawdowCaster(appdata_full v)
{
	return vertShawdowCasterCore(v);
}

float4 fragShadowCaster(vertexShadowCasterOutput i) : SV_TARGET
{
	float4 diffuseColor = tex2D(_Diffuse, i.uv);
	clip(diffuseColor.a * i.alpha - _CutOff);
	SHADOW_CASTER_FRAGMENT(i)
}

// struct BaseVertexOutput 
// {
// 	float4 pos : SV_POSITION;
// 	float4 uv : TEXCOORD0;
// 	float3 eyeVec : TEXCOORD1;
// 	half4 ambientOrLightmapUV : TEXCOORD2;
// 	float3 posWorld : TEXCOORD3;
// 	UNITY_SHADOW_COORDS(4)
// 	UNITY_FOG_COORDS(5)
// 	half4 color : COLOR;
// 	float3 normalWorld : NORMAL;
// };




// FragmentCommonData FragmentDataSetup(half4 albedo, BaseVertexOutput i)
// {
// 	FragmentCommonData o = (FragmentCommonData)0;
// 	half oneMinusReflectivity;
	
// 	o.specColor = _SpecColor.rgb;
// 	o.diffColor = EnergyConservationBetweenDiffuseAndSpecular (albedo.rgb, o.specColor, /*out*/ oneMinusReflectivity);
// 	o.oneMinusReflectivity = oneMinusReflectivity;
// 	o.smoothness = 1.0h;
// 	o.eyeVec = NormalizePerPixelNormal(i.eyeVec);
// 	o.posWorld = i.posWorld;
// 	o.normalWorld = i.normalWorld;	
	
// 	o.diffColor = PreMultiplyAlpha (o.diffColor, albedo.a, o.oneMinusReflectivity, /*out*/ o.alpha);
// 	return o;
// }



// BaseVertexOutput vertCore(appdata_full v, bool bWave)
// {
// 	BaseVertexOutput o;
// 	UNITY_INITIALIZE_OUTPUT(BaseVertexOutput, o);
// 	o.uv.xy = TRANSFORM_TEX(v.texcoord, _Diffuse);
	
// #ifdef LIGHTMAP_ON
// 	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
// #endif

// 	float4 posWorld;
// 	if (bWave)
// 	{
// 		posWorld = Wave(v);
// 	}
// 	else
// 	{
// 		posWorld = mul(unity_ObjectToWorld, v.vertex);
// 	}
	
// 	float distance = length(posWorld.xyz - _WorldSpaceCameraPos.xyz);
// 	half alpha = 1.0 - saturate(distance - _WavesControl.w / _ProjectionParams.z - _WavesControl.w);
// 	v.vertex = mul(unity_WorldToObject, posWorld);
// 	o.pos = UnityObjectToClipPos(v.vertex);
// 	o.color = half4(_Color.rgb, alpha);

// 	o.eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos);
// 	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
// 	o.normalWorld = normalWorld;
// 	o.posWorld = posWorld;
// 	UNITY_TRANSFER_SHADOW(o, v.texcoord1);
// 	o.ambientOrLightmapUV = EZVertexGIForward(v, posWorld, normalWorld);

// 	UNITY_TRANSFER_FOG(o, o.pos);
// 	return o;
// }

// BaseVertexOutput waveVert(appdata_full v)
// {
// 	return vertCore(v, true);
// }

// BaseVertexOutput vert(appdata_full v)
// {
// 	return vertCore(v, false);
// }

// vertexShadowCasterOutput vertShawdowCasterCore(appdata_full v, bool bWave)
// {
// 	vertexShadowCasterOutput o;
// 	TRANSFER_SHADOW_CASTER_NOPOS(o,o.pos)
// 	o.uv = TRANSFORM_TEX(v.texcoord, _Diffuse);
// 	float4 posWorld;
// 	if (bWave)
// 	{
// 		posWorld = Wave(v);
// 	}
// 	else
// 	{
// 		posWorld = mul(unity_ObjectToWorld, v.vertex);
// 	}

// 	float distance = length(posWorld.xyz - _WorldSpaceCameraPos.xyz);
// 	half alpha = 1.0 - saturate(distance - _WavesControl.w / _ProjectionParams.z - _WavesControl.w);
// 	v.vertex = mul(unity_WorldToObject, posWorld);
// 	o.pos = UnityObjectToClipPos(v.vertex);
// 	o.alpha = alpha;
// 	return o;
// }

// vertexShadowCasterOutput waveVertShawdowCaster(appdata_full v)
// {
// 	return vertShawdowCasterCore(v, true);
// }

// vertexShadowCasterOutput vertShawdowCaster(appdata_full v)
// {
// 	return vertShawdowCasterCore(v, false);
// }

// half4 fragShadowCaster(vertexShadowCasterOutput i) : SV_TARGET
// {
// 	half4 diffuseColor = tex2D(_Diffuse, i.uv);
	
// 	clip(diffuseColor.a * i.alpha - _CutOff);
// 	SHADOW_CASTER_FRAGMENT(i)
// }

#endif