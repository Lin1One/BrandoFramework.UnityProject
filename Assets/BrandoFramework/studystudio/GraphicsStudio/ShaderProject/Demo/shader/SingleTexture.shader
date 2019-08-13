// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


//BlinnPhong光照模型
Shader "Demo/SingleTexture"
{

    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        // 2D是纹理属性的声明方式。使用一个字符串后跟一个花括号作为它的初始值，
        // “white”是内置纹理的名字，也就是一个全白的纹理
        _MainTex ("Main Tex", 2D) = "white" {}    
        _Specular ("Specular", Color) = (1,1,1,1)    
        _Gloss ("Gloss", Range(8.0, 256)) = 20
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

            fixed4 _Color;
            fixed4 _Specular;
            float _Gloss;

            sampler2D _MainTex;
            // 需要为纹理类型的属性声明一个float4类型的变量_MainTex_ST。
            // 使用纹理名_ST的方式来声明某个纹理的属性。其中，ST是缩放（scale）和平移
            // _MainTex_ST.xy存储的是缩放值，而_MainTex_ST.zw存储的是偏移值
            float4 _MainTex_ST;


            struct a2v 
            {    
                float4 vertex : POSITION;    
                float3 normal : NORMAL;    
                float4 texcoord : TEXCOORD0;
            };

            struct v2f 
            {    
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;    
                float3 worldPos : TEXCOORD1;    
                // 在v2f结构体中添加了用于存储纹理坐标的变量uv，
                // 以便在片元着色器中使用该坐标进行纹理采样。
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);    
                o.worldNormal = UnityObjectToWorldNormal(v.normal);    
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;    
                // 在顶点着色器中，我们使用纹理的属性值_MainTex_ST来对顶点纹理坐标进行变换，
                // 得到最终的纹理坐标。
                // 计算过程是，首先使用缩放属性_MainTex_ST.xy对顶点纹理坐标进行缩放，
                // 然后再使用偏移属性_MainTex_ST.zw对结果进行偏移。

                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                // Unity提供了一个内置宏TRANSFORM_TEX来帮我们计算上述过程。 
                //o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);    
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {   
                //世界空间下的法线方向
                fixed3 worldNormal = normalize(i.worldNormal);
                //世界空间的光照方向。
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                // 使用Cg的tex2D函数对纹理进行采样。
                // 第一个参数是需要被采样的纹理，
                // 第二个参数是一个float2类型的纹理坐标，
                // 它将返回计算得到的纹素值。
                // 使用采样结果和颜色属性_Color的乘积来作为材质的反射率 albedo， 
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

                // albedo 和环境光照相乘得到环境光部分。
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                // 随后，我们使用albedo来计算漫反射光照的结果
                fixed3 diffuse = _LightColor0.rgb * 
                    albedo * 
                    max(0, dot(worldNormal, worldLightDir));

                // 和环境光照、高光反射光照相加后返回。
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));    
                fixed3 halfDir = normalize(worldLightDir + viewDir);    
                fixed3 specular = _LightColor0.rgb * 
                    _Specular.rgb * 
                    pow(max(0, dot(worldNormal, halfDir)), _Gloss);
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
            
        }
    }
    Fallback "Specular"
}
