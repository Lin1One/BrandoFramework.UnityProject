// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Demo/ToonShading"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _MainTex("Main Tex",2D) = "white" {}
        _Ramp("Ramp Texture",2D) = "white" {}
        _Outline("Qutline",Range(0,1)) = 0.1
        _OutlineColor("Outline Color",Color) = (0,0,0,1)
        _Specular("Specular",Color) = (1,1,1,1)
        _SpecularScale("Specular Scale",Range(0,0.1)) = 0.01
    }
    SubShader
    {
        Pass
        {
            Name "OUTLINE"
            //把正面的三角面片剔除，而只渲染背面。
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _Ramp;
            fixed _Outline;
            fixed4 _OutlineColor;
            fixed4 _Specular;
            fixed _SpecularScale;
            fixed4 _MainTex_ST;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv :TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (a2v v)
            {
                v2f o;
                float4 pos = mul(UNITY_MATRIX_MV,v.vertex); 
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
                normal.z = -0.7;
                pos = pos + float4(normalize(normal),0) * _Outline;
                o.pos = mul(UNITY_MATRIX_P,pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(_OutlineColor.rgb,1);
            }
            ENDCG    
        }

        Pass
        {

            Tags{"LightMode" = "ForwardBase"}
            Cull Off
            CGPROGRAM    
            #pragma vertex vert    
            #pragma fragment frag    
            #pragma multi_compile_fwdbase
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            sampler2D _Ramp;
            fixed _Outline;
            fixed4 _OutlineColor;
            fixed4 _Specular;
            fixed _SpecularScale;
            fixed4 _MainTex_ST;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord :TEXCOORD0;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 uv :TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
            };



            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i):SV_TARGET
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 WorldLightDir = normalize(UnityObjectToWorldNormal(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                //Blinn-Phong模型
                fixed3 worldHalfDir = normalize(WorldLightDir + worldViewDir);
                fixed4 c = tex2D(_MainTex,i.uv);
                fixed3 albedo = c.rgb * _Color.rgb;
                fixed3 ambinet = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
                //半兰伯特漫反射系数
                fixed diff = dot(worldNormal,WorldLightDir);
                diff = (diff * 0.5 + 0.5);// * atten;
                //漫反射系数对渐变纹理_Ramp进行采样
                fixed3 diffuse = _LightColor0.rgb * albedo * 
                    tex2D(_Ramp,float2(diff,diff)).rgb;
                
                fixed spec = dot(worldNormal,worldHalfDir);
                fixed w = fwidth(spec) * 2.0;
                //为了在_SpecularScale为0时，可以完全消除高光反射的光照
                fixed3 specular = _Specular.rgb * lerp(0,1,smoothstep(-w,w,spec + _SpecularScale - 1))
                    * step(0.0001,_SpecularScale);
                return(fixed4(ambinet + diffuse + specular,1.0));
            }
            

            ENDCG
        }
    }
    Fallback "Diffuse"
}
