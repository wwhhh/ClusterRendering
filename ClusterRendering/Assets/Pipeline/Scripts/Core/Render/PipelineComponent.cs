using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// pipeline 绘制时所用到的一些结构的声明
/// </summary>
public class PipelineComponent
{

    public struct PipelineBuffers
    {
        public ComputeBuffer argsBuffer;    // 绘制参数
        public ComputeBuffer pointsBuffer;  // 顶点信息
        public ComputeBuffer resultBuffer;   // 剔除结果
        public ComputeBuffer clusterBuffer;  // 簇信息
    }

    public struct PipelineTargets
    {
        public RenderTexture target;
        public RenderTexture targetDepth;
        public RenderTexture[] targetGBuffer;

        public int[] _gBufferIDs;
        public RenderBuffer[] _gBuffers;

        public void Init()
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
    }

    public struct PipelineCommandData
    {
        public ScriptableRenderContext context;
        public PipelineAssets asset;
        public CommandBuffer command;
    }
}