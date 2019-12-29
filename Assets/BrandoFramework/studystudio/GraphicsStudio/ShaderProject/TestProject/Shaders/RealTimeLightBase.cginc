#include "LightMapCginc.cginc"
#include "StandardLighting.cginc"

uniform half _Metalic;

struct EZVertexForwardBaseOutput
{
    float4 pos                              : SV_POSITION;
    float4 tex                              : TEXCOORD;
    float3 eyeVec                           : TEXCOORD1;
    float4 tangentToWorldAndPackedData[3]   : TEXCOORD2; // [3x3] tangentToWorld 1x3 viewdirForParallax or worldPos
    half4 ambientOrLightmapUV                : TEXCOORD5; // sh or lightmap uv
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)
    #if _PARALLAXMAP
        float3 posWorld                     : TEXCOORD8;
    #endif
};

#if _PARALLAXMAP
    #define PBS_INWORLDPOS(i) i.posWorld
    #define PBS_INWORLDPOSADD(i) i.posWorld
#else
    #define PBS_INWORLDPOSADD(i) i.posWorld
    #define PBS_INWORLDPOS(i) half3(i.tangentToWorldAndPackedData[0].w,i.tangentToWorldAndPackedData[1].w,i.tangentToWorldAndPackedData[2].w)
#endif

inline half4 EZVertexGIForward(appdata_full v, float3 posWorld, half3 normalWorld)
{
    half4 ambientOrLightmapUV = 0;
    // Static lightmaps
    #ifdef LIGHTMAP_ON
        ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        ambientOrLightmapUV.zw = 0;
    // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
    #elif UNITY_SHOULD_SAMPLE_SH
        #ifdef VERTEXLIGHT_ON
            // Approximated illumination from non-important point lights
            ambientOrLightmapUV.rgb = Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, posWorld, normalWorld);
        #endif

        ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    return ambientOrLightmapUV;
}

EZVertexForwardBaseOutput EZVertFBase(appdata_full v)
{
    EZVertexForwardBaseOutput o;
    UNITY_INITIALIZE_OUTPUT(EZVertexForwardBaseOutput, o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);

    //packed world pos
    o.tangentToWorldAndPackedData[0].w = posWorld.x;
    o.tangentToWorldAndPackedData[1].w = posWorld.y;
    o.tangentToWorldAndPackedData[2].w = posWorld.z;

    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = 0;
    o.tex.xy = v.texcoord.xy;
#ifdef LIGHTMAP_ON
	o.tex.zw = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

    o.eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);

    float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
    float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
    o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
    o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
    o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];

    UNITY_TRANSFER_SHADOW(o, v.texcoord1);

    o.ambientOrLightmapUV = EZVertexGIForward(v, posWorld, normalWorld);

    #ifdef _PARALLAXMA
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.posWorld = half3(o.tangentToWorldAndPackedData[0].w, o.tangentToWorldAndPackedData[1].w, o.tangentToWorldAndPackedData[2].w);
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif

    UNITY_TRANSFER_FOG(o,o.pos);
    return o;
}

struct EZVertexForwardAddOutput 
{
    float4 pos                          : SV_POSITION;
    float4 tex                          : TEXCOORD0;
    float3 eyeVec                       : TEXCOORD1;
    float4 tangentToWorldAndLightDir[3] : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:lightDir]
    float3 posWorld                     : TEXCOORD5;
    UNITY_SHADOW_COORDS(6)
    UNITY_FOG_COORDS(7)

    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if defined(_PARALLAXMAP)
    half3 viewDirForParallax            : TEXCOORD8;
#endif
};

EZVertexForwardAddOutput EZVertForwardAdd(appdata_full v)
{
    EZVertexForwardAddOutput o;
    UNITY_INITIALIZE_OUTPUT(EZVertexForwardAddOutput, o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = 0; 
    o.tex.xy = v.texcoord.xy;
    o.eyeVec = normalize(posWorld.xyz - _WorldSpaceCameraPos); 
    o.posWorld = posWorld.xyz;
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

    float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
    o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
    o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
    o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
    //We need this for shadow receiving
    UNITY_TRANSFER_SHADOW(o, v.texcoord1);

    float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
    #ifndef USING_DIRECTIONAL_LIGHT
        lightDir = normalize(lightDir);
    #endif
    o.tangentToWorldAndLightDir[0].w = lightDir.x;
    o.tangentToWorldAndLightDir[1].w = lightDir.y;
    o.tangentToWorldAndLightDir[2].w = lightDir.z;

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
    #endif

    UNITY_TRANSFER_FOG(o,o.pos);
    return o;
}

inline half3 TranslateNormal(half3 normal, float4 tangentToWorldData[3])
{
	//half3 N;
	//N.x = dot(tangentToWorldData[0].xyz, normal);
	//N.y = dot(tangentToWorldData[1].xyz, normal);
	//N.z = dot(tangentToWorldData[2].xyz, normal);
	//Unity_SafeNormalize(N);
	//return N;
	
	half3 normalWorld = normalize(tangentToWorldData[0].xyz * normal.x + tangentToWorldData[1].xyz * normal.y + tangentToWorldData[2].xyz * normal.z);
	return normalWorld;
}

inline FragmentCommonData RealTimeFragmentSetUp(half4 albedo, half4 wn, float3 i_eyeVec, float4 tangentToWorld[3], float3 i_posWorld, half calcNormal)
{
	FragmentCommonData o = (FragmentCommonData)0;
	o.specColor = CalcSpeculorColorFromMetalic(albedo.rgb, _Metalic);
	
	if (calcNormal == 1)
	{
		o.normalWorld = TranslateNormal(wn.xyz, tangentToWorld);
	}
	else
	{
		o.normalWorld = wn.xyz;
	}

	o.oneMinusReflectivity = 1 - SpecularStrength(o.specColor);
	o.alpha = 1.0h;
	o.diffColor = albedo.rgb * o.oneMinusReflectivity;
	o.smoothness = wn.w;
	o.posWorld = i_posWorld;
	o.eyeVec = i_eyeVec;
	return o;
}