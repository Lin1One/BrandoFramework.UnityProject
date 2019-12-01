Shader "Studio/SimpleWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _WaveLength("波长",float) = 2
        _WavaAmplitude("振幅",Range(0,1)) = 0.3
        _WaveSpeed("Speed",Float ) = 1
        _WaveLength2("波长2",float) = 2
        _WavaAmplitude2("振幅2",Range(0,1)) = 0.3
        _WaveSpeed2("Speed2",Float ) = 1
        _WaveLength3("波长3",float) = 2
        _WavaAmplitude3("振幅3",Range(0,1)) = 0.3
        _WaveSpeed3("Speed3",Float ) = 1

    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
        }
        Pass
        {
            Tags 
            {
                "LightMode"="ForwardBase"
            }
            LOD 200
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

        fixed2 _WaveDir1;
        fixed2 _WaveDir2;
        fixed2 _WaveDir3;

        float _WavaAmplitude;
        float _WaveSpeed;
        float _WaveLength;
        float _WavaAmplitude2;
        float _WaveSpeed2;
        float _WaveLength2;
        float _WavaAmplitude3;
        float _WaveSpeed3;
        float _WaveLength3;   


        float GetWaterHeight(float x,float z)
        {
            float pi = 3.1415;
            _WaveDir1 = (1,1);
            float _WaveFrequency1 = 2 * pi /_WaveLength;

            _WaveDir2 = (0.8,-0.5);
            float _WaveFrequency2 = 2 * pi /_WaveLength2;

            _WaveDir3 = (-0.8,-0.7);
            float _WaveFrequency3 = 2 * pi /_WaveLength3;

            float waveValue1 = _WavaAmplitude * sin(dot(_WaveDir1,float2(x,z)) * _WaveFrequency1 + _Time.y * _WaveSpeed);
            float waveValue2 = _WavaAmplitude2 * sin(dot(_WaveDir2,float2(x,z)) * _WaveFrequency2 + _Time.y * _WaveSpeed2) ;
            float waveValue3 = _WavaAmplitude3 * sin(dot(_WaveDir3,float2(x,z)) * _WaveFrequency3 + _Time.y * _WaveSpeed3) ;
            return  waveValue1 + waveValue2 + waveValue3;
        }

            float3 GetWaterNormal(float x,float z)
            {
                float pi = 3.1415;
                _WaveDir1 = (1,1);
                float _WaveFrequency1 = 2 * pi /_WaveLength;

                _WaveDir2 = (0.8,-0.5);
                float _WaveFrequency2 = 2 * pi /_WaveLength2;

                _WaveDir3 = (-0.8,-0.7);
                float _WaveFrequency3 = 2 * pi /_WaveLength3;

                float waveNormalX1 = _WaveFrequency1 * _WaveDir1.x * _WavaAmplitude * cos(dot(_WaveDir1,float2(x,z)) * _WaveFrequency1 + _Time.y * _WaveSpeed);
                float waveNormalX2 = _WaveFrequency2 * _WaveDir2.x * _WavaAmplitude2 * sin(dot(_WaveDir2,float2(x,z)) * _WaveFrequency2 + _Time.y * _WaveSpeed2) ;
                float waveNormalX3 = _WaveFrequency3 * _WaveDir3.x * _WavaAmplitude3 * sin(dot(_WaveDir3,float2(x,z)) * _WaveFrequency3 + _Time.y * _WaveSpeed3) ;

                float waveNormalY1 = _WaveFrequency1 * _WaveDir1.y * _WavaAmplitude * cos(dot(_WaveDir1,float2(x,z)) * _WaveFrequency1 + _Time.y * _WaveSpeed);
                float waveNormalY2 = _WaveFrequency2 * _WaveDir2.y * _WavaAmplitude2 * sin(dot(_WaveDir2,float2(x,z)) * _WaveFrequency2 + _Time.y * _WaveSpeed2) ;
                float waveNormalY3 = _WaveFrequency3 * _WaveDir3.y * _WavaAmplitude3 * sin(dot(_WaveDir3,float2(x,z)) * _WaveFrequency3 + _Time.y * _WaveSpeed3) ;
                return  float3(waveNormalX1 + waveNormalX2 + waveNormalX3,1,waveNormalY1 + waveNormalY2 + waveNormalY3);
            }
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal :NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 worldPos :TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 worldNormal :TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                float waveValue = GetWaterHeight(v.vertex.x,v.vertex.z);
                v.vertex.y += waveValue;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 newNormal = GetWaterNormal(v.vertex.x,v.vertex.z);
                o.worldNormal = UnityObjectToWorldNormal(newNormal);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 diffuse = _LightColor0.rgb * col.rgb * 
                    saturate(dot(i.worldNormal, worldLightDir));  

                fixed3 reflectDir = normalize(reflect(-worldLightDir, i.worldNormal));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                float _Gloss = 20;
                fixed3 specular = _LightColor0.rgb * col.rgb * 
                    pow(saturate(dot(reflectDir, viewDir)), _Gloss);   
                return  fixed4(diffuse * _Color.rgb + specular,1);
                //(col.x *col.w,col.y*col.w,col.z*col.w,1);
            }
            ENDCG
        }
    }
}
