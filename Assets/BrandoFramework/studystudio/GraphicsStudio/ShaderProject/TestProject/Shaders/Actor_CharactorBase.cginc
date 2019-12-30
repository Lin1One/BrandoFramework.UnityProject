#include "UnityCG.cginc"
#include "StandardVert.cginc"
#include "StandardLighting.cginc"
#include "LightMapCginc.cginc"


//uniform half _LoginScene = 0;
// half4 _LoginSceneLightDir0;
// half4 _LoginSceneLightColor0;
// half4 _LoginSceneLightDir1;
// half4 _LoginSceneLightColor1;
// half4 _LoginSceneLightDir2;
// half4 _LoginSceneLightColor2;

uniform half _LightStrength = 1.0;
uniform half4 _frozenSatColor = half4(0.5686, 0.6902, 0.8588, 0.702);
uniform half _frozenSaturation = 0.5;
uniform half _frozenContrast = 0.89;
uniform half _GlowScale;

sampler2D _PartMaskTex;

float4 _HitLightPos;
half4 _HitLightColor;
half _HitLightStrenth;
half _HitLightRange;
half _HitLightAttenLimit;
half4 _MainLightFlatDir;

uniform sampler2D _Albedo;
uniform	sampler2D _AddDiffuse;
uniform half4 _AlbedoMainColor;
uniform sampler2D _MaskTex;

uniform sampler2D _NormMap;

uniform half4 _RimLightColor;
uniform half _RimLightScale;
uniform half _RimLightPower;

uniform half4 _PlayerLightColor;
uniform half4 _WorldSpaceLightDir = half4(0,0,1,0);

uniform half4 _SecPlayerLightColor;
uniform half4 _SecWorldSpaceLightDir = half4(0,0,1,0);
		
uniform half4 _RT_FisrtLightColor = half4(0, 0, 0, 1);
uniform half4 _RT_FirstLightDir;
uniform half4 _RT_SecLightColor = half4(0, 0, 0, 1);
uniform half4 _RT_SecLightDir;

uniform half4 _indirectDiffuse;
uniform half _MaxCubeMip;
UNITY_DECLARE_TEXCUBE(_Cube);

// 皮肤
uniform sampler2D _SpecularAOTex;

// 头发(new)
half _SpecularMultiplier, _PrimaryShift, _Specular, _SecondaryShift, _SpecularMultiplier2;
half4 _SpecularColor, _AlbedoColor, _SpecularColor2;

//获取头发高光
fixed StrandSpecular(fixed3 T, fixed3 V, fixed3 L, fixed exponent)
{
	fixed3 H = normalize(L + V);
	fixed dotTH = dot(T, H);
	fixed sinTH = sqrt(1 - dotTH * dotTH);
	fixed dirAtten = smoothstep(-1, 0, dotTH);
	return dirAtten * pow(sinTH, exponent);
}

//沿着法线方向调整Tangent方向
fixed3 ShiftTangent(fixed3 T, fixed3 N, fixed shift)
{
	return normalize(T + shift * N);
}

fixed3 HairShadingKay(fixed3 worldLightDir, fixed3 worldViewDir, fixed3 worldBinormal, fixed3 worldNormal, half shift, half noise = 1.0)
{
	//计算切线方向的偏移度
	half3 t1 = ShiftTangent(worldBinormal, worldNormal, _PrimaryShift + shift);
	half3 t2 = ShiftTangent(worldBinormal, worldNormal, _SecondaryShift + shift);

	//计算高光强度
	half3 spec1 = StrandSpecular(t1, worldViewDir, worldLightDir, _SpecularMultiplier)* _SpecularColor;
	half3 spec2 = StrandSpecular(t2, worldViewDir, worldLightDir, _SpecularMultiplier2)* _SpecularColor2;

	fixed3 finalColor = 0;
	finalColor.rgb = spec1 * _Specular;//第一层高光
	finalColor.rgb += spec2 * _SpecularColor2 * noise * _Specular;//第二层高光
	return finalColor;
}

inline FragmentCommonData SelfSpecularSetup(float4 i_tex, half3 diffRGB)
{
	half2 uv = i_tex.xy;
	half4 mask = tex2D(_MaskTex, uv).rgba;
	half3 specColor = mask.rgb;

	half smoothness = mask.a * _GlossMapScale;

	half3 albedo = _AlbedoMainColor.rgb * diffRGB;

	half oneMinusReflectivity = 1 - max(max(specColor.r, specColor.g), specColor.b);
	half3 diffColor = albedo * oneMinusReflectivity;

	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.smoothness = smoothness;
	return o;
}

