#ifndef UNITY_LIGHTING_COMMON_INCLUDED
#define UNITY_LIGHTING_COMMON_INCLUDED

fixed4 _LightColor0;
fixed4 _SpecColor;

struct UnityLight
{
    half3 color;//光源颜色
    half3 dir; //光源方向
    //不推荐使用：Ndotl现在可以即时计算，不再存储。
    half  ndotl; // Deprecated: Ndotl is now calculated on the fly and is no longer stored. Do not used it.
};

struct UnityIndirect
{
    half3 diffuse;//漫反射颜色
    half3 specular;//镜面反射颜色
};

//全局光照结构体
struct UnityGI
{
    UnityLight light;//表示第一个光源
    UnityIndirect indirect;//Unity间接光源参数的结构体
};

struct UnityGIInput
{
    // 像素光源，由引擎准备并传输过来
    UnityLight light; // pixel light, sent from the engine
    //光源在世界空间中的位置坐标
    float3 worldPos;
    //光源世界空间中的视角方向向量坐标
    half3 worldViewDir;
    //衰减值
    half atten;
    //环境光颜色
    half3 ambient;


    //插值的光照贴图UV作为完整浮点精度数据传递到片段着色器，
    //因此lightmapUV（在光照贴图片段着色器内部用作tmp）也应该是完全浮点精度，
    //以避免在采样纹理之前丢失数据。
    // interpolated lightmap UVs are passed as full float precision data to fragment shaders
    // so lightmapUV (which is used as a tmp inside of lightmap fragment shaders) should
    // also be full float precision to avoid data loss before sampling a texture.
    
    //xy = static lightmapUV（静态光照贴图的UV）
    //zw = dynamic lightmapUV（动态光照贴图的UV）
    float4 lightmapUV; // .xy = static lightmap UV, .zw = dynamic lightmap UV

#if defined(UNITY_SPECCUBE_BLENDING) || 
    defined(UNITY_SPECCUBE_BOX_PROJECTION) || 
    defined(UNITY_ENABLE_REFLECTION_BUFFERS)
    float4 boxMin[2]; //box最小值
#endif

#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    float4 boxMax[2];//box最大值
    float4 probePosition[2]; //光照探针的位置
#endif
    // HDR cubemap properties, use to decompress HDR texture
    //光照探针的高动态范围图像（High-Dynamic Range）
    float4 probeHDR[2];
};

#endif
