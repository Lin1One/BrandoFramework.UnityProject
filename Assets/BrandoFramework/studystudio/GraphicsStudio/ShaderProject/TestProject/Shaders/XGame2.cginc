// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

#ifndef _XGAME2_CGINC
#define _XGAME2_CGINC
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"
#include "LightMapCginc.cginc"

uniform sampler2D _ShadowMaskMap;
uniform sampler2D _LightColorMap;
//uniform float4 _GlobalLightColor;
uniform half4 _GlobalLightColor;
uniform float4 _GlobalLightDir;
static const half PI = 3.141593h;
//uniform float4 _AmbientColor = float4(1, 1, 1, 1);
uniform half4 _AmbientColor = half4(1, 1, 1, 1);

//uniform float4 _GlobalSecLightColor;
uniform half4 _GlobalSecLightColor;
uniform float4 _GlobalSecLightPos;

//uniform fixed _NightMultiplier;
uniform half _NightMultiplier;

uniform half4 _GlobalEmissiveColor = half4(1, 1, 1, 1);

//uniform fixed _RainSceneElementSmothess = 3;
//uniform fixed _RainSceneElementSpecColor = 3;
//uniform fixed _RainSceneTerrainSmothess = 1.5;
//uniform fixed _RainSceneTerrainSpecColor = 3;
uniform half _RainSceneElementSmothess = 3;
uniform half _RainSceneElementSpecColor = 3;
uniform half _RainSceneTerrainSmothess = 1.5;
uniform half _RainSceneTerrainSpecColor = 3;

uniform half4 _PointLightColor;
uniform half _PointLightIntensity;

struct LightMapVertexOutput 
{
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;
	float4 tSpace0 : TEXCOORD1;
	float4 tSpace1 : TEXCOORD2;
	float4 tSpace2 : TEXCOORD3;
	UNITY_FOG_COORDS(4)
#if SHADOWS_SCREEN
	SHADOW_COORDS(5)
#endif
};

struct shadowV2f
{
	V2F_SHADOW_CASTER;
	float2 uv : TEXCOORD1;
};

struct shadowAppdata
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
};

shadowV2f shadowVert(shadowAppdata v)
{
	shadowV2f o;
	TRANSFER_SHADOW_CASTER(o)
	o.uv = v.texcoord;
	return o;
}


//Roughness���ֳ���
//NoV:���������ߵļн�
//�ֳ���ԽС ���ص���ɫԽ�ӽ�SpecularColor��Խ����ɫԽ��
//Nov Խ�󷵻ص���ɫԽ�ӽ�SpecularColor��ԽС��Խ�ӽ���ɫ
inline half3 EnvBRDFApprox(half3 SpecularColor, half Roughness, half NoV)
{
	// [ Lazarov 2013, "Getting More Physical in Call of Duty: Black Ops II" ]
	// Adaptation to fit our G term.
	const half4 c0 = { -1, -0.0275, -0.572, 0.022 };
	const half4 c1 = { 1, 0.0425, 1.04, -0.04 };
	half4 r = Roughness * c0 + c1; // r.x = 1 - Roughness r.y = 0.0425 - -0.0275*Roughness
	half a004 = min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y; //
	half2 AB = half2(-1.04, 1.04) * a004 + r.zw;

	return SpecularColor * AB.x + AB.y;
}

//Roughness:�ֲڶ�
//RoL���ӽ�
//Roughness Խ�󣬷���ֵ����ROL�仯Խ������
inline half PhongApprox(half Roughness, half RoL)
{
	//half a = Roughness * Roughness;			// 1 mul
	////										//!! Ronin Hack?
	//half a2 = a * a;						// 1 mul
	//a2 = max(a, 0.008);						// avoid underflow in FP16, next sqr should be bigger than 6.1e-5

	//half rcp_a2 = 1.0h / (a2);					// 1 rcp
	half rcp_a2 = exp2(-6.88886882 * Roughness + 6.88886882);
	//rcp_a2 = min(rcp_a2, 10);
	// Spherical Gaussian approximation: pow( x, n ) ~= exp( (n + 0.775) * (x - 1) )
	// Phong: n = 0.5 / a2 - 0.5
	// 0.5 / ln(2), 0.275 / ln(2)
	half c = 0.72134752 * rcp_a2 + 0.39674113;	// 1 mad
	half p = rcp_a2 * exp2(c * RoL - c);		// 2 mad, 1 exp2, 1 mul
												// Total 7 instr
	return min(p, rcp_a2);						// Avoid overflow/underflow on Mali GPUs
}

