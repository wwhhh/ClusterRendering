﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ClusterRendering;

public abstract class ICommand
{

    public abstract void Init(GraphicsPipelineAsset asset);

    public abstract void Render(RenderTarget rt, RenderType type = RenderType.RENDER_DEFERRED_SCENE);

    public abstract void Clear();

    public abstract void Dispose();

}