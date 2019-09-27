using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICommand
{

    public abstract void Init(GraphicsPipelineAsset asset);

    public abstract void Render(Camera camera);

    public abstract void Clear();

    public abstract void Dispose();

}