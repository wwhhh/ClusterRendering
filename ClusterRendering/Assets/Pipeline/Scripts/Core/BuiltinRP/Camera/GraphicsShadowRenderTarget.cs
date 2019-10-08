using UnityEngine;
using UnityEngine.Rendering;

public class GraphicsShadowRenderTarget : RenderTarget
{

    [Range(1, 4096)]
    public int resolution = 2048;
    public bool use32BitsDepth;

    // 用于计算阴影相机位置的远裁剪面位置
    [Range(0, 1000)]
    public int farClipShadowDistance = 50;
    public GraphicsDirectionalLight mainLight;

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
    }

    private void SetTransform()
    {
        Camera camera = Camera.main;

        Vector3[] nearCorners = new Vector3[4];
        Vector3[] farCorners = new Vector3[4];
        camera.CalculateFrustumCorners(camera.rect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
        camera.CalculateFrustumCorners(camera.rect, farClipShadowDistance, Camera.MonoOrStereoscopicEye.Mono, farCorners);

        Vector3 average = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            average += nearCorners[i];
            average += farCorners[i];
        }

        average /= 8;

        float range = 0;
        for (int i = 0; i < 4; i++)
        {
            float distance = Vector3.Distance(average, nearCorners[i]);
            if (range < distance)
            {
                range = distance;
            }

            distance = Vector3.Distance(average, farCorners[i]);
            if (range < distance)
            {
                range = distance;
            }
        }

        cam.orthographicSize = range;
        cam.nearClipPlane = 0;
        cam.farClipPlane = farClipShadowDistance;

        Vector3 targetPosition = average - transform.forward * farClipShadowDistance * 0.5f;
        transform.position = targetPosition;
        transform.rotation = mainLight.transform.rotation;
        Shader.SetGlobalFloat(ShaderIDs.ID_ShadowmapSize, range);
    }

    private void SetCameraArgs()
    {
        // 自动适应相机位置
        SetTransform();

        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
        Matrix4x4 vp = proj * cam.worldToCameraMatrix;
        Shader.SetGlobalMatrix(ShaderIDs.ID_ShadowMatrixVP, vp);
    }

    private void Update()
    {
        SetCameraArgs();
        SceneController.instance.RenderShadow(this);
    }

}