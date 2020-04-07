// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Phong_Gouraud 着色为顶点着色
Shader "GraphicsStudio/LightModel/Phong_Gouraud"
{
    Properties
    {
         _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
        //高光反射颜色 
        _Specular("Specular",Color) = (1,1,1,1)
        //高光反射范围
         _Gloss("Gloss",Range(8.0,256)) = 20
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

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
                fixed3 color : COLOR;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);    
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;    
                fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));    
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

                //高光部分
                // 由于Cg的reflect函数的入射方向要求是由光源指向交点处的，
                // 因此需要对worldLightDir取反后再传给reflect函数。
                fixed3 reflectDir = normalize(reflect(-worldLightDir,worldNormal));

                //通过_WorldSpaceCameraPos得到了世界空间中的摄像机位置，
                //再把顶点位置从模型空间变换到世界空间下，
                //再通过和_WorldSpaceCameraPos相减即可得到世界空间下的视角方向。
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
                
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(reflectDir,viewDir)),_Gloss);
                //再和环境光、漫反射光相加存储到最后的颜色中
                o.color = diffuse + ambient + specular;    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color, 1.0);
            }
            
            ENDCG
            
        }
    }
    Fallback "Specular"
}
