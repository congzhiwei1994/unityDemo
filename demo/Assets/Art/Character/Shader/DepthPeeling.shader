Shader "czw/Character/Hair/DepthPeeling"
{
    Properties
    {
        _BaseColor("Base Color",color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue"="Transparent"
        }

        Pass
        {

            Name "DepthPeelingPass"
            Tags
            {
                "LightMode" = "DepthPeelingPass"
            }

            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_MaxDepthTex);
            SAMPLER(sampler_MaxDepthTex);

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            int _DepthPeelingPassCount;


            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS :NORMAL;
                float4 tangentOS :TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            struct DepthPeelingOutput
            {
                float4 color :SV_TARGET0;
                float4 depth :SV_TARGET1;
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            // 结构体函数
            DepthPeelingOutput frag(Varyings input)
            {
                DepthPeelingOutput output;

                half4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                output.color = baseMap * _BaseColor;
                output.depth = input.positionCS.z;

                // 第一次循环，直接画颜色与深度
                UNITY_BRANCH if (_DepthPeelingPassCount == 0)
                {
                    return output;
                }

                float2 screenUV = input.positionCS.xy / _ScreenParams.xy;
                float depthTex = SAMPLE_TEXTURE2D(_MaxDepthTex, sampler_MaxDepthTex, screenUV).r;

                //如果当前相机离相机更近，那么丢弃该像素,只渲染距离摄像机远的像素
                #if UNITY_REVERSED_Z
                if (input.positionCS.z >= depthTex)
                {
                    discard;
                }
                #else
                if (input.positionCS.z <= depthTex)
                {
                    discard;
                }
                #endif


                return output;
            }
            ENDHLSL
        }

    }
}