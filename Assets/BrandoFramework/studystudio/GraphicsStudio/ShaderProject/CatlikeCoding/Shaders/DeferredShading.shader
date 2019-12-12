Shader "Custom/DeferredShading" {
	
	Properties {
	}
	SubShader {
		Pass {
			Cull Off
			ZTest Always
			ZWrite Off
			Blend [_SrcBlend] [_DstBlend]
			//Blend DstColor Zero
			//Blend One One
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "MyDeferredShading.cginc"

			#pragma multi_compile _ SHADOWS_SCREEN
			#pragma multi_compile _ UNITY_HDR_ON

			#pragma target 3.0

			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram

			#pragma exclude_renderers nomrt

			ENDCG
		}

		Pass {
			Cull Off
			ZTest Always
			ZWrite Off
			//Blend DstColor Zero
			Stencil {
				Ref [_StencilNonBackground]
				ReadMask [_StencilNonBackground]
				CompBack Equal
				CompFront Equal
			}

			CGPROGRAM
			#pragma target 3.0
			//#pragma multi_compile _ SHADOWS_SCREEN

			#pragma vertex VertexProgram
			#pragma fragment FragmentProgram
			#pragma exclude_renderers nomrt
			#include "UnityCG.cginc"
			
			sampler2D _LightBuffer;

			struct VertexData {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				};

			struct Interpolators {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators VertexProgram (VertexData v) {
				Interpolators i;
				i.pos = UnityObjectToClipPos(v.vertex);
				i.uv = v.uv;
				return i;
			}


			float4 FragmentProgram (Interpolators i) : SV_Target {
				float4 color = tex2D(_LightBuffer, i.uv);
				return -log2(color);
			}
			ENDCG
		}
	}
}