inline FragmentCommonData SelfRoughnessSetup(vertexOutput vertextO, half3 diffRGB)
{
	half3 albedo = _AlbedoMainColor.rgb * diffRGB;
	float3 tangent = float3(vertextO.tSpace0.x, 
		vertextO.tSpace1.x, 
		vertextO.tSpace2.x);
	float3 binormal = float3(vertextO.tSpace0.y, 
		vertextO.tSpace1.y, 
		vertextO.tSpace2.y);
	float3 normal = float3(vertextO.tSpace0.z, 
		vertextO.tSpace1.z, 
		vertextO.tSpace2.z);
	half4 mask = tex2D(_MaskTex, vertextO.uv.xy);
	float3 normalTangent;
	normalTangent.xy = mask.rg * 2 - 1;
	normalTangent.z = sqrt(1 - saturate(dot(normalTangent.xy, normalTangent.xy)));

	FragmentCommonData o = (FragmentCommonData)0;
	o.normalWorld = normalize(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z);
#if _INVERSEROUGHNESS_ON
	o.smoothness = mask.a * _GlossMapScale;
#else
	o.smoothness = 1 - mask.a;
#endif
	
	o.specColor = lerp(unity_ColorSpaceDielectricSpec.rgb, albedo, mask.b);
	half oneMinusReflectivity = unity_ColorSpaceDielectricSpec.a - 
		mask.b * unity_ColorSpaceDielectricSpec.a;
	o.diffColor = oneMinusReflectivity * albedo;
	o.diffColor = albedo;
	o.oneMinusReflectivity = oneMinusReflectivity;
	return o;
}

inline half3 MyPerPixelWorldNormal(vertexOutput vertextO, out half rim, out half environment)
{
	half3 tangent = half3(vertextO.tSpace0.x, vertextO.tSpace1.x, vertextO.tSpace2.x);
	half3 binormal = half3(vertextO.tSpace0.y, vertextO.tSpace1.y, vertextO.tSpace2.y);
	half3 normal = half3(vertextO.tSpace0.z, vertextO.tSpace1.z, vertextO.tSpace2.z);
	half4 xxx = tex2D(_NormMap, vertextO.uv.xy);
	rim = xxx.z;
	environment = xxx.w;
	half3 normalTangent;
	normalTangent.xy = xxx.xy * 2 - 1;
	normalTangent.z = sqrt(1 - saturate(dot(normalTangent.xy, normalTangent.xy)));
	half3 normalWorld = normalize(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z);
	return normalWorld;
}

inline FragmentCommonData SelfFragmentSetup(vertexOutput vertextO, out half rim, out half environment)
{
	half4 diffuse = tex2D(_MainTex, vertextO.uv.xy);
	float alpha = diffuse.a * _AlbedoMainColor.a;
#if defined(_ALPHATEST_ON)
	clip(alpha - 0.5f);
#endif

	FragmentCommonData o = SelfSpecularSetup(vertextO.uv, diffuse.rgb);
	half3 normalTex = MyPerPixelWorldNormal(vertextO, rim, environment);
	o.normalWorld = normalTex.xyz;
	o.posWorld = half3(vertextO.tSpace0.w, vertextO.tSpace1.w, vertextO.tSpace2.w);
	o.eyeVec = vertextO.eyeVec;
	o.alpha = alpha;
	return o;
}

inline FragmentCommonData SelfRoughnessFragmentSetup(vertexOutput vertextO)
{
	half4 diffuse = tex2D(_Albedo, vertextO.uv.xy);
	float alpha = diffuse.a * _AlbedoMainColor.a;
#if defined(_ALPHATEST_ON)
	clip(alpha - 0.5f);
#endif

	FragmentCommonData o = SelfRoughnessSetup(vertextO, diffuse.rgb);
	o.posWorld = float3(vertextO.tSpace0.w, vertextO.tSpace1.w, vertextO.tSpace2.w);
	o.eyeVec = vertextO.eyeVec;
	o.alpha = alpha;
	return o;
}
 
inline half4 caculateAddLight(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s)
{
	UnityLight light;
	light.color = lightColor;
	light.dir = dir;	
	UnityIndirect noIndirect = ZeroIndirect();
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);
	return final;
}

inline half3 GetEnv(half roughness, half3 R)
{
	half mip = roughness * _MaxCubeMip;
	half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(_Cube, R, mip);
	return rgbm.xyz;
}

