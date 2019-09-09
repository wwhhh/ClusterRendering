using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

public unsafe class CameraUtils
{

    public static float4* GetFrustumPlanes(Camera camera, float4* planes)
    {
        Transform trs = camera.transform;

        float3* corners = stackalloc float3[4];
        float3* corners2 = stackalloc float3[4];
        GetFrustumCorner(camera, camera.nearClipPlane, corners);
        GetFrustumCorner(camera, camera.farClipPlane, corners2);
        planes[0] = MathUtils.GetPlane(corners[0], corners[1], corners[2]);//近
        planes[1] = MathUtils.GetPlane(corners[0], corners[2], corners2[0]);//左
        planes[2] = MathUtils.GetPlane(corners[1], corners[0], corners2[0]);//下
        planes[3] = MathUtils.GetPlane(corners[3], corners[1], corners2[1]);//右
        planes[4] = MathUtils.GetPlane(corners[2], corners[3], corners2[2]);//上
        planes[5] = MathUtils.GetPlane(corners2[1], corners2[2], corners2[3]);//远

        return planes;
    }

    public static void GetFrustumCorner(Camera camera, float distance, float3* corners)
    {
        Transform trs = camera.transform;

        float fov = Mathf.Deg2Rad * camera.fieldOfView * 0.5f;
        float upLength = distance * tan(fov);
        float rightLength = upLength * camera.aspect;
        float3 farPoint = trs.position + distance * trs.forward;
        float3 upVec = upLength * trs.up;
        float3 rightVec = rightLength * trs.right;
        corners[0] = farPoint - upVec - rightVec;
        corners[1] = farPoint - upVec + rightVec;
        corners[2] = farPoint + upVec - rightVec;
        corners[3] = farPoint + upVec + rightVec;
    }

    public static void GetFrustumCorner(Camera camera, float3* corners)
    {
        float fov = tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f);
        void GetCorner(float dist, ref Camera persp)
        {
            Transform trs = camera.transform;

            float upLength = dist * (fov);
            float rightLength = upLength * persp.aspect;
            float3 farPoint = trs.position + dist * trs.forward;
            float3 upVec = upLength * trs.up;
            float3 rightVec = rightLength * trs.right;
            corners[0] = farPoint - upVec - rightVec;
            corners[1] = farPoint - upVec + rightVec;
            corners[2] = farPoint + upVec - rightVec;
            corners[3] = farPoint + upVec + rightVec;
            corners += 4;
        }
        GetCorner(camera.nearClipPlane, ref camera);
        GetCorner(camera.farClipPlane, ref camera);
    }
}
