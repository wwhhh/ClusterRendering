using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public abstract class CommandBase : ScriptableObject
{

    protected abstract void Init(PipelineAssets resources);
    protected abstract void Dispose();
    public abstract bool CheckProperty();
    public virtual void FrameUpdate(Camera cam) { }
    public virtual void PreRenderFrame(Camera cam) { }
}