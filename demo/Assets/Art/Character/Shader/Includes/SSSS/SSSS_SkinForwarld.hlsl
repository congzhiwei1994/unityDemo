#ifndef SKIN_SSSS_SKIN_FORWARD_INCLUDE
#define SKIN_SSSS_SKIN_FORWARD_INCLUDE

#include "Assets/Art/Character/Shader/Includes/SSSS/SSSS_SkinLighting.hlsl"


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
    float4 screenPos : TEXCOORD7;
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
    output.screenPos = ComputeScreenPos(output.positionCS);
    return output;
}


half4 LitPassFragment_Diffuse(Varyings input) : SV_Target
{
    half occlusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, input.uv);

    half4 bumpMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
    half3 normalTS = UnpackNormalScale(bumpMap, _BumpScale);

    half2 detailBumpUV = input.uv * _DetailBumpMap_ST.xy + _DetailBumpMap_ST.zw;
    half4 detailbump = SAMPLE_TEXTURE2D(_DetailBumpMap, sampler_DetailBumpMap, detailBumpUV);
    half3 detailTS = UnpackNormalScale(detailbump, _DetailScale);

    half detailMask = SAMPLE_TEXTURE2D(_DetailMaskMap, sampler_DetailMaskMap, input.uv);
    normalTS = lerp(normalTS, detailTS, detailMask);

    float3 bitangent = input.tangentWS.w * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tbn = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
    float3 normalWS = NormalizeNormalPerPixel(TransformTangentToWorld(normalTS, tbn));

    occlusion = lerp(1, occlusion, _Occlusion);

    float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    Light mainLight = GetMainLight(shadowCoord);
    half3 diffuse = SkinSSSS_DirectDiffuse(normalWS, mainLight, input.positionWS);
    half3 bakeGI = SampleSH(normalWS);
    half3 color = diffuse + bakeGI * occlusion;
    return half4(color, 1);
}


half4 LitPassFragment_Specular(Varyings input) : SV_Target
{
    half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);;
    half3 specularMap = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, input.uv);
    half roughnessMap = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, input.uv);
    half occlusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, input.uv);
    occlusion = lerp(1, occlusion, _Occlusion);
    
    half4 bumpMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
    half3 normalTS = UnpackNormalScale(bumpMap, _BumpScale);

    half2 detailBumpUV = input.uv * _DetailBumpMap_ST.xy + _DetailBumpMap_ST.zw;
    half4 detailbump = SAMPLE_TEXTURE2D(_DetailBumpMap, sampler_DetailBumpMap, detailBumpUV);
    half3 detailTS = UnpackNormalScale(detailbump, _DetailScale);

    half detailMask = SAMPLE_TEXTURE2D(_DetailMaskMap, sampler_DetailMaskMap, input.uv);
    normalTS = lerp(normalTS, detailTS, detailMask);

    float3 bitangent = input.tangentWS.w * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tbn = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);
    float3 normalWS = NormalizeNormalPerPixel(TransformTangentToWorld(normalTS, tbn));

    float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
    Light mainLight = GetMainLight(shadowCoord);

    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    half roughness0 = roughnessMap * _Roughness0;
    half roughness1 = roughnessMap * _Roughness1;
    
    //采样SSSBlurRT
    float2 screenUV = input.positionCS.xy / _ScreenParams.xy;
    float4 SkinSSSMap = SAMPLE_TEXTURE2D(_SSSBlurRT, sampler_SSSBlurRT, screenUV);

    half metallic = 0;
    half3 roughnessFactor = half3(roughness0, roughness1, _RoughnessMix);
    half3 diffuse = lerp(albedo, half3(0, 0, 0), metallic);
    half3 specualrColor = specularMap * _Specular * 0.08;

    half3 specular = SkinSSSS_DirectSpecular(normalWS, viewDirWS, mainLight, roughnessFactor, specualrColor);

    half3 color = diffuse * SkinSSSMap + specular;


    return half4(color, 1);
}


#endif
