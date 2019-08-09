// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Demo/DiffuseVertexLevel"
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
                fixed3 color : COLOR;
            };

            v2f vert (a2v v)
            {
                v2f o;
                // 把顶点位置从模型空间转换到裁剪空间中
                o.pos = UnityObjectToClipPos(v.vertex);    
                // 通过Unity的内置变量UNITY_LIGHTMODEL_AMBIENT得到了环境光部分
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;    
                // 把法线转换到世界空间  
                // 模型空间到世界空间的变换矩阵的逆矩阵_World2Object
                // 归一化操作
                fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));    
                // 光源方向 
                // 归一化操作
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);    
                // Unity提供给我们一个内置变量_LightColor0来访问该Pass处理的光源的颜色和强度信息
                // saturate函数是Cg提供的一种函数，它的作用是可以把参数截取到[0, 1]的范围内
                //最后，与光源的颜色和强度以及材质的漫反射颜色相乘即可得到最终的漫反射光照部分
                fixed3 diffuse = _LightColor0.rgb * 
                    _Diffuse.rgb * 
                    saturate(dot(worldNormal, worldLight));    
                // 对环境光和漫反射光部分相加，得到最终的光照结果
                o.color = diffuse + ambient;    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.color, 1.0);
            }
            
            ENDCG
            
        }
    }
    Fallback "Diffuse"
}
