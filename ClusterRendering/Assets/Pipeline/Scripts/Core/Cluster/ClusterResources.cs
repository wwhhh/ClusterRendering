using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ClusterProperty
{
    public string name;
    public int clusterCount;
}

public class ClusterMatResources : ScriptableObject
{
    public List<ClusterProperty> clusterProperties;
}