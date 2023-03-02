Shader "czw/Scene/FlowMap/Water"
{
    Properties
    {
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            float4 _BaseMap_ST;

            CBUFFER_END
            TEXTURE2D(_FlowMapTex);
            SAMPLER(sampler_linear_clamp); // SAMPLER(sampler_FlowMapTex);

            float3 KW_FlowMapOffset;
            half KW_FlowMapSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);

                o.positionCS = vertexInput.positionCS;
                o.positionWS = vertexInput.positionWS;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 flowMapUV = (i.positionWS.xz - KW_FlowMapOffset.xz) / KW_FlowMapSize + 0.5;
                float2 flowmap = SAMPLE_TEXTURE2D(_FlowMapTex, sampler_linear_clamp, flowMapUV).xy * 2 - 1;
                return float4(flowmap, 0, 0);
            }
            ENDHLSL
        }


        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

    }
}