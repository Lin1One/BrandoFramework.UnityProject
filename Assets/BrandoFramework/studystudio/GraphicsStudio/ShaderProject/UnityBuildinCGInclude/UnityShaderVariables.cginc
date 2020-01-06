#ifndef UNITY_SHADER_VARIABLES_INCLUDED
#define UNITY_SHADER_VARIABLES_INCLUDED

#include "HLSLSupport.cginc"

#if defined (DIRECTIONAL_COOKIE) || defined (DIRECTIONAL)
#define USING_DIRECTIONAL_LIGHT
#endif

#if defined(UNITY_SINGLE_PASS_STEREO) || defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
#define USING_STEREO_MATRICES
#endif

#if defined(USING_STEREO_MATRICES)
    #define glstate_matrix_projection unity_StereoMatrixP[unity_StereoEyeIndex]
    #define unity_MatrixV unity_StereoMatrixV[unity_StereoEyeIndex]
    #define unity_MatrixInvV unity_StereoMatrixInvV[unity_StereoEyeIndex]
    #define unity_MatrixVP unity_StereoMatrixVP[unity_StereoEyeIndex]

    #define unity_CameraProjection unity_StereoCameraProjection[unity_StereoEyeIndex]
    #define unity_CameraInvProjection unity_StereoCameraInvProjection[unity_StereoEyeIndex]
    #define unity_WorldToCamera unity_StereoWorldToCamera[unity_StereoEyeIndex]
    #define unity_CameraToWorld unity_StereoCameraToWorld[unity_StereoEyeIndex]
    #define _WorldSpaceCameraPos unity_StereoWorldSpaceCameraPos[unity_StereoEyeIndex]
#endif

#define UNITY_MATRIX_P glstate_matrix_projection
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_I_V unity_MatrixInvV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_M unity_ObjectToWorld

//环境光颜色
#define UNITY_LIGHTMODEL_AMBIENT (glstate_lightmodel_ambient * 2)

// ----------------------------------------------------------------------------


CBUFFER_START(UnityPerCamera)
    // Time (t = time since current level load) values from Unity
    float4 _Time; // (t/20, t, t*2, t*3)
    float4 _SinTime; // sin(t/8), sin(t/4), sin(t/2), sin(t)
    float4 _CosTime; // cos(t/8), cos(t/4), cos(t/2), cos(t)
    float4 unity_DeltaTime; // dt, 1/dt, smoothdt, 1/smoothdt

#if !defined(USING_STEREO_MATRICES)
    float3 _WorldSpaceCameraPos;//世界空间相机坐标
#endif

    //投影参数
    // x = 1,如果投影翻转则x = -1 || x = 1 or -1 (-1 if projection is flipped) 
    // camera近裁剪平面 || y = near plane
    // camera远裁剪平面 || z = far plane
    // 1/远裁剪平面 || w = 1/far plane
    float4 _ProjectionParams;

    // x = width 屏幕宽度
    // y = height 屏幕高度
    // z = 1 + 1.0/width 1 + 1.0/屏幕宽度
    // w = 1 + 1.0/height 1 + 1.0/height屏幕高度
    // 指像素数
    float4 _ScreenParams;

    // Values used to linearize the Z buffer (http://www.humus.name/temp/Linearize%20depth.txt)
    // x = 1-far/near
    // y = far/near
    // z = x/far
    // w = y/far
    // or in case of a reversed depth buffer (UNITY_REVERSED_Z is 1)
    // x = -1+far/near
    // y = 1
    // z = x/far
    // w = 1/far
    float4 _ZBufferParams;

    // x = 正交相机的宽度 orthographic camera's width 
    // y = 正交相机的高度 orthographic camera's height
    // z = unused
    // w = 在相机为正交投影时为1.0，透视投影时为0.0.|| 1.0 if camera is ortho, 0.0 if perspective
    float4 unity_OrthoParams;