inline half D_GGX(half roughness, half NoH)
{
	half a = roughness * roughness;
	half a2 = a * a;
	half d = NoH * NoH * (a2 - 1.0) + 1.00001h;
	half d2 = (PI * d * d);
	//return a2 / (PI * d * d);
	return a2 / d2;
}

inline half D_Beckmann(half Roughness, half NoH)
{
	half a = Roughness * Roughness;
	half a2 = a * a;
	half NoH2 = NoH * NoH;
	//return exp((NoH2 - 1) / (a2 * NoH2)) / (PI * a2 * NoH2 * NoH2);
	half a2NoH2 = a2 * NoH2;
	return exp((NoH2 - 1) / a2NoH2) / (PI * a2NoH2 * NoH2);
}

inline half D_BlinnPhong(half roughness, half NoH)
{
	const half c = 1.0 / PI;
	half a = roughness * roughness;
	half a2 = a * a;
	half rcp_a2 = 1.0 / a2;
	half tmp = max(2.0 * rcp_a2 - 2.0, 0);
	return rcp_a2 * c * pow(NoH, tmp);
}

inline half3 Fresnel(half3 specColor, half VoH)
{
	//Raise to power of 5
	half VoH1 = 1.0 - VoH;
	//half VoH5 = VoH1 * VoH1 * VoH1 * VoH1 * VoH1;
	half VoH2 = VoH1 * VoH1;
	half VoH5 = VoH2 * VoH2 * VoH1;
	return specColor + (1.0 - specColor) * VoH5;
}

inline half G_Schlick(half roughness, half NoV, half NoL)
{
	half k = roughness * roughness * 0.5;

	/*half GtermV = NoV * (1.0 - k) + k;
	half GtermL = NoL * (1.0 - k) + k;*/
	half temp = 1.0 - k;
	half GtermV = NoV * temp + k;
	half GtermL = NoL * temp + k;
	return 0.25 / (GtermV * GtermL);
}

inline half3 SpecularHigh(half3 specularColor, half roughness, half NoV, half NoL, half NoH, half VoH)
{
	half G = G_Schlick(roughness, NoV, NoL);
	half3 F = Fresnel(specularColor, VoH);
	half D = D_GGX(roughness, NoH);
	return G * D * F;
}

inline half3 GTerm(half3 HoL, half roughness)
{
	half G = 4.0 * HoL * HoL * (roughness + 0.5);
	return 1.0 / G;
}

inline half3 GetDiffuseAndSpecularColor(half3 albedo, half metallic, out half3 specColor)
{
	//half4 ColorSpaceDielectricSpec = half4(0.04, 0.04, 0.04, 1.0 - 0.04);
	/*half4 ColorSpaceDielectricSpec = half4(0.220916301, 0.220916301, 0.220916301, 1.0 - 0.220916301);*/
	half4 ColorSpaceDielectricSpec = half4(0.220916301, 0.220916301, 0.220916301, 0.779083699);
	specColor = lerp(ColorSpaceDielectricSpec.rgb, albedo, metallic);
	return albedo * (1 - metallic ) * ColorSpaceDielectricSpec.a;
}

inline half BRDFSpecularTerm(half NoL, half NoH, half HoL, half roughness)
{
	half a = roughness * roughness;
	half a2 = a * a;
	half d = NoH * NoH * (a2 - 1.h) + 1.00001h;
	half specularTerm = a / (max(0.32h, HoL) * (1.5h + roughness) * d);
	//half specularTerm = a2 / (max(0.1h, HoL*HoL) * (roughness + 0.5h) * (d * d) * 4);

	specularTerm = specularTerm - 1e-4h;
	specularTerm = clamp(specularTerm, 0.0, 100.0);	// Prevent FP16 overflow on mobiles

	return specularTerm * NoL;
}



inline half EnvBRDFApproxNonmetal(half Roughness, half NoV)
{
	// Same as EnvBRDFApprox( 0.04, Roughness, NoV )
	const half2 c0 = { -1, -0.0275 };
	const half2 c1 = { 1, 0.0425 };
	half2 r = Roughness * c0 + c1;
	return min(r.x * r.x, exp2(-9.28 * NoV)) * r.x + r.y;
}

