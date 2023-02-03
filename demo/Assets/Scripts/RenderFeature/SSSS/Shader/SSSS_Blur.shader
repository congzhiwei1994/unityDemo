Shader "czw/RenderFeature/Skin/SSSS_Blur"
{
    Properties
    {
        _MainTex("MainTex",2D) ="White"{}
        _SkinDepthRT("_SkinDepthRT",2D) ="White"{}
    }

    CGINCLUDE
    #include "SeparableSubsurfaceScatterCommon.cginc"
    float EnableSkinSSSDebug;
    ENDCG

    SubShader
    {
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            Name "XBlur"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment XBlur_frag
            #pragma multi_compile _ ENABLE_SKIN_SSSS_DEBUG_ON


            float4 XBlur_frag(v2f i) : SV_TARGET
            {
                #ifdef ENABLE_SKIN_SSSS_DEBUG_ON
                    return tex2Dlod(_MainTex, float4(i.uv, 0, 0));
                #endif

                float4 mainTex = tex2D(_MainTex, i.uv);
                float SSSIntencity = (_SSSScale * _ScreenSize.z);
                //右半部分
                float3 XBlurPlus = SeparableSubsurface(mainTex, i.uv, float2(SSSIntencity, 0), SSSIntencity).rgb;
                //左半部分
                float3 XBlurNagteiv = SeparableSubsurface(mainTex, i.uv, float2(-SSSIntencity, 0), SSSIntencity).rgb;
                float3 XBlur = (XBlurPlus + XBlurNagteiv) / 2;
                return float4(XBlur, mainTex.a);
            }
            ENDCG
        }

        Pass
        {
            Name "YBlur"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment YBlur_frag
            #pragma multi_compile _ ENABLE_SKIN_SSSS_DEBUG_ON

            float4 YBlur_frag(v2f i) : SV_TARGET
            {
                #ifdef ENABLE_SKIN_SSSS_DEBUG_ON
                    return tex2Dlod(_MainTex, float4(i.uv, 0, 0));
                #endif

                float4 mainTex = tex2D(_MainTex, i.uv);
                if (EnableSkinSSSDebug)
                    return mainTex;

                float SSSIntencity = (_SSSScale * _ScreenSize.w);
                //上半部分
                float3 YBlurPlus = SeparableSubsurface(mainTex, i.uv, float2(0, SSSIntencity), SSSIntencity).rgb;
                //下半部分
                float3 YBlurNagteiv = SeparableSubsurface(mainTex, i.uv, float2(0, -SSSIntencity), SSSIntencity).rgb;
                float3 YBlur = (YBlurPlus + YBlurNagteiv) / 2;
                return float4(YBlur, mainTex.a);
            }
            ENDCG
        }
    }
}