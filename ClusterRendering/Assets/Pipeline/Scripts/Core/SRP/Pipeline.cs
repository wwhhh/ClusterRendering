using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static PipelineComponent;

public class Pipeline : RenderPipeline
{

    public PipelineCommandData data;
    private PipelineAssets asset;
    private bool bInit = false;

    ClusterRendering rendering;

    public Pipeline(PipelineAssets asset)
    {
        this.asset = asset;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        if (!bInit) { InitData(context); }

        ExecuteCamera(context, cameras);
        context.Submit();
    }

    private void InitData(ScriptableRenderContext context)
    {
        bInit = true;

        data = new PipelineCommandData();
        data.command = new CommandBuffer();
        data.asset = asset;
        data.context = context;

        rendering = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClusterRendering>();
    }

    private void ExecuteCamera(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            Render(context, camera);
        }
    }

    private void Render(ScriptableRenderContext context, Camera camera)
    {
        context.SetupCameraProperties(camera);

        var filteringSettings = new FilteringSettings()
        {
            renderQueueRange = RenderQueueRange.opaque
        };

        Clear(context, camera);

        CommandBuffer drawCommand = new CommandBuffer();
        //rendering.Render(drawCommand);
        context.ExecuteCommandBuffer(drawCommand);
    }

    private void Clear(ScriptableRenderContext context, Camera camera)
    {
        CommandBuffer clearCommand = new CommandBuffer();
        clearCommand.ClearRenderTarget(camera.clearFlags == CameraClearFlags.Depth, camera.clearFlags == CameraClearFlags.Color, camera.backgroundColor);
        context.ExecuteCommandBuffer(clearCommand);
    }

}