#if defined(STEREO_CUBEMAP_RENDER_ON)
    //x-component is the half stereo separation value, which a positive for right eye and negative for left eye. The y,z,w components are unused.
    float4 unity_HalfStereoSeparation;
#endif
CBUFFER_END


CBUFFER_START(UnityPerCameraRare)
    float4 unity_CameraWorldClipPlanes[6];

#if !defined(USING_STEREO_MATRICES)
    // Projection matrices of the camera. Note that this might be different from projection matrix
    // that is set right now, e.g. while rendering shadows the matrices below are still the projection
    // of original camera.
    float4x4 unity_CameraProjection;
    float4x4 unity_CameraInvProjection;
    float4x4 unity_WorldToCamera;
    float4x4 unity_CameraToWorld;
#endif
CBUFFER_END

// ----------------------------------------光照参数------------------------------------

CBUFFER_START(UnityLighting)

    #ifdef USING_DIRECTIONAL_LIGHT
        half4 _WorldSpaceLightPos0;
    #else
        float4 _WorldSpaceLightPos0;
    #endif

    // xyz = pos
    // w = 1/range 
    float4 _LightPositionRange; 


    // 点光源投射参数 for point light projection: 
    // x = zfar / (znear - zfar)
    // y = (znear * zfar) / (znear - zfar)
    // z= shadow bias
    // w= shadow scale bias
    float4 _LightProjectionParams; 

    //世界空间四个顶点光源的position的x，y,z坐标，衰减
    float4 unity_4LightPosX0;
    float4 unity_4LightPosY0;
    float4 unity_4LightPosZ0;
    half4 unity_4LightAtten0;

    //点光源的颜色
    half4 unity_LightColor[8];

    // view space的点光源位置，其值为(position,1)。
    //如果为directional light 则其值为方向(-direction,0)
    float4 unity_LightPosition[8]; // view-space vertex light positions (position,1), or (-direction,0) for directional lights.

    // x = cos(spotAngle/2) or -1 for non-spot
    // y = 1/cos(spotAngle/4) or 1 for non-spot
    // z = quadratic attenuation
    // w = range*range
    half4 unity_LightAtten[8];

    //view space 的spot light的方向，如果无spot light则其值为(0,0,1,0)
    float4 unity_SpotDirection[8]; // view-space spot light directions, or (0,0,1,0) for non-spot

    // SH lighting environment
    // 球谐光照参数
    half4 unity_SHAr;
    half4 unity_SHAg;
    half4 unity_SHAb;
    half4 unity_SHBr;
    half4 unity_SHBg;
    half4 unity_SHBb;
    half4 unity_SHC;

    // part of Light because it can be used outside of shadow distance
    fixed4 unity_OcclusionMaskSelector;
    fixed4 unity_ProbesOcclusion;
CBUFFER_END

CBUFFER_START(UnityLightingOld)
    half3 unity_LightColor0, unity_LightColor1, unity_LightColor2, unity_LightColor3; // keeping those only for any existing shaders; remove in 4.0
CBUFFER_END


// -----------------------------------------阴影参数-----------------------------------

CBUFFER_START(UnityShadows)
    float4 unity_ShadowSplitSpheres[4];
    float4 unity_ShadowSplitSqRadii;
    float4 unity_LightShadowBias;
    float4 _LightSplitsNear;
    float4 _LightSplitsFar;
    // position点从世界坐标转到阴影空间，通常用来计算阴影坐标Shadow coordinate
    float4x4 unity_WorldToShadow[4];
    half4 _LightShadowData;                 //阴影数据
    float4 unity_ShadowFadeCenterAndType;
CBUFFER_END

// ------------------------------------每次绘制参数----------------------------------------

CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
    float4 unity_LODFade; // x is the fade value ranging within [0,1]. y is x quantized into 16 levels
    // w 分量用于标定奇数负比例变换（odd-negativescale transforms），通常取值为1.0或者-1.0。
    float4 unity_WorldTransformParams; // w is usually 1.0, or -1.0 for odd-negative scale transforms
    float4 unity_RenderingLayer;
