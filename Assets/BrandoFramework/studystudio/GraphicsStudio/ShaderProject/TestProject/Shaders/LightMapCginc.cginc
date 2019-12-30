#ifndef _LIGHTMAP_CGINC
#define _LIGHTMAP_CGINC

uniform float4 _ShadowColor;
uniform float4 _LightMapWeight;

inline half3 GetLightMapDiffuse(half2 uv, out half shadowMask)
{
	shadowMask = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, uv)).r;
	half3 lightMapColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uv));
	return lightMapColor * _LightMapWeight.y + _ShadowColor.rgb * (1.0 - shadowMask);
}

inline half3 CalcLightDiffuse(half3 lightDiffuse, half shadowMask, half3 lightMapDiffuse)
{
	return mul(lightDiffuse, shadowMask) * _LightMapWeight.x + lightMapDiffuse;
}

inline half3 CalcLightDiffuseByUV(half2 uv, half3 lightDiffuse)
{
	half shadowMask;
	half3 lightMapDiffuse = GetLightMapDiffuse(uv, shadowMask);
	return CalcLightDiffuse(lightDiffuse, shadowMask, lightMapDiffuse);
}

inline half3 CalcLightDiffuseMultiDiffuseColor(half3 lightDiffuse, half3 diffuseColor)
{
	return lightDiffuse * diffuseColor;
}

inline half3 CalcLightMapLow(half2 uv, half3 lightDiffuse, half3 diffuseColor)
{
	lightDiffuse = CalcLightDiffuseByUV(uv, lightDiffuse);
	return CalcLightDiffuseMultiDiffuseColor(lightDiffuse, diffuseColor);
}

inline half3 CalcLightMapHigh(half2 uv, half3 lightDiffuse, half3 lightSpecular, half3 diffuseColor)
{
	half shadowMask;
	half3 lightMapDiffuse = GetLightMapDiffuse(uv, shadowMask);
	lightDiffuse = CalcLightDiffuse(lightDiffuse, shadowMask, lightMapDiffuse);
	//lightSpecular = mul(lightSpecular, max(shadowMask, 1)) * _LightMapWeight.x;
	lightSpecular = lightSpecular * _LightMapWeight.x * shadowMask;

	return lightDiffuse * diffuseColor + lightSpecular;
}

#endif