using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClusterResourcesEditor
{

    [MenuItem("Assets/Cluster/CreateScriptObject")]
    static void CreateScriptObject()
    {
        ClusterMatResources res = ScriptableObject.CreateInstance<ClusterMatResources>();
        res.name = "SceneManager";
        res.clusterProperties = new List<ClusterProperty>();
        AssetDatabase.CreateAsset(res, "Assets/ClusterMatResources.asset");
    }

}