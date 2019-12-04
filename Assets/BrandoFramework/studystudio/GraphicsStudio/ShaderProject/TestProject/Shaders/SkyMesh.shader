// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TestProject/SkyMesh"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[Enum(CullMode)] 
		_CullMode("Cull Mode", float) = 1
		_PaintScale("EMSSIVE", Range(0,2)) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Geometry+500" "PreviewType" = "Skybox" }
		Cull[_CullMode]
		ZWrite Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform half _PaintScale;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 final = tex2D(_MainTex, i.uv);
				final *= _PaintScale;
				return final;
			}
			ENDCG
		}
	}
}
