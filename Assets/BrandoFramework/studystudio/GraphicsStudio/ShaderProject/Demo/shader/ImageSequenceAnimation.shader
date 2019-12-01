Shader "Demo/ImageSequenceAnimation"
{

    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)     
        _MainTex ("Main Tex", 2D) = "white" {}
        _HorizontalAmount("水平关键帧数量",Float) = 4
        _VerticalAmount("竖直关键帧数量",Float) = 4
        _Speed("Speed",Range(1,100)) = 30
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            Zwrite off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _HorizontalAmount;
            float _VerticalAmount;
            float _Speed;

            struct a2v 
            {    
                float4 vertex : POSITION;    
                float4 texcoord : TEXCOORD0;
            };

            struct v2f 
            {    
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {  
                // t/20,t,2t,3t
                float time = floor(_Time.y * _Speed);
                float row = floor(time/_HorizontalAmount);
                float column = time - row * _HorizontalAmount;
                half2 uv = i.uv + half2(column, -row);    
                uv.x /=  _HorizontalAmount;    
                uv.y /= _VerticalAmount;
                fixed4 c = tex2D(_MainTex,uv);
                c *= _Color;
                return c;
            }
            ENDCG
            
        }
    }
    Fallback "Transparent/VertexLit"
}
