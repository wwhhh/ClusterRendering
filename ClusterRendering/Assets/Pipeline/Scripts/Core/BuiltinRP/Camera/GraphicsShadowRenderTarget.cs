using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsShadowRenderTarget : RenderTarget
{

    [Range(1, 4096)]
    public int resolution = 2048;
    public bool use32BitsDepth;

    public RenderTargetIdentifier shadowmapIdentifier;
    RenderTexture _shadowmapRT;

    private void Start()
    {
        SetUpCamera();
        Resize(resolution);
    }

    private void SetUpCamera()
    {
        if (cam == null) cam = gameObject.AddComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.orthographic = true;
        cam.orthographicSize = 20;
        cam.enabled = false;
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.useOcclusionCulling = false;
        cam.allowHDR = false;
        cam.allowMSAA = false;
    }

    private void Resize(int resolution)
    {
        if (_shadowmapRT != null)
        {
            RenderTexture.ReleaseTemporary(_shadowmapRT);
            _shadowmapRT = null;
        }

        _shadowmapRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        _shadowmapRT.filterMode = FilterMode.Point;
        _shadowmapRT.autoGenerateMips = false;
        shadowmapIdentifier = new RenderTargetIdentifier(_shadowmapRT);

        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap, _shadowmapRT);
        Shader.SetGlobalInt(ShaderIDs.ID_ShadowmapSize, resolution);
    }

    private void SetCameraArgs()
    {
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        Shader.SetGlobalMatrix(ShaderIDs.ID_ShadowMatrixVP, vp);
    }

    private void LateUpdate()
    {
        SetCameraArgs();
    }

    private void Update()
    {
        SceneController.instance.RenderShadow(this);
    }

}