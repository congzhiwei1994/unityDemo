Shader "czw/RenderFeature/SSAO"
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
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        Cull Off
        ZWrite Off
        ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Assets/Scripts/RenderFeature/SSAO/Shader/SSAO.hlsl"
        ENDHLSL

        Pass
        {
            // 0 - Occlusion estimation with CameraDepthTexture
            Blend One One

            Name "SSAO_Occlusion"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment SSAO

            // ����Normal��ѹ��, �����Ǹ߾���Normal.
            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
            // ���������ͼ.
            #pragma multi_compile_local _SOURCE_DEPTH _SOURCE_DEPTH_NORMALS
            // ����Ǵ�Depth ����ؼ��־�����ʲô����.
            #pragma multi_compile_local _RECONSTRUCT_NORMAL_LOW _RECONSTRUCT_NORMAL_MEDIUM _RECONSTRUCT_NORMAL_HIGH
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