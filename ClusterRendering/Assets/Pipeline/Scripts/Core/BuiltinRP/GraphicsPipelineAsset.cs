using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsPipelineAsset : MonoBehaviour
{
    public bool enableFrustumCulling;
    public Material defaultMaterial;
    public ComputeShader frustumCulling;
    public ClusterResources res;
}