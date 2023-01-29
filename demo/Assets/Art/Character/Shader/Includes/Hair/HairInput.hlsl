#ifndef HAIR_INPUT_INCLUDE
#define HAIR_INPUT_INCLUDE

#define _NORMALMAP

CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4 _BaseColor;
half _HairClip;
half _Roughness;
half _Specular;
half _Scatter;
half _NoiseIntensity;
CBUFFER_END

// -------------------------------------------
TEXTURE2D(_SSSBlurRT);
SAMPLER(sampler_SSSBlurRT);

TEXTURE2D(_OpacityMap);
SAMPLER(sampler_OpacityMap);

TEXTURE2D(_FlowMap);
SAMPLER(sampler_FlowMap);

TEXTURE2D(_IDMap);
SAMPLER(sampler_IDMap);

#endif
