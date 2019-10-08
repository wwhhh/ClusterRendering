using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsPipelineAsset : MonoBehaviour
{

    // 渲染参数
    public bool enableFrustumCulling;
    public bool enableShadow;

    // 场景参数
    public ClusterResources res;

    // 材质
    public Material deferredMaterial;
    public Material shadowMaterial;

    // Compute Shader
    public ComputeShader frustumCulling;

}