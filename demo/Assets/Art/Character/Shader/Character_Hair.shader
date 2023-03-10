Shader "czw/Character/Hair"
{
    Properties
    {

        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        [NOScaleOffset] [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [NOScaleOffset] _OpacityMap("OpacityMap", 2D) = "white" {}
        [NOScaleOffset] _FlowMap("FlowMap", 2D) = "white" {}
        [NOScaleOffset] _IDMap("IDMap", 2D) = "white" {}
        [NOScaleOffset] _DepthMap("DepthMap", 2D) = "white" {}

        [Space(10)]
        _HairClip("HairClip", Range(0.0, 1.0)) = 0.1
        _HairShadowClip("Hair ShadowClip", Range(0.0, 1.0)) = 0.1
        _Specular("Specular", Range(0.0, 1.0)) = 0.5
        _Roughness("Roughness", Range(0.0, 1.0)) = 0.35
        _Scatter("Scatter", Range(0.0, 1.0)) = 1.0
        _NoiseIntensity("NoiseIntensity", Range(0.0, 1.0)) = 1.0

    }

    SubShader
    {

        Tags
        {
            "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "DepthPeelingPass"
            Tags
            {
                "LightMode" = "DepthPeelingPass"
            }


            ZWrite On
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma target 4.5

            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #pragma vertex LitPassVertex
            // #pragma fragment LitPassFragment
            #pragma fragment DepthPeelingFragment
            
            #include"Assets/Art/Character/Shader/Includes/Hair/HairInput.hlsl"
            #include"Assets/Art/Character/Shader/Includes/Hair/HairForwarld.hlsl"
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

            
            #include"Assets/Art/Character/Shader/Includes/Hair/HairInput.hlsl"
            #include"Assets/Art/Character/Shader/Includes/Hair/HairShadowCaster.hlsl"
            ENDHLSL
        }

    }
}