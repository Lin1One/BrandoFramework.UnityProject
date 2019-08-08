// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//shader 名称
Shader "Demo/simpleshader"
{
    Properties
    {
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            //顶点着色器代码函数名称
            #pragma vertex vert
            //片元着色器函数名称
            #pragma fragment frag

            //application To vertexshader
            struct a2v 
            {
                //由 Render 提供的数据
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            // 顶点着色器的输出
            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed3 color :COLOR0;
            }; 
            // UNITY_MATRIX_MVP ：模型-观察-投影矩阵
            // mul(UNITY_MATRIX_MVP,*) 会自动转换为 UnityObjectToClipPos 方法
            v2f vert(a2v v )
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.normal * 0.5 + fixed3(0.5,0.5,0.5);
                return o;
            }

            // SV_Target： 限定输出
            float4 frag(v2f i) : SV_Target
            {
                return fixed4(i.color,1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
