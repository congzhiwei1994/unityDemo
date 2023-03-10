#ifndef SKIN_SSSS_INPUT_INCLUDE
#define SKIN_SSSS_INPUT_INCLUDE

#define _NORMALMAP


CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half _Smoothness;
half _BumpScale;
half _Occlusion;
half _Roughness0;
half _Roughness1;
half _RoughnessMix;
half _Specular;
half _DetailScale;
float4 _DetailBumpMap_ST;
CBUFFER_END

TEXTURE2D(_SSSBlurRT);
SAMPLER(sampler_SSSBlurRT);


TEXTURE2D(_RoughnessMap);
SAMPLER(sampler_RoughnessMap);

TEXTURE2D(_SpecularMap);
SAMPLER(sampler_SpecularMap);

TEXTURE2D(_DetailBumpMap);
SAMPLER(sampler_DetailBumpMap);

TEXTURE2D(_OcclusionMap);
SAMPLER(sampler_OcclusionMap);

TEXTURE2D(_DetailMaskMap);
SAMPLER(sampler_DetailMaskMap);

#endif
