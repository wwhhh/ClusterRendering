using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class GraphicsRenderTarget : RenderTarget
{

    public GraphicsPipeline pipeline;
    public GraphicsLighting directionLighting;

    public RenderTargetIdentifier[] gBufferIdentifier;
    public RenderTargetIdentifier depthIdentifier;

    RenderTexture targetRT;
    RenderTexture targetDepthRT;
    RenderTexture[] targetGBufferRT;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        Resize();
    }

    private void Resize()
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

        gBufferIdentifier = new RenderTargetIdentifier[targetGBufferRT.Length];
        for (int i = 0; i < targetGBufferRT.Length; i++)
        {
            gBufferIdentifier[i] = new RenderTargetIdentifier(targetGBufferRT[i]);
        }

        depthIdentifier = new RenderTargetIdentifier(targetDepthRT);

        cam.depthTextureMode = DepthTextureMode.Depth;
        // 全局深度图
        Shader.SetGlobalTexture(ShaderIDs.ID_DepthTexture, targetDepthRT);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer0, targetGBufferRT[0]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer1, targetGBufferRT[1]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer2, targetGBufferRT[2]);
        Shader.SetGlobalTexture(ShaderIDs.ID_GBuffer3, targetGBufferRT[3]);
    }

    private void LateUpdate()
    {
        SetGlobalArgs(cam);
    }

    private void Update()
    {
        GL.Clear(true, true, Color.black);

        SceneController.instance.Render(this);
        directionLighting.Render(targetRT);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 后处理
        Graphics.Blit(targetRT, destination);
    }

    private void SetGlobalArgs(Camera cam)
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        Matrix4x4 invvp = vp.inverse;
        Shader.SetGlobalMatrix(ShaderIDs.ID_InvVP, invvp);
    }

}