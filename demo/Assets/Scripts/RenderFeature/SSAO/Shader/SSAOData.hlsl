#ifndef POST_SSAO_DATA_INCLUDE
#define POST_SSAO_DATA_INCLUDE


/**
 * \brief ��ȡ�ӿռ���������ֵ��
 * �����������������Բ�ֵ. ͸�������������,
 * \param uv 
 * \return 
 */
float SampleAndGetLinearEyeDepth1(float2 uv)
{
    float rawDepth = SampleSceneDepth(uv.xy);
    #if defined(_ORTHOGRAPHIC)
    return LinearDepthToEyeDepth(rawDepth);
    #else
    return LinearEyeDepth(rawDepth, _ZBufferParams);
    #endif
}

/**
 * \brief ����ViewPos(������������������λ��), ������������ϵ��, �����λ�õ���ĻUV���������������ĵ������.
 * \param uv 
 * \param depth 
 * \return 
 */
half3 ReconstructViewPos1(float2 uv, float depth)
{
    // Screen is y-inverted.
    uv.y = 1.0 - uv.y;

    // view pos in world space
    #if defined(_ORTHOGRAPHIC)
        float zScale = depth * _ProjectionParams.w; // divide by far plane
        float3 viewPos = _CameraViewTopLeftCorner[unity_eyeIndex].xyz
                            + _CameraViewXExtent[unity_eyeIndex].xyz * uv.x
                            + _CameraViewYExtent[unity_eyeIndex].xyz * uv.y
                            + _CameraViewZExtent[unity_eyeIndex].xyz * zScale; 
    
    #else

    float zScale = depth * _ProjectionParams2.x; // divide by near plane
    float3 viewPos = _CameraViewTopLeftCorner.xyz + _CameraViewXExtent.xyz * uv.x + _CameraViewYExtent.xyz * uv.y;
    viewPos *= zScale;
    #endif

    return half3(viewPos);
}

void SampleDepthNormalView1(float2 uv, out float depth, out half3 normal, out half3 vpos)
{
    depth = SampleAndGetLinearEyeDepth1(uv);
    vpos = ReconstructViewPos1(uv, depth);

    #if defined(_SOURCE_DEPTH_NORMALS)
    normal = half3(SampleSceneNormals(uv));
    #else
    normal = ReconstructNormal(uv, depth, vpos);
    #endif
}

#endif
