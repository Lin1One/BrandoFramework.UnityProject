#ifndef UNITY_STANDARD_CORE_INCLUDED
#define UNITY_STANDARD_CORE_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityStandardInput.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

#include "AutoLight.cginc"
//-------------------------------------------------------------------------------------
// counterpart for NormalizePerPixelNormal
// skips normalization per-vertex and expects normalization to happen per-pixel
// 逐顶点归一化
half3 NormalizePerVertexNormal (float3 n) // takes float to avoid overflow
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return normalize(n);
    #else
        return n; // will normalize per-pixel instead
    #endif
}

//逐像素归一化
float3 NormalizePerPixelNormal (float3 n)
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return n;
    #else
        return normalize((float3)n); // takes float to avoid overflow
    #endif
}

//------------------------------------【函数3】MainLight函数-----------------------------------------
//  用途：该函数为主光照函数
//  说明：实例化一个UnityLight结构体对象，并进行相应的填充
//--------------------------------------------------------------------------------------------------------
UnityLight MainLight ()
{
    UnityLight l;
    l.color = _LightColor0.rgb;
    l.dir = _WorldSpaceLightPos0.xyz;
    return l;
}

//额外光照
UnityLight AdditiveLight (half3 lightDir, half atten)
{
    UnityLight l;

    l.color = _LightColor0.rgb;
    l.dir = lightDir;
    #ifndef USING_DIRECTIONAL_LIGHT
        l.dir = NormalizePerPixelNormal(l.dir);
    #endif

    // shadow the light
    l.color *= atten;
    return l;
}

UnityLight DummyLight ()
{
    UnityLight l;
    l.color = 0;
    l.dir = half3 (0,1,0);
    return l;
}

UnityIndirect ZeroIndirect ()
{
    UnityIndirect ind;
    ind.diffuse = 0;
    ind.specular = 0;
    return ind;
}

//-------------------------------------------------------------------------------------
// Common fragment setup

// deprecated
half3 WorldNormal(half4 tan2world[3])
{
    return normalize(tan2world[2].xyz);
}

// deprecated
#ifdef _TANGENT_TO_WORLD
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        half3 t = tan2world[0].xyz;
        half3 b = tan2world[1].xyz;
        half3 n = tan2world[2].xyz;

    #if UNITY_TANGENT_ORTHONORMALIZE
        n = NormalizePerPixelNormal(n);

        // ortho-normalize Tangent
        t = normalize (t - n * dot(t, n));

        // recalculate Binormal
        half3 newB = cross(n, t);
        b = newB * sign (dot (newB, b));
    #endif

        return half3x3(t, b, n);
    }
#else
    half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
    {
        return half3x3(0,0,0,0,0,0,0,0,0);
    }
#endif

//逐顶点求世界坐标法线
//参数：（UV，TBN矩阵）
float3 PerPixelWorldNormal(float4 i_tex, float4 tangentToWorld[3])
{
#ifdef _NORMALMAP
    half3 tangent = tangentToWorld[0].xyz;
    half3 binormal = tangentToWorld[1].xyz;
    half3 normal = tangentToWorld[2].xyz;

    //正交，需重新计算 B
    #if UNITY_TANGENT_ORTHONORMALIZE
        normal = NormalizePerPixelNormal(normal);

        // ortho-normalize Tangent
        tangent = normalize (tangent - normal * dot(tangent, normal));

        // recalculate Binormal
        half3 newB = cross(normal, tangent);
        binormal = newB * sign (dot (newB, binormal));
    #endif

    //切线坐标法线
    half3 normalTangent = NormalInTangentSpace(i_tex);
    //世界坐标法线
    float3 normalWorld = NormalizePerPixelNormal(
        tangent * normalTangent.x + 
        binormal * normalTangent.y + 
        normal * normalTangent.z);
#else
    float3 normalWorld = normalize(tangentToWorld[2].xyz);
#endif
    return normalWorld;
}

#ifdef _PARALLAXMAP
    #define IN_VIEWDIR4PARALLAX(i) 
        NormalizePerPixelNormal(half3(
            i.tangentToWorldAndPackedData[0].w,
            i.tangentToWorldAndPackedData[1].w,
            i.tangentToWorldAndPackedData[2].w))
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) 
        NormalizePerPixelNormal(i.viewDirForParallax.xyz)
