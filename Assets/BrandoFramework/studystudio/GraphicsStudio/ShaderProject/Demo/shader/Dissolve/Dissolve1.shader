
Shader "ShaderStudio/Dissolve1" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseMap("噪声图 (R)", 2D) = "white"{}
		_BurnAmount ("Burn Amount", Range(0.0, 1.0)) = 0.0
		_LineWidth("Burn Line Width", Range(0.0, 0.2)) = 0.1
		_BurnColor("Burn First Color", Color) = (1, 0, 0, 1)
		_FlyThreshold("开始飘散阈值",Float) = 1
		_FlyFactor("飘散距离阈值",Float) = 1
	}
	SubShader 
	{
		Tags 
		{ 
			"RenderType"="Opaque" "Queue"="Geometry"
		}
		Pass 
		{
			Cull Off
			CGPROGRAM
			
			#include "Lighting.cginc"
			
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _NoiseMap;
			float4 _MainTex_ST;
			float4 _BurnMap_ST;
			fixed _BurnAmount;
			fixed _LineWidth;
			fixed4 _BurnColor;
			half _FlyThreshold;
			half _FlyFactor;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				v.vertex.xyz += v.normal * saturate(_BurnAmount - _FlyThreshold) * _FlyFactor;  
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target 
			{
				fixed3 dissolve = tex2D(_NoiseMap, i.uv).rgb;		
				clip(dissolve.r - _BurnAmount);
		
				fixed3 albedo = tex2D(_MainTex, i.uv).rgb;
				fixed3 burnColor = tex2D(_NoiseMap, i.uv).rgb;
				burnColor = burnColor * _BurnColor.rgb;
				fixed t = 1 - smoothstep(0.0, _LineWidth, dissolve.r - _BurnAmount);
				fixed3 finalColor = lerp(albedo, burnColor, t );
				return fixed4(finalColor, 1);
			}
			ENDCG
		}
	}
	FallBack Off
}

