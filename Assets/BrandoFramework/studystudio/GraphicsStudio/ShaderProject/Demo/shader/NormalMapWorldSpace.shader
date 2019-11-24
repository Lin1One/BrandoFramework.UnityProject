// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Demo/NormalMapWorldSpace"
{

    Properties 
    {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _MainTex ("Main Tex", 2D) = "white" {} 
        _BumpMap ("Normal Map",2D) = "bump" {}
        _BumpScale("Bump Scale", Float) = 1.0   
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
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _BumpScale;


            struct a2v 
            {    
                float4 vertex : POSITION;    
                float3 normal : NORMAL;   
                //tangent的类型是float4，而非float3，
                // 需要使用tangent.w分量来决定切线空间中的第三个坐标轴——副切线的方向性。
                float4 tangent : TANGENT; 
                float4 texcoord : TEXCOORD0;
            };

            struct v2f 
            {    
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;    
                float4 tangentToW0: TEXCOORD1;
                float4 tangentToW1: TEXCOORD2;
                float4 tangentToW2: TEXCOORD3;
            };

            v2f vert(a2v v) 
            {   
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);    
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                float3 worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                float3 worldBinormal = cross(worldNormal,worldTangent) * v.tangent.w;
                o.tangentToW0 = float4(worldTangent.x,worldBinormal.x,worldNormal.x,worldPos.x);
                o.tangentToW1 = float4(worldTangent.y,worldBinormal.y,worldNormal.y,worldPos.y);
                o.tangentToW2 = float4(worldTangent.z,worldBinormal.z,worldNormal.z,worldPos.z);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {   
                float3 worldPos = float3(i.tangentToW0.w,i.tangentToW1.w,i.tangentToW2.w);
                fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 ViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                fixed3 bump = UnpackNormal(tex2D(_BumpMap,i.uv.zw));
                bump.xy *= _BumpScale;
                bump.z = sqrt(1.0 - saturate(dot(bump.xy,bump.xy)));
                bump = normalize(half3(
                    dot(i.tangentToW0.xyz,bump),
                    dot(i.tangentToW1.xyz,bump),
                    dot(i.tangentToW2.xyz,bump)));
                fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse = _LightColor0.rgb * 
                    albedo * max(0, dot(bump, lightDir));

                fixed3 halfDir = normalize(lightDir + ViewDir);    
                fixed3 specular = _LightColor0.rgb * 
                    _Specular.rgb * pow(max(0, dot(bump, halfDir)), _Gloss);
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
            
        }
    }
    Fallback "Specular"
}
