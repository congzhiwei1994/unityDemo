#ifndef POST_SSAO_INCLUDE
#define POST_SSAO_INCLUDE

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

#include "SSAOData.hlsl"

CBUFFER_START(UnityPerMaterial)
half4 _BaseColor;
float4 _BaseMap_ST;
CBUFFER_END
TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);


half4 _SSAOParams;
half4 _CameraViewTopLeftCorner;
half4x4 _CameraViewProjections;

float4 _SourceSize;
float4 _ProjectionParams2;
float4 _CameraViewXExtent;
float4 _CameraViewYExtent;
float4 _CameraViewZExtent;


struct Attributes
{
    // float4 positionOS : POSITION;
    float4 positionHCS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};


Varyings vert(Attributes input)
{
    Varyings output;

    // Note: The pass is setup with a mesh already in CS
    // Therefore, we can just output vertex position
    output.positionCS = float4(input.positionHCS.xyz, 1.0);

    #if UNITY_UV_STARTS_AT_TOP
    output.positionCS.y *= _ScaleBiasRt.x;
    #endif
    output.uv = input.uv;
    // uv加了一个很小的epsilon, 避免重建法线的时候出现问题.
    output.uv += 1.0e-6;
    return output;
}

// Distance-based AO estimator based on Morgan 2011
// "Alchemy screen-space ambient obscurance algorithm"
// http://graphics.cs.williams.edu/papers/AlchemyHPG11/
half4 SSAO(Varyings input) : SV_Target
{
    float2 uv = input.uv;
    half3x3 camTransform = (half3x3)_CameraViewProjections;

    // Get the depth, normal and view position for this fragment
    float depth_o;
    half3 norm_o;
    half3 vpos_o;
    SampleDepthNormalView1(uv, depth_o, norm_o, vpos_o);

    return 1;
}


half4 frag(Varyings i) : SV_Target
{
    half4 c;
    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
    c = baseMap * _BaseColor;
    return c;
}


#endif