inline half3 RGBMDecode(half4 rgbm, half MaxValue)
{
	return rgbm.rgb * (rgbm.a * MaxValue);
}

inline half3 CaculateNomal(half3 normaltex)
{
	half3 normal;
	normal.xy = normaltex.xy * 2 - 1;
	normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));

	return normal;
}

inline half3 CaculateN(LightMapVertexOutput i, half3 normal)
{
	half3 N;
	N.x = dot(i.tSpace0.xyz, normal);
	N.y = dot(i.tSpace1.xyz, normal);
	N.z = dot(i.tSpace2.xyz, normal);
	N = Unity_SafeNormalize(N);

	return N;
}

#define REFLECTION_CAPTURE_ROUGHEST_MIP 1
#define REFLECTION_CAPTURE_ROUGHNESS_MIP_SCALE 1.2
/**
* Compute absolute mip for a reflection capture cubemap given a roughness.
*/
inline half CalcCubeMap(half Roughness, half CubemapMaxMip)
{
	// Heuristic that maps roughness to mip level
	// This is done in a way such that a certain mip level will always have the same roughness, regardless of how many mips are in the texture
	// Using more mips in the cubemap just allows sharper reflections to be supported
	half LevelFrom1x1 = REFLECTION_CAPTURE_ROUGHEST_MIP - REFLECTION_CAPTURE_ROUGHNESS_MIP_SCALE * log2(Roughness);
	return CubemapMaxMip - 1 - LevelFrom1x1;
}

LightMapVertexOutput ProcessVertexShader(appdata_full v)
{
	LightMapVertexOutput o;
	o.pos = UnityObjectToClipPos(v.vertex);

	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
#if !_LOW_QUALITY_ON	
	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
	float tangentSign = v.tangent.w * unity_WorldTransformParams.w;
	float3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
	o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
	o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
	o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
#endif	
	o.uv = 0;
	o.uv.xy = v.texcoord.xy;

#ifdef LIGHTMAP_ON
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if SHADOWS_SCREEN
	TRANSFER_SHADOW(o);
#endif
	UNITY_TRANSFER_FOG(o, o.pos);
	return o;
}

inline half3 LightWithBRDFSpecular(LightMapVertexOutput i, half3 normal, half3 albedo, half roughness, half3 specularColor)
{
	half3 N = CaculateN(i, normal);
	half3 worldPos = half3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
	half3 V = normalize(_WorldSpaceCameraPos - worldPos);
	half3 L = normalize(_GlobalLightDir.xyz);
	half NoL = saturate(dot(N, L));
	half3 H = Unity_SafeNormalize(V + L);
	half HoL = saturate(dot(L, H));
	half NoH = saturate(dot(N, H));

	half specularTerm = BRDFSpecularTerm(NoL, NoH, HoL, roughness);
	//half specularTerm = D_BlinnPhong(roughness, NoH);
	half3 lightDiffuse = NoL * _GlobalLightColor.rgb;
	half3 lightSpecular = specularTerm *  specularColor * _GlobalLightColor.rgb;

	half3 diffuseColor = albedo;

#ifdef LIGHTMAP_ON
	return CalcLightMapHigh(i.uv.zw, lightDiffuse, lightSpecular, diffuseColor);
#else
	return lightDiffuse * diffuseColor + lightSpecular;
#endif
}

inline half3 SceneLightWithBRDFSpecular(half2 uv, half3 N, half3 worldPos, half3 albedo, half roughness, half3 specularColor)
{
	half3 V = normalize(_WorldSpaceCameraPos - worldPos);
	half3 L = normalize(_GlobalLightDir.xyz);
	half NoL = saturate(dot(N, L));
	half3 H = Unity_SafeNormalize(V + L);
	half HoL = saturate(dot(L, H));
	half NoH = saturate(dot(N, H));

	half specularTerm = BRDFSpecularTerm(NoL, NoH, HoL, roughness);
	//half specularTerm = D_BlinnPhong(roughness, NoH);
	half3 lightDiffuse = NoL * _GlobalLightColor.rgb;
	half3 lightSpecular = saturate(specularTerm * specularColor * _GlobalLightColor.rgb);
	half3 diffuseColor = albedo;
	return CalcLightMapHigh(uv, lightDiffuse, lightSpecular, diffuseColor);
}

