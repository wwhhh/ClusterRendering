using UnityEngine;
using UnityEngine.Rendering;
using static PipelineComponent;

[CreateAssetMenu(menuName = "Pipeline/CreatePipelineAsset")]
public class PipelineAssets : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new Pipeline(this);
    }

    public Light directionalLight;
    public ComputeShader frustumCulling;
    public Camera cam;

    public int instanceCount;       // Cluster数量
    public int vertexCount;         // 顶点的数量

}