using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GraphicsPipeline : MonoBehaviour
{
    public GraphicsPipelineAsset asset;

    public RenderTexture target;
    public RenderTexture targetDepth;
    public RenderTexture[] targetGBuffer;

    public int[] _gBufferIDs;
    public RenderBuffer[] _gBuffers;

    void Start()
    {
        InitTargetBuffers();
        SceneController.instance.SetAsset(asset);
        SceneController.instance.LoadScene("1");
    }

    public void InitTargetBuffers()
    {
        target = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        targetDepth = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        targetGBuffer = new RenderTexture[]
        {
                new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear)
        };

        _gBufferIDs = new int[]
        {
                Shader.PropertyToID("_GBuffer0"),
                Shader.PropertyToID("_GBuffer1"),
                Shader.PropertyToID("_GBuffer2"),
                Shader.PropertyToID("_GBuffer3"),
        };

        _gBuffers = new RenderBuffer[targetGBuffer.Length];
    }

    private void Update()
    {
        SceneController.instance.Render();
    }

    private void OnPreRender()
    {
    }

    private void OnPostRender()
    {
    }

}
