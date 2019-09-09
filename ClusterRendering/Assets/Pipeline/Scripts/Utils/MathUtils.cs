using System.Runtime.CompilerServices;
using Unity.Mathematics;
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

}
