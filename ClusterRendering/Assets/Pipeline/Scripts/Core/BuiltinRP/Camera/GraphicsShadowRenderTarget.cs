//using UnityEngine;
//using UnityEngine.Rendering;

//public class GraphicsShadowRenderTarget : RenderTarget
//{
//    public bool debug = true;
//    public bool autoFixedFrustum = true;

//    [Range(1, 4096)]
//    public int resolution = 2048;

//    // 用于计算阴影相机位置的远裁剪面位置
//    [Range(0, 1000)]
//    public float farClipShadowDistance = 50;

//    [Range(0, 0.1f)]
//    public float shadowBias = 0.01f;

//    [Range(0, 1f)]
//    public float shadowSoftValue = 1f;

//    public GraphicsDirectionalLight mainLight;

//    [System.NonSerialized]
//    public RenderTexture shadowmapRT;

//    private void Start()
//    {
//        SetUpCamera();
//        Resize(resolution);
//    }

//    private void SetUpCamera()
//    {
//        if (cam == null) cam = gameObject.AddComponent<Camera>();
//        cam.depthTextureMode = DepthTextureMode.Depth;
//        cam.orthographic = true;
//        cam.enabled = false;
//        cam.backgroundColor = Color.clear;
//        cam.clearFlags = CameraClearFlags.SolidColor;
//        cam.useOcclusionCulling = false;
//        cam.allowHDR = false;
//        cam.allowMSAA = false;
//    }

//    private void Resize(int resolution)
//    {
//        if (shadowmapRT != null)
//        {
//            RenderTexture.ReleaseTemporary(shadowmapRT);
//            shadowmapRT = null;
//        }

//        shadowmapRT = new RenderTexture(new RenderTextureDescriptor
//        {
//            width = resolution,
//            height = resolution,
//            depthBufferBits = 16,
//            colorFormat = RenderTextureFormat.Shadowmap,
//            autoGenerateMips = false,
//            bindMS = false,
//            dimension = TextureDimension.Tex2DArray,
//            enableRandomWrite = false,
//            memoryless = RenderTextureMemoryless.None,
//            shadowSamplingMode = UnityEngine.Rendering.ShadowSamplingMode.RawDepth,
//            msaaSamples = 1,
//            sRGB = false,
//            useMipMap = false,
//            volumeDepth = 4,
//            vrUsage = VRTextureUsage.None
//        });
//        shadowmapRT.filterMode = FilterMode.Bilinear;
//        shadowmapRT.autoGenerateMips = false;

//        Shader.SetGlobalFloat(ShaderIDs.ID_ShadowBias, shadowBias);
//        Shader.SetGlobalFloat(ShaderIDs.ID_ShadowSoftValue, shadowSoftValue);
//        Shader.SetGlobalFloat(ShaderIDs.ID_ShadowmapSize, resolution);
//        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap, shadowmapRT);

//    }

//    private void SetTransform()
//    {
//        Camera camera = Camera.main;

//        Vector3[] nearCorners = new Vector3[4];
//        Vector3[] farCorners = new Vector3[4];

//        CameraUtils.GetFrustumCorner(camera, camera.nearClipPlane, nearCorners);
//        CameraUtils.GetFrustumCorner(camera, farClipShadowDistance, farCorners);

//        Vector3 average = Vector3.zero;

//        for (int i = 0; i < 4; i++)
//        {
//            average += nearCorners[i];
//            average += farCorners[i];
//        }

//        average /= 8;

//        float range = 0;
//        for (int i = 0; i < 4; i++)
//        {
//            float distance = Vector3.Distance(average, nearCorners[i]);
//            if (range < distance)
//            {
//                range = distance;
//            }

//            distance = Vector3.Distance(average, farCorners[i]);
//            if (range < distance)
//            {
//                range = distance;
//            }
//        }

//        cam.orthographicSize = range;
//        cam.nearClipPlane = 0;
//        cam.farClipPlane = farClipShadowDistance;
//        cam.rect = new Rect(0, 0, 0.5f, 0.5f);

//        Vector3 targetPosition = average - transform.forward * farClipShadowDistance * 0.5f;
//        transform.rotation = mainLight.transform.rotation;
//        Shader.SetGlobalFloat(ShaderIDs.ID_ShadowmapSize, range);

//        if (ShadowMatrixVP == Matrix4x4.identity)
//        {
//            transform.position = targetPosition;
//        }
//        else
//        {
//            Matrix4x4 invShadowVP = ShadowMatrixVP.inverse;

//            Vector3 ndcPos = ShadowMatrixVP.MultiplyPoint(targetPosition);
//            Vector2 uv = new Vector2(ndcPos.x, ndcPos.y) * 0.5f + Vector2.one * 0.5f;
//            uv.x = (int)(uv.x * resolution + 0.5);
//            uv.y = (int)(uv.y * resolution + 0.5);
//            uv /= resolution;
//            uv = uv * 2f - Vector2.one;
//            ndcPos = new Vector3(uv.x, uv.y, ndcPos.z);

//            targetPosition = invShadowVP.MultiplyPoint(ndcPos);
//            transform.position = targetPosition;
//        }
//    }

//    private Matrix4x4 ShadowMatrixVP = Matrix4x4.identity;
//    private Matrix4x4 ShadowMatrixVP_RT = Matrix4x4.identity;
//    private void SetCameraArgs()
//    {
//        // 自动适应相机位置
//        if (autoFixedFrustum) SetTransform();

//        // 平行光投影矩阵
//        Matrix4x4 proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true);
//        ShadowMatrixVP_RT = proj * cam.worldToCameraMatrix;

//        proj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
//        ShadowMatrixVP = proj * cam.worldToCameraMatrix;
//        Shader.SetGlobalMatrix(ShaderIDs.ID_ShadowMatrixVP, ShadowMatrixVP);
//    }

//    private void Update()
//    {
//        SceneController.instance.RenderShadow(this);
//    }

//    private void LateUpdate()
//    {
//        SetCameraArgs();
//    }

//    public Matrix4x4 GetVPMatrix()
//    {
//        return ShadowMatrixVP_RT;
//    }

//}