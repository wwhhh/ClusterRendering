using UnityEngine;
using Framework;
using System.Collections.Generic;
using System;

public class SceneController : Singleton<SceneController>
{

    GraphicsPipelineAsset asset;

    public enum SceneMode
    {
        Add = 0,
        Only
    }

    Dictionary<string, ClusterRendering> dic = new Dictionary<string, ClusterRendering>();

    public void SetAsset(GraphicsPipelineAsset asset)
    {
        this.asset = asset;
    }

    public void LoadScene(string name, SceneMode mode = SceneMode.Only)
    {
        if (mode == SceneMode.Only) UnloadAll();

        ClusterRendering rendering = new ClusterRendering();
        rendering.Init(name, asset);
        dic.Add(name, rendering);
    }

    public void UnloadScene(string name)
    {
        if (!dic.ContainsKey(name)) return;

        ClusterRendering rendering = dic[name];
        rendering.Dispose();

        dic.Remove(name);
    }

    public void Render()
    {
        foreach (var key in dic.Keys)
        {
            ClusterRendering rendering = dic[key];
            rendering.Render();
        }
    }

    public void UnloadAll()
    {
        foreach (var key in dic.Keys)
        {
            UnloadScene(key);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnloadAll();
    }

}