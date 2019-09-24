using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct MaterialProperties
{
    public float3 _Color;
    [Range(0, 1)]
    public float _SmoothnessIntensity;
    [Range(0, 1)]
    public float _MetallicIntensity;
    public int _AlbedoTex;
    public int _NormalTex;
}