inline half3 SceneLightWithBlinSpecular(half2 uv, half3 N, half3 worldPos, half3 albedo, half roughness, half3 specularColor)
{
	half3 V = normalize(_WorldSpaceCameraPos - worldPos);
	half3 L = normalize(_GlobalLightDir.xyz);
	half NoL = saturate(dot(N, L));
	half3 H = Unity_SafeNormalize(V + L);
	half NoH = saturate(dot(N, H));

	half specularTerm = D_BlinnPhong(roughness, NoH);
	half3 lightDiffuse = NoL * _GlobalLightColor.rgb;
	half3 lightSpecular = saturate(specularTerm * specularColor * _GlobalLightColor.rgb);
	half3 diffuseColor = albedo;
#ifdef LIGHTMAP_ON
	return CalcLightMapHigh(uv, lightDiffuse, lightSpecular, diffuseColor);
#else
	return lightDiffuse * diffuseColor + lightSpecular;
#endif
}

inline void CalcPointLight(
	half3 R,
	half3 L,
	half3 N,
	half3 lightColor,
	half roughness,
	half lambertOffset,
	out half3 lightSpecular,
	out half3 lightDiffuse
)
{
	half NoL = saturate(dot(N, L));
	half RoL = saturate(dot(R, L));
	lightSpecular = PhongApprox(roughness, RoL) * NoL * lightColor;

	half scaledLambert = lerp(lambertOffset, 1.0, NoL);
	lightDiffuse = scaledLambert * lightColor;
}

inline half3 CalcBlinnSpecular(half3 N, half3 eyeVec, half3 albedo, half roughness, half3 specularColor, half3 L, half3 lightColor, half lightWeight, half lambertOffset)
{
	half3 V = eyeVec;
	half NoL = saturate(dot(N, L));
	half3 H = Unity_SafeNormalize(V + L);
	half NoH = saturate(dot(N, H));
	//half specularTerm = D_BlinnPhong(roughness, NoH);
	half specularTerm = D_GGX(roughness, NoH);
	half3 lightDiffuse = lerp(lambertOffset, 1.0, NoL) * lightColor;
	half3 lightSpecular = specularTerm * specularColor * lightColor;
	lightDiffuse = lightDiffuse * lightWeight;
	lightSpecular = max(0, lightSpecular * lightWeight);
	return lightDiffuse * albedo + lightSpecular + UNITY_LIGHTMODEL_AMBIENT * albedo;
}

inline half3 MonsterLightWithBlinSpecular(half3 N, half3 worldPos, half3 albedo, half roughness, half3 specularColor, half3 L, half3 lightColor, half lightWeight)
{
	half3 V = normalize(_WorldSpaceCameraPos - worldPos);
	half NoL = saturate(dot(N, L));
	half3 H = Unity_SafeNormalize(V + L);
	half NoH = saturate(dot(N, H));
	//half HoL = saturate(dot(L, H));

	half specularTerm = D_BlinnPhong(roughness, NoH);
	//half specularTerm = BRDFSpecularTerm(NoL, NoH, HoL, roughness);
	
	half3 lightDiffuse = NoL * lightColor;
	half3 lightSpecular = specularTerm * specularColor * lightColor;

	lightDiffuse = lightDiffuse * lightWeight;
	lightSpecular = max(0, lightSpecular * lightWeight);

	return lightDiffuse * albedo + lightSpecular;
}


inline half3 LightCommonProcess(LightMapVertexOutput i, half3 normal, half3 albedo)
{
#if !_LOW_QUALITY_ON
	half3 lightDiffuse;
	half3 N = CaculateN(i, normal);
	half3 worldPos = half3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
	half3 L = normalize(_GlobalLightDir.xyz);
	half NoL = saturate(dot(N, L));
	//diffuseColor = (1.0 - metal) * albedo;
	//diffuseColor = albedo - (albedo * metal * _MetallicOffset);	// 1 mad
	lightDiffuse = NoL * _GlobalLightColor.rgb;
#else
	half3 lightDiffuse = _GlobalLightColor.rgb;
#endif

	half3 diffuseColor = albedo;

#ifdef LIGHTMAP_ON
	return CalcLightMapLow(i.uv.zw, lightDiffuse, diffuseColor);
#else
	return lightDiffuse * diffuseColor;
#endif
}

#endif