#else
    #define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)
    #define IN_VIEWDIR4PARALLAX_FWDADD(i) half3(0,0,0)
#endif

#if UNITY_REQUIRE_FRAG_WORLDPOS
    #if UNITY_PACK_WORLDPOS_WITH_TANGENT
        #define IN_WORLDPOS(i) half3(
            i.tangentToWorldAndPackedData[0].w,
            i.tangentToWorldAndPackedData[1].w,
            i.tangentToWorldAndPackedData[2].w)
    #else
        #define IN_WORLDPOS(i) i.posWorld
    #endif
    #define IN_WORLDPOS_FWDADD(i) i.posWorld
#else
    #define IN_WORLDPOS(i) half3(0,0,0)
    #define IN_WORLDPOS_FWDADD(i) half3(0,0,0)
#endif

#define IN_LIGHTDIR_FWDADD(i) half3(
    i.tangentToWorldAndLightDir[0].w, 
    i.tangentToWorldAndLightDir[1].w, 
    i.tangentToWorldAndLightDir[2].w)

//片段数据初始化宏
#define FRAGMENT_SETUP(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec.xyz, IN_VIEWDIR4PARALLAX(i), i.tangentToWorldAndPackedData, IN_WORLDPOS(i));

#define FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
    FragmentSetup(i.tex, i.eyeVec.xyz, IN_VIEWDIR4PARALLAX_FWDADD(i), i.tangentToWorldAndLightDir, IN_WORLDPOS_FWDADD(i));

//FragmentCommonData结构体:存放片段着色常用变量
struct FragmentCommonData
{
    half3 diffColor, specColor;//漫反射颜色；镜面反射颜色
    // Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
    // Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
    //注意：出于优化目的，平滑度和 oneMinusReflectivity 主要用于DX9 SM2.0级别。
    //大部分数学运算都是在这些（1-x）值上完成的，这样可以节省一些宝贵的ALU插槽。
    half oneMinusReflectivity, smoothness; //1减去反射率；粗糙度
    float3 normalWorld;//世界空间中的法线向量坐标
    float3 eyeVec;//视角向量坐标
    half alpha;//透明度
    float3 posWorld;//在世界坐标中的位置坐标

#if UNITY_STANDARD_SIMPLE
    half3 reflUVW; //反射率的UVW
#endif

#if UNITY_STANDARD_SIMPLE
    half3 tangentSpaceNormal;//切线空间法线
#endif
};

#ifndef UNITY_SETUP_BRDF_INPUT
    #define UNITY_SETUP_BRDF_INPUT SpecularSetup
#endif

