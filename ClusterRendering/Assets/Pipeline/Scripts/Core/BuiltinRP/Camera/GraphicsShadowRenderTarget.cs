using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GraphicsShadowRenderTarget : MonoBehaviour
{
    Camera cam;
    RenderTexture shadowmapRT;
    RenderTexture shadowDepthRT;

    [Range(1, 4096)]
    public int resolution = 2048;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        Resize(resolution);
    }

    private void Resize(int resolution)
    {
        shadowmapRT = new RenderTexture(new RenderTextureDescriptor
        {
            width = resolution,
            height = resolution,
            depthBufferBits = 16,
            colorFormat = RenderTextureFormat.Depth,
            autoGenerateMips = false,
            bindMS = false,
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            enableRandomWrite = false,
            memoryless = RenderTextureMemoryless.None,
            shadowSamplingMode = UnityEngine.Rendering.ShadowSamplingMode.RawDepth,
            msaaSamples = 1,
            sRGB = false,
            useMipMap = false,
            volumeDepth = 4,
            vrUsage = VRTextureUsage.None
        });

        shadowDepthRT = new RenderTexture(new RenderTextureDescriptor
        {
            width = resolution,
            height = resolution,
            depthBufferBits = 16,
            colorFormat = RenderTextureFormat.Shadowmap,
            autoGenerateMips = false,
            bindMS = false,
            dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
            enableRandomWrite = false,
            memoryless = RenderTextureMemoryless.None,
            shadowSamplingMode = UnityEngine.Rendering.ShadowSamplingMode.RawDepth,
            msaaSamples = 1,
            sRGB = false,
            useMipMap = false,
            volumeDepth = 4,
            vrUsage = VRTextureUsage.None
        });
    }

    private void OnPreRender()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;

        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;

        Shader.SetGlobalMatrix(ShaderIDs.ID_ShadowMatrixVP, vp);
        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap, shadowDepthRT);
    }

    private void OnPostRender()
    {
        Graphics.SetRenderTarget(shadowDepthRT, 0, CubemapFace.Unknown, 16);
        GL.Clear(true, true, Color.black);

        SceneController.instance.RenderShadow(cam);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(shadowDepthRT, destination);
    }

}