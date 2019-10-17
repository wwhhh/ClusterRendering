using Unity.Mathematics;
using UnityEngine;

public unsafe struct CascadeShadowmap
{
    public Camera camera;
    public OrthoCam shadowCam;
    public int[] ShadowDistances;

    public Matrix4x4 Execute(int index)
    {

        float3* nearCorners = stackalloc float3[4];
        float3* farCorners = stackalloc float3[4];

        if (index == 0)
        {
            CameraUtils.GetFrustumCorner(camera, camera.nearClipPlane, nearCorners);
            CameraUtils.GetFrustumCorner(camera, ShadowDistances[index], farCorners);
        }
        else
        {
            CameraUtils.GetFrustumCorner(camera, ShadowDistances[index - 1], nearCorners);
            CameraUtils.GetFrustumCorner(camera, ShadowDistances[index], farCorners);
        }

        float3 average = (nearCorners[0] + farCorners[2]) * 0.5f;
        float size = Vector3.Distance(nearCorners[0], farCorners[2]);

        shadowCam.size = size;
        shadowCam.position = average;
        shadowCam.UpdateProjectionMatrix();
        shadowCam.UpdateTRSMatrix();

        Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(shadowCam.projectionMatrix, true);
        Matrix4x4 w2v = shadowCam.worldToCameraMatrix;
        Matrix4x4 matrixVP = gpuProj * w2v;
        return matrixVP;
    }

}