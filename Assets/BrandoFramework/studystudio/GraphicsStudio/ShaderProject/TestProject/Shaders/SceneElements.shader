Shader "TestProject/SceneElements" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		//GlowScale("    GlowScale", Range(0, 2)) = 1
		[Toggle] 
		_Normal("使用法线", Float) = 0
		_NormMap("    Normal (RG) Smoothness(B)", 2D) = "bump" {}
		_Metalic("    金属度", Range(0, 1)) = 0.5

		_MaskTex("Specular Mask (R), Aplha(G), Emissive or Height(B))", 2D) = "white" {}
		
		[Toggle] 
		_SpecularMask("开启高光遮罩 (mask贴图红通道)", Float) = 0

		[Toggle] 
		_AlphaTest("开启 Alpha (mask贴图绿通道)", Float) = 0
		_CutOff("    Alpha cutoff", Range(0, 0.9)) = 0.5

		[Toggle]
		_Emssive("开启自发光 (mask贴图蓝通道)", Float) = 0
		_PaintColor("    自发光颜色", Color) = (1, 1, 1, 1)
		_PaintScale("    自发光强度", Range(0,10)) = 1

		[Toggle] 
		_UseHeight("使用高度图 (mask贴图蓝通道)", Float) = 0
		
		[Toggle] 
		_ForceBake("强行烘焙", Float) = 0
		
		[Enum(UnityEngine.Rendering.CullMode)] 
		_Cull("Cull mode", Float) = 2
	}

	SubShader
	{
		// high quality
		Tags{"RenderType" = "Opaque" "DepthReplaceTag" = "On"}
		Cull[_Cull]
		LOD 200
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "SceneElementsBase.cginc"

			#pragma target 3.0 
			
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile __ _REALTIME_ON

			#pragma shader_feature _SPECULARMASK_ON
			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _EMSSIVE_ON
			#pragma shader_feature _USEHEIGHT_ON
			#pragma shader_feature _NORMAL_ON
			#pragma shader_feature _ALPHATEST_ON
			

			#pragma skip_variants  LIGHTPROBE_SH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
			
			half4 frag(vertexOutput i) : COLOR
			{
				half3 albedo = 0;
				half3 finalColor = 0;
				half emisScale = 0;
			#if _REALTIME_ON
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				albedo = s.diffColor;
				finalColor = SceneCaculateLight(i, s);
				finalColor += CalcLightMapColor(i, s.diffColor);
			#else
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				albedo = s.diffColor;
				finalColor = SceneCaculateLight(i, s);
			#endif
				
			#if _EMSSIVE_ON
				#if _REALTIME_ON
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * emisScale * _PaintScale;
				#else
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * _PaintColor.rgb * emisScale * _PaintScale;
				#endif
				
				finalColor += emissiveColor;
			#endif

				half4 Color = half4(finalColor * _LightStrength, 1.0h);
				UNITY_APPLY_FOG(i.fogCoord, Color);
				return Color;	
			}
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			Fog{ Mode Off }
			ZWrite On 
			ZTest LEqual 
			Offset 1, 1

			CGPROGRAM
			#include "XGame2.cginc"
			#pragma vertex shadowVert
			#pragma fragment shadowFrag
			#pragma glsl_no_auto_normalization
			#pragma multi_compile_shadowcaster
			#pragma shader_feature _ALPHATEST_ON
			#pragma skip_variants  LIGHTPROBE_SH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
			
			uniform half _CutOff;
			uniform sampler2D _MaskTex;
			half4 shadowFrag(shadowV2f i) : SV_Target
			{
			#if _ALPHATEST_ON
				half3 mask = tex2D(_MaskTex, i.uv);
				clip(mask.g - _CutOff);
			#endif

				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	
	SubShader
	{
		// medium quality
		Tags{"RenderType" = "Opaque" "DepthReplaceTag" = "On"}
		Cull[_Cull]
		LOD 16
		
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "SceneElementsBase.cginc"
			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma shader_feature _SPECULARMASK_ON
			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _EMSSIVE_ON
			#pragma shader_feature _ALPHATEST_ON
			#pragma multi_compile __ _REALTIME_ON
			#pragma skip_variants  LIGHTPROBE_SH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
			
			half4 frag(vertexOutput i) : COLOR
			{
				half3 albedo = 0;
				half3 finalColor = 0;
				half emisScale = 0;
			#if _REALTIME_ON
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);
				finalColor = CalcBlinnSpecular(s.normalWorld, -s.eyeVec, s.diffColor, 1 - s.smoothness, s.specColor, normalize(_WorldSpaceLightPos0.xyz), _LightColor0.rgb * atten, 1.0, 0.0);
				finalColor += CalcLightMapColor(i, s.diffColor);
			#else
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				albedo = s.diffColor;
				half shadowMask;
				half3 lightMapDiffuse = GetLightMapDiffuse(i.uv.zw, shadowMask);
				finalColor = CalcBlinnSpecular(s.normalWorld, -s.eyeVec, s.diffColor, 1 - s.smoothness, s.specColor, normalize(_GlobalLightDir.xyz), _GlobalLightColor.rgb * shadowMask, 1.0, 0.0);
				finalColor += lightMapDiffuse*0.00001; // 仅仅引用下lightMapDiffuse，不然报错
			#endif
				
			#if _EMSSIVE_ON
				#if _REALTIME_ON
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * emisScale * _PaintScale;
				#else
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * _PaintColor.rgb * emisScale * _PaintScale;
				#endif
				
				finalColor += emissiveColor;
			#endif

				half4 Color = half4(finalColor * _LightStrength, 1.0h);
				UNITY_APPLY_FOG(i.fogCoord, Color);
				return Color;	
			}
			ENDCG
		}

		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			Fog{ Mode Off }
			ZWrite On 
			ZTest LEqual 
			Offset 1, 1

			CGPROGRAM
			#include "XGame2.cginc"
			#pragma vertex shadowVert
			#pragma fragment shadowFrag
			#pragma glsl_no_auto_normalization
			#pragma multi_compile_shadowcaster
			#pragma shader_feature _ALPHATEST_ON
			#pragma skip_variants  LIGHTPROBE_SH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
			
			uniform half _CutOff;
			uniform sampler2D _MaskTex;
			half4 shadowFrag(shadowV2f i) : SV_Target
			{
			#if _ALPHATEST_ON
				half3 mask = tex2D(_MaskTex, i.uv);
				clip(mask.g - _CutOff);
			#endif

				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	
	SubShader
	{
		// low quality
		Tags{"RenderType" = "Opaque" "DepthReplaceTag" = "On"}
		Cull[_Cull]
		LOD 120
		
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#include "SceneElementsBase.cginc"
			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma shader_feature _SPECULARMASK_ON
			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _EMSSIVE_ON
			#pragma shader_feature _ALPHATEST_ON
			#pragma multi_compile __ _REALTIME_ON

			#pragma skip_variants  LIGHTPROBE_SH SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK SHADOWS_SCREEN
			
			half4 frag(vertexOutput i) : COLOR
			{
				half3 albedo = 0;
				half3 finalColor = 0;
				half emisScale = 0;
			#if _REALTIME_ON
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				finalColor = CalcBlinnSpecular(s.normalWorld, -s.eyeVec, s.diffColor, 1 - s.smoothness, s.specColor, normalize(_WorldSpaceLightPos0.xyz), _LightColor0.rgb, 1.0, 0.0);
			#else
				FragmentCommonData s = SceneElementFragmentSetup(i, emisScale);
				albedo = s.diffColor;
				half shadowMask;
				half3 lightMapDiffuse = GetLightMapDiffuse(i.uv.zw, shadowMask);
				finalColor = CalcBlinnSpecular(s.normalWorld, -s.eyeVec, s.diffColor, 1 - s.smoothness, s.specColor, normalize(_GlobalLightDir.xyz), _GlobalLightColor.rgb * shadowMask, 1.0, 0.0);
				finalColor += lightMapDiffuse*0.00001; // 仅仅引用下lightMapDiffuse，不然报错
			#endif
				
			#if _EMSSIVE_ON
				#if _REALTIME_ON
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * emisScale * _PaintScale;
				#else
					half3 emissiveColor = albedo * max(_GlobalEmissiveColor.rgb, half3(0.85f, 0.33f, 0.17f)*0.24) * _PaintColor.rgb * emisScale * _PaintScale;
				#endif
				
				finalColor += emissiveColor;
			#endif

				half4 Color = half4(finalColor * _LightStrength, 1.0h);
				UNITY_APPLY_FOG(i.fogCoord, Color);
				return Color;	
			}
			ENDCG
		}
	}
	
	Fallback off
}
