#ifndef HAIR_FORWARD_INCLUDE
#define HAIR_FORWARD_INCLUDE

#include "Assets/Art/Character/Shader/Includes/Hair//HairLighting.hlsl"


struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float4 tangentOS : TANGENT;
    float2 texcoord : TEXCOORD0;
    float2 lightmapUV : TEXCOORD1;
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

    float3 positionWS : TEXCOORD2;
    float3 normalWS : TEXCOORD3;
    float4 tangentWS : TEXCOORD4;
    float3 viewDirWS : TEXCOORD5;
    half4 fogFactorAndVertexLight : TEXCOORD6;

    float4 positionCS : SV_POSITION;
};


Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);

    output.uv = input.texcoord;
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = viewDirWS;
    output.tangentWS = tangentWS;

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(1, vertexLight);
    output.positionWS = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;
    return output;
}


half4 LitPassFragment(Varyings input) : SV_Target
{
    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    half alpha = SAMPLE_TEXTURE2D(_OpacityMap, sampler_OpacityMap, input.uv);
    float3 bitangent = input.tangentWS.w * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tbn = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

    half idMap = SAMPLE_TEXTURE2D(_IDMap, sampler_IDMap, input.uv).r;
    half3 noise = lerp(half3(0, 0, -1), half3(0, 0, 1), idMap) * _NoiseIntensity;
    noise += half3(0, 1, 0);
    float3 normalWS = TransformTangentToWorld(noise, tbn);

    half depth = SAMPLE_TEXTURE2D(_DepthMap, sampler_DepthMap, input.uv).r;
    
    HairData hair_data;
    hair_data.diffuse =  baseMap.rgb;
    hair_data.specular = half3(_Specular, _Specular, _Specular);
    hair_data.roughness = _Roughness;
    hair_data.scatter = _Scatter;
    hair_data.normalWS = normalize(normalWS);
    hair_data.viewWS = normalize(input.viewDirWS);
    hair_data.ShadowCoord = TransformWorldToShadowCoord(input.positionWS);
    hair_data.positionWS = input.positionWS;
    hair_data.bakedGI = SampleSH(hair_data.normalWS) * depth;

    half3 color = HairLighting(hair_data);
    clip(alpha - _HairClip);
    color = clamp(0,1.6,color);
    return half4(color, 1);
}


#endif
