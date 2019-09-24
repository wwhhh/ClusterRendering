using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VirtualMaterialManager
{

    private ComputeBuffer perprotiesBuffer;
    private ComputeBuffer texturesBuffer;

    /// <summary>
    /// 初始化管理器
    /// </summary>
    public void Init() { }

    /// <summary>
    /// 清除当前加载的材质
    /// </summary>
    public void Clear() { }

    /// <summary>
    /// 释放管理器的所有资源
    /// </summary>
    public void Dispose() { }

}