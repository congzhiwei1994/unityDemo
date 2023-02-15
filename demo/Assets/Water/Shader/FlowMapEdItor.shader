Shader "czw/Tools/FlowMap/FlowMapEditor"
{
    Properties
    {

        _MainTex("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue"="Geometry" "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline"
        }
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        CBUFFER_END

        float2 _MousePos;
        half _Size;
        half2 _Direction;
        float _BrushStrength;
        float isErase;
        float _UvScale;

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
            float2 uv : TEXCOORD0;
            float fogCoord : TEXCOORD1;
        };


        Varyings vert(Attributes v)
        {
            Varyings o = (Varyings)0;

            o.positionCS = mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(v.positionOS.xyz, 1.0)));
            o.uv = v.uv;
            o.fogCoord = ComputeFogFactor(o.positionCS.z);

            return o;
        }

        half4 frag(Varyings i) : SV_Target
        {
            half4 c;
            half4 flowMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

            if (i.uv.x <= 0.01 || i.uv.x >= 0.99 || i.uv.y <= 0.01 || i.uv.y >= 0.99)
                return half4(0.5, 0.5, 0, 0);

            half grad = length(_MousePos.xy - i.uv);
            grad = 1 - saturate(grad * _Size);

            half2 newVal;
            if (isErase > 0.5)
            {
                newVal = lerp(flowMap, half2(0.5, 0.5), grad * _BrushStrength * 0.25);
            }

            else
            {
                newVal = flowMap + grad * _Direction * _BrushStrength;
                newVal = clamp(newVal, 0.05, 0.95);
            }
            return float4(newVal, 0, 0);
        }

        half4 fragResize(Varyings i) : SV_Target
        {
            i.uv = (i.uv - 0.5) * _UvScale + 0.5;
            if (i.uv.x <= 0.01 || i.uv.x >= 0.99 || i.uv.y <= 0.01 || i.uv.y >= 0.99)
                return half4(0.5, 0.5, 0, 0);

            half2 clampMask = half2(abs(i.uv.x - 0.5), abs(i.uv.y - 0.5)) * 2;
            clampMask = pow(clampMask, 10);
            clampMask = saturate(max(clampMask.x, clampMask.y));
            half2 resizedFlowMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).xy;

            return float4(resizedFlowMap, 0, 0);
        }
        ENDHLSL

        Pass
        {
            Name "Unlit0"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "Unlit1"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragResize
            ENDHLSL
        }

    }
}