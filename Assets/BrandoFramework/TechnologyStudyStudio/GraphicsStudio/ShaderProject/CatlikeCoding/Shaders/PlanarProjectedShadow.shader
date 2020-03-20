// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GraphicsStudio/PlanarProjectedShadow" {

	Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}


	SubShader {

		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+20" }
		Pass {
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators MyVertexProgram (VertexData v) {
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return i;
			}

			float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
				return tex2D(_MainTex, i.uv) ;//* _Tint;
			}

			ENDCG
		}

		//阴影pass
		Pass
		{
			Name "Shadow"

			//用使用模板测试以保证alpha显示正确
			Stencil
			{
				Ref 0
				Comp equal
				Pass incrWrap
				Fail keep
				ZFail keep
			}

			//透明混合模式
			Blend SrcAlpha OneMinusSrcAlpha

			//关闭深度写入
			ZWrite off

			//深度稍微偏移防止阴影与地面穿插
			Offset -1 , 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _Tint;
			float4 _LightDir;
			float4 _ShadowColor;
			float _ShadowFalloff;

			float3 ShadowProjectPos(float4 vertPos)
			{
				float3 shadowPos;

				//得到顶点的世界空间坐标
				float3 worldPos = mul(unity_ObjectToWorld , vertPos).xyz;

				//灯光方向
				float3 lightDir = normalize(_LightDir.xyz);

				//阴影的世界空间坐标（低于地面的部分不做改变）
				shadowPos.y = min(worldPos .y , _LightDir.w);
				shadowPos.xz = worldPos .xz - 
					lightDir.xz * max(0 , worldPos .y - _LightDir.w) / lightDir.y; 
				return shadowPos;
			}

			v2f vert (appdata v)
			{
				v2f o;

				//得到阴影的世界空间坐标
				float3 shadowPos = ShadowProjectPos(v.vertex);

				//转换到裁切空间
				o.vertex = UnityWorldToClipPos(shadowPos);

				//得到中心点世界坐标
				float3 center =float3( unity_ObjectToWorld[0].w , _LightDir.w , unity_ObjectToWorld[2].w);
				//计算阴影衰减
				float falloff = 1-saturate(distance(shadowPos , center) * _ShadowFalloff);

				//阴影颜色
				o.color = _Tint; 
				o.color.a *= falloff;
				//o.color.a *= 1;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
		
	}
}