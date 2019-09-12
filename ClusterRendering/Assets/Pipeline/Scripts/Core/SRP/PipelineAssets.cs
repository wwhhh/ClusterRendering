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

    //public PipelineTargets targets;
    //public PipelineBuffers buffers;

}