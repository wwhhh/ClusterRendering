using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using static ClusterComponents;
using static ClusterRendering;

public unsafe class ClusterFrustumCulling : ICommand
{
    GraphicsPipelineAsset asset;

    ComputeShader frustumCulling;
    ComputeBuffer resultBuffer;
    ComputeBuffer clusterBuffer;
    bool enableFrustumCulling;

    float[] results;
    Vector4[] planesVector;
    Cluster[] clusters;

    string sceneName;
    int instanceCount;

    bool bRunning;

    public void LoadScene(string sceneName, int clusterCount)
    {
        this.sceneName = sceneName;
        this.instanceCount = clusterCount;

        ParseSceneData(sceneName);
        CreateBuffers();

        bRunning = true;
    }

    public void UnLoadScene()
    {
        Clear();

        bRunning = false;
    }

    public override void Init(GraphicsPipelineAsset asset)
    {
        this.asset = asset;
        enableFrustumCulling = asset.enableFrustumCulling;
        frustumCulling = asset.frustumCulling;
    }

    public override void Render(RenderTarget rt)
    {
        if (!bRunning) return;

        Camera camera = rt.cam;
        float4* planes = stackalloc float4[6];
        CameraUtils.GetFrustumPlanes(camera, planes);
        UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref planesVector[0]), planes, 6 * sizeof(float4));

        // 视锥体裁剪
        if (enableFrustumCulling)
        {
            frustumCulling.SetVectorArray(ShaderIDs.ID_FrustumPlanes, planesVector);
            frustumCulling.Dispatch(KERNEL_FRUSTUM_CULLING, instanceCount, 1, 1);
        }

        Shader.SetGlobalBuffer(ShaderIDs.ID_ResultBuffer, resultBuffer);
    }

    void ParseSceneData(string sceneName)
    {
        clusters = ClusterUtils.ReadBytes<Cluster>(PATH_FOLDER + "/" + sceneName + "/" + PATH_CLUSTER + sceneName);
        if (clusters == null || clusters.Length == 0)
        {
            Debug.LogError("场景数据初始化失败，请检查文件路径或者二进制文件是否存在");
        }
    }

    void CreateBuffers()
    {
        results = new float[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            results[i] = 1f;
        }

        resultBuffer = new ComputeBuffer(instanceCount, sizeof(int));
        resultBuffer.SetData(results);

        clusterBuffer = new ComputeBuffer(instanceCount, sizeof(Cluster));
        clusterBuffer.SetData(clusters);

        planesVector = new Vector4[6];
        frustumCulling.SetBuffer(KERNEL_FRUSTUM_CULLING, "resultBuffer", resultBuffer);
        frustumCulling.SetBuffer(KERNEL_FRUSTUM_CULLING, "clusterBuffer", clusterBuffer);
    }

    public override void Clear()
    {
        if (resultBuffer != null) resultBuffer.Release();
        resultBuffer = null;

        if (clusterBuffer != null) clusterBuffer.Release();
        clusterBuffer = null;

        results = null;
        clusters = null;
        planesVector = null;
    }

    public override void Dispose()
    {
        Clear();
    }
}