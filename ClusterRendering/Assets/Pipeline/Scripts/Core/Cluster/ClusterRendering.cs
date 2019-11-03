using UnityEngine;
using UnityEngine.Rendering;
using static ClusterComponents;

public unsafe class ClusterRendering : ICommand, ISceneComponent
{
    GraphicsPipelineAsset asset;

    int instanceCount;
    int vertexCount;
    ComputeBuffer argsBuffer;
    ComputeBuffer pointsBuffer;

    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    Point[] points;

    string sceneName;
    bool bRunning;

    CommandBuffer drawCmd;
    CommandBuffer[] shadowCmds;

    public ClusterRendering()
    {
    }

    public override void Init(GraphicsPipelineAsset asset)
    {
        this.asset = asset;
    }

    public void LoadScene(string name, int clusterCount)
    {
        sceneName = name;
        instanceCount = clusterCount;

        ParseSceneData(sceneName);
        CreateBuffers();

        bRunning = true;
    }

    public void UnLoadScene()
    {
        Clear();
        bRunning = false;
    }

    public override void Render(RenderTarget rt)
    {
        if (!bRunning) return;
        Camera camera = rt.cam;
        int cascadeCount = 1;

        Shader.SetGlobalBuffer(ShaderIDs.ID_VertexBuffer, pointsBuffer);
        if (asset.enableShadow) // 阴影绘制
        {

        }

        if (drawCmd == null) // Cluster场景绘制
        {
            if (shadowCmds != null && shadowCmds.Length == cascadeCount)
            {
                for (int i = 0; i < shadowCmds.Length; i++)
                    camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, shadowCmds[i]);
            }

            GraphicsRenderTarget renderTarget = rt as GraphicsRenderTarget;
            CreateDrawCmd(renderTarget);
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, drawCmd);
        }
    }

    void ParseSceneData(string sceneName)
    {
        points = ClusterUtils.ReadBytes<Point>(PATH_FOLDER + "/"+ sceneName +"/" + PATH_VERTEX + sceneName);
        if (points == null || points.Length == 0)
        {
            Debug.LogError("场景数据初始化失败，请检查文件路径或者二进制文件是否存在");
        }

        // 初始化全局参数
        if (instanceCount < 1) instanceCount = 1;
        instanceCount = points.Length / CLUSTERCLIPCOUNT + (points.Length % CLUSTERCLIPCOUNT > 0 ? 1 : 0);
        vertexCount = CLUSTERCLIPCOUNT; // 顶点坐标应该是包含index信息的顶点坐标
    }

    void CreateBuffers()
    {
        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)vertexCount;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        pointsBuffer = new ComputeBuffer(instanceCount * CLUSTERCLIPCOUNT, sizeof(Point));
        pointsBuffer.SetData(points);
    }

    CommandBuffer CreateShadowCmd(GraphicsDirectionalLight light, int pass)
    {
        return null;
    }

    void CreateDrawCmd(GraphicsRenderTarget rt)
    {
        drawCmd = CommandBufferPool.Get("DrawProceduralIndirect: Draw Clusters");
        drawCmd.SetRenderTarget(rt.gBufferIdentifier, rt.depthIdentifier);
        drawCmd.ClearRenderTarget(true, true, Color.black);
        drawCmd.DrawProceduralIndirect(Matrix4x4.identity, asset.deferredMaterial, 0, MeshTopology.Triangles, argsBuffer);
    }

    void ReleaseBuffers()
    {
        if (pointsBuffer != null) pointsBuffer.Release();
        pointsBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;
    }

    void ReleaseNative()
    {
        points = null;
    }

    public override void Clear()
    {
        ReleaseNative();
        ReleaseBuffers();

        foreach (var cmd in shadowCmds) CommandBufferPool.Release(cmd);
        CommandBufferPool.Release(drawCmd);
    }

    public override void Dispose()
    {
        Clear();
    }

}