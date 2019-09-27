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
    Dictionary<string, ClusterRendering> dicShadow = new Dictionary<string, ClusterRendering>();

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

        ClusterRendering renderer = new ClusterRendering(ClusterRendering.RenderType.RENDER_DEFERRED_SCENE);
        renderer.Init(asset);
        renderer.LoadScene(name, clusterCount);
        dicRenderer.Add(name, renderer);

        ClusterRendering shadow = new ClusterRendering(ClusterRendering.RenderType.RENDER_SHADOW);
        shadow.Init(asset);
        shadow.LoadScene(name, clusterCount);
        dicShadow.Add(name, shadow);
    }

    public void UnloadScene(Dictionary<string, ClusterRendering> dic, string name)
    {
        if (!dic.ContainsKey(name)) return;
        matManager.Clear();

        ClusterRendering rendering = dic[name];
        rendering.Dispose();
        dic.Remove(name);
        dicShadow.Remove(name);
    }

    public void Render(Camera camera)
    {
        frustumCulling.Render(camera);
        matManager.Render(camera);

        foreach (var key in dicRenderer.Keys)
        {
            ClusterRendering rendering = dicRenderer[key];
            rendering.Render(camera);
        }
    }

    public void RenderShadow(Camera shadowCamera)
    {
        foreach (var key in dicShadow.Keys)
        {
            ClusterRendering shadow = dicShadow[key];
            shadow.Render(shadowCamera);
        }
    }

    public void UnloadAll()
    {
        foreach (var key in dicRenderer.Keys)
        {
            UnloadScene(dicRenderer, key);
        }

        foreach (var key in dicShadow.Keys)
        {
            UnloadScene(dicShadow, key);
        }
        matManager.Dispose();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnloadAll();
    }

}