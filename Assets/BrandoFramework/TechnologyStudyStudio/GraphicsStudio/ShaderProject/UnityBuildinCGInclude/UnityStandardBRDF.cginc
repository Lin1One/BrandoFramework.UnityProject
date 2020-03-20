#ifndef UNITY_STANDARD_BRDF_INCLUDED
#define UNITY_STANDARD_BRDF_INCLUDED

#include "UnityCG.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityLightingCommon.cginc"

//-----------------------------------------------------------------------------
// Helper to convert smoothness to roughness
//-----------------------------------------------------------------------------

//用于将感性粗糙度计算为学术意义上的粗糙度
float PerceptualRoughnessToRoughness(float perceptualRoughness)
{
    return perceptualRoughness * perceptualRoughness;
}

half RoughnessToPerceptualRoughness(half roughness)
{
    return sqrt(roughness);
}

// Smoothness is the user facing name
// it should be perceptualSmoothness but we don't want the user to have to deal with this name
half SmoothnessToRoughness(half smoothness)
{
    return (1 - smoothness) * (1 - smoothness);
}

//由光滑度获得粗糙度
//用于计算感性粗糙度，smoothness即材质的光滑度贴图/参数。
float SmoothnessToPerceptualRoughness(float smoothness)
{
    return (1 - smoothness);
}

//-------------------------------------------------------------------------------------

inline half Pow4 (half x)
{
    return x*x*x*x;
}

inline float2 Pow4 (float2 x)
{
    return x*x*x*x;
}

inline half3 Pow4 (half3 x)
{
    return x*x*x*x;
}

inline half4 Pow4 (half4 x)
{
    return x*x*x*x;
}

// Pow5 uses the same amount of instructions as generic pow(), but has 2 advantages:
// 1) better instruction pipelining
// 2) no need to worry about NaNs
inline half Pow5 (half x)
{
    return x*x * x*x * x;
}

inline half2 Pow5 (half2 x)
{
    return x*x * x*x * x;
}

inline half3 Pow5 (half3 x)
{
    return x*x * x*x * x;
}

inline half4 Pow5 (half4 x)
{
    return x*x * x*x * x;
}

// ------------------------------------ 菲涅尔反射F -------------------------------------
//Fresnel Schlicks近似式
// 菲涅尔反射F
// Schlick菲涅尔反射的公式为：
// F=F0+(1-F0)*(1-（H*V))^5
// F0：光线垂直入射时的表面反射率
// H：半角向量
// V：视线
// 此处输入的参数cosA，为saturate(dot(H,V))的值，也可能是abs(dot(H,V))。
inline half3 FresnelTerm (half3 F0, half cosA)
{
    half t = Pow5 (1 - cosA);   // ala Schlick interpoliation
    return F0 + (1-F0) * t;
}

//F0到F90之间的线性插值
inline half3 FresnelLerp (half3 F0, half3 F90, half cosA)
{
    half t = Pow5 (1 - cosA);   // ala Schlick interpoliation
    return lerp (F0, F90, t);
}
// approximage Schlick with ^4 instead of ^5
inline half3 FresnelLerpFast (half3 F0, half3 F90, half cosA)
{
    half t = Pow4 (1 - cosA);
    return lerp (F0, F90, t);
}

// Note: Disney diffuse must be multiply by diffuseAlbedo / PI. This is done outside of this function.
//注意：迪斯尼漫反射必须乘以diffuseAlbedo / PI。 这是在此函数之外完成的。
half DisneyDiffuse(half NdotV, half NdotL, half LdotH, half perceptualRoughness)
{
    half fd90 = 0.5 + 2 * LdotH * LdotH * perceptualRoughness;
    // Two schlick fresnel term
    //两个schlick菲涅耳项
    half lightScatter   = (1 + (fd90 - 1) * Pow5(1 - NdotL));
    half viewScatter    = (1 + (fd90 - 1) * Pow5(1 - NdotV));
    return lightScatter * viewScatter;
}


// ------------------------------------ 遮挡可见性项V -------------------------------------

// NOTE: Visibility term here is the full form from Torrance-Sparrow model, it includes Geometric term: V = G / (N.L * N.V)
// This way it is easier to swap Geometric terms and more room for optimizations (except maybe in case of CookTorrance geom term)

