// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'


//BlinnPhong光照模型
//为前向渲染定义了Base Pass和Additional Pass来处理多个光源
Shader "Demo/AttenuationAndShadowUse BuildInFunctions"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        _Specular("Specular",Color) = (1,1,1,1)
        _Gloss("Gloss",Range(8.0,256)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            //在Shader中使用光照衰减等光照变量可以被正确赋值
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2) //声明一个对阴影纹理采样的坐标
                //这个宏的参数需要是下一个可用的插值寄存器的索引值
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);      
                //o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                //用于在顶点着色器中计算上一步中声明的阴影纹理坐标       
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed shadow = SHADOW_ATTENUATION(i);
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * 
                    saturate(dot(worldNormal, worldLightDir));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));    
                fixed3 halfDir = normalize(worldLightDir + viewDir);    
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * 
                    pow(max(0, dot(worldNormal, halfDir)), _Gloss); 
                //fixed atten = 1.0;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                //return fixed4(ambient + (diffuse + specular) * atten * shadow, 1.0);
                return fixed4(ambient + (diffuse + specular) * atten, 1.0);
            }
            
            ENDCG
        }

        Pass{
                Tags{"LightMode" = "ForwardAdd"}
                //Additional Pass计算得到的光照结果可以在帧缓存中与之前的光照结果进行叠加。
                //如果没有使用Blend命令的话，Additional Pass会直接覆盖掉之前的光照结果。
                Blend One One

                CGPROGRAM
                #pragma multi_compile_fwdadd_full-shadows
                #pragma vertex vert
                #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;

            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2) //声明一个对阴影纹理采样的坐标
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);      
                //o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(i.worldNormal);
#ifdef USING_DIRECTIONAL_LIGHT        
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
#else        
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
#endif
                //fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * 
                    saturate(dot(worldNormal, worldLightDir));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));    
                fixed3 halfDir = normalize(worldLightDir + viewDir);    
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * 
                    pow(max(0, dot(worldNormal, halfDir)), _Gloss); 

// #ifdef USING_DIRECTIONAL_LIGHT    
//                 fixed atten = 1.0;
// #else
//     #if defined (POINT)
// 				float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
// 				fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
// 	#elif defined (SPOT)
// 				float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
// 				fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
// 	#else
// 				fixed atten = 1.0;
// 	#endif
// #endif
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                //fixed atten = 1.0;
                return fixed4((diffuse + specular) * atten, 1.0);
            }
            ENDCG
            
        }
    }
    FallBack "Diffuse"
}

