using UnityEngine;
using Framework;
using System.Collections.Generic;
using System;

public class SceneController : Singleton<SceneController>
{

    GraphicsPipelineAsset asset;

    VirtualMaterialManager matManager;

    public enum SceneMode
    {
        Add = 0,
        Only
    }

    Dictionary<string, ClusterRendering> dic = new Dictionary<string, ClusterRendering>();

    public void SetAsset(GraphicsPipelineAsset asset)
    {
        this.asset = asset;
        matManager = new VirtualMaterialManager();
        matManager.Init(asset);
    }

    public void LoadScene(string name, SceneMode mode = SceneMode.Only)
    {
        if (mode == SceneMode.Only) UnloadAll();

        ClusterRendering rendering = new ClusterRendering();
        rendering.Init(name, asset);
        dic.Add(name, rendering);

        matManager.Load(name);
    }

    public void UnloadScene(string name)
    {
        if (!dic.ContainsKey(name)) return;

        ClusterRendering rendering = dic[name];
        rendering.Dispose();

        dic.Remove(name);

        matManager.Clear();
    }

    public void Render()
    {
        foreach (var key in dic.Keys)
        {
            ClusterRendering rendering = dic[key];
            rendering.Render();
        }

        matManager.UpdateFrame();
    }

    public void UnloadAll()
    {
        foreach (var key in dic.Keys)
        {
            UnloadScene(key);
        }

        matManager.Dispose();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnloadAll();
    }

}