// Generic Smith-Schlick visibility term
inline half SmithVisibilityTerm (half NdotL, half NdotV, half k)
{
    half gL = NdotL * (1-k) + k;
    half gV = NdotV * (1-k) + k;
    return 1.0 / (gL * gV + 1e-5f); // This function is not intended to be running on Mobile,
                                    // therefore epsilon is smaller than can be represented by half
}

// Smith-Schlick derived for Beckmann
//Schlick-Beckman GSF
// SmithBeckmann 公式
//SmithBeckmannVisibilityTerm函数计算了k的值，并调用SmithVisibilityTerm函数进一步计算
//SmithBeckmann公式的分子在SmithVisibilityTerm中的 gL&gV 计算中与
// V=G/(4(n⋅l)(n⋅v))中的(n⋅l)(n⋅v)消项。
inline half SmithBeckmannVisibilityTerm (half NdotL, half NdotV, half roughness)
{
    half c = 0.797884560802865h; // c = sqrt(2 / Pi)
    half k = roughness * c;
    // *0.25是先抵消V=G/(4(n⋅l)(n⋅v))中的4。
    return SmithVisibilityTerm (NdotL, NdotV, k) * 0.25f; // * 0.25 is the 1/4 of the visibility term
}

// Ref: http://jcgt.org/published/0003/02/03/paper.pdf
// 遮挡可见性项V,在Unity中V项即是公式中的G项,为了简化运算，使得V=G/(4(n⋅l)(n⋅v))
// Torrance-Sparrow 微表面模型的公式：f(l,v)= D(h) F(v,h) G(l,v,h)/(4(n⋅l)(n⋅v))
//Smith-Joint GGX 公式
// Smith-Joint的近似公式（UE4使用了同样的公式）
inline float SmithJointGGXVisibilityTerm (float NdotL, float NdotV, float roughness)
{
#if 0
    // Original formulation:
    //  lambda_v    = (-1 + sqrt(a2 * (1 - NdotL2) / NdotL2 + 1)) * 0.5f;
    //  lambda_l    = (-1 + sqrt(a2 * (1 - NdotV2) / NdotV2 + 1)) * 0.5f;
    //  G           = 1 / (1 + lambda_v + lambda_l);

    // Reorder code to be more optimal
    half a          = roughness;
    half a2         = a * a;

    half lambdaV    = NdotL * sqrt((-NdotV * a2 + NdotV) * NdotV + a2);
    half lambdaL    = NdotV * sqrt((-NdotL * a2 + NdotL) * NdotL + a2);

    // Simplify visibility term: (2.0f * NdotL * NdotV) /  ((4.0f * NdotL * NdotV) * (lambda_v + lambda_l + 1e-5f));
    return 0.5f / (lambdaV + lambdaL + 1e-5f);  // This function is not intended to be running on Mobile,
                                                // therefore epsilon is smaller than can be represented by half
#else
    // Approximation of the above formulation (simplify the sqrt, not mathematically correct but close enough)
    float a = roughness;
    float lambdaV = NdotL * (NdotV * (1 - a) + a);
    float lambdaL = NdotV * (NdotL * (1 - a) + a);

#if defined(SHADER_API_SWITCH)
    return 0.5f / (lambdaV + lambdaL + 1e-4f); // work-around against hlslcc rounding error
#else
    return 0.5f / (lambdaV + lambdaL + 1e-5f);
#endif

#endif
}

// ------------------------------------ 微表面分布项D -------------------------------------
inline float GGXTerm (float NdotH, float roughness)
{
    //此功能不适合在移动设备上运行，因此 ε 小于 0.5
    float a2 = roughness * roughness;
    float d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
    //UNITY_INV_PI =1 / UNITY_PI
    return UNITY_INV_PI * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
                                            // therefore epsilon is smaller than what can be represented by half
}

inline half PerceptualRoughnessToSpecPower (half perceptualRoughness)
{
    half m = PerceptualRoughnessToRoughness(perceptualRoughness);   // m is the true academic roughness.
    half sq = max(1e-4f, m*m);
    half n = (2.0 / sq) - 2.0;                          // https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
    n = max(n, 1e-4f);                                  // prevent possible cases of pow(0,0), which could happen when roughness is 1.0 and NdotH is zero
    return n;
}