half4 caculateLight(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s, half attenCoef)
{
	#if _REALTIME_ON
		UnityLight mainLight = MainLight();
		UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	#else 
		UnityLight mainLight = MyMainLight(lightColor, normalize(dir));
		UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
		atten = max(atten, attenCoef);
	#endif

	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, mainLight);
	#if _USEAMBIENT_ON
		gi.indirect.diffuse = ShadeSHPerPixel(s.normalWorld, 0, s.posWorld);
	#elif _REALTIME_ON
		gi.indirect.diffuse = _indirectDiffuse;
	#else
		gi.indirect.diffuse = half3(0.1, 0.1, 0.1) * (1 - attenCoef) + _indirectDiffuse * attenCoef;
		
	#endif

	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	return final;
}

half4 RoughnessCaculateLight(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s, half attenCoef, bool reflect = false)
{
#if _REALTIME_ON
	UnityLight mainLight = MainLight();
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
#else 
	UnityLight mainLight = MyMainLight(lightColor, normalize(dir));
	half atten = 1.0;
	//UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	//atten = max(atten, attenCoef);
#endif

	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, mainLight, reflect);
	if (!reflect)
	{
		gi.indirect.diffuse = ShadeSHPerPixel(s.normalWorld, 0, s.posWorld);
	}
	
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	return final;
}

inline half4 caculateAddLight1(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s, half atten = 1.0, bool reflect = true)
{
	UnityLight light;
	light.color = lightColor;// *lerp(0.5, 1, atten);
	light.dir = dir;
	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, light, reflect);
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	return final;
}

inline half4 caculateAddLight4(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s, half atten = 1.0, bool reflect = true)
{
	UnityLight light;
	light.color = lightColor;// *lerp(0.5, 1, atten);
	light.dir = dir;
	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, light, reflect);
	//gi.indirect.diffuse = float3(0.1, 0.1, 0.1);
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	return final;
}

inline half4 caculateAddLight2(half3 lightColor, half3 dir, vertexOutput i, FragmentCommonData s, bool reflect = true)
{
	UnityLight light;
	light.color = lightColor;// *lerp(0.5, 1, atten);
	light.dir = dir;
	
	UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
	UnityGI gi = FragmentGI(s, 1, i.ambientOrLightmapUV, atten, light, reflect);
	
	half4 final = BRDF2_Unity_PBS(s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, gi.indirect);
	return final;
}