//高光版片段数据初始化--基于高光颜色
inline FragmentCommonData SpecularSetup (float4 i_tex)
{
    half4 specGloss = SpecularGloss(i_tex.xy);
    half3 specColor = specGloss.rgb;
    half smoothness = specGloss.a;

    half oneMinusReflectivity;
    half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

//片段着色器数据初始化--基于粗糙度
inline FragmentCommonData RoughnessSetup(float4 i_tex)
{
    half2 metallicGloss = MetallicRough(i_tex.xy);
    half metallic = metallicGloss.x;
    half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

    half oneMinusReflectivity;
    half3 specColor;
    half3 diffColor = DiffuseAndSpecularFromMetallic(Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

//片段着色器数据初始化--基于金属度
inline FragmentCommonData MetallicSetup (float4 i_tex)
{
    half2 metallicGloss = MetallicGloss(i_tex.xy);
    half metallic = metallicGloss.x;
    half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

    half oneMinusReflectivity;
    half3 specColor;
    half3 diffColor = DiffuseAndSpecularFromMetallic (Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

    FragmentCommonData o = (FragmentCommonData)0;
    o.diffColor = diffColor;
    o.specColor = specColor;
    o.oneMinusReflectivity = oneMinusReflectivity;
    o.smoothness = smoothness;
    return o;
}

// parallax transformed texcoord is used to sample occlusion
//函数FragmentSetup：填充一个FragmentCommonData结构体并于返回值中返回，进行片段函数相关参数的初始化
inline FragmentCommonData FragmentSetup (inout float4 i_tex, float3 i_eyeVec, 
    half3 i_viewDirForParallax, float4 tangentToWorld[3], float3 i_posWorld)
{
    i_tex = Parallax(i_tex, i_viewDirForParallax);

    half alpha = Alpha(i_tex.xy);
    #if defined(_ALPHATEST_ON)
        clip (alpha - _Cutoff);
    #endif

    FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
    o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
    o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
    o.posWorld = i_posWorld;

    // NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
    //着色器依赖于预乘alpha混合（_SrcBlend = One，_DstBlend = OneMinusSrcAlpha）
    o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
    return o;
}

//函数：片段着色部分全局光照的处理函数
inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, 
    half atten, UnityLight light, bool reflections)
{
    //【1】实例化一个UnityGIInput的对象
    UnityGIInput d;
    //【2】填充此UnityGIInput对象的各个值
    d.light = light;
    d.worldPos = s.posWorld;
    d.worldViewDir = -s.eyeVec;
    d.atten = atten;
    #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
        d.ambient = 0;
        d.lightmapUV = i_ambientOrLightmapUV;
    #else
        d.ambient = i_ambientOrLightmapUV.rgb;
        d.lightmapUV = 0;
    #endif

    //反射探针的相关计算
    d.probeHDR[0] = unity_SpecCube0_HDR;
    d.probeHDR[1] = unity_SpecCube1_HDR;
    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
      d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
    #endif
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
      d.boxMax[0] = unity_SpecCube0_BoxMax;
      d.probePosition[0] = unity_SpecCube0_ProbePosition;
      d.boxMax[1] = unity_SpecCube1_BoxMax;
      d.boxMin[1] = unity_SpecCube1_BoxMin;
      d.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

    //【3】根据填充好的UnityGIInput结构体对象，调用一下UnityGlobalIllumination函数
    if(reflections)
    {
        //计算反射的环境数据，包括镜面照明和天空等
        Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
        // Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself
        #if UNITY_STANDARD_SIMPLE
            g.reflUVW = s.reflUVW;
        #endif

        return UnityGlobalIllumination (d, occlusion, s.normalWorld, g);
    }
    else
    {
        return UnityGlobalIllumination (d, occlusion, s.normalWorld);
    }
}

inline UnityGI FragmentGI (FragmentCommonData s, 
    half occlusion, 
    half4 i_ambientOrLightmapUV, 
    half atten, 
    UnityLight light)
{
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, true);
}


//-------------------------------------------------------------------------------------
//-----------------------------【函数OutputForward】----------------------------------------------
// 用途：正向渲染通道输出函数
// 输入参数：一个half4类型的一个颜色值output，一个half型的透明度值alphaFromSurface
// 返回值：经过透明处理的half4型的输出颜色值
//-------------------------------------------------------------------------------------------------
half4 OutputForward (half4 output, half alphaFromSurface)
{
    #if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
        output.a = alphaFromSurface;
    #else
        UNITY_OPAQUE_ALPHA(output.a);
    #endif
    return output;
}

//正向全局顶点光照
inline half4 VertexGIForward(VertexInput v, float3 posWorld, half3 normalWorld)
{
    //【1】定义一个half4型的ambientOrLightmapUV变量，并将四个分量都置为0
    half4 ambientOrLightmapUV = 0;
    //【2】对ambientOrLightmapUV变量的四个分量赋值
    // Static lightmaps
    #ifdef LIGHTMAP_ON //烘焙GI（Bake GI）
        //【2-1】若没有定义LIGHTMAP_OFF（关闭光照贴图）宏，也就是此情况下启用静态的光照贴图，则计算对应的光照贴图坐标
        ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        ambientOrLightmapUV.zw = 0;
    // Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
    // 不使用光照贴图，计算顶点光照颜色
    #elif UNITY_SHOULD_SAMPLE_SH //SH(球谐函数) 在存在GI的情况下是不进行计算的
        #ifdef VERTEXLIGHT_ON
            // Approximated illumination from non-important point lights
            //在正向基础渲染通道中使用，根据4个不同的点光源计算出漫反射光照参数的rgb值
            ambientOrLightmapUV.rgb = Shade4PointLights (
                unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                unity_4LightAtten0, posWorld, normalWorld);
        #endif

        ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON //预计算实时GI（PRGI，Precompute Realtime GI）
        ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    //在启用光照贴图的情况下，其xyzw分量用来存储光照贴图的UV。
    //在不启用光照贴图的情况下，其rgb(xyz)分量用来保存SH计算的颜色。
    return ambientOrLightmapUV;
}

// ------------------------------------------------------------------
//  Base forward pass (directional light, emission, lightmaps, ...)
// 基础前向渲染通道（方向光，自发光，LightMap）
struct VertexOutputForwardBase
{
    UNITY_POSITION(pos);            //float4 pos : SV_POSITION
    float4 tex                            : TEXCOORD0;
    float4 eyeVec                         : TEXCOORD1;    // eyeVec.xyz | fogCoord
    float4 tangentToWorldAndPackedData[3] : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
    half4 ambientOrLightmapUV             : TEXCOORD5;    // SH or Lightmap UV
    UNITY_LIGHTING_COORDS(6,7)

    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
    float3 posWorld                     : TEXCOORD8;
#endif

    UNITY_VERTEX_INPUT_INSTANCE_ID      //为顶点实例化一个ID
    UNITY_VERTEX_OUTPUT_STEREO          //声明该顶点是否位于视线域中,来判断这个顶点是否输出到片段着色器
};

//  用途：正向渲染基础通道的顶点着色函数
//  说明：实例化一个VertexOutputForwardBase结构体对象，并进行相应的填充
//  输入：VertexInput结构体
//  输出：VertexOutputForwardBase结构体
VertexOutputForwardBase vertForwardBase (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    //【1】实例化一个VertexOutputForwardBase结构体对象
    VertexOutputForwardBase o;
    //用Unity内置的宏初始化参数，将给定类型的名称变量初始化为零
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
    //o.instanceID = UNITY_GET_INSTANCE_ID(input)
    UNITY_TRANSFER_INSTANCE_ID(v, o);   
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    //【2】通过物体坐标系到世界坐标系的变换矩阵乘以物体的顶点位置,得到对象在世界坐标系中的位置
    // 根据Shader Mode的不同来将其存储在posWorld（SM3.0）
    // tangentToWorldAndPackedData[3]的w分量用来存储视差视线
    // （SM2.0）中。tangentToWorldAndPackedData[3]的w分量存储世界坐标
    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    #if UNITY_REQUIRE_FRAG_WORLDPOS
        #if UNITY_PACK_WORLDPOS_WITH_TANGENT
            o.tangentToWorldAndPackedData[0].w = posWorld.x;
            o.tangentToWorldAndPackedData[1].w = posWorld.y;
            o.tangentToWorldAndPackedData[2].w = posWorld.z;
        #else
            o.posWorld = posWorld.xyz;
        #endif
    #endif
    
    //【4】输出的顶点位置（像素位置）为模型视图投影矩阵乘以顶点位置，也就是将三维空间中的坐标投影到了二维窗口
    o.pos = UnityObjectToClipPos(v.vertex);
    
    //【5】计算纹理坐标，使用UnityStandardInput.cginc头文件中的辅助函数。
    o.tex = TexCoords(v);
    
    //【6】视线的方向= 对象在世界坐标系中的位置减去摄像机的世界空间位置，并进行逐顶点归一化
    o.eyeVec.xyz = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    
    //【7】计算物体在世界空间中的法线坐标
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    //【8】进行世界空间中的切线相关参数的计算与赋值,切线空间变换到世界空间矩阵
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        //否则，三个分量直接取为0，0和上面计算得到的normalWorld
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    //We need this for shadow receving
    //【9】阴影的获取
    UNITY_TRANSFER_LIGHTING(o, v.uv1);

    //【10】进行顶点正向相关的全局光照操作
    o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);

    //【11】若定义了_PARALLAXMAP宏，则计算视差的视角方向并赋值 
    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif
    //【13】从顶点中输出雾数据
    UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o,o.pos);
    return o;
}