// BlinnPhong normalized as normal distribution function (NDF)
// for use in micro-facet model: spec=D*G*F
// eq. 19 in https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
// BlinnPhong的D项实现
inline half NDFBlinnPhongNormalizedTerm (half NdotH, half n)
{
    // norm = (n+2)/(2*pi)
    half normTerm = (n + 2.0) * (0.5/UNITY_PI);

    half specTerm = pow (NdotH, n);
    return specTerm * normTerm;
}

//-------------------------------------------------------------------------------------
/*
// https://s3.amazonaws.com/docs.knaldtech.com/knald/1.0.0/lys_power_drops.html

const float k0 = 0.00098, k1 = 0.9921;
// pass this as a constant for optimization
const float fUserMaxSPow = 100000; // sqrt(12M)
const float g_fMaxT = ( exp2(-10.0/fUserMaxSPow) - k0)/k1;
float GetSpecPowToMip(float fSpecPow, int nMips)
{
   // Default curve - Inverse of TB2 curve with adjusted constants
   float fSmulMaxT = ( exp2(-10.0/sqrt( fSpecPow )) - k0)/k1;
   return float(nMips-1)*(1.0 - clamp( fSmulMaxT/g_fMaxT, 0.0, 1.0 ));
}

    //float specPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
    //float mip = GetSpecPowToMip (specPower, 7);
*/

inline float3 Unity_SafeNormalize(float3 inVec)
{
    float dp3 = max(0.001f, dot(inVec, inVec));
    return inVec * rsqrt(dp3);
}

//--------------------------------------BRDF 函数-----------------------------------------------

// Note: BRDF entry points use smoothness and oneMinusReflectivity for optimization
// purposes, mostly for DX9 SM2.0 level. Most of the math is being done on these (1-x) values, and that saves
// a few precious ALU slots.
// 注意：BRDF入口点使用平滑度和oneMinusReflectivity进行优化，主要用于DX9 SM2.0级别。 
// 大多数数学运算都是在这些（1-x）值上完成的，这样可以节省一些宝贵的ALU插槽。


// Main Physically Based BRDF
// Derived from Disney work and based on Torrance-Sparrow micro-facet model
// 源自迪士尼PBR 工作流，基于Torrance-Sparrow微面模型

// Torrance-Sparrow 微表面模型的公式：
// f(l,v)=D(h)F(v,h)G(l,v,h)/(4(n⋅l)(n⋅v))
// BRDF公式：
// BRDF = kD / pi + kS * (D * V * F) / 4

// D——微表面分布项
// V——遮挡可见性项
// F——菲涅尔反射项
// kD——漫反射系数
// kS——镜面反射系数
// Note：V(Visibility)项即G(l,v,h)/(4(n⋅l)(n⋅v))的集合。

