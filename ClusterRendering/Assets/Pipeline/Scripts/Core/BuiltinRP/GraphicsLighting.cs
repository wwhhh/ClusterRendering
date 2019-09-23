using UnityEngine;

public class GraphicsLighting : MonoBehaviour
{
    private Shader _lightingShader;

    private Material _mat;
    private Material LightingMaterial
    {
        get
        {
            if (_mat == null)
            {
                _mat = new Material(_lightingShader);
            }

            return _mat;
        }
    }

    private void Awake()
    {
        _lightingShader = Shader.Find("Deferred/StandardLighting");
    }

    /// <summary>
    /// 延迟光照绘制
    /// </summary>
    public void Render(RenderTexture rt)
    {
        Light directionalLight = transform.GetComponent<Light>();
        if (directionalLight == null)
        {
            Debug.LogError("缺少主光源, 延迟渲染被打断");
            return;
        }

        LightingMaterial.SetVector(ShaderIDs.ID_DLightDir, -directionalLight.transform.forward);
        LightingMaterial.SetVector(ShaderIDs.ID_DLightColor, directionalLight.color * directionalLight.intensity);

        Graphics.Blit(null, rt, LightingMaterial, 0);
    }

}