half4 fragForwardBaseInternal (VertexOutputForwardBase i)
{
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
    //定义并初始化类型为FragmentCommonData的变量s 
    FRAGMENT_SETUP(s)
    //设置实例化数据（实例ID，变化矩阵）
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    //设置主光照
    UnityLight mainLight = MainLight ();

    //设置阴影的衰减系数
    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld);

    // Occlusion函数用于进行全局光照的第一步。环境遮挡
    // 其输入参数为一个float2型的纹理坐标，而其half型的返回值将作为FragmentGI函数的一个输入参数。
    half occlusion = Occlusion(i.tex.xy);
    //填充全局光照数据
    UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

    //加上BRDF-基于物理的光照
    half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, 
        s.oneMinusReflectivity, s.smoothness, 
        s.normalWorld, -s.eyeVec, 
        gi.light, gi.indirect);
    //加上自发光
    c.rgb += Emission(i.tex.xy);

    //设置雾效
    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
    UNITY_APPLY_FOG(_unity_fogCoord, c.rgb);
    return OutputForward (c, s.alpha);
}

//  用途：正向渲染基础通道的片段着色函数
//  输入：VertexOutputForwardBase结构体
//  输出：一个half4类型的颜色值
half4 fragForwardBase (VertexOutputForwardBase i) : SV_Target   // backward compatibility (this used to be the fragment entry function)
{
    return fragForwardBaseInternal(i);
}