//   BRDF = kD / pi + kS * (D * V * F) / 4
//   I = BRDF * NdotL
//
// * NDF (depending on UNITY_BRDF_GGX):
//  a) Normalized BlinnPhong
//  b) GGX
// * Smith for Visiblity term
// * Schlick approximation for Fresnel
half4 BRDF1_Unity_PBS (
    half3 diffColor,    //漫反射颜色的值
    half3 specColor,    //镜面反射颜色值
    half oneMinusReflectivity, //1减去反射率的值
    half smoothness,    //光滑度
    float3 normal,      //法线的方向
    float3 viewDir,     //视线的方向
    UnityLight light,   //Unity中光源参数
    UnityIndirect gi)   //漫反射颜色diffuse和镜面反射颜色specular的光线反射结构体，表示间接光照信息
{
    //1.计算感性粗糙度
    float perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    //2.计算半角向量。
    float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

// NdotV should not be negative for visible pixels, but it can happen due to perspective projection and normal mapping
// In this case normal should be modified to become valid (i.e facing camera) and not cause weird artifacts.
// but this operation adds few ALU and users may not want it. Alternative is to simply take the abs of NdotV (less correct but works too).
// Following define allow to control this. Set it to 0 if ALU is critical on your platform.
// This correction is interesting for GGX with SmithJoint visibility function because artifacts are more visible in this case due to highlight edge of rough surface
// Edit: Disable this code by default for now as it is not compatible with two sided lighting used in SpeedTree.
// NdotV对于可见像素不应为负，但由于透视投影和法线贴图而可能发生
// 在这种情况下，应修改法线以使其有效（即朝向相机），并且不会引起怪异的伪影。
// 但此操作添加少量的ALU（逻辑运算单元），用户可能不想要它。 
// 另一种方法是简单地使用NdotV的绝对值（不太正确，但也可以）。
// 按照define来控制。 如果ALU在您的平台上很重要，请将其设置为0。
// 这种校正对于使用Smith-Joint GGX能见度函数是很有用的，因为会导致粗糙表面的高光边缘异常会更明显。
// 编辑：默认情况下，由于与SpeedTree中使用的双面照明不兼容，因此默认情况下暂时禁用此代码。
#define UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV 0

#if UNITY_HANDLE_CORRECTLY_NEGATIVE_NDOTV
    // The amount we shift the normal toward the view vector is defined by the dot product.
    half shiftAmount = dot(normal, viewDir);
    normal = shiftAmount < 0.0f ? normal + viewDir * (-shiftAmount + 1e-5f) : normal;
    // A re-normalization should be applied here but as the shift is small we don't do it to save ALU.
    //这里应该应用重新规范化，但是由于偏移很小，因此我们不这样做以节省ALU。
    //normal = normalize(normal);

    float nv = saturate(dot(normal, viewDir)); // TODO: this saturate should no be necessary here
#else
    half nv = abs(dot(normal, viewDir));    // This abs allow to limit artifact
#endif

    //1.计算NdotL/NdotH/LdotV/LdotH用于后续计算
    float nl = saturate(dot(normal, light.dir));
    float nh = saturate(dot(normal, halfDir));
    half lv = saturate(dot(light.dir, viewDir));
    half lh = saturate(dot(light.dir, halfDir));

    // Diffuse term
    // 2.漫反射项
    // Unity中的diffuseTerm 计算，并没有除π，反而乘了NdotL
    // BRDF = kD / pi + kS * (D * V * F) / 4
    half diffuseTerm = DisneyDiffuse(nv, nl, lh, perceptualRoughness) * nl;


    //Part3 1.计算高光项的一部分，菲涅尔项最后再添加。如果镜面反射高光关闭，那么镜面反射项为0。
    // 镜面反射项
    // Specular term
    // HACK: theoretically we should divide diffuseTerm by Pi and not multiply specularTerm!
    // BUT 1) that will make shader look significantly darker than Legacy ones
    // and 2) on engine side "Non-important" lights have to be divided by Pi too in cases when they are injected into ambient SH
    
     // HACK：理论上，我们应该将diffuseTerm除以Pi，而不要乘以specularTerm！，但是
     //1）将使着色器看起来比旧版着色器暗得多
     //2）在引擎方面，“非重要”灯在注入周围环境SH的情况下也必须由Pi划分
     // roughness 平方

     //3.计算粗糙度，使用PerceptualRoughnessToRoughness函数将感性粗糙度转换到学术意义上的粗糙度
    float roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
#if UNITY_BRDF_GGX
    // GGX with roughtness to 0 would mean no specular at all, using max(roughness, 0.002) here to match HDrenderloop roughtness remapping.
    // 使用 GGX 算法时，粗糙度为0 根本就没有镜面反射
    // 使用max（roughness，0.002）匹配HDrenderloop粗糙度重新映射。
    roughness = max(roughness, 0.002);
    float V = SmithJointGGXVisibilityTerm (nl, nv, roughness);
    float D = GGXTerm (nh, roughness);
#else
    // Legacy
    // V项和D项使用Smith-Beckmann和Blinn-Phong公式实现。
    half V = SmithBeckmannVisibilityTerm (nl, nv, roughness);
    half D = NDFBlinnPhongNormalizedTerm (nh, PerceptualRoughnessToSpecPower(perceptualRoughness));
#endif

    // Torrance-Sparrow模型，菲涅耳稍后再应用
    // Torrance-Sparrow model, Fresnel is applied later
    float specularTerm = V*D * UNITY_PI; 

//2.如果开启了颜色空间GAMMA校正，那么这里会进行一次计算。
#ifdef UNITY_COLORSPACE_GAMMA
        specularTerm = sqrt(max(1e-4h, specularTerm));
#endif

    // specularTerm * nl can be NaN on Metal in some cases, use max() to make sure it's a sane value
    specularTerm = max(0, specularTerm * nl);
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

//3.计算surfaceReduction参数。Unity在注释中给出了它的公式,
//  但并没有查到计算他的目的，在这里它用于间接光照的计算。
    // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(roughness^2+1)
    half surfaceReduction;
#ifdef UNITY_COLORSPACE_GAMMA
        surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;      // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
#else
        surfaceReduction = 1.0 / (roughness*roughness + 1.0);           // fade \in [0.5;1]
#endif

    //4.为了提供真正的Lambert照明，如果SpecColor的各个通道值均为0，那么就是全漫反射。
    // To provide true Lambert lighting, we need to be able to kill specular completely.
    specularTerm *= any(specColor) ? 1.0 : 0.0;

    half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));

    //5.最后的color输出，分为三个部分：漫反射+镜面反射+表面衰减。
    // 漫反射：输入的漫反射颜色（纹理）*GI的漫反射颜色（间接光照）+输入的漫反射颜色（纹理）*光照颜色（直接光照）*漫反射项
    // 镜面反射：镜面反射项（V项和D项）*光照颜色（直接光照）*菲涅尔项（F项）
    // 表面衰减：表面衰减系数*GI镜面反射（间接光照）*菲涅尔插值
    half3 color =   diffColor * (gi.diffuse + light.color * diffuseTerm) + 
                    specularTerm * light.color * FresnelTerm (specColor, lh) + 
                    surfaceReduction * gi.specular * FresnelLerp (specColor, grazingTerm, nv);

    return half4(color, 1);
}

