using UnityEngine;
using static PipelineComponent;

[System.Serializable]
public abstract class CommandBase : MonoBehaviour
{
    private bool _initialized = false;
    private bool _enabled = false;

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (value == _enabled) return;
            if (value) OnEnable();
            else OnDisable();
        }
    }

    protected abstract void Init(PipelineCommandData asset);
    protected abstract void Dispose();
    public virtual void PreRender(PipelineCommandData asset, Camera cam) { }
    public virtual void PostRender(PipelineCommandData asset, Camera cam) { }

    public void CheckInit(PipelineCommandData data)
    {
        if (_initialized) return;

        _initialized = true;
        Init(data);
    }

    private void OnEnable()
    {
        CommandManager.instance.Add(this);
    }

    private void OnDisable()
    {
        CommandManager.instance.Remove(this);
    }

}