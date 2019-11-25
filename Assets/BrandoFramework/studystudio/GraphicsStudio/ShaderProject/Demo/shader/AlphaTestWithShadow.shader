Shader "Demo/AlphaTestWithShadow"
{

    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)     
        _MainTex ("Main Tex", 2D) = "white" {}
        _Cutoff("alpha 测试阈值",Range(0,1)) = 0.5 
    }

    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest"
            //不会受到投影器（Projectors）的影响
            "IgnoreProjector" = "True"
            // RenderType标签可以让Unity把这个Shader归入到提前定义的组（这里就是TransparentCutout组）中，
            // 以指明该Shader是一个使用了透明度测试的Shader。
            "RenderType" = "TransparentCutout"
        }
        Pass
        {
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;


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
                float2 uv : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {  
                fixed4 texColor = tex2D(_MainTex,i.uv);
                clip(texColor.a - _Cutoff);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 disffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal,worldLightDir));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                return fixed4((ambient + disffuse) * atten,1.0);
            }
            ENDCG
            
        }
    }
    Fallback "Transparent/Cutout/VertexLit"
}
