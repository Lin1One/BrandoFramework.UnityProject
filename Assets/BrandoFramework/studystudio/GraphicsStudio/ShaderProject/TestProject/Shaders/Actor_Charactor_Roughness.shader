Shader "GraphicsStudio/Actor/Charactor_Roughness"
{
	Properties
	{
		_Albedo ("Albedo", 2D) = "white" {}
		_AlbedoMainColor("基础颜色", Color) = (1,1,1,1)
		[NoScaleOffset]_AddDiffuse("第二纹理",2D) = "black"{}
		[Toggle]_AlphaTest("开启 Alpha Test", Float) = 0
		[NoScaleOffset]_MaskTex("Mask贴图,RG 法线， B 金属度， A 粗糙度" , 2D) = "white" {}
		[NoScaleOffset]_SpecularAOTex("R 环境遮挡，G 毛发", 2D) = "white"{}
		[NoScaleOffset]_PartMaskTex("局部Mask，R 头发 Mask， G 眼睛 Mask， B 皮肤 Mask",2D) = "black"{}
		[Toggle]_InverseroughNess("开启 Alpha Test", Float) = 0
		_GlossMapScale("光泽度尺寸", Range(0,2)) = 1

		[Header(............Skin..............)]
		[Toggle] _SkinRender("开启皮肤渲染", Float) = 1
		[Toggle] _AO("开启AO", Float) = 0


		[Header(................Hair.................)]
		[Toggle] _HAIR("开启头发渲染", Float) = 0
		_Specular("高光强度", Range(0, 5)) = 1.0
		_SpecularColor("第一层高光颜色", Color) = (1,1,1,1)
		_SpecularMultiplier("第一层高光宽度", float) = 100.0
		_PrimaryShift("第一层高光偏移", float) = 0.0
		[Space(10)]
		_SpecularColor2("第二层高光颜色", Color) = (0.5,0.5,0.5,1)
		_SpecularMultiplier2("第二层高光宽度", float) = 100.0
		_SecondaryShift("第二层高光偏移", float) = .7

		[Header(................Rim Light.................)]
		[Toggle] _HitLight("边缘光Mask", float) = 0
		_RimLightColor("Rim Light Color", Color) = (1, 1, 1, 1)
		_RimLightScale("Rim Light Scale", float) = 1
		_RimLightPower("Rim Light Power", float) = 3

		[Space(10)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull mode", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "DepthReplaceTag" = "On" }
		Cull[_Cull]

		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
		}

		Pass
		{	
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#include "Actor_CharactorBase.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
		
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _SKINRENDER_ON
			#pragma shader_feature _AO_ON
			#pragma shader_feature _HAIR_ON
			#pragma shader_feature _HITLIGHT_ON
			#pragma multi_compile __ SHADOWS_SCREEN
			#pragma multi_compile __ _REALTIME_ON
			#pragma multi_compile __ _FROZENSURF_ON
			#pragma multi_compile __ ATMOSPHERICS_FOG
			#pragma skip_variants SPOT  LIGHTMAP_SHADOW_MIXING  LIGHTMAP_ON VERTEXLIGHT_ON SHADOWS_CUBE  POINT_COOKIE

			#pragma shader_feature _INVERSEROUGHNESS_ON

			half4 frag(vertexOutput i) : SV_Target
			{
				FragmentCommonData s = SelfRoughnessFragmentSetup(i);
                // 皮肤 & 头发
				half3 specAo = tex2D(_SpecularAOTex, i.uv.xy).rgb;
				half3 mask = tex2D(_PartMaskTex, i.uv.xy).rgb;
				#if !defined(_SKINRENDER_ON)
					mask.b = 0.0; // no ssss
				#endif
				#if !defined(_HAIR_ON)
					mask.r = 0.0; // no hair
				#endif

				#if defined(_AO_ON)
					half ao = specAo.r;
				#else
					half ao = 1;
				#endif

				float4 pbrLighting = GetPbrLighting(s, i, mask.r, specAo.g, mask.b, mask.g) * ao;
				pbrLighting.a = mask.b; // 4s强度
				return pbrLighting;
			}

			ENDCG
		}
	}
}
