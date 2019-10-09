using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderIDs
{
    public static readonly int ID_FrustumPlanes = Shader.PropertyToID("_FrustumPlanes");
    public static readonly int ID_FrustumPoints = Shader.PropertyToID("_FrustumPoints");
    public static readonly int ID_VertexBuffer = Shader.PropertyToID("_VertexBuffer");
    public static readonly int ID_ResultBuffer = Shader.PropertyToID("_ResultBuffer");
    public static readonly int ID_InvVP = Shader.PropertyToID("_InvVP");
    public static readonly int ID_DLightDir = Shader.PropertyToID("_CurLightDir");
    public static readonly int ID_DLightColor = Shader.PropertyToID("_CurLightColor");
    public static readonly int ID_DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int ID_GBuffer0 = Shader.PropertyToID("_GBuffer0");
    public static readonly int ID_GBuffer1 = Shader.PropertyToID("_GBuffer1");
    public static readonly int ID_GBuffer2 = Shader.PropertyToID("_GBuffer2");
    public static readonly int ID_GBuffer3 = Shader.PropertyToID("_GBuffer3");
    public static readonly int ID_MaterialProperties= Shader.PropertyToID("_MaterialPropertiesBuffer");
    public static readonly int ID_Shadowmap = Shader.PropertyToID("_Shadowmap");
    public static readonly int ID_ShadowmapSize = Shader.PropertyToID("_ShadowmapSize");
    public static readonly int ID_ShadowMatrixVP = Shader.PropertyToID("_ShadowMatrixVP");
    public static readonly int ID_ShadowMatrixVPRT = Shader.PropertyToID("_ShadowMatrixVPRT");
    public static readonly int ID_ShadowBias = Shader.PropertyToID("_ShadowBias");
    public static readonly int ID_ShadowSoftValue = Shader.PropertyToID("_ShadowSoftValue");
}