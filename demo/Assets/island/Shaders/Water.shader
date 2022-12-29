Shader "czw/Island/water"
{
    Properties
    {
        _ShallowColor("ShallowColor", Color) = (1,1,1,1)
        _DeepColor("DeepColor", Color) = (1,1,1,1)
        _DeepDistance("DeepDistance", Range(0.0, 1.0)) = 1
        _FresnelColor("FresnelColor", Color) = (1,1,1,1)
        _FresnelRange("_FresnelRange",Range(0,5)) = 1
        _PlanarRefleDistort("PlanarRefleDistort",Range(0,1)) = 0.5
        //        _ShallowDistance("ShallowDistance", Range(0.0, 1.0)) = 1
        //        _Density("Density", Range(0.0, 1.0)) = 1

        _NormalMap("BumpMap",2D) = "bump"{}
        _NormalTilling("NormalTilling", Range(0.0, 1.0)) = 0.5
        _NormalScale("NormalScale", Range(0.0, 2.0)) = 0.5
        _OpaqueDistort("OpaqueDistort", Range(0.0, 1.0)) = 0.5

        _FoamMap("FoamMap",2D) = "white"{}
        _FoamMapTilling("FoamMapTilling", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {

        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel"="4.5"
        }
        LOD 300

        Pass
        {
            Name "Forward"
            Tags
            {
                "LightMode"="UniversalForwardOnly"
            }

            HLSLPROGRAM
            // #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            // Unity defined keywords
            #pragma multi_compile_fog
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #define _NORMALMAP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            half _DeepDistance;
            half3 _DeepColor;
            half3 _ShallowColor;
            half4 _FresnelColor;
            half4 _NoiseMap_ST;
            half4 _NormalMap_ST;
            half _OpaqueDistort;
            half _FresnelRange;
            half _PlanarRefleDistort;
            half _NormalTilling;
            half _NormalScale;
            half _FoamMapTilling;
            CBUFFER_END

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            TEXTURE2D(_PlanarReflectionTexture);
            SAMPLER(sampler_PlanarReflectionTexture);

            TEXTURE2D(_FoamMap);
            SAMPLER(sampler_FoamMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
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
                float3 positionVS : TEXCOORD7;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3 GetWorldSpaceViewDir1(float3 positionWS)
            {
                // Perspective
                return GetCurrentViewPosition() - positionWS;
            }

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);

                output.uv = input.texcoord;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = tangentWS;
                output.viewDirWS = normalize(GetWorldSpaceViewDir1(vertexInput.positionWS));
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.positionVS = vertexInput.positionVS;

                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                return output;
            }

            half3 WaterNoramlTS(half2 worldUV, half tilling)
            {
                half2 normalUVA = worldUV * 0.01 * tilling + _NormalMap_ST.zw * _Time.y;
                half4 normalMapA = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, normalUVA);
                half3 normalTSA = UnpackNormalScale(normalMapA, _NormalScale);
                half2 normalUVB = worldUV * 0.02 * tilling - _NormalMap_ST.zw * _Time.y;
                half4 normalMapB = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, normalUVB);
                half3 normalTSB = UnpackNormalScale(normalMapB, _NormalScale);;
                half3 normalTS = BlendNormal(normalTSA, normalTSB);
                return normalTS;
            }

            half4 LitPassFragment(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 worldUV = i.positionWS.xz;

                float2 screenUV = i.positionCS.xy / _ScreenParams.xy;
                half depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV);
                float3 depthpositionWS = ComputeWorldSpacePosition(screenUV, depth,
                                                                   UNITY_MATRIX_I_VP);
                float depthDistance = length(depthpositionWS - i.positionWS);
                float depthFade = saturate(depthDistance / max(0.01, (_DeepDistance * 50)));

                half3 normalTS = WaterNoramlTS(worldUV, _NormalTilling * 2);
                half3 normalWS = normalTS.yxz;

                half2 opaqueUV = lerp(screenUV, screenUV + normalTS * 0.01, _OpaqueDistort);
                half3 opaqueColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, opaqueUV);


                half NoV = saturate(dot(normalize(i.normalWS), i.viewDirWS));

                half3 fresnelColor = pow((1 - NoV), _FresnelRange * 10) * _FresnelColor.rgb;

                half2 planeRefleUV = lerp(screenUV, normalWS.xy + screenUV, _PlanarRefleDistort * 0.01);
                half3 planeRefleTex = SAMPLE_TEXTURE2D(_PlanarReflectionTexture, sampler_PlanarReflectionTexture,
                                                       planeRefleUV);

                half2 foamUV = depthpositionWS.xz * _FoamMapTilling *0.1 + normalTS * 0.01 ;
                half3 foamMap = SAMPLE_TEXTURE2D(_FoamMap, sampler_FoamMap, foamUV);
                foamMap = foamMap * (1 - depthFade);
                half3 waterColor = lerp(_ShallowColor, _DeepColor, depthFade);
                half3 sceneColor = lerp(opaqueColor, planeRefleTex, depthFade) * waterColor;
                sceneColor += foamMap;
                return sceneColor.xyzz;
            }
            ENDHLSL
        }

    }
}