// Based on Minimalist CookTorrance BRDF
// Implementation is slightly different from original derivation: http://www.thetenthplanet.de/archives/255
// 基于简约的CookTorrance BRDF
// 实现与原始推导略有不同：
// * NDF (depending on UNITY_BRDF_GGX):
//  a) BlinnPhong
//  b) [Modified] GGX
// * Modified Kelemen and Szirmay-​Kalos for Visibility term
// * Fresnel approximated with 1/LdotH
half4 BRDF2_Unity_PBS (
    half3 diffColor, 
    half3 specColor, 
    half oneMinusReflectivity, 
    half smoothness,
    float3 normal, 
    float3 viewDir,
    UnityLight light, 
    UnityIndirect gi)
{
    float3 halfDir = Unity_SafeNormalize (float3(light.dir) + viewDir);

    half nl = saturate(dot(normal, light.dir));
    float nh = saturate(dot(normal, halfDir));
    half nv = saturate(dot(normal, viewDir));
    float lh = saturate(dot(light.dir, halfDir));

    // Specular term
    half perceptualRoughness = SmoothnessToPerceptualRoughness (smoothness);
    half roughness = PerceptualRoughnessToRoughness(perceptualRoughness);

#if UNITY_BRDF_GGX
    // GGX Distribution multiplied by combined approximation of Visibility and Fresnel
    // See "Optimizing PBR for Mobile" from Siggraph 2015 moving mobile graphics course
    // https://community.arm.com/events/1155
    half a = roughness;
    float a2 = a*a;

    float d = nh * nh * (a2 - 1.f) + 1.00001f;
#ifdef UNITY_COLORSPACE_GAMMA
    // Tighter approximation for Gamma only rendering mode!
    // DVF = sqrt(DVF);
    // DVF = (a * sqrt(.25)) / (max(sqrt(0.1), lh)*sqrt(roughness + .5) * d);
    float specularTerm = a / (max(0.32f, lh) * (1.5f + roughness) * d);
#else
    float specularTerm = a2 / (max(0.1f, lh*lh) * (roughness + 0.5f) * (d * d) * 4);
#endif

    // on mobiles (where half actually means something) denominator have risk of overflow
    // clamp below was added specifically to "fix" that, but dx compiler (we convert bytecode to metal/gles)
    // sees that specularTerm have only non-negative terms, so it skips max(0,..) in clamp (leaving only min(100,...))
#if defined (SHADER_API_MOBILE)
    specularTerm = specularTerm - 1e-4f;
#endif

#else

    // Legacy
    half specularPower = PerceptualRoughnessToSpecPower(perceptualRoughness);
    // Modified with approximate Visibility function that takes roughness into account
    // Original ((n+1)*N.H^n) / (8*Pi * L.H^3) didn't take into account roughness
    // and produced extremely bright specular at grazing angles

    half invV = lh * lh * smoothness + perceptualRoughness * perceptualRoughness; // approx ModifiedKelemenVisibilityTerm(lh, perceptualRoughness);
    half invF = lh;

    half specularTerm = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);

