using Unity.Mathematics;

public class ClusterComponents
{
    public const int CLUSTERCLIPCOUNT = 255;
    public const string PATH_FOLDER= "Assets/Resources/Scenes/";
    public const string PATH_BINARY = "BinaryInfos/";
    public const string PATH_VERTEX = "vertex_";
    public const string PATH_CLUSTER = "cluster_";
    public const string PATH_MATERIAL = "material_";
    public const string PATH_TEX_TEXTURE = "tex_";
    public const string PATH_TEX_NORMAL = "normal_";

    public const int KERNEL_FRUSTUM_CULLING = 0;
}

public struct Cluster
{
    public int index;
    public float3 position;
    public float3 extent;
}

public struct Point
{
    public float3 vertex;
    public float3 normal;
    public float4 tangent;
    public float2 uv0;
    public int materialID;
}