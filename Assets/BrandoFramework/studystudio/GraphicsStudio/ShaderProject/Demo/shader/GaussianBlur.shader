Shader "Demo/GaussianBlur"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "White"{}
		_BlurSize("Blur Size", Float) = 1.0
    }
    SubShader
    {
		

		CGINCLUDE
#include "UnityCG.cginc"
		sampler2D _MainTex;
		half4 _MainTex_TexelSize;	//Unity提供的访问xxx纹理对应的每个纹素的大小
		float _BlurSize;

		struct v2f 
		{
			float4 pos : SV_POSITION;
			half2 uv[5] :TEXCOORD0;
		}; 

		fixed4 frag(v2f i) : SV_Target
		{
			//由于它的对称性，我们只需要记录3个高斯权重，
			float weight[3] = {0.4026, 0.2442, 0.0545};
			fixed3 sum = tex2D(_MainTex,i.uv[0]).rgb * weight[0];
			for (int it = 1; it < 3; it++) 
			{        
				sum += tex2D(_MainTex, i.uv[it]).rgb * weight[it];        
				sum += tex2D(_MainTex, i.uv[2*it]).rgb * weight[it];
			}
			return fixed4(sum, 1.0);
		}

		ENDCG

		ZWrite off
		Cull Off
		ZTest Always

        Pass
        {
			NAME "GAUSSIAN_BLUR_VERTICAL"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag  

            v2f vert(appdata_img v )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				half2 uv = v.texcoord;
				//_BlurSize 控制采样偏移距离
				o.uv[0] = uv;
				o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
				o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
				o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
				o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
                return o;
            }
            ENDCG
        }

		Pass 
		{    
			NAME "GAUSSIAN_BLUR_HORIZONTAL"
			CGPROGRAM    
			#pragma vertex vert    
			#pragma fragment frag    

			v2f vert(appdata_img v )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				half2 uv = v.texcoord;
				//_BlurSize 控制采样偏移距离
				o.uv[0] = uv;
				o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.x * 1.0) * _BlurSize;
				o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.x * 1.0) * _BlurSize;
				o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.x * 2.0) * _BlurSize;
				o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.x * 2.0) * _BlurSize;
                return o;
            }
			ENDCG
		}
    }

	Fallback Off

	//fixed luminance(fixed4 color) {    return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;}
}
