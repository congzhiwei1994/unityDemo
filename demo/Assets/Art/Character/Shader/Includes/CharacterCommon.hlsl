#ifndef CHARACTER_COMMON_INCLUDE
#define CHARACTER_COMMON_INCLUDE

#define PI          3.14159265358979323846

struct SSSSInput
{
    float3 DiffuseColor;
    float3 SpecularColor;
    float Lobe0Roughness;
    float Lobe1Roughness;
    float LobeMix;
    float Occlusion;
    float EnvRotation;
    float3 WorldPos;
    float3 worldNormal;
    float3 worldNormal_Blur;
    float3 ViewDirWS;
    Texture2D SSSLUT;
    SamplerState sampler_SSSLUT;
    float Curvature;
    float ClearCoat;
    float ClearCoatRoughness;
    float3 ClearCoatNormal;
};


// float Pow4(float x)
// {
//     return (x * x) * (x * x);
// }

inline half Pow5(half x)
{
    return x * x * x * x * x;
}

// Appoximation of joint Smith term for GGX
/// [Heitz 2014, "Understanding the Masking-Shadowing Function in Microfacet-Based BRDFs"]
float Vis_SmithJointApprox(float a2, float NoV, float NoL)
{
    float a = sqrt(a2);
    float Vis_SmithV = NoL * (NoV * (1 - a) + a);
    float Vis_SmithL = NoV * (NoL * (1 - a) + a);
    return 0.5 * rcp(Vis_SmithV + Vis_SmithL);
}


// GGX / Trowbridge-Reitz
// [Walter et al. 2007, "Microfacet models for refraction through rough surfaces"]
float D_GGX_UE4(float a2, float NoH)
{
    float d = (NoH * a2 - NoH) * NoH + 1; // 2 mad
    return a2 / (PI * d * d); // 4 mul, 1 rcp
}

// [Schlick 1994, "An Inexpensive BRDF Model for Physically-Based Rendering"]
float3 F_Schlick_UE4(float3 SpecularColor, float VoH)
{
    float Fc = Pow5(1 - VoH); // 1 sub, 3 mul
    //return Fc + (1 - Fc) * SpecularColor;		// 1 add, 3 mad

    // Anything less than 2% is physically impossible and is instead considered to be shadowing
    return saturate(50.0 * SpecularColor.g) * Fc + (1 - Fc) * SpecularColor;
}


float3 DualSpecularGGX(float Lobe0Roughness, float Lobe1Roughness, float LobeMix, float3 SpecularColor, float NoH,
                       float NoV, float NoL, float VoH)
{
    float Lobe0Alpha2 = Pow4(Lobe0Roughness);
    float Lobe1Alpha2 = Pow4(Lobe1Roughness);
    float AverageAlpha2 = Pow4((Lobe0Roughness + Lobe1Roughness) * 0.5);

    // Generalized microfacet specular
    float D = lerp(D_GGX_UE4(Lobe0Alpha2, NoH), D_GGX_UE4(Lobe1Alpha2, NoH), 1.0 - LobeMix);
    float Vis = Vis_SmithJointApprox(AverageAlpha2, NoV, NoL);
    float3 F = F_Schlick_UE4(SpecularColor, VoH);

    return (D * Vis) * F;
}


#endif
