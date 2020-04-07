Shader "GraphicsStudio/LightModel/HalfLambert"
{
Properties
    {
         _Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
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

            struct a2v 
            {
                //模型顶点的法线信息存储到normal变量中
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            v2f vert (a2v v)
            {
                v2f o;
                // 把顶点位置从模型空间转换到裁剪空间中
                o.pos = UnityObjectToClipPos(v.vertex);    
 
                // 把法线转换到世界空间  
                // 模型空间到世界空间的变换矩阵的逆矩阵_World2Object
                // 归一化操作
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 通过Unity的内置变量UNITY_LIGHTMODEL_AMBIENT得到了环境光部分
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                // 归一化后的世界坐标法线
                fixed3 worldNormal = normalize(i.worldNormal);
                //世界坐标光源方向
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                //半兰伯特量
                fixed halfLambert = dot(worldNormal,worldLightDir) * 0.5 + 0.5;
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * halfLambert;
                //兰伯特模型漫反射
                //fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));

                // 对环境光和漫反射光部分相加，得到最终的光照结果
                fixed3 color = diffuse + ambient;  
                return fixed4(color, 1.0);
            }  
            ENDCG
        }
    }
    Fallback "Diffuse"
}
