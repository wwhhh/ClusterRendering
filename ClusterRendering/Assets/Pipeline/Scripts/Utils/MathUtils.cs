using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class MathUtils
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float4 GetPlane(float3 a, float3 b, float3 c)
    {
        float3 normal = normalize(cross(b - a, c - a));
        return float4(normal, -dot(normal, a));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float4 GetPlane(float3 normal, float3 inPoint)
    {
        return new float4(normal, -dot(normal, inPoint));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float4x4 GetWorldToLocal(float4x4 localToWorld)
    {
        float4x4 rotation = float4x4(float4(localToWorld.c0.xyz, 0), float4(localToWorld.c1.xyz, 0), float4(localToWorld.c2.xyz, 0), float4(0, 0, 0, 1));
        rotation = transpose(rotation);
        float3 localPos = mul(rotation, localToWorld.c3).xyz;
        localPos = -localPos;
        rotation.c3 = float4(localPos.xyz, 1);
        return rotation;
    }
    public static float3x4 GetWorldToLocal(Transform trans)
    {
        float4x4 fx = trans.worldToLocalMatrix;
        return new float3x4(fx.c0.xyz, fx.c1.xyz, fx.c2.xyz, fx.c3.xyz);
    }
}