CBUFFER_END

#if defined(USING_STEREO_MATRICES)
GLOBAL_CBUFFER_START(UnityStereoGlobals)
    float4x4 unity_StereoMatrixP[2];
    float4x4 unity_StereoMatrixV[2];
    float4x4 unity_StereoMatrixInvV[2];
    float4x4 unity_StereoMatrixVP[2];

    float4x4 unity_StereoCameraProjection[2];
    float4x4 unity_StereoCameraInvProjection[2];
    float4x4 unity_StereoWorldToCamera[2];
    float4x4 unity_StereoCameraToWorld[2];

    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
GLOBAL_CBUFFER_END
#endif

#if defined(USING_STEREO_MATRICES) && defined(UNITY_STEREO_MULTIVIEW_ENABLED)
GLOBAL_CBUFFER_START(UnityStereoEyeIndices)
    float4 unity_StereoEyeIndices[2];
GLOBAL_CBUFFER_END
#endif

#if defined(UNITY_STEREO_MULTIVIEW_ENABLED) && defined(SHADER_STAGE_VERTEX)
    #define unity_StereoEyeIndex UNITY_VIEWID
    UNITY_DECLARE_MULTIVIEW(2);
#elif defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
    static uint unity_StereoEyeIndex;
#elif defined(UNITY_SINGLE_PASS_STEREO)
    GLOBAL_CBUFFER_START(UnityStereoEyeIndex)
        int unity_StereoEyeIndex;
    GLOBAL_CBUFFER_END
#endif

CBUFFER_START(UnityPerDrawRare)
    float4x4 glstate_matrix_transpose_modelview0;
CBUFFER_END


// --------------------------------------- 每帧参数 -------------------------------------

CBUFFER_START(UnityPerFrame)
    //环境光颜色
    fixed4 glstate_lightmodel_ambient;
    fixed4 unity_AmbientSky;
    fixed4 unity_AmbientEquator;
    fixed4 unity_AmbientGround;
    fixed4 unity_IndirectSpecColor;

#if !defined(USING_STEREO_MATRICES)
    float4x4 glstate_matrix_projection;
    float4x4 unity_MatrixV;
    float4x4 unity_MatrixInvV;
    float4x4 unity_MatrixVP;
    int unity_StereoEyeIndex;
#endif

    fixed4 unity_ShadowColor;
CBUFFER_END


// ----------------------------------------------------------------------------

CBUFFER_START(UnityFog)
    fixed4 unity_FogColor;
    // x = density / sqrt(ln(2)), useful for Exp2 mode
    // y = density / ln(2), useful for Exp mode
    // z = -1/(end-start), useful for Linear mode
    // w = end/(end-start), useful for Linear mode
    float4 unity_FogParams;
CBUFFER_END


// ----------------------------------------------------------------------------
// Lightmaps

// Main lightmap
UNITY_DECLARE_TEX2D_HALF(unity_Lightmap);
// Directional lightmap (always used with unity_Lightmap, so can share sampler)
UNITY_DECLARE_TEX2D_NOSAMPLER_HALF(unity_LightmapInd);
// Shadowmasks
UNITY_DECLARE_TEX2D(unity_ShadowMask);

// Dynamic GI lightmap
UNITY_DECLARE_TEX2D(unity_DynamicLightmap);
UNITY_DECLARE_TEX2D_NOSAMPLER(unity_DynamicDirectionality);
UNITY_DECLARE_TEX2D_NOSAMPLER(unity_DynamicNormal);

CBUFFER_START(UnityLightmaps)
    float4 unity_LightmapST;    //光照贴图缩放偏移
    float4 unity_DynamicLightmapST;
CBUFFER_END


// ------------------------------------反射探针参数----------------------------------------
// Reflection Probes

