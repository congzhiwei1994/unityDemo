Shader "czw/Character/Hair/DepthPeelingBlend"
{
    Properties
    {
        _BaseColor("Base Color",color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white" {}
    }

    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)

        CBUFFER_END

        TEXTURE2D(_CameraDepthTexture);
        SAMPLER(sampler_CameraDepthTexture);

        TEXTURE2D(_DepthTex);
        SAMPLER(sampler_DepthTex);

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD1;
        };


        Varyings vert(Attributes v)
        {
            Varyings o = (Varyings)0;

            o.positionCS = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(v.positionOS.xyz, 1.0)));
            o.uv = v.uv;
            return o;
        }

        half4 frag(Varyings i) : SV_Target
        {
            float2 uv = i.positionCS.xy / _ScreenParams.xy;
            // uv = i.uv;
            half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
            // 场景不透明的深度
            float sceneDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
            // 头发的深度
            float blendDepth = SAMPLE_TEXTURE2D(_DepthTex, sampler_DepthTex, uv);

            // 不透明物体遮挡住头发的话，去掉头发的深度
            #if UNITY_REVERSED_Z
            UNITY_BRANCH if (sceneDepth > blendDepth)
            {
                discard;
            }
            #else
            UNITY_BRANCH if (sceneDepth < blendDepth)
            {
                discard;
            }
            #endif

            #if UNITY_REVERSED_Z
            UNITY_BRANCH if (blendDepth = 0)
            {
                discard;
            }
            #else
            UNITY_BRANCH if (blendDepth = 1)
            {
                discard;
            }
            #endif
            return color;
        }
        ENDHLSL

        //与后层的颜色RT混合
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        //第一次混合，与黑色混合
        Pass
        {
            Blend SrcColor Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

    }
}