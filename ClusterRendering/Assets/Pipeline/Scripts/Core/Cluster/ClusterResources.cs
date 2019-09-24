using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClusterResources : ScriptableObject
{
    public List<SceneStreaming> clusterProperties;

    public SceneStreaming GetSceneStreaming(string sceneName)
    {
        foreach (var p in clusterProperties)
        {
            if (p.name == sceneName)
            {
                return p;
            }
        }
        return null;
    }

    #region EDITOR

    [MenuItem("Assets/Resources/Common/CreateScriptObject")]
    static void CreateScriptObject()
    {
        ClusterResources res = ScriptableObject.CreateInstance<ClusterResources>();
        res.name = "SceneManager";
        res.clusterProperties = new List<SceneStreaming>();
        AssetDatabase.CreateAsset(res, "Assets/ClusterMatResources.asset");
        AssetDatabase.Refresh();
    }

    #endregion

}