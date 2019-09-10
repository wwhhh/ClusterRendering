using UnityEngine;

/// <summary>
/// pipeline 绘制时所用到的一些结构的声明
/// </summary>
public class PipelineComponent
{

    public struct PipelineTargets
    {
        public RenderTexture renderTarget;
        public RenderTexture[] gBufferTextures;
    }

    public struct PipelineBuffers
    {
        public ComputeBuffer argsBuffer;    // 绘制参数
        public ComputeBuffer pointsBuffer;  // 顶点信息
        public ComputeBuffer resultBuffer;   // 剔除结果
        public ComputeBuffer clusterBuffer;  // 簇信息
    }
}