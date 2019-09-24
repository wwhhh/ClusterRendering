﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe struct VirtualMaterialManager
{

    private ComputeBuffer _perprotiesBuffer;
    private ComputeBuffer _texturesBuffer;

    private MaterialProperties[] _properties;
    private GraphicsPipelineAsset _asset;

    /// <summary>
    /// 初始化管理器
    /// </summary>
    public void Init(GraphicsPipelineAsset asset)
    {
        _asset = asset;
    }

    public void Load(string name)
    {
        ClusterResources res = _asset.res;
        SceneStreaming ss = res.GetSceneStreaming(name);
        VirtualMaterial vm = ss.vm;
        _properties = vm.allProperties.ToArray();

        _perprotiesBuffer = new ComputeBuffer(_properties.Length, sizeof(MaterialProperties));
        _perprotiesBuffer.SetData(_properties);
    }

    public void UpdateFrame()
    {
        Material mat = _asset.defaultMaterial;
        mat.SetBuffer(ShaderIDs.ID_MaterialProperties, _perprotiesBuffer);
    }

    /// <summary>
    /// 清除当前加载的材质
    /// </summary>
    public void Clear()
    {
        _properties = null;
    }

    /// <summary>
    /// 释放管理器的所有资源
    /// </summary>
    public void Dispose()
    {

    }

}