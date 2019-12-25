Shader "GraphicsStudio/Scene/Grass"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Color("植被颜色",Color) = (1,1,1,1)
		_SpecColor("镜面反射颜色",Color) = (0.2,0.2,0.2,1)
		_CutOff("Alpha 裁剪", Range(0,1)) = 0.5
		[Toggle] _Wave("是否摆动",float) = 0
		_WavesControl("植被摆动参数",Vector) = (0.1,2,2,30)//"(风速, (Wave size) Z(Wind amount) (可视距离)"
		[Toggle] _ForceBake("烘焙",float) = 0
	}

	SubShader
	{
		//High Quality
		LOD 200
		Cull off

		pass
		{
			Tags{"LightMode" = "ForwardBase" "DepthReplaceTag" = "On" "RenderType" = "Qpaque"}
			
			CGPROGRAM
			#pragma target 3.0
			#include "Scene_WaveGrassCginc.cginc"
			#pragma vertex waveVert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwddbase 
			//实时
			#pragma multi_compile __ _REALTIME_ON
			#pragma multi_compile __ ATMOSPHERICS_FOG
			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _WAVE_ON
			#pragma skip_variants LIGHTPROBE_SH SPOT POINT POINT_COOKIE SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING

		

		float4 frag(vertexOutput i):SV_TARGET
		{
			float4 diffuseColor = tex2D(_Diffuse,i.uv) * float4(i.color.rgb,1);
			clip(diffuseColor.a - _CutOff);

			float4 color;
		#if _REALTIME_ON && !_FORCEBAKE_ON
			FragmentCommonData fragData = FragmentDataSetup(diffuseColor,i);
			UnityLight mainLight = MainLight();
			UNITY_LIGHT_ATTENUATION(atten, i, fragData.posWorld);
			UnityGI gi = FragmentGI (fragData, 1, i.ambientOrLightmapUV, atten, mainLight, false);
			gi.indirect.diffuse = ShadeSHPerPixel(fragData.normalWorld, 0, fragData.posWorld);
			color = BRDF2_Unity_PBS (fragData.diffColor, 
				fragData.specColor, 
				fragData.oneMinusReflectivity, 
				fragData.smoothness, 
				fragData.normalWorld, 
				-fragData.eyeVec, 
				gi.light,
				gi.indirect);
		#else
			#ifdef LIGHTMAP_ON
				color.rgb = CalcLightMapLow(i.uv.zw, _GlobalLightColor.rgb, diffuseColor.rgb);
			#else
				color.rgb = _GlobalLightColor.rgb * diffuseColor.rgb;
			#endif
		#endif
			color.rgb = color.rgb * _LightStrength;
			color.a = i.color.a;
			UNITY_APPLY_FOG(i.fogCoord, color.rgb);
			return color;
		}
		ENDCG
		}

		Pass 
		{
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#include "Scene_WaveGrassCginc.cginc"

		#pragma vertex waveVertShawdowCaster
		#pragma fragment fragShadowCaster
		#pragma shader_feature _WAVE_ON
		#pragma skip_variants  SHADOWS_SOFT LIGHTPROBE_SH SPOT POINT POINT_COOKIE SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
		ENDCG
		}
	}

	SubShader
	{
		//Medium Quality
		LOD 160
		Cull off

		pass
		{
			Tags {"LightMode" = "Forward" "DepthReplaceTag" = "On" "RenderType" = "Opaque"}
			
			CGPROGRAM
			#include "Scene_WaveGrassCginc.cginc"
			#pragma target 3.0
			#pragma vertex waveVert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma multi_compile __ _REALTIME_ON

			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _WAVE_ON
			#pragma skip_variants LIGHTPROBE_SH SPOT POINT POINT_COOKIE SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING

			float4 frag(vertexOutput i): SV_TARGET
			{
				float4 diffuseColor = tex2D(_Diffuse,i.uv.xy) * float4(i.color.rgb,1);
				clip(diffuseColor.a - _CutOff);

				float4 resultColor;
			#if _REALTIME_ON && !_FORCEBAKE_ON
				FragmentCommonData fragData = FragmentDataSetup(diffuseColor,i);
				UNITY_LIGHT_ATTENUATION(atten,i, fragData.posWorld);
				resultColor.rgb = CalcBlinnSpecular(fragData.normalWorld, 
					-i.eyeVec, 
					fragData.diffColor
					1 - fragData.smoothness,
					fragData.specColor
					normalize(_WorldSpaceLightPos0.xyz),
					_LightColor0.rgb * atten, 
					1.0, 
					0.0);
			#else
				#ifdef LIGHTMAP_ON
					resultColor.rgb = CalcLightMapLow(i.uv.zw, _GlobalLightColor.rgb, diffuseColor.rgb);
				#else
					resultColor.rgb = _GlobalLightColor.rgb * diffuseColor;
				#endif
			#endif
				resultColor.rgb = resultColor.rgb * _LightStrength;
				resultColor.a = i.color.a;
				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return resultColor;
			}
			ENDCG
		}

		pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            
			CGPROGRAM
			#include "Scene_WaveGrassCginc.cginc"
			#pragma vertex waveVertShawdowCaster
            #pragma fragment fragShadowCaster
			#pragma skip_variants  SHADOWS_SOFT LIGHTPROBE_SH SPOT POINT POINT_COOKIE SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING
            ENDCG
        }
	}

	SubShader
    {
		// low quality
		LOD 120
		Cull off
        pass
        {
            Tags {"LightMode" = "ForwardBase" "DepthReplaceTag" = "On" "RenderType" = "Opaque"}
			
			CGPROGRAM
			#include "Scene_WaveGrassCginc.cginc"
			#pragma target 3.0
            #pragma vertex waveVert
            #pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile __ _REALTIME_ON
			#pragma shader_feature _FORCEBAKE_ON
			#pragma shader_feature _WAVE_ON
			#pragma skip_variants LIGHTPROBE_SH SPOT POINT POINT_COOKIE SHADOWS_CUBE VERTEXLIGHT_ON LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK SHADOWS_SCREEN
			
            half4 frag(vertexOutput i) : SV_TARGET
            {
				half4 diffuseColor = tex2D(_Diffuse, i.uv) * half4(i.color.rgb, 1);
			    clip(diffuseColor.a - _CutOff);
				 
				half4 c;
			#if _REALTIME_ON && !_FORCEBAKE_ON
				FragmentCommonData s = FragmentDataSetup(diffuseColor, i);
				c.rgb = CalcBlinnSpecular(s.normalWorld, 
					-i.eyeVec, 
					s.diffColor, 
					1 - s.smoothness, 
					s.specColor, 
					normalize(_WorldSpaceLightPos0.xyz), 
					_LightColor0.rgb, 
					1.0, 
					0.0);
			#else
				#ifdef LIGHTMAP_ON
					c.rgb = CalcLightMapLow(i.uv.zw, _GlobalLightColor.rgb, diffuseColor.rgb);
				#else
					c.rgb = _GlobalLightColor.rgb * diffuseColor;
				#endif
			#endif
				
				c.rgb = c.rgb * _LightStrength;
				c.a = i.color.a;
				UNITY_APPLY_FOG(i.fogCoord, c.rgb);
				return c;	
            }
            ENDCG
        }
    }
	
	Fallback off
	
}
