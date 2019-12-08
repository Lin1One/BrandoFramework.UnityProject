// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Demo/Reflection"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _ReflectColor("Reflection Color",Color) = (1,1,1,1)
        _ReflectAmount("Reflect Amount",Range(0,1)) = 1
        _CubeMap("Reflection Cubemap",Cube) = "_Skybox" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            fixed4 _ReflectColor;
            half _ReflectAmount;
            samplerCUBE _CubeMap;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal :NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldViewDir : TEXCOORD1;
                float3 worldRefl : TEXCOORD2;
                float4 worldPos :TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
                o.worldRefl = reflect(-o.worldViewDir,o.worldNormal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(i.worldViewDir);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0,dot(worldNormal,worldLightDir));
                fixed3 reflection = texCUBE(_CubeMap,i.worldRefl).rgb * _ReflectColor.rgb;
                fixed3 finalColor =  ambient + lerp(diffuse,reflection,_ReflectAmount);
                return fixed4(finalColor,1);

            }
            ENDCG
        }
    }
}
