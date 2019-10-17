using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct OrthoCam
{

    public float4x4 worldToCameraMatrix;
    public float4x4 localToWorldMatrix;
    public float3 right;
    public float3 up;
    public float3 forward;
    public float3 position;
    public float size;
    public float nearClipPlane;
    public float farClipPlane;
    public float4x4 projectionMatrix;

    public void UpdateTRSMatrix()
    {
        localToWorldMatrix.c0 = new float4(right, 0);
        localToWorldMatrix.c1 = new float4(up, 0);
        localToWorldMatrix.c2 = new float4(forward, 0);
        localToWorldMatrix.c3 = new float4(position, 1);
        worldToCameraMatrix = MathUtils.GetWorldToLocal(localToWorldMatrix);
        worldToCameraMatrix.c0.z = -worldToCameraMatrix.c0.z;
        worldToCameraMatrix.c1.z = -worldToCameraMatrix.c1.z;
        worldToCameraMatrix.c2.z = -worldToCameraMatrix.c2.z;
        worldToCameraMatrix.c3.z = -worldToCameraMatrix.c3.z;
    }
    public void UpdateProjectionMatrix()
    {
        projectionMatrix = Matrix4x4.Ortho(-size, size, -size, size, nearClipPlane, farClipPlane);
    }
}