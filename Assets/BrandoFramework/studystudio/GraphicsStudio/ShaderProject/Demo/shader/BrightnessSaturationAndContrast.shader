// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//shader 名称
Shader "Demo/BrightnessSaturationAndContrast"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "White"{}
		_Brightness("_Brightness",Float) = 1
		_Saturation("_Saturation",Float) = 1
		_Contrast("_Contrast",Float) = 1
    }
    SubShader
    {
        Pass
        {
			ZWrite Off
			Cull Off
			ZTest Off

            CGPROGRAM
			#include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag       


            sampler2D _MainTex;
			half _Brightness;
			half _Saturation;
			half _Contrast;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                half2 uv :TEXCOORD0;
            }; 

            v2f vert(appdata_img v )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
				fixed4 renderTex = tex2D(_MainTex,i.uv);
			    //亮度
                fixed3 finalColor = renderTex.rgb * _Brightness;
				
				//通过对每个颜色分量乘以一个特定的系数再相加得到的。
				//计算该像素对应的亮度值（luminance），
				//从而得到希望的饱和度颜色。
				fixed luminance = 
                    0.2125 * renderTex.r +
					0.7154 * renderTex.g +
					0.0721 * renderTex.b;
				//使用该亮度值创建了一个饱和度为0的颜色值，
				//并使用_Saturation属性在其和上一步得到的颜色之间进行插值
				// 从而得到希望的饱和度颜色
				fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
				finalColor = lerp(luminanceColor, finalColor, _Saturation);
				//创建一个对比度为0的颜色值（各分量均为0.5），
				//再使用_Contrast属性在其和上一步得到的颜色之间进行插值，从而得到最终的处理结果。
				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);    
				finalColor = lerp(avgColor, finalColor, _Contrast);    
				return fixed4(finalColor, renderTex.a);
            }
            ENDCG
        }
    }

	Fallback Off
}
