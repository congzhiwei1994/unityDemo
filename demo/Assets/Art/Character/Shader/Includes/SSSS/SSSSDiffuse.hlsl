#ifndef SKIN_SSSS_DIFFUSE_INCLUDE
#define SKIN_SSSS_DIFFUSE_INCLUDE


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
    float4 positionCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);

    output.uv = input.texcoord;
    output.normalWS = normalInput.normalWS;
    output.viewDirWS = viewDirWS;
    output.tangentWS = tangentWS;

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
    output.positionWS = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;
    return output;
}


half4 LitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);;
    half3 specular = SAMPLE_TEXTURE2D(_SpecularMap, sampler_SpecularMap, input.uv);
    half roughnessMap = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, input.uv);
    half occlusion = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, input.uv);

    half4 bumpMap = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv);
    half4 bumpMap_Bur = SAMPLE_TEXTURE2D_LOD(_BumpMap, sampler_BumpMap, input.uv, 4);
    half3 normalTS = UnpackNormalScale(bumpMap, _BumpScale);
    half3 normalTS_Bur = UnpackNormalScale(bumpMap_Bur, _BumpScale);

    half2 detailBumpUV = input.uv * _DetailBumpMap_ST.xy + _DetailBumpMap_ST.zw;
    half4 detailbump = SAMPLE_TEXTURE2D(_DetailBumpMap, sampler_DetailBumpMap, detailBumpUV);
    half4 detailbump_Bur = SAMPLE_TEXTURE2D_LOD(_DetailBumpMap, sampler_DetailBumpMap, detailBumpUV, 4);
    half3 detailTS = UnpackNormalScale(detailbump, _DetailScale);
    half3 detailTS_Bur = UnpackNormalScale(detailbump_Bur, _DetailScale);

    half detailMask = SAMPLE_TEXTURE2D(_DetailMaskMap, sampler_DetailMaskMap, input.uv);
    normalTS = lerp(normalTS, detailTS, detailMask);
    normalTS_Bur = lerp(normalTS_Bur, detailTS_Bur, detailMask);

    float3 bitangent = input.tangentWS.w * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tbn = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

    float3 normalWS = NormalizeNormalPerPixel(TransformTangentToWorld(normalTS, tbn));
    float3 normalWS_Bur = NormalizeNormalPerPixel(TransformTangentToWorld(normalTS_Bur, tbn));


    occlusion = lerp(1, occlusion, _Occlusion);

    half alpha = albedoAlpha.a;
    half3 albedo = albedoAlpha.rgb * _BaseColor.rgb;

    SkinInput skin_input;
    skin_input.DiffuseColor = lerp(albedo, half3(0, 0, 0), metallic);
    skin_input.SpecularColor = specular * _Specular * 0.08;
    skin_input.Lobe0Roughness = _Lobe0Roughness * roughness;
    skin_input.Lobe1Roughness = _Lobe1Roughness * roughness;
    skin_input.LobeMix = _LobeMix;
    skin_input.Occlusion = _OcclusionStrength;
    skin_input.EnvRotation = _EnvRotation;
    skin_input.WorldPos = input.positionWS;
    skin_input.worldNormal = normalWS;
    skin_input.worldNormal_Blur = normalWS_Bur;
    skin_input.ViewDirWS = SafeNormalize(input.viewDirWS);
    skin_input.SSSLUT = _SSSLUT;
    skin_input.sampler_SSSLUT = sampler_SSSLUT;
    skin_input.Curvature = curvature;
    skin_input.ClearCoatRoughness = _ClearCoatRoughness;
    skin_input.ClearCoatNormal = normalWS_Bur;
    skin_input.ClearCoat = _ClearCoatIntensity * SAMPLE_TEXTURE2D(
        _ClearCoatMaskMap, sampler_ClearCoatMaskMap, input.uv);


    InputData inputData;
    inputData = (InputData)0;
    float3 bitangent = input.tangentWS.w * cross(input.normalWS.xyz, input.tangentWS.xyz);
    half3x3 tbn = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

    inputData.positionWS = input.positionWS;
    half3 viewDirWS = SafeNormalize(input.viewDirWS);
    inputData.normalWS = TransformTangentToWorld(normalTS, tbn);

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = viewDirWS;
    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = half4(1, 1, 1, 1);
    half4 color = 1;

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color = clamp(0, 1.6, color);
    return color;
}

#endif