half4 GetPbrLighting(FragmentCommonData s, vertexOutput i, half hair = 0.0, half shift = 0.0, half skin = 0.0, half eye = 0.0)
{
	half2 addUV = i.uv.xy;

	//用于额外的贴图
	s.diffColor += (tex2D(_AddDiffuse, i.uv.xy) * step(i.uv.y, 0.328125));

	half3 cameraDir = normalize(_WorldSpaceCameraPos.xyz - s.posWorld);
	half3 mainDir1 = normalize(_GlobalLightDir.xyz);

	half3 tempCameraDir = cameraDir;
	//tempCameraDir.y = 0.01;
	tempCameraDir = normalize(tempCameraDir);
	half3 tempMainDir = -normalize(_MainLightFlatDir.xyz);
	//tempMainDir.y = 0.01;
	tempMainDir = normalize(tempMainDir);
	half doc = dot(tempMainDir, tempCameraDir);

	doc = (doc + 1.0) / 2.0;
	//half angleVL = max(0.3, 1.0 - exp(-5.0*doc))*1.2;
	half angleVL = saturate(max(0.3, doc) * 1.2);

	half4 final = half4(0, 0, 0, 1);
	half weightMin = 0.3;
	half weightMax = 1.0;

	float3 tangent = normalize(float3(i.tSpace0.y, i.tSpace1.y, i.tSpace2.y));
	fixed3 hairSpecColor = 0.0;

#if _REALTIME_ON
	// if (_LoginScene == 1)
	// {
	// 	final = caculateAddLight2(_LoginSceneLightColor0, _LoginSceneLightDir0.xyz, i, s, true);
	// 	final += caculateAddLight2(_LoginSceneLightColor1, _LoginSceneLightDir1.xyz, i, s, false);
	// 	final += caculateAddLight2(_LoginSceneLightColor2, _LoginSceneLightDir2.xyz, i, s, false);


	// 	hairSpecColor = HairShadingKay(_LoginSceneLightDir0.xyz, cameraDir, tangent, s.normalWorld, shift);
	// }
	// else
	// {
		half dayNight = (_LightColor0.r + _LightColor0.g + _LightColor0.b) * 0.3333;
		dayNight = saturate(max(0.2, dayNight)); //白天夜晚的亮度差值

		final = caculateAddLight1(_LightColor0.rgb, -normalize(_MainLightFlatDir.xyz), i, s, 1, true);
		//在背光面和夜晚高光削弱
		s.smoothness *= dayNight * angleVL;
		final += caculateAddLight4(_LightColor0.rgb * (1 - angleVL)*0.3, -i.eyeVec, i, s, 1, false);

		hairSpecColor = HairShadingKay(normalize(-_MainLightFlatDir.xyz), cameraDir, tangent, s.normalWorld, shift) * dayNight * angleVL;
	//}
#else
	hairSpecColor = HairShadingKay(_WorldSpaceLightPos0.xyz, cameraDir, tangent, s.normalWorld, shift)*angleVL;

	if (_LightStrength == 1)
	{
		final = caculateAddLight1(_GlobalLightColor.rgb*_LightMapWeight.x*1.8, -normalize(_MainLightFlatDir.xyz), i, s, 1, true);
		//在背光面和夜晚高光削弱
		s.smoothness *= angleVL;
		final += caculateAddLight4(_GlobalLightColor.rgb *_LightMapWeight.x * 0.4, -i.eyeVec, i, s, 1, false);
	}
	else //无双处理
	{
		half3 mainLi2 = _PlayerLightColor.rgb * _WorldSpaceLightDir.w;
		half3 viewSpaceLightDir = mul(UNITY_MATRIX_V, half4(_WorldSpaceLightDir.xyz, 0)).xyz;
		half3 mainDir2 = normalize(cameraDir + viewSpaceLightDir);
		half4 wsfinal1 = caculateAddLight1(mainLi2, mainDir2, i, s, 1, true);

		half3 viewSpaceSecLightDir = mul(UNITY_MATRIX_V, half4(_SecWorldSpaceLightDir.xyz, 0)).xyz;
		half3 secLightColor = _SecPlayerLightColor.rgb * _SecWorldSpaceLightDir.w;
		half3 secLightDir = normalize(viewSpaceSecLightDir);
		half4 wsfinal2 = caculateAddLight1(secLightColor, secLightDir, i, s, false);
		
		// 宝崽插值效果平滑
		half4 gdFinal = caculateAddLight1(_GlobalLightColor.rgb*_LightMapWeight.x*1.8, -normalize(_MainLightFlatDir.xyz), i, s, 1, true);
		s.smoothness *= angleVL;
		gdFinal += caculateAddLight4(_GlobalLightColor.rgb *_LightMapWeight.x * 0.4, -i.eyeVec, i, s, 1, false);
		final = (wsfinal1 + wsfinal2) * (1 - _LightStrength) + gdFinal * _LightStrength;
	}
#endif

	//边缘光，避免死黑
	//half spowValue = max(1.0 - saturate(dot(s.normalWorld, -s.eyeVec)), 0.008);
	//half3 srimLighting = pow(spowValue, 1.5) * 0.6;
	//final.rgb += srimLighting * s.diffColor;
	
	final.rgb = hairSpecColor * hair + final.rgb;
	final = OutputForward(final, s.alpha);
	#if _HITLIGHT_ON
		float3 lightDirection =  s.posWorld - _HitLightPos;
		float dist = length(lightDirection);
		float hitLightAtten = saturate(1.0 - (dist / _HitLightRange));
		float lightAtten = lerp(_HitLightAttenLimit, 1.0, hitLightAtten);
		half4 hlColor = _HitLightColor * _HitLightStrenth * lightAtten * sign(hitLightAtten);
		final.rgb += MonsterLightWithBlinSpecular(s.normalWorld, s.posWorld, s.diffColor, 1.0 - s.smoothness, s.specColor, -normalize(lightDirection), hlColor, 2.0);
		
		#if _HITLIGHT_ON2
			final.rgb *= _GlowScale;
		#endif
	#endif

	#if _HITLIGHT_ON2
		half powValue = max(1.0 - saturate(dot(s.normalWorld, -s.eyeVec)), 0.008);
		half3 rimLighting = (((_RimLightScale * _RimLightColor.xyz)) * pow(powValue, _RimLightPower));
		final.rgb += rimLighting;
		final.rgb = final.rgb * _GlowScale;
	#endif

	UNITY_APPLY_FOG(i.fogCoord, final);
	#if _FROZENSURF_ON 
		half edgeValue = max(1.0 - saturate(dot(s.normalWorld, -s.eyeVec)), 0.008);
		half3 finalColor = lerp(final.rgb, half3(0.418, 0.677, 0.926), edgeValue);
		// 饱和度调整
		half gray = dot(finalColor.rgb, _frozenSatColor.rgb);
		half3 grayColor = half3(gray, gray, gray);
		finalColor = lerp(grayColor, finalColor, _frozenSaturation);
		// 对比度调整
		finalColor = lerp(half3(0.5, 0.5, 0.5), finalColor, _frozenContrast);
		final = half4(finalColor, final.a);
	#endif

	return final;
}


