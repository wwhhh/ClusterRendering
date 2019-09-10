using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Pipeline : RenderPipeline
{

    public PipelineAssets asset;

    public Pipeline(PipelineAssets asset)
    {
        this.asset = asset;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        ExecuteCamera(context, cameras);
        context.Submit();
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
        // Clear
        Clear(context, camera);
        // PreRender
        PreRender();
        // Culling
        Culling();
        // PostRender
        PostRender();
        // OnRenderImage
        OnRenderImage();
    }

    private void Clear(ScriptableRenderContext context, Camera camera)
    {
        CommandBuffer clearCommand = new CommandBuffer();
        clearCommand.ClearRenderTarget(camera.clearFlags == CameraClearFlags.Depth, camera.clearFlags == CameraClearFlags.Color, camera.backgroundColor);
        context.ExecuteCommandBuffer(clearCommand);
    }

    /// <summary>
    /// 用于设置值
    /// </summary>
    private void PreRender()
    {

    }

    private void Culling()
    {

    }

    private void PostRender()
    {

    }

    private void OnRenderImage()
    {

    }
}