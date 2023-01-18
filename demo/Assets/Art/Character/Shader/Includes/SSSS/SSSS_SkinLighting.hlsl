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

half3 SkinSSSS_DirectDiffuse(float3 N, Light light, float3 positionWS)
{
    float3 L = light.direction;
    float atten = light.shadowAttenuation * light.distanceAttenuation;

    half NoL = saturate(dot(N, L));
    float3 radiance = NoL * atten * light.color;
    float3 addDiffuse = GetAdditionDiffuseLights(N, positionWS);
    float3 diffuse = radiance + addDiffuse;
    return diffuse;
}

half3 SkinSSSS_DirectSpecular(float3 N, float3 V, Light light, float3 RoughnessFactor, float3 SpecularColor)
{
    float Lobe0Roughness = RoughnessFactor.x;
    float Lobe1Roughness = RoughnessFactor.y;
    float LobeMix = RoughnessFactor.z;

    float3 L = light.direction;
    float3 H = normalize(L + V);
    float NoH = saturate(dot(N, H));
    float NoV = saturate(abs(dot(N, V)) + 1e-5);
    float NoL = saturate(dot(N, L));
    float VoH = saturate(dot(V, H));

    float3 SpecularBRDF = DualSpecularGGX(Lobe0Roughness, Lobe1Roughness, LobeMix, SpecularColor, NoH, NoV, NoL,
                                          VoH);
    SpecularBRDF *= light.shadowAttenuation;
    return SpecularBRDF;
}

half3 IndirectSpecualr(float3 N, float3 V, float3 RoughnessFactor, half3 SpecularColor)
{
    float Lobe0Roughness = RoughnessFactor.x;
    float Lobe1Roughness = RoughnessFactor.y;
    float LobeMix = RoughnessFactor.z;

    float NoV = saturate(abs(dot(N, V)) + 1e-5);
    half3 R = reflect(-V, N);

    half3 SpecularLobe0 = SpecularIBL(R, 1, Lobe0Roughness, SpecularColor, NoV);
    half3 SpecularLobe1 = SpecularIBL(R, 1, Lobe1Roughness, SpecularColor, NoV);
    half3 DualLobe = lerp(SpecularLobe0, SpecularLobe1, 1.0 - LobeMix);

    return DualLobe;
}


#endif
