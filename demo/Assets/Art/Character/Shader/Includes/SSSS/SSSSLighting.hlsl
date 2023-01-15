#ifndef SKIN_SSSS_LIGHTING_INCLUDE
#define SKIN_SSSS_LIGHTING_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include"Assets/Art/Character/Shader/Includes/CharacterCommon.hlsl"

float3 GetAdditionDiffuseLights(float3 N, float3 positionWS)
{
    float3 color = 0;
    int pixelLightCount = GetAdditionalLightsCount();
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, positionWS);

        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            float attenuation = light.distanceAttenuation * light.shadowAttenuation;
            float3 attenuatedLightColor = light.color * attenuation;
            float3 Diffuse = saturate(dot(N, light.direction)) * attenuatedLightColor;
            color += Diffuse;
        }
    }
    return color;
}

half3 Skin_SSSS_Diffuse(float3 N, Light light, float3 positionWS)
{
    float3 L = light.direction;
    float atten = light.shadowAttenuation * light.distanceAttenuation;

    half NoL = saturate(dot(N, L));
    float3 radiance = NoL * atten * light.color;

    //==================多光源   ============================================== //
    float3 addDiffuse = GetAdditionDiffuseLights(N, positionWS);
    float3 diffuse = radiance + addDiffuse;

    half3 c = 0;
    #ifdef DISABLE_SKIN_SH
     c = diffuse;
    #else
    c += SampleSH(N);
    #endif
    return c;
}

half3 Skin_SSSS_Specular(float3 N, Light light, float3 positionWS, float roughness, float4 albedo,
                         float3 RoughnessFactor, float3 SpecularColor)
{
    float Lobe0Roughness = RoughnessFactor.x;
    float Lobe1Roughness = RoughnessFactor.y;
    float LobeMix = RoughnessFactor.z;

    float3 H = normalize(L + V);
    float NoH = saturate(dot(N, H));
    float NoV = saturate(abs(dot(N, V)) + 1e-5);
    float NoL = dot(N, L);
    float VoH = saturate(dot(V, H));
    
   float3 SpecularBRDF = DualSpecularGGX(Lobe0Roughness, Lobe1Roughness, LobeMix, SpecularColor, NoH, NoV, NoL_Spec,
                                   VoH);

    //采样SSSBlurRT
    float2 screenUV = input.screenPos.xy / input.screenPos.w;
    float4 SkinSSSMap = SAMPLE_TEXTURE2D(_SSSBlurRT, sampler_SSSBlurRT, screenUV);

    // float3 SkinSSS = BaseMap * pow(SkinSSSMap,0.45) + Specular.xyzz;
    float3 SkinSSS = _BaseColor.rgb * BaseMap * SkinSSSMap + Specular.xyzz;
    return SkinSSS.xyzz;
}


#endif
