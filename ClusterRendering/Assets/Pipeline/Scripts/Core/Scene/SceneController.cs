using UnityEngine;
using Framework;
using System.Collections.Generic;
using System;

public class SceneController : Singleton<SceneController>
{

    GraphicsPipelineAsset asset;

    VirtualMaterialManager matManager;
    ClusterFrustumCulling frustumCulling;

    public enum SceneMode
    {
        Add = 0,
        Only
    }

    Dictionary<string, ClusterRendering> dicRenderer = new Dictionary<string, ClusterRendering>();

    public void SetAsset(GraphicsPipelineAsset asset)
    {
        this.asset = asset;
        matManager = new VirtualMaterialManager();
        matManager.Init(asset);

        frustumCulling = new ClusterFrustumCulling();
        frustumCulling.Init(asset);
    }

    public void LoadScene(string name, SceneMode mode = SceneMode.Only)
    {
        if (mode == SceneMode.Only) UnloadAll();

        int clusterCount =  asset.res.GetClusterCount(name);

        frustumCulling.LoadScene(name, clusterCount);
        matManager.LoadScene(name);

        ClusterRendering renderer = new ClusterRendering();
        renderer.Init(asset);
        renderer.LoadScene(name, clusterCount);
        dicRenderer.Add(name, renderer);
    }

    public void UnloadScene(Dictionary<string, ClusterRendering> dic, string name)
    {
        if (!dic.ContainsKey(name)) return;
        matManager.Clear();

        ClusterRendering rendering = dic[name];
        rendering.Dispose();
        dic.Remove(name);
    }

    public void Render(RenderTarget rt)
    {
        frustumCulling.Render(rt, ClusterRendering.RenderType.RENDER_DEFERRED_SCENE);
        matManager.Render(rt);

        foreach (var key in dicRenderer.Keys)
        {
            ClusterRendering rendering = dicRenderer[key];
            rendering.Render(rt, ClusterRendering.RenderType.RENDER_DEFERRED_SCENE);
        }
    }

    public void RenderShadow(RenderTarget rt)
    {
        foreach (var key in dicRenderer.Keys)
        {
            ClusterRendering shadow = dicRenderer[key];
            shadow.Render(rt, ClusterRendering.RenderType.RENDER_SHADOW);
        }
    }

    public void UnloadAll()
    {
        foreach (var key in dicRenderer.Keys)
        {
            UnloadScene(dicRenderer, key);
        }

        matManager.Dispose();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnloadAll();
    }

}