// -------------------------------- 额外光源通道 ------------------------------------
//  Additive forward pass (one light per pass)

struct VertexOutputForwardAdd
{
    UNITY_POSITION(pos);
    float4 tex                          : TEXCOORD0;
    float4 eyeVec                       : TEXCOORD1;    // eyeVec.xyz | fogCoord
    float4 tangentToWorldAndLightDir[3] : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:lightDir]
    float3 posWorld                     : TEXCOORD5;
    UNITY_LIGHTING_COORDS(6, 7)

    // next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if defined(_PARALLAXMAP)
    half3 viewDirForParallax            : TEXCOORD8;
#endif

    UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputForwardAdd vertForwardAdd (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputForwardAdd o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = TexCoords(v);
    o.eyeVec.xyz = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    o.posWorld = posWorld.xyz;
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndLightDir[0].xyz = 0;
        o.tangentToWorldAndLightDir[1].xyz = 0;
        o.tangentToWorldAndLightDir[2].xyz = normalWorld;
    #endif
    //We need this for shadow receiving and lighting
    UNITY_TRANSFER_LIGHTING(o, v.uv1);

    float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
    #ifndef USING_DIRECTIONAL_LIGHT
        lightDir = NormalizePerVertexNormal(lightDir);
    #endif
    o.tangentToWorldAndLightDir[0].w = lightDir.x;
    o.tangentToWorldAndLightDir[1].w = lightDir.y;
    o.tangentToWorldAndLightDir[2].w = lightDir.z;

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
    #endif

    UNITY_TRANSFER_FOG_COMBINED_WITH_EYE_VEC(o, o.pos);
    return o;
}

half4 fragForwardAddInternal (VertexOutputForwardAdd i)
{
    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

    FRAGMENT_SETUP_FWDADD(s)

    UNITY_LIGHT_ATTENUATION(atten, i, s.posWorld)
    UnityLight light = AdditiveLight (IN_LIGHTDIR_FWDADD(i), atten);
    UnityIndirect noIndirect = ZeroIndirect ();

    half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);

    UNITY_EXTRACT_FOG_FROM_EYE_VEC(i);
    UNITY_APPLY_FOG_COLOR(_unity_fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
    return OutputForward (c, s.alpha);
}

half4 fragForwardAdd (VertexOutputForwardAdd i) : SV_Target     // backward compatibility (this used to be the fragment entry function)
{
    return fragForwardAddInternal(i);
}

// ------------------------------------- 延迟渲染通道 -------------------------------------
//  Deferred pass

struct VertexOutputDeferred
{
    UNITY_POSITION(pos);
    float4 tex                            : TEXCOORD0;
    float3 eyeVec                         : TEXCOORD1;
    float4 tangentToWorldAndPackedData[3] : TEXCOORD2;    // [3x3:tangentToWorld | 1x3:viewDirForParallax or worldPos]
    half4 ambientOrLightmapUV             : TEXCOORD5;    // SH or Lightmap UVs

