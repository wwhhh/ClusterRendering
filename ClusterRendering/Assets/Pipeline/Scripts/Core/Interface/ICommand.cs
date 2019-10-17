using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClusterRendering;

public abstract class ICommand
{

    public abstract void Init(GraphicsPipelineAsset asset);

    public abstract void Render(RenderTarget rt);

    public abstract void Clear();

    public abstract void Dispose();

}