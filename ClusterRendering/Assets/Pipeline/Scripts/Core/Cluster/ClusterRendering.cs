using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static ClusterComponents;

public unsafe class ClusterRendering : MonoBehaviour
{
    public Material instanceMaterial;
    public Light directionalLight;
    public ComputeShader frustumCulling;
    public Camera cam;

    private Bounds bounds;
    private int instanceCount;
    private int vertexCount;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer resultBuffer;
    private ComputeBuffer clusterBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private Point[] points;
    private Cluster[] clusters;
    private float[] results;

    private Vector4[] planesVector;

    void Start()
    {
        // 获得场景名称，用来得到二进制数据路径
        Scene scene = SceneManager.GetActiveScene();

        ParseSceneData(scene.name);
        CreateBuffers();
        CreateFrustumCulling();
    }

    private void Update()
    {
        //Render(null);
    }

    public void Render(CommandBuffer command)
    {
        float4* planes = stackalloc float4[6];
        CameraUtils.GetFrustumPlanes(cam, planes);
        UnsafeUtility.MemCpy(UnsafeUtility.AddressOf(ref planesVector[0]), planes, 6 * sizeof(float4));

        // 这个值必须不停的设置
        frustumCulling.SetVectorArray(ShaderIDs.ID_FrustumPlanes, planesVector);
        frustumCulling.Dispatch(KERNEL_FRUSTUM_CULLING, instanceCount, 1, 1);
        // 材质参数设置
        SetMaterialArgs();
        // 绘制
        //Graphics.DrawProceduralIndirect(instanceMaterial, bounds, MeshTopology.Triangles, argsBuffer);
        command.DrawProceduralIndirect(Matrix4x4.identity, instanceMaterial, 0, MeshTopology.Triangles, argsBuffer);
    }

    void ParseSceneData(string sceneName)
    {
        points = ClusterUtils.ReadBytes<Point>(PATH_FOLDER + PATH_VERTEX + sceneName);
        clusters = ClusterUtils.ReadBytes<Cluster>(PATH_FOLDER + PATH_CLUSTER + sceneName);

        if (points == null || points.Length == 0 || clusters == null || clusters.Length == 0)
        {
            Debug.LogError("场景数据初始化失败，请检查文件路径或者二进制文件是否存在");
        }

        // 初始化全局参数
        if (instanceCount < 1) instanceCount = 1;
        instanceCount = points.Length / CLUSTERCLIPCOUNT + (points.Length % CLUSTERCLIPCOUNT > 0 ? 1 : 0);
        vertexCount = CLUSTERCLIPCOUNT; // 顶点坐标应该是包含index信息的顶点坐标
        
        results = new float[instanceCount];
        bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
    }

    void CreateBuffers()
    {
        CreateArgsBuffer();
        CreateVertexBuffer();
        CreateResultBuffer();
        CreateClusterBuffer();
    }

    void CreateArgsBuffer()
    {
        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = (uint)vertexCount;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);
    }

    void CreateVertexBuffer()
    {
        pointsBuffer = new ComputeBuffer(instanceCount * CLUSTERCLIPCOUNT, sizeof(Point));
        pointsBuffer.SetData(points);
    }

    void CreateResultBuffer()
    {
        for (int i = 0; i < instanceCount; i++)
        {
            results[i] = 1f;
        }

        resultBuffer = new ComputeBuffer(instanceCount, sizeof(int));
        resultBuffer.SetData(results);
    }

    void CreateClusterBuffer()
    {
        clusterBuffer = new ComputeBuffer(instanceCount, sizeof(Cluster));
        clusterBuffer.SetData(clusters);
    }

    void CreateFrustumCulling()
    {
        planesVector = new Vector4[6];
        frustumCulling.SetBuffer(KERNEL_FRUSTUM_CULLING, "resultBuffer", resultBuffer);
        frustumCulling.SetBuffer(KERNEL_FRUSTUM_CULLING, "clusterBuffer", clusterBuffer);
    }

    private void SetMaterialArgs()
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        Matrix4x4 invvp = vp.inverse;
        Shader.SetGlobalMatrix(ShaderIDs.ID_InvVP, invvp);

        if (directionalLight != null)
        {
            Shader.SetGlobalVector(ShaderIDs.ID_DLightDir, -directionalLight.transform.forward);
            Shader.SetGlobalVector(ShaderIDs.ID_DLightColor, directionalLight.color * directionalLight.intensity);
        }

        instanceMaterial.SetBuffer(ShaderIDs.ID_VertexBuffer, pointsBuffer);
        instanceMaterial.SetBuffer(ShaderIDs.ID_ResultBuffer, resultBuffer);
    }

    void OnDisable()
    {
        ReleaseNative();
        ReleaseBuffers();
    }

    void ReleaseBuffers()
    {
        if (pointsBuffer != null) pointsBuffer.Release();
        pointsBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;

        if (resultBuffer != null) resultBuffer.Release();
        resultBuffer = null;

        if (clusterBuffer != null) clusterBuffer.Release();
        clusterBuffer = null;
    }

    void ReleaseNative()
    {
    }

    void OnGUI()
    {
        GUI.Label(new Rect(265, 12, 200, 30), "Instance Count: " + instanceCount.ToString("N0"));
    }

}