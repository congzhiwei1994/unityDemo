Shader "czw/Post-Processing/Scan"
{
    Properties
    {
//        _ScanDistance("_ScanDistance",float )= 1
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
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 positionOS : TEXCOORD1;
                half2 uv_depth : TEXCOORD2;
                float4 interpolatedRay : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            float4 _BaseMap_ST;
            float4x4 _FrustumCornersRay;
            half _FogDensity;
            float4 _FogColor;
            float _FogStart;
            float _FogEnd;
            half _FogXSpeed;
            half _FogYSpeed;
            half _NoiseAmount;
            float4 _ScreenWithFogTex_TexelSize;
            CBUFFER_END
            float4 _ScanCenter;
            float3 _ScanColor;
            float _Scale;
            float _ScanDistance;
            half _FallOff;
            float _ScanLineWith;
            float _ScanLineRange;
            float3 _LineColor;
            float3 _DotColor;

            TEXTURE2D(_PostProcessingScanTexture);
            SAMPLER(sampler_PostProcessingScanTexture);
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_NoiseTexture);
            SAMPLER(sampler_NoiseTexture);

            Varyings vert(Attributes v)
            {
                Varyings o = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
                o.uv_depth = v.texcoord;
                o.uv = v.texcoord;
                #if UNITY_UV_STARTS_AT_TOP
                if (_ScreenWithFogTex_TexelSize.y < 0)
                    o.uv_depth.y = 1 - o.uv_depth.y;
                #endif

                int index = 0;
                if (v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
                {
                    index = 0;
                }
                else if (v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
                {
                    index = 1;
                }
                else if (v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
                {
                    index = 2;
                }
                else
                {
                    index = 3;
                }

                #if UNITY_UV_STARTS_AT_TOP
                if (_ScreenWithFogTex_TexelSize.y < 0)
                    index = 3 - index;
                #endif
                o.interpolatedRay = _FrustumCornersRay[index];
                o.positionCS = vertexInput.positionCS;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half2 screenUV = i.positionCS.xy / _ScreenParams.xy;
                half depthMap = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV);
                half3 screenTex = SAMPLE_TEXTURE2D(_PostProcessingScanTexture, sampler_PostProcessingScanTexture,
                                                   i.uv);
                half grayTex = Luminance(screenTex);
                half linearDepth = LinearEyeDepth(depthMap, _ZBufferParams);
                float3 pixelWorldPos = (_WorldSpaceCameraPos) + linearDepth * i.interpolatedRay.xyz;

                float distanceFromCenter = distance(pixelWorldPos, _ScanCenter);
                distanceFromCenter = saturate(distanceFromCenter - (_ScanDistance * 4 - 0.5) * 10);

                float fallOff = 1 - saturate(distanceFromCenter / _FallOff);
                fallOff = 1 - distanceFromCenter;

                half2 lineXZ = frac(pixelWorldPos.xz);
                half dotMask = pow(1 - distance(lineXZ, half2(0.5, 0.5)), 50);
                half3 cotColor = _DotColor * 2 * saturate(dotMask) * fallOff;

                half2 lineMask = 1 - step(lineXZ, half2(0.98, 0.98));
                half3 lineColor = (lineMask.x + lineMask.y) * _LineColor.xyz * fallOff;


                half scanLineMask = 1 - abs(distanceFromCenter * 2 - 1);

                half3 sceneColor = lerp(grayTex, screenTex, saturate(distanceFromCenter));
                sceneColor += scanLineMask * _ScanColor + lineColor + cotColor;
                return sceneColor.xyzz;
            }
            ENDHLSL
        }
    }
}