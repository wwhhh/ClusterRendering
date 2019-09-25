using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphicsPipeline : MonoBehaviour
{
    public GraphicsPipelineAsset asset;
    public GraphicsLighting directionLighting;

    private RenderTexture targetRT;
    private RenderTexture targetDepthRT;
    private RenderTexture[] targetGBufferRT;

    private RenderBuffer[] gBuffers;
    private RenderBuffer depthBuffer;

    private void Awake()
    {
        InitTargetBuffers();
        SceneController.instance.SetAsset(asset);
    }

    public void InitTargetBuffers()
    {
        targetRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        targetDepthRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        targetGBufferRT = new RenderTexture[]
        {
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear)
        };

        gBuffers = new RenderBuffer[targetGBufferRT.Length];
        for (int i = 0; i < targetGBufferRT.Length; i++)
        {
            gBuffers[i] = targetGBufferRT[i].colorBuffer;
        }

        depthBuffer = new RenderBuffer();
        depthBuffer = targetDepthRT.depthBuffer;
    }

    private void OnPostRender()
    {

        Camera cam = Camera.current;
        cam.depthTextureMode = DepthTextureMode.Depth;

        // 全局深度图
        Shader.SetGlobalTexture(ShaderIDs.ID_DepthTexture, targetDepthRT);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer0, targetGBufferRT[0]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer1, targetGBufferRT[1]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer2, targetGBufferRT[2]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer3, targetGBufferRT[3]);

        Graphics.SetRenderTarget(gBuffers, depthBuffer);
        GL.Clear(true, true, Color.black);

        SceneController.instance.Render();

        // 绘制到RT上
        directionLighting.Render(targetRT);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 后处理
        Graphics.Blit(targetRT, destination, asset.gammaMaterial);
    }

}