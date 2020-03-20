#ifndef UNITY_SHADER_UTILITIES_INCLUDED
#define UNITY_SHADER_UTILITIES_INCLUDED

#include "UnityShaderVariables.cginc"

inline float4 UnityObjectToClipPos(in float3 pos)
{
    // More efficient than computing M*VP matrix product
    //比计算M * VP矩阵乘积效率更高
    return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(pos, 1.0)));
}
// UnityObjectToClipPos 的 float4的重载； 避免为现有着色器“数值隐式截断”警告
// overload for float4; avoids "implicit truncation" warning for existing shaders
inline float4 UnityObjectToClipPos(float4 pos) 
{
    return UnityObjectToClipPos(pos.xyz);
}

#endif
