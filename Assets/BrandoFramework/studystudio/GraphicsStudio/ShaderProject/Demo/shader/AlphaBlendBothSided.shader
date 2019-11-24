Shader "Demo/AlphaBlendWithBothSide"
{

    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)     
        _MainTex ("Main Tex", 2D) = "white" {}
        _AlphaScale ("Alpha Scale",Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest"
            //不会受到投影器（Projectors）的影响
            "IgnoreProjector" = "True"
            // RenderType标签可以让Unity把这个Shader归入到提前定义的组（这里就是Transparent组）中，
            // 指明该Shader是一个使用了透明度混合的Shader
            "RenderType" = "Transparent"
        }
        Pass
        {
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            Cull Front
            //Zwrite off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AlphaScale;


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
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {  
                fixed4 texColor = tex2D(_MainTex,i.uv);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 disffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal,worldLightDir));
                return fixed4(ambient + disffuse,texColor.a * _AlphaScale);
            }
            ENDCG
            
        }
        Pass
        {
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            Cull back
            //Zwrite off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AlphaScale;


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
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {  
                fixed4 texColor = tex2D(_MainTex,i.uv);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 disffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal,worldLightDir));
                return fixed4(ambient + disffuse,texColor.a * _AlphaScale);
            }
            ENDCG
            
        }
    }
    Fallback "Transparent/VertexLit"
}
