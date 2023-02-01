Shader "czw/Character/Skin/SSSS(Seperable SubSurface Scattering)"
{
    Properties
    {

        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        [Space(10)]
        [Header(Normal map)]
        _BumpMap("BumpMap", 2D) = "bump" {}
        _BumpScale("Bump Scale", Range(0.0, 1.0)) = 1.0
        _DetailBumpMap("Detail BumpMap", 2D) = "bump" {}
        _DetailScale("Detail Scale", Range(0.0, 1.0)) = 1.0
        [NOScaleOffset] _DetailMaskMap("DetailMaskMap", 2D) = "white" {}

        [Space(10)]
        _SpecularMap("Specular Map", 2D) = "white" {}
        _RoughnessMap("Roughnessr Map", 2D) = "white" {}
        _OcclusionMap("Occlusion Map", 2D) = "white" {}

        [Space(10)]
        _Specular("Specular", Range(0.0, 1.0)) = 0.5
        _Roughness0("Roughness0", Range(0.0, 1.0)) = 0.35
        _Roughness1("Roughness1", Range(0.0, 1.0)) = 0.25
        _RoughnessMix("RoughnessMix", Range(0.0, 1.0)) = 0.85
        _Occlusion("Occlusion", Range(0.0, 1.0)) = 1.0

    }

    SubShader
    {

        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel"="4.5"
        }

        Pass
        {
            Name "SkinSSS_Diffuse"
            Tags
            {
                "LightMode" = "SkinSSSS"
            }

            HLSLPROGRAM
            #pragma target 4.5

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

            #pragma multi_compile _ DISABLE_SH
            #pragma multi_compile _ DISABLE_SPECULAR

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment_Diffuse

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include"Assets/Art/Character/Shader/Includes/SSSS/SSSS_SkinInput.hlsl"
            #include"Assets/Art/Character/Shader/Includes/SSSS/SSSS_SkinForwarld.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma target 4.5

            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment_Specular

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include"Assets/Art/Character/Shader/Includes/SSSS/SSSS_SkinInput.hlsl"
            #include"Assets/Art/Character/Shader/Includes/SSSS/SSSS_SkinForwarld.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
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