#ifdef UNITY_COLORSPACE_GAMMA
    specularTerm = sqrt(max(1e-4f, specularTerm));
#endif

#endif

#if defined (SHADER_API_MOBILE)
    specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
#endif
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specularTerm = 0.0;
#endif

    // surfaceReduction = Int D(NdotH) * NdotH * Id(NdotL>0) dH = 1/(realRoughness^2+1)

    // 1-0.28*x^3 as approximation for (1/(x^4+1))^(1/2.2) on the domain [0;1]
    // 1-x^3*(0.6-0.08*x)   approximation for 1/(x^4+1)
#ifdef UNITY_COLORSPACE_GAMMA
    half surfaceReduction = 0.28;
#else
    half surfaceReduction = (0.6-0.08*perceptualRoughness);
#endif

    surfaceReduction = 1.0 - roughness*perceptualRoughness*surfaceReduction;

    half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));
    half3 color =   (diffColor + specularTerm * specColor) * light.color * nl
                    + gi.diffuse * diffColor
                    + surfaceReduction * gi.specular * FresnelLerpFast (specColor, grazingTerm, nv);

    return half4(color, 1);
}

sampler2D_float unity_NHxRoughness;
half3 BRDF3_Direct(half3 diffColor, half3 specColor, half rlPow4, half smoothness)
{
    half LUT_RANGE = 16.0; // must match range in NHxRoughness() function in GeneratedTextures.cpp
    // Lookup texture to save instructions
    half specular = tex2D(unity_NHxRoughness, half2(rlPow4, SmoothnessToPerceptualRoughness(smoothness))).r * LUT_RANGE;
#if defined(_SPECULARHIGHLIGHTS_OFF)
    specular = 0.0;
#endif

    return diffColor + specular * specColor;
}

half3 BRDF3_Indirect(half3 diffColor, half3 specColor, UnityIndirect indirect, half grazingTerm, half fresnelTerm)
{
    half3 c = indirect.diffuse * diffColor;
    c += indirect.specular * lerp (specColor, grazingTerm, fresnelTerm);
    return c;
}

// Old school, not microfacet based Modified Normalized Blinn-Phong BRDF
// Implementation uses Lookup texture for performance
//
// * Normalized BlinnPhong in RDF form
// * Implicit Visibility term
// * No Fresnel term
//
// TODO: specular is too weak in Linear rendering mode
half4 BRDF3_Unity_PBS (
    half3 diffColor, 
    half3 specColor, 
    half oneMinusReflectivity, 
    half smoothness,
    float3 normal, 
    float3 viewDir,
    UnityLight light, 
    UnityIndirect gi)
{
    float3 reflDir = reflect (viewDir, normal);

    half nl = saturate(dot(normal, light.dir));
    half nv = saturate(dot(normal, viewDir));

    // Vectorize Pow4 to save instructions
    half2 rlPow4AndFresnelTerm = Pow4 (float2(dot(reflDir, light.dir), 1-nv));  // use R.L instead of N.H to save couple of instructions
    half rlPow4 = rlPow4AndFresnelTerm.x; // power exponent must match kHorizontalWarpExp in NHxRoughness() function in GeneratedTextures.cpp
    half fresnelTerm = rlPow4AndFresnelTerm.y;

    half grazingTerm = saturate(smoothness + (1-oneMinusReflectivity));

    half3 color = BRDF3_Direct(diffColor, specColor, rlPow4, smoothness);
    color *= light.color * nl;
    color += BRDF3_Indirect(diffColor, specColor, gi, grazingTerm, fresnelTerm);

    return half4(color, 1);
}

// Include deprecated function
#define INCLUDE_UNITY_STANDARD_BRDF_DEPRECATED
#include "UnityDeprecated.cginc"
#undef INCLUDE_UNITY_STANDARD_BRDF_DEPRECATED

#endif // UNITY_STANDARD_BRDF_INCLUDED