//声明了一个TextureCube类型的对象。
UNITY_DECLARE_TEXCUBE(unity_SpecCube0);
//声明了一个TextureCube类型的对象（无Sampler）。
UNITY_DECLARE_TEXCUBE_NOSAMPLER(unity_SpecCube1);

CBUFFER_START(UnityReflectionProbes)
    float4 unity_SpecCube0_BoxMax;
    float4 unity_SpecCube0_BoxMin;
    float4 unity_SpecCube0_ProbePosition;
    half4  unity_SpecCube0_HDR;

    float4 unity_SpecCube1_BoxMax;
    float4 unity_SpecCube1_BoxMin;
    float4 unity_SpecCube1_ProbePosition;
    half4  unity_SpecCube1_HDR;
CBUFFER_END


// ----------------------------------------------------------------------------
// Light Probe Proxy Volume

// UNITY_LIGHT_PROBE_PROXY_VOLUME is used as a shader keyword coming from tier settings and may be also disabled with nolppv pragma.
// We need to convert it to 0/1 and doing a second check for safety.
#ifdef UNITY_LIGHT_PROBE_PROXY_VOLUME
    #undef UNITY_LIGHT_PROBE_PROXY_VOLUME
    // Requires quite modern graphics support (3D float textures with filtering)
    // Note: Keep this in synch with the list from LightProbeProxyVolume::HasHardwareSupport && SurfaceCompiler::IsLPPVAvailableForAnyTargetPlatform
    #if !defined(UNITY_NO_LPPV) && (defined (SHADER_API_D3D11) || defined (SHADER_API_D3D12) || defined (SHADER_API_GLCORE) || defined (SHADER_API_XBOXONE) || defined (SHADER_API_PSSL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_METAL) || defined(SHADER_API_SWITCH))
        #define UNITY_LIGHT_PROBE_PROXY_VOLUME 1
    #else
        #define UNITY_LIGHT_PROBE_PROXY_VOLUME 0
    #endif
#else
    #define UNITY_LIGHT_PROBE_PROXY_VOLUME 0
#endif

#if UNITY_LIGHT_PROBE_PROXY_VOLUME
    UNITY_DECLARE_TEX3D_FLOAT(unity_ProbeVolumeSH);

    CBUFFER_START(UnityProbeVolume)
        // x = Disabled(0)/Enabled(1)
        // y = Computation are done in global space(0) or local space(1)
        // z = Texel size on U texture coordinate
        float4 unity_ProbeVolumeParams;

        float4x4 unity_ProbeVolumeWorldToObject;
        float3 unity_ProbeVolumeSizeInv;
        float3 unity_ProbeVolumeMin;
    CBUFFER_END
#endif

static float4x4 unity_MatrixMVP = mul(unity_MatrixVP, unity_ObjectToWorld);
static float4x4 unity_MatrixMV = mul(unity_MatrixV, unity_ObjectToWorld);
static float4x4 unity_MatrixTMV = transpose(unity_MatrixMV);
static float4x4 unity_MatrixITMV = transpose(mul(unity_WorldToObject, unity_MatrixInvV));
// make them macros so that they can be redefined in UnityInstancing.cginc
#define UNITY_MATRIX_MVP    unity_MatrixMVP
#define UNITY_MATRIX_MV     unity_MatrixMV
#define UNITY_MATRIX_T_MV   unity_MatrixTMV
#define UNITY_MATRIX_IT_MV  unity_MatrixITMV

// ----------------------------------------------------------------------------
//  Deprecated

// There used to be fixed function-like texture matrices, defined as UNITY_MATRIX_TEXTUREn. These are gone now; and are just defined to identity.
#define UNITY_MATRIX_TEXTURE0 float4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)
#define UNITY_MATRIX_TEXTURE1 float4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)
#define UNITY_MATRIX_TEXTURE2 float4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)
#define UNITY_MATRIX_TEXTURE3 float4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)

#endif