    #if UNITY_REQUIRE_FRAG_WORLDPOS && !UNITY_PACK_WORLDPOS_WITH_TANGENT
        float3 posWorld                     : TEXCOORD6;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


VertexOutputDeferred vertDeferred (VertexInput v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutputDeferred o;
    UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
    #if UNITY_REQUIRE_FRAG_WORLDPOS
        #if UNITY_PACK_WORLDPOS_WITH_TANGENT
            o.tangentToWorldAndPackedData[0].w = posWorld.x;
            o.tangentToWorldAndPackedData[1].w = posWorld.y;
            o.tangentToWorldAndPackedData[2].w = posWorld.z;
        #else
            o.posWorld = posWorld.xyz;
        #endif
    #endif
    o.pos = UnityObjectToClipPos(v.vertex);

    o.tex = TexCoords(v);
    o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
    float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #ifdef _TANGENT_TO_WORLD
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
        o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
        o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
        o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
    #else
        o.tangentToWorldAndPackedData[0].xyz = 0;
        o.tangentToWorldAndPackedData[1].xyz = 0;
        o.tangentToWorldAndPackedData[2].xyz = normalWorld;
    #endif

    o.ambientOrLightmapUV = 0;
    #ifdef LIGHTMAP_ON
        o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #elif UNITY_SHOULD_SAMPLE_SH
        o.ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, o.ambientOrLightmapUV.rgb);
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif

    #ifdef _PARALLAXMAP
        TANGENT_SPACE_ROTATION;
        half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
        o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
        o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
        o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
    #endif

    return o;
}

void fragDeferred (
    VertexOutputDeferred i,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    ,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
#endif
)
{
    #if (SHADER_TARGET < 30)
        outGBuffer0 = 1;
        outGBuffer1 = 1;
        outGBuffer2 = 0;
        outEmission = 0;
        #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
            outShadowMask = 1;
        #endif
        return;
    #endif

    UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);

    FRAGMENT_SETUP(s)
    UNITY_SETUP_INSTANCE_ID(i);

    // no analytic lights in this pass
    UnityLight dummyLight = DummyLight ();
    half atten = 1;

    // only GI
    half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
    bool sampleReflectionsInDeferred = false;
#else
    bool sampleReflectionsInDeferred = true;
#endif

    UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

    half3 emissiveColor = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;

    #ifdef _EMISSION
        emissiveColor += Emission (i.tex.xy);
    #endif

    #ifndef UNITY_HDR_ON
        emissiveColor.rgb = exp2(-emissiveColor.rgb);
    #endif

    UnityStandardData data;
    data.diffuseColor   = s.diffColor;
    data.occlusion      = occlusion;
    data.specularColor  = s.specColor;
    data.smoothness     = s.smoothness;
    data.normalWorld    = s.normalWorld;

    UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

    // Emissive lighting buffer
    outEmission = half4(emissiveColor, 1);

    // Baked direct lighting occlusion if any
    #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
        outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
    #endif
}


//
// Old FragmentGI signature. Kept only for backward compatibility and will be removed soon
//
//函数：片段着色部分全局光照的处理函数
inline UnityGI FragmentGI(
    float3 posWorld,
    half occlusion, half4 i_ambientOrLightmapUV, 
    half atten, half smoothness, 
    half3 normalWorld, half3 eyeVec,
    UnityLight light,
    bool reflections)
{
    // we init only fields actually used
    FragmentCommonData s = (FragmentCommonData)0;
    s.smoothness = smoothness;
    s.normalWorld = normalWorld;
    s.eyeVec = eyeVec;
    s.posWorld = posWorld;
    return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, reflections);
}
inline UnityGI FragmentGI (
    float3 posWorld,
    half occlusion, 
    half4 i_ambientOrLightmapUV, 
    half atten, 
    half smoothness, 
    half3 normalWorld, 
    half3 eyeVec,
    UnityLight light)
{
    return FragmentGI (posWorld, occlusion, i_ambientOrLightmapUV, atten, smoothness, normalWorld, eyeVec, light, true);
}

#endif // UNITY_STANDARD_CORE_INCLUDED
