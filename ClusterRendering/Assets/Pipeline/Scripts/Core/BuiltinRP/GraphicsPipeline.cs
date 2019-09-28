using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphicsPipeline : MonoBehaviour
{
    GraphicsPipelineAsset asset;

    private void Awake()
    {
        asset = GetComponent<GraphicsPipelineAsset>();

        SceneController.instance.SetAsset(asset);
    }

    public void ScenePreRender(Camera cam)
    {

    }

    public void ScenePostRender(Camera cam)
    {
        
    }

    public void InitTargetBuffers()
    {
    }

}