using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GraphicsShadowRenderTarget : MonoBehaviour
{
    Camera cam;
    RenderTexture shadowmapRT;

    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        Resize();
    }

    private void Resize()
    {
        shadowmapRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
    }

    private void OnPreRender()
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;

        Shader.SetGlobalMatrix(ShaderIDs.ID_ShadowMatrixVP, vp);
        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap, shadowmapRT);

        cam.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnPostRender()
    {
        
    }


}