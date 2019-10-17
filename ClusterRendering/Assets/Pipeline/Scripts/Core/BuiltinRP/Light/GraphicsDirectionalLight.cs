using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class GraphicsDirectionalLight : GraphicsLight
{

    private Shader _lightingShader;
    private Material _lightingMat;
    private Material lightingMaterial
    {
        get
        {
            if (_lightingMat == null)
            {
                _lightingMat = new Material(_lightingShader);
            }

            return _lightingMat;
        }
    }

    public static GraphicsDirectionalLight I = null;

    private void OnEnable()
    {
        _lightingShader = Shader.Find("Deferred/StandardLighting");

        I = this;
    }

    private void Start()
    {
        CreateRenderTexture();
    }

    private void Update()
    {
        UpdateCameraParams();
    }

    /// <summary>
    /// 延迟光照绘制
    /// </summary>
    public override void Render(RenderTexture rt)
    {
        Light directionalLight = transform.GetComponent<Light>();
        if (directionalLight == null)
        {
            Debug.LogError("缺少主光源, 延迟渲染被打断");
            return;
        }

        lightingMaterial.SetVector(ShaderIDs.ID_DLightDir, -directionalLight.transform.forward);
        lightingMaterial.SetVector(ShaderIDs.ID_DLightColor, directionalLight.color * directionalLight.intensity);

        Graphics.Blit(null, rt, lightingMaterial, 0);
    }


    #region 阴影

    [System.NonSerialized]
    public OrthoCam shadowCamera;

    [System.NonSerialized]
    public RenderTexture[] shadowmapRTSingle;

    public int[] cascadeShadowDistances;

    private const int RESOLUTION = 1024;

    private void CreateRenderTexture()
    {

        shadowmapRTSingle = new RenderTexture[4];
        shadowmapRTSingle[0] = new RenderTexture(RESOLUTION, RESOLUTION, 0);
        shadowmapRTSingle[1] = new RenderTexture(RESOLUTION, RESOLUTION, 0);
        shadowmapRTSingle[2] = new RenderTexture(RESOLUTION, RESOLUTION, 0);
        shadowmapRTSingle[3] = new RenderTexture(RESOLUTION, RESOLUTION, 0);

        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap0, shadowmapRTSingle[0]);
        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap1, shadowmapRTSingle[1]);
        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap2, shadowmapRTSingle[2]);
        Shader.SetGlobalTexture(ShaderIDs.ID_Shadowmap3, shadowmapRTSingle[3]);
    }

    private void UpdateCameraParams()
    {
        shadowCamera.forward = transform.forward;
        shadowCamera.up = transform.up;
        shadowCamera.right = transform.right;
    }

    #endregion

}