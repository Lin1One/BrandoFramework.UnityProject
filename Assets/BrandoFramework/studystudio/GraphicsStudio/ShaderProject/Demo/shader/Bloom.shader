Shader "Demo/Bloom"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "White"{}
		//_Bloom是高斯模糊后的较亮区域，
		_Bloom ("Bloom (RGB)", 2D) = "black" {}
		_BlurSize("Blur Size", Float) = 1.0
		//用于提取较亮区域使用的阈值
		_LuminanceThreshold("Luminance Threshold",Float) = 1.0
    }
    SubShader
    {
		CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _Bloom;
		half4 _MainTex_TexelSize;	//Unity提供的访问xxx纹理对应的每个纹素的大小
		float _BlurSize;
		float _LuminanceThreshold;

		struct v2f 
		{
			float4 pos : SV_POSITION;
			half2 uv :TEXCOORD0;
		}; 

		v2f vertExtractBright(appdata_img v) 
		{    
			v2f o;    
			o.pos = UnityObjectToClipPos(v.vertex);    
			o.uv = v.texcoord;    
			return o;
		}

		fixed luminance(fixed4 color) 
		{    
			return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
		}

		//并把结果截取到0～1范围内
		fixed4 fragExtractBright(v2f i) : SV_Target 
		{    
			fixed4 c = tex2D(_MainTex, i.uv);    
			fixed val = clamp(luminance(c) - _LuminanceThreshold, 0.0, 1.0);    
			return c * val;
		}

		
		struct v2fBloom 
		{    
			float4 pos : SV_POSITION;    
			//xy分量对应了_MainTex，即原图像的纹理坐标。而它的zw分量是_Bloom
			half4 uv : TEXCOORD0;
		};

		v2fBloom vertBloom(appdata_img v) 
		{    v2fBloom o;    
			o.pos = UnityObjectToClipPos(v.vertex);    
			o.uv.xy = v.texcoord;    
			o.uv.zw = v.texcoord;    
	#if UNITY_UV_STARTS_AT_TOP    
			//需要对这个纹理坐标进行平台差异化处理
			if (_MainTex_TexelSize.y < 0.0)        
			o.uv.w = 1.0 - o.uv.w;    
	#endif
			return o;
		}

		fixed4 fragBloom(v2fBloom i) : SV_Target 
		{    
			return tex2D(_MainTex, i.uv.xy) + tex2D(_Bloom, i.uv.zw);
		}

		ENDCG

		ZWrite off
		Cull Off
		ZTest Always

        Pass
        {
			CGPROGRAM    
			#pragma vertex vertExtractBright    
			#pragma fragment fragExtractBright    
			ENDCG
		}
		UsePass "Demo/GaussianBlur/GAUSSIAN_BLUR_VERTICAL"
		UsePass "Demo/GaussianBlur/GAUSSIAN_BLUR_HORIZONTAL"
		Pass 
		{    
			CGPROGRAM    
			#pragma vertex vertBloom    
			#pragma fragment fragBloom    
			ENDCG
		}
    }

	Fallback Off

	//fixed luminance(fixed4 color) {    return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;}
}
