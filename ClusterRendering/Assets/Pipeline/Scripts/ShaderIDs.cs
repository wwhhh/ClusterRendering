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
}