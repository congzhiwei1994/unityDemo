﻿Shader "URP/UnlitMultiPass"
{
    Properties
    {
        _BaseColor("Base Color",color) = (1,1,1,1)
        _BaseMap("BaseMap", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        half4 _BaseColor;
        float4 _BaseMap_ST;
        CBUFFER_END
        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);
        // #define smp _linear_clampU_mirrorV
        // SAMPLER(smp);


        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float fogCoord : TEXCOORD1;
        };


        Varyings vert(Attributes v)
        {
            Varyings o = (Varyings)0;

            o.positionCS = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(v.positionOS.xyz, 1.0)));
            o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
            o.fogCoord = ComputeFogFactor(o.positionCS.z);

            return o;
        }

        half4 frag(Varyings i) : SV_Target
        {
            half4 c;
            half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
            c = baseMap * _BaseColor;
            c.rgb = MixFog(c.rgb, i.fogCoord);
            return c;
        }
        ENDHLSL

        Pass
        {
            Blend One One

            Name "Unlit0"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Blend One One

            Name "Unlit1"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

    }
}