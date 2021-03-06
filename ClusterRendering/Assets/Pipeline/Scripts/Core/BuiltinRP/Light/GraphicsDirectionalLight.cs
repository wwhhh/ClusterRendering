﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    #region